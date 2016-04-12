module MessageParsersTest

open System
open System.IO
open System.Text.RegularExpressions

open NUnit.Framework
open FsUnit

open MessageTypes
open MessageParsers

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
