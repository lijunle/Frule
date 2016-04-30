module Program

open Folder
open Rule
open ServiceUrl
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
    let serviceUrl = time (fun () -> getServiceUrl email password)
    let (>>=) v f = Option.bind f v
    let rules =
        serviceUrl >>= (fun serviceUrl ->
        getRules email password serviceUrl |> Some)
    printfn "%A" rules
    let folders =
        serviceUrl >>= (fun serviceUrl ->
        getFolderHierarchy email password serviceUrl |> Some)
    printfn "%A" folders
    0
