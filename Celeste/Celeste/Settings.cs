// Decompiled with JetBrains decompiler
// Type: Celeste.Settings
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

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
    public static Settings Instance;
    public static bool Existed;
    public static string LastVersion;
    public const string EnglishLanguage = "english";
    public string Version;
    public string DefaultFileName = "";
    public bool Fullscreen;
    public int WindowScale = 6;
    public int ViewportPadding;
    public bool VSync = true;
    public bool DisableFlashes;
    public ScreenshakeAmount ScreenShake = ScreenshakeAmount.Half;
    public RumbleAmount Rumble = RumbleAmount.On;
    public GrabModes GrabMode;
    public CrouchDashModes CrouchDashMode;
    public int MusicVolume = 10;
    public int SFXVolume = 10;
    public SpeedrunType SpeedrunClock;
    public int LastSaveFile;
    public string Language = "english";
    public bool Pico8OnMainMenu;
    public bool SetViewportOnce;
    public bool VariantsUnlocked;
    public Binding Left = new Binding();
    public Binding Right = new Binding();
    public Binding Down = new Binding();
    public Binding Up = new Binding();
    public Binding MenuLeft = new Binding();
    public Binding MenuRight = new Binding();
    public Binding MenuDown = new Binding();
    public Binding MenuUp = new Binding();
    public Binding Grab = new Binding();
    public Binding Jump = new Binding();
    public Binding Dash = new Binding();
    public Binding Talk = new Binding();
    public Binding Pause = new Binding();
    public Binding Confirm = new Binding();
    public Binding Cancel = new Binding();
    public Binding Journal = new Binding();
    public Binding QuickRestart = new Binding();
    public Binding DemoDash = new Binding();
    public Binding RightMoveOnly = new Binding();
    public Binding LeftMoveOnly = new Binding();
    public Binding UpMoveOnly = new Binding();
    public Binding DownMoveOnly = new Binding();
    public Binding RightDashOnly = new Binding();
    public Binding LeftDashOnly = new Binding();
    public Binding UpDashOnly = new Binding();
    public Binding DownDashOnly = new Binding();
    public bool LaunchWithFMODLiveUpdate;
    public bool LaunchInDebugMode;
    public const string Filename = "settings";

    [XmlAnyElement("LaunchInDebugModeComment")]
    public XmlComment DebugModeComment
    {
      get => new XmlDocument().CreateComment("\n\t\tLaunchWithFMODLiveUpdate:\n\t\t\tThis Enables FMOD Studio Live Update so you can interact with the sounds in real time.\n\t\t\tNote this will also require access to the private network.\n\t\t\n\t\tLaunchInDebugMode:\n\t\t\tDebug Mode can destroy save files, crash the game, and do other unwanted behaviour.\n\t\t\tIt is not documented. Use at own risk.\n\t");
      set
      {
      }
    }

    public Settings()
    {
      if (Celeste.PlayMode != Celeste.PlayModes.Debug)
        this.Fullscreen = true;
      if (!Celeste.IsGGP)
        return;
      this.Fullscreen = false;
      this.VSync = false;
    }

    public void AfterLoad()
    {
      Binding.SetExclusive(this.MenuLeft, this.MenuRight, this.MenuUp, this.MenuDown, this.Confirm, this.Cancel, this.Journal, this.Pause);
      this.MusicVolume = Calc.Clamp(this.MusicVolume, 0, 10);
      this.SFXVolume = Calc.Clamp(this.SFXVolume, 0, 10);
      this.WindowScale = Math.Min(this.WindowScale, this.MaxScale);
      this.WindowScale = Calc.Clamp(this.WindowScale, 3, 10);
      this.SetDefaultKeyboardControls(false);
      this.SetDefaultButtonControls(false);
      if (this.LaunchInDebugMode)
        Celeste.PlayMode = Celeste.PlayModes.Debug;
      Settings.LastVersion = Settings.Existed ? Settings.Instance.Version : Celeste.Instance.Version.ToString();
      Settings.Instance.Version = Celeste.Instance.Version.ToString();
    }

    public void SetDefaultKeyboardControls(bool reset)
    {
      if (reset || this.Left.Keyboard.Count <= 0)
      {
        this.Left.Keyboard.Clear();
        this.Left.Add(Keys.Left);
      }
      if (reset || this.Right.Keyboard.Count <= 0)
      {
        this.Right.Keyboard.Clear();
        this.Right.Add(Keys.Right);
      }
      if (reset || this.Down.Keyboard.Count <= 0)
      {
        this.Down.Keyboard.Clear();
        this.Down.Add(Keys.Down);
      }
      if (reset || this.Up.Keyboard.Count <= 0)
      {
        this.Up.Keyboard.Clear();
        this.Up.Add(Keys.Up);
      }
      if (reset || this.MenuLeft.Keyboard.Count <= 0)
      {
        this.MenuLeft.Keyboard.Clear();
        this.MenuLeft.Add(Keys.Left);
      }
      if (reset || this.MenuRight.Keyboard.Count <= 0)
      {
        this.MenuRight.Keyboard.Clear();
        this.MenuRight.Add(Keys.Right);
      }
      if (reset || this.MenuDown.Keyboard.Count <= 0)
      {
        this.MenuDown.Keyboard.Clear();
        this.MenuDown.Add(Keys.Down);
      }
      if (reset || this.MenuUp.Keyboard.Count <= 0)
      {
        this.MenuUp.Keyboard.Clear();
        this.MenuUp.Add(Keys.Up);
      }
      if (reset || this.Grab.Keyboard.Count <= 0)
      {
        this.Grab.Keyboard.Clear();
        this.Grab.Add(Keys.Z, Keys.V, Keys.LeftShift);
      }
      if (reset || this.Jump.Keyboard.Count <= 0)
      {
        this.Jump.Keyboard.Clear();
        this.Jump.Add(Keys.C);
      }
      if (reset || this.Dash.Keyboard.Count <= 0)
      {
        this.Dash.Keyboard.Clear();
        this.Dash.Add(Keys.X);
      }
      if (reset || this.Talk.Keyboard.Count <= 0)
      {
        this.Talk.Keyboard.Clear();
        this.Talk.Add(Keys.X);
      }
      if (reset || this.Pause.Keyboard.Count <= 0)
      {
        this.Pause.Keyboard.Clear();
        this.Pause.Add(Keys.Enter);
      }
      if (reset || this.Confirm.Keyboard.Count <= 0)
      {
        this.Confirm.Keyboard.Clear();
        this.Confirm.Add(Keys.C);
      }
      if (reset || this.Cancel.Keyboard.Count <= 0)
      {
        this.Cancel.Keyboard.Clear();
        this.Cancel.Add(Keys.X, Keys.Back);
      }
      if (reset || this.Journal.Keyboard.Count <= 0)
      {
        this.Journal.Keyboard.Clear();
        this.Journal.Add(Keys.Tab);
      }
      if (reset || this.QuickRestart.Keyboard.Count <= 0)
      {
        this.QuickRestart.Keyboard.Clear();
        this.QuickRestart.Add(Keys.R);
      }
      if (reset)
      {
        this.DemoDash.Keyboard.Clear();
        this.LeftMoveOnly.Keyboard.Clear();
        this.RightMoveOnly.Keyboard.Clear();
        this.UpMoveOnly.Keyboard.Clear();
        this.DownMoveOnly.Keyboard.Clear();
        this.LeftDashOnly.Keyboard.Clear();
        this.RightDashOnly.Keyboard.Clear();
        this.UpDashOnly.Keyboard.Clear();
        this.DownDashOnly.Keyboard.Clear();
      }
      Settings.TranslateKeys(this.Left.Keyboard);
      Settings.TranslateKeys(this.Right.Keyboard);
      Settings.TranslateKeys(this.Down.Keyboard);
      Settings.TranslateKeys(this.Up.Keyboard);
      Settings.TranslateKeys(this.MenuLeft.Keyboard);
      Settings.TranslateKeys(this.MenuRight.Keyboard);
      Settings.TranslateKeys(this.MenuDown.Keyboard);
      Settings.TranslateKeys(this.MenuUp.Keyboard);
      Settings.TranslateKeys(this.Grab.Keyboard);
      Settings.TranslateKeys(this.Jump.Keyboard);
      Settings.TranslateKeys(this.Dash.Keyboard);
      Settings.TranslateKeys(this.Talk.Keyboard);
      Settings.TranslateKeys(this.Pause.Keyboard);
      Settings.TranslateKeys(this.Confirm.Keyboard);
      Settings.TranslateKeys(this.Cancel.Keyboard);
      Settings.TranslateKeys(this.Journal.Keyboard);
      Settings.TranslateKeys(this.QuickRestart.Keyboard);
      Settings.TranslateKeys(this.DemoDash.Keyboard);
      Settings.TranslateKeys(this.LeftMoveOnly.Keyboard);
      Settings.TranslateKeys(this.RightMoveOnly.Keyboard);
      Settings.TranslateKeys(this.UpMoveOnly.Keyboard);
      Settings.TranslateKeys(this.DownMoveOnly.Keyboard);
      Settings.TranslateKeys(this.LeftDashOnly.Keyboard);
      Settings.TranslateKeys(this.RightDashOnly.Keyboard);
      Settings.TranslateKeys(this.UpDashOnly.Keyboard);
      Settings.TranslateKeys(this.DownDashOnly.Keyboard);
    }

    private static void TranslateKeys(List<Keys> keys)
    {
      for (int index = 0; index < keys.Count; ++index)
        keys[index] = Keyboard.GetKeyFromScancodeEXT(keys[index]);
    }

    public void SetDefaultButtonControls(bool reset)
    {
      if (reset || this.Left.Controller.Count <= 0)
      {
        this.Left.Controller.Clear();
        this.Left.Add(Buttons.LeftThumbstickLeft, Buttons.DPadLeft);
      }
      if (reset || this.Right.Controller.Count <= 0)
      {
        this.Right.Controller.Clear();
        this.Right.Add(Buttons.LeftThumbstickRight, Buttons.DPadRight);
      }
      if (reset || this.Down.Controller.Count <= 0)
      {
        this.Down.Controller.Clear();
        this.Down.Add(Buttons.LeftThumbstickDown, Buttons.DPadDown);
      }
      if (reset || this.Up.Controller.Count <= 0)
      {
        this.Up.Controller.Clear();
        this.Up.Add(Buttons.LeftThumbstickUp, Buttons.DPadUp);
      }
      if (reset || this.MenuLeft.Controller.Count <= 0)
      {
        this.MenuLeft.Controller.Clear();
        this.MenuLeft.Add(Buttons.LeftThumbstickLeft, Buttons.DPadLeft);
      }
      if (reset || this.MenuRight.Controller.Count <= 0)
      {
        this.MenuRight.Controller.Clear();
        this.MenuRight.Add(Buttons.LeftThumbstickRight, Buttons.DPadRight);
      }
      if (reset || this.MenuDown.Controller.Count <= 0)
      {
        this.MenuDown.Controller.Clear();
        this.MenuDown.Add(Buttons.LeftThumbstickDown, Buttons.DPadDown);
      }
      if (reset || this.MenuUp.Controller.Count <= 0)
      {
        this.MenuUp.Controller.Clear();
        this.MenuUp.Add(Buttons.LeftThumbstickUp, Buttons.DPadUp);
      }
      if (reset || this.Grab.Controller.Count <= 0)
      {
        this.Grab.Controller.Clear();
        this.Grab.Add(Buttons.LeftTrigger, Buttons.RightTrigger, Buttons.LeftShoulder, Buttons.RightShoulder);
      }
      if (reset || this.Jump.Controller.Count <= 0)
      {
        this.Jump.Controller.Clear();
        this.Jump.Add(Buttons.A, Buttons.Y);
      }
      if (reset || this.Dash.Controller.Count <= 0)
      {
        this.Dash.Controller.Clear();
        this.Dash.Add(Buttons.X, Buttons.B);
      }
      if (reset || this.Talk.Controller.Count <= 0)
      {
        this.Talk.Controller.Clear();
        this.Talk.Add(Buttons.B);
      }
      if (reset || this.Pause.Controller.Count <= 0)
      {
        this.Pause.Controller.Clear();
        this.Pause.Add(Buttons.Start);
      }
      if (reset || this.Confirm.Controller.Count <= 0)
      {
        this.Confirm.Controller.Clear();
        this.Confirm.Add(Buttons.A);
      }
      if (reset || this.Cancel.Controller.Count <= 0)
      {
        this.Cancel.Controller.Clear();
        this.Cancel.Add(Buttons.B, Buttons.X);
      }
      if (reset || this.Journal.Controller.Count <= 0)
      {
        this.Journal.Controller.Clear();
        this.Journal.Add(Buttons.LeftTrigger);
      }
      if (reset || this.QuickRestart.Controller.Count <= 0)
        this.QuickRestart.Controller.Clear();
      if (!reset)
        return;
      this.DemoDash.Controller.Clear();
      this.LeftMoveOnly.Controller.Clear();
      this.RightMoveOnly.Controller.Clear();
      this.UpMoveOnly.Controller.Clear();
      this.DownMoveOnly.Controller.Clear();
      this.LeftDashOnly.Controller.Clear();
      this.RightDashOnly.Controller.Clear();
      this.UpDashOnly.Controller.Clear();
      this.DownDashOnly.Controller.Clear();
    }

    public int MaxScale => Math.Min(Engine.Instance.GraphicsDevice.Adapter.CurrentDisplayMode.Width / 320, Engine.Instance.GraphicsDevice.Adapter.CurrentDisplayMode.Height / 180);

    public void ApplyVolumes()
    {
      this.ApplySFXVolume();
      this.ApplyMusicVolume();
    }

    public void ApplySFXVolume() => Audio.SfxVolume = (float) this.SFXVolume / 10f;

    public void ApplyMusicVolume() => Audio.MusicVolume = (float) this.MusicVolume / 10f;

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
      Fonts.Load(Dialog.Languages[this.Language].FontFace);
    }

    public static void Initialize()
    {
      if (UserIO.Open(UserIO.Mode.Read))
      {
        Settings.Instance = UserIO.Load<Settings>("settings");
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
