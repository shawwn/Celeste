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

# Modding Tutorial

Let's do some basic modding. We're going to make Madeline climb walls 3x faster.

- Clone the repository.

- Install the `Content` folder. (See the installation steps above.)

- Open [./Celeste/Celeste/Player.cs](https://github.com/shawwn/Celeste/blob/master/Celeste/Celeste/Player.cs) in an editor.

- Change the following code:

```
        private const float ClimbUpSpeed = -45f;
        private const float ClimbDownSpeed = 80f;
        private const float ClimbSlipSpeed = 30f;
        private const float ClimbAccel = 900f;
```

... to this:
```
        private const float ClimbUpSpeed = -3*45f;
        private const float ClimbDownSpeed = 3*80f;
        private const float ClimbSlipSpeed = 30f;
        private const float ClimbAccel = 3*900f;
```

- Open `Celeste/Celeste.sln` in [JetBrains Rider](https://www.jetbrains.com/rider/). Click `Build -> Build Solution`.

(You might want to make sure the configuration is set to `Release | Any CPU`, but the defaults will probably work fine.)

- Open a terminal or command prompt. Navigate to wherever you cloned the repository.

- Run `./Celeste/Game/Celeste` (or if you're on Windows, run `./Celeste/Game/Celeste.exe`)

- Climb some walls!

Now experiment with the other constants. You can change things like `Gravity`, `MaxRun`, and `DashTime`.

Comment out some code in the player's update loop and see what breaks. (This is one of the best ways to learn how a codebase works.)

