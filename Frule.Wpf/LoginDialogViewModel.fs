namespace ViewModels

open System.Windows

type LoginDialogViewModel() =
    let shutdown () =
        Application.Current.Shutdown()
    let login () =
        ()
    member val Email = "" with get, set
    member val Password = "" with get, set
    member val LoginCommand = Command(login)
    member val CancelCommand = Command(shutdown)
