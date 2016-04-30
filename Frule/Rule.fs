module Rule

open Microsoft.Exchange.WebServices.Data

let isRule (rule : Rule) =
    rule.Actions <> null &&
    rule.Actions.MoveToFolder <> null &&
    rule.Conditions <> null &&
    rule.Conditions.FromAddresses <> null &&
    rule.Conditions.SentToAddresses <> null

let toRule (rule : Rule) =
    {
        Id = rule.Id;
        Name = rule.DisplayName;
        Folder = rule.Actions.MoveToFolder;
        FromAddresses = rule.Conditions.FromAddresses;
        SentToAddresses = rule.Conditions.SentToAddresses;
    }

let getRules email password serviceUrl =
    let credential = WebCredentials(email, password)
    let service = ExchangeService(ExchangeVersion.Exchange2013_SP1, Credentials = credential)
    service.Url <- serviceUrl
    service.GetInboxRules()
    |> Seq.filter isRule
    |> Seq.map toRule
