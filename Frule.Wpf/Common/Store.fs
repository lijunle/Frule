module Store

let create () = {
    SelectedRule = SuperEvent<Rule>(Rule.Zero);
}
