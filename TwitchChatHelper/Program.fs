// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open MyCfg

open System
open System.IO
open System.Text

open MessageParsers
open CliArguments
open MessagePrint
open MailboxSender
open MessageTypes
open ConsoleOutHelpers

let mutable lastStamp  = 1

/// Print current Mailbox counters (temp, dev only).
let printStats() = 
    if lastStamp = 30 then
        lastStamp <- 1
        
        let prt = sprintf "STATS. Reader: %d, writer: %d." (MailboxReceiver.reveicerCount()) (senderCount())
        printColored ConsoleColor.DarkRed prt
    else
        lastStamp <- lastStamp + 1

/// Process one read from input stream.
let processOneLine (ircReader:StreamReader)(logFile :string) = 
    let msg_string = ircReader.ReadLine()
    
    match msg_string with
    | null -> () /// wtf?
    | _ -> 
        let msg = parseMessage msg_string
        PrintMsg msg        
        MailboxReceiver.MailboxReceiver.PostMessage msg
        printStats() // TODO: better print. Remake.

        // TODO: this should be lgged inside the mailbox. But there is now way to send log file as parameter.
        // Alt is to dynnamically create file with tules there (like chan name).
        match msg with 
        | ChanellMessage message ->              
            FileLogger.LogMessageAsync logFile message.Message |> Async.RunSynchronously
        | CommandsNotice message ->              
            FileLogger.LogMessageAsync "notice.txt" msg_string |> Async.RunSynchronously        
        | _ -> ()
        


// big chans problem
// http://stackoverflow.com/questions/36116231/using-writelineasync-in-f
// https://discuss.dev.twitch.tv/t/irc-client-can-send-but-not-receive/1533
[<EntryPoint>]
let main argv = 
    // https://en.wikipedia.org/wiki/Text_mode
    //Console.WindowWidth <- 132;
    //Console.WindowHeight <- 50;
     
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

    let irc_reader = Connection.GetReaderInstance()

    // Login and join.
    MailboxSender.PostAndReplyLogin oauth nick |> ignore // must wait for result.
    MailboxSender.PostAndReplyJoin channel |> ignore // must wait for result.
    
    // TODO: from cli
    MailboxSender.PostReqCapabilities()

    // Read untill end.
    while not irc_reader.EndOfStream do
        processOneLine irc_reader logFile
    
    printfn "Done"
    0 // return an integer exit code
