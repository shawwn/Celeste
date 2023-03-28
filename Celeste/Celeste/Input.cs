// Decompiled with JetBrains decompiler
// Type: Celeste.Input
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public static class Input
  {
    private static int gamepad = 0;
    public static readonly int MaxBindings = 8;
    public static VirtualButton ESC;
    public static VirtualButton Pause;
    public static VirtualButton MenuLeft;
    public static VirtualButton MenuRight;
    public static VirtualButton MenuUp;
    public static VirtualButton MenuDown;
    public static VirtualButton MenuConfirm;
    public static VirtualButton MenuCancel;
    public static VirtualButton MenuJournal;
    public static VirtualButton QuickRestart;
    public static VirtualIntegerAxis MoveX;
    public static VirtualIntegerAxis MoveY;
    public static VirtualIntegerAxis GliderMoveY;
    public static VirtualJoystick Aim;
    public static VirtualJoystick Feather;
    public static VirtualJoystick MountainAim;
    public static VirtualButton Jump;
    public static VirtualButton Dash;
    public static VirtualButton Grab;
    public static VirtualButton Talk;
    public static VirtualButton CrouchDash;
    private static bool grabToggle;
    public static Vector2 LastAim;
    public static string OverrideInputPrefix = (string) null;
    private static Dictionary<Keys, string> keyNameLookup = new Dictionary<Keys, string>();
    private static Dictionary<Buttons, string> buttonNameLookup = new Dictionary<Buttons, string>();
    private static Dictionary<string, Dictionary<string, string>> guiPathLookup = new Dictionary<string, Dictionary<string, string>>();
    private static float[] rumbleStrengths = new float[4]
    {
      0.15f,
      0.4f,
      1f,
      0.05f
    };
    private static float[] rumbleLengths = new float[5]
    {
      0.1f,
      0.25f,
      0.5f,
      1f,
      2f
    };

    public static int Gamepad
    {
      get => gamepad;
      set
      {
        int num = Calc.Clamp(value, 0, MInput.GamePads.Length - 1);
        if (gamepad == num)
          return;
        gamepad = num;
        Initialize();
      }
    }

    public static void Initialize()
    {
      bool flag = false;
      if (MoveX != null)
        flag = MoveX.Inverted;
      Deregister();
      MoveX = new VirtualIntegerAxis(Settings.Instance.Left, Settings.Instance.LeftMoveOnly, Settings.Instance.Right, Settings.Instance.RightMoveOnly, Gamepad, 0.3f);
      MoveX.Inverted = flag;
      MoveY = new VirtualIntegerAxis(Settings.Instance.Up, Settings.Instance.UpMoveOnly, Settings.Instance.Down, Settings.Instance.DownMoveOnly, Gamepad, 0.7f);
      GliderMoveY = new VirtualIntegerAxis(Settings.Instance.Up, Settings.Instance.UpMoveOnly, Settings.Instance.Down, Settings.Instance.DownMoveOnly, Gamepad, 0.3f);
      Aim = new VirtualJoystick(Settings.Instance.Up, Settings.Instance.UpDashOnly, Settings.Instance.Down, Settings.Instance.DownDashOnly, Settings.Instance.Left, Settings.Instance.LeftDashOnly, Settings.Instance.Right, Settings.Instance.RightDashOnly, Gamepad, 0.25f);
      Aim.InvertedX = flag;
      Feather = new VirtualJoystick(Settings.Instance.Up, Settings.Instance.UpMoveOnly, Settings.Instance.Down, Settings.Instance.DownMoveOnly, Settings.Instance.Left, Settings.Instance.LeftMoveOnly, Settings.Instance.Right, Settings.Instance.RightMoveOnly, Gamepad, 0.25f);
      Feather.InvertedX = flag;
      Jump = new VirtualButton(Settings.Instance.Jump, Gamepad, 0.08f, 0.2f);
      Dash = new VirtualButton(Settings.Instance.Dash, Gamepad, 0.08f, 0.2f);
      Talk = new VirtualButton(Settings.Instance.Talk, Gamepad, 0.08f, 0.2f);
      Grab = new VirtualButton(Settings.Instance.Grab, Gamepad, 0.0f, 0.2f);
      CrouchDash = new VirtualButton(Settings.Instance.DemoDash, Gamepad, 0.08f, 0.2f);
      Binding left = new Binding();
      left.Add(Keys.A);
      left.Add(Buttons.RightThumbstickLeft);
      Binding right = new Binding();
      right.Add(Keys.D);
      right.Add(Buttons.RightThumbstickRight);
      Binding up = new Binding();
      up.Add(Keys.W);
      up.Add(Buttons.RightThumbstickUp);
      Binding down = new Binding();
      down.Add(Keys.S);
      down.Add(Buttons.RightThumbstickDown);
      MountainAim = new VirtualJoystick(up, down, left, right, Gamepad, 0.1f);
      Binding binding = new Binding();
      binding.Add(Keys.Escape);
      ESC = new VirtualButton(binding, Gamepad, 0.1f, 0.2f);
      Pause = new VirtualButton(Settings.Instance.Pause, Gamepad, 0.1f, 0.2f);
      QuickRestart = new VirtualButton(Settings.Instance.QuickRestart, Gamepad, 0.1f, 0.2f);
      MenuLeft = new VirtualButton(Settings.Instance.MenuLeft, Gamepad, 0.0f, 0.4f);
      MenuLeft.SetRepeat(0.4f, 0.1f);
      MenuRight = new VirtualButton(Settings.Instance.MenuRight, Gamepad, 0.0f, 0.4f);
      MenuRight.SetRepeat(0.4f, 0.1f);
      MenuUp = new VirtualButton(Settings.Instance.MenuUp, Gamepad, 0.0f, 0.4f);
      MenuUp.SetRepeat(0.4f, 0.1f);
      MenuDown = new VirtualButton(Settings.Instance.MenuDown, Gamepad, 0.0f, 0.4f);
      MenuDown.SetRepeat(0.4f, 0.1f);
      MenuJournal = new VirtualButton(Settings.Instance.Journal, Gamepad, 0.0f, 0.2f);
      MenuConfirm = new VirtualButton(Settings.Instance.Confirm, Gamepad, 0.0f, 0.2f);
      MenuCancel = new VirtualButton(Settings.Instance.Cancel, Gamepad, 0.0f, 0.2f);
    }

    public static void Deregister()
    {
      if (ESC != null)
        ESC.Deregister();
      if (Pause != null)
        Pause.Deregister();
      if (MenuLeft != null)
        MenuLeft.Deregister();
      if (MenuRight != null)
        MenuRight.Deregister();
      if (MenuUp != null)
        MenuUp.Deregister();
      if (MenuDown != null)
        MenuDown.Deregister();
      if (MenuConfirm != null)
        MenuConfirm.Deregister();
      if (MenuCancel != null)
        MenuCancel.Deregister();
      if (MenuJournal != null)
        MenuJournal.Deregister();
      if (QuickRestart != null)
        QuickRestart.Deregister();
      if (MoveX != null)
        MoveX.Deregister();
      if (MoveY != null)
        MoveY.Deregister();
      if (GliderMoveY != null)
        GliderMoveY.Deregister();
      if (Aim != null)
        Aim.Deregister();
      if (MountainAim != null)
        MountainAim.Deregister();
      if (Jump != null)
        Jump.Deregister();
      if (Dash != null)
        Dash.Deregister();
      if (Grab != null)
        Grab.Deregister();
      if (Talk != null)
        Talk.Deregister();
      if (CrouchDash == null)
        return;
      CrouchDash.Deregister();
    }

    public static bool AnyGamepadConfirmPressed(out int gamepadIndex)
    {
      bool flag = false;
      gamepadIndex = -1;
      int gamepadIndex1 = MenuConfirm.GamepadIndex;
      for (int index = 0; index < MInput.GamePads.Length; ++index)
      {
        MenuConfirm.GamepadIndex = index;
        if (MenuConfirm.Pressed)
        {
          flag = true;
          gamepadIndex = index;
          break;
        }
      }
      MenuConfirm.GamepadIndex = gamepadIndex1;
      return flag;
    }

    public static void Rumble(RumbleStrength strength, RumbleLength length)
    {
      float num = 1f;
      if (Settings.Instance.Rumble == RumbleAmount.Half)
        num = 0.5f;
      if (Settings.Instance.Rumble == RumbleAmount.Off || MInput.GamePads.Length == 0 || MInput.Disabled)
        return;
      MInput.GamePads[Gamepad].Rumble(rumbleStrengths[(int) strength] * num, rumbleLengths[(int) length]);
    }

    public static void RumbleSpecific(float strength, float time)
    {
      float num = 1f;
      if (Settings.Instance.Rumble == RumbleAmount.Half)
        num = 0.5f;
      if (Settings.Instance.Rumble == RumbleAmount.Off || MInput.GamePads.Length == 0 || MInput.Disabled)
        return;
      MInput.GamePads[Gamepad].Rumble(strength * num, time);
    }

    public static bool GrabCheck
    {
      get
      {
        switch (Settings.Instance.GrabMode)
        {
          case GrabModes.Invert:
            return !Grab.Check;
          case GrabModes.Toggle:
            return grabToggle;
          default:
            return Grab.Check;
        }
      }
    }

    public static bool DashPressed
    {
      get
      {
        switch (Settings.Instance.CrouchDashMode)
        {
          case CrouchDashModes.Hold:
            return Dash.Pressed && !CrouchDash.Check;
          default:
            return Dash.Pressed;
        }
      }
    }

    public static bool CrouchDashPressed
    {
      get
      {
        switch (Settings.Instance.CrouchDashMode)
        {
          case CrouchDashModes.Hold:
            return Dash.Pressed && CrouchDash.Check;
          default:
            return CrouchDash.Pressed;
        }
      }
    }

    public static void UpdateGrab()
    {
      if (Settings.Instance.GrabMode != GrabModes.Toggle || !Grab.Pressed)
        return;
      grabToggle = !grabToggle;
    }

    public static void ResetGrab() => grabToggle = false;

    public static Vector2 GetAimVector(Facings defaultFacing = Facings.Right)
    {
      Vector2 vector2 = Aim.Value;
      if (vector2 == Vector2.Zero)
      {
        if (SaveData.Instance != null && SaveData.Instance.Assists.DashAssist)
          return LastAim;
        LastAim = Vector2.UnitX * (float) defaultFacing;
      }
      else if (SaveData.Instance != null && SaveData.Instance.Assists.ThreeSixtyDashing)
      {
        LastAim = vector2.SafeNormalize();
      }
      else
      {
        float radiansA = vector2.Angle();
        float num = (float) (0.39269909262657166 - ((double) radiansA < 0.0 ? 1.0 : 0.0) * 0.0872664600610733);
        LastAim = (double) Calc.AbsAngleDiff(radiansA, 0.0f) >= (double) num ? ((double) Calc.AbsAngleDiff(radiansA, 3.1415927f) >= (double) num ? ((double) Calc.AbsAngleDiff(radiansA, -1.5707964f) >= (double) num ? ((double) Calc.AbsAngleDiff(radiansA, 1.5707964f) >= (double) num ? new Vector2((float) Math.Sign(vector2.X), (float) Math.Sign(vector2.Y)).SafeNormalize() : new Vector2(0.0f, 1f)) : new Vector2(0.0f, -1f)) : new Vector2(-1f, 0.0f)) : new Vector2(1f, 0.0f);
      }
      return LastAim;
    }

    public static string GuiInputPrefix(PrefixMode mode = PrefixMode.Latest)
    {
      if (!string.IsNullOrEmpty(OverrideInputPrefix))
        return OverrideInputPrefix;
      if (!(mode != PrefixMode.Latest ? MInput.GamePads[Gamepad].Attached : MInput.ControllerHasFocus))
        return "keyboard";
      switch (GamePad.GetGUIDEXT(MInput.GamePads[Gamepad].PlayerIndex))
      {
        case "4c05c405":
        case "4c05cc09":
          return "ps4";
        case "7e050920":
        case "7e053003":
          return "ns";
        case "d1180094":
          return "stadia";
        default:
          return "xb1";
      }
    }

    public static bool GuiInputController(PrefixMode mode = PrefixMode.Latest) => !GuiInputPrefix(mode).Equals("keyboard");

    public static MTexture GuiButton(VirtualButton button, PrefixMode mode = PrefixMode.Latest, string fallback = "controls/keyboard/oemquestion")
    {
      string prefix = GuiInputPrefix(mode);
      int num = GuiInputController(mode) ? 1 : 0;
      string input = "";
      if (num != 0)
      {
        using (List<Buttons>.Enumerator enumerator = button.Binding.Controller.GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            Buttons current = enumerator.Current;
            if (!buttonNameLookup.TryGetValue(current, out input))
              buttonNameLookup.Add(current, input = current.ToString());
          }
        }
      }
      else
      {
        Keys key = FirstKey(button);
        if (!keyNameLookup.TryGetValue(key, out input))
          keyNameLookup.Add(key, input = key.ToString());
      }
      MTexture mtexture = GuiTexture(prefix, input);
      return mtexture == null && fallback != null ? GFX.Gui[fallback] : mtexture;
    }

    public static MTexture GuiSingleButton(Buttons button, PrefixMode mode = PrefixMode.Latest, string fallback = "controls/keyboard/oemquestion")
    {
      string prefix = !GuiInputController(mode) ? "xb1" : GuiInputPrefix(mode);
      string input = "";
      if (!buttonNameLookup.TryGetValue(button, out input))
        buttonNameLookup.Add(button, input = button.ToString());
      MTexture mtexture = GuiTexture(prefix, input);
      return mtexture == null && fallback != null ? GFX.Gui[fallback] : mtexture;
    }

    public static MTexture GuiKey(Keys key, string fallback = "controls/keyboard/oemquestion")
    {
      string input;
      if (!keyNameLookup.TryGetValue(key, out input))
        keyNameLookup.Add(key, input = key.ToString());
      MTexture mtexture = GuiTexture("keyboard", input);
      return mtexture == null && fallback != null ? GFX.Gui[fallback] : mtexture;
    }

    public static Buttons FirstButton(VirtualButton button)
    {
      using (List<Buttons>.Enumerator enumerator = button.Binding.Controller.GetEnumerator())
      {
        if (enumerator.MoveNext())
          return enumerator.Current;
      }
      return Buttons.A;
    }

    public static Keys FirstKey(VirtualButton button)
    {
      foreach (Keys keys in button.Binding.Keyboard)
      {
        if (keys != Keys.None)
          return keys;
      }
      return Keys.None;
    }

    public static MTexture GuiDirection(Vector2 direction) => GuiTexture("directions", Math.Sign(direction.X).ToString() + "x" + (object) Math.Sign(direction.Y));

    private static MTexture GuiTexture(string prefix, string input)
    {
      Dictionary<string, string> dictionary;
      if (!guiPathLookup.TryGetValue(prefix, out dictionary))
        guiPathLookup.Add(prefix, dictionary = new Dictionary<string, string>());
      string id;
      if (!dictionary.TryGetValue(input, out id))
        dictionary.Add(input, id = "controls/" + prefix + "/" + input);
      if (GFX.Gui.Has(id))
        return GFX.Gui[id];
      return prefix != "fallback" ? GuiTexture("fallback", input) : (MTexture) null;
    }

    public static void SetLightbarColor(Color color)
    {
      color.R = (byte) (Math.Pow((double) color.R / (double) byte.MaxValue, 3.0) * (double) byte.MaxValue);
      color.G = (byte) (Math.Pow((double) color.G / (double) byte.MaxValue, 3.0) * (double) byte.MaxValue);
      color.B = (byte) (Math.Pow((double) color.B / (double) byte.MaxValue, 3.0) * (double) byte.MaxValue);
      GamePad.SetLightBarEXT((PlayerIndex) Gamepad, color);
    }

    public enum PrefixMode
    {
      Latest,
      Attached,
    }
  }
}
