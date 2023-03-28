// Decompiled with JetBrains decompiler
// Type: Celeste.KeyboardConfigUI
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class KeyboardConfigUI : TextMenu
  {
    private bool remapping;
    private float remappingEase;
    private Binding remappingBinding;
    private string remappingText;
    private float inputDelay;
    private float timeout;
    private bool closing;
    private float closingDelay;
    private bool resetHeld;
    private float resetTime;
    private float resetDelay;

    public KeyboardConfigUI()
    {
      this.Add((TextMenu.Item) new TextMenu.Header(Dialog.Clean("KEY_CONFIG_TITLE")));
      this.Add((TextMenu.Item) new InputMappingInfo(false));
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
      button.ConfirmSfx = "event:/ui/main/button_lowkey";
      button.OnPressed = (Action) (() =>
      {
        this.resetHeld = true;
        this.resetTime = 0.0f;
        this.resetDelay = 0.0f;
      });
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
      this.Add(new TextMenu.Setting(txt, binding, false).Pressed((Action) (() =>
      {
        this.remappingText = txt;
        this.Remap(binding);
      })).AltPressed((Action) (() => this.Clear(binding))));
    }

    private void Remap(Binding binding)
    {
      this.remapping = true;
      this.remappingBinding = binding;
      this.timeout = 5f;
      this.Focused = false;
    }

    private void AddRemap(Keys key)
    {
      while (this.remappingBinding.Keyboard.Count >= Input.MaxBindings)
        this.remappingBinding.Keyboard.RemoveAt(0);
      this.remapping = false;
      this.inputDelay = 0.25f;
      if (!this.remappingBinding.Add(key))
        Audio.Play("event:/ui/main/button_invalid");
      Input.Initialize();
    }

    private void Clear(Binding binding)
    {
      if (binding.ClearKeyboard())
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
          Settings.Instance.SetDefaultKeyboardControls(true);
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
      this.Focused = !this.closing && (double) this.inputDelay <= 0.0 && !this.remapping;
      if (!this.closing && Input.MenuCancel.Pressed && !this.remapping)
        this.OnCancel();
      if ((double) this.inputDelay > 0.0 && !this.remapping)
        this.inputDelay -= Engine.RawDeltaTime;
      this.remappingEase = Calc.Approach(this.remappingEase, this.remapping ? 1f : 0.0f, Engine.RawDeltaTime * 4f);
      if ((double) this.remappingEase >= 0.25 && this.remapping)
      {
        if (Input.ESC.Pressed || (double) this.timeout <= 0.0)
        {
          this.remapping = false;
          this.Focused = true;
        }
        else
        {
          Keys[] pressedKeys = MInput.Keyboard.CurrentState.GetPressedKeys();
          if (pressedKeys != null && pressedKeys.Length != 0 && MInput.Keyboard.Pressed(pressedKeys[pressedKeys.Length - 1]))
            this.AddRemap(pressedKeys[pressedKeys.Length - 1]);
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
      Vector2 vector2 = new Vector2(1920f, 1080f) * 0.5f;
      base.Render();
      if ((double) this.remappingEase > 0.0)
      {
        Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * 0.95f * Ease.CubeInOut(this.remappingEase));
        ActiveFont.Draw(Dialog.Get("KEY_CONFIG_CHANGING"), vector2 + new Vector2(0.0f, -8f), new Vector2(0.5f, 1f), Vector2.One * 0.7f, Color.LightGray * Ease.CubeIn(this.remappingEase));
        ActiveFont.Draw(this.remappingText, vector2 + new Vector2(0.0f, 8f), new Vector2(0.5f, 0.0f), Vector2.One * 2f, Color.White * Ease.CubeIn(this.remappingEase));
      }
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
