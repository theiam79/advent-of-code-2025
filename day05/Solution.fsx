open System.IO
open System
let filePath = "./input"

type Range = { 
    Start: int64; 
    End: int64;
}

let GetRange (input: string) = 
    let dashIndex = input.IndexOf('-')
    let startRange = input[..(dashIndex - 1)] |> int64
    let endRange = input[(dashIndex + 1)..] |> int64
    {
        Start = startRange
        End = endRange
    }


let rec Consolidate (currentRange: Range) (remainingRanges: List<Range>) = 
    seq {
            match remainingRanges with
            | [] -> yield currentRange
            | head::tail -> 
                match currentRange.End >= head.Start with
                | true ->
                    let combined = {
                        Start = currentRange.Start
                        End = Math.Max(currentRange.End, head.End)
                    }
                    yield! Consolidate combined tail
                | _ ->
                    yield currentRange
                    yield! Consolidate head tail
    }

let (rawRanges, ingredients) = 
    File.ReadAllLines(filePath)
    |> Array.filter (fun s -> not (String.IsNullOrWhiteSpace s))
    |> Array.partition (fun s -> s.Contains "-")

let processRanges input = 
    match input with
    | head::tail -> Consolidate head tail
    | _ -> []

let consolidatedRanges = 
    rawRanges
    |> Seq.map GetRange
    |> Seq.sortBy (fun r -> r.Start)
    |> Seq.toList
    |> processRanges

let ingredientIds = 
    ingredients
    |> Seq.map (fun x -> x |> int64)
    |> Seq.distinct
    |> Seq.toList

let freshIngredients = 
    ingredientIds
    |> Seq.filter (fun i ->
        consolidatedRanges
        |> Seq.exists (fun f -> f.Start <= i && i <= f.End)
    )

    // query {
    //     for r in consolidatedRanges do
    //     for i in ingredientIds do
    //     where (r.Start <= i && i <= r.End)
    //     select i
    // } 
    |> Seq.distinct
    |> Seq.length

let totalIngredients = 
    consolidatedRanges
    |> Seq.map (fun r -> r.End - r.Start + 1L)
    |> Seq.sum

printfn "Solution 1: %d" freshIngredients
printfn "Solution 2: %d" totalIngredients
