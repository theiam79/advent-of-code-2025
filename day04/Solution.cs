#!/usr/bin/env dotnet
#:package System.CommandLine@2.0.0

using System.CommandLine;
using System.Drawing;
using System.IO;
using System.Linq;


var inputOption = new Option<FileInfo>(name: "input")
{
    Description = "The file containing the puzzle inputs",
    Arity = ArgumentArity.ExactlyOne,
    DefaultValueFactory = _ => new FileInfo("./day04/input")
};

inputOption.AcceptExistingOnly();

var rootCommand = new RootCommand("2025 Day 2 AoC solver in C#")
{
    inputOption
};

Point[] _offsets = [new (-1,-1), new(-1,0), new(-1,1), new(0,-1), new(0,+1), new(1,-1), new(1,0), new(1,1)];

rootCommand.SetAction(async Task<int>(parseResult, ct) =>
{
    if (parseResult.GetValue(inputOption) is not FileInfo inputFile)
    {
        return -1;
    }

    var points = new HashSet<Point>();
    var  rawInput = await File.ReadAllLinesAsync(inputFile.FullName, ct);

    int rowIt = 0;
    foreach (var line in rawInput)
    {
        int colIt = 0;
        foreach (var ch in line)
        {
            if (ch == '@') { points.Add(new(rowIt, colIt)); }
            colIt++;
        }
        rowIt++;
    }

    var solution1 = RemoveAccessible([.. points], false);
    var solution2 = RemoveAccessible([.. points], true);

    Console.WriteLine($"Solution 1: {solution1}");
    Console.WriteLine($"Solution 2: {solution2}");

    return 0;

    int RemoveAccessible(HashSet<Point> grid, bool cascade)
    {
        int found = 0;

        foreach (var p in points) {
            HashSet<Point> neighbors = [.. _offsets.Select(o => o + p)];

            if (points.Intersect(neighbors).Take(4).Count() < 4) {
                found++;
                if (cascade) { points.Remove(p); }
            }
        }

        var cascaded = cascade switch
        {
            true when found > 0 => RemoveAccessible(grid, cascade),
            _ => 0
        };

        return found + cascaded;
    }
});

var parseResult = rootCommand.Parse(args);
return await parseResult.InvokeAsync();

record struct Point(int Row, int Column)
{
    public static Point operator +(Point p1, Point p2) => new(p1.Row + p2.Row, p1.Column + p2.Column);
}