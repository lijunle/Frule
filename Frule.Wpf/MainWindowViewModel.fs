namespace ViewModels

open FSharp.ViewModule

type RuleViewModel(update, rule : Rule) as this =
    inherit ViewModelBase()

    let mutable model = rule
    let name = this.Factory.Backing(<@ this.Name @>, rule.Name)

    member this.Update newRule =
        model <- newRule
        name.Value <- newRule.Name
        this

    member this.Model with get() = model
    member this.Id = rule.Id
    member this.Name with get() = name.Value and set v = update (Rule.updateName v rule)
    member this.FolderId = rule.FolderId
    member this.FromAddresses = rule.FromAddresses
    member this.SentToAddresses = rule.SentToAddresses

type MainWindowViewModel() as this =
    inherit ViewModelBase()

    let loadingFolder = { Id = null; Name= "Loading"; Children= []; }
    let loginErrorFolder = { Id = null; Name= "Login Error"; Children= []; }
    let inboxFolder = this.Factory.Backing(<@ this.InboxFolder @>, loadingFolder)
    let selectedFolder = this.Factory.Backing(<@ this.SelectedFolder @>, loginErrorFolder)
    let rules = this.Factory.Backing(<@ this.InternalRules @>, [])

    let updateRule (rule : Rule) =
        let update (r : RuleViewModel) = if r.Id = rule.Id then r.Update rule else r
        let newRules = rules.Value |> List.map update
        rules.Value <- newRules

    let loadDataAsync user = async {
        do! Async.SwitchToThreadPool () // TODO Make native async operators and avoid this

        let folderResult = User.getInboxFolder user
        inboxFolder.Value <- Result.orDefault folderResult loginErrorFolder

        let rulesResult = User.getRules user
        rules.Value <- Result.orDefault rulesResult [] |> List.map (fun r -> RuleViewModel(updateRule, r))
    }

    let login _ =
        Views.LoginDialog().ShowDialog() |> ignore
        User.get () |> loadDataAsync

    let save _ = async {
        do! Async.SwitchToThreadPool () // TODO Make native async operators and avoid this

        let user = User.get ()
        let rules = rules.Value |> List.map (fun r -> r.Model)
        Rule.saveToServer user rules |> ignore
    }

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
    member this.SaveCommand = this.Factory.CommandAsync(save)
    member this.SelectFolderCommand = this.Factory.CommandSyncParam(fun v -> selectedFolder.Value <- v)
