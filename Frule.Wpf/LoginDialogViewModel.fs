namespace ViewModels

open FSharp.ViewModule
open System.Windows

type LoginDialogViewModel() as this =
    inherit ViewModelBase()

    let email = this.Factory.Backing(<@ this.Email @>, "")
    let password = this.Factory.Backing(<@ this.Password @>, "")

    let login () =
        User.set (email.Value, password.Value)
        // TODO verify the email and password
        // TODO close login dialog

    member this.Email with get() = email.Value and set value = email.Value <- value
    member this.Password with get() = password.Value and set value = password.Value <- value
    member this.LoginCommand = this.Factory.CommandSync(login)
    member this.CancelCommand = this.Factory.CommandSync(Application.Current.Shutdown)
