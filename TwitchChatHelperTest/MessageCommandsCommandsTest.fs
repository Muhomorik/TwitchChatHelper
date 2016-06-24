module MessageCommandsCommandsTest

open System
open System.IO
open System.Text.RegularExpressions

open NUnit.Framework
open FsUnit

open MessageTypes
open MessageTypesBasic
open MessageCapabilitiesTypes
open MessageCommandsTypes
open MessageParsers

// Goes after: 
// https://github.com/justintv/Twitch-API/blob/master/IRC.md#commands
// Enables USERSTATE, GLOBALUSERSTATE, ROOMSTATE, HOSTTARGET, NOTICE and CLEARCHAT raw commands.

/// RegEx for chanell messages.

[<Test>]
let``RecvCap: test commands ack``()=
    let msg = ":tmi.twitch.tv CAP * ACK :twitch.tv/commands"
    let cmd = parseMessage msg

    match cmd with
    | CommandsAck a  -> 
        a |> should equal true       
    | _ -> true |> should equal false


let msg_notice_slown_on = ":tmi.twitch.tv NOTICE #channel :This room is now in slow mode. You may send messages every 120 seconds."

[<Test>]
let``RecvCap: test commands NOTICE slown_on``()=
    let msg = msg_notice_slown_on 
    let cmd = parseMessage msg

    match cmd with
    | CommandsNotice a  -> 
        a.MsgId |> should equal MsgId.SlowOn       
        a.TwitchGroup |> should equal "tmi.twitch.tv"       
        a.Channel |> should equal "#channel"       
        a.Message |> should equal "This room is now in slow mode. You may send messages every 120 seconds."       
    | _ -> true |> should equal false

let msg_notice_slown_off = ":tmi.twitch.tv NOTICE #channel :This room is no longer in slow mode."

[<Test>]
let``RecvCap: test commands NOTICE slown_off``()=
    let msg = msg_notice_slown_off 
    let cmd = parseMessage msg

    match cmd with
    | CommandsNotice a  -> 
        a.MsgId |> should equal MsgId.SlowOff       
        a.TwitchGroup |> should equal "tmi.twitch.tv"       
        a.Channel |> should equal "#channel"       
        a.Message |> should equal "This room is no longer in slow mode."       
    | _ -> true |> should equal false


// TODO: move to tags when tags done.

//let msg_notice_slown_on = ":tmi.twitch.tv NOTICE #channel :This room is now in slow mode. You may send messages every 120 seconds."
//let msg_notice_slown_off = ":tmi.twitch.tv NOTICE #channel :This room is no longer in slow mode."
//
//let msg_notice1 = "@msg-id=slow_off :tmi.twitch.tv NOTICE #channel :This room is no longer in slow mode."
////  TODO: other messages, find in log.
//[<TestFixture>]
//type ``Recv: test parse command notice`` () = 
//    static member TestData =
//        [|
//            [|msg_notice1, "slow_off", "tmi.twitch.tv" , "#channel", "This room is no longer in slow mode."|];
//        |]
//
//    [<TestCaseSource("TestData")>]
//    member x.``Recv: test parse command notice`` (testData:(string*string*string*string*string)) =
//        let cmd, msgId, twitchGroup, channel, message = testData
//        
//        let retv_msg = parseMessage cmd
//
//        // TODO: change type.
//        match retv_msg with
//        | CommandsNotice m -> 
//            m.MsgId |> should equal MsgId.SlowOff
//            m.TwitchGroup |> should equal twitchGroup
//            m.Channel |> should equal channel
//            m.Message |> should equal message
//        | _ -> 
//            true |> should equal false

[<Test>]
let``RecvCap: test commands Hostarget start``()=
    let msg = ":tmi.twitch.tv HOSTTARGET #hosting_channel :target_channel 285" 
    // TODO: start number i prel. no real data.
    // TODO: check hasthag for target channel.
    let cmd = parseMessage msg

    match cmd with
    | CommandsHostTargetStart a  -> 
        a.TwitchGroup |> should equal "tmi.twitch.tv"       
        a.ChannelHosting |> should equal "#hosting_channel"       
        a.ChannelTarget |> should equal "target_channel"       
        a.Number |> should equal "285"       
    | _ -> true |> should equal false

[<Test>]
let``RecvCap: test commands Hostarget stop``()=
    let msg = ":tmi.twitch.tv HOSTTARGET #hosting_channel :- 100" 
    // TODO: start number i prel. no real data.
    // TODO: check hasthag for target channel.
    let cmd = parseMessage msg

    match cmd with
    | CommandsHostTargetStop a  -> 
        a.TwitchGroup |> should equal "tmi.twitch.tv"       
        a.ChannelHosting |> should equal "#hosting_channel"              
        a.Number |> should equal "100"       
    | _ -> true |> should equal false

[<Test>]
let``RecvCap: test commands CommandsClearChatUser``()=
    let msg = ":tmi.twitch.tv CLEARCHAT #channel :twitch_username" 
    let cmd = parseMessage msg

    match cmd with
    | CommandsClearChatUser a  -> 
        a.TwitchGroup |> should equal "tmi.twitch.tv"       
        a.Channel |> should equal "#channel"              
        a.Nickname |> should equal "twitch_username"       
    | _ -> true |> should equal false

[<Test>]
let``RecvCap: test commands CommandsClearChat``()=
    let msg = ":tmi.twitch.tv CLEARCHAT #channel" 
    let cmd = parseMessage msg

    match cmd with
    | CommandsClearChat a  -> 
        a.TwitchGroup |> should equal "tmi.twitch.tv"       
        a.Channel |> should equal "#channel"                     
    | _ -> true |> should equal false

[<Test>]
let``RecvCap: test commands CommandsUserstate``()=
    let msg = ":tmi.twitch.tv USERSTATE #channel" 
    let cmd = parseMessage msg

    match cmd with
    | CommandsUserstate a  -> 
        a.TwitchGroup |> should equal "tmi.twitch.tv"       
        a.Channel |> should equal "#channel"                     
    | _ -> true |> should equal false


[<Test>]
let``RecvCap: test commands CommandsReconnet``()=
    let msg = "RECONNECT" 
    let cmd = parseMessage msg

    match cmd with
    | CommandsReconnect -> 
        true |> should equal true                     
    | _ -> true |> should equal false

[<Test>]
let``RecvCap: test commands CommandsRoomstate``()=
    let msg = ":tmi.twitch.tv ROOMSTATE #channel" 
    let cmd = parseMessage msg

    match cmd with
    | CommandsRoomstate a  -> 
        a.TwitchGroup |> should equal "tmi.twitch.tv"       
        a.Channel |> should equal "#channel"                     
    | _ -> true |> should equal false
