module TwitchCommandsCommandsCapabilities

open System.IO

// Enables USERSTATE, GLOBALUSERSTATE, ROOMSTATE, HOSTTARGET, NOTICE and CLEARCHAT raw commands.
// https://github.com/justintv/Twitch-API/blob/master/IRC.md#commands

/// Enables USERSTATE, GLOBALUSERSTATE, ROOMSTATE, HOSTTARGET, NOTICE and CLEARCHAT raw commands.
let SendCapabilitiesCommandsAsyns (conn:StreamWriter) =    
    conn.WriteLineAsync( "CAP REQ :twitch.tv/commands" ) |> Async.AwaitTask
