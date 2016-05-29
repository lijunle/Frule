namespace ViewModels

open FSharp.ViewModule

type MainWindowViewModel(store : Store) as this =
    inherit ViewModelBase()

    let login _ =
        Views.LoginDialog().ShowDialog() |> ignore
        User.get () |> Store.loadAsync store

    let save _ = async {
        do! Async.SwitchToThreadPool () // TODO Make native async operators and avoid this

        let user = User.get ()
        let modifiedRules = Store.getDiffRules store.SavedRules.Value store.Rules.Value
        Rule.saveToServer user modifiedRules |> ignore
        store.SavedRules.Trigger store.Rules.Value
    }

    do
        store.InboxFolder.Publish.Add(fun _ -> this.RaisePropertyChanged(<@ this.InboxFolder @>))
        store.SelectedRule.Publish.Add (fun _ -> this.RaisePropertyChanged(<@ this.SelectedRule @>))
        store.SaveButtonEnabled.Publish.Add(fun _ -> this.RaisePropertyChanged(<@ this.SaveCommand @>))

        SuperEvent.zip4 store.SavedRules store.Rules store.SelectedFolder store.SelectedRule
            |> Event.add (fun _ -> this.RaisePropertyChanged(<@ this.DisplayRules @>))

        store.SelectedFolder.Publish
            |> Event.add (fun _ -> store.SelectedRule.Trigger Rule.Zero)

        SuperEvent.zip store.SavedRules store.Rules
            |> Event.map (fun (v1, v2) -> Store.compare v1 v2 <> 0)
            |> Event.add (store.SaveButtonEnabled.Trigger)

    member __.InboxFolder with get() = store.InboxFolder.Value
    member __.DisplayRules with get() = RuleListViewModel store
    member __.SelectedRule with get() = RuleInfoViewModel store

    member this.LoginCommand = this.Factory.CommandAsync(login)
    member this.SaveCommand = this.Factory.CommandAsyncChecked(save, fun _ -> store.SaveButtonEnabled.Value)
    member this.SelectFolderCommand = this.Factory.CommandSyncParam(store.SelectedFolder.Trigger)
