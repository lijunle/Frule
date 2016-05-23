module User

open Service

let construct (email, password) =
    Result.result {
        let! serviceUrl = getServiceUrl (email, password)
        return { Email = email; Password = password; ServiceUrl = serviceUrl }
    }
