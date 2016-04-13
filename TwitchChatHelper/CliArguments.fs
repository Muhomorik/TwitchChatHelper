module CliArguments

open System
open Argu

/// CLI arguments.
type CLIArguments =
    | Channel of string
    | FileLog of string
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Channel _ -> "Channel, starting with a #."
            | FileLog _ -> "File to log messages."

// Example:
// --channel /var/run
// --filelog twitch_msg_log.txt

/// Default log file.
[<Literal>]
let defaultFile = "twitch_msg_log.txt"

/// Build the argument parser
let parser = ArgumentParser.Create<CLIArguments>()

/// Post-process file and check that filename and with .txt.
let postProcessFile (p:string) = 
    if not (p.EndsWith(".txt")) then invalidArg "FileLog" "Log file must end with .txt" 
    else p

/// Parse log filename from cli or return dafault one.
let parseFileLog (results:ParseResults<CLIArguments>)= 
    match (results.Contains <@ FileLog @>) with
    | true -> 
        let file =  results.PostProcessResults (<@ FileLog @>, postProcessFile) 
                    |> List.head
        printfn "Using msg file: %s" file
        file
    | false -> 
        printfn "Using default %s" defaultFile
        defaultFile

/// Post-process channel and check that channel starts with #.
let postProcessChannel (p:string) = 
    if not (p.StartsWith("#")) then invalidArg "Channel" "Channel name must start with #." 
    else p


/// Parse log filename from cli.
let parseChannel (results:ParseResults<CLIArguments>)= 
    match (results.Contains <@ Channel @>) with
    | true -> 
        let ch =  results.PostProcessResults (<@ Channel @>, postProcessChannel) 
                    |> List.head
        //printfn "Using msg file: %s" file
        Some(ch)
    | false -> 
        printfn "Using default %s" defaultFile
        None

/// Read channel from console.        
let ReadChannelFromConsole (lines: string option)= 
    
    // If Some - fallthrough
    // None - enter lines.
    match lines with
    | Some a -> a
    | None -> 
        printfn "Enter a channel starting with a # "
        
        let newLine = Console.ReadLine() 
        match newLine with
        | null -> 
            // null if no more lines are available.
            printfn "No input provided. Exit"
            exit 0
        | "" ->
            printfn "No input provided. Exit"
            exit 0
        | s -> postProcessChannel s            