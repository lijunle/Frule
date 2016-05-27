﻿module User

open Service

let construct (email, password) =
    Result.result {
        let! serviceUrl = getServiceUrl (email, password)
        return { Email = email; Password = password; ServiceUrl = serviceUrl }
    }

let getInboxFolder user =
    Result.result {
        let service = getService user
        let! inboxFolder = Folder.getFolderHierarchy service
        return inboxFolder
    }
