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

let zip3 (e1 : SuperEvent<'a>) (e2 : SuperEvent<'b>) (e3 : SuperEvent<'c>) =
    let mutable v1 = e1.Value
    let mutable v2 = e2.Value
    let mutable v3 = e3.Value
    let s1 = e1.Publish |> Event.map (fun v -> v1 <- v)
    let s2 = e2.Publish |> Event.map (fun v -> v2 <- v)
    let s3 = e3.Publish |> Event.map (fun v -> v3 <- v)
    s1
        |> Event.merge s2
        |> Event.merge s3
        |> Event.map (fun _ -> (v1, v2, v3))

let zip4 (e1 : SuperEvent<'a>) (e2 : SuperEvent<'b>) (e3 : SuperEvent<'c>) (e4 : SuperEvent<'d>) =
    let mutable v1 = e1.Value
    let mutable v2 = e2.Value
    let mutable v3 = e3.Value
    let mutable v4 = e4.Value
    let s1 = e1.Publish |> Event.map (fun v -> v1 <- v)
    let s2 = e2.Publish |> Event.map (fun v -> v2 <- v)
    let s3 = e3.Publish |> Event.map (fun v -> v3 <- v)
    let s4 = e4.Publish |> Event.map (fun v -> v4 <- v)
    s1
        |> Event.merge s2
        |> Event.merge s3
        |> Event.merge s4
        |> Event.map (fun _ -> (v1, v2, v3, v4))
