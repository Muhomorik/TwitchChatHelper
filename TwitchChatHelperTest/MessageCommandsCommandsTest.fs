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



let msg_notice1 = "@msg-id=slow_off :tmi.twitch.tv NOTICE #channel :This room is no longer in slow mode."
//  TODO: other messages, find in log.
[<TestFixture>]
type ``Recv: test parse command notice`` () = 
    static member TestData =
        [|
            [|msg_notice1, "slow_off", "tmi.twitch.tv" , "#channel", "This room is no longer in slow mode."|];
        |]

    [<TestCaseSource("TestData")>]
    member x.``Recv: test parse command notice`` (testData:(string*string*string*string*string)) =
        let cmd, msgId, twitchGroup, channel, message = testData
        
        let retv_msg = parseMessage cmd

        // TODO: change type.
        match retv_msg with
        | CommandsNotice m -> 
            m.MsgId |> should equal MsgId.SlowOff
            m.TwitchGroup |> should equal twitchGroup
            m.Channel |> should equal channel
            m.Message |> should equal message
        | _ -> 
            true |> should equal false