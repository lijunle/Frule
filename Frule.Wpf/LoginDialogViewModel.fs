namespace ViewModels

open FSharp.ViewModule
open System.Windows

type LoginDialogViewModel() as this =
    inherit ViewModelBase()

    let email = this.Factory.Backing(<@ this.Email @>, "")
    let password = this.Factory.Backing(<@ this.Password @>, "")

    let login (dialog : Window) =
        let user = User.construct (email.Value, password.Value)
        match user with
        | Success v ->
            User.set v
            dialog.Close()
        | Failure _ ->
            ()

    let cancel (dialog : Window) =
        dialog.Close()

    member this.Email with get() = email.Value and set value = email.Value <- value
    member this.Password with get() = password.Value and set value = password.Value <- value
    member this.LoginCommand = this.Factory.CommandSyncParam(login)
    member this.CancelCommand = this.Factory.CommandSyncParam(cancel)
