module FileLogger

open System.IO

// TODO: create log folder if missing

// Write unknown command to other log async.
let LogWriteOtherAsync (line:string) = async {
    use sw = new StreamWriter(new FileStream("twitch_log_unknown.txt",  FileMode.Append, FileAccess.Write, FileShare.Write, bufferSize= 4096, useAsync= true))
    sw.AutoFlush <- true
    do! sw.WriteLineAsync(line) |>  Async.AwaitTask
}

// Write message to log async.
let LogMessageAsync (file:string)(line:string) = async {
    use sw = new StreamWriter(new FileStream(file,  FileMode.Append, FileAccess.Write, FileShare.Write, bufferSize= 4096, useAsync= true))
    sw.AutoFlush <- true
    do! sw.WriteLineAsync(line) |>  Async.AwaitTask
}