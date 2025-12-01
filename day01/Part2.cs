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

    using var inputStream = inputFile.OpenText();

    var startingLocation = 50;
    var currentLocation = startingLocation;
    var currentLine = "";

    var zeroCount = 0;

    var index = 0;

    while((currentLine = inputStream.ReadLine()) != null)
    {
        index++;

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
        var revolutionRemainder = turnAmount % 100;

        var actualChange = revolutionRemainder * sign;

        var relativePosition = currentLocation + actualChange;

        var crossedZeroDuringRemainder = (relativePosition < 0 && currentLocation != 0) || relativePosition > 100;

        var totalTimesCrossingZero = fullRevolutions + (crossedZeroDuringRemainder ? 1 : 0);

        var normalizedPosition = (relativePosition + 100) % 100;

        if (normalizedPosition == 0)
        {
            zeroCount++;
        }

        zeroCount += totalTimesCrossingZero;

        // Console.WriteLine($"{currentLocation} => {currentLine} | {actualChange} (+{fullRevolutions} Revs) | {relativePosition} ({normalizedPosition}) | {totalTimesCrossingZero} ({zeroCount})");
        currentLocation = normalizedPosition;
    }

    Console.WriteLine($"The password is {zeroCount}");
    return Task.FromResult(0);
});

var parseResult = rootCommand.Parse(args);
return await parseResult.InvokeAsync();