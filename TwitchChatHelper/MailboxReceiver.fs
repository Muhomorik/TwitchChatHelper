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
let cleanOld (unixStampOld:int)(unixStampNow:int) = MessageCounter.cleanOldCmd cmdCounter unixStampOld unixStampNow

/// Counter for received messages.
let reveicerCount() = cmdCounter.Count

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
            let oldTime = MessageCounter.deadUnixTime DateTime.Now
            cleanOld oldTime unixTime

            // Process based on message type.
            match msg with 
            | ChanellMessage message ->              
                // Log to file, can be done in main, but here will only run for rught message type.
                MailboxLogger.MailboxLogger.PostMessage msg
                return! messageLoop()
            | SuccConnection a-> 
                return! messageLoop()
            | Ping -> 
                // This may be a problem...
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
                // Leave channel when host leaves.
                let ch = a.Channel.Replace("#", "") // Remove # from channel
                let nick = a.NicknameAlterative
                match ch with
                | ch when ch = nick -> 
                    printfn "Leave %s because host left." a.Channel
                    MailboxSender.MailboxSender.PostPart a.Channel
                | _ -> ()

                return! messageLoop()            
            
            // Capabilities.
            | MembershipAck a -> 
                return! messageLoop()
            | MembershipMode a ->
                // Leave channel when host loses (+) mode.
                match a.Mode with
                | false ->
                    let ch = a.Channel.Replace("#", "") // Remove # from channel
                    let nick = a.Username
                    match ch with
                    | ch when ch = nick -> 
                        printfn "Leave %s because host lost mod." a.Channel
                        MailboxSender.MailboxSender.PostPart a.Channel
                    | _ -> () 
                | true -> ()
 
                return! messageLoop()

            // Commands
            | CommandsAck a -> 
                return! messageLoop()            
            | CommandsNotice a -> 
                return! messageLoop()            
            | CommandsHostTargetStart a -> 
                return! messageLoop()            
            | CommandsHostTargetStop a -> 
                return! messageLoop()
            | CommandsClearChatUser a -> 
                return! messageLoop()            
            | CommandsClearChat a -> 
                return! messageLoop()            
            | CommandsUserstate a -> 
                return! messageLoop()            
            | CommandsReconnect -> 
                return! messageLoop()            
            | CommandsRoomstate a -> 
                return! messageLoop()

            // Log unknown
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

