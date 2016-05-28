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
    let loadingRuleStore = { Modified = false; Model = []; }
    let loadingState = { RuleStore = loadingRuleStore; SelectedFolder = loadingFolder; }

    let update s t =
        match t with
        | RuleStoreUpdated r -> { s with RuleStore = r }
        | SelectedFolderChagned f -> { s with SelectedFolder = f }

    let toList s =
        s.RuleStore.Model |> List.filter (fun r -> r.FolderId = s.SelectedFolder.Id)

type MainWindowViewModel() as this =
    inherit ViewModelBase()

    let loadingFolder = { Id = null; Name= "Loading"; Children= []; }
    let loginErrorFolder = { Id = null; Name= "Login Error"; Children= []; }
    let inboxFolder = this.Factory.Backing(<@ this.InboxFolder @>, loadingFolder)

    let emptyRule = { Modified = false; Instance = null; Id = null; Name = ""; FolderId = null; FromAddresses = []; SentToAddresses = [] }
    let ruleStore = Event<RuleStore>()
    let folderSelectedEvent = Event<Folder>()
    let ruleSelectedEvent = Event<Rule>()

    let updateRuleName (ruleName : string) =
        let newRule = Rule.updateName ruleName this.SelectedRule
        let newRules = this.RuleStore.Model |> List.map (fun r -> if r.Id = this.SelectedRule.Id then newRule else r)
        let newState = { Modified = true; Model = newRules; }
        ruleStore.Trigger newState

    let loadDataAsync user = async {
        do! Async.SwitchToThreadPool () // TODO Make native async operators and avoid this

        let folderResult = User.getInboxFolder user
        inboxFolder.Value <- Result.orDefault folderResult loginErrorFolder

        let rulesResult = User.getRules user
        ruleStore.Trigger { Modified = false; Model = Result.orDefault rulesResult []; }
    }

    let login _ =
        Views.LoginDialog().ShowDialog() |> ignore
        User.get () |> loadDataAsync

    let save _ = async {
        do! Async.SwitchToThreadPool () // TODO Make native async operators and avoid this

        let user = User.get ()
        let rules = this.RuleStore.Model
        Rule.saveToServer user rules |> ignore
        ruleStore.Trigger { Modified = false; Model = rules; }
    }

    do
        Event.merge
            (ruleStore.Publish |> Event.map RuleStoreUpdated)
            (folderSelectedEvent.Publish |> Event.map SelectedFolderChagned)
            |> Event.scan DisplayRule.update DisplayRule.loadingState
            |> Event.map DisplayRule.toList
            |> Event.add (fun r -> this.DisplayRule <- r; this.RaisePropertyChanged(<@ this.DisplayRule @>))

        ruleStore.Publish
            |> Event.add (fun s -> this.RuleStore <- s)

        ruleSelectedEvent.Publish
            |> Event.add (fun r -> this.SelectedRule <- r; this.RaisePropertyChanged(<@ this.SelectedRule @>))

        ruleStore.Publish
            |> Event.add (fun s -> this.SaveEnabled <- s.Modified; this.RaisePropertyChanged(<@ this.SaveCommand @>))

        Async.Start (User.get () |> loadDataAsync)

    member val RuleStore = DisplayRule.loadingRuleStore with get, set
    member val DisplayRule = [] with get, set
    member val SelectedRule = emptyRule with get, set
    member val SaveEnabled = false with get, set

    member this.InboxFolder with get() = [inboxFolder.Value]
    member this.LoginCommand = this.Factory.CommandAsync(login)
    member this.SaveCommand = this.Factory.CommandAsyncChecked(save, fun _ -> this.SaveEnabled)
    member this.SelectFolderCommand = this.Factory.CommandSyncParam(folderSelectedEvent.Trigger)
    member this.SelectRuleCommand = this.Factory.CommandSyncParam(ruleSelectedEvent.Trigger)
    member this.ChangeRuleNameCommand = this.Factory.CommandSyncParam(updateRuleName)
