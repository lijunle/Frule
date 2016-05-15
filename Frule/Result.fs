module Result

let bind f x =
    match x with
    | Success v -> f v
    | Failure e -> Failure e

let ret x =
    Success x

let ifSuccessDo x f =
    bind (f >> ret) x |> ignore

type Builder() =
    member this.Bind(x, f) = bind f x
    member this.Return(x) = ret x

let result = new Builder()
