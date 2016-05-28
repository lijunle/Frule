module Event

let zip (d1, e1) (d2, e2) =
    let mutable v1 = d1
    let mutable v2 = d2
    Event.merge
        (e1 |> Event.map (fun v -> v1 <- v))
        (e2 |> Event.map (fun v -> v2 <- v))
        |> Event.map (fun _ -> (v1, v2))
