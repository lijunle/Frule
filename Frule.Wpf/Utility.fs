[<AutoOpen>]
module Utility

open System
open System.Windows

type WindowState =
    | LoginDialog
    | MainWindow
    | Shutdown

let getXamlResource xaml =
    Application.LoadComponent(Uri(xaml, UriKind.Relative))
    :?> _
