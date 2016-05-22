namespace ViewModels

open FSharp.ViewModule
open Folder
open Rule
open Service

type MainWindowViewModel() =
    inherit ViewModelBase()

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

    member this.InboxFolder with get() = [inboxFolder]
    member this.LoginCommand = this.Factory.CommandSync(fun _ -> Views.LoginDialog().ShowDialog() |> ignore)
