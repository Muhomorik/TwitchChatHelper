module MessageCounterTest

open NUnit.Framework
open FsUnit
open System
open MessageCounter
open System.Collections.Concurrent

[<Test>]
let``test dateTimeToUnixTime``()=
    
    let dt = new DateTime(2000, 1, 2, 23, 1, 30)
    let unix = dateTimeToUnixTime dt

    unix |> should equal 946854090

[<Test>]
let``test deadUnixTime``()=
    
    let dt = new DateTime(2000, 1, 2, 23, 1, 30)
    let unixNow = dateTimeToUnixTime dt
    let unixOld = deadUnixTime dt

    unixNow - unixOld |> should equal 30

[<Test>]
let``test cmdEnqueue``()=
    
    let cmdCounter = ConcurrentQueue<int>()

    cmdCounter.Count |> should equal 0
    cmdEnqueue cmdCounter
    cmdCounter.Count |> should equal 1
           
[<Test>]
let``test isOld true``()=
    
    let cmdCounter = ConcurrentQueue<int>()

    cmdCounter.Enqueue <| 10
    
    let old = isOld cmdCounter 12

    old |> should be True

[<Test>]
let``test isOld false``()=
    
    let cmdCounter = ConcurrentQueue<int>()

    cmdCounter.Enqueue <| 10
    
    let old = isOld cmdCounter 8

    old |> should be False

   
[<Test>]
let``test cleanOldCmd``()=
    
    let cmdCounter = ConcurrentQueue<int>()

    cmdCounter.Enqueue <| 1
    cmdCounter.Enqueue <| 2
    cmdCounter.Enqueue <| 3
    cmdCounter.Enqueue <| 4
    cmdCounter.Enqueue <| 5
    cmdCounter.Enqueue <| 6

    cmdCounter.Count |> should equal 6

    cleanOldCmd cmdCounter 4 6
    cmdCounter.Count |> should equal 2    