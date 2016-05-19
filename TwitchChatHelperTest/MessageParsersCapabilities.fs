module MessageParsersCapabilities

open System
open System.IO
open System.Text.RegularExpressions

open NUnit.Framework
open FsUnit

open MessageTypes
open MessageTypesBasic
open MessageCapabilitiesTypes
open MessageParsers

/// MODE: Someone gained or lost operator:
/// :jtv MODE #channel +o operator_user
type MessageMembershipMode = {
    Channel:string    
    /// 1: +o, 0: -o
    Mode:bool    
    Username:string}

/// Twitch message temp
type MessageTmp = 
  | MembershipMode of MessageMembershipMode
  | Other of string  // ofr not parsed messages.

// Goes after: 
// https://github.com/justintv/Twitch-API/blob/master/IRC.md#twitch-capabilities
// Using IRCv3 capability registration, it is possible to register for Twitch-specific capabilities. The capabilities are defined below:

/// RegEx for chanell messages.


/// Converts mode string to bool value.
/// 1: +o, 
/// 0: -o 
let modeToBool mode = 
    match mode  with
    | "+" -> true
    | "-" -> false
    | _ -> failwith "unknown mode"

/// Pattern for membership capability ack.
let (|PatternMembershipMode|_|) (cmd: string) =
   let pattern_CapMembershipMode = @"^:jtv\s*MODE\s+(?<channel>#[\w]{2,24})\s*(?<mode>[\+-]{1})o\s*(?<username>[\w]{2,24})$"
   
   let m = Regex.Match(cmd, pattern_CapMembershipMode, RegexOptions.Compiled) 
   match m.Success with
   | true ->
        let p = MembershipMode {
            Channel = m.Groups.["channel"].Value
            Mode = modeToBool <| m.Groups.["mode"].Value
            Username = m.Groups.["username"].Value
            }
        Some(p)
   | false -> None

/// Parse message into object.
let parseMessage2 cmd =
    match cmd with
    // often used commands.  
    | PatternMembershipMode a -> a    
    | _ -> Other cmd


[<Test>]
let``RecvCap: test membership ack``()=
    let msg = ":tmi.twitch.tv CAP * ACK :twitch.tv/membership"
    let cmd = parseMessage msg

    match cmd with
    | MembershipAck a  -> 
        a |> should equal true       
    | _ -> true |> should equal false

// NAMES, JOIN, PART, same as base. 
// But other users also, may want to filter.

// MODE

let msg_memb_mode_plus = ":jtv MODE #channel +o operator_user"
let msg_memb_mode_minus = ":jtv MODE #channel -o operator_user"

/// Pattern for membership mode.
[<TestFixture>]
type ``Recv: test regex membership mode`` () = 
    static member TestData =
        [|
            [|msg_memb_mode_plus, "#channel" , true, "operator_user"|];
            [|msg_memb_mode_minus, "#channel" , false, "operator_user"|];
        |]

    [<TestCaseSource("TestData")>]
    member x.``Recv: test regex membership mode`` (testData:(string*string*bool*string)) =
        let cmd, channel, mode, username = testData
        
        let retv_msg = parseMessage2 cmd

        match retv_msg with
        | MembershipMode m -> 
            m.Channel |> should equal channel
            m.Mode |> should equal mode
            m.Username |> should equal username
        | _ -> 
            true |> should equal false


// REAL ONES

