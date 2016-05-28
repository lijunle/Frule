﻿[<AutoOpen>]
module SuperEvent

type SuperEvent<'t>(initialValue) as this =
    inherit Event<'t>()

    let mutable value = initialValue

    do this.Publish.Add (fun v -> value <- v)

    member __.Value with get () = value

let pair (e1 : SuperEvent<'a>) (e2 : SuperEvent<'b>) =
    Event.pair (e1.Value, e1.Publish) (e2.Value, e2.Publish)
