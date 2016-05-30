module App

open FsXaml
open System
open System.Windows
open ViewModels
open Views

type App = XAML<"App.xaml">

let registerDialogState (dialog : Window) (event : SuperEvent<DialogState>)  =
    event.Publish.Add (fun v ->
        match v with
        | Open -> dialog.ShowDialog() |> ignore
        | Close -> dialog.Hide())

[<STAThread>]
[<EntryPoint>]
let main _ =
    let store = Store.create()
    let windowViewModel = MainWindowViewModel(store)
    let window = MainWindow(DataContext = windowViewModel)
    let dialogViewModel = LoginDialogViewModel(store)
    let dialog = LoginDialog(DataContext = dialogViewModel)
    store.LoginDialogState |> registerDialogState dialog
    Async.Start (Store.loadAsync store)
    App().Run(window)
