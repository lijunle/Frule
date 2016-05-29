module User

open System
open System.IO

let fileName = "Frule.dat"

let get () =
    try
        let data = File.ReadAllText(fileName).Split('\t')
        let email = data.[0]
        let password = data.[1]
        let serviceUrl = data.[2]
        { Email = email; Password = password; ServiceUrl = Uri(serviceUrl) }
    with _ ->
        { Email = "invalid"; Password = ""; ServiceUrl = Uri("http://invalid") }

let set user =
    try
        let { Email = email; Password = password; ServiceUrl = serviceUrl } = user
        let data = sprintf "%s\t%s\t%s" email password (serviceUrl.ToString())
        File.WriteAllText(fileName, data)
    with _ ->
        ()
