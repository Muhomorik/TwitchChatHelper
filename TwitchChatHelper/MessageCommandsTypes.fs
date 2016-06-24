module MessageCommandsTypes

/// Twitch message
type MsgId = 
  /// This room is now in subscribers-only mode.
  | SubsOn
  /// This room is no longer in subscribers-only mode.
  | SubsOff
  /// This room is now in slow mode. You may send messages every slow_duration seconds.
  | SlowOn
  /// This room is no longer in slow mode.
  | SlowOff
  /// This room is now in r9k mode.
  | R9kOn
  /// This room is no longer in r9k mode.
  | R9kOff
  /// Now hosting target_channel.
  | HostOn
  /// Exited host mode.
  | HostOff

/// NOTICE.
/// @msg-id=slow_off :tmi.twitch.tv NOTICE #channel :This room is no longer in slow mode.
type MessageCommandNotice = {
    MsgId :MsgId
    TwitchGroup :string
    Channel:string 
    Message :string}  // TODO: add time in seconds as int to type, slow_on.

/// Host starts message.
/// :tmi.twitch.tv HOSTTARGET #hosting_channel :target_channel [number]
type MessageCommandHostTargetStart = {
    TwitchGroup :string
    ChannelHosting:string 
    ChannelTarget:string 
    Number :string}

/// Host stops message.
/// :tmi.twitch.tv HOSTTARGET #hosting_channel :- [number]
type MessageCommandHostTargetStop = {
    TwitchGroup :string
    ChannelHosting:string 
    Number :string}

/// Username is timed out on channel.
/// :tmi.twitch.tv CLEARCHAT #channel :twitch_username
type MessageCommandClearchatUser = {
    TwitchGroup :string
    Channel :string 
    Nickname :string}

/// Chat is cleared on channel.
/// :tmi.twitch.tv CLEARCHAT #channel
type MessageCommandClearChat = {
    TwitchGroup :string
    Channel :string}

/// Use with tags CAP. See USERSTATE tags.
/// :tmi.twitch.tv USERSTATE #channel
type MessageCommandUserstate = {
    TwitchGroup :string
    Channel :string}

/// ROOMSTATE
/// Use with tags CAP. See ROOMSTATE tags.
/// :tmi.twitch.tv ROOMSTATE #channel
type MessageCommandRoomstate = {
    TwitchGroup :string
    Channel :string}