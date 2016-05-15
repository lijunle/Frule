module Service

open Microsoft.Exchange.WebServices.Data

let getService email password : Result<ExchangeService> =
    let credential = WebCredentials(email, password)
    Result.result {
        let! url = ServiceUrl.getServiceUrl email password
        let service = ExchangeService(ExchangeVersion.Exchange2013_SP1, Credentials = credential, Url = url)
        return service
    }
