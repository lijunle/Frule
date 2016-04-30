module ServiceUrl

open System
open Microsoft.Exchange.WebServices.Autodiscover
open Microsoft.Exchange.WebServices.Data

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
        Option.ofObj service.Url
    with e -> None

let getServiceUrl email password =
    // TODO read from local cache
    autodiscoverServiceUrl email password
