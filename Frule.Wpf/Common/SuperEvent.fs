[<AutoOpen>]
module SuperEvent

type SuperEvent<'t>(initialValue) as this =
    inherit Event<'t>()

    let mutable value = initialValue

    do this.Publish.Add (fun v -> value <- v)

    member __.Value with get () = value

let zip (e1 : SuperEvent<'a>) (e2 : SuperEvent<'b>) =
    let mutable v1 = e1.Value
    let mutable v2 = e2.Value
    Event.merge
        (e1.Publish |> Event.map (fun v -> v1 <- v))
        (e2.Publish |> Event.map (fun v -> v2 <- v))
        |> Event.map (fun _ -> (v1, v2))
