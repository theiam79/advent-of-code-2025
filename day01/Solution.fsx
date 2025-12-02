open System.IO
open System
let filePath = "./input"
let startPos = 50
let parseMove (move: string) =
    let sign = 
        match move[0] with
        | 'R' -> 1
        | 'L' -> -1
        | _ -> raise (NotImplementedException("Unable to extract move direction"))

    let amount = 
        move[1..] 
        |> int

    sign * amount

type MoveResult = { EndPosition: int; StoppedOnZero: bool; PassedZeroCount: int }

let applyMove (move: int, currentPos: int) =
    let fullRevolutions = abs move / 100
    let remainder = move % 100
    let newRelativePosition = currentPos + remainder
    let newExactPosition = (newRelativePosition + 100) % 100
    let zeroPassesDuringRemainder = 
        match newRelativePosition with
        | value when value > 100 -> 1
        | value when value < 0 && currentPos <> 0 -> 1
        | _ -> 0
    let endedOnZero = 
        newExactPosition = 0
    
    {
        EndPosition = newExactPosition
        StoppedOnZero = endedOnZero
        PassedZeroCount = fullRevolutions + zeroPassesDuringRemainder
    }

let passwordResult = 
    File.ReadLines(filePath)
    |> Seq.map parseMove
    |> Seq.fold (fun (currentPosition, stoppedOnZero, passedZero) element ->
        let moveResult = applyMove(element, currentPosition)
        let stoppedOnZeroCount = 
            if moveResult.StoppedOnZero then stoppedOnZero + 1 else stoppedOnZero
        let passedZeroCount = passedZero + moveResult.PassedZeroCount
        (moveResult.EndPosition, stoppedOnZeroCount, passedZeroCount)
    ) (startPos, 0, 0)

let _, stoppedOnZero, passedZero = passwordResult

printfn "Solution 1: %d" stoppedOnZero
printfn "Solution 2: %d" (stoppedOnZero + passedZero)
