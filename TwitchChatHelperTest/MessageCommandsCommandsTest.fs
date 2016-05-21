module MessageCommandsCommandsTest

open System
open System.IO
open System.Text.RegularExpressions

open NUnit.Framework
open FsUnit

open MessageTypes
open MessageTypesBasic
open MessageCapabilitiesTypes
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