module CommandTypes

open MessageTypes
open MessageTypesBasic

/// Commands that can be send.

type TwitchCommandLogin = {
    Username:string
    Oauth:string}

/// Twitch message
type TwitchCommand = 
    /// Oauth pass string
    | Login of TwitchCommandLogin*AsyncReplyChannel<bool> // TODO: as type
    | ChannelJoin of string*AsyncReplyChannel<bool>
    | ChanellMessage of ChanellMsg // same as in recv.
    ///  Leave a channel.
    | ChannelPart of string

    // Capabilities
    | ReqMembership
    
    // Commands
    | ReqCommands
