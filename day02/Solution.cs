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
    
    var sumOfbadProductCodes = input
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
        .Where(x => x.Length % 2 == 0)
        .Select(x =>
        {
            var midPoint = x.Length / 2;
            var firstHalf = x[..midPoint];
            var secondHalf = x[midPoint..];
            return (FullValue: x, FirstHalf: firstHalf, SecondHalf: secondHalf);
        })
        .Where(x => x.FirstHalf == x.SecondHalf)
        .Select(x => long.Parse(x.FullValue))
        .Sum();

    Console.WriteLine($"Solution 1: {sumOfbadProductCodes}");


    



    return 0;

    IEnumerable<string> ExtractCodesFromRange(long start, long end)
    {
        for (var code = start; code <= end; code++)
        {
            yield return code.ToString();
        }
    }


});

var parseResult = rootCommand.Parse(args);
return await parseResult.InvokeAsync();