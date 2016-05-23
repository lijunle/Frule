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

let getServiceUrl (email, password) =
    let credential = WebCredentials(email, password)
    let service = ExchangeService(ExchangeVersion.Exchange2013_SP1, Credentials = credential)
    let url = autodiscoverServiceUrl email service
    url

let getService user =
    let { Email = email; Password = password; ServiceUrl = serviceUrl } = user
    let credential = WebCredentials(email, password)
    let service = ExchangeService(ExchangeVersion.Exchange2013_SP1, Credentials = credential, Url = serviceUrl)
    service
