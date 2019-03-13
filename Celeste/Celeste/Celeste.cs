// Decompiled with JetBrains decompiler
// Type: Celeste.Celeste
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Celeste.Pico8;
using Microsoft.Xna.Framework;
using Monocle;
using Steamworks;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Celeste
{
  public class Celeste : Engine
  {
    public static Celeste.Celeste.PlayModes PlayMode = Celeste.Celeste.PlayModes.Normal;
    public static readonly AppId_t SteamID = new AppId_t(504230U);
    private bool firstLoad = true;
    public AutoSplitterInfo AutoSplitterInfo = new AutoSplitterInfo();
    public const int GameWidth = 320;
    public const int GameHeight = 180;
    public const int TargetWidth = 1920;
    public const int TargetHeight = 1080;
    public const string EventName = "";
    public const string PLATFORM = "PC";
    public static Celeste.Celeste Instance;
    public static VirtualRenderTarget HudTarget;
    public static DisconnectedControllerUI DisconnectUI;
    public static Coroutine SaveRoutine;
    private static int _mainThreadId;

    public static Vector2 TargetCenter
    {
      get
      {
        return Vector2.op_Division(new Vector2(1920f, 1080f), 2f);
      }
    }

    public Celeste()
      : base(1920, 1080, 960, 540, nameof (Celeste), Settings.Instance.Fullscreen)
    {
      this.Version = new System.Version(1, 2, 6, 1);
      Celeste.Celeste.Instance = this;
      Engine.ExitOnEscapeKeypress = false;
      this.set_IsFixedTimeStep(true);
      Settings.Instance.AfterLoad();
      if (Settings.Instance.Fullscreen)
        Engine.ViewPadding = Settings.Instance.ViewportPadding;
      Settings.Instance.ApplyScreen();
      Engine.Graphics.set_SynchronizeWithVerticalRetrace(Settings.Instance.VSync);
      Engine.Graphics.ApplyChanges();
      Stats.MakeRequest();
      Console.WriteLine("CELESTE : " + (object) this.Version);
    }

    protected override void Initialize()
    {
      base.Initialize();
      Sfxs.Initialize();
      Tags.Initialize();
      Input.Initialize();
      Engine.Commands.Enabled = Celeste.Celeste.PlayMode == Celeste.Celeste.PlayModes.Debug;
      Engine.Scene = (Scene) new GameLoader();
    }

    protected override void LoadContent()
    {
      base.LoadContent();
      if (this.firstLoad)
      {
        GameLoader.Stopwatch.Start();
        Celeste.Celeste.HudTarget = VirtualContent.CreateRenderTarget("hud-target", 1922, 1082, false, true, 0);
        GFX.LoadGui();
        GFX.LoadOverworld();
        GFX.LoadGame();
        GFX.LoadOther();
        GFX.LoadMountain();
        GFX.LoadPortraits();
        this.firstLoad = false;
      }
      if (GFX.Game != null)
      {
        Draw.Particle = GFX.Game["util/particle"];
        Draw.Pixel = new MTexture(GFX.Game["util/pixel"], 1, 1, 1, 1);
      }
      GFX.LoadEffects();
    }

    protected override void Update(GameTime gameTime)
    {
      SteamAPI.RunCallbacks();
      if (Celeste.Celeste.SaveRoutine != null)
        Celeste.Celeste.SaveRoutine.Update();
      this.AutoSplitterInfo.Update();
      Audio.Update();
      base.Update(gameTime);
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
      if (Celeste.Celeste.DisconnectUI == null)
        return;
      Celeste.Celeste.DisconnectUI.Render();
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

    public static bool IsMainThread
    {
      get
      {
        return Thread.CurrentThread.ManagedThreadId == Celeste.Celeste._mainThreadId;
      }
    }

    private static void Main(string[] args)
    {
      Celeste.Celeste celeste;
      try
      {
        Celeste.Celeste._mainThreadId = Thread.CurrentThread.ManagedThreadId;
        Settings.Initialize();
        if (SteamAPI.RestartAppIfNecessary(Celeste.Celeste.SteamID))
          return;
        if (!SteamAPI.Init())
        {
          ErrorLog.Write("Steam not found!");
          ErrorLog.Open();
          return;
        }
        if (!Settings.Existed)
          Settings.Instance.Language = SteamApps.GetCurrentGameLanguage();
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
        celeste = new Celeste.Celeste();
      }
      catch (Exception ex)
      {
        ErrorLog.Write(ex);
        ErrorLog.Open();
        return;
      }
      celeste.RunWithLogging();
    }

    public static void ReloadAssets(bool levels, bool graphics, bool hires, AreaKey? area = null)
    {
      if (levels)
        Celeste.Celeste.ReloadLevels(area);
      if (!graphics)
        return;
      Celeste.Celeste.ReloadGraphics(hires);
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
      if (Engine.Scene is Level)
      {
        Level scene = Engine.Scene as Level;
        if (scene.CanPause)
        {
          scene.Pause(0, false, false);
          return true;
        }
      }
      else if (Engine.Scene is Emulator)
      {
        Emulator scene = Engine.Scene as Emulator;
        if (scene.CanPause)
        {
          scene.CreatePauseMenu();
          return true;
        }
      }
      else if (Engine.Scene is IntroVignette)
      {
        IntroVignette scene = Engine.Scene as IntroVignette;
        if (scene.CanPause)
        {
          scene.OpenMenu();
          return true;
        }
      }
      else if (Engine.Scene is CoreVignette)
      {
        CoreVignette scene = Engine.Scene as CoreVignette;
        if (scene.CanPause)
        {
          scene.OpenMenu();
          return true;
        }
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
