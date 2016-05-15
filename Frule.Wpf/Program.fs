module Program

open System
open System.Windows

let getXamlResource xaml =
    Application.LoadComponent(Uri(xaml, UriKind.Relative))
    :?> _

[<STAThread>]
[<EntryPoint>]
let main args =
    let mainWindow : Window = getXamlResource "MainWindow.xaml"
    Application().Run(mainWindow)
