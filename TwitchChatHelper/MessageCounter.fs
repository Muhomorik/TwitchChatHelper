module MessageCounter

// Message counters.

open System
open System.Threading
open System.Collections.Concurrent

// TODO: config parameters in/as file.

/// Message limit for sender, msg/30s.
/// TODO: move to main(), rethink structure.
[<Literal>]
let LimitSender = 20

/// Convert DateTime to Unix timestamp
let dateTimeToUnixTime (dt:DateTime) =  
    let sub = new DateTime(1970, 1, 1)
    let unixTimestamp  = (dt.Subtract(sub)).TotalSeconds
    (int)unixTimestamp

/// Get current time as Unix timestamp.
let currentUnixTime() = dateTimeToUnixTime DateTime.UtcNow 

/// Dead-out time (now-30s).
let deadUnixTime() = dateTimeToUnixTime (DateTime.UtcNow - new TimeSpan(0, 0, 30)) 

/// Add command to counter.
let cmdEnqueue (cmdCounter:ConcurrentQueue<int>) = cmdCounter.Enqueue <| currentUnixTime()

/// Check if next item in cmdCounter is old without removing it.
let isOld (cmdCounter:ConcurrentQueue<int>) (unixStamOld:int)= 
    let ok, unixStamp =  cmdCounter.TryPeek()
    
    match ok with
    | true -> 
        match unixStamp with
        | curr when curr <= unixStamOld -> true
        | _ -> false
    | false -> false

/// Remove old message count from counter.
let cleanOldCmd (cmdCounter:ConcurrentQueue<int>)(unixStampNow:int) =
    while (isOld cmdCounter unixStampNow) do
        cmdCounter.TryDequeue() |> ignore

let cmdCounterCount (cmdCounter:ConcurrentQueue<int>) = cmdCounter.Count

/// Wait untill sender lock is out.
let waitForUnlock (cmdCounter:ConcurrentQueue<int>)(limit:int)(unixTime:int) =
    while ((cmdCounterCount cmdCounter) > limit) do
        cleanOldCmd cmdCounter unixTime
        printfn "Waiting for sender to unlock..."
        Thread.Sleep(800)

/// Waits 30sec, blocking.
let waitForUnlock30 (cmdCounter:ConcurrentQueue<int>)(unixTime:int) = waitForUnlock cmdCounter LimitSender unixTime