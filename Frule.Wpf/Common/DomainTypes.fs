[<AutoOpen>]
module DomainTypes

type RuleStore = {
    Rules: Rule list;
}

type Store = {
    SelectedRule: SuperEvent<Rule>;
}
