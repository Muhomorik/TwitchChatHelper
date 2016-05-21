module MessageTypes

// Messages can be received.

open MessageTypesBasic
open MessageCapabilitiesTypes
open MessageCommandsTypes

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

  // Commands
  | CommandsAck of bool
  | CommandsNotice of MessageCommandNotice
  
  | CommandsHostTargetStart of MessageCommandHostTargetStart
  | CommandsHostTargetStop of MessageCommandHostTargetStop
  
  | CommandsClearChatUser of MessageCommandClearchatUser
  | CommandsClearChat of MessageCommandClearChat

  | Other of string  // ofr not parsed messages.