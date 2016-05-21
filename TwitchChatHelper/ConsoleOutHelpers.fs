module ConsoleOutHelpers

open System

let colorInfo = ConsoleColor.DarkYellow

let colorReq = ConsoleColor.Green
let colorReqAck = ConsoleColor.Green

let colorPing = ConsoleColor.DarkMagenta
let colorPong = ConsoleColor.DarkMagenta

let colorJoin = ConsoleColor.Cyan
let colorLeave = ConsoleColor.Cyan

let colorClearChatUser = ConsoleColor.Yellow

let printColored c (s:string) = 
    let old = System.Console.ForegroundColor 
    try 
        System.Console.ForegroundColor <- c;
        printfn "%s" s
    finally
        System.Console.ForegroundColor <- old