open System.IO
open System
let filePath = "./input"

type Range = { 
    Start: int64; 
    End: int64;
}
with
    member this.Values() = 
        seq { this.Start..this.End }

let GetRange (input: string) = 
    let dashIndex = input.IndexOf('-')
    let startRange = input[..(dashIndex - 1)] |> int64
    let endRange = input[(dashIndex + 1)..] |> int64
    {
        Start = startRange
        End = endRange
    }

let ChunkString (input: string) (size: int) = 
    match size with 
    | value when value > 0 -> input |> Seq.chunkBySize size |> Seq.map (fun x -> new string(x))
    | _ -> Seq.empty

type SequenceResult = { Value: int64; Sequence: string; RepeatCount: int;}

let rec FindSequences (input) (size) = 
    let chunks = ChunkString input size

    match Seq.tryHead chunks, size with
    | Some sequence, _ when Seq.forall (fun x -> x = sequence) chunks -> Some { Value = input |> int64; Sequence = sequence; RepeatCount = Seq.length chunks }
    | _, currentLength when currentLength > 1 -> FindSequences input (currentLength - 1)
    | _, _ -> None

let solution1, solution2 = 
    File.ReadLines(filePath)
    |> Seq.collect (fun x -> x.Split ',')
    |> Seq.map GetRange
    |> Seq.collect (fun x -> x.Values())
    |> Seq.map (fun x -> x.ToString())
    |> Seq.map (fun x -> FindSequences x (x.Length / 2))
    |> Seq.choose id
    |> Seq.fold (fun (sumOfPairs, sumOfAllSequences) element -> 
        let pairValue = 
            match element.RepeatCount with
            | 2 -> element.Value
            | _ -> 0
        sumOfPairs + pairValue, sumOfAllSequences + element.Value
    ) (0L,0L)

printfn "Solution 1: %d" solution1
printfn "Solution 2: %d" solution2
