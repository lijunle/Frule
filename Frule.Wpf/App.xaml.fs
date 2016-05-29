module App

open FsXaml
open System
open ViewModels
open Views

type App = XAML<"App.xaml">

[<STAThread>]
[<EntryPoint>]
let main _ =
    let store = Store.create()
    let viewModel = MainWindowViewModel(store)
    let window = MainWindow(DataContext = viewModel)
    Async.Start (Store.loadAsync store)
    App().Run(window)
