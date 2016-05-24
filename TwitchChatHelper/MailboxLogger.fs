module MailboxLogger

open MessageTypes
open FileLogger

// Mailbox to log things to file.
// Saves rules and file settings.

// https://msdn.microsoft.com/en-us/visualfsharpdocs/conceptual/collections.map-module-[fsharp]

/// Save settings for file logger, channel and file. Key - channel. Value - filepath.
type ConcurrentLogSettings() =    
    let mutable _stack : Map<string,string> = Map.empty

    member this.Add key value =
        lock _stack (fun () -> 
            _stack <- _stack.Add(key, value))
    
    member this.Remove key =
        lock _stack (fun () -> 
            _stack <- _stack.Remove(key))    
    
    member this.Tryfind  key =
        lock _stack (fun () -> 
            match _stack.TryFind(key) with
            | Some(value) -> Some(value)
            | None -> None
        )    
    
    member this.Count =
        lock _stack (fun () -> 
             _stack.Count)

/// Instance of logg settings with:
/// Key - channel, Value - file.
let SettingsChannel = new ConcurrentLogSettings()

/// Process message logging.
type MailboxLogger () = 

    /// Create the agent
    static let agent = MailboxProcessor.Start(fun inbox -> 

        // the message processing function
        let rec messageLoop() = async{
            let! msg = inbox.Receive()

            // Process based on message type.
            match msg with 
            | ChanellMessage message ->              
                /// Try to fin file associated with channel and write if found.
                let logFile = SettingsChannel.Tryfind message.Channel
                match logFile with
                | Some file -> 
                    do! LogMessageAsync file message.Message
                | None -> ()
                return! messageLoop()
            | CommandsNotice message ->
                //FileLogger.LogMessageAsync "notice.txt" message.
                return! messageLoop()
            | _ -> 
                return! messageLoop()

            }

        // start the loop 
        messageLoop()
        )

    /// Post message
    static let postMessage (i:Message) = 
        agent.Post( i )

    // public interface to hide the implementation
    static member PostMessage (i:Message) = postMessage i 
