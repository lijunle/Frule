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

type RuleListViewModel(selectRule : (Rule -> unit), rules : RuleItemViewModel list, selectedRule : Rule) =
    inherit ViewModelBase()

    static member Zero = RuleListViewModel(ignore, [], Rule.Zero)

    static member Create f (savedStore, currentStore, selectedFolder, selectedRule : Rule) =
        let list = RuleItemViewModel.constructList savedStore currentStore selectedFolder
        RuleListViewModel(f, list, selectedRule)

    member __.List = rules
    member __.SelectedRuleId = selectedRule.Id

    member this.SelectRuleCommand = this.Factory.CommandSyncParam(selectRule)
