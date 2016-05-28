[<AutoOpen>]
module SuperEvent

type SuperEvent<'t>(initialValue) as this =
    inherit Event<'t>()

    let mutable value = initialValue

    do this.Publish.Add (fun v -> value <- v)

    member __.Value with get () = value
