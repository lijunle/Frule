[<AutoOpen>]
module DomainTypes

type DialogState =
    | Open
    | Close

type Store = {
    User: SuperEvent<User option>;
    InboxFolder: SuperEvent<Folder list>;
    Rules: SuperEvent<Rule list>;
    SavedRules: SuperEvent<Rule list>;
    SelectedRule: SuperEvent<Rule>;
    SelectedFolder: SuperEvent<Folder>;
    SaveButtonEnabled: SuperEvent<bool>;
    LoginDialogState: SuperEvent<DialogState>;
}
