module Folder

open Microsoft.Exchange.WebServices.Data

let buildFolderHierarchy (inboxId, inboxName) (folders : Folder list) (rules : DomainTypes.Rule list) =
    let rec loop (folderId, folderName) =
        let children =
            folders
            |> List.filter (fun folder -> folder.ParentFolderId = folderId)
            |> List.map (fun folder -> loop (folder.Id, folder.DisplayName))

        let targetRules =
            rules
            |> List.filter (fun rule -> rule.Folder = folderId)

        {
            Id = folderId;
            Name = folderName;
            Children = children;
            Rules = targetRules;
        }

    loop (inboxId, inboxName)

let getFolders email password serviceUrl =
    let credential = WebCredentials(email, password)
    let service =
        ExchangeService(ExchangeVersion.Exchange2013_SP1, Credentials = credential, Url = serviceUrl)
    let inboxWellKnownId = FolderId(WellKnownFolderName.Inbox)
    let propertySet = PropertySet.FirstClassProperties
    let folders =
        service.SyncFolderHierarchy(inboxWellKnownId, propertySet, null)
        |> Seq.map (fun changed -> changed.Folder)
        |> List.ofSeq
    folders

let getFolderHierarchy email password serviceUrl rules =
    let folders = getFolders email password serviceUrl
    let inboxId = folders.[0].ParentFolderId
    let inboxName = WellKnownFolderName.Inbox.ToString()
    buildFolderHierarchy (inboxId, inboxName) folders rules
