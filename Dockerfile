FROM mono:4.2.3.4-onbuild
MAINTAINER Dmitri Peredera <dmitri.peredera@gmail.com>	
CMD [ "mono",  "/usr/src/app/source/TwitchChatHelper/bin/Release/TwitchChatHelper.exe" ]