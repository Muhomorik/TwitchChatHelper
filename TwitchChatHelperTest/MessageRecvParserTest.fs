module MessageRecvParsersTest

open System
open System.IO
open System.Text.RegularExpressions

open NUnit.Framework
open FsUnit

open MessageTypes
open MessageParsers

// Follows this doc: https://github.com/justintv/Twitch-API/blob/master/IRC.md

//
// Upon a Successful Connection
// 

let msg_succ_con_1 = ":tmi.twitch.tv 001 twitch_username :Welcome, GLHF!"
let msg_succ_con_2 = ":tmi.twitch.tv 002 twitch_username :Your host is tmi.twitch.tv"
let msg_succ_con_3 = ":tmi.twitch.tv 003 twitch_username :This server is rather new"
let msg_succ_con_4 = ":tmi.twitch.tv 004 twitch_username :-"
let msg_succ_con_5 = ":tmi.twitch.tv 372 twitch_username :You are in a maze of twisty passages, all alike."
let msg_succ_con_6 = ":tmi.twitch.tv 376 twitch_username :>"

[<TestFixture>]
type ``Recv: Upon a Successful Connection`` () = 
    static member TestData =
        [|
            [|msg_succ_con_1, "tmi.twitch.tv" , "001", "twitch_username", "Welcome, GLHF!"|];
            [|msg_succ_con_2, "tmi.twitch.tv" , "002", "twitch_username", "Your host is tmi.twitch.tv"|];
            [|msg_succ_con_3, "tmi.twitch.tv" , "003", "twitch_username", "This server is rather new"|];
            [|msg_succ_con_4, "tmi.twitch.tv" , "004", "twitch_username", "-"|];
            [|msg_succ_con_5, "tmi.twitch.tv" , "372", "twitch_username", "You are in a maze of twisty passages, all alike."|];
            [|msg_succ_con_6, "tmi.twitch.tv" , "376", "twitch_username", ">"|];
        |]

    [<TestCaseSource("TestData")>]
    member x.``test regex successful connection`` (testData:(string*string*string*string*string)) =
        let cmd, serv, code, nickname, text = testData
        
        let retv_msg = parseMessage cmd

        match retv_msg with
        | SuccConnection m -> 
            m.TwitchGroup |> should equal serv
            m.Code |> should equal code
            m.Nickname |> should equal nickname
            m.Message |> should equal text
        | _ -> 
            true |> should equal false

//
// PING
// 

[<Test>]
let``Recv: test regex ping``()=
    let msg = "PING :tmi.twitch.tv"
    let cmd = parseMessage msg

    match cmd with
    | Ping  -> 
        true |> should equal true       
    | _ -> true |> should equal false 

//
// Invalid commands you can send
// WHO #channel
// 

let msg_invalidCommand = ":tmi.twitch.tv 421 twitch_username WHO :Unknown command"

[<TestFixture>]
type ``Recv: test regex invalid command`` () = 
    static member TestData =
        [|
            [|msg_invalidCommand, "tmi.twitch.tv" , "421", "twitch_username", "Unknown command"|];
        |]

    [<TestCaseSource("TestData")>]
    member x.``Recv: test regex invalid command`` (testData:(string*string*string*string*string)) =
        let cmd, twitchGroup, code, nickname, message = testData
        
        let retv_msg = parseMessage cmd

        match retv_msg with
        | InvalidCommand m -> 
            m.TwitchGroup |> should equal twitchGroup
            m.Code |> should equal code
            m.Nickname |> should equal nickname
            m.Message |> should equal message
        | _ -> 
            true |> should equal false


//
// JOIN: Opening up a chat room
// JOIN #channel
//

[<Test>]
let``Recv: test regex channel Join``()=
    let msg = ":twitch_username!twitch_username@twitch_username.tmi.twitch.tv JOIN #channel"
    let cmd = parseMessage msg

    match cmd with     
    | ChannelJoin m -> 
        m.NicknameAlterative |> should equal "twitch_username"
        m.Nickname |> should equal "twitch_username"
        m.Channel |> should equal "#channel"
    | _ -> true |> should equal false

 
