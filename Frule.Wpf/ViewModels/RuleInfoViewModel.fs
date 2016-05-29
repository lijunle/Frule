namespace ViewModels

open FSharp.ViewModule

type RuleInfoViewModel(updateRule : (Rule -> unit), rule : Rule) =
    inherit ViewModelBase()

    let updateRuleName (ruleName : string) =
        Rule.updateName ruleName rule |> updateRule

    static member Zero = RuleInfoViewModel(ignore, Rule.Zero)
    static member Create f r = RuleInfoViewModel(f, r)

    member internal __.Rule = rule

    member __.Name = rule.Name
    member __.FromAddresses = rule.FromAddresses
    member __.SentToAddresses = rule.SentToAddresses

    member this.ChangeNameCommand = this.Factory.CommandSyncParam(updateRuleName)
