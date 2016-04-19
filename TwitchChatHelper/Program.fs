// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open System
open System.IO
open System.Text
open System.Net
open System.Net.Sockets
open System.Text.RegularExpressions

//open Argu

open MyCfg

open MessageTypes
open MessageParsers
open TwitchCommands
open CliArguments

// File writer
let testWrite(line:string) = async {
    use sw = new StreamWriter(new FileStream("twitch_log_other.txt",  FileMode.Append, FileAccess.Write, FileShare.Write, bufferSize= 4096, useAsync= true))
    do! sw.WriteLineAsync(line) |>  Async.AwaitTask
}

/// Process received message.
let ProcessMessageAsync (fileWriter:StreamWriter) (ircClient:TcpClient) = async{
    let! msgText = ReceiveMessageAsync ircClient

    let msg = parseMessage msgText
    MailboxReceiver.MailboxReceiver.PostMessage(msg)

    match msg with
    | Msg m -> 
        fileWriter.WriteLine(m.Message)    

    | Ping p -> 
        do! SendPongAsync ircClient
        MailboxReceiver.MailboxReceiver.PostPong()
    | Other o -> do! testWrite msgText // async to err file (full line), should not be many.
    | _  -> ()
}

// big chans problem
// http://stackoverflow.com/questions/36116231/using-writelineasync-in-f
// https://discuss.dev.twitch.tv/t/irc-client-can-send-but-not-receive/1533
[<EntryPoint>]
let main argv = 
    Console.OutputEncoding <- Encoding.Unicode    
    printf "%s \n" "Работаю!"  // Must print nice, or utf not set properly! "Working" iin russian.

    // Check login details and quit with error if missing.
    if (String.IsNullOrWhiteSpace(nick)) then printfn "Nickname is empty"; exit 5
    if (String.IsNullOrWhiteSpace(oauth)) then printfn "Oauth is empty"; exit 5

    /// Parsed CLI paramss input.
    let results = parser.Parse(argv)
    
    /// Log file
    let logFile = parseFileLog(results)
    
    /// Channel from cli or console.
    let channel = results   |> parseChannel
                            |> ReadChannelFromConsole 

    // Login and join channel.
    use irc_client = new TcpClient() |> IrcConnectAsync |> Async.RunSynchronously
    use irc_reader = IrcReaderAsync irc_client          |> Async.RunSynchronously        

    // Login and join.
    LoginAndJoinAsync oauth nick channel irc_client     |> Async.StartImmediate

    /// File to write message log.
    use fileWriter = new StreamWriter(logFile, true ) // append
    fileWriter.AutoFlush <- true
    
    while not irc_reader.EndOfStream do
        ProcessMessageAsync fileWriter irc_client  |> Async.RunSynchronously

    printfn "Done"
    0 // return an integer exit code
