module Connection

open System
open System.IO
open System.Net.Sockets

/// IRC server.
let server = "irc.chat.twitch.tv"

/// Twitch port
let port  = 6667

/// TcpClient initialization.
let private conn = Lazy.Create(fun() -> 
    printfn "CONNECT"
    let irc_client = new TcpClient()
    irc_client.Connect(server, port)
    irc_client)

/// TcpClient instance. 
let GetConnInstance() = 
    let currConn = conn.Value
    match currConn.Connected with
    | true -> 
        currConn
    | false ->
        printfn "RECONNECT"
        currConn.Connect(server, port)
        currConn

/// StreamReader initialization.
let private ircReader = Lazy.Create(fun() -> 
    let conn = GetConnInstance()
    let sr = new StreamReader( conn.GetStream() )
    sr)

/// StreamReader instance. 
let GetReaderInstance() = ircReader.Value

/// StreamWriter initialization.
let private ircWriter = Lazy.Create(fun() -> 
    let conn = GetConnInstance()
    let sr = new StreamWriter( conn.GetStream() )
    sr.AutoFlush <- true
    sr)

/// StreamWriter instance. 
let GetWriterInstance() = ircWriter.Value
