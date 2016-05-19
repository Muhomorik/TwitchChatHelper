module MessageCapabilitiesParsers

open System
open MessageTypes
open System.Text.RegularExpressions

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