# My little helper #

My little helper in F# for twitch chat api (irc)

Mostly going by [justintv/Twitch-API](https://github.com/justintv/Twitch-API/blob/master/IRC.md) docs.

Sometimes... you need to ban a few hundreds people. And this is a try to figure out a way to do it efficiently :smile:

This one is used to collect messages (no nicknames &etc). Those are then used in R to figure things out.

Note to myself:

- Login done using the OAuth token, get it at http://twitchapps.com/.
- Don't push it to GitHub :D

**DateTime.Now** is used internally everywhere.

Tested it on CS channel with 50k+ viewers and it works nice.

# Workign on #

- More agents! (MailboxProcessor so far, move to Akka.Net when structure is known).
- Move things to own projects
- Message of Tags

# Flow #

Everything starts in main thread where command line arguments are read and parsed.

Messages are received in loop and parsed. Parsed messages are printed and logged in main. Also, are send to the Reader Mailbox (fire-and-forget).

![chetHelperFlowYed](chetHelperFlowYed.png)

## Mailbox agents ##

There are two main F# MailboxProcessor agents here. Lightweight mailboxes, runs in own threads asynchronously.

- **ReaderMailbox**. Write handlers here. Handles received messages and Posts replies back via WriterMailbox.
- **WriterMailbox**. Send replies back to twitch. Output rate is limited according to the limits.

## Features ##

- Handles disconnects and no connecion errors (doesn't crash anymore ;))

# Screenshots #

Login

![screen_login](myimg/screen_login.png)

Chat

![screen_chat](myimg/screen_chat.png)

#  Config #

* Is stored in MyCfg.fs so far. Username and oauth.

## Command line args: ##

* --channel #channel
* --filelog twitch_log.txt

If channel is missing console is going to ask for input.

If the file is missing, the default one is going to be used.

# Docker #
The idea is to be able to run in container. Docker build file is [here](Dockerfile).

Note: don't set target to .NET 4.5.2, assemblies are missing then. Use 4.5 or 4.6. 
More about @ [mono/compatibility/](http://www.mono-project.com/docs/about-mono/compatibility/)

Build using Docker CLI:

    cd \    
    cd '.\Users\Dmitri\Desktop\Documents\Visual Studio 2015\Projects\TwitchChatHelper\'    
    docker build -t mytwitchhelper .
    docker run mytwitchhelper

Currently builds with no parameters - program exits.

## Uses ##

- Argu. A declarative CLI argument/XML configuration parser for F#
    * [GitHub](https://github.com/fsprojects/Argu) [Tutorial](http://fsprojects.github.io/Argu/tutorial.html)

## TODO ##

- [ ] Config: nickname, oauth login to settings file (or smth)
- [ ] Save login details and channel for Reconnect.
- [ ] Floating window message/pm counter, avoid bans.


