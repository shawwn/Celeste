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
    public int WindowScale = 6;
    public bool VSync = true;
    public RumbleAmount Rumble = RumbleAmount.On;
    public int MusicVolume = 10;
    public int SFXVolume = 10;
    public string Language = "english";
    public static Settings Instance;
    public static bool Existed;
    public static string LastVersion;
    public const string EnglishLanguage = "english";
    public string Version;
    public bool Fullscreen;
    public int ViewportPadding;
    public bool DisableFlashes;
    public bool DisableScreenShake;
    public SpeedrunType SpeedrunClock;
    public int LastSaveFile;
    public bool Pico8OnMainMenu;
    public bool SetViewportOnce;
    public bool VariantsUnlocked;
    public Keys Left;
    public Keys Right;
    public Keys Down;
    public Keys Up;
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
    public bool LaunchWithFMODLiveUpdate;
    public bool LaunchInDebugMode;
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
        this.Left = (Keys) 0;
      if (reset)
        this.Right = (Keys) 0;
      if (reset)
        this.Up = (Keys) 0;
      if (reset)
        this.Down = (Keys) 0;
      if (reset || this.Grab == null || this.Grab.Count <= 0)
      {
        List<Keys> keysList = new List<Keys>();
        keysList.Add((Keys) 90);
        keysList.Add((Keys) 86);
        keysList.Add((Keys) 160);
        this.Grab = keysList;
      }
      if (reset || this.Jump == null || this.Jump.Count <= 0)
      {
        List<Keys> keysList = new List<Keys>();
        keysList.Add((Keys) 67);
        this.Jump = keysList;
      }
      if (reset || this.Dash == null || this.Dash.Count <= 0)
      {
        List<Keys> keysList = new List<Keys>();
        keysList.Add((Keys) 88);
        this.Dash = keysList;
      }
      if (reset || this.Talk == null || this.Talk.Count <= 0)
      {
        List<Keys> keysList = new List<Keys>();
        keysList.Add((Keys) 88);
        this.Talk = keysList;
      }
      if (reset || this.Pause == null)
        this.Pause = new List<Keys>();
      if (reset || this.Confirm == null || this.Confirm.Count <= 0)
      {
        List<Keys> keysList = new List<Keys>();
        keysList.Add((Keys) 67);
        this.Confirm = keysList;
      }
      if (reset || this.Cancel == null || this.Cancel.Count <= 0)
      {
        List<Keys> keysList = new List<Keys>();
        keysList.Add((Keys) 88);
        this.Cancel = keysList;
      }
      if (reset || this.Journal == null || this.Journal.Count <= 0)
      {
        List<Keys> keysList = new List<Keys>();
        keysList.Add((Keys) 9);
        this.Journal = keysList;
      }
      if (!reset && this.QuickRestart != null && this.QuickRestart.Count > 0)
        return;
      List<Keys> keysList1 = new List<Keys>();
      keysList1.Add((Keys) 82);
      this.QuickRestart = keysList1;
    }

    public void SetDefaultButtonControls(bool reset)
    {
      if (reset || this.BtnJump == null || this.BtnJump.Count <= 0)
      {
        List<Buttons> buttonsList = new List<Buttons>();
        buttonsList.Add((Buttons) 4096);
        buttonsList.Add((Buttons) 32768);
        this.BtnJump = buttonsList;
      }
      if (reset || this.BtnDash == null || this.BtnDash.Count <= 0)
      {
        List<Buttons> buttonsList = new List<Buttons>();
        buttonsList.Add((Buttons) 16384);
        buttonsList.Add((Buttons) 8192);
        this.BtnDash = buttonsList;
      }
      if (reset || this.BtnGrab == null || this.BtnGrab.Count <= 0)
      {
        List<Buttons> buttonsList = new List<Buttons>();
        buttonsList.Add((Buttons) 8388608);
        buttonsList.Add((Buttons) 4194304);
        buttonsList.Add((Buttons) 256);
        buttonsList.Add((Buttons) 512);
        this.BtnGrab = buttonsList;
      }
      if (reset || this.BtnTalk == null || this.BtnTalk.Count <= 0)
      {
        List<Buttons> buttonsList = new List<Buttons>();
        buttonsList.Add((Buttons) 8192);
        this.BtnTalk = buttonsList;
      }
      if (!reset && this.BtnAltQuickRestart != null)
        return;
      this.BtnAltQuickRestart = new List<Buttons>();
    }

    public int MaxScale
    {
      get
      {
        return Math.Min(Engine.Instance.get_GraphicsDevice().get_Adapter().get_CurrentDisplayMode().get_Width() / 320, Engine.Instance.get_GraphicsDevice().get_Adapter().get_CurrentDisplayMode().get_Height() / 180);
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
      (Engine.Scene as Overworld).GetUI<OuiMainMenu>()?.CreateButtons();
    }
  }
}
