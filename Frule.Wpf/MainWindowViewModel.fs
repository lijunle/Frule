namespace ViewModels

open FSharp.ViewModule
open Folder
open Rule
open Service

type MainWindowViewModel() as this =
    inherit ViewModelBase()

    let loadingFolder = { Id = null; Name= "Loading"; Children= []; Rules= [] }
    let inboxFolder = this.Factory.Backing(<@ this.InboxFolder @>, loadingFolder)

    let loadData (email, password) =
        let resultFolder =
            Result.result {
                let! service = getService email password
                let! rules = getRules service |> Result.map List.ofSeq
                let! result = getFolderHierarchy service rules
                return result
            }

        inboxFolder.Value <-
            match resultFolder with
            | Success f -> f
            | Failure _ -> { Id = null; Name= "Login Error"; Children= []; Rules= [] }

    let login () =
        Views.LoginDialog().ShowDialog() |> ignore
        User.get () |> loadData

    do
        User.get () |> loadData

    member this.InboxFolder with get() = [inboxFolder.Value]
    member this.LoginCommand = this.Factory.CommandSync(login)
