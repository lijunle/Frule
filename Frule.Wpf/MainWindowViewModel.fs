namespace ViewModels

open Service
open Rule
open Folder

type MainWindowViewModel() =
    let email = ""
    let password = ""

    let resultFolder =
        Result.result {
            let! service = getService email password
            let! rules = getRules service |> Result.map List.ofSeq
            let! result = getFolderHierarchy service rules
            return result
        }

    let inboxFolder =
        match resultFolder with
        | Success f -> f
        | Failure e -> { Id = null; Name= "Inbox"; Children= []; Rules= [] }

    member x.InboxFolder with get() = [inboxFolder]
