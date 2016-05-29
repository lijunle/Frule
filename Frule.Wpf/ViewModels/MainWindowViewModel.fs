namespace ViewModels

type MainWindowViewModel() as this =
    inherit ViewModelSuperBase()

    let loadingFolder = { Id = null; Name= "Loading"; Children= []; }
    let loginErrorFolder = { Id = null; Name= "Login Error"; Children= []; }

    let store = Store.create()
    let inboxFolder = this.SuperEvent<Folder list>([loadingFolder], <@ this.InboxFolder @>)
    let displayRules = this.SuperEvent<RuleListViewModel>(RuleListViewModel.Zero, <@ this.DisplayRules @>)
    let saveEnabled = this.SuperEvent<bool>(false, <@ this.SaveCommand @>)

    let selectRule =
        store.SelectedRule.Trigger

    let loadDataAsync user = async {
        do! Async.SwitchToThreadPool () // TODO Make native async operators and avoid this

        let folderResult = User.getInboxFolder user
        inboxFolder.Trigger (Result.orDefault folderResult loginErrorFolder |> List.singleton)

        let rulesResult = User.getRules user
        let loadedRules = Result.orDefault rulesResult []
        store.Rules'.Trigger loadedRules
        store.SavedRules.Trigger loadedRules
    }

    let login _ =
        Views.LoginDialog().ShowDialog() |> ignore
        User.get () |> loadDataAsync

    let save _ = async {
        do! Async.SwitchToThreadPool () // TODO Make native async operators and avoid this

        let user = User.get ()
        let modifiedRules = Store.getDiffRules store.SavedRules.Value store.Rules'.Value
        Rule.saveToServer user modifiedRules |> ignore
        store.SavedRules.Trigger store.Rules'.Value
    }

    do
        store.SelectedRule.Publish.Add (fun _ -> this.RaisePropertyChanged(<@ this.SelectedRule @>))

        store.SelectedFolder.Publish
            |> Event.add (fun _ -> store.SelectedRule.Trigger Rule.Zero)

        SuperEvent.zip4 store.SavedRules store.Rules' store.SelectedFolder store.SelectedRule
            |> Event.map (RuleListViewModel.Create selectRule)
            |> Event.add (displayRules.Trigger)

        SuperEvent.zip store.SavedRules store.Rules'
            |> Event.map (fun (v1, v2) -> Store.compare v1 v2 <> 0)
            |> Event.add (saveEnabled.Trigger)

        Async.Start (User.get () |> loadDataAsync)

    member __.InboxFolder with get() = inboxFolder.Value
    member __.DisplayRules with get() = displayRules.Value
    member __.SelectedRule with get() = RuleInfoViewModel store

    member this.LoginCommand = this.Factory.CommandAsync(login)
    member this.SaveCommand = this.Factory.CommandAsyncChecked(save, fun _ -> saveEnabled.Value)
    member this.SelectFolderCommand = this.Factory.CommandSyncParam(store.SelectedFolder.Trigger)
