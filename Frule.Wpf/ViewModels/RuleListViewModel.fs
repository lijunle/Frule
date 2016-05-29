namespace ViewModels

open FSharp.ViewModule

type RuleListViewModel(selectRule : (Rule -> unit), rules : Rule list) =
    inherit ViewModelBase()

    static member Zero = RuleListViewModel(ignore, [])
    static member Create f x = RuleListViewModel(f, x)

    member __.List = rules

    member this.SelectRuleCommand = this.Factory.CommandSyncParam(selectRule)
