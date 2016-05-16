module MailboxSender

open System
open System.Collections.Concurrent

open TwitchCommands
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
let waitForUnlock = MessageCounter.waitForUnlock30 cmdCounter

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
            waitForUnlock unixTime // BLOCKING sleep.

            // Handle message
            match msg with 
            | ChanellMessage message ->              
                // TODO: message
                return! messageLoop()
            
            | Login (lg, replyChannel) -> 
                let irc_writer = Connection.GetWriterInstance()

                printfn "Login"
                SendPass irc_writer lg.Oauth
                SendNick irc_writer lg.Username
                
                replyChannel.Reply(true)
                return! messageLoop()
            
            | ChannelJoin (channel, replyChannel) -> 
                let irc_writer = Connection.GetWriterInstance()
                
                printfn "Join %s" channel
                do! SendJoinAsync irc_writer channel

                replyChannel.Reply(true)
                return! messageLoop()
            }

        // start the loop 
        messageLoop()
        )

    /// Post Pong.
    static let postPong() = 
        cmdEnqueue()
        let irc_writer = Connection.GetWriterInstance()
        SendPong irc_writer

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
