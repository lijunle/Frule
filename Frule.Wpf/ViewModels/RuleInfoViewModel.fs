namespace ViewModels

open FSharp.ViewModule

type RuleInfoViewModel(store : Store) =
    inherit ViewModelBase()

    let selectedRule = store.SelectedRule.Value

    let updateRule (rule : Rule) =
        let updateTargetRule (r : Rule) = if r.Id = store.SelectedRule.Value.Id then rule else r
        let newRules = store.Rules'.Value |> List.map updateTargetRule
        store.Rules'.Trigger newRules

    let updateRuleName (ruleName : string) =
        Rule.updateName ruleName selectedRule |> updateRule

    member __.Name = selectedRule.Name
    member __.FromAddresses = selectedRule.FromAddresses
    member __.SentToAddresses = selectedRule.SentToAddresses

    member this.ChangeNameCommand = this.Factory.CommandSyncParam(updateRuleName)
