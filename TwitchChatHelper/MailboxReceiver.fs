module MailboxReceiver

open MessageTypes
open System
open MailboxSender
open System.Collections.Concurrent

/// thread-safe first in-first out (FIFO) collection
let private cmdCounter = ConcurrentQueue<int>()

/// Local Enqueue function.
let cmdEnqueue() = MessageCounter.cmdEnqueue cmdCounter

/// Remove old values from counter (partial). Add current unix time as parameter.
let cleanOld = MessageCounter.cleanOldCmd cmdCounter

/// Process received messages.
type MailboxReceiver () = 

    /// Create the agent
    static let agent = MailboxProcessor.Start(fun inbox -> 

        // the message processing function
        let rec messageLoop() = async{
            let! msg = inbox.Receive()

            // Message counter.
            // Remove old values.
            let unixTime = MessageCounter.dateTimeToUnixTime DateTime.Now
            cleanOld unixTime

            // Process based on message type.
            match msg with 
            | ChanellMessage message ->              
                /// TODO: handle message.
                return! messageLoop()
            | SuccConnection a-> 
                return! messageLoop()
            | Ping -> 
                MailboxSender.PostPong()
                return! messageLoop()
            | InvalidCommand a -> 
                return! messageLoop()
            | ChannelJoin a -> 
                return! messageLoop()
            | ChannelNicknames a -> 
                return! messageLoop()
            | ChannelNicknamesEnd a -> 
                return! messageLoop()
            | ChannelLeave a -> 
                return! messageLoop()
            | Other a ->
                do! FileLogger.LogWriteOtherAsync a // async to err file (full line), should not be many.
                return! messageLoop()
            }

        // start the loop 
        messageLoop()
        )

    /// Post message
    static let postMessage (i:Message) = 
        cmdEnqueue()
        agent.Post( i )

    // public interface to hide the implementation
    static member PostMessage (i:Message) = postMessage i 

