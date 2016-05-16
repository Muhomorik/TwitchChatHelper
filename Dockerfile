FROM mono:3.10-onbuild
MAINTAINER Dmitri Peredera <dmitri.peredera@gmail.com>

CMD [ "mono",  "/usr/src/app/source/TwitchChatHelper/bin/Release/TwitchChatHelper.exe" ]