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

type RuleListViewModel(store : Store) as this =
    inherit ViewModelBase()

    let savedRules = store.SavedRules
    let currentRules = store.Rules
    let selectedFolder = store.SelectedFolder
    let selectedRule = store.SelectedRule

    let constructList () =
        RuleItemViewModel.constructList savedRules.Value currentRules.Value selectedFolder.Value

    do
        SuperEvent.any4 savedRules currentRules selectedFolder selectedRule
            |> Event.add (fun _ ->
                this.RaisePropertyChanged(<@ this.List @>)
                this.RaisePropertyChanged(<@ this.SelectedRuleId @>))

    member __.List with get() = constructList()
    member __.SelectedRuleId with get() = selectedRule.Value.Id

    member this.SelectRuleCommand = this.Factory.CommandSyncParam(selectedRule.Trigger)
