module MessagePrint

open ConsoleOutHelpers
open MessageTypes

/// Print message in console.
let PrintMsg message = 
  match message with
    | Msg m -> 
        printfn "%*s | %s" 24 m.Nickname m.Message
    | Info i -> 
        printfn "Info: %15s | %3s | %s | %s" i.TwitchAddr i.Code i.Nickname i.Message
    | Ping p -> 
        printColored colorPing (sprintf "PING from %s" p.TwitchAddr)
    | ChanellJoin p -> 
        printColored colorJoin (sprintf "JOIN | %s | %s | %s | %s" p.Nickname p.NameAddr p.Cmd p.Channel)
    | Nicknames p -> 
        printColored colorInfo (sprintf "Nicknames | %s | %3s | %s | %s | %s" p.TwitchAddr p.Code p.Nickname1 p.Channel p.Nickname2)
    | NicknamesEnd p -> 
        printColored colorInfo (sprintf "/NAMES | %s | %3s | %s | %s | %s" p.NameAddr p.Code p.Nickname p.Channel p.Message)
    | Other o -> printfn "Oher: %s" o