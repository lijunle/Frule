module Result

let orDefault x d =
    match x with
    | Success v -> v
    | Failure _ -> d

let bind f x =
    match x with
    | Success v -> f v
    | Failure e -> Failure e

let ret x =
    Success x

let map f x =
    match x with
    | Success v -> f v |> ret
    | Failure e -> Failure e

type Builder() =
    member this.Bind(x, f) = bind f x
    member this.Return(x) = ret x

let result = new Builder()
