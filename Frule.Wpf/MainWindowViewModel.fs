﻿namespace ViewModels

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

    let loadDataAsync user = async {
        do! Async.SwitchToThreadPool () // TODO Make native async operators and avoid this
        loadData user
    }

    let login _ =
        Views.LoginDialog().ShowDialog() |> ignore
        User.get () |> loadDataAsync

    do Async.Start (User.get () |> loadDataAsync)

    member this.InboxFolder with get() = [inboxFolder.Value]
    member this.LoginCommand = this.Factory.CommandAsync(login)
