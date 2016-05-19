module MessageTypes

// Messages can be received.

open MessageTypesBasic
open MessageCapabilitiesTypes

/// Twitch message
type Message = 
  // Basic.
  | SuccConnection of MessageSuccConn
  | Ping
  | InvalidCommand of MessageInvalidCommand
  | ChannelJoin of MessageChanellJoin
  | ChannelNicknames of MessageTwitchChanellNicknames
  | ChannelNicknamesEnd of MessageTwitchChanellEndNames
  | ChannelLeave of MessageChanellLeave
  | ChanellMessage of ChanellMsg

  // Capabilities.
  | MembershipAck of bool
  | MembershipMode of MessageMembershipMode

  | Other of string  // ofr not parsed messages.