[<Test>]
let``Recv: test regex channel nicknames``()=
    let msg = ":twitch_username.tmi.twitch.tv 353 twitch_username = #channel :twitch_username"
    let cmd = parseMessage msg

    match cmd with     
    | ChannelNicknames m -> 
        m.Nickname |> should equal "twitch_username"
        m.Code |> should equal "353"
        m.NicknameJoin |> should equal "twitch_username"
        m.Channel |> should equal "#channel"
        m.Nicknames |> should equal "twitch_username"
    | _ -> true |> should equal false


[<Test>]
let``Recv: test regex channel nicknames end``()=
    let msg = ":twitch_username.tmi.twitch.tv 366 twitch_username #channel :End of /NAMES list"
    let cmd = parseMessage msg

    match cmd with
    | ChannelNicknamesEnd m ->
        m.NameAddr |> should equal "twitch_username"
        m.Code |> should equal "366"
        m.Nickname |> should equal "twitch_username"
        m.Channel |> should equal "#channel"
    | _ -> true |> should equal false


//
// PART: Leaving a chat room
// PART #channel
// 

[<Test>]
let``Recv: test regex channel part/leave``()=
    let msg = ":twitch_username!twitch_username@twitch_username.tmi.twitch.tv PART #channel"
    let cmd = parseMessage msg

    match cmd with
    | ChannelLeave m ->
        m.NicknameAlterative |> should equal "twitch_username"
        m.Nickname |> should equal "twitch_username"
        m.Channel |> should equal "#channel"
    | _ -> true |> should equal false

//
// PRIVMSG: Sending a message
// PRIVMSG #channel :Message to send
// 

let msg_msg1 = ":twitch_username!twitch_username@twitch_username.tmi.twitch.tv PRIVMSG #channel :message here"

// Test EN locale.
let msg_en = ":nickname!nickname@nickname.tmi.twitch.tv PRIVMSG #gamer2 :So is this the stream with giveaways?"
// Test russian locale.
let msg_ru = ":nightbot!nightbot@nightbot.tmi.twitch.tv PRIVMSG #lady__angel :хочешь порадовать девушку или выразить благодарность"

// TODO: find and test nickname with alternative.

[<TestFixture>]
type ``Recv: test regex channel message`` () = 
    static member TestData =
        [|
            [|msg_msg1, "twitch_username" , "twitch_username", "#channel", "message here"|];
            [|msg_en, "nickname" , "nickname", "#gamer2", "So is this the stream with giveaways?"|];
            [|msg_ru, "nightbot" , "nightbot", "#lady__angel", "хочешь порадовать девушку или выразить благодарность"|];
        |]

    [<TestCaseSource("TestData")>]
    member x.``test regex successful connection`` (testData:(string*string*string*string*string)) =
        let cmd, nicknameAlterative, nickname, channel, message = testData
        
        let retv_msg = parseMessage cmd

        match retv_msg with
        | ChanellMessage m ->
            m.NicknameAlterative |> should equal nicknameAlterative
            m.Nickname |> should equal nickname
            m.Channel |> should equal channel
            m.Message |> should equal message
        | _ -> true |> should equal false


//
// Twitch Capabilities.
// TODO: move from here.
//

//
// Membership
// CAP REQ :twitch.tv/membership
// 

let cap_membership = ":tmi.twitch.tv CAP * ACK :twitch.tv/membership"

//
// NAMES
// The list of current chatters in a channel:
// 

let cap_names_1 = ":twitch_username.tmi.twitch.tv 353 twitch_username = #channel :twitch_username user2 user3"
let cap_names_2 = ":twitch_username.tmi.twitch.tv 353 twitch_username = #channel :user5 user6 nicknameN"
let cap_names_3 = ":twitch_username.tmi.twitch.tv 366 twitch_username #channel :End of /NAMES list"

