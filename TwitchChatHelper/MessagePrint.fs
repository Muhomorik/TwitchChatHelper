module MessagePrint

open ConsoleOutHelpers
open MessageTypes

/// Print message in console.
let PrintMsg message = 
  match message with
    | ChanellMessage m -> 
        printfn "%*s | %s" 24 m.Nickname m.Message
    | Ping -> 
        printColored colorPing "PING"
    | SuccConnection i -> 
        printfn "Info: %15s | %3s | %s | %s" i.TwitchGroup i.Code i.Nickname i.Message
    | ChannelJoin p -> 
        printColored colorJoin (sprintf "JOIN | %s | %s | %s" p.Nickname p.NicknameAlterative p.Channel)
    | ChannelNicknames p -> 
        printColored colorInfo (sprintf "Nicknames | %s | %3s | %s | %s | %s" p.Nickname p.Code p.NicknameJoin p.Channel p.Nicknames)
    | ChannelNicknamesEnd p -> 
        printColored colorInfo (sprintf "/NAMES | %s | %3s | %s | %s" p.NameAddr p.Code p.Nickname p.Channel)
    | ChannelLeave p -> 
        printColored colorLeave (sprintf "LEAVE | %s | %s " p.Channel p.Nickname)
    | InvalidCommand p -> 
        printColored colorInfo (sprintf "LEAVE | %s | %s " p.Code p.Message)

    // Capabilities.
    | MembershipAck a -> 
        printColored colorReqAck (sprintf "Req Membership ACK | %b " a)    
    | MembershipMode a -> 
        printColored colorInfo (sprintf "Membership MODE | %s | %b | %s"  a.Channel a.Mode a.Username)

    // Commands
    | CommandsAck a -> 
        printColored colorReqAck (sprintf "Req Commands ACK | %b " a)

    | Other o -> printfn "Other: %s" o