namespace ViewModels

type MainWindowViewModel(store : Store) =
    member __.InboxFolder = FolderListViewModel store
    member __.DisplayRules = RuleListViewModel store
    member __.SelectedRule = RuleInfoViewModel store
    member __.UserInfo = UserInfoViewModel store
