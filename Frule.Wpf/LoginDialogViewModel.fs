namespace ViewModels

open FSharp.ViewModule
open System.Windows

type LoginDialogViewModel() as this =
    inherit ViewModelBase()

    let email = this.Factory.Backing(<@ this.Email @>, "")
    let password = this.Factory.Backing(<@ this.Password @>, "")
    let loginCommand =
        this.Factory.CommandSync(fun _ -> ())

    member this.Email with get() = email.Value and set value = email.Value <- value
    member this.Password with get() = password.Value and set value = password.Value <- value
    member this.LoginCommand = loginCommand
