module MessageParsersBasic

open System.Text.RegularExpressions
open MessageTypes

// TODO: somehow join nickname patterns.

/// Twitch username pattern.
[<Literal>]
let pattern_twitchUsername =  @"(?<nickname>[\w_\.]+)"

/// Twitch username pattern, when there are two usernames.
[<Literal>]
let pattern_twitchUsername2 =  @"(?<nickname2>[\w_\.]+)"

/// Twitch alternative username pattern.
[<Literal>]
let pattern_twitchUsernameAlt =  @"(?<nicknameAlt>[\w_\.]+)"

/// Twitch channel pattern.
[<Literal>]
let pattern_channel =  @"(?<channel>#[\w]{2,24})"

/// RegEx for successful connection message.
[<Literal>]
let pattern_successfulConnection =  @"^:(?<twitchGroup>[\w\.]+)\s+(?<code>\d+)\s+" + pattern_twitchUsername + "\s+:(?<message>.*)$"

let (|PatternSuccConn|_|) (cmd: string) =
   let m = Regex.Match(cmd, pattern_successfulConnection, RegexOptions.Compiled) 
   match m.Success with
   | true -> 
        let p = SuccConnection {
            TwitchGroup = m.Groups.["twitchGroup"].Value
            Code = m.Groups.["code"].Value
            Nickname = m.Groups.["nickname"].Value
            Message = m.Groups.["message"].Value
            }
        Some(p)
   | false -> None

/// RegEx for successful connection message.
/// PING :tmi.twitch.tv
[<Literal>]
let pattern_ping =  @"^PING :tmi.twitch.tv$"

let (|PatternPing|_|) (cmd: string) =
   let m = Regex.Match(cmd, pattern_ping, RegexOptions.Compiled) 
   match m.Success with
   | true ->  Some(Ping)
   | false -> None

/// RegEx for invalid command.
[<Literal>]
let pattern_invalidCommand =  @"^:(?<twitchGroup>[\w\.]+)\s+(?<code>\d+)\s+" + pattern_twitchUsername + "\s+WHO\s*:(?<message>.*)"

let (|PatternInvalidCommand|_|) (cmd: string) =
   let m = Regex.Match(cmd, pattern_invalidCommand, RegexOptions.Compiled) 
   match m.Success with
   | true -> 
        let p = InvalidCommand {
            TwitchGroup = m.Groups.["twitchGroup"].Value
            Code = m.Groups.["code"].Value
            Nickname = m.Groups.["nickname"].Value
            Message = m.Groups.["message"].Value
            }
        Some(p)
   | false -> None

[<Literal>]
let pattern_channelJoin = @"^:" + pattern_twitchUsernameAlt + "!" + pattern_twitchUsername + "@" + pattern_twitchUsername2 + "\.tmi\.twitch\.tv\s+JOIN\s+(?<channel>#[\w]{2,24})$"

let (|PatternChannelJoin|_|) (cmd: string) =
   let m = Regex.Match(cmd, pattern_channelJoin, RegexOptions.Compiled) 
   match m.Success with
   | true ->
        // check nicknames (unclead if ok)
        let nickname = m.Groups.["nickname"].Value 
        let nickname2 = m.Groups.["nickname2"].Value
        if nickname <> nickname2 then printfn "NICKNAMES NOT EQUAL: %s, %s" nickname nickname2
         
        let p = ChannelJoin {
            NicknameAlterative = m.Groups.["nicknameAlt"].Value
            Nickname = m.Groups.["nickname"].Value
            Channel = m.Groups.["channel"].Value
            }
        Some(p)
   | false -> None

/// Twitch JOIN response with nicknames, code 353. TODO: figure out what is what.
/// :twitch_username.tmi.twitch.tv 353 twitch_username = #channel :twitch_usernames
[<Literal>]
let pattern_ChanellNicknames = @"^:" + pattern_twitchUsername + "\.tmi\.twitch\.tv\s+(?<code>\w*)\s+" + pattern_twitchUsername2 + "\s+=\s+(?<channel>#\w*)\s+:(?<nicknames>[\w_\.\s]+)$"

