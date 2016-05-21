module MessageCommandsParsers

// https://github.com/justintv/Twitch-API/blob/master/IRC.md#commands

open System
open MessageTypes
open MessageParsersBasic
open System.Text.RegularExpressions
open MessageCommandsTypes

/// Pattern for membership capability ack.
let (|PatternCommandsAck|_|) (cmd: string) =

   // let pattern_CapMembershipAck = @"^:tmi\.twitch\.tv CAP \* ACK :twitch\.tv/membership$"
   // let m = Regex.Match(cmd, pattern_CapMembershipAck, RegexOptions.Compiled) 
   // match m.Success with
   let x = String.Equals(":tmi.twitch.tv CAP * ACK :twitch.tv/commands", cmd, System.StringComparison.InvariantCultureIgnoreCase)
   
   match x with
   | true ->
        let p = CommandsAck true
        Some(p)
   | false -> None


let parseMsgId (msgString :string) =
    match msgString with
    | "subs_on"     -> MsgId.SubsOn
    | "subs_off"    -> MsgId.SubsOff
    | "slow_on"     -> MsgId.SlowOn
    | "slow_off"    -> MsgId.SlowOff
    | "r9k_on"      -> MsgId.R9kOn
    | "r9k_off"     -> MsgId.R9kOff
    | "host_on"     -> MsgId.HostOn
    | "host_off"    -> MsgId.HostOff
    | _ -> failwith "Unknown MsgId"


/// Pattern for command NOTICE.
/// @msg-id=slow_off :tmi.twitch.tv NOTICE #channel :This room is no longer in slow mode.
[<Literal>]
let pattern_CommandNotice =  @"^@msg-id=(?<msgid>(subs_on|subs_off|slow_on|slow_off|r9k_on|r9k_off|host_on|host_off))\s+:(?<twitchGroup>[\w\.]+)\s+NOTICE\s+" + pattern_channel + "\s+:(?<message>.*)$"
// ^@msg-id=(?<msgid>(subs_on|subs_off|slow_on|slow_off|r9k_on|r9k_off|host_on|host_off))\s+:(?<twitchGroup>[\w\.]+)\s+NOTICE\s+(?<channel>#[\w]{2,24})\s+:(?<message>.*)$

/// Pattern for command NOTICE.
let (|PatternCommandsNotice|_|) (cmd: string) =
   let m = Regex.Match(cmd, pattern_CommandNotice, RegexOptions.Compiled) 
   match m.Success with
   | true -> 
        let p = CommandsNotice {
            MsgId = parseMsgId <| m.Groups.["msgid"].Value 
            TwitchGroup = m.Groups.["twitchGroup"].Value
            Channel= m.Groups.["channel"].Value 
            Message = m.Groups.["message"].Value           
            }
        Some(p)
   | false -> None