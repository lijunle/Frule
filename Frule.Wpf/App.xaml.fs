module App

open FsXaml
open System

type App = XAML<"App.xaml">

[<STAThread>]
[<EntryPoint>]
let main argv =
    App().Run()
