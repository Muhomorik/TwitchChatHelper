module MessageTypes

// Messages can be received.

/// Twitch informational message like 'hello'.
/// :tmi.twitch.tv 003 twitch_username :This server is rather new
type MessageSuccConn = {
    TwitchGroup:string
    Code:string    
    Nickname:string 
    Message:string}

/// Invalid commands.
/// :tmi.twitch.tv 421 twitch_username WHO :Unknown command
type MessageInvalidCommand = {
    TwitchGroup:string
    Code:string    
    Nickname:string 
    Message:string}

/// Twitch JOIN message.
/// :twitch_username!twitch_username@twitch_username.tmi.twitch.tv JOIN #channel
type MessageChanellJoin = {
    NicknameAlterative:string     
    Nickname:string
    Channel:string}

/// Twitch JOIN response with nicknames, code 353.
/// :twitch_username.tmi.twitch.tv 353 twitch_username = #channel :twitch_username
type MessageTwitchChanellNicknames = {
    Nickname:string
    Code:string    
    NicknameJoin:string 
    Channel:string
    Nicknames:string} // TODO: as list

/// Twitch end if nicknames.
/// :twitch_username.tmi.twitch.tv 366 twitch_username #channel :End of /NAMES list
type MessageTwitchChanellEndNames = {
    NameAddr:string    
    Code:string    
    Nickname:string     
    Channel:string}

/// PART: Leaving a chat room.
/// :twitch_username!twitch_username@twitch_username.tmi.twitch.tv PART #channel
type MessageChanellLeave = {
    NicknameAlterative:string     
    Nickname:string
    Channel:string}

/// PRIVMSG: Sending a message, basic.
/// :twitch_username!twitch_username@twitch_username.tmi.twitch.tv PRIVMSG #channel :message here
type ChanellMsg = {
    NicknameAlterative:string 
    Nickname:string
    Channel:string 
    Message:string}

/// Twitch message
type Message = 
  | SuccConnection of MessageSuccConn
  | Ping
  | InvalidCommand of MessageInvalidCommand
  | ChannelJoin of MessageChanellJoin
  | ChannelNicknames of MessageTwitchChanellNicknames
  | ChannelNicknamesEnd of MessageTwitchChanellEndNames
  | ChannelLeave of MessageChanellLeave
  | ChanellMessage of ChanellMsg
  | Other of string  // ofr not parsed messages.