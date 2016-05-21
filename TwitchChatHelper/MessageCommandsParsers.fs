module MessageCommandsParsers

// https://github.com/justintv/Twitch-API/blob/master/IRC.md#commands

open System
open MessageTypes
open MessageParsersBasic
open System.Text.RegularExpressions
open MessageCommandsTypes

/// Pattern for membership capability ack.
let (|PatternCommandsAck|_|) (cmd: string) =
   let x = String.Equals(":tmi.twitch.tv CAP * ACK :twitch.tv/commands", cmd, System.StringComparison.InvariantCultureIgnoreCase)
   
   match x with
   | true ->
        let p = CommandsAck true
        Some(p)
   | false -> None

/// Parse mgg-id to enum
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


/// Pattern for command Host starts message:.
/// :tmi.twitch.tv HOSTTARGET #hosting_channel :target_channel [number].
[<Literal>]
let pattern_CommandsHostTargetStart =  @"^:(?<twitchGroup>[\w\.]+)\s+HOSTTARGET\s+" + pattern_channel + "\s+:" + pattern_channel2 + "\s(?<number>\d+)$"
// ^:(?<twitchGroup>[\w\.]+)\s+HOSTTARGET\s+(?<channel>#[\w]{2,24})\s+:(?<channel1>#[\w]{2,24})\s(?<number>\d+)$

/// Pattern for command Host starts message:.
let (|PatternCommandsHostTargetStart|_|) (cmd: string) =
   let m = Regex.Match(cmd, pattern_CommandsHostTargetStart, RegexOptions.Compiled) 
   match m.Success with
   | true -> 
        let p = CommandsHostTargetStart {
            TwitchGroup = m.Groups.["twitchGroup"].Value 
            ChannelHosting = m.Groups.["channel"].Value
            ChannelTarget= m.Groups.["channel_2"].Value 
            Number = m.Groups.["number"].Value           
            }
        Some(p)
   | false -> None

/// Pattern for command Host stop message.
/// :tmi.twitch.tv HOSTTARGET #hosting_channel :- [number]
[<Literal>]
let pattern_CommandsHostTargetStop =  @"^:(?<twitchGroup>[\w\.]+)\s+HOSTTARGET\s+" + pattern_channel + "\s+:\-\s+(?<number>\d+)$"
// ^:(?<twitchGroup>[\w\.]+)\s+HOSTTARGET\s+(?<channel>#[\w]{2,24})\s+:\-\s+(?<number>\d+)$

/// Pattern for command Host stop message.
let (|PatternCommandsHostTargetStop|_|) (cmd: string) =
   let m = Regex.Match(cmd, pattern_CommandsHostTargetStop, RegexOptions.Compiled) 
   match m.Success with
   | true -> 
        let p = CommandsHostTargetStop {
            TwitchGroup = m.Groups.["twitchGroup"].Value 
            ChannelHosting = m.Groups.["channel"].Value 
            Number = m.Groups.["number"].Value           
            }
        Some(p)
   | false -> None

/// Pattern for command CLEARCHAT user.
/// :tmi.twitch.tv CLEARCHAT #channel :twitch_username
[<Literal>]
let pattern_CommandsClearChatUser =  @"^:(?<twitchGroup>[\w\.]+)\s+CLEARCHAT\s+" + pattern_channel + "\s+:" + pattern_twitchUsername + "$"
// ^:(?<twitchGroup>[\w\.]+)\s+CLEARCHAT\s+(?<channel>#[\w]{2,24})\s+:(?<nickname>[\w_\.]+)$

/// Pattern for  CLEARCHAT user.
let (|PatternCommandsClearChatUser|_|) (cmd: string) =
   let m = Regex.Match(cmd, pattern_CommandsClearChatUser, RegexOptions.Compiled) 
   match m.Success with
   | true -> 
        let p = CommandsClearChatUser {
            TwitchGroup = m.Groups.["twitchGroup"].Value 
            Channel = m.Groups.["channel"].Value 
            Nickname = m.Groups.["nickname"].Value           
            }
        Some(p)
   | false -> None

/// Pattern for command CLEARCHAT channel.
/// :tmi.twitch.tv CLEARCHAT #channel
[<Literal>]
let pattern_CommandsClearChat =  @"^:(?<twitchGroup>[\w\.]+)\s+CLEARCHAT\s+" + pattern_channel + "$"
// ^:(?<twitchGroup>[\w\.]+)\s+CLEARCHAT\s+(?<channel>#[\w]{2,24})$

/// Pattern for CLEARCHAT channel.
let (|PatternCommandsClearChat|_|) (cmd: string) =
   let m = Regex.Match(cmd, pattern_CommandsClearChat, RegexOptions.Compiled) 
   match m.Success with
   | true -> 
        let p = CommandsClearChat {
            TwitchGroup = m.Groups.["twitchGroup"].Value 
            Channel = m.Groups.["channel"].Value            
            }
        Some(p)
   | false -> None

/// Pattern for command USERSTATE.
/// :tmi.twitch.tv CLEARCHAT #channel
[<Literal>]
let pattern_CommandsUserstate =  @"^:(?<twitchGroup>[\w\.]+)\s+USERSTATE\s+" + pattern_channel + "$"
// ^:(?<twitchGroup>[\w\.]+)\s+USERSTATE\s+(?<channel>#[\w]{2,24})$

/// Pattern for CLEARCHAT channel.
let (|PatternCommandsUserstate|_|) (cmd: string) =
   let m = Regex.Match(cmd, pattern_CommandsUserstate, RegexOptions.Compiled) 
   match m.Success with
   | true -> 
        let p = CommandsUserstate {
            TwitchGroup = m.Groups.["twitchGroup"].Value 
            Channel = m.Groups.["channel"].Value            
            }
        Some(p)
   | false -> None


/// Pattern for RECONNECT. TODO no actual data on RECONNECT.
let (|PatternCommandsReconnect|_|) (cmd: string) =
   let x = String.Equals("RECONNECT", cmd, System.StringComparison.InvariantCultureIgnoreCase)
   
   match x with
   | true ->
        Some(CommandsReconnect)
   | false -> None

/// Pattern for command ROOMSTATE.
/// :tmi.twitch.tv ROOMSTATE #channel
[<Literal>]
let pattern_CommandsRoomstate =  @"^:(?<twitchGroup>[\w\.]+)\s+ROOMSTATE\s+" + pattern_channel + "$"
// ^:(?<twitchGroup>[\w\.]+)\s+ROOMSTATE\s+(?<channel>#[\w]{2,24})$

/// Pattern for CLEARCHAT channel.
let (|PatternCommandsRoomstate|_|) (cmd: string) =
   let m = Regex.Match(cmd, pattern_CommandsRoomstate, RegexOptions.Compiled) 
   match m.Success with
   | true -> 
        let p = CommandsRoomstate {
            TwitchGroup = m.Groups.["twitchGroup"].Value 
            Channel = m.Groups.["channel"].Value            
            }
        Some(p)
   | false -> None