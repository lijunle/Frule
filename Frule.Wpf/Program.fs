module Program

open System
open System.Windows

[<STAThread>]
[<EntryPoint>]
let main args =
    let application = Application(ShutdownMode = ShutdownMode.OnExplicitShutdown)
    let rec setState state =
        match state with
        | Shutdown ->
            application.Shutdown()
        | LoginDialog ->
            MainWindow.instance.Hide()
            LoginDialog.initialize setState
            LoginDialog.instance.Show()
        | MainWindow (email, password) ->
            LoginDialog.instance.Hide()
            MainWindow.initialize setState (email, password)
            MainWindow.instance.Show()
    application.Startup.Add (fun _ -> setState LoginDialog)
    application.Run()
