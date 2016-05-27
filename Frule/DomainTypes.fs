[<AutoOpen>]
module DomainTypes

open Microsoft.Exchange.WebServices.Data
open System

type Result<'t> =
    | Success of 't
    | Failure of Exception

type User = {
    Email: string;
    Password: string;
    ServiceUrl: Uri;
}

type Rule = {
    Id: string;
    Name: string;
    FolderId: FolderId;
    FromAddresses: EmailAddress seq;
    SentToAddresses: EmailAddress seq;
}

type Folder = {
    Id: FolderId;
    Name: string;
    Children: Folder list;
    Rules: Rule list;
}
