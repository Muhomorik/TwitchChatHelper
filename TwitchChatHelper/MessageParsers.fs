module MessageParsers

open MessageTypes
open MessageParsersBasic
open MessageCapabilitiesParsers


/// Parse message into object.
let parseMessage cmd =
    match cmd with
    // often used commands.
    | PatternChannelMessage a -> a    
    | PatternPing a -> a
    // Less common commands.
    | PatternSuccConn a -> a
    | PatternInvalidCommand a -> a
    | PatternChannelJoin a -> a
    | PatternChannelNicknames a -> a
    | PatternChannelNicknamesEnd a -> a
    | PatternChannelPart a -> a

    // Capabilities
    | PatternMembershipAck a -> a

    /// Unknown cmd, log it and fix later.
    | _ -> Other cmd


