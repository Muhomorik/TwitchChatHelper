module MessageParsersTest

open System
open System.IO
open System.Text.RegularExpressions

open NUnit.Framework
open FsUnit

open MessageTypes
open MessageParsers

// ORDER tests, use examles from manual

// Test EN locale.
let msg_en = ":nickname!nickname@nickname.tmi.twitch.tv PRIVMSG #gamer2 :So is this the stream with giveaways?"

let f_Nicname = "nickname"
let f_NameAddr = "nickname@nickname.tmi.twitch.tv"
let f_Cmd = "PRIVMSG"
let f_Channel = "#gamer2"
let f_MessageStarts = "So"

// Test russian locale.
let msg_ru = ":nightbot!nightbot@nightbot.tmi.twitch.tv PRIVMSG #lady__angel :хочешь порадовать девушку или выразить благодарность"

let nb_Nicname = "nightbot"
let nb_NameAddr = "nightbot@nightbot.tmi.twitch.tv"
let nb_Cmd = "PRIVMSG"
let nb_Channel = "#lady__angel"
let nb_MessageStarts = "хочешь"

// Hello messages
let msg_hello = ":tmi.twitch.tv 001 nickname :Welcome, GLHF!"

let msgHello_TwitchAddr = "tmi.twitch.tv"
let msgHello_Code = "001"
let msgHello_Nickname = "nickname"
let msgHello_Message = "Welcome, GLHF!"

// Ping
let msg_ping = "PING :tmi.twitch.tv"

let msgPing_TwitchAddr = "tmi.twitch.tv"

[<Test>]
let``test message parse EN``()=
    let retv_msg = parseMessage msg_en
    
    match retv_msg with
    | Msg m -> 
        m.Nickname |> should equal f_Nicname
        m.NameAddr |> should equal f_NameAddr
        m.Cmd |> should equal f_Cmd
        m.Channel |> should equal f_Channel
        m.Message |> should startWith f_MessageStarts
    | _ -> 
        true |> should equal false

[<Test>]
let``test message parse RU``()=
    let retv_msg = parseMessage msg_ru
    
    match retv_msg with
    | Msg m -> 
        m.Nickname |> should equal nb_Nicname
        m.NameAddr |> should equal nb_NameAddr
        m.Cmd |> should equal nb_Cmd
        m.Channel |> should equal nb_Channel
        m.Message |> should startWith nb_MessageStarts
    | _ -> 
        true |> should equal false


[<Test>]
let``test message parse Hello type``()=
    let retv_msg = parseMessage msg_hello
    
    match retv_msg with
    | Info m -> 
        m.TwitchAddr |> should equal msgHello_TwitchAddr
        m.Code |> should equal msgHello_Code
        m.Nickname |> should equal msgHello_Nickname
        m.Message |> should startWith msgHello_Message
    | _ -> 
        true |> should equal false


[<Test>]
let``test message parse Ping``()=
    let retv_msg = parseMessage msg_ping
    
    match retv_msg with
    | Ping m -> 
        m.TwitchAddr |> should equal msgPing_TwitchAddr
    | _ -> 
        true |> should equal false

// Hello messages
let msg_Nick = ":twitch_username.tmi.twitch.tv 353 twitch_username = #channel :twitch_username"

let msgNick_TwitchAddr = "twitch_username.tmi.twitch.tv"
let msgNick_Code = "353"
let msgNick_Nickname = "twitch_username"
let msgNick_Channel = "#channel"
let msgNick_Nickname2 = "twitch_username"

[<Test>]
let``test message parse JOIN nicknames``()=
    let retv_msg = parseMessage msg_Nick
    
    match retv_msg with
    | Nicknames m -> 
        m.TwitchAddr |> should equal msgNick_TwitchAddr
        m.Code |> should equal msgNick_Code
        m.Nickname1 |> should equal msgNick_Nickname
        m.Channel |> should equal msgNick_Channel
        m.Nickname2 |> should equal msgNick_Nickname2
    | _ -> 
        true |> should equal false


let msg_join = ":twitch_username!twitch_username@twitch_username.tmi.twitch.tv JOIN #channel"

let msgJoin_Nickname = "twitch_username"
let msgJoin_TwitchAddr = "twitch_username@twitch_username.tmi.twitch.tv"
let msgJoin_Cmd = "JOIN"
let msgJoin_Channel = "#channel"

[<Test>]
let``test message parse JOIN message``()=
    let retv_msg = parseMessage msg_join
    
    match retv_msg with
    | ChanellJoin m -> 
        m.Nickname |> should equal msgJoin_Nickname
        m.NameAddr |> should equal msgJoin_TwitchAddr
        m.Cmd |> should equal msgJoin_Cmd
        m.Channel |> should equal msgJoin_Channel
    | _ -> 
        true |> should equal false

let msg_endNames = ":twitch_username.tmi.twitch.tv 366 twitch_username #channel :End of /NAMES list"

let msgendNames_nameaddr = "twitch_username.tmi.twitch.tv"
let msgendNames_code = "366"
let msgendNames_nickname = "twitch_username"
let msgendNames_Channel = "#channel"
let msgendNames_message = "End of /NAMES list"

[<Test>]
let``test message parse End of Names``()=
    let retv_msg = parseMessage msg_endNames
    
    match retv_msg with
    | NicknamesEnd m -> 
        m.NameAddr |> should equal msgendNames_nameaddr
        m.Code |> should equal msgendNames_code        
        m.Nickname |> should equal msgendNames_nickname
        m.Channel |> should equal msgendNames_Channel
        m.Message |> should equal msgendNames_message
    | _ -> 
        true |> should equal false