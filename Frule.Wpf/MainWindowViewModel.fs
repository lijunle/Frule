namespace ViewModels

open FSharp.ViewModule

type MainWindowViewModel() as this =
    inherit ViewModelBase()

    let loadingFolder = { Id = null; Name= "Loading"; Children= []; }
    let loginErrorFolder = { Id = null; Name= "Login Error"; Children= []; }
    let inboxFolder = this.Factory.Backing(<@ this.InboxFolder @>, loadingFolder)
    let selectedFolder = this.Factory.Backing(<@ this.SelectedFolder @>, loginErrorFolder)
    let rules = this.Factory.Backing(<@ this.InternalRules @>, [])

    let loadDataAsync user = async {
        do! Async.SwitchToThreadPool () // TODO Make native async operators and avoid this

        let folderResult = User.getInboxFolder user
        inboxFolder.Value <- Result.orDefault folderResult loginErrorFolder

        let rulesResult = User.getRules user
        rules.Value <- Result.orDefault rulesResult []
    }

    let login _ =
        Views.LoginDialog().ShowDialog() |> ignore
        User.get () |> loadDataAsync

    do
        Async.Start (User.get () |> loadDataAsync)

        this.DependencyTracker.AddPropertyDependencies(
            <@ this.Rules @>,
            [ <@@ this.SelectedFolder @@>; <@@ this.InternalRules @@> ])

    member private this.SelectedFolder with get() = selectedFolder.Value
    member private this.InternalRules with get() = rules.Value

    member this.InboxFolder with get() = [inboxFolder.Value]
    member this.Rules with get() = rules.Value |> List.filter (fun r -> r.FolderId = selectedFolder.Value.Id)
    member this.LoginCommand = this.Factory.CommandAsync(login)
    member this.SelectFolderCommand = this.Factory.CommandSyncParam(fun v -> selectedFolder.Value <- v)
