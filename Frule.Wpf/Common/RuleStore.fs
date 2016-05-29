module RuleStore

let Zero = {
    Rules = [];
}

let compare s1 s2 =
    let list1 = s1.Rules
    let list2 = s2.Rules
    List.compareWith (fun r1 r2 -> if r1 = r2 then 0 else 1) list1 list2

let getDiffRules s1 s2 =
    let list1 = s1.Rules
    let list2 = s2.Rules
    list2 |> List.filter (fun r -> not (List.contains r list1))
