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

type RuleListViewModel(selectRule : (Rule -> unit), rules : RuleItemViewModel list) =
    inherit ViewModelBase()

    static member Zero = RuleListViewModel(ignore, [])
    static member Create f x = RuleListViewModel(f, x)

    static member FilterRules (savedStore, currentStore, selectedFolder) =
        let modifiedRules = RuleStore.getDiffRules savedStore currentStore
        let displayRules = currentStore.Rules |> List.filter (fun r -> r.FolderId = selectedFolder.Id)
        let displayModifiers = displayRules |> List.map (fun r -> List.contains r modifiedRules)
        List.zip displayRules displayModifiers

    member __.List = rules

    member this.SelectRuleCommand = this.Factory.CommandSyncParam(selectRule)
