// Decompiled with JetBrains decompiler
// Type: Celeste
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Celeste.Pico8;
using Microsoft.Xna.Framework;
using Monocle;
using SDL2;
// using Steamworks;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Celeste
{
  public class Celeste : Engine
  {
    public const int GameWidth = 320;
    public const int GameHeight = 180;
    public const int TargetWidth = 1920;
    public const int TargetHeight = 1080;
    public static Celeste.PlayModes PlayMode = Celeste.PlayModes.Normal;
    public const string EventName = "";
    public const bool Beta = false;
    public const string PLATFORM = "PC";
    public static bool IsGGP = SDL.SDL_GetPlatform().Equals("Stadia");
    public static Celeste Instance;
    public static VirtualRenderTarget HudTarget;
    public static VirtualRenderTarget WipeTarget;
    public static DisconnectedControllerUI DisconnectUI;
    private bool firstLoad = true;
    public AutoSplitterInfo AutoSplitterInfo = new AutoSplitterInfo();
    public static Coroutine SaveRoutine;
    public static Stopwatch LoadTimer;
    // public static readonly AppId_t SteamID = new AppId_t(504230U);
    private static int _mainThreadId;

    public static Vector2 TargetCenter => new Vector2(1920f, 1080f) / 2f;

    public Celeste()
      : base(1920, 1080, 960, 540, nameof (Celeste), Settings.Instance.Fullscreen, Settings.Instance.VSync)
    {
      this.Version = new System.Version(1, 4, 0, 0);
      Celeste.Instance = this;
      Engine.ExitOnEscapeKeypress = false;
      this.IsFixedTimeStep = true;
      Stats.MakeRequest();
      // StatsForStadia.MakeRequest();
      Console.WriteLine("CELESTE : " + (object) this.Version);
    }

    protected override void Initialize()
    {
      base.Initialize();
      Settings.Instance.AfterLoad();
      if (Settings.Instance.Fullscreen)
        Engine.ViewPadding = Settings.Instance.ViewportPadding;
      Settings.Instance.ApplyScreen();
      SFX.Initialize();
      Tags.Initialize();
      Input.Initialize();
      Engine.Commands.Enabled = Celeste.PlayMode == Celeste.PlayModes.Debug;
      Engine.Scene = (Scene) new GameLoader();
    }

    protected override void LoadContent()
    {
      base.LoadContent();
      Console.WriteLine("BEGIN LOAD");
      Celeste.LoadTimer = Stopwatch.StartNew();
      PlaybackData.Load();
      if (this.firstLoad)
      {
        this.firstLoad = false;
        Celeste.HudTarget = VirtualContent.CreateRenderTarget("hud-target", 1922, 1082);
        Celeste.WipeTarget = VirtualContent.CreateRenderTarget("wipe-target", 1922, 1082);
        OVR.Load();
        if (!Celeste.IsGGP)
        {
          GFX.Load();
          MTN.Load();
        }
      }
      if (GFX.Game != null)
      {
        Monocle.Draw.Particle = GFX.Game["util/particle"];
        Monocle.Draw.Pixel = new MTexture(GFX.Game["util/pixel"], 1, 1, 1, 1);
      }
      GFX.LoadEffects();
    }

    protected override void Update(GameTime gameTime)
    {
      // SteamAPI.RunCallbacks();
      // StatsForStadia.BeginFrame(this.Window.Handle);
      if (Celeste.SaveRoutine != null)
        Celeste.SaveRoutine.Update();
      this.AutoSplitterInfo.Update();
      Audio.Update();
      base.Update(gameTime);
      Input.UpdateGrab();
    }

    protected override void OnSceneTransition(Scene last, Scene next)
    {
      if (!(last is OverworldLoader) || !(next is Overworld))
        base.OnSceneTransition(last, next);
      Engine.TimeRate = 1f;
      Audio.PauseGameplaySfx = false;
      Audio.SetMusicParam("fade", 1f);
      Distort.Anxiety = 0.0f;
      Distort.GameRate = 1f;
      Glitch.Value = 0.0f;
    }

    protected override void RenderCore()
    {
      base.RenderCore();
      if (Celeste.DisconnectUI == null)
        return;
      Celeste.DisconnectUI.Render();
    }

    public static void Freeze(float time)
    {
      if ((double) Engine.FreezeTimer >= (double) time)
        return;
      Engine.FreezeTimer = time;
      if (Engine.Scene == null)
        return;
      Engine.Scene.Tracker.GetEntity<CassetteBlockManager>()?.AdvanceMusic(time);
    }

    public static bool IsMainThread => Thread.CurrentThread.ManagedThreadId == Celeste._mainThreadId;

    private static void Main(string[] args)
    {
      Celeste celeste;
      try
      {
        Environment.SetEnvironmentVariable("FNA_AUDIO_DISABLE_SOUND", "1");
        for (int index = 0; index < args.Length; ++index)
        {
          if (args[index] == "--graphics" && index < args.Length - 1)
          {
            Environment.SetEnvironmentVariable("FNA3D_FORCE_DRIVER", args[index + 1]);
            ++index;
          }
          else if (args[index] == "--disable-lateswaptear")
            Environment.SetEnvironmentVariable("FNA3D_DISABLE_LATESWAPTEAR", "1");
        }
        Celeste._mainThreadId = Thread.CurrentThread.ManagedThreadId;
        Settings.Initialize();
        // if (SteamAPI.RestartAppIfNecessary(Celeste.SteamID))
        //   return;
        // if (!SteamAPI.Init())
        // {
        //   ErrorLog.Write("Steam not found!");
        //   ErrorLog.Open();
        //   return;
        // }
        // if (!Settings.Existed)
        //   Settings.Instance.Language = SteamApps.GetCurrentGameLanguage();
        int num = Settings.Existed ? 1 : 0;
        for (int index = 0; index < args.Length - 1; ++index)
        {
          if (args[index] == "--language" || args[index] == "-l")
            Settings.Instance.Language = args[++index];
          else if (args[index] == "--default-language" || args[index] == "-dl")
          {
            if (!Settings.Existed)
              Settings.Instance.Language = args[++index];
          }
          else if (args[index] == "--gui" || args[index] == "-g")
            Input.OverrideInputPrefix = args[++index];
        }
        celeste = new Celeste();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        ErrorLog.Write(ex);
        try
        {
          ErrorLog.Open();
          return;
        }
        catch
        {
          Console.WriteLine("Failed to open the log!");
          return;
        }
      }
      // celeste.RunWithLogging();
      celeste.Run();
      RunThread.WaitAll();
      celeste.Dispose();
      Audio.Unload();
    }

    public static void ReloadAssets(bool levels, bool graphics, bool hires, AreaKey? area = null)
    {
      if (levels)
        Celeste.ReloadLevels(area);
      if (!graphics)
        return;
      Celeste.ReloadGraphics(hires);
    }

    public static void ReloadLevels(AreaKey? area = null)
    {
    }

    public static void ReloadGraphics(bool hires)
    {
    }

    public static void ReloadPortraits()
    {
    }

    public static void ReloadDialog()
    {
    }

    private static void CallProcess(string path, string args = "", bool createWindow = false)
    {
      Process process = new Process();
      process.StartInfo = new ProcessStartInfo()
      {
        FileName = path,
        WorkingDirectory = Path.GetDirectoryName(path),
        RedirectStandardOutput = false,
        CreateNoWindow = !createWindow,
        UseShellExecute = false,
        Arguments = args
      };
      process.Start();
      process.WaitForExit();
    }

    public static bool PauseAnywhere()
    {
      switch (Engine.Scene)
      {
        case Level _:
          Level scene1 = Engine.Scene as Level;
          if (scene1.CanPause)
          {
            scene1.Pause();
            return true;
          }
          break;
        case Emulator _:
          Emulator scene2 = Engine.Scene as Emulator;
          if (scene2.CanPause)
          {
            scene2.CreatePauseMenu();
            return true;
          }
          break;
        case IntroVignette _:
          IntroVignette scene3 = Engine.Scene as IntroVignette;
          if (scene3.CanPause)
          {
            scene3.OpenMenu();
            return true;
          }
          break;
        case CoreVignette _:
          CoreVignette scene4 = Engine.Scene as CoreVignette;
          if (scene4.CanPause)
          {
            scene4.OpenMenu();
            return true;
          }
          break;
      }
      return false;
    }

    public enum PlayModes
    {
      Normal,
      Debug,
      Event,
      Demo,
    }
  }
}
