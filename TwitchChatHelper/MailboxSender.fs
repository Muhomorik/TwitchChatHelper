module MailboxSender

open System
open System.Collections.Concurrent

open ConsoleOutHelpers
open TwitchCommands
open TwitchCommandsCapabilities
open TwitchCommandsCommandsCapabilities
open CommandTypes

// Mailbox that sends messages to twitter.

// Partial application and handlers around counters.

/// thread-safe first in-first out (FIFO) collection
let private cmdCounter = ConcurrentQueue<int>()

/// Local Enqueue function.
let cmdEnqueue() = MessageCounter.cmdEnqueue cmdCounter

/// Remove old values from counter (partial). Add current unix time as parameter.
let cleanOld = MessageCounter.cleanOldCmd cmdCounter

/// Wait for query to unlock (blocking, partial).
let waitForUnlock (unixTime:int) = MessageCounter.waitForUnlock30 cmdCounter unixTime

/// Counter for send messages.
let senderCount() = cmdCounter.Count

// TODO: there should be no prints here.

/// Process output messages to stream.
type MailboxSender () =  

    /// Create the agent
    static let agent = MailboxProcessor.Start(fun inbox -> 

        // the message processing function
        let rec messageLoop() = async{
            let! msg = inbox.Receive()

            // Message limiter/wait if too many.
            // Remove old values and wait untill lock is gone (if any).
            let unixTime = MessageCounter.dateTimeToUnixTime DateTime.Now
            let oldTime = MessageCounter.deadUnixTime DateTime.Now
            
            cleanOld oldTime unixTime
            waitForUnlock unixTime 0// BLOCKING sleep.

            // Handle message
            match msg with 
            | ChanellMessage message ->              
                // TODO: message
                return! messageLoop()
            
            | Login (lg, replyChannel) -> 
                Connection.GetWriterInstance() |> Option.map (fun irc_writer -> 
                    printfn "Login"
                    SendPass irc_writer lg.Oauth
                    SendNick irc_writer lg.Username
                ) |> ignore
                replyChannel.Reply(true)
                return! messageLoop()
            
            | ChannelJoin (channel, replyChannel) -> 
                let irc_writer = Connection.GetWriterInstance()
                
                match irc_writer with
                | Some ircWriter ->
                    printfn "Join %s" channel                    
                    do! SendJoinAsync ircWriter channel
                | None -> printfn "Skip Join %s" channel

                replyChannel.Reply(true)
                return! messageLoop()            
            
            | ChannelPart (channel) -> 
                let irc_writer = Connection.GetWriterInstance()
                
                match irc_writer with
                | Some ircWriter ->
                    printfn "Part %s" channel
                    do! SendPartAsync ircWriter channel
                | None -> printfn "Skip Part %s" channel
                
                return! messageLoop()             
            // Capabilities
            | ReqMembership -> 
                let irc_writer = Connection.GetWriterInstance()
                 
                match irc_writer with
                | Some ircWriter ->
                    printfn "Req Capabilities"
                    do! SendCapabilitiesMembershipAsyns ircWriter
                | None -> printfn "Skip Req Capabilities"                
                
                return! messageLoop()            
            
            // Commands
            | ReqCommands -> 
                let irc_writer = Connection.GetWriterInstance()
                
                match irc_writer with
                | Some ircWriter ->
                    printfn "Req Commands"
                    do! SendCapabilitiesCommandsAsyns ircWriter
                | None ->printfn "Skip Req Commands"

                return! messageLoop()
            }

        // start the loop 
        messageLoop()
        )

    /// Post Pong.
    // TODO: must deq. here or counter increasing only w/o blocking.
    static let postPong() = 
        cmdEnqueue()

        // TODO: remove this ugly fix in sender.
        let unixTime = MessageCounter.dateTimeToUnixTime DateTime.Now
        let oldTime = MessageCounter.deadUnixTime DateTime.Now
        cleanOld oldTime unixTime

        printColored colorPing "PONG"
        let irc_writer = Connection.GetWriterInstance()
        
        match irc_writer with
        | Some ircWriter ->
            SendPong ircWriter
        | None ->printfn "Skip Pong"        
        
    /// Post Login.
    static let postLogin(oauth:string)(username:string) = agent.PostAndReply(fun replyChannel -> 
        cmdEnqueue()
        cmdEnqueue() // sends 2 cmds.
        let vr :TwitchCommandLogin = {Username = username; Oauth = oauth}
        Login (vr, replyChannel) )

    /// Post Join.
    static let postJoin (channel:string) = agent.PostAndReply(fun replyChannel -> 
        cmdEnqueue()
        ChannelJoin (channel, replyChannel))
    
    /// Post Part.
    static let postPart (channel:string) = 
        cmdEnqueue()
        agent.Post( ChannelPart channel)

    //
    // Capabilities
    //
    
    static let postReqCapabilities () = 
        cmdEnqueue()
        agent.Post( ReqMembership )

    //
    // Commands
    //

    static let postReqCommands () = 
        cmdEnqueue()
        agent.Post( ReqCommands )

    //
    // Hide implementation.
    //

    /// Post Message.
    static member PostMessage (i:TwitchCommand) = agent.Post( i )
    
    /// Immediately post PONG, no query here.
    static member PostPong () = postPong()

    /// Send Login (NICK + PASS).
    static member PostAndReplyLogin (oauth:string)(username:string) = postLogin (oauth:string)(username:string)
    
    /// Send JOIN.
    static member PostAndReplyJoin (channel:string) = postJoin (channel:string)     
    
    /// Send PART.
    static member PostPart (channel:string) = postPart channel    
    
    /// Adds membership state event (NAMES, JOIN, PART, or MODE) functionality.
    static member PostReqCapabilities () = postReqCapabilities ()
    
    /// Enables USERSTATE, GLOBALUSERSTATE, ROOMSTATE, HOSTTARGET, NOTICE and CLEARCHAT raw commands.
    static member PostReqCommands () = postReqCommands ()
