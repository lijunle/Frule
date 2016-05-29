namespace ViewModels

open FSharp.ViewModule

type RuleInfoViewModel(store : Store) as this =
    inherit ViewModelBase()

    let currentRules = store.Rules
    let selectedRule = store.SelectedRule
    let selectedFolder = store.SelectedFolder

    let updateRule (rule : Rule) =
        let updateTargetRule (r : Rule) = if r.Id = selectedRule.Value.Id then rule else r
        let newRules = currentRules.Value |> List.map updateTargetRule
        currentRules.Trigger newRules

    let updateRuleName (ruleName : string) =
        Rule.updateName ruleName selectedRule.Value |> updateRule

    do
        selectedRule.Publish.Add (fun _ ->
            this.RaisePropertyChanged(<@ this.Name @>)
            this.RaisePropertyChanged(<@ this.FromAddresses @>)
            this.RaisePropertyChanged(<@ this.SentToAddresses @>))

        selectedFolder.Publish.Add(fun _ ->
            selectedRule.Trigger Rule.Zero)

    member __.Name with get() = selectedRule.Value.Name
    member __.FromAddresses with get() = selectedRule.Value.FromAddresses
    member __.SentToAddresses with get() = selectedRule.Value.SentToAddresses

    member this.ChangeNameCommand = this.Factory.CommandSyncParam(updateRuleName)
