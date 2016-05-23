module Program

open Folder
open Rule
open Service
open System
open System.Diagnostics

let time fn =
    let watcher = Stopwatch.StartNew()
    let hashCode = watcher.GetHashCode()
    printfn "[%d] Start at %A" hashCode DateTime.Now
    let result = fn ()
    printfn "[%d] End at %A, Elapsed: %A" hashCode DateTime.Now watcher.Elapsed
    result

[<EntryPoint>]
let main argv =
    let email = argv.[0]
    let password = argv.[1]
    let getFolders () =
        Result.result {
            let! user = User.construct (email, password)
            let service = getService user
            let! rules = getRules service |> Result.map List.ofSeq
            let! result = getFolderHierarchy service rules
            return result
        }
    let folders = time getFolders
    printfn "%A" folders
    0
