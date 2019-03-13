// Decompiled with JetBrains decompiler
// Type: Celeste.Input
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
        return Celeste.Input.gamepad;
      }
      set
      {
        int num = Calc.Clamp(value, 0, MInput.GamePads.Length - 1);
        if (Celeste.Input.gamepad == num)
          return;
        Celeste.Input.gamepad = num;
        Celeste.Input.Initialize();
      }
    }

    public static void Initialize()
    {
      bool flag = false;
      if (Celeste.Input.MoveX != null)
        flag = Celeste.Input.MoveX.Inverted;
      Celeste.Input.Deregister();
      Celeste.Input.QuickRestart = new VirtualButton();
      using (List<Keys>.Enumerator enumerator = Settings.Instance.QuickRestart.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Keys current = enumerator.Current;
          Celeste.Input.QuickRestart.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(current));
        }
      }
      Celeste.Input.MoveX = new VirtualIntegerAxis(new VirtualAxis.Node[4]
      {
        (VirtualAxis.Node) new VirtualAxis.PadDpadLeftRight(Celeste.Input.Gamepad),
        (VirtualAxis.Node) new VirtualAxis.PadLeftStickX(Celeste.Input.Gamepad, 0.3f),
        (VirtualAxis.Node) new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, (Keys) 37, (Keys) 39),
        (VirtualAxis.Node) new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, Settings.Instance.Left, Settings.Instance.Right)
      });
      Celeste.Input.MoveX.Inverted = flag;
      Celeste.Input.MoveY = new VirtualIntegerAxis(new VirtualAxis.Node[4]
      {
        (VirtualAxis.Node) new VirtualAxis.PadDpadUpDown(Celeste.Input.Gamepad),
        (VirtualAxis.Node) new VirtualAxis.PadLeftStickY(Celeste.Input.Gamepad, 0.7f),
        (VirtualAxis.Node) new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, (Keys) 38, (Keys) 40),
        (VirtualAxis.Node) new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, Settings.Instance.Up, Settings.Instance.Down)
      });
      Celeste.Input.Aim = new VirtualJoystick(false, new VirtualJoystick.Node[4]
      {
        (VirtualJoystick.Node) new VirtualJoystick.PadDpad(Celeste.Input.Gamepad),
        (VirtualJoystick.Node) new VirtualJoystick.PadLeftStick(Celeste.Input.Gamepad, 0.25f),
        (VirtualJoystick.Node) new VirtualJoystick.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, (Keys) 37, (Keys) 39, (Keys) 38, (Keys) 40),
        (VirtualJoystick.Node) new VirtualJoystick.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, Settings.Instance.Left, Settings.Instance.Right, Settings.Instance.Up, Settings.Instance.Down)
      });
      Celeste.Input.Aim.InvertedX = flag;
      Celeste.Input.MountainAim = new VirtualJoystick(true, new VirtualJoystick.Node[2]
      {
        (VirtualJoystick.Node) new VirtualJoystick.PadRightStick(Celeste.Input.Gamepad, 0.1f),
        (VirtualJoystick.Node) new VirtualJoystick.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, (Keys) 65, (Keys) 68, (Keys) 87, (Keys) 83)
      });
      Celeste.Input.Jump = new VirtualButton(0.08f);
      Celeste.Input.AddButtonsTo(Celeste.Input.Jump, Settings.Instance.BtnJump);
      using (List<Keys>.Enumerator enumerator = Settings.Instance.Jump.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Keys current = enumerator.Current;
          Celeste.Input.Jump.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(current));
        }
      }
      Celeste.Input.Dash = new VirtualButton(0.08f);
      Celeste.Input.AddButtonsTo(Celeste.Input.Dash, Settings.Instance.BtnDash);
      using (List<Keys>.Enumerator enumerator = Settings.Instance.Dash.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Keys current = enumerator.Current;
          Celeste.Input.Dash.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(current));
        }
      }
      Celeste.Input.Talk = new VirtualButton(0.08f);
      Celeste.Input.AddButtonsTo(Celeste.Input.Talk, Settings.Instance.BtnTalk);
      using (List<Keys>.Enumerator enumerator = Settings.Instance.Talk.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Keys current = enumerator.Current;
          Celeste.Input.Talk.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(current));
        }
      }
      Celeste.Input.Grab = new VirtualButton();
      Celeste.Input.AddButtonsTo(Celeste.Input.Grab, Settings.Instance.BtnGrab);
      using (List<Keys>.Enumerator enumerator = Settings.Instance.Grab.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Keys current = enumerator.Current;
          Celeste.Input.Grab.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(current));
        }
      }
      Celeste.Input.ESC = new VirtualButton(new VirtualButton.Node[1]
      {
        (VirtualButton.Node) new VirtualButton.KeyboardKey((Keys) 27)
      });
      Celeste.Input.Pause = new VirtualButton(new VirtualButton.Node[1]
      {
        (VirtualButton.Node) new VirtualButton.PadButton(Celeste.Input.Gamepad, (Buttons) 16)
      });
      using (List<Keys>.Enumerator enumerator = Settings.Instance.Pause.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Keys current = enumerator.Current;
          Celeste.Input.Pause.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(current));
        }
      }
      Celeste.Input.MenuLeft = new VirtualButton(new VirtualButton.Node[4]
      {
        (VirtualButton.Node) new VirtualButton.KeyboardKey((Keys) 37),
        (VirtualButton.Node) new VirtualButton.KeyboardKey(Settings.Instance.Left),
        (VirtualButton.Node) new VirtualButton.PadDPadLeft(Celeste.Input.Gamepad),
        (VirtualButton.Node) new VirtualButton.PadLeftStickLeft(Celeste.Input.Gamepad, 0.4f)
      });
      Celeste.Input.MenuLeft.SetRepeat(0.4f, 0.1f);
      Celeste.Input.MenuRight = new VirtualButton(new VirtualButton.Node[4]
      {
        (VirtualButton.Node) new VirtualButton.KeyboardKey(Settings.Instance.Right),
        (VirtualButton.Node) new VirtualButton.KeyboardKey((Keys) 39),
        (VirtualButton.Node) new VirtualButton.PadDPadRight(Celeste.Input.Gamepad),
        (VirtualButton.Node) new VirtualButton.PadLeftStickRight(Celeste.Input.Gamepad, 0.4f)
      });
      Celeste.Input.MenuRight.SetRepeat(0.4f, 0.1f);
      Celeste.Input.MenuUp = new VirtualButton(new VirtualButton.Node[4]
      {
        (VirtualButton.Node) new VirtualButton.KeyboardKey(Settings.Instance.Up),
        (VirtualButton.Node) new VirtualButton.KeyboardKey((Keys) 38),
        (VirtualButton.Node) new VirtualButton.PadDPadUp(Celeste.Input.Gamepad),
        (VirtualButton.Node) new VirtualButton.PadLeftStickUp(Celeste.Input.Gamepad, 0.4f)
      });
      Celeste.Input.MenuUp.SetRepeat(0.4f, 0.1f);
      Celeste.Input.MenuDown = new VirtualButton(new VirtualButton.Node[4]
      {
        (VirtualButton.Node) new VirtualButton.KeyboardKey(Settings.Instance.Down),
        (VirtualButton.Node) new VirtualButton.KeyboardKey((Keys) 40),
        (VirtualButton.Node) new VirtualButton.PadDPadDown(Celeste.Input.Gamepad),
        (VirtualButton.Node) new VirtualButton.PadLeftStickDown(Celeste.Input.Gamepad, 0.4f)
      });
      Celeste.Input.MenuDown.SetRepeat(0.4f, 0.1f);
      Celeste.Input.MenuJournal = new VirtualButton(new VirtualButton.Node[2]
      {
        (VirtualButton.Node) new VirtualButton.PadLeftTrigger(Celeste.Input.Gamepad, 0.25f),
        (VirtualButton.Node) new VirtualButton.PadButton(Celeste.Input.Gamepad, (Buttons) 256)
      });
      using (List<Keys>.Enumerator enumerator = Settings.Instance.Journal.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Keys current = enumerator.Current;
          Celeste.Input.MenuJournal.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(current));
        }
      }
      Celeste.Input.MenuConfirm = new VirtualButton(new VirtualButton.Node[1]
      {
        (VirtualButton.Node) new VirtualButton.PadButton(Celeste.Input.Gamepad, (Buttons) 4096)
      });
      using (List<Keys>.Enumerator enumerator = Settings.Instance.Confirm.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Keys current = enumerator.Current;
          Celeste.Input.MenuConfirm.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(current));
        }
      }
      Celeste.Input.MenuConfirm.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey((Keys) 13));
      Celeste.Input.MenuCancel = new VirtualButton(new VirtualButton.Node[2]
      {
        (VirtualButton.Node) new VirtualButton.PadButton(Celeste.Input.Gamepad, (Buttons) 16384),
        (VirtualButton.Node) new VirtualButton.PadButton(Celeste.Input.Gamepad, (Buttons) 8192)
      });
      using (List<Keys>.Enumerator enumerator = Settings.Instance.Cancel.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Keys current = enumerator.Current;
          Celeste.Input.MenuCancel.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey(current));
        }
      }
      Celeste.Input.MenuCancel.Nodes.Add((VirtualButton.Node) new VirtualButton.KeyboardKey((Keys) 8));
    }

    public static void Deregister()
    {
      if (Celeste.Input.ESC != null)
        Celeste.Input.ESC.Deregister();
      if (Celeste.Input.Pause != null)
        Celeste.Input.Pause.Deregister();
      if (Celeste.Input.MenuLeft != null)
        Celeste.Input.MenuLeft.Deregister();
      if (Celeste.Input.MenuRight != null)
        Celeste.Input.MenuRight.Deregister();
      if (Celeste.Input.MenuUp != null)
        Celeste.Input.MenuUp.Deregister();
      if (Celeste.Input.MenuDown != null)
        Celeste.Input.MenuDown.Deregister();
      if (Celeste.Input.MenuConfirm != null)
        Celeste.Input.MenuConfirm.Deregister();
      if (Celeste.Input.MenuCancel != null)
        Celeste.Input.MenuCancel.Deregister();
      if (Celeste.Input.MenuJournal != null)
        Celeste.Input.MenuJournal.Deregister();
      if (Celeste.Input.QuickRestart != null)
        Celeste.Input.QuickRestart.Deregister();
      if (Celeste.Input.MoveX != null)
        Celeste.Input.MoveX.Deregister();
      if (Celeste.Input.MoveY != null)
        Celeste.Input.MoveY.Deregister();
      if (Celeste.Input.Aim != null)
        Celeste.Input.Aim.Deregister();
      if (Celeste.Input.MountainAim != null)
        Celeste.Input.MountainAim.Deregister();
      if (Celeste.Input.Jump != null)
        Celeste.Input.Jump.Deregister();
      if (Celeste.Input.Dash != null)
        Celeste.Input.Dash.Deregister();
      if (Celeste.Input.Grab != null)
        Celeste.Input.Grab.Deregister();
      if (Celeste.Input.Talk == null)
        return;
      Celeste.Input.Talk.Deregister();
    }

    private static void AddButtonsTo(VirtualButton vbtn, List<Buttons> buttons)
    {
      using (List<Buttons>.Enumerator enumerator = buttons.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Buttons current = enumerator.Current;
          if (current == 8388608)
            vbtn.Nodes.Add((VirtualButton.Node) new VirtualButton.PadLeftTrigger(Celeste.Input.Gamepad, 0.25f));
          else if (current == 4194304)
            vbtn.Nodes.Add((VirtualButton.Node) new VirtualButton.PadRightTrigger(Celeste.Input.Gamepad, 0.25f));
          else
            vbtn.Nodes.Add((VirtualButton.Node) new VirtualButton.PadButton(Celeste.Input.Gamepad, current));
        }
      }
    }

    public static bool QuickResetPressed()
    {
      if (Celeste.Input.QuickRestart.Pressed)
        return true;
      MInput.GamePadData gamePad = MInput.GamePads[Celeste.Input.gamepad];
      using (List<Buttons>.Enumerator enumerator = Settings.Instance.BtnAltQuickRestart.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Buttons current = enumerator.Current;
          if (gamePad.Pressed(current))
            return true;
        }
      }
      return (gamePad.Check((Buttons) 256) || gamePad.Check((Buttons) 8388608)) && (gamePad.Check((Buttons) 512) || gamePad.Check((Buttons) 4194304)) && Celeste.Input.Pause.Pressed;
    }

    public static bool AnyGamepadConfirmPressed(out int gamepadIndex)
    {
      if (Celeste.Input.MenuConfirm.Pressed)
      {
        gamepadIndex = Celeste.Input.Gamepad;
        return true;
      }
      foreach (VirtualButton.Node node in Celeste.Input.MenuConfirm.Nodes)
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
      MInput.GamePads[Celeste.Input.Gamepad].Rumble(Celeste.Input.rumbleStrengths[(int) strength] * num, Celeste.Input.rumbleLengths[(int) length]);
    }

    public static void RumbleSpecific(float strength, float time)
    {
      float num = 1f;
      if (Settings.Instance.Rumble == RumbleAmount.Half)
        num = 0.5f;
      if (Settings.Instance.Rumble == RumbleAmount.Off || MInput.GamePads.Length == 0 || MInput.Disabled)
        return;
      MInput.GamePads[Celeste.Input.Gamepad].Rumble(strength * num, time);
    }

    public static Vector2 GetAimVector(Facings defaultFacing = Facings.Right)
    {
      Vector2 vector2 = Celeste.Input.Aim.Value;
      if (Vector2.op_Equality(vector2, Vector2.get_Zero()))
        return Vector2.op_Multiply(Vector2.get_UnitX(), (float) defaultFacing);
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
      return new Vector2((float) Math.Sign((float) vector2.X), (float) Math.Sign((float) vector2.Y)).SafeNormalize();
    }

    private static string GuiInputPrefix()
    {
      if (!string.IsNullOrEmpty(Celeste.Input.OverrideInputPrefix))
        return Celeste.Input.OverrideInputPrefix;
      return MInput.GamePads[Celeste.Input.Gamepad].Attached ? "xb1" : "keyboard";
    }

    public static bool GuiInputController()
    {
      return !Celeste.Input.GuiInputPrefix().Equals("keyboard");
    }

    public static MTexture GuiButton(VirtualButton button, string fallback = "controls/keyboard/oemquestion")
    {
      string prefix = Celeste.Input.GuiInputPrefix();
      int num = Celeste.Input.GuiInputController() ? 1 : 0;
      string input = "";
      if (num != 0)
      {
        foreach (VirtualButton.Node node in button.Nodes)
        {
          if (node is VirtualButton.PadButton)
          {
            Buttons button1 = (node as VirtualButton.PadButton).Button;
            if (!Celeste.Input.buttonNameLookup.TryGetValue(button1, out input))
            {
              Celeste.Input.buttonNameLookup.Add(button1, input = button1.ToString());
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
        Keys key = Celeste.Input.FirstKey(button);
        if (!Celeste.Input.keyNameLookup.TryGetValue(key, out input))
          Celeste.Input.keyNameLookup.Add(key, input = key.ToString());
      }
      MTexture mtexture = Celeste.Input.GuiTexture(prefix, input);
      if (mtexture == null && fallback != null)
        return GFX.Gui[fallback];
      return mtexture;
    }

    public static MTexture GuiSingleButton(Buttons button, string fallback = "controls/keyboard/oemquestion")
    {
      string prefix = !Celeste.Input.GuiInputController() ? "xb1" : Celeste.Input.GuiInputPrefix();
      string input = "";
      if (button == 8388608)
        input = "LeftTrigger";
      else if (button == 4194304)
        input = "RightTrigger";
      else if (!Celeste.Input.buttonNameLookup.TryGetValue(button, out input))
        Celeste.Input.buttonNameLookup.Add(button, input = button.ToString());
      MTexture mtexture = Celeste.Input.GuiTexture(prefix, input);
      if (mtexture == null && fallback != null)
        return GFX.Gui[fallback];
      return mtexture;
    }

    public static MTexture GuiKey(Keys key, string fallback = "controls/keyboard/oemquestion")
    {
      string input;
      if (!Celeste.Input.keyNameLookup.TryGetValue(key, out input))
        Celeste.Input.keyNameLookup.Add(key, input = key.ToString());
      MTexture mtexture = Celeste.Input.GuiTexture("keyboard", input);
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
      return (Buttons) 4096;
    }

    public static Keys FirstKey(VirtualButton button)
    {
      foreach (VirtualButton.Node node in button.Nodes)
      {
        if (node is VirtualButton.KeyboardKey && (node as VirtualButton.KeyboardKey).Key != null)
          return (node as VirtualButton.KeyboardKey).Key;
      }
      return (Keys) 0;
    }

    public static MTexture GuiDirection(Vector2 direction)
    {
      return Celeste.Input.GuiTexture(Celeste.Input.GuiInputPrefix(), Math.Sign((float) direction.X).ToString() + "x" + (object) Math.Sign((float) direction.Y));
    }

    private static MTexture GuiTexture(string prefix, string input)
    {
      Dictionary<string, string> dictionary;
      if (!Celeste.Input.guiPathLookup.TryGetValue(prefix, out dictionary))
        Celeste.Input.guiPathLookup.Add(prefix, dictionary = new Dictionary<string, string>());
      string id;
      if (!dictionary.TryGetValue(input, out id))
        dictionary.Add(input, id = "controls/" + prefix + "/" + input);
      if (!GFX.Gui.Has(id))
        return (MTexture) null;
      return GFX.Gui[id];
    }

    public static void SetLightbarColor(Color color)
    {
    }
  }
}
