module TwitchCommands

open System.IO


/// Send PASS
let SendPass (conn:StreamWriter)(oauth:string) =    
    conn.WriteLine( sprintf "PASS %s" oauth)

/// Send PASS
let SendPassAsync (conn:StreamWriter)(oauth:string) = async{   
    do! conn.WriteLineAsync( sprintf "PASS %s" oauth) |> Async.AwaitTask
    }

/// Send NICK
let SendNick (conn:StreamWriter)(nick:string) =
    conn.WriteLine( sprintf "NICK %s" nick )

/// Send JOIN.
let SendJoin (conn:StreamWriter)(channel:string) =
    if(not (channel.StartsWith("#"))) then invalidArg "channel" "Channel name must start with #"

    conn.WriteLine( sprintf "JOIN %s\n" channel )

/// Send JOIN.
let SendJoinAsync (conn:StreamWriter)(channel:string) = async{
    if(not (channel.StartsWith("#"))) then invalidArg "channel" "Channel name must start with #"

    do! conn.WriteLineAsync( sprintf "JOIN %s\n" channel ) |> Async.AwaitTask
    }

/// Send PONG.
let SendPong (conn:StreamWriter) =
    conn.WriteLine( "PONG :tmi.twitch.tv" )

/// Send PONG.
let SendPongAsync (conn:StreamWriter) = async{
    do! conn.WriteLineAsync( "PONG :tmi.twitch.tv" ) |> Async.AwaitTask
    }

/// Send PART (leave) to channel.
let SendPartAsync (conn:StreamWriter)(channel:string) = async{
    do! conn.WriteLineAsync( sprintf "PART %s" channel ) |> Async.AwaitTask
    }