// Decompiled with JetBrains decompiler
// Type: Input
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

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
    private static float[] rumbleLengths = new float[4]
    {
      0.1f,
      0.25f,
      0.5f,
      1f
    };
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
    public static VirtualJoystick Aim;
    public static VirtualJoystick MountainAim;
    public static VirtualButton Jump;
    public static VirtualButton Dash;
    public static VirtualButton Grab;
    public static VirtualButton Talk;

    public static int Gamepad
    {
      get
      {
        return Input.gamepad;
      }
      set
      {
        int num = Calc.Clamp(value, 0, MInput.GamePads.Length - 1);
        if (Input.gamepad == num)
          return;
        Input.gamepad = num;
        Input.Initialize();
      }
    }

    public static void Initialize()
    {
      bool flag = false;
      if (Input.MoveX != null)
        flag = Input.MoveX.Inverted;
      Input.Deregister();
      Input.QuickRestart = new VirtualButton();
      foreach (Keys key in Settings.Instance.QuickRestart)
        Input.QuickRestart.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(key));
      Input.MoveX = new VirtualIntegerAxis(new VirtualAxis.Node[4]
      {
        (VirtualAxis.Node) new VirtualAxis.PadDpadLeftRight(Input.Gamepad),
        (VirtualAxis.Node) new VirtualAxis.PadLeftStickX(Input.Gamepad, 0.3f),
        (VirtualAxis.Node) new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, Keys.Left, Keys.Right),
        (VirtualAxis.Node) new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, Settings.Instance.Left, Settings.Instance.Right)
      });
      Input.MoveX.Inverted = flag;
      Input.MoveY = new VirtualIntegerAxis(new VirtualAxis.Node[5]
      {
        (VirtualAxis.Node) new VirtualAxis.PadDpadUpDown(Input.Gamepad),
        (VirtualAxis.Node) new VirtualAxis.PadLeftStickY(Input.Gamepad, 0.7f),
        (VirtualAxis.Node) new VirtualAxis.PadLeftTrigger(Input.Gamepad, 0.7f),
        (VirtualAxis.Node) new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, Keys.Up, Keys.Down),
        (VirtualAxis.Node) new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, Settings.Instance.Up, Settings.Instance.Down)
      });
      Input.Aim = new VirtualJoystick(false, new VirtualJoystick.Node[4]
      {
        (VirtualJoystick.Node) new VirtualJoystick.PadDpad(Input.Gamepad),
        (VirtualJoystick.Node) new VirtualJoystick.PadLeftStick(Input.Gamepad, 0.25f),
        (VirtualJoystick.Node) new VirtualJoystick.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, Keys.Left, Keys.Right, Keys.Up, Keys.Down),
        (VirtualJoystick.Node) new VirtualJoystick.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, Settings.Instance.Left, Settings.Instance.Right, Settings.Instance.Up, Settings.Instance.Down)
      });
      Input.Aim.InvertedX = flag;
      Input.MountainAim = new VirtualJoystick(true, new VirtualJoystick.Node[2]
      {
        (VirtualJoystick.Node) new VirtualJoystick.PadRightStick(Input.Gamepad, 0.1f),
        (VirtualJoystick.Node) new VirtualJoystick.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, Keys.A, Keys.D, Keys.W, Keys.S)
      });
      Input.Jump = new VirtualButton(0.08f);
      Input.AddButtonsTo(Input.Jump, Settings.Instance.BtnJump);
      foreach (Keys key in Settings.Instance.Jump)
        Input.Jump.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(key));
      Input.Dash = new VirtualButton(0.08f);
      Input.AddButtonsTo(Input.Dash, Settings.Instance.BtnDash);
      foreach (Keys key in Settings.Instance.Dash)
        Input.Dash.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(key));
      Input.Talk = new VirtualButton(0.08f);
      Input.AddButtonsTo(Input.Talk, Settings.Instance.BtnTalk);
      foreach (Keys key in Settings.Instance.Talk)
        Input.Talk.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(key));
      Input.Grab = new VirtualButton();
      Input.AddButtonsTo(Input.Grab, Settings.Instance.BtnGrab);
      foreach (Keys key in Settings.Instance.Grab)
        Input.Grab.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(key));
      Input.ESC = new VirtualButton(new VirtualButton.Node[1]
      {
        (VirtualButton.Node) new VirtualButton.KeyboardKey(Keys.Escape)
      });
      Input.Pause = new VirtualButton(new VirtualButton.Node[1]
      {
        (VirtualButton.Node) new VirtualButton.PadButton(Input.Gamepad, Buttons.Start)
      });
      foreach (Keys key in Settings.Instance.Pause)
        Input.Pause.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(key));
      Input.MenuLeft = new VirtualButton(new VirtualButton.Node[4]
      {
        (VirtualButton.Node) new VirtualButton.KeyboardKey(Keys.Left),
        (VirtualButton.Node) new VirtualButton.KeyboardKey(Settings.Instance.Left),
        (VirtualButton.Node) new VirtualButton.PadDPadLeft(Input.Gamepad),
        (VirtualButton.Node) new VirtualButton.PadLeftStickLeft(Input.Gamepad, 0.4f)
      });
      Input.MenuLeft.SetRepeat(0.4f, 0.1f);
      Input.MenuRight = new VirtualButton(new VirtualButton.Node[4]
      {
        (VirtualButton.Node) new VirtualButton.KeyboardKey(Settings.Instance.Right),
        (VirtualButton.Node) new VirtualButton.KeyboardKey(Keys.Right),
        (VirtualButton.Node) new VirtualButton.PadDPadRight(Input.Gamepad),
        (VirtualButton.Node) new VirtualButton.PadLeftStickRight(Input.Gamepad, 0.4f)
      });
      Input.MenuRight.SetRepeat(0.4f, 0.1f);
      Input.MenuUp = new VirtualButton(new VirtualButton.Node[4]
      {
        (VirtualButton.Node) new VirtualButton.KeyboardKey(Settings.Instance.Up),
        (VirtualButton.Node) new VirtualButton.KeyboardKey(Keys.Up),
        (VirtualButton.Node) new VirtualButton.PadDPadUp(Input.Gamepad),
        (VirtualButton.Node) new VirtualButton.PadLeftStickUp(Input.Gamepad, 0.4f)
      });
      Input.MenuUp.SetRepeat(0.4f, 0.1f);
      Input.MenuDown = new VirtualButton(new VirtualButton.Node[4]
      {
        (VirtualButton.Node) new VirtualButton.KeyboardKey(Settings.Instance.Down),
        (VirtualButton.Node) new VirtualButton.KeyboardKey(Keys.Down),
        (VirtualButton.Node) new VirtualButton.PadDPadDown(Input.Gamepad),
        (VirtualButton.Node) new VirtualButton.PadLeftStickDown(Input.Gamepad, 0.4f)
      });
      Input.MenuDown.SetRepeat(0.4f, 0.1f);
      Input.MenuJournal = new VirtualButton(new VirtualButton.Node[2]
      {
        (VirtualButton.Node) new VirtualButton.PadLeftTrigger(Input.Gamepad, 0.25f),
        (VirtualButton.Node) new VirtualButton.PadButton(Input.Gamepad, Buttons.LeftShoulder)
      });
      foreach (Keys key in Settings.Instance.Journal)
        Input.MenuJournal.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(key));
      Input.MenuConfirm = new VirtualButton(new VirtualButton.Node[1]
      {
        (VirtualButton.Node) new VirtualButton.PadButton(Input.Gamepad, Buttons.A)
      });
      foreach (Keys key in Settings.Instance.Confirm)
        Input.MenuConfirm.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(key));
      Input.MenuConfirm.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(Keys.Enter));
      Input.MenuCancel = new VirtualButton(new VirtualButton.Node[2]
      {
        (VirtualButton.Node) new VirtualButton.PadButton(Input.Gamepad, Buttons.X),
        (VirtualButton.Node) new VirtualButton.PadButton(Input.Gamepad, Buttons.B)
      });
      foreach (Keys key in Settings.Instance.Cancel)
        Input.MenuCancel.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(key));
      Input.MenuCancel.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(Keys.Back));
    }

    public static void Deregister()
    {
      if (Input.ESC != null)
        Input.ESC.Deregister();
      if (Input.Pause != null)
        Input.Pause.Deregister();
      if (Input.MenuLeft != null)
        Input.MenuLeft.Deregister();
      if (Input.MenuRight != null)
        Input.MenuRight.Deregister();
      if (Input.MenuUp != null)
        Input.MenuUp.Deregister();
      if (Input.MenuDown != null)
        Input.MenuDown.Deregister();
      if (Input.MenuConfirm != null)
        Input.MenuConfirm.Deregister();
      if (Input.MenuCancel != null)
        Input.MenuCancel.Deregister();
      if (Input.MenuJournal != null)
        Input.MenuJournal.Deregister();
      if (Input.QuickRestart != null)
        Input.QuickRestart.Deregister();
      if (Input.MoveX != null)
        Input.MoveX.Deregister();
      if (Input.MoveY != null)
        Input.MoveY.Deregister();
      if (Input.Aim != null)
        Input.Aim.Deregister();
      if (Input.MountainAim != null)
        Input.MountainAim.Deregister();
      if (Input.Jump != null)
        Input.Jump.Deregister();
      if (Input.Dash != null)
        Input.Dash.Deregister();
      if (Input.Grab != null)
        Input.Grab.Deregister();
      if (Input.Talk == null)
        return;
      Input.Talk.Deregister();
    }

    private static void AddButtonsTo(VirtualButton vbtn, List<Buttons> buttons)
    {
      foreach (Buttons button in buttons)
      {
        switch (button)
        {
          case Buttons.RightTrigger:
            vbtn.Nodes.Add((VirtualButton.Node) new VirtualButton.PadRightTrigger(Input.Gamepad, 0.25f));
            break;
          case Buttons.LeftTrigger:
            vbtn.Nodes.Add((VirtualButton.Node) new VirtualButton.PadLeftTrigger(Input.Gamepad, 0.25f));
            break;
          default:
            vbtn.Nodes.Add((VirtualButton.Node) new VirtualButton.PadButton(Input.Gamepad, button));
            break;
        }
      }
    }

    public static bool QuickResetPressed()
    {
      if (Input.QuickRestart.Pressed)
        return true;
      MInput.GamePadData gamePad = MInput.GamePads[Input.gamepad];
      foreach (Buttons button in Settings.Instance.BtnAltQuickRestart)
      {
        if (gamePad.Pressed(button))
          return true;
      }
      return (gamePad.Check(Buttons.LeftShoulder) || gamePad.Check(Buttons.LeftTrigger)) && (gamePad.Check(Buttons.RightShoulder) || gamePad.Check(Buttons.RightTrigger)) && Input.Pause.Pressed;
    }

    public static bool AnyGamepadConfirmPressed(out int gamepadIndex)
    {
      if (Input.MenuConfirm.Pressed)
      {
        gamepadIndex = Input.Gamepad;
        return true;
      }
      foreach (VirtualButton.Node node in Input.MenuConfirm.Nodes)
      {
        VirtualButton.PadButton padButton = node as VirtualButton.PadButton;
        if (padButton != null)
        {
          for (int index = 0; index < MInput.GamePads.Length; ++index)
          {
            if (MInput.GamePads[index].Attached && MInput.GamePads[index].Pressed(padButton.Button))
            {
              gamepadIndex = index;
              return true;
            }
          }
        }
      }
      gamepadIndex = -1;
      return false;
    }

    public static void Rumble(RumbleStrength strength, RumbleLength length)
    {
      float num = 1f;
      if (Settings.Instance.Rumble == RumbleAmount.Half)
        num = 0.5f;
      if (Settings.Instance.Rumble == RumbleAmount.Off || MInput.GamePads.Length == 0 || MInput.Disabled)
        return;
      MInput.GamePads[Input.Gamepad].Rumble(Input.rumbleStrengths[(int) strength] * num, Input.rumbleLengths[(int) length]);
    }

    public static void RumbleSpecific(float strength, float time)
    {
      float num = 1f;
      if (Settings.Instance.Rumble == RumbleAmount.Half)
        num = 0.5f;
      if (Settings.Instance.Rumble == RumbleAmount.Off || MInput.GamePads.Length == 0 || MInput.Disabled)
        return;
      MInput.GamePads[Input.Gamepad].Rumble(strength * num, time);
    }

    public static Vector2 GetAimVector(Facings defaultFacing = Facings.Right)
    {
      Vector2 vector2 = Input.Aim.Value;
      if (vector2 == Vector2.Zero)
        return Vector2.UnitX * (float) defaultFacing;
      if (SaveData.Instance != null && SaveData.Instance.Assists.ThreeSixtyDashing)
        return vector2.SafeNormalize();
      float radiansA = vector2.Angle();
      float num = (float) (0.392699092626572 - ((double) radiansA < 0.0 ? 1.0 : 0.0) * 0.0872664600610733);
      if ((double) Calc.AbsAngleDiff(radiansA, 0.0f) < (double) num)
        return new Vector2(1f, 0.0f);
      if ((double) Calc.AbsAngleDiff(radiansA, 3.141593f) < (double) num)
        return new Vector2(-1f, 0.0f);
      if ((double) Calc.AbsAngleDiff(radiansA, -1.570796f) < (double) num)
        return new Vector2(0.0f, -1f);
      if ((double) Calc.AbsAngleDiff(radiansA, 1.570796f) < (double) num)
        return new Vector2(0.0f, 1f);
      return new Vector2((float) Math.Sign(vector2.X), (float) Math.Sign(vector2.Y)).SafeNormalize();
    }

    private static string GuiInputPrefix()
    {
      if (!string.IsNullOrEmpty(Input.OverrideInputPrefix))
        return Input.OverrideInputPrefix;
      if (!MInput.GamePads[Input.Gamepad].Attached)
        return "keyboard";
      string guidext = GamePad.GetGUIDEXT(MInput.GamePads[Input.Gamepad].PlayerIndex);
      if (guidext.Equals("4c05c405") || guidext.Equals("4c05cc09"))
        return "ps4";
      return guidext.Equals("7e050920") || guidext.Equals("7e053003") ? "ns" : "xb1";
    }

    public static bool GuiInputController()
    {
      return !Input.GuiInputPrefix().Equals("keyboard");
    }

    public static MTexture GuiButton(VirtualButton button, string fallback = "controls/keyboard/oemquestion")
    {
      string prefix = Input.GuiInputPrefix();
      bool flag = Input.GuiInputController();
      string input = "";
      if (flag)
      {
        foreach (VirtualButton.Node node in button.Nodes)
        {
          if (node is VirtualButton.PadButton)
          {
            Buttons button1 = (node as VirtualButton.PadButton).Button;
            if (!Input.buttonNameLookup.TryGetValue(button1, out input))
            {
              Input.buttonNameLookup.Add(button1, input = button1.ToString());
              break;
            }
            break;
          }
          if (node is VirtualButton.PadLeftTrigger)
          {
            input = "LeftTrigger";
            break;
          }
          if (node is VirtualButton.PadRightTrigger)
          {
            input = "RightTrigger";
            break;
          }
        }
      }
      else
      {
        Keys key = Input.FirstKey(button);
        if (!Input.keyNameLookup.TryGetValue(key, out input))
          Input.keyNameLookup.Add(key, input = key.ToString());
      }
      MTexture mtexture = Input.GuiTexture(prefix, input);
      if (mtexture == null && fallback != null)
        return GFX.Gui[fallback];
      return mtexture;
    }

    public static MTexture GuiSingleButton(Buttons button, string fallback = "controls/keyboard/oemquestion")
    {
      string prefix = !Input.GuiInputController() ? "xb1" : Input.GuiInputPrefix();
      string input = "";
      switch (button)
      {
        case Buttons.RightTrigger:
          input = "RightTrigger";
          break;
        case Buttons.LeftTrigger:
          input = "LeftTrigger";
          break;
        default:
          if (!Input.buttonNameLookup.TryGetValue(button, out input))
            Input.buttonNameLookup.Add(button, input = button.ToString());
          break;
      }
      MTexture mtexture = Input.GuiTexture(prefix, input);
      if (mtexture == null && fallback != null)
        return GFX.Gui[fallback];
      return mtexture;
    }

    public static MTexture GuiKey(Keys key, string fallback = "controls/keyboard/oemquestion")
    {
      string input;
      if (!Input.keyNameLookup.TryGetValue(key, out input))
        Input.keyNameLookup.Add(key, input = key.ToString());
      MTexture mtexture = Input.GuiTexture("keyboard", input);
      if (mtexture == null && fallback != null)
        return GFX.Gui[fallback];
      return mtexture;
    }

    public static Buttons FirstButton(VirtualButton button)
    {
      foreach (VirtualButton.Node node in button.Nodes)
      {
        if (node is VirtualButton.PadButton)
          return (node as VirtualButton.PadButton).Button;
      }
      return Buttons.A;
    }

    public static Keys FirstKey(VirtualButton button)
    {
      foreach (VirtualButton.Node node in button.Nodes)
      {
        if (node is VirtualButton.KeyboardKey && (uint) (node as VirtualButton.KeyboardKey).Key > 0U)
          return (node as VirtualButton.KeyboardKey).Key;
      }
      return Keys.None;
    }

    public static MTexture GuiDirection(Vector2 direction)
    {
      return Input.GuiTexture(Input.GuiInputPrefix(), Math.Sign(direction.X).ToString() + "x" + (object) Math.Sign(direction.Y));
    }

    private static MTexture GuiTexture(string prefix, string input)
    {
      Dictionary<string, string> dictionary;
      if (!Input.guiPathLookup.TryGetValue(prefix, out dictionary))
        Input.guiPathLookup.Add(prefix, dictionary = new Dictionary<string, string>());
      string id;
      if (!dictionary.TryGetValue(input, out id))
        dictionary.Add(input, id = "controls/" + prefix + "/" + input);
      if (!GFX.Gui.Has(id))
        return (MTexture) null;
      return GFX.Gui[id];
    }

    public static void SetLightbarColor(Color color)
    {
      color.R = (byte) (Math.Pow((double) color.R / (double) byte.MaxValue, 3.0) * (double) byte.MaxValue);
      color.G = (byte) (Math.Pow((double) color.G / (double) byte.MaxValue, 3.0) * (double) byte.MaxValue);
      color.B = (byte) (Math.Pow((double) color.B / (double) byte.MaxValue, 3.0) * (double) byte.MaxValue);
      GamePad.SetLightBarEXT((PlayerIndex) Input.Gamepad, color);
    }
  }
}

