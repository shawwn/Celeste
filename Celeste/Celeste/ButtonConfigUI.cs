// Decompiled with JetBrains decompiler
// Type: Celeste.ButtonConfigUI
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
  [Tracked(false)]
  public class ButtonConfigUI : TextMenu
  {
    private bool remapping;
    private float remappingEase;
    private Binding remappingBinding;
    private string remappingText;
    private float inputDelay;
    private float timeout;
    private bool closing;
    private float closingDelay;
    private bool waitingForController;
    private bool resetHeld;
    private float resetTime;
    private float resetDelay;
    private List<Buttons> all = new List<Buttons>()
    {
      Buttons.A,
      Buttons.B,
      Buttons.X,
      Buttons.Y,
      Buttons.LeftShoulder,
      Buttons.RightShoulder,
      Buttons.LeftTrigger,
      Buttons.RightTrigger
    };
    public static readonly string StadiaControllerDisclaimer = "No endorsement or affiliation is intended between Stadia and the manufacturers\nof non-Stadia controllers or consoles. STADIA, the Stadia beacon, Google, and related\nmarks and logos are trademarks of Google LLC. All other trademarks are the\nproperty of their respective owners.";

    public ButtonConfigUI()
    {
      this.Add((TextMenu.Item) new TextMenu.Header(Dialog.Clean("BTN_CONFIG_TITLE")));
      this.Add((TextMenu.Item) new InputMappingInfo(true));
      this.Add((TextMenu.Item) new TextMenu.SubHeader(Dialog.Clean("KEY_CONFIG_GAMEPLAY")));
      this.AddMap("LEFT", Settings.Instance.Left);
      this.AddMap("RIGHT", Settings.Instance.Right);
      this.AddMap("UP", Settings.Instance.Up);
      this.AddMap("DOWN", Settings.Instance.Down);
      this.AddMap("JUMP", Settings.Instance.Jump);
      this.AddMap("DASH", Settings.Instance.Dash);
      this.AddMap("GRAB", Settings.Instance.Grab);
      this.AddMap("TALK", Settings.Instance.Talk);
      this.Add((TextMenu.Item) new TextMenu.SubHeader(Dialog.Clean("KEY_CONFIG_MENUS")));
      this.Add((TextMenu.Item) new TextMenu.SubHeader(Dialog.Clean("KEY_CONFIG_MENU_NOTICE"), false));
      this.AddMap("LEFT", Settings.Instance.MenuLeft);
      this.AddMap("RIGHT", Settings.Instance.MenuRight);
      this.AddMap("UP", Settings.Instance.MenuUp);
      this.AddMap("DOWN", Settings.Instance.MenuDown);
      this.AddMap("CONFIRM", Settings.Instance.Confirm);
      this.AddMap("CANCEL", Settings.Instance.Cancel);
      this.AddMap("JOURNAL", Settings.Instance.Journal);
      this.AddMap("PAUSE", Settings.Instance.Pause);
      this.Add((TextMenu.Item) new TextMenu.SubHeader(""));
      TextMenu.Button button = new TextMenu.Button(Dialog.Clean("KEY_CONFIG_RESET"));
      button.IncludeWidthInMeasurement = false;
      button.AlwaysCenter = true;
      button.OnPressed = (Action) (() =>
      {
        this.resetHeld = true;
        this.resetTime = 0.0f;
        this.resetDelay = 0.0f;
      });
      button.ConfirmSfx = "event:/ui/main/button_lowkey";
      this.Add((TextMenu.Item) button);
      this.Add((TextMenu.Item) new TextMenu.SubHeader(Dialog.Clean("KEY_CONFIG_ADVANCED")));
      this.AddMap("QUICKRESTART", Settings.Instance.QuickRestart);
      this.AddMap("DEMO", Settings.Instance.DemoDash);
      this.Add((TextMenu.Item) new TextMenu.SubHeader(Dialog.Clean("KEY_CONFIG_MOVE_ONLY")));
      this.AddMap("LEFT", Settings.Instance.LeftMoveOnly);
      this.AddMap("RIGHT", Settings.Instance.RightMoveOnly);
      this.AddMap("UP", Settings.Instance.UpMoveOnly);
      this.AddMap("DOWN", Settings.Instance.DownMoveOnly);
      this.Add((TextMenu.Item) new TextMenu.SubHeader(Dialog.Clean("KEY_CONFIG_DASH_ONLY")));
      this.AddMap("LEFT", Settings.Instance.LeftDashOnly);
      this.AddMap("RIGHT", Settings.Instance.RightDashOnly);
      this.AddMap("UP", Settings.Instance.UpDashOnly);
      this.AddMap("DOWN", Settings.Instance.DownDashOnly);
      this.OnESC = this.OnCancel = (Action) (() =>
      {
        MenuOptions.UpdateCrouchDashModeVisibility();
        this.Focused = false;
        this.closing = true;
      });
      this.MinWidth = 600f;
      this.Position.Y = this.ScrollTargetY;
      this.Alpha = 0.0f;
    }

    private void AddMap(string label, Binding binding)
    {
      string txt = Dialog.Clean("KEY_CONFIG_" + label);
      this.Add(new TextMenu.Setting(txt, binding, true).Pressed((Action) (() =>
      {
        this.remappingText = txt;
        this.Remap(binding);
      })).AltPressed((Action) (() => this.Clear(binding))));
    }

    private void Remap(Binding binding)
    {
      if (!Input.GuiInputController())
        return;
      this.remapping = true;
      this.remappingBinding = binding;
      this.timeout = 5f;
      this.Focused = false;
    }

    private void AddRemap(Buttons btn)
    {
      while (this.remappingBinding.Controller.Count >= Input.MaxBindings)
        this.remappingBinding.Controller.RemoveAt(0);
      this.remapping = false;
      this.inputDelay = 0.25f;
      if (!this.remappingBinding.Add(btn))
        Audio.Play("event:/ui/main/button_invalid");
      Input.Initialize();
    }

    private void Clear(Binding binding)
    {
      if (binding.ClearGamepad())
        return;
      Audio.Play("event:/ui/main/button_invalid");
    }

    public override void Update()
    {
      if (this.resetHeld)
      {
        this.resetDelay += Engine.DeltaTime;
        this.resetTime += Engine.DeltaTime;
        if ((double) this.resetTime > 1.5)
        {
          this.resetDelay = 0.0f;
          this.resetHeld = false;
          Settings.Instance.SetDefaultButtonControls(true);
          Input.Initialize();
          Audio.Play("event:/ui/main/button_select");
        }
        if (!Input.MenuConfirm.Check && (double) this.resetDelay > 0.30000001192092896)
        {
          Audio.Play("event:/ui/main/button_invalid");
          this.resetHeld = false;
        }
        if (this.resetHeld)
          return;
      }
      base.Update();
      this.Focused = !this.closing && (double) this.inputDelay <= 0.0 && !this.waitingForController && !this.remapping;
      if (!this.closing)
      {
        if (!MInput.GamePads[Input.Gamepad].Attached)
          this.waitingForController = true;
        else if (this.waitingForController)
          this.waitingForController = false;
        if (Input.MenuCancel.Pressed && !this.remapping)
          this.OnCancel();
      }
      if ((double) this.inputDelay > 0.0 && !this.remapping)
        this.inputDelay -= Engine.RawDeltaTime;
      this.remappingEase = Calc.Approach(this.remappingEase, this.remapping ? 1f : 0.0f, Engine.RawDeltaTime * 4f);
      if ((double) this.remappingEase >= 0.25 && this.remapping)
      {
        if (Input.ESC.Pressed || (double) this.timeout <= 0.0 || !Input.GuiInputController())
        {
          this.remapping = false;
          this.Focused = true;
        }
        else
        {
          MInput.GamePadData gamePad = MInput.GamePads[Input.Gamepad];
          float num = 0.25f;
          if (gamePad.LeftStickLeftPressed(num))
            this.AddRemap(Buttons.LeftThumbstickLeft);
          else if (gamePad.LeftStickRightPressed(num))
            this.AddRemap(Buttons.LeftThumbstickRight);
          else if (gamePad.LeftStickUpPressed(num))
            this.AddRemap(Buttons.LeftThumbstickUp);
          else if (gamePad.LeftStickDownPressed(num))
            this.AddRemap(Buttons.LeftThumbstickDown);
          else if (gamePad.RightStickLeftPressed(num))
            this.AddRemap(Buttons.RightThumbstickLeft);
          else if (gamePad.RightStickRightPressed(num))
            this.AddRemap(Buttons.RightThumbstickRight);
          else if (gamePad.RightStickDownPressed(num))
            this.AddRemap(Buttons.RightThumbstickDown);
          else if (gamePad.RightStickUpPressed(num))
            this.AddRemap(Buttons.RightThumbstickUp);
          else if (gamePad.LeftTriggerPressed(num))
            this.AddRemap(Buttons.LeftTrigger);
          else if (gamePad.RightTriggerPressed(num))
            this.AddRemap(Buttons.RightTrigger);
          else if (gamePad.Pressed(Buttons.DPadLeft))
            this.AddRemap(Buttons.DPadLeft);
          else if (gamePad.Pressed(Buttons.DPadRight))
            this.AddRemap(Buttons.DPadRight);
          else if (gamePad.Pressed(Buttons.DPadUp))
            this.AddRemap(Buttons.DPadUp);
          else if (gamePad.Pressed(Buttons.DPadDown))
            this.AddRemap(Buttons.DPadDown);
          else if (gamePad.Pressed(Buttons.A))
            this.AddRemap(Buttons.A);
          else if (gamePad.Pressed(Buttons.B))
            this.AddRemap(Buttons.B);
          else if (gamePad.Pressed(Buttons.X))
            this.AddRemap(Buttons.X);
          else if (gamePad.Pressed(Buttons.Y))
            this.AddRemap(Buttons.Y);
          else if (gamePad.Pressed(Buttons.Start))
            this.AddRemap(Buttons.Start);
          else if (gamePad.Pressed(Buttons.Back))
            this.AddRemap(Buttons.Back);
          else if (gamePad.Pressed(Buttons.LeftShoulder))
            this.AddRemap(Buttons.LeftShoulder);
          else if (gamePad.Pressed(Buttons.RightShoulder))
            this.AddRemap(Buttons.RightShoulder);
          else if (gamePad.Pressed(Buttons.LeftStick))
            this.AddRemap(Buttons.LeftStick);
          else if (gamePad.Pressed(Buttons.RightStick))
            this.AddRemap(Buttons.RightStick);
        }
        this.timeout -= Engine.RawDeltaTime;
      }
      this.closingDelay -= Engine.RawDeltaTime;
      this.Alpha = Calc.Approach(this.Alpha, !this.closing || (double) this.closingDelay > 0.0 ? 1f : 0.0f, Engine.RawDeltaTime * 8f);
      if (!this.closing || (double) this.Alpha > 0.0)
        return;
      this.Close();
    }

    public override void Render()
    {
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * Ease.CubeOut(this.Alpha));
      Vector2 position = new Vector2(1920f, 1080f) * 0.5f;
      if (MInput.GamePads[Input.Gamepad].Attached)
      {
        base.Render();
        if (Celeste.IsGGP && Input.GuiInputPrefix() != "stadia")
        {
          float num = 0.33f;
          ActiveFont.Draw(ButtonConfigUI.StadiaControllerDisclaimer, new Vector2(50f, (float) (1080.0 - 50.0 * (double) this.Alpha)), new Vector2(0.0f, 1f), Vector2.One * num, Color.White * this.Alpha * 0.5f);
        }
        if ((double) this.remappingEase > 0.0)
        {
          Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * 0.95f * Ease.CubeInOut(this.remappingEase));
          ActiveFont.Draw(Dialog.Get("BTN_CONFIG_CHANGING"), position + new Vector2(0.0f, -8f), new Vector2(0.5f, 1f), Vector2.One * 0.7f, Color.LightGray * Ease.CubeIn(this.remappingEase));
          ActiveFont.Draw(this.remappingText, position + new Vector2(0.0f, 8f), new Vector2(0.5f, 0.0f), Vector2.One * 2f, Color.White * Ease.CubeIn(this.remappingEase));
        }
      }
      else
        ActiveFont.Draw(Dialog.Clean("BTN_CONFIG_NOCONTROLLER"), position, new Vector2(0.5f, 0.5f), Vector2.One, Color.White * Ease.CubeOut(this.Alpha));
      if (!this.resetHeld)
        return;
      float num1 = Ease.CubeInOut(Calc.Min(1f, this.resetDelay / 0.2f));
      float num2 = Ease.SineOut(Calc.Min(1f, this.resetTime / 1.5f));
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * 0.95f * num1);
      float width = 480f;
      double x = (1920.0 - (double) width) / 2.0;
      Draw.Rect((float) x, 530f, width, 20f, Color.White * 0.25f * num1);
      Draw.Rect((float) x, 530f, width * num2, 20f, Color.White * num1);
    }
  }
}
