module Service

open Microsoft.Exchange.WebServices.Autodiscover
open Microsoft.Exchange.WebServices.Data
open System

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
    // TODO read from local cache
    autodiscoverServiceUrl email service

let getService email password : Result<ExchangeService> =
    Result.result {
        let credential = WebCredentials(email, password)
        let service = ExchangeService(ExchangeVersion.Exchange2013_SP1, Credentials = credential)

        let! url = getServiceUrl email service
        service.Url <- url

        return service
    }
