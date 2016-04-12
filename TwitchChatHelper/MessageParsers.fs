module MessageParsers

open System
open System.IO
open System.Text.RegularExpressions

open ConsoleOutHelpers
open MessageTypes


/// RegEx for chanell messages.
let pattern_ChanellMsg = @"^:(?<nickname>\w*)!(?<nameaddr>\w*@\w*\.\w*\.twitch.tv)\s+(?<cmd>\w*)\s+(?<channel>#\w*)\s*:(?<message>.*)$"

/// RegEx for ifo (login) messages
let pattern_info = @"^:(?<twitchaddr>[\w\.]+)\s+(?<code>\d+)\s+(?<nickname>[\w_\.]+)\s+:(?<message>.*)$"

/// RegEx for Ping message.
let pattern_ping = @"^PING :(?<twitchaddr>[\w\.]+)$"

///Match the pattern using a cached compiled Regex
let (|CompiledMatch|_|) pattern input =
    if input = null then None
    else
        let m = Regex.Match(input, pattern, RegexOptions.Compiled)
        if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
        else None

/// Parse message into object.
let parseMessage name =
    match name with
    | CompiledMatch pattern_ChanellMsg [nickname; nameAddr; cmd; channel; message] ->
        let p = Msg {
                Nickname = nickname
                NameAddr = nameAddr
                Cmd = cmd
                Channel = channel
                Message = message
            }
        p
    | CompiledMatch pattern_info [addr; code; nickname; message] ->
        let p = Info {
                TwitchAddr = addr
                Code = code
                Nickname = nickname
                Message = message
            }
        p
    | CompiledMatch pattern_ping [addr] ->
        let p = Ping {
                TwitchAddr = addr
            }
        p
    | _ -> 
        let p = Other name
        p

/// Print message in console.
let PrintMsg message = 
  match message with
    | Msg m -> 
        printfn "%*s | %s" 24 m.Nickname m.Message
    | Info i -> 
        printfn "%15s | %3s | %s | %s" i.TwitchAddr i.Code i.Nickname i.Message
    | Ping p -> 
        printColored colorPing (sprintf "PING from %s" p.TwitchAddr)
    | Other o -> printfn "Oher: %s" o

