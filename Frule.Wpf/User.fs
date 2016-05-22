module User

open System.IO

let fileName = "Frule.dat"

let get () =
    try
        let data = File.ReadAllText(fileName).Split('\t')
        let email = data.[0]
        let password = data.[1]
        (email, password)
    with _ ->
        ("", "")

let set (email, password) =
    try
        let data = sprintf "%s\t%s" email password
        File.WriteAllText(fileName, data)
    with _ ->
        ()