//
// JOIN
// Someone joined a channel:
// 

let cap_join = ":twitch_username!twitch_username@twitch_username.tmi.twitch.tv JOIN #channel"

//
// PART
// Someone left a channel:
//

let cap_part = ":twitch_username!twitch_username@twitch_username.tmi.twitch.tv PART #channel"

//
// MODE
// Someone gained or lost operator:
// 

let cap_mode_plus = ":jtv MODE #channel +o operator_user"
let cap_mode_minus = ":jtv MODE #channel -o operator_user"

//
// Commands
// CAP REQ :twitch.tv/commands
// 

let cmd_common = ":tmi.twitch.tv CAP * ACK :twitch.tv/commands"

//
// NOTICE: General notices from the server
// 

// TODO: msg-id as unum

let notice = "@msg-id=slow_off :tmi.twitch.tv NOTICE #channel :This room is no longer in slow mode."

//
// HOSTTARGET
// Number is assumed to be the number of viewers watching the host.
// 

let host_start = ":tmi.twitch.tv HOSTTARGET #hosting_channel :target_channel [number]"
let host_stop =  ":tmi.twitch.tv HOSTTARGET #hosting_channel :- [number]"

//
// CLEARCHAT
// 

/// Username is timed out on channel:
let clear_timeout = ":tmi.twitch.tv CLEARCHAT #channel :twitch_username"

/// Chat is cleared on channel:
let clear_chat= ":tmi.twitch.tv CLEARCHAT #channel"

//
// USERSTATE
// Use with tags CAP. See USERSTATE tags below.
// 

let userstate = ":tmi.twitch.tv USERSTATE #channel"

//
// TODO: RECONNECT
// 

// Twitch IRC processes ocasionally need to be restarted. 
// When this happens, clients that have requested the IRCv3 twitch.tv/commands capability are issued a RECONNECT. 
// After a short period of time, the connection will be closed.

//
// ROOMSTATE
// Use with tags CAP. See ROOMSTATE tags below.
// 

let roomstate = ":tmi.twitch.tv ROOMSTATE #channel"

//
// Tags
// CAP REQ :twitch.tv/tags
// Adds IRC v3 message tags to PRIVMSG, USERSTATE, NOTICE and GLOBALUSERSTATE (if enabled with commands CAP)

let tags = ":tmi.twitch.tv CAP * ACK :twitch.tv/tags"

//
// PRIVMSG
// 

let IRCv3_PRIVMSG = "@color=#0D4200;display-name=TWITCH_UserNaME;emotes=25:0-4,12-16/1902:6-10;mod=0;subscriber=0;turbo=1;user-id=1337;user-type=global_mod :twitch_username!twitch_username@twitch_username.tmi.twitch.tv PRIVMSG #channel :Kappa Keepo Kappa"

//
// USERSTATE
// 

let IRCv3_USERSTATE  = "@color=#0D4200;display-name=TWITCH_UserNaME;emote-sets=0,33,50,237,793,2126,3517,4578,5569,9400,10337,12239;mod=1;subscriber=1;turbo=1;user-type=staff :tmi.twitch.tv USERSTATE #channel"

//
// GLOBALUSERSTATE
// GLOBALUSERSTATE is sent on successful login, if the capabilities have been acknowledged before then.
// 

let IRCv3_GLOBALUSERSTATE = "@color=#0D4200;display-name=TWITCH_UserNaME;emote-sets=0,33,50,237,793,2126,3517,4578,5569,9400,10337,12239;turbo=0;user-id=1337;user-type=admin :tmi.twitch.tv GLOBALUSERSTATE"

//
// ROOMSTATE
// ROOMSTATE is sent when joining a channel and every time one of the chat room settings, like slow mode, change
// 

let IRCv3_ROOMSTATE = "@broadcaster-lang=;r9k=0;slow=0;subs-only=0 :tmi.twitch.tv ROOMSTATE #channel"

