module Rule

open Microsoft.Exchange.WebServices.Data

let isRule (rule : Rule) =
    isNull rule.Actions ||
    isNull rule.Actions.MoveToFolder ||
    isNull rule.Conditions ||
    isNull rule.Conditions.FromAddresses ||
    isNull rule.Conditions.SentToAddresses
    |> not

let toRule (rule : Rule) =
    {
        Id = rule.Id;
        Name = rule.DisplayName;
        Folder = rule.Actions.MoveToFolder;
        FromAddresses = rule.Conditions.FromAddresses;
        SentToAddresses = rule.Conditions.SentToAddresses;
    }

let getRules (service : ExchangeService) =
    try
        service.GetInboxRules()
        |> Seq.filter isRule
        |> Seq.map toRule
        |> Success
    with e ->
        Failure e
