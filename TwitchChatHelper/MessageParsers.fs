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

/// RegEx. Twitch JOIN message response.
let pattern_ChanellJoin = @"^:(?<nickname>\w*)!(?<nameaddr>[\w\.@]+)\s+(?<cmd>\w*)\s+(?<channel>#\w*)$"

/// Twitch JOIN response with nicknames, code 353.
let pattern_ChanellNicknames = @"^:(?<nameaddr>[\w\.]+)\s+(?<code>\w*)\s+(?<nickname1>[\w_\.]+)\s+=\s+(?<channel>#\w*)\s+:(?<nickname2>[\w_\.]+)$"

/// Twitch end of nicknames, code 366.
let pattern_ChanellNicknamesEnd = @"^:(?<nameaddr>[\w\.]+)\s+(?<code>\w*)\s+(?<nickname>[\w_\.]+)\s+(?<channel>#\w*)\s+:(?<message>[\w \/]+)$"


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
    | CompiledMatch pattern_ChanellJoin [nickname; nameaddr; cmd; chanell] ->
        let p = ChanellJoin {
                Nickname = nickname
                NameAddr = nameaddr
                Cmd = cmd
                Channel = chanell
            }
        p
    | CompiledMatch pattern_ChanellNicknames [addr; code; nickname1; chanell; nickname2] ->
        let p = Nicknames {
                TwitchAddr = addr
                Code = code
                Nickname1 = nickname1
                Channel = chanell
                Nickname2 = nickname2
            }
        p
    | CompiledMatch pattern_ChanellNicknamesEnd [nameaddr; code; nickname; chanell; message] ->
        let p = NicknamesEnd {
                NameAddr = nameaddr
                Code = code
                Nickname = nickname
                Channel = chanell
                Message = message
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
        printfn "Info: %15s | %3s | %s | %s" i.TwitchAddr i.Code i.Nickname i.Message
    | Ping p -> 
        printColored colorPing (sprintf "PING from %s" p.TwitchAddr)
    | ChanellJoin p -> 
        printColored colorInfo (sprintf "JOIN | %s | %s | %s | %s" p.Nickname p.NameAddr p.Cmd p.Channel)
    | Nicknames p -> 
        printColored colorInfo (sprintf "Nicknames | %s | %3s | %s | %s | %s" p.TwitchAddr p.Code p.Nickname1 p.Channel p.Nickname2)
    | NicknamesEnd p -> 
        printColored colorInfo (sprintf "/NAMES | %s | %3s | %s | %s | %s" p.NameAddr p.Code p.Nickname p.Channel p.Message)
    | Other o -> printfn "Oher: %s" o

