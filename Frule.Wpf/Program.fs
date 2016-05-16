module Program

open System
open System.Windows

[<STAThread>]
[<EntryPoint>]
let main args =
    let application = Application(ShutdownMode = ShutdownMode.OnExplicitShutdown)
    let setState state =
        match state with
        | Shutdown ->
            application.Shutdown()
        | LoginDialog ->
            MainWindow.instance.Hide()
            LoginDialog.instance.Show()
        | MainWindow ->
            LoginDialog.instance.Hide()
            MainWindow.instance.Show()
    MainWindow.initialize setState
    LoginDialog.initialize setState
    application.Startup.Add (fun _ -> setState LoginDialog)
    application.Run()
