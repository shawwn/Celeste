# Celeste

This codebase can be used to build a custom version of Celeste on macOS and Windows. The source files were generated using [JetBrains Rider](https://www.jetbrains.com/rider/).

# Demo

After changing some constants in Player.cs, I recorded this video:

[https://www.youtube.com/watch?v=XNC5gJCAyKQ](https://www.youtube.com/watch?v=XNC5gJCAyKQ)

# Installation

## Windows

- copy `C:\Program Files (x86)\Steam\steamapps\common\Celeste\Content` to `Celeste\Game\`

- run `Celeste\Game\Celeste.exe`

## MacOS

- Install mono:

```
brew install mono
```

- copy `~/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/MacOS/Content` to `./Celeste/Game/`

- run `./Celeste/Game/Celeste`


# Modding Tutorial

Let's do some basic modding. We're going to make Madeline climb walls 3x faster.

- Clone the repository: `git clone https://github.com/shawwn/Celeste`

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

- Open `Celeste/Celeste.sln` in [JetBrains Rider](https://www.jetbrains.com/rider/)

- Click `Build -> Build Solution`

(You might want to make sure the configuration is set to `Release | Any CPU`, but the defaults will probably work fine.)

- if you're on Windows: run `Celeste\Game\Celeste.exe`

- if you're on macOS: run `./Celeste/Game/Celeste`

- Climb some walls!

# Next Steps 

- Experiment with changing the other constants in Player.cs. You can change things like `Gravity`, `MaxRun`, and `DashTime`.

- Comment out some code in the player's update loop and see what breaks. This is one of the best ways to learn how a codebase works.

