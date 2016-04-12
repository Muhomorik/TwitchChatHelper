module ConsoleOutHelpers

open System

let colorInfo = ConsoleColor.DarkYellow
let colorPing = ConsoleColor.DarkMagenta
let colorJoin = ConsoleColor.Cyan

let printColored c (s:string) = 
    let old = System.Console.ForegroundColor 
    try 
        System.Console.ForegroundColor <- c;
        printfn "%s" s
    finally
        System.Console.ForegroundColor <- old