[<AutoOpen>]
module DomainTypes

type RuleStore = {
    Rules: Rule list;
}

type Store = {
    Rules': SuperEvent<Rule list>; // TODO pending to change back to Rules
    SavedRules: SuperEvent<Rule list>;
    SelectedRule: SuperEvent<Rule>;
}
