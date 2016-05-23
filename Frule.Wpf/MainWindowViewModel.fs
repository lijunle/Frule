namespace ViewModels

open FSharp.ViewModule

type MainWindowViewModel() as this =
    inherit ViewModelBase()

    let loadingFolder = { Id = null; Name= "Loading"; Children= []; Rules= [] }
    let inboxFolder = this.Factory.Backing(<@ this.InboxFolder @>, loadingFolder)

    let loadData user =
        let resultFolder = User.getInboxFolder user

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
