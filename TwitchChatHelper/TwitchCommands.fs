module TwitchCommands

open System
open System.IO
open System.Text
open System.Net
open System.Net.Sockets

open ConsoleOutHelpers

/// Twitch server
//let server = "irc.twitch.tv"
let server = "irc.chat.twitch.tv"

/// Twitch port
let port  = 6667

/// Get current TcpClient and reconnect if connection is lost.
let IrcConnectAsync (conn:TcpClient) = async{
    match conn.Connected with
    | true -> 
        return conn
    | false ->
        printfn "RECONNECT"
        conn.Connect(server, port)
        return conn
}
   
/// Get StreamReader for TcpClient. 
let IrcReaderAsync (client: TcpClient) = async {
    //try
        let! conn = IrcConnectAsync client
        let sr = new StreamReader( conn.GetStream() )
        return sr
//    with
//    | :? IOException as msg -> 
//        // force connect again, Connected failed.
//        printfn "RECONNECT on EXC"
//        client.Connect(server, port)
//
//        let! conn = IrcConnectAsync client
//        let sr = new StreamReader( conn.GetStream() )
//        return Some(sr)
//    | _ -> 
//        reraise()
//        return None 
}

/// Get StreamWriter for TcpClient. 
let IrcWriterAsync (client: TcpClient) = async {
    let! conn = IrcConnectAsync client
    let irc_writer = new StreamWriter( conn.GetStream() )
    irc_writer.AutoFlush <- true
    return irc_writer 
}

/// Receive message async.
let ReceiveMessageAsync (conn:TcpClient) = async{
    let! cli = IrcReaderAsync conn
    //let msg = cli |> Option.map (fun v -> v.ReadLineAsync() |> Async.AwaitTask) 
    let msg2 = cli.ReadLineAsync() |> Async.AwaitTask

    return! msg2
}

/// Send Pass
let SendPass(oauth:string)(conn:TcpClient) =    
    let irc_writer = IrcWriterAsync conn |> Async.RunSynchronously
    
    irc_writer.WriteLine( sprintf "PASS %s" oauth)
    printColored colorInfo "Send PASS"

/// Send password asyns.
let SendPassAsync(oauth:string)(conn:TcpClient) = async{
    return SendPass oauth conn
}

/// Send Nickname
let SendNick(nick:string)(conn:TcpClient) =
    let irc_writer = IrcWriterAsync conn |> Async.RunSynchronously
    
    irc_writer.WriteLine( sprintf "NICK %s" nick )
    printColored colorInfo "Send NICK"

// Send Nickname Async.
let SendNickAsync(nick:string)(conn:TcpClient) = async{
    return SendNick nick conn
}

/// Send JOIN.
let SendJoin(channel:string)(conn:TcpClient) =
    
    if(not (channel.StartsWith("#"))) then invalidArg "channel" "Channel name must start with #"

    let irc_writer = IrcWriterAsync conn |> Async.RunSynchronously

    irc_writer.WriteLine( sprintf "JOIN %s\n" channel )
    printColored colorInfo (sprintf "JOIN %s\n" channel)

/// Send JOIN async.
let SendJoinAsync(channel:string)(conn:TcpClient) = async{
    return SendJoin channel conn
}

/// Send PONG.
let SendPong(conn:TcpClient) =
    let irc_writer = IrcWriterAsync conn |> Async.RunSynchronously
    irc_writer.WriteLineAsync( "PONG :tmi.twitch.tv" ) |> Async.AwaitTask

/// Send PONG async.
let SendPongAsync(conn:TcpClient) = async{
    let! ircWriter = IrcWriterAsync conn
    ircWriter.WriteLine( "PONG :tmi.twitch.tv" )
}

/// Loging and join given channel.
let LoginAndJoinAsync (oauth:string)(nick:string)(channel:string) (conn:TcpClient) = async{
    do! SendPassAsync oauth conn
    do! SendNickAsync nick conn
    do! SendJoinAsync channel conn
}