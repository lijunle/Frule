[<AutoOpen>]
module ViewModelSuperBase

open FSharp.ViewModule

type ViewModelSuperBase() as this =
    inherit ViewModelBase()

    member __.SuperEvent<'t>(initialValue, expr : Quotations.Expr) =
        let event = SuperEvent<'t>(initialValue)
        event.Publish.Add (fun _ -> this.RaisePropertyChanged(expr))
        event
