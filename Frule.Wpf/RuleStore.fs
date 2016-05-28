module RuleStore

let Zero = {
    Modified = false;
    Model = [];
}

let compare s1 s2 =
    let list1 = s1.Model
    let list2 = s2.Model
    let comparer (r1 : Rule) (r2 : Rule) =
        if { r1 with Modified = false } = { r2 with Modified = false }
        then 0
        else 1
    List.compareWith comparer list1 list2
