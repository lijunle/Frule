module Store

let create () = {
    InboxFolder = SuperEvent<Folder list>([Folder.Loading]);
    Rules = SuperEvent<Rule list>([]);
    SavedRules = SuperEvent<Rule list>([]);
    SelectedRule = SuperEvent<Rule>(Rule.Zero);
    SelectedFolder = SuperEvent<Folder>(Folder.Loading);
    SaveButtonEnabled = SuperEvent<bool>(false)
}

let loadAsync store user = async {
    do! Async.SwitchToThreadPool () // TODO Make native async operators and avoid this

    let folderResult = User.getInboxFolder user
    store.InboxFolder.Trigger (Result.orDefault folderResult Folder.LoginError |> List.singleton)

    let rulesResult = User.getRules user
    let loadedRules = Result.orDefault rulesResult []
    store.Rules.Trigger loadedRules
    store.SavedRules.Trigger loadedRules
}

let compare list1 list2 =
    List.compareWith (fun r1 r2 -> if r1 = r2 then 0 else 1) list1 list2

let getDiffRules list1 list2 =
    list2 |> List.filter (fun r -> not (List.contains r list1))
