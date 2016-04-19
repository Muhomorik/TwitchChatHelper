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

let succ_con_1 = ":tmi.twitch.tv 001 twitch_username :Welcome, GLHF!"
let succ_con_2 = ":tmi.twitch.tv 002 twitch_username :Your host is tmi.twitch.tv"
let succ_con_3 = ":tmi.twitch.tv 003 twitch_username :This server is rather new"
let succ_con_4 = ":tmi.twitch.tv 004 twitch_username :-"
let succ_con_5 = ":tmi.twitch.tv 372 twitch_username :You are in a maze of twisty passages, all alike."
let succ_con_6 = ":tmi.twitch.tv 376 twitch_username :>"

[<TestFixture>]
type ``Recv: Upon a Successful Connection`` () = 
    
    static member TestData =
        [|
            [|succ_con_1, "tmi.twitch.tv" , "001", "twitch_username", "Welcome, GLHF!"|];
            [|succ_con_2, "tmi.twitch.tv" , "002", "twitch_username", "Your host is tmi.twitch.tv"|];
            [|succ_con_3, "tmi.twitch.tv" , "003", "twitch_username", "This server is rather new"|];
            [|succ_con_4, "tmi.twitch.tv" , "004", "twitch_username", "-"|];
            [|succ_con_5, "tmi.twitch.tv" , "372", "twitch_username", "You are in a maze of twisty passages, all alike."|];
            [|succ_con_6, "tmi.twitch.tv" , "376", "twitch_username", ">"|];
        |]

    [<TestCaseSource("TestData")>]
    member x.``twitch says hello`` (testData:(string*string*string*string*string)) =
        let msg, serv, code, nickname, text = testData
        
        let retv_msg = parseMessage msg

        match retv_msg with
        | Info m -> 
            m.TwitchAddr |> should equal serv
            m.Code |> should equal code
            m.Nickname |> should equal nickname
            m.Message |> should equal text
        | _ -> 
            true |> should equal false

//
// Commands you can send
// WHO #channel
// 

// TODO: test
let command_invalid = ":tmi.twitch.tv 421 twitch_username WHO :Unknown command"

//
// JOIN: Opening up a chat room
// JOIN #channel
//
 
let command_join_1 = ":twitch_username!twitch_username@twitch_username.tmi.twitch.tv JOIN #channel"
let command_join_2 = ":twitch_username.tmi.twitch.tv 353 twitch_username = #channel :twitch_username"
let command_join_3 = ":twitch_username!twitch_username@twitch_username.tmi.twitch.tv JOIN #channel"

//
// PART: Leaving a chat room
// PART #channel
// 

let cmd_part = ":twitch_username!twitch_username@twitch_username.tmi.twitch.tv PART #channel"

//
// PRIVMSG: Sending a message
// PRIVMSG #channel :Message to send
// 

let cmd_pvtmsg = ":twitch_username!twitch_username@twitch_username.tmi.twitch.tv PRIVMSG #channel :message here"

//
// Twitch Capabilities.
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




// ORDER tests, use examles from manual

// Test EN locale.
let msg_en = ":nickname!nickname@nickname.tmi.twitch.tv PRIVMSG #gamer2 :So is this the stream with giveaways?"

let f_Nicname = "nickname"
let f_NameAddr = "nickname@nickname.tmi.twitch.tv"
let f_Cmd = "PRIVMSG"
let f_Channel = "#gamer2"
let f_MessageStarts = "So"

// Test russian locale.
let msg_ru = ":nightbot!nightbot@nightbot.tmi.twitch.tv PRIVMSG #lady__angel :хочешь порадовать девушку или выразить благодарность"

let nb_Nicname = "nightbot"
let nb_NameAddr = "nightbot@nightbot.tmi.twitch.tv"
let nb_Cmd = "PRIVMSG"
let nb_Channel = "#lady__angel"
let nb_MessageStarts = "хочешь"

// Hello messages
let msg_hello = ":tmi.twitch.tv 001 nickname :Welcome, GLHF!"

let msgHello_TwitchAddr = "tmi.twitch.tv"
let msgHello_Code = "001"
let msgHello_Nickname = "nickname"
let msgHello_Message = "Welcome, GLHF!"

