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
        Instance = rule;
        Id = rule.Id;
        Name = rule.DisplayName;
        FolderId = rule.Actions.MoveToFolder;
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

let updateName name rule =
    let instance = rule.Instance
    instance.DisplayName <- name
    { rule with Name = name; }

let private toOperation rule =
    SetRuleOperation(rule.Instance) :> RuleOperation

let saveToServer user rules =
    Result.result {
        let service = Service.getService user
        let ruleOperations = rules |> List.map toOperation
        return service.UpdateInboxRules(ruleOperations, true)
    }
