﻿namespace ViewModels

open FSharp.ViewModule

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

    let ruleStore = SuperEvent<RuleStore>(RuleStore.Zero)
    let ruleStoreSaved = SuperEvent<RuleStore>(RuleStore.Zero)
    let selectedFolder = SuperEvent<Folder>(loadingFolder)
    let inboxFolder = this.SuperEvent<Folder list>([loadingFolder], <@ this.InboxFolder @>)
    let selectedRule = this.SuperEvent<RuleInfoViewModel>(RuleInfoViewModel.Zero, <@ this.SelectedRule @>)
    let displayRules = this.SuperEvent<Rule list>([], <@ this.DisplayRules @>)
    let saveEnabled = this.SuperEvent<bool>(false, <@ this.SaveCommand @>)

    let updateRule (rule : Rule) =
        let newRules = ruleStore.Value.Rules |> List.map (fun r -> if r.Id = selectedRule.Value.Rule.Id then rule else r)
        let newState = { Rules = newRules; }
        ruleStore.Trigger newState

    let loadDataAsync user = async {
        do! Async.SwitchToThreadPool () // TODO Make native async operators and avoid this

        let folderResult = User.getInboxFolder user
        inboxFolder.Trigger (Result.orDefault folderResult loginErrorFolder |> List.singleton)

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
        selectedFolder.Publish
            |> Event.add (fun _ -> selectedRule.Trigger RuleInfoViewModel.Zero)

        SuperEvent.zip ruleStore selectedFolder
            |> Event.map (fun (s, f) -> s.Rules |> List.filter (fun r -> r.FolderId = f.Id))
            |> Event.add (displayRules.Trigger)

        SuperEvent.zip ruleStoreSaved ruleStore
            |> Event.map (fun (v1, v2) -> RuleStore.compare v1 v2 <> 0)
            |> Event.add (saveEnabled.Trigger)

        Async.Start (User.get () |> loadDataAsync)

    member __.InboxFolder with get() = inboxFolder.Value
    member __.DisplayRules with get() = displayRules.Value
    member __.SelectedRule with get() = selectedRule.Value

    member this.LoginCommand = this.Factory.CommandAsync(login)
    member this.SaveCommand = this.Factory.CommandAsyncChecked(save, fun _ -> saveEnabled.Value)
    member this.SelectFolderCommand = this.Factory.CommandSyncParam(selectedFolder.Trigger)
    member this.SelectRuleCommand = this.Factory.CommandSyncParam(RuleInfoViewModel.Create updateRule >> selectedRule.Trigger)