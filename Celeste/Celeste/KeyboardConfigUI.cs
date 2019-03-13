// Decompiled with JetBrains decompiler
// Type: Celeste.KeyboardConfigUI
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
  public class KeyboardConfigUI : TextMenu
  {
    private float remappingEase = 0.0f;
    private float inputDelay = 0.0f;
    private bool remapping;
    private float timeout;
    private KeyboardConfigUI.Mappings remappingKey;
    private bool closing;

    public KeyboardConfigUI()
    {
      this.Reload(-1);
      this.OnESC = this.OnCancel = (Action) (() =>
      {
        this.Focused = false;
        this.closing = true;
      });
      this.MinWidth = 600f;
      this.Position.Y = this.ScrollTargetY;
      this.Alpha = 0.0f;
    }

    public void Reload(int index = -1)
    {
      this.Clear();
      List<Keys> keys1 = new List<Keys>()
      {
        Keys.Escape
      };
      keys1.AddRange((IEnumerable<Keys>) Settings.Instance.Pause);
      List<Keys> keys2 = new List<Keys>();
      keys2.AddRange((IEnumerable<Keys>) Settings.Instance.Confirm);
      if (!keys2.Contains(Keys.Enter))
        keys2.Add(Keys.Enter);
      List<Keys> keys3 = new List<Keys>();
      keys3.AddRange((IEnumerable<Keys>) Settings.Instance.Cancel);
      if (!keys3.Contains(Keys.Back))
        keys3.Add(Keys.Back);
      List<Keys> keys4 = new List<Keys>()
      {
        Settings.Instance.Left
      };
      if (Settings.Instance.Left != Keys.Left)
        keys4.Add(Keys.Left);
      List<Keys> keys5 = new List<Keys>()
      {
        Settings.Instance.Right
      };
      if (Settings.Instance.Right != Keys.Right)
        keys5.Add(Keys.Right);
      List<Keys> keys6 = new List<Keys>()
      {
        Settings.Instance.Up
      };
      if (Settings.Instance.Up != Keys.Up)
        keys6.Add(Keys.Up);
      List<Keys> keys7 = new List<Keys>()
      {
        Settings.Instance.Down
      };
      if (Settings.Instance.Down != Keys.Down)
        keys7.Add(Keys.Down);
      this.Add((TextMenu.Item) new TextMenu.Header(Dialog.Clean("KEY_CONFIG_TITLE", (Language) null)));
      this.Add((TextMenu.Item) new TextMenu.SubHeader(Dialog.Clean("KEY_CONFIG_MOVEMENT", (Language) null)));
      this.Add(new TextMenu.Setting(this.Label(KeyboardConfigUI.Mappings.Left), keys4).Pressed((Action) (() => this.Remap(KeyboardConfigUI.Mappings.Left))));
      this.Add(new TextMenu.Setting(this.Label(KeyboardConfigUI.Mappings.Right), keys5).Pressed((Action) (() => this.Remap(KeyboardConfigUI.Mappings.Right))));
      this.Add(new TextMenu.Setting(this.Label(KeyboardConfigUI.Mappings.Up), keys6).Pressed((Action) (() => this.Remap(KeyboardConfigUI.Mappings.Up))));
      this.Add(new TextMenu.Setting(this.Label(KeyboardConfigUI.Mappings.Down), keys7).Pressed((Action) (() => this.Remap(KeyboardConfigUI.Mappings.Down))));
      this.Add((TextMenu.Item) new TextMenu.SubHeader(Dialog.Clean("KEY_CONFIG_GAMEPLAY", (Language) null)));
      this.Add(new TextMenu.Setting(this.Label(KeyboardConfigUI.Mappings.Jump), Settings.Instance.Jump).Pressed((Action) (() => this.Remap(KeyboardConfigUI.Mappings.Jump))));
      this.Add(new TextMenu.Setting(this.Label(KeyboardConfigUI.Mappings.Dash), Settings.Instance.Dash).Pressed((Action) (() => this.Remap(KeyboardConfigUI.Mappings.Dash))));
      this.Add(new TextMenu.Setting(this.Label(KeyboardConfigUI.Mappings.Grab), Settings.Instance.Grab).Pressed((Action) (() => this.Remap(KeyboardConfigUI.Mappings.Grab))));
      this.Add(new TextMenu.Setting(this.Label(KeyboardConfigUI.Mappings.Talk), Settings.Instance.Talk).Pressed((Action) (() => this.Remap(KeyboardConfigUI.Mappings.Talk))));
      this.Add((TextMenu.Item) new TextMenu.SubHeader(Dialog.Clean("KEY_CONFIG_MENUS", (Language) null)));
      this.Add(new TextMenu.Setting(this.Label(KeyboardConfigUI.Mappings.Confirm), keys2).Pressed((Action) (() => this.Remap(KeyboardConfigUI.Mappings.Confirm))));
      this.Add(new TextMenu.Setting(this.Label(KeyboardConfigUI.Mappings.Cancel), keys3).Pressed((Action) (() => this.Remap(KeyboardConfigUI.Mappings.Cancel))));
      this.Add(new TextMenu.Setting(this.Label(KeyboardConfigUI.Mappings.Pause), keys1).Pressed((Action) (() => this.Remap(KeyboardConfigUI.Mappings.Pause))));
      this.Add(new TextMenu.Setting(this.Label(KeyboardConfigUI.Mappings.Journal), Settings.Instance.Journal).Pressed((Action) (() => this.Remap(KeyboardConfigUI.Mappings.Journal))));
      this.Add(new TextMenu.Setting(this.Label(KeyboardConfigUI.Mappings.QuickRestart), Settings.Instance.QuickRestart).Pressed((Action) (() => this.Remap(KeyboardConfigUI.Mappings.QuickRestart))));
      this.Add((TextMenu.Item) new TextMenu.SubHeader(""));
      TextMenu.Button button = new TextMenu.Button(Dialog.Clean("KEY_CONFIG_RESET", (Language) null));
      button.IncludeWidthInMeasurement = false;
      button.AlwaysCenter = true;
      button.OnPressed = (Action) (() =>
      {
        Settings.Instance.SetDefaultKeyboardControls(true);
        Input.Initialize();
        this.Reload(this.Selection);
      });
      this.Add((TextMenu.Item) button);
      if (index < 0)
        return;
      this.Selection = index;
    }

    private void Remap(KeyboardConfigUI.Mappings mapping)
    {
      this.remapping = true;
      this.remappingKey = mapping;
      this.timeout = 5f;
      this.Focused = false;
    }

    private void SetRemap(Keys key)
    {
      this.remapping = false;
      this.inputDelay = 0.25f;
      if (key == Keys.None || key == Keys.Left && this.remappingKey != KeyboardConfigUI.Mappings.Left || (key == Keys.Right && this.remappingKey != KeyboardConfigUI.Mappings.Right || key == Keys.Up && this.remappingKey != KeyboardConfigUI.Mappings.Up) || (key == Keys.Down && this.remappingKey != KeyboardConfigUI.Mappings.Down || key == Keys.Enter && this.remappingKey != KeyboardConfigUI.Mappings.Confirm) || key == Keys.Back && this.remappingKey != KeyboardConfigUI.Mappings.Cancel)
        return;
      switch (this.remappingKey)
      {
        case KeyboardConfigUI.Mappings.Left:
          Settings.Instance.Left = key != Keys.Left ? key : Keys.None;
          break;
        case KeyboardConfigUI.Mappings.Right:
          Settings.Instance.Right = key != Keys.Right ? key : Keys.None;
          break;
        case KeyboardConfigUI.Mappings.Up:
          Settings.Instance.Up = key != Keys.Up ? key : Keys.None;
          break;
        case KeyboardConfigUI.Mappings.Down:
          Settings.Instance.Down = key != Keys.Down ? key : Keys.None;
          break;
        case KeyboardConfigUI.Mappings.Jump:
          Settings.Instance.Jump.Clear();
          Settings.Instance.Jump.Add(key);
          break;
        case KeyboardConfigUI.Mappings.Dash:
          Settings.Instance.Dash.Clear();
          Settings.Instance.Dash.Add(key);
          break;
        case KeyboardConfigUI.Mappings.Grab:
          Settings.Instance.Grab.Clear();
          Settings.Instance.Grab.Add(key);
          break;
        case KeyboardConfigUI.Mappings.Talk:
          Settings.Instance.Talk.Clear();
          Settings.Instance.Talk.Add(key);
          break;
        case KeyboardConfigUI.Mappings.Confirm:
          if (!Settings.Instance.Cancel.Contains(key) && !Settings.Instance.Pause.Contains(key))
          {
            Settings.Instance.Confirm.Clear();
            if (key != Keys.Enter)
              Settings.Instance.Confirm.Add(key);
            break;
          }
          break;
        case KeyboardConfigUI.Mappings.Cancel:
          if (!Settings.Instance.Confirm.Contains(key) && !Settings.Instance.Pause.Contains(key))
          {
            Settings.Instance.Cancel.Clear();
            if (key != Keys.Back)
              Settings.Instance.Cancel.Add(key);
            break;
          }
          break;
        case KeyboardConfigUI.Mappings.Pause:
          if (!Settings.Instance.Confirm.Contains(key) && !Settings.Instance.Cancel.Contains(key))
          {
            Settings.Instance.Pause.Clear();
            Settings.Instance.Pause.Add(key);
            break;
          }
          break;
        case KeyboardConfigUI.Mappings.Journal:
          Settings.Instance.Journal.Clear();
          Settings.Instance.Journal.Add(key);
          break;
        case KeyboardConfigUI.Mappings.QuickRestart:
          Settings.Instance.QuickRestart.Clear();
          Settings.Instance.QuickRestart.Add(key);
          break;
      }
      Input.Initialize();
      this.Reload(this.Selection);
    }

    private string Label(KeyboardConfigUI.Mappings mapping)
    {
      return Dialog.Clean("KEY_CONFIG_" + mapping.ToString(), (Language) null);
    }

    public override void Update()
    {
      base.Update();
      if ((double) this.inputDelay > 0.0 && !this.remapping)
      {
        this.inputDelay -= Engine.DeltaTime;
        if ((double) this.inputDelay <= 0.0)
          this.Focused = true;
      }
      this.remappingEase = Calc.Approach(this.remappingEase, this.remapping ? 1f : 0.0f, Engine.DeltaTime * 4f);
      if ((double) this.remappingEase > 0.5 && this.remapping)
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
            this.SetRemap(pressedKeys[pressedKeys.Length - 1]);
        }
        this.timeout -= Engine.DeltaTime;
      }
      this.Alpha = Calc.Approach(this.Alpha, this.closing ? 0.0f : 1f, Engine.DeltaTime * 8f);
      if (!this.closing || (double) this.Alpha > 0.0)
        return;
      this.Close();
    }

    public override void Render()
    {
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * Ease.CubeOut(this.Alpha));
      base.Render();
      if ((double) this.remappingEase <= 0.0)
        return;
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * 0.95f * Ease.CubeInOut(this.remappingEase));
      Vector2 vector2 = new Vector2(1920f, 1080f) * 0.5f;
      ActiveFont.Draw(Dialog.Get("KEY_CONFIG_CHANGING", (Language) null), vector2 + new Vector2(0.0f, -8f), new Vector2(0.5f, 1f), Vector2.One * 0.7f, Color.LightGray * Ease.CubeIn(this.remappingEase));
      ActiveFont.Draw(this.Label(this.remappingKey), vector2 + new Vector2(0.0f, 8f), new Vector2(0.5f, 0.0f), Vector2.One * 2f, Color.White * Ease.CubeIn(this.remappingEase));
    }

    private enum Mappings
    {
      Left,
      Right,
      Up,
      Down,
      Jump,
      Dash,
      Grab,
      Talk,
      Confirm,
      Cancel,
      Pause,
      Journal,
      QuickRestart,
    }
  }
}

