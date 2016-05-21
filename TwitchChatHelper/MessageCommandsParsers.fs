module MessageCommandsParsers

// https://github.com/justintv/Twitch-API/blob/master/IRC.md#commands

open System
open MessageTypes
open MessageParsersBasic
open System.Text.RegularExpressions

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