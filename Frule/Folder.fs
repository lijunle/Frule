module Folder

open Microsoft.Exchange.WebServices.Data

let Loading = {
    Id = null;
    Name = "Loading";
    Children = [];
}

let LoginError = {
    Id = null;
    Name= "Login Error";
    Children= [];
}

let buildFolderHierarchy (inboxId, inboxName) (folders : Folder list) =
    let rec loop (folderId, folderName) =
        let children =
            folders
            |> List.filter (fun folder -> folder.ParentFolderId = folderId)
            |> List.map (fun folder -> loop (folder.Id, folder.DisplayName))

        {
            Id = folderId;
            Name = folderName;
            Children = children;
        }

    loop (inboxId, inboxName)

let getFolders (service : ExchangeService) =
    try
        let inboxWellKnownId = FolderId(WellKnownFolderName.Inbox)
        let propertySet = PropertySet.FirstClassProperties
        let folders =
            service.SyncFolderHierarchy(inboxWellKnownId, propertySet, null)
            |> Seq.map (fun changed -> changed.Folder)
            |> List.ofSeq
        Success folders
    with e ->
        Failure e

let getFolderHierarchy (service : ExchangeService) =
    Result.result {
        let! folders = getFolders service
        let inboxId = folders.[0].ParentFolderId
        let inboxName = WellKnownFolderName.Inbox.ToString()
        return buildFolderHierarchy (inboxId, inboxName) folders
    }
