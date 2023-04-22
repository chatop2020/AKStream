#!/bin/bash

#作者:自动畅
#微信号:auto-chang
#公众号：畅聊了个科技(IT-chang)
#Time：Saturday, April 22, 2023
#Describe：Power Generation for Love

usage() {
  cat << EOF >&2
Usage: $PROGNAME
 -keeper --install-keeper
 -web --install-Web
 -dk --deploy-keeper
 -dw --deploy-Web
 -h --help
EOF
exit 1
}


if [ "$1" == "" ]; then
    usage
fi

while true; do
  case "$1" in
    -keeper | --install-keeper ) ACTION=install-keeper; shift ;;
    -web | --install-Web ) ACTION=install-Web; shift ;;
    -dk | --deploy-keeper ) ACTION=deploy-keeper; shift ;;
    -dW | --deploy-Web ) ACTION=deploy-Web; shift ;;
    -h | --help ) usage; exit 1 ;;
    -- ) shift; break ;;
    * ) break ;;
  esac
done


APP_KEEPER_NAME=ak-keeper
APP_WEB_NAME=ak-web

echo "Welcome to the AKStream(c# NetCore Programming Language),You have chosen an $ACTION "
echo "You have chosen an $ACTION "

if [ "$ACTION" == install-keeper ]; then
    if [! -f ./ubuntu-zlm-ffmpeg-dotnet.tar ] 
    then 
         wget  https://github.com/itchangc/toolbox/blob/main/ubuntu-zlm-ffmpeg-dotnet.tar
         docker import ubuntu-zlm-ffmpeg-dotnet.tar
    fi
    docker build -f Dockerfile-Keeper -t $APP_KEEPER_NAME .
fi

if [ "$ACTION" == install-Web ]; then
    docker build -f Dockerfile-Web -t $APP_WEB_NAME .
fi

if [ "$ACTION" == deploy-keeper ]; then
  # shellcheck disable=SC2046
  docker stop $(docker ps | grep $APP_KEEPER_NAME | awk '{print $1}')
  # shellcheck disable=SC2046
  docker rm $(docker ps -a | grep $APP_KEEPER_NAME | awk '{print $1}')
  # shellcheck disable=SC2046
  docker rmi $(docker images | grep $APP_KEEPER_NAME | awk '{print $3}')

  docker run -p 6880:6880 \
    -p 10001-1010:10001-1010 \
    -p 10001-1010:10001-1010/udp \
    -p 20002-20200:20002-20200 \
    -p 20002-20200:20002-20200/udp \
    -p 6881:80 \
    -p 6882:1935 \
    -p 6883:554 \
    -p 6883:554/udp \
    -p 10000:10000 \
    -p 10000:10000/udp \
    -p 8000:8000/udp \
    -p 30000-30035:30000-30035/udp \
    -v ./AKStreamKeeper/Config/AKStreamKeeper.json:/root/AKStreamKeeper/Config/AKStreamKeeper.json \
    -v ./AKStreamKeeper/Config/logconfig.xml:/root/AKStreamKeeper/Config/logconfig.xml \
    --name=$APP_KEEPER_NAME \
    --restart=always \
    -d $APP_KEEPER_NAME
fi
if [ "$ACTION" == deploy-Web ]; then
  # shellcheck disable=SC2046
  docker stop $(docker ps | grep $APP_WEB_NAME | awk '{print $1}')
  # shellcheck disable=SC2046
  docker rm $(docker ps -a | grep $APP_WEB_NAME | awk '{print $1}')
  # shellcheck disable=SC2046
  docker rmi $(docker images | grep $APP_WEB_NAME | awk '{print $3}')

  docker run -p 5800:5800 \
    -p 6880:6880 \
    -p 5060:5060 \
    -p 5060:5060/udp \
    -v ./AKStreamWeb/Config/AKStreamWeb.json:/app/Config/AKStreamWeb.json \
    -v ./AKStreamWeb/Config/SipClientConfig.json:/app/Config/SipClientConfig.json  \
    -v ./AKStreamWeb/Config/SipServerConfig.json:/app/Config/SipServerConfig.json \
    -v ./AKStreamWeb/Config/logconfig.xml:/app/Config/logconfig.xml \
    --name=$APP_WEB_NAME \
    --restart=always \
    -d $APP_WEB_NAME
fi

echo "Successful script execution, best wishes for you"


