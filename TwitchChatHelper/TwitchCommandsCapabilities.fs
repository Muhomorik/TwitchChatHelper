module TwitchCommandsCapabilities

open System.IO

/// Adds membership state event (NAMES, JOIN, PART, or MODE) functionality.
let SendCapabilitiesMembershipAsyns (conn:StreamWriter) =    
    conn.WriteLineAsync( "CAP REQ :twitch.tv/membership" ) |> Async.AwaitTask


