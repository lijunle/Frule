namespace ViewModels

open FSharp.ViewModule

type UserInfoViewModel(store : Store) as this =
    inherit ViewModelBase()

    let user = store.User
    let currentRules = store.Rules
    let savedRules = store.SavedRules
    let saveButtonEnabled = store.SaveButtonEnabled

    let login _ =
        Views.LoginDialog().ShowDialog() |> ignore
        User.get () |> user.Trigger // TODO move to LoginViewModel
        Store.loadAsync store

    let save _ = async {
        do! Async.SwitchToThreadPool () // TODO Make native async operators and avoid this

        match user.Value with
        | None -> ()
        | Some user ->
            let modifiedRules = Store.getDiffRules savedRules.Value currentRules.Value
            Rule.saveToServer user modifiedRules |> ignore
            savedRules.Trigger currentRules.Value
    }

    do
        saveButtonEnabled.Publish.Add(fun _ -> this.RaisePropertyChanged(<@ this.SaveCommand @>))

        SuperEvent.zip savedRules currentRules
            |> Event.map (fun (v1, v2) -> Store.compare v1 v2 <> 0)
            |> Event.add (saveButtonEnabled.Trigger)

    member this.LoginCommand = this.Factory.CommandAsync(login)
    member this.SaveCommand = this.Factory.CommandAsyncChecked(save, fun _ -> saveButtonEnabled.Value)
