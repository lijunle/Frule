module Program

open System
open System.Windows

let getXamlResource xaml =
    Application.LoadComponent(Uri(xaml, UriKind.Relative))
    :?> _

let startApplication (application: Application) (event: StartupEventArgs) =
    let loginDialog : Window = getXamlResource "LoginDialog.xaml"
    loginDialog.Closing.Add (fun _ -> application.Shutdown())
    loginDialog.Show()
    ()

[<STAThread>]
[<EntryPoint>]
let main args =
    let application = Application(ShutdownMode = ShutdownMode.OnExplicitShutdown)
    application.Startup.Add (startApplication application)
    application.Run()
