namespace ViewModels

open FSharp.ViewModule

type RuleListViewModel(rules : Rule list) =
    inherit ViewModelBase()

    static member Zero = RuleListViewModel([])

    member __.List = rules
