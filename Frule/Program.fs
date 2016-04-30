module Program

open ServiceUrl

[<EntryPoint>]
let main argv =
    let email = argv.[0]
    let password = argv.[1]
    let serviceUrl = getServiceUrl email password
    match serviceUrl with
    | None -> printfn "Service URL not found!"
    | Some serviceUrl -> printfn "%A" serviceUrl
    0
