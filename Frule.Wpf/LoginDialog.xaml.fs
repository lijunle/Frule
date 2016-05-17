module LoginDialog

open System.Windows
open System.Windows.Controls

type LoginDialogViewModel(setState) as this =
    let shutdown () =
        setState Shutdown
    let login () =
        setState (MainWindow (this.Email, this.Password))
    member val Email = "" with get, set
    member val Password = "" with get, set
    member val CancelCommand = Command(shutdown)
    member val LoginCommand = Command(login)

let instance : Window = getXamlResource "LoginDialog.xaml"

let initialize setState =
    instance.DataContext <- LoginDialogViewModel(setState)
    instance.Closing.Add (fun _ -> setState Shutdown)
    ()
