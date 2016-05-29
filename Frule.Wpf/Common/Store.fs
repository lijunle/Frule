module Store

let create () = {
    Rules' = SuperEvent<Rule list>([]);
    SavedRules = SuperEvent<Rule list>([]);
    SelectedRule = SuperEvent<Rule>(Rule.Zero);
    SelectedFolder = SuperEvent<Folder>(Folder.Loading);
}

let compare list1 list2 =
    List.compareWith (fun r1 r2 -> if r1 = r2 then 0 else 1) list1 list2

let getDiffRules list1 list2 =
    list2 |> List.filter (fun r -> not (List.contains r list1))
