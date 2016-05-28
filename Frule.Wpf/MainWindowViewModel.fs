namespace ViewModels

open FSharp.ViewModule

type DisplayRuleState = {
    RuleStore: RuleStore;
    SelectedFolder: Folder;
}

type DisplayRuleChangeType =
    | RuleStoreUpdated of RuleStore
    | SelectedFolderChagned of Folder

module DisplayRule =
    let loadingFolder = { Id = null; Name= "Loading"; Children= []; }
    let loadingRuleStore = { Rules = []; }
    let loadingState = { RuleStore = loadingRuleStore; SelectedFolder = loadingFolder; }

    let update s t =
        match t with
        | RuleStoreUpdated r -> { s with RuleStore = r }
        | SelectedFolderChagned f -> { s with SelectedFolder = f }

    let toList s =
        s.RuleStore.Rules |> List.filter (fun r -> r.FolderId = s.SelectedFolder.Id)

type ViewModelSuperBase() as this =
    inherit ViewModelBase()

    member __.SuperEvent<'t>(initialValue, expr : Quotations.Expr) =
        let event = SuperEvent<'t>(initialValue)
        event.Publish.Add (fun _ -> this.RaisePropertyChanged(expr))
        event

type MainWindowViewModel() as this =
    inherit ViewModelSuperBase()

    let loadingFolder = { Id = null; Name= "Loading"; Children= []; }
    let loginErrorFolder = { Id = null; Name= "Login Error"; Children= []; }
    let inboxFolder = this.Factory.Backing(<@ this.InboxFolder @>, loadingFolder)

    let emptyRule = { Instance = null; Id = null; Name = ""; FolderId = null; FromAddresses = []; SentToAddresses = [] }
    let ruleStore = SuperEvent<RuleStore>(RuleStore.Zero)
    let ruleStoreSaved = SuperEvent<RuleStore>(RuleStore.Zero)
    let folderSelectedEvent = Event<Folder>()
    let selectedRule = this.SuperEvent<Rule>(emptyRule, <@ this.SelectedRule @>)

    let updateRuleName (ruleName : string) =
        let newRule = Rule.updateName ruleName this.SelectedRule
        let newRules = ruleStore.Value.Rules |> List.map (fun r -> if r.Id = this.SelectedRule.Id then newRule else r)
        let newState = { Rules = newRules; }
        ruleStore.Trigger newState

    let loadDataAsync user = async {
        do! Async.SwitchToThreadPool () // TODO Make native async operators and avoid this

        let folderResult = User.getInboxFolder user
        inboxFolder.Value <- Result.orDefault folderResult loginErrorFolder

        let rulesResult = User.getRules user
        let store = { Rules = Result.orDefault rulesResult []; }
        ruleStore.Trigger store
        ruleStoreSaved.Trigger store
    }

    let login _ =
        Views.LoginDialog().ShowDialog() |> ignore
        User.get () |> loadDataAsync

    let save _ = async {
        do! Async.SwitchToThreadPool () // TODO Make native async operators and avoid this

        let user = User.get ()
        let modifiedRules = RuleStore.getDiffRules ruleStoreSaved.Value ruleStore.Value
        Rule.saveToServer user modifiedRules |> ignore
        ruleStoreSaved.Trigger ruleStore.Value
    }

    do
        Event.merge
            (ruleStore.Publish |> Event.map RuleStoreUpdated)
            (folderSelectedEvent.Publish |> Event.map SelectedFolderChagned)
            |> Event.scan DisplayRule.update DisplayRule.loadingState
            |> Event.map DisplayRule.toList
            |> Event.add (fun r -> this.DisplayRule <- r; this.RaisePropertyChanged(<@ this.DisplayRule @>))

        Event.pair
            (RuleStore.Zero, ruleStoreSaved.Publish)
            (RuleStore.Zero, ruleStore.Publish)
            |> Event.map (fun (v1, v2) -> RuleStore.compare v1 v2)
            |> Event.add (fun v -> this.SaveEnabled <- v <> 0; this.RaisePropertyChanged(<@ this.SaveCommand @>))

        Async.Start (User.get () |> loadDataAsync)

    member val DisplayRule = [] with get, set
    member val SaveEnabled = false with get, set
    member __.SelectedRule with get() = selectedRule.Value

    member this.InboxFolder with get() = [inboxFolder.Value]
    member this.LoginCommand = this.Factory.CommandAsync(login)
    member this.SaveCommand = this.Factory.CommandAsyncChecked(save, fun _ -> this.SaveEnabled)
    member this.SelectFolderCommand = this.Factory.CommandSyncParam(folderSelectedEvent.Trigger)
    member this.SelectRuleCommand = this.Factory.CommandSyncParam(selectedRule.Trigger)
    member this.ChangeRuleNameCommand = this.Factory.CommandSyncParam(updateRuleName)
