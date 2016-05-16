module LoginDialog

open System.Windows
open System.Windows.Controls

type LoginDialogViewModel() =
    member val Email = "" with get, set
    member val Password = "" with get, set

let instance : Window = getXamlResource "LoginDialog.xaml"

let initialize setState =
    let viewModel = LoginDialogViewModel()
    instance.DataContext <- viewModel

    let login () =
        printf "%A" viewModel
        setState MainWindow

    instance.Closing.Add (fun _ -> setState Shutdown)

    (instance.FindName("cancelButton") :?> Button).Click.Add (fun _ -> setState Shutdown)
    (instance.FindName("loginButton") :?> Button).Click.Add (fun _ -> login())

    ()
