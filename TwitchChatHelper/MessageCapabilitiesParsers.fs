module MessageCapabilitiesParsers

open System
open MessageTypes

/// Pattern for membership capability ack.
let (|PatternMembershipAck|_|) (cmd: string) =

   // let pattern_CapMembershipAck = @"^:tmi\.twitch\.tv CAP \* ACK :twitch\.tv/membership$"
   // let m = Regex.Match(cmd, pattern_CapMembershipAck, RegexOptions.Compiled) 
   // match m.Success with
   let x = String.Equals(":tmi.twitch.tv CAP * ACK :twitch.tv/membership", cmd, System.StringComparison.InvariantCultureIgnoreCase)
   
   match x with
   | true ->
        let p = MembershipAck true
        Some(p)
   | false -> None