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
    try
        printfn "CONNECT"
        let irc_client = new TcpClient()
        irc_client.Connect(server, port)
        Some(irc_client)
    with
        | :? SocketException as ex -> 
            printfn "Got SocketException. No connection. Skip everything."
            None
        | _ as ex->                 
            // don't handle any other cases
            printfn "%s" ex.Message 
            reraise()
    )

/// TcpClient instance. 
let GetConnInstance() = 
    let currConn = conn.Value |> Option.map (fun currConn ->
        match currConn.Connected with
        | true -> 
            currConn
        | false ->
            printfn "RECONNECT"
            currConn.Connect(server, port)
            currConn
    )
    currConn

/// StreamReader initialization.
let private ircReader = Lazy.Create(fun() -> 
    let conn = GetConnInstance() |> Option.map (fun conn ->
        let sr = new StreamReader( conn.GetStream() )
        sr)
    conn
    )

/// StreamReader instance. 
let GetReaderInstance() = ircReader.Value

/// StreamWriter initialization.
let private ircWriter = Lazy.Create(fun() -> 
    let conn = GetConnInstance() |> Option.map (fun conn ->
        let sr = new StreamWriter( conn.GetStream() )
        sr.AutoFlush <- true
        sr)
    conn
    )

/// StreamWriter instance. 
let GetWriterInstance() = ircWriter.Value
