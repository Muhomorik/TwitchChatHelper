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
    Message :string}

/// Host starts message.
/// :tmi.twitch.tv HOSTTARGET #hosting_channel :target_channel [number]
type MessageCommandHostTargetStart = {
    TwitchGroup :string
    ChannelHosting:string 
    ChannelTarget:string 
    Number :string}