let (|PatternChannelNicknames|_|) (cmd: string) =
   let m = Regex.Match(cmd, pattern_ChanellNicknames, RegexOptions.Compiled) 
   match m.Success with
   | true ->
        let p = ChannelNicknames {
            Nickname = m.Groups.["nickname"].Value
            Code = m.Groups.["code"].Value
            NicknameJoin = m.Groups.["nickname2"].Value
            Channel = m.Groups.["channel"].Value
            Nicknames = m.Groups.["nicknames"].Value
            }
        Some(p)
   | false -> None


/// Twitch end of nicknames, code 366.
/// :twitch_username.tmi.twitch.tv 366 twitch_username #channel :End of /NAMES list
[<Literal>]
let pattern_ChanellNicknamesEnd = @"^:(?<nameaddr>[\w]{2,24})\.tmi\.twitch\.tv\s+(?<code>\w*)\s+" + pattern_twitchUsername + "\s+(?<channel>#\w*)\s+:End of /NAMES list$"

let (|PatternChannelNicknamesEnd|_|) (cmd: string) =
   let m = Regex.Match(cmd, pattern_ChanellNicknamesEnd, RegexOptions.Compiled) 
   match m.Success with
   | true ->
        let p = ChannelNicknamesEnd {
            NameAddr = m.Groups.["nameaddr"].Value
            Code = m.Groups.["code"].Value
            Nickname = m.Groups.["nickname"].Value
            Channel = m.Groups.["channel"].Value
            }
        Some(p)
   | false -> None

/// PART: Leaving a chat room
/// :twitch_username!twitch_username@twitch_username.tmi.twitch.tv PART #channel
[<Literal>]
let pattern_channelPart = @"^:" + pattern_twitchUsernameAlt + "!" + pattern_twitchUsername + "@" + pattern_twitchUsername2 + "\.tmi\.twitch\.tv\s+PART\s+(?<channel>#[\w]{2,24})$"

let (|PatternChannelPart|_|) (cmd: string) =
   let m = Regex.Match(cmd, pattern_channelPart, RegexOptions.Compiled) 
   match m.Success with
   | true ->
        // check nicknames (unclead if ok)
        let nickname = m.Groups.["nickname"].Value 
        let nickname2 = m.Groups.["nickname2"].Value
        if nickname <> nickname2 then printfn "NICKNAMES NOT EQUAL: %s, %s" nickname nickname2
         
        let p = ChannelLeave {
            NicknameAlterative = m.Groups.["nicknameAlt"].Value
            Nickname = m.Groups.["nickname"].Value
            Channel = m.Groups.["channel"].Value
            }
        Some(p)
   | false -> None

/// RegEx for chanell messages.

let pattern_ChanellMsg = @"^:" + pattern_twitchUsernameAlt + "!" + pattern_twitchUsername + "@" + pattern_twitchUsername2 + "\.tmi\.twitch\.tv\s+PRIVMSG\s+(?<channel>#[\w]{2,24})\s*:(?<message>.*)$"

let (|PatternChannelMessage|_|) (cmd: string) =
   let m = Regex.Match(cmd, pattern_ChanellMsg, RegexOptions.Compiled) 
   match m.Success with
   | true ->
        // check nicknames (unclead if ok)
        let nickname = m.Groups.["nickname"].Value 
        let nickname2 = m.Groups.["nickname2"].Value
        if nickname <> nickname2 then printfn "NICKNAMES NOT EQUAL: %s, %s" nickname nickname2
         
        let p = ChanellMessage {
            NicknameAlterative = m.Groups.["nicknameAlt"].Value
            Nickname = m.Groups.["nickname"].Value
            Channel = m.Groups.["channel"].Value
            Message = m.Groups.["message"].Value
            }
        Some(p)
   | false -> None