[<AutoOpen>]
module Utility

open System
open System.Windows
open System.Windows.Input

type WindowState =
    | LoginDialog
    | MainWindow
    | Shutdown

type Command(execute, ?canExecute) =
    let canExecute' = defaultArg canExecute (fun _ -> true)
    interface ICommand with
        member this.CanExecute(parameter : obj) = canExecute'()
        member this.Execute(parameter : obj) = execute()
        member this.add_CanExecuteChanged(handler) =
            CommandManager.RequerySuggested.AddHandler handler
        member this.remove_CanExecuteChanged(handler) =
            CommandManager.RequerySuggested.RemoveHandler handler

let getXamlResource xaml =
    Application.LoadComponent(Uri(xaml, UriKind.Relative))
    :?> _
