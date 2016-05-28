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

    let ruleStore = SuperEvent<RuleStore>(RuleStore.Zero)
    let ruleStoreSaved = SuperEvent<RuleStore>(RuleStore.Zero)
    let selectedFolder = SuperEvent<Folder>(loadingFolder)
    let selectedRule = this.SuperEvent<Rule>(Rule.Zero, <@ this.SelectedRule @>)
    let displayRules = this.SuperEvent<Rule list>([], <@ this.DisplayRules @>)
    let saveEnabled = this.SuperEvent<bool>(false, <@ this.SaveCommand @>)

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
            (selectedFolder.Publish |> Event.map SelectedFolderChagned)
            |> Event.scan DisplayRule.update DisplayRule.loadingState
            |> Event.map DisplayRule.toList
            |> Event.add (displayRules.Trigger)

        Event.pair
            (RuleStore.Zero, ruleStoreSaved.Publish)
            (RuleStore.Zero, ruleStore.Publish)
            |> Event.map (fun (v1, v2) -> RuleStore.compare v1 v2 <> 0)
            |> Event.add (saveEnabled.Trigger)

        Async.Start (User.get () |> loadDataAsync)

    member __.DisplayRules with get() = displayRules.Value
    member __.SelectedRule with get() = selectedRule.Value

    member this.InboxFolder with get() = [inboxFolder.Value]
    member this.LoginCommand = this.Factory.CommandAsync(login)
    member this.SaveCommand = this.Factory.CommandAsyncChecked(save, fun _ -> saveEnabled.Value)
    member this.SelectFolderCommand = this.Factory.CommandSyncParam(selectedFolder.Trigger)
    member this.SelectRuleCommand = this.Factory.CommandSyncParam(selectedRule.Trigger)
    member this.ChangeRuleNameCommand = this.Factory.CommandSyncParam(updateRuleName)
