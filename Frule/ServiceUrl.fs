module ServiceUrl

open Microsoft.Exchange.WebServices.Autodiscover
open Microsoft.Exchange.WebServices.Data
open System

let redirectionUrlValidationCallback (redirectionUrl : string) =
    let redirectionUri = new Uri(redirectionUrl)
    let result = redirectionUri.Scheme = "https"
    result

let autodiscoverServiceUrl email password =
    try
        let credential = WebCredentials(email, password)
        let service = ExchangeService(ExchangeVersion.Exchange2013_SP1, Credentials = credential)
        let callback = AutodiscoverRedirectionUrlValidationCallback(redirectionUrlValidationCallback)
        service.AutodiscoverUrl(email, callback)
        Result.ret service.Url
    with e ->
        Result.Failure e

let getServiceUrl email password =
    // TODO read from local cache
    autodiscoverServiceUrl email password
