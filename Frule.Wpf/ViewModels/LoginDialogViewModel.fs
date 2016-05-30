namespace ViewModels

open FSharp.ViewModule
open Views

type LoginDialogViewModel(store : Store) as this =
    inherit ViewModelBase()

    let email = this.Factory.Backing(<@ this.Email @>, "")
    let password = this.Factory.Backing(<@ this.Password @>, "")
    let state = this.Factory.Backing(<@ this.State @>, "")

    let login _ =
        async {
            do! Async.SwitchToThreadPool () // TODO Make native async operators and avoid this
            state.Value <- "Start login..."
            let user = User.construct (email.Value, password.Value)
            match user with
            | Success v ->
                state.Value <- "Login successful."
                User.set v
                store.User.Trigger (Some v)
                store.LoginDialogState.Trigger Close
            | Failure e ->
                state.Value <- sprintf "Login failed. %s" e.Message
        }

    let cancel () =
        store.LoginDialogState.Trigger Close

    static member Register store =
        let mutable dialog = null
        store.LoginDialogState.Publish.Add (fun v ->
            match v with
            | Open ->
                let dialogViewModel = LoginDialogViewModel(store)
                dialog <- LoginDialog(DataContext = dialogViewModel)
                dialog.ShowDialog() |> ignore
            | Close ->
                dialog.Close())

    member __.Email with get() = email.Value and set value = email.Value <- value
    member __.Password with get() = password.Value and set value = password.Value <- value
    member __.State with get() = state.Value and set value = state.Value <- value
    member this.LoginCommand = this.Factory.CommandAsync(login)
    member this.CancelCommand = this.Factory.CommandSync(cancel)
