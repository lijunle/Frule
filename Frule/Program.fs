module Program

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
    let userResult = User.construct (email, password)

    let inboxFolder = time (fun () ->
        Result.result {
            let! user = userResult
            let! inboxFolder = User.getInboxFolder user
            return inboxFolder
        })
    printfn "%A" inboxFolder

    let rules = time (fun () ->
        Result.result {
            let! user = userResult
            let! rules = User.getRules user
            return rules
        })
    printfn "%A" rules

    0
