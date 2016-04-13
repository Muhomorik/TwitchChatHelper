# My little helper #

My little helper in F# for twitch chat api (irc)

Mostly going by [justintv/Twitch-API](https://github.com/justintv/Twitch-API/blob/master/IRC.md) docs.

Sometimes... you need to ba a few hundreds people. And this is a try to figure out a way to do it efficient :smile:

This one is use to collect messages (no nicknames &etc). Those are then used in R to figure things out.

Note to myself:

- Login done using the OAuth token, get it at http://twitchapps.com/.
- Don't push it to GitHub :D

Login

![](myimg/screen_login.png)

Chat

![](myimg/screen_chat.png)

##  Config ##

* Is storedMyCfg.fs so far. Username and oauth.

Command line args:

* --channel #channel
* --filelog twitch_log.txt

If channel is missing console is going to ask for input.

If fil is missing, the default is going to be used.

## Uses ##

- Argu. A declarative CLI argument/XML configuration parser for F#
    * [GitHub](https://github.com/fsprojects/Argu) [Tutorial](http://fsprojects.github.io/Argu/tutorial.html)

## TODO ##

- [x] Command line: channel, 
- [ ] Config: nickname, oauth login to settings file (or smth)
- [x] Output filename as cli parameter.