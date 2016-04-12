﻿// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open System
open System.IO
open System.Text
open System.Net
open System.Net.Sockets
open System.Text.RegularExpressions

open MyCfg
open ConsoleOutHelpers
open MessageTypes
open MessageParsers
open TwitchCommands

let channelRita = "#ritagamer2"
let channelSlide = "#p0wersl1de"
let channelLagyAndel = "#lady__angel"
let channelMargareth = "#margaret_hilda_thatcher"

// File writer

let testWrite(line:string) = async {
    use sw = new StreamWriter(new FileStream("twitch_log_other.txt",  FileMode.Append, FileAccess.Write, FileShare.Write, bufferSize= 4096, useAsync= true))
    do! sw.WriteLineAsync(line) |>  Async.AwaitTask
}
//testWrite z |> Async.Start


// http://stackoverflow.com/questions/36116231/using-writelineasync-in-f
[<EntryPoint>]
let main argv = 
    Console.OutputEncoding <- Encoding.Unicode    
    printf "%s \n" "Работаю!"  // Must print nice, or utf not set properly! "Working" iin russian.

    if (String.IsNullOrWhiteSpace(nick)) then printfn "Nickname is empty"; exit 5
    if (String.IsNullOrWhiteSpace(oauth)) then printfn "Oauth is empty"; exit 5

    SendPass oauth
    SendNick nick
    SendJoin channelMargareth

    use streamWriter = new StreamWriter("twitch_log.txt" )
    streamWriter.AutoFlush <- true
    

    while not irc_reader.EndOfStream do
        let msgText = ReceiveMessage()
        
        let msg = parseMessage msgText
        PrintMsg msg
        
        match msg with
        | Msg m -> 
            // TODO: msg as type.
            match m.Cmd with
            | "PRIVMSG" ->
                // Log message only.
                try
                    streamWriter.WriteLine(m.Message)
                finally
                    streamWriter.Flush()
            | "JOIN" ->
                printColored colorPing (sprintf "JOIN %s" m.Channel)
            | _ -> testWrite msgText |> Async.Start  // async to err file (full line), should not be many.
            

        | Ping p -> SendPong()
        | Other o -> testWrite msgText |> Async.Start  // async to err file (full line), should not be many.
        | _  -> ()

    0 // return an integer exit code
