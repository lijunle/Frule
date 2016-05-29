namespace ViewModels

open FSharp.ViewModule

type RuleListViewModel(selectRule : (Rule -> unit), rules : (Rule * bool) list) =
    inherit ViewModelBase()

    static member Zero = RuleListViewModel(ignore, [])
    static member Create f x = RuleListViewModel(f, x)

    static member FilterRules (savedStore, currentStore, selectedFolder) =
        let modifiedRules = RuleStore.getDiffRules savedStore currentStore
        let displayRules = currentStore.Rules |> List.filter (fun r -> r.FolderId = selectedFolder.Id)
        let displayModifiers = displayRules |> List.map (fun r -> List.contains r modifiedRules)
        List.zip displayRules displayModifiers

    member __.List = rules |> List.map (fun (r, m) -> (r, if m then "Bold" else "Normal"))

    member this.SelectRuleCommand = this.Factory.CommandSyncParam(selectRule)
