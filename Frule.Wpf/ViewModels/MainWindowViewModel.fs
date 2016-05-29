namespace ViewModels

open FSharp.ViewModule

type MainWindowViewModel(store : Store) as this =
    inherit ViewModelBase()

    let currentRules = store.Rules
    let savedRules = store.SavedRules
    let saveButtonEnabled = store.SaveButtonEnabled

    let login _ =
        Views.LoginDialog().ShowDialog() |> ignore
        User.get () |> Store.loadAsync store

    let save _ = async {
        do! Async.SwitchToThreadPool () // TODO Make native async operators and avoid this

        let user = User.get ()
        let modifiedRules = Store.getDiffRules savedRules.Value currentRules.Value
        Rule.saveToServer user modifiedRules |> ignore
        savedRules.Trigger currentRules.Value
    }

    do
        saveButtonEnabled.Publish.Add(fun _ -> this.RaisePropertyChanged(<@ this.SaveCommand @>))

        SuperEvent.zip savedRules currentRules
            |> Event.map (fun (v1, v2) -> Store.compare v1 v2 <> 0)
            |> Event.add (saveButtonEnabled.Trigger)

    member __.InboxFolder = FolderListViewModel store
    member __.DisplayRules = RuleListViewModel store
    member __.SelectedRule = RuleInfoViewModel store

    member this.LoginCommand = this.Factory.CommandAsync(login)
    member this.SaveCommand = this.Factory.CommandAsyncChecked(save, fun _ -> saveButtonEnabled.Value)
