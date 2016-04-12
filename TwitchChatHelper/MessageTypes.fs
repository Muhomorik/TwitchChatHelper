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

/// Twitch JOIN response with nicknames, code 353.
type MessageTwitchChanellNicknames = {
    TwitchAddr:string
    Code:string    
    Nickname1:string 
    Channel:string
    Nickname2:string}

/// Twitch JOIN message.
/// :twitch_username!twitch_username@twitch_username.tmi.twitch.tv JOIN #channel
type MessageTwitchChanellJoin = {
    Nickname:string     
    NameAddr:string
    Cmd:string
    Channel:string}

/// Twitch end if nicknames.
/// :twitch_username.tmi.twitch.tv 366 twitch_username #channel :End of /NAMES list
type MessageTwitchChanellEndNames = {
    NameAddr:string    
    Code:string    
    Nickname:string     
    Channel:string
    Message:string}

/// Twitch message
type Message = 
  | Msg of MessageChanell
  | Info of MessageTwitchInfo
  | Ping of MessageTwitchPing
  | ChanellJoin of MessageTwitchChanellJoin
  | Nicknames of MessageTwitchChanellNicknames
  | NicknamesEnd of MessageTwitchChanellEndNames
  | Other of string  // ofr not parsed messages.