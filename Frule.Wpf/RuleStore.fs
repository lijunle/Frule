module RuleStore

let Zero = {
    Model = [];
}

let private comparer (r1 : Rule) (r2 : Rule) =
    if r1 = r2 then 0 else 1

let compare s1 s2 =
    let list1 = s1.Model
    let list2 = s2.Model
    List.compareWith comparer list1 list2

let getDiffRules s1 s2 =
    let list1 = s1.Model
    let list2 = s2.Model
    List.zip list1 list2
    |> List.filter (fun (r1, r2) -> comparer r1 r2 <> 0)
    |> List.map (fun (r1, r2) -> r2)
