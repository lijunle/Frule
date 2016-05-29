[<AutoOpen>]
module DomainTypes

type Store = {
    Rules: SuperEvent<Rule list>;
    SavedRules: SuperEvent<Rule list>;
    SelectedRule: SuperEvent<Rule>;
    SelectedFolder: SuperEvent<Folder>;
}
