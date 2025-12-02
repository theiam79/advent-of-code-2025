#!/usr/bin/env dotnet
#:package System.CommandLine@2.0.0

using System.CommandLine;
using System.IO;

var inputOption = new Option<FileInfo>(name: "input")
{
    Description = "The file containing the puzzle inputs",
    Arity = ArgumentArity.ExactlyOne,
    DefaultValueFactory = _ => new FileInfo("./day01/input")
};

inputOption.AcceptExistingOnly();

var rootCommand = new RootCommand("2025 Day 1 AoC solver in C#")
{
    inputOption
};

rootCommand.SetAction((parseResult, ct) =>
{
    if (parseResult.GetValue(inputOption) is not FileInfo inputFile)
    {
        return Task.FromResult(-1);
    }

    int startingLocation, currentLocation;
    startingLocation = currentLocation = 50;

    string? currentLine;

    int stopOnZeroCount = 0;
    int crossZeroCount = 0;

    using var inputStream = inputFile.OpenText();

    while((currentLine = inputStream.ReadLine()) != null)
    {
        var sign = currentLine[0] switch
        {
            'R' => 1,
            'L' => -1,
            _ => throw new NotImplementedException("Unhandled input direction")
        };

        if (!int.TryParse(currentLine[1..], out var turnAmount))
        {
            throw new ApplicationException("Unable to parse input turn amount");
        }

        var fullRevolutions = turnAmount / 100;

        crossZeroCount += fullRevolutions;

        var remainingMove = turnAmount % 100 * sign;
        var relativePosition = currentLocation + remainingMove;

        if ((relativePosition < 0 && currentLocation != 0) || relativePosition > 100)
        {
            crossZeroCount++;
        }

        var normalizedPosition = (relativePosition + 100) % 100;

        if (normalizedPosition == 0)
        {
            stopOnZeroCount++;
        }

        currentLocation = normalizedPosition;
    }

    Console.WriteLine($"Solution 1: {stopOnZeroCount}");
    Console.WriteLine($"Solution 2: {stopOnZeroCount + crossZeroCount}");
    return Task.FromResult(0);
});

var parseResult = rootCommand.Parse(args);
return await parseResult.InvokeAsync();