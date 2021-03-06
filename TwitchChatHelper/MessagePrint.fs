﻿module MessagePrint

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
        printColored colorJoin (sprintf "JOIN | %s | %s | %s" p.Channel p.NicknameAlterative p.Nickname  )
    | ChannelNicknames p -> 
        printColored colorInfo (sprintf "Nicknames | %s | %3s | %s | %s | %s" p.Channel  p.Nickname p.Code p.NicknameJoin p.Nicknames)
    | ChannelNicknamesEnd p -> 
        printColored colorInfo (sprintf "/NAMES | %s | %3s | %s | %s" p.Channel p.NameAddr p.Code p.Nickname )
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
    | CommandsNotice a -> 
        printColored colorInfo (sprintf "Notice | %s " a.Message)    
    
    | CommandsHostTargetStart a -> 
        printColored colorInfo (sprintf "HOSTTARGET Start | Hosting: %s | Target: %s | Viewvers: %s" a.ChannelHosting a.ChannelTarget a.Number)
    | CommandsHostTargetStop a -> 
        printColored colorInfo (sprintf "HOSTTARGET Stop| Hosting: %s | Viewvers: %s" a.ChannelHosting a.Number)    
    
    | CommandsClearChatUser a -> 
        printColored colorClearChatUser (sprintf "Clear chat | %s | %s" a.Channel a.Nickname)    
    | CommandsClearChat a -> 
        printColored colorClearChat (sprintf "Clear chat | %s " a.Channel)    
        
    | CommandsUserstate a -> 
        printColored colorInfo (sprintf "UserState | %s " a.Channel)        
    | CommandsReconnect -> 
        printColored colorReconnect "RECONNECT"   
    | CommandsRoomstate a -> 
        printColored colorInfo (sprintf "ROOMSTATE | %s " a.Channel) 
    
    | Other o -> printfn "Other: %s" o