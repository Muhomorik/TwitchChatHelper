module TwitchCommands

open System
open System.IO
open System.Text
open System.Net
open System.Net.Sockets

open ConsoleOutHelpers

/// Twitch server
let server = "irc.twitch.tv"

/// Twitch port
let port  = 6667

// establish a connection to the server
let irc_client = new TcpClient();
irc_client.Connect( server, port )    
    
// get the input and output streams
let irc_reader = new StreamReader( irc_client.GetStream() )

// writer
let irc_writer = new StreamWriter( irc_client.GetStream() )
irc_writer.AutoFlush <- true

let ReceiveMessage() = 
    //use irc_reader = new StreamReader( irc_client.GetStream() )
    let msg = irc_reader.ReadLine()
    msg

let SendPass(oauth:string) =
    irc_writer.WriteLine( sprintf "PASS %s" oauth)
    printColored colorInfo "Send PASS"

let SendNick(nick:string) =
    irc_writer.WriteLine( sprintf "NICK %s" nick )
    printColored colorInfo "Send NICK"

let SendJoin(channel:string) =
    irc_writer.WriteLine( sprintf "JOIN %s\n" channel )
    printColored colorInfo (sprintf "JOIN %s\n" channel)

let SendPong() =
    irc_writer.WriteLine( "PONG :tmi.twitch.tv" )
    printColored colorPing ("PONG")