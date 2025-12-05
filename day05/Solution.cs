#!/usr/bin/env dotnet
#:package System.CommandLine@2.0.0

using System.CommandLine;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;


var inputOption = new Option<FileInfo>(name: "input")
{
    Description = "The file containing the puzzle inputs",
    Arity = ArgumentArity.ExactlyOne,
    DefaultValueFactory = _ => new FileInfo("./day05/input")
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

    using var reader = inputFile.OpenText();

    string? currentLine = "";

    List<Range> freshIngredients = await Consolidate(ExtractFreshRanges()).ToListAsync(cancellationToken: ct);
    HashSet<long> ingredientIds = await ExtractIngredientIds().ToHashSetAsync(cancellationToken: ct);

    var solution1 = (from f in freshIngredients
                    from i in ingredientIds
                    where f.Begin <= i && i <= f.End
                    select i)
                    .ToHashSet()
                    .Count;

    var solution2 = freshIngredients.Select(i => i.End - i.Begin + 1).Sum();


    async IAsyncEnumerable<Range> ExtractFreshRanges()
    {
        while((currentLine = await reader.ReadLineAsync(ct)) != null)
        {
            var indexOfDash = currentLine.IndexOf('-');
            if (indexOfDash == -1) { break;}
            var start = currentLine[..indexOfDash];
            var end = currentLine[(indexOfDash + 1)..];

            if (!long.TryParse(start, out var startNumber) || !long.TryParse(end, out var endNumber))
            {
                throw new ApplicationException($"Failed to parse start/end codes for value: {currentLine}");
            }
            
            yield return new(startNumber, endNumber);
        }
    }

    async IAsyncEnumerable<long> ExtractIngredientIds()
    {
        while((currentLine = await reader.ReadLineAsync(ct)) != null)
        {
            if (!long.TryParse(currentLine, out var ingredientId))
            {
                throw new ApplicationException($"Failed to parse start/end codes for value: {currentLine}");
            }
            
            yield return ingredientId;
        }
    }

    Console.WriteLine($"Solution 1: {solution1}");
    Console.WriteLine($"Solution 2: {solution2}");

    return 0;
});

var parseResult = rootCommand.Parse(args);
return await parseResult.InvokeAsync();

async IAsyncEnumerable<Range> Consolidate(IAsyncEnumerable<Range> ranges)
{
    var ordered = ranges.OrderBy(r => r.Begin);

    var enumerator = ordered.GetAsyncEnumerator();
    await enumerator.MoveNextAsync();

    Range current = enumerator.Current;
    await enumerator.MoveNextAsync();

    Range next = enumerator.Current;

    while (true)
    {
        if (next == default)
        {
            yield return current;
            break;
        }

        if (current.End >= next.Begin)
        {
            current = new(current.Begin, Math.Max(current.End, next.End));
        }
        else
        {
            yield return current;
            current = next;
        }

        await enumerator.MoveNextAsync();
        next = enumerator.Current;
    }
}
record struct Range(long Begin, long End);