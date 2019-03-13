# Celeste

This codebase builds and runs on macOS. It was generated using [JetBrains Rider](https://www.jetbrains.com/rider/).

Demo of some basic modding by tweaking the constants in Player.cs:

[https://www.youtube.com/watch?v=XNC5gJCAyKQ](https://www.youtube.com/watch?v=XNC5gJCAyKQ)

# How to run

- Install Celeste via Steam

- Copy the Content directory to `./Celeste/Game/`. For example, on MacOS you'd copy `~/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/MacOS/Content` to `./Celeste/Game/`.

The `./Celeste/Game/` folder should contain the `Content` folder. Try running `ls ./Celeste/Game/Content/Maps/0-Intro.bin` to verify that the game data is in the right place.

## MacOS
```
brew install mono sdl2 sdl2_gfx sdl2_image sdl2_image sdl2_ttf
```

Then run `./Celeste/Game/Celeste` and Celeste should start!

# Misc

Other platforms probably work fine, but if someone could test them out, that would be much appreciated.

Note that to get Windows or Linux running, you'll probably have to copy the correct FMOD DLLs to `./Celeste/Game/`. You can snag those from Celeste's steam folder.