// Ping
let msg_ping = "PING :tmi.twitch.tv"

let msgPing_TwitchAddr = "tmi.twitch.tv"

[<Test>]
let``test message parse EN``()=
    let retv_msg = parseMessage msg_en
    
    match retv_msg with
    | Msg m -> 
        m.Nickname |> should equal f_Nicname
        m.NameAddr |> should equal f_NameAddr
        m.Cmd |> should equal f_Cmd
        m.Channel |> should equal f_Channel
        m.Message |> should startWith f_MessageStarts
    | _ -> 
        true |> should equal false

[<Test>]
let``test message parse RU``()=
    let retv_msg = parseMessage msg_ru
    
    match retv_msg with
    | Msg m -> 
        m.Nickname |> should equal nb_Nicname
        m.NameAddr |> should equal nb_NameAddr
        m.Cmd |> should equal nb_Cmd
        m.Channel |> should equal nb_Channel
        m.Message |> should startWith nb_MessageStarts
    | _ -> 
        true |> should equal false


[<Test>]
let``test message parse Hello type``()=
    let retv_msg = parseMessage msg_hello
    
    match retv_msg with
    | Info m -> 
        m.TwitchAddr |> should equal msgHello_TwitchAddr
        m.Code |> should equal msgHello_Code
        m.Nickname |> should equal msgHello_Nickname
        m.Message |> should startWith msgHello_Message
    | _ -> 
        true |> should equal false


[<Test>]
let``test message parse Ping``()=
    let retv_msg = parseMessage msg_ping
    
    match retv_msg with
    | Ping m -> 
        m.TwitchAddr |> should equal msgPing_TwitchAddr
    | _ -> 
        true |> should equal false

// Hello messages
let msg_Nick = ":twitch_username.tmi.twitch.tv 353 twitch_username = #channel :twitch_username"

let msgNick_TwitchAddr = "twitch_username.tmi.twitch.tv"
let msgNick_Code = "353"
let msgNick_Nickname = "twitch_username"
let msgNick_Channel = "#channel"
let msgNick_Nickname2 = "twitch_username"

[<Test>]
let``test message parse JOIN nicknames``()=
    let retv_msg = parseMessage msg_Nick
    
    match retv_msg with
    | Nicknames m -> 
        m.TwitchAddr |> should equal msgNick_TwitchAddr
        m.Code |> should equal msgNick_Code
        m.Nickname1 |> should equal msgNick_Nickname
        m.Channel |> should equal msgNick_Channel
        m.Nickname2 |> should equal msgNick_Nickname2
    | _ -> 
        true |> should equal false


let msg_join = ":twitch_username!twitch_username@twitch_username.tmi.twitch.tv JOIN #channel"

let msgJoin_Nickname = "twitch_username"
let msgJoin_TwitchAddr = "twitch_username@twitch_username.tmi.twitch.tv"
let msgJoin_Cmd = "JOIN"
let msgJoin_Channel = "#channel"

[<Test>]
let``test message parse JOIN message``()=
    let retv_msg = parseMessage msg_join
    
    match retv_msg with
    | ChanellJoin m -> 
        m.Nickname |> should equal msgJoin_Nickname
        m.NameAddr |> should equal msgJoin_TwitchAddr
        m.Cmd |> should equal msgJoin_Cmd
        m.Channel |> should equal msgJoin_Channel
    | _ -> 
        true |> should equal false

let msg_endNames = ":twitch_username.tmi.twitch.tv 366 twitch_username #channel :End of /NAMES list"

let msgendNames_nameaddr = "twitch_username.tmi.twitch.tv"
let msgendNames_code = "366"
let msgendNames_nickname = "twitch_username"
let msgendNames_Channel = "#channel"
let msgendNames_message = "End of /NAMES list"

[<Test>]
let``test message parse End of Names``()=
    let retv_msg = parseMessage msg_endNames
    
    match retv_msg with
    | NicknamesEnd m -> 
        m.NameAddr |> should equal msgendNames_nameaddr
        m.Code |> should equal msgendNames_code        
        m.Nickname |> should equal msgendNames_nickname
        m.Channel |> should equal msgendNames_Channel
        m.Message |> should equal msgendNames_message
    | _ -> 
        true |> should equal false