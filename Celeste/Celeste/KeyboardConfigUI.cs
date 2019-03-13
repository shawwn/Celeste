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
    private bool remapping;
    private float remappingEase;
    private float inputDelay;
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
      this.Position.Y = (__Null) (double) this.ScrollTargetY;
      this.Alpha = 0.0f;
    }

    public void Reload(int index = -1)
    {
      this.Clear();
      List<Keys> keysList1 = new List<Keys>();
      keysList1.Add((Keys) 27);
      List<Keys> keys1 = keysList1;
      keys1.AddRange((IEnumerable<Keys>) Settings.Instance.Pause);
      List<Keys> keys2 = new List<Keys>();
      keys2.AddRange((IEnumerable<Keys>) Settings.Instance.Confirm);
      if (!keys2.Contains((Keys) 13))
        keys2.Add((Keys) 13);
      List<Keys> keys3 = new List<Keys>();
      keys3.AddRange((IEnumerable<Keys>) Settings.Instance.Cancel);
      if (!keys3.Contains((Keys) 8))
        keys3.Add((Keys) 8);
      List<Keys> keysList2 = new List<Keys>();
      keysList2.Add(Settings.Instance.Left);
      List<Keys> keys4 = keysList2;
      if (Settings.Instance.Left != 37)
        keys4.Add((Keys) 37);
      List<Keys> keysList3 = new List<Keys>();
      keysList3.Add(Settings.Instance.Right);
      List<Keys> keys5 = keysList3;
      if (Settings.Instance.Right != 39)
        keys5.Add((Keys) 39);
      List<Keys> keysList4 = new List<Keys>();
      keysList4.Add(Settings.Instance.Up);
      List<Keys> keys6 = keysList4;
      if (Settings.Instance.Up != 38)
        keys6.Add((Keys) 38);
      List<Keys> keysList5 = new List<Keys>();
      keysList5.Add(Settings.Instance.Down);
      List<Keys> keys7 = keysList5;
      if (Settings.Instance.Down != 40)
        keys7.Add((Keys) 40);
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
        Celeste.Input.Initialize();
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
      if ((key == null || key == 37 && this.remappingKey != KeyboardConfigUI.Mappings.Left || (key == 39 && this.remappingKey != KeyboardConfigUI.Mappings.Right || key == 38 && this.remappingKey != KeyboardConfigUI.Mappings.Up) || (key == 40 && this.remappingKey != KeyboardConfigUI.Mappings.Down || key == 13 && this.remappingKey != KeyboardConfigUI.Mappings.Confirm) ? 0 : (key != 8 ? 1 : (this.remappingKey == KeyboardConfigUI.Mappings.Cancel ? 1 : 0))) == 0)
        return;
      switch (this.remappingKey)
      {
        case KeyboardConfigUI.Mappings.Left:
          Settings.Instance.Left = key != 37 ? key : (Keys) 0;
          break;
        case KeyboardConfigUI.Mappings.Right:
          Settings.Instance.Right = key != 39 ? key : (Keys) 0;
          break;
        case KeyboardConfigUI.Mappings.Up:
          Settings.Instance.Up = key != 38 ? key : (Keys) 0;
          break;
        case KeyboardConfigUI.Mappings.Down:
          Settings.Instance.Down = key != 40 ? key : (Keys) 0;
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
            if (key != 13)
            {
              Settings.Instance.Confirm.Add(key);
              break;
            }
            break;
          }
          break;
        case KeyboardConfigUI.Mappings.Cancel:
          if (!Settings.Instance.Confirm.Contains(key) && !Settings.Instance.Pause.Contains(key))
          {
            Settings.Instance.Cancel.Clear();
            if (key != 8)
            {
              Settings.Instance.Cancel.Add(key);
              break;
            }
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
      Celeste.Input.Initialize();
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
        if (Celeste.Input.ESC.Pressed || (double) this.timeout <= 0.0)
        {
          this.remapping = false;
          this.Focused = true;
        }
        else
        {
          Keys[] pressedKeys = ((KeyboardState) ref MInput.Keyboard.CurrentState).GetPressedKeys();
          if (pressedKeys != null && pressedKeys.Length != 0 && MInput.Keyboard.Pressed((Keys) (int) pressedKeys[pressedKeys.Length - 1]))
            this.SetRemap((Keys) (int) pressedKeys[pressedKeys.Length - 1]);
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
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.op_Multiply(Color.get_Black(), Ease.CubeOut(this.Alpha)));
      base.Render();
      if ((double) this.remappingEase <= 0.0)
        return;
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.op_Multiply(Color.op_Multiply(Color.get_Black(), 0.95f), Ease.CubeInOut(this.remappingEase)));
      Vector2 vector2 = Vector2.op_Multiply(new Vector2(1920f, 1080f), 0.5f);
      ActiveFont.Draw(Dialog.Get("KEY_CONFIG_CHANGING", (Language) null), Vector2.op_Addition(vector2, new Vector2(0.0f, -8f)), new Vector2(0.5f, 1f), Vector2.op_Multiply(Vector2.get_One(), 0.7f), Color.op_Multiply(Color.get_LightGray(), Ease.CubeIn(this.remappingEase)));
      ActiveFont.Draw(this.Label(this.remappingKey), Vector2.op_Addition(vector2, new Vector2(0.0f, 8f)), new Vector2(0.5f, 0.0f), Vector2.op_Multiply(Vector2.get_One(), 2f), Color.op_Multiply(Color.get_White(), Ease.CubeIn(this.remappingEase)));
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
