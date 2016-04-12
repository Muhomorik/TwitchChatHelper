module MessageTypes

/// Chanell message.
type MessageChanell = {
    Nickname:string 
    NameAddr:string
    Cmd:string
    Channel:string 
    Message:string}

/// Twitch informational message like 'hello'.
type MessageTwitchInfo = {
    TwitchAddr:string
    Code:string    
    Nickname:string 
    Message:string}

/// Twith PING message.
type MessageTwitchPing = {
    TwitchAddr:string}

/// Twitch message
type Message = 
  | Msg of MessageChanell
  | Info of MessageTwitchInfo
  | Ping of MessageTwitchPing
  | Other of string  // ofr not parsed messages.