#!/usr/bin/env dotnet
#:package System.CommandLine@2.0.0

using System.CommandLine;
using System.IO;
using System.Linq;

var inputOption = new Option<FileInfo>(name: "input")
{
    Description = "The file containing the puzzle inputs",
    Arity = ArgumentArity.ExactlyOne,
    DefaultValueFactory = _ => new FileInfo("./day03/input")
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

    var input = await File.ReadAllLinesAsync(inputFile.FullName, ct);

    var solution1 = CheckBanks(input, 2)
        .Select(i => new string(i))
        .Sum(i => int.Parse(i));
    
    var solution2 = CheckBanks(input, 12)
        .Select(i => new string(i))
        .Sum(i => long.Parse(i));

    Console.WriteLine($"Solution 1: {solution1}");
    Console.WriteLine($"Solution 2: {solution2}");

    return 0;

    IEnumerable<char[]> CheckBanks(IEnumerable<string> banks, int needed)
    {
        foreach(var bank in banks)
        {
            yield return FindNextBattery(bank.AsSpan(), needed);
        }
    }

    char[] FindNextBattery(ReadOnlySpan<char> input, int remainingNeeded)
    {
        if (remainingNeeded <= 0) { return []; }
        char highest = '0';

        int indexOfHighest = 0;
        for (var i = 0; i <= input.Length - remainingNeeded; i++)
        {
            var current = input[i];

            if (current <= highest) { continue; }

            highest = current;
            indexOfHighest = i;
            if (highest == '9') { break; }
        }

        return [highest, .. FindNextBattery(input[++indexOfHighest..], remainingNeeded - 1)];
    }
});


var parseResult = rootCommand.Parse(args);
return await parseResult.InvokeAsync();

record RepeatResult(long Value, string RepeatingSequence, int Occurences);