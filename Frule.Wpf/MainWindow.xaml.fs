module MainWindow

open Folder
open Rule
open Service
open System.Windows
open System.Windows.Controls

type MainWindowViewModel(setState) =
    member val InboxFolder = [] with get, set

let instance : Window = getXamlResource "MainWindow.xaml"

let initialize setState (email, password) =
    let resultFolder =
        Result.result {
            let! service = getService email password
            let! rules = getRules service |> Result.map List.ofSeq
            let! result = getFolderHierarchy service rules
            return result
        }
    let inboxFolder =
        match resultFolder with
        | Success f -> f
        | Failure e -> { Id = null; Name= "Inbox"; Children= []; Rules= [] }
    instance.DataContext <- MainWindowViewModel(setState, InboxFolder = [inboxFolder])
    instance.Closing.Add (fun _ -> setState Shutdown)
    ()
