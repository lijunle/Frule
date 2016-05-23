namespace ViewModels

open FSharp.ViewModule

type MainWindowViewModel() as this =
    inherit ViewModelBase()

    let loadingFolder = { Id = null; Name= "Loading"; Children= []; Rules= [] }
    let inboxFolder = this.Factory.Backing(<@ this.InboxFolder @>, loadingFolder)

    let loadData (email, password) =
        let resultFolder =
            Result.result {
                let! user = User.construct (email, password)
                let! inboxFolder = User.getInboxFolder user
                return inboxFolder
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
