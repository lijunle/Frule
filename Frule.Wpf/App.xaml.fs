module App

open FsXaml
open System
open System.Windows
open ViewModels
open Views

type App = XAML<"App.xaml">

[<STAThread>]
[<EntryPoint>]
let main _ =
    let store = Store.create()
    let windowViewModel = MainWindowViewModel(store)
    let window = MainWindow(DataContext = windowViewModel)
    LoginDialogViewModel.Register store
    Async.Start (Store.loadAsync store)
    App().Run(window)
