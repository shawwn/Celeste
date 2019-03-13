// Decompiled with JetBrains decompiler
// Type: Celeste.Settings
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Celeste
{
  [Serializable]
  public class Settings
  {
    public bool Fullscreen = false;
    public int WindowScale = 6;
    public int ViewportPadding = 0;
    public bool VSync = true;
    public bool DisableFlashes = false;
    public bool DisableScreenShake = false;
    public RumbleAmount Rumble = RumbleAmount.On;
    public int MusicVolume = 10;
    public int SFXVolume = 10;
    public int LastSaveFile = 0;
    public string Language = "english";
    public bool Pico8OnMainMenu = false;
    public bool SetViewportOnce = false;
    public bool VariantsUnlocked = false;
    public Keys Left = Keys.None;
    public Keys Right = Keys.None;
    public Keys Down = Keys.None;
    public Keys Up = Keys.None;
    public bool LaunchWithFMODLiveUpdate = false;
    public bool LaunchInDebugMode = false;
    public static Settings Instance;
    public static bool Existed;
    public static string LastVersion;
    public const string EnglishLanguage = "english";
    public string Version;
    public SpeedrunType SpeedrunClock;
    public List<Keys> Grab;
    public List<Keys> Jump;
    public List<Keys> Dash;
    public List<Keys> Talk;
    public List<Keys> Pause;
    public List<Keys> Confirm;
    public List<Keys> Cancel;
    public List<Keys> Journal;
    public List<Keys> QuickRestart;
    public List<Buttons> BtnGrab;
    public List<Buttons> BtnJump;
    public List<Buttons> BtnDash;
    public List<Buttons> BtnTalk;
    public List<Buttons> BtnAltQuickRestart;
    public const string Filename = "settings";

    [XmlAnyElement("LaunchInDebugModeComment")]
    public XmlComment DebugModeComment
    {
      get
      {
        return new XmlDocument().CreateComment("\n\t\tLaunchWithFMODLiveUpdate:\n\t\t\tThis Enables FMOD Studio Live Update so you can interact with the sounds in real time.\n\t\t\tNote this will also require access to the private network.\n\t\t\n\t\tLaunchInDebugMode:\n\t\t\tDebug Mode can destroy save files, crash the game, and do other unwanted behaviour.\n\t\t\tIt is not documented. Use at own risk.\n\t");
      }
      set
      {
      }
    }

    public Settings()
    {
      if (Celeste.Celeste.PlayMode == Celeste.Celeste.PlayModes.Debug)
        return;
      this.Fullscreen = true;
    }

    public void AfterLoad()
    {
      this.MusicVolume = Calc.Clamp(this.MusicVolume, 0, 10);
      this.SFXVolume = Calc.Clamp(this.SFXVolume, 0, 10);
      this.WindowScale = Math.Min(this.WindowScale, this.MaxScale);
      this.WindowScale = Calc.Clamp(this.WindowScale, 3, 8);
      this.SetDefaultKeyboardControls(false);
      this.SetDefaultButtonControls(false);
      if (this.LaunchInDebugMode)
        Celeste.Celeste.PlayMode = Celeste.Celeste.PlayModes.Debug;
      Settings.LastVersion = Settings.Existed ? Settings.Instance.Version : Celeste.Celeste.Instance.Version.ToString();
      Settings.Instance.Version = Celeste.Celeste.Instance.Version.ToString();
    }

    public void SetDefaultKeyboardControls(bool reset)
    {
      if (reset)
        this.Left = Keys.None;
      if (reset)
        this.Right = Keys.None;
      if (reset)
        this.Up = Keys.None;
      if (reset)
        this.Down = Keys.None;
      if (reset || this.Grab == null || this.Grab.Count <= 0)
        this.Grab = new List<Keys>()
        {
          Keys.Z,
          Keys.V,
          Keys.LeftShift
        };
      if (reset || this.Jump == null || this.Jump.Count <= 0)
        this.Jump = new List<Keys>() { Keys.C };
      if (reset || this.Dash == null || this.Dash.Count <= 0)
        this.Dash = new List<Keys>() { Keys.X };
      if (reset || this.Talk == null || this.Talk.Count <= 0)
        this.Talk = new List<Keys>() { Keys.X };
      if (reset || this.Pause == null)
        this.Pause = new List<Keys>();
      if (reset || this.Confirm == null || this.Confirm.Count <= 0)
        this.Confirm = new List<Keys>() { Keys.C };
      if (reset || this.Cancel == null || this.Cancel.Count <= 0)
        this.Cancel = new List<Keys>() { Keys.X };
      if (reset || this.Journal == null || this.Journal.Count <= 0)
        this.Journal = new List<Keys>() { Keys.Tab };
      if (!reset && this.QuickRestart != null && this.QuickRestart.Count > 0)
        return;
      this.QuickRestart = new List<Keys>() { Keys.R };
    }

    public void SetDefaultButtonControls(bool reset)
    {
      if (reset || this.BtnJump == null || this.BtnJump.Count <= 0)
        this.BtnJump = new List<Buttons>()
        {
          Buttons.A,
          Buttons.Y
        };
      if (reset || this.BtnDash == null || this.BtnDash.Count <= 0)
        this.BtnDash = new List<Buttons>()
        {
          Buttons.X,
          Buttons.B
        };
      if (reset || this.BtnGrab == null || this.BtnGrab.Count <= 0)
        this.BtnGrab = new List<Buttons>()
        {
          Buttons.LeftTrigger,
          Buttons.RightTrigger,
          Buttons.LeftShoulder,
          Buttons.RightShoulder
        };
      if (reset || this.BtnTalk == null || this.BtnTalk.Count <= 0)
        this.BtnTalk = new List<Buttons>() { Buttons.B };
      if (!reset && this.BtnAltQuickRestart != null)
        return;
      this.BtnAltQuickRestart = new List<Buttons>();
    }

    public int MaxScale
    {
      get
      {
        return Math.Min(Engine.Instance.GraphicsDevice.Adapter.CurrentDisplayMode.Width / 320, Engine.Instance.GraphicsDevice.Adapter.CurrentDisplayMode.Height / 180);
      }
    }

    public void ApplyVolumes()
    {
      this.ApplySFXVolume();
      this.ApplyMusicVolume();
    }

    public void ApplySFXVolume()
    {
      Audio.SfxVolume = (float) this.SFXVolume / 10f;
    }

    public void ApplyMusicVolume()
    {
      Audio.MusicVolume = (float) this.MusicVolume / 10f;
    }

    public void ApplyScreen()
    {
      if (this.Fullscreen)
      {
        Engine.ViewPadding = this.ViewportPadding;
        Engine.SetFullscreen();
      }
      else
      {
        Engine.ViewPadding = 0;
        Engine.SetWindowed(320 * this.WindowScale, 180 * this.WindowScale);
      }
    }

    public void ApplyLanguage()
    {
      if (!Dialog.Languages.ContainsKey(this.Language))
        this.Language = "english";
      Dialog.Language = Dialog.Languages[this.Language];
    }

    public static void Initialize()
    {
      if (UserIO.Open(UserIO.Mode.Read))
      {
        Settings.Instance = UserIO.Load<Settings>("settings", false);
        UserIO.Close();
      }
      Settings.Existed = Settings.Instance != null;
      if (Settings.Instance != null)
        return;
      Settings.Instance = new Settings();
    }

    public static void Reload()
    {
      Settings.Initialize();
      Settings.Instance.AfterLoad();
      Settings.Instance.ApplyVolumes();
      Settings.Instance.ApplyScreen();
      Settings.Instance.ApplyLanguage();
      if (!(Engine.Scene is Overworld))
        return;
      OuiMainMenu ui = (Engine.Scene as Overworld).GetUI<OuiMainMenu>();
      if (ui != null)
        ui.CreateButtons();
    }
  }
}

