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

        var actualChange = turnAmount * sign;
        var reducedChange = actualChange % 100;

        var previousLocation = currentLocation;
        var relativePosition = currentLocation + reducedChange;
        var normalized = (relativePosition + 100) % 100;

        if (normalized == 0)
        {
            zeroCount++;
        }
        //Console.WriteLine($"{currentLocation} => {currentLine} | {actualChange} ({reducedChange}) | {relativePosition} ({normalized})");
        currentLocation = normalized;
    }

    Console.WriteLine($"The password is {zeroCount}");
    return Task.FromResult(0);
});

var parseResult = rootCommand.Parse(args);
return await parseResult.InvokeAsync();