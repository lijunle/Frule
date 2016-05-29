namespace ViewModels

open FSharp.ViewModule

type RuleInfoViewModel(rule : Rule) =
    inherit ViewModelBase()

    static member Zero = RuleInfoViewModel(Rule.Zero)

    member internal __.Rule = rule

    member __.Name = rule.Name
    member __.FromAddresses = rule.FromAddresses
    member __.SentToAddresses = rule.SentToAddresses
