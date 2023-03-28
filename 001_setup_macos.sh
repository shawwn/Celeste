set -ex
home="$(pwd)"

if [ ! -d Celeste.app ]
then
  if [ ! -d ~/Library/Application\ Support/Steam/steamapps/common/Celeste/Celeste.app ]
  then
    echo "Can't find Celeste.app; install it, then copy it to $(pwd)/Celeste.app"
    exit 1
  fi
  ln -s ~/Library/Application\ Support/Steam/steamapps/common/Celeste/Celeste.app Celeste.app
fi

cd Celeste/Game

if [ ! -d Content ]
then
  ln -s ../../Celeste.app/Contents/Resources/Content Content
fi

