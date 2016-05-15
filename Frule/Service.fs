module Service

open Microsoft.Exchange.WebServices.Autodiscover
open Microsoft.Exchange.WebServices.Data
open System
open System.IO

let cacheFileName = "service.dat"

let getServiceUrlFromFile email =
    let split (line : string) =
        let pair = line.Split('\t')
        (pair.[0], pair.[1])
    try
        let cache = File.ReadAllLines(cacheFileName)
                    |> Array.map split
                    |> dict
        let (ok, value) = cache.TryGetValue email
        if ok
        then Uri value |> Some
        else None
    with e -> None

let writeServiceUrlToFile email url =
    try
        let line = sprintf "%s\t%s" email url
        File.AppendAllText(cacheFileName, line)
    with e -> ()

let toString (uri : Uri) =
    uri.ToString()

let redirectionUrlValidationCallback =
    let callback (redirectionUrl : string) =
        let redirectionUri = Uri redirectionUrl
        let result = redirectionUri.Scheme = "https"
        result

    AutodiscoverRedirectionUrlValidationCallback callback

let autodiscoverServiceUrl email (service: ExchangeService) =
    try
        service.AutodiscoverUrl(email, redirectionUrlValidationCallback)
        Result.ret service.Url
    with e ->
        Result.Failure e

let getServiceUrl email (service: ExchangeService) =
    let cachedEmail = getServiceUrlFromFile email
    match cachedEmail with
    | Some url -> Result.ret url
    | None ->
        let url = autodiscoverServiceUrl email service
        Result.ifSuccessDo url (toString >> writeServiceUrlToFile email)
        url

let getService email password : Result<ExchangeService> =
    Result.result {
        let credential = WebCredentials(email, password)
        let service = ExchangeService(ExchangeVersion.Exchange2013_SP1, Credentials = credential)

        let! url = getServiceUrl email service
        service.Url <- url

        return service
    }
