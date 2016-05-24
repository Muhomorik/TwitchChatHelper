module ConcurrentLogSettingsTest

open NUnit.Framework
open FsUnit

open MailboxLogger

[<Test>]
let``test  ConcurrentLogSettings Add``()=
    
    let set = new ConcurrentLogSettings()
    
    set.Count |> should equal 0
    set.Add "k1" "v1"
    set.Count |> should equal 1

[<Test>]
let``test  ConcurrentLogSettings Remove``()=
    
    let set = new ConcurrentLogSettings()
    
    set.Count |> should equal 0
    set.Add "k1" "v1"
    set.Add "k2" "v2"
    set.Count |> should equal 2
    set.Remove "k1"
    set.Count |> should equal 1

[<Test>]
let``test  ConcurrentLogSettings Tryfind``()=
    
    let set = new ConcurrentLogSettings()
    
    set.Count |> should equal 0
    set.Add "k1" "v1"
    set.Add "k2" "v2"

    set.Tryfind "k1" |> should equal (Some("v1"))
    set.Tryfind "k2" |> should equal (Some("v2"))