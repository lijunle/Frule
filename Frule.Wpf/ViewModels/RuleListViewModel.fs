namespace ViewModels

open FSharp.ViewModule

[<AutoOpen>]
module RuleList =
    type RuleItemViewModel = {
        Rule: Rule;
        FontWeight: string;
    }

module RuleItemViewModel =
    let create (rule, modified) = {
        Rule = rule;
        FontWeight = if modified then "Bold" else "Normal";
    }

    let constructList savedRules currentRules selectedFolder =
        let modifiedRules = Store.getDiffRules savedRules currentRules
        let displayRules = currentRules |> List.filter (fun r -> r.FolderId = selectedFolder.Id)
        let displayModifiers = displayRules |> List.map (fun r -> List.contains r modifiedRules)
        let displayPairs = List.zip displayRules displayModifiers
        displayPairs |> List.map create

type RuleListViewModel(store : Store) =
    inherit ViewModelBase()

    let savedRules = store.SavedRules.Value
    let currentRules = store.Rules'.Value
    let selectedRule = store.SelectedFolder.Value

    member __.List = RuleItemViewModel.constructList savedRules currentRules selectedRule
    member __.SelectedRuleId = store.SelectedRule.Value.Id

    member this.SelectRuleCommand = this.Factory.CommandSyncParam(store.SelectedRule.Trigger)
