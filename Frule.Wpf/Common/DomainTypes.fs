[<AutoOpen>]
module DomainTypes

type Store = {
    InboxFolder: SuperEvent<Folder list>;
    Rules: SuperEvent<Rule list>;
    SavedRules: SuperEvent<Rule list>;
    SelectedRule: SuperEvent<Rule>;
    SelectedFolder: SuperEvent<Folder>;
    SaveButtonEnabled: SuperEvent<bool>;
}
