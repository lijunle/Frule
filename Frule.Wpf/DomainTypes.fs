[<AutoOpen>]
module DomainTypes

type RuleStore = {
    Modified: bool;
    Model: Rule list;
}
