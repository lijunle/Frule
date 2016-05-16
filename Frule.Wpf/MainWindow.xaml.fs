module MainWindow

open System.Windows
open System.Windows.Controls

let instance : Window = getXamlResource "MainWindow.xaml"

let initialize setState =
    instance.Closing.Add (fun _ -> setState Shutdown)
    ()
