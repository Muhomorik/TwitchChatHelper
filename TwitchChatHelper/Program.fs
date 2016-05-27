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
let processOneLine (ircReader:StreamReader) = 
    try    
        let msg_string = ircReader.ReadLine()
    
        match msg_string with
        | null -> () /// wtf?
        | _ -> 
            let msg = parseMessage msg_string
            PrintMsg msg        
            MailboxReceiver.MailboxReceiver.PostMessage msg
            printStats() // TODO: better print. Remake.

            // TODO: remove. Temp logger to find commands that  are missing in manual.
            match msg with 
            | CommandsNotice message ->              
                FileLogger.LogMessageAsync "notice.txt" msg_string |> Async.RunSynchronously        
            | _ -> ()
    with
        | :? IOException as ex ->
            printfn "Got IOException. Reconecting to the network..."
            ircReader.Close()            
            System.Threading.Thread.Sleep 1000
        | _ ->                 
            // don't handle any other cases 
            reraise()         


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

    /// Add to log settings. TODO: kind of ugly.
    MailboxLogger.SettingsChannel.Add channel logFile
    
    // TODO: from cli
    MailboxSender.PostReqCapabilities()
    MailboxSender.PostReqCommands() 
    
    // Login and join.
    MailboxSender.PostAndReplyLogin oauth nick |> ignore // must wait for result.
    MailboxSender.PostAndReplyJoin channel |> ignore // must wait for result.
   
    
    let irc_reader = Connection.GetReaderInstance()

    if irc_reader.IsSome then
        // Read untill end.
        while not irc_reader.Value.EndOfStream do
            processOneLine irc_reader.Value
    else
        printfn "Connection failed."

    printfn "Done"
    0 // return an integer exit code
