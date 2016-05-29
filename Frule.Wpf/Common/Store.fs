module Store

let create () = {
    User = SuperEvent<User option>(User.get());
    InboxFolder = SuperEvent<Folder list>([Folder.Loading]);
    Rules = SuperEvent<Rule list>([]);
    SavedRules = SuperEvent<Rule list>([]);
    SelectedRule = SuperEvent<Rule>(Rule.Zero);
    SelectedFolder = SuperEvent<Folder>(Folder.Loading);
    SaveButtonEnabled = SuperEvent<bool>(false);
}

let loadAsync store = async {
    do! Async.SwitchToThreadPool () // TODO Make native async operators and avoid this

    match store.User.Value with
    | None -> ()
    | Some user ->
        let folderResult = User.getInboxFolder user
        let inboxFolder = Result.orDefault folderResult Folder.LoginError
        store.InboxFolder.Trigger (inboxFolder |> List.singleton)

        let rulesResult = User.getRules user
        let loadedRules = Result.orDefault rulesResult []
        store.Rules.Trigger loadedRules
        store.SavedRules.Trigger loadedRules
}

let compare list1 list2 =
    List.compareWith (fun r1 r2 -> if r1 = r2 then 0 else 1) list1 list2

let getDiffRules list1 list2 =
    list2 |> List.filter (fun r -> not (List.contains r list1))
