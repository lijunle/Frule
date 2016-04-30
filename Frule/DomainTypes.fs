[<AutoOpen>]
module DomainTypes

open Microsoft.Exchange.WebServices.Data

type Rule = {
    Id: string;
    Name: string;
    Folder: FolderId;
    FromAddresses: EmailAddress seq;
    SentToAddresses: EmailAddress seq;
}
