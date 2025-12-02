#!/usr/bin/env dotnet
#:package System.CommandLine@2.0.0

using System.CommandLine;
using System.IO;
using System.Linq;

var inputOption = new Option<FileInfo>(name: "input")
{
    Description = "The file containing the puzzle inputs",
    Arity = ArgumentArity.ExactlyOne,
    DefaultValueFactory = _ => new FileInfo("./day02/input")
};

inputOption.AcceptExistingOnly();

var rootCommand = new RootCommand("2025 Day 2 AoC solver in C#")
{
    inputOption
};

rootCommand.SetAction(async Task<int>(parseResult, ct) =>
{
    if (parseResult.GetValue(inputOption) is not FileInfo inputFile)
    {
        return -1;
    }

    var input = await File.ReadAllTextAsync(inputFile.FullName, ct);
    
    var badProductCodes = input
        .Split(',')
        .Select(x =>
        {
            var indexOfDash = x.IndexOf('-');
            var start = x[..indexOfDash];
            var end = x[(indexOfDash + 1)..];

            if (!long.TryParse(start, out var startNumber) || !long.TryParse(end, out var endNumber))
            {
                throw new ApplicationException($"Failed to parse start/end codes for value: {x}");
            }

            return (Start: startNumber, End: endNumber);
        })
        .SelectMany(x => ExtractCodesFromRange(x.Start, x.End))
        .Select(CheckRepeating)
        .OfType<RepeatResult>();

    var solution1= badProductCodes
        .Where(x => x.Occurences == 2)
        .Select(x => x.Value)
        .Sum();
    
    var solution2= badProductCodes
        .Select(x => x.Value)
        .Sum();
        

    Console.WriteLine($"Solution 1: {solution1}");
    Console.WriteLine($"Solution 1: {solution2}");

    return 0;

    IEnumerable<string> ExtractCodesFromRange(long start, long end)
    {
        for (var code = start; code <= end; code++)
        {
            yield return code.ToString();
        }
    }

    RepeatResult? CheckRepeating(string input)
    {
        for (var i = input.Length / 2; i >= 1 ; i--)
        {
            var sub = input[..i];

            var chunks = input.Chunk(sub.Length);

            if (chunks.Skip(1).Any(c => string.Concat(c) != sub))
            {
                continue;
            }

            return new RepeatResult(long.Parse(input), sub, chunks.Count());
        }
        
        return null;
    }
});


var parseResult = rootCommand.Parse(args);
return await parseResult.InvokeAsync();

record RepeatResult(long Value, string RepeatingSequence, int Occurences);