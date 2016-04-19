module MailboxReceiver

open MessageTypes
open ConsoleOutHelpers
open MessagePrint

type parserMsg =
    | Msg of Message
    | Pong // resonse to ping.
    | Done

type MailboxReceiver () = 

    /// Create the agent
    static let agent = MailboxProcessor.Start(fun inbox -> 

        // the message processing function
        let rec messageLoop() = async{
            let! msg = inbox.Receive()

            match msg with 
            | Msg message ->              
                PrintMsg message
                return! messageLoop()
            | Pong -> 
                printColored colorPing ("PONG")
                return! messageLoop()
            | Done -> 
                printfn "Done!"
                return () 
            }

        // start the loop 
        messageLoop()
        )

    // public interface to hide the implementation
    static member DoDone () = agent.Post( Done )
    static member PostMessage (i:Message) = agent.Post( Msg(i) )
    static member PostPong () = agent.Post( Pong )

//let x = new MessageBasedCounter()
//MessageBasedCounter.DoDone()