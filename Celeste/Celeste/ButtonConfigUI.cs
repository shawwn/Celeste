// Decompiled with JetBrains decompiler
// Type: Celeste.ButtonConfigUI
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
  public class ButtonConfigUI : TextMenu
  {
    private bool remapping;
    private float remappingEase;
    private float inputDelay;
    private float timeout;
    private ButtonConfigUI.Mappings remappingKey;
    private bool closing;
    private float closingDelay;
    private bool waitingForController;
    private List<Buttons> all;

    public ButtonConfigUI()
    {
      List<Buttons> buttonsList = new List<Buttons>();
      buttonsList.Add((Buttons) 4096);
      buttonsList.Add((Buttons) 8192);
      buttonsList.Add((Buttons) 16384);
      buttonsList.Add((Buttons) 32768);
      buttonsList.Add((Buttons) 256);
      buttonsList.Add((Buttons) 512);
      buttonsList.Add((Buttons) 8388608);
      buttonsList.Add((Buttons) 4194304);
      this.all = buttonsList;
      // ISSUE: explicit constructor call
      base.\u002Ector();
      this.Reload(-1);
      this.OnESC = this.OnCancel = (Action) (() =>
      {
        this.Focused = false;
        this.closing = true;
        if (Settings.Instance.BtnJump.Count <= 0)
          this.ForceRemap(ButtonConfigUI.Mappings.Jump);
        if (Settings.Instance.BtnDash.Count <= 0)
          this.ForceRemap(ButtonConfigUI.Mappings.Dash);
        if (Settings.Instance.BtnGrab.Count <= 0)
          this.ForceRemap(ButtonConfigUI.Mappings.Grab);
        if (Settings.Instance.BtnTalk.Count > 0)
          return;
        this.ForceRemap(ButtonConfigUI.Mappings.Talk);
      });
      this.MinWidth = 600f;
      this.Position.Y = (__Null) (double) this.ScrollTargetY;
      this.Alpha = 0.0f;
    }

    public void Reload(int index = -1)
    {
      this.Clear();
      this.Add((TextMenu.Item) new TextMenu.Header(Dialog.Clean("BTN_CONFIG_TITLE", (Language) null)));
      this.Add((TextMenu.Item) new ButtonConfigUI.Info());
      this.Add(new TextMenu.Setting(this.Label(ButtonConfigUI.Mappings.Jump), Settings.Instance.BtnJump).Pressed((Action) (() => this.Remap(ButtonConfigUI.Mappings.Jump))).AltPressed((Action) (() => this.ClearRemap(ButtonConfigUI.Mappings.Jump))));
      this.Add(new TextMenu.Setting(this.Label(ButtonConfigUI.Mappings.Dash), Settings.Instance.BtnDash).Pressed((Action) (() => this.Remap(ButtonConfigUI.Mappings.Dash))).AltPressed((Action) (() => this.ClearRemap(ButtonConfigUI.Mappings.Dash))));
      this.Add(new TextMenu.Setting(this.Label(ButtonConfigUI.Mappings.Grab), Settings.Instance.BtnGrab).Pressed((Action) (() => this.Remap(ButtonConfigUI.Mappings.Grab))).AltPressed((Action) (() => this.ClearRemap(ButtonConfigUI.Mappings.Grab))));
      this.Add(new TextMenu.Setting(this.Label(ButtonConfigUI.Mappings.Talk), Settings.Instance.BtnTalk).Pressed((Action) (() => this.Remap(ButtonConfigUI.Mappings.Talk))).AltPressed((Action) (() => this.ClearRemap(ButtonConfigUI.Mappings.Talk))));
      this.Add(new TextMenu.Setting(this.Label(ButtonConfigUI.Mappings.QuickRestart), Settings.Instance.BtnAltQuickRestart).Pressed((Action) (() => this.Remap(ButtonConfigUI.Mappings.QuickRestart))).AltPressed((Action) (() => this.ClearRemap(ButtonConfigUI.Mappings.QuickRestart))));
      this.Add((TextMenu.Item) new TextMenu.SubHeader(""));
      TextMenu.Button button = new TextMenu.Button(Dialog.Clean("KEY_CONFIG_RESET", (Language) null));
      button.IncludeWidthInMeasurement = false;
      button.AlwaysCenter = true;
      button.OnPressed = (Action) (() =>
      {
        Settings.Instance.SetDefaultButtonControls(true);
        Celeste.Input.Initialize();
        this.Reload(this.Selection);
      });
      this.Add((TextMenu.Item) button);
      if (index < 0)
        return;
      this.Selection = index;
    }

    private void Remap(ButtonConfigUI.Mappings mapping)
    {
      if (!Celeste.Input.GuiInputController())
        return;
      this.remapping = true;
      this.remappingKey = mapping;
      this.timeout = 5f;
      this.Focused = false;
    }

    private void ClearRemap(ButtonConfigUI.Mappings mapping)
    {
      switch (mapping)
      {
        case ButtonConfigUI.Mappings.Jump:
          Settings.Instance.BtnJump.Clear();
          break;
        case ButtonConfigUI.Mappings.Dash:
          Settings.Instance.BtnDash.Clear();
          break;
        case ButtonConfigUI.Mappings.Grab:
          Settings.Instance.BtnGrab.Clear();
          break;
        case ButtonConfigUI.Mappings.Talk:
          Settings.Instance.BtnTalk.Clear();
          break;
        case ButtonConfigUI.Mappings.QuickRestart:
          Settings.Instance.BtnAltQuickRestart.Clear();
          break;
      }
      Celeste.Input.Initialize();
      this.Reload(this.Selection);
    }

    private void AddRemap(Buttons btn)
    {
      this.remapping = false;
      this.inputDelay = 0.25f;
      List<Buttons> buttonsList = (List<Buttons>) null;
      switch (this.remappingKey)
      {
        case ButtonConfigUI.Mappings.Jump:
          buttonsList = Settings.Instance.BtnJump;
          break;
        case ButtonConfigUI.Mappings.Dash:
          buttonsList = Settings.Instance.BtnDash;
          break;
        case ButtonConfigUI.Mappings.Grab:
          buttonsList = Settings.Instance.BtnGrab;
          break;
        case ButtonConfigUI.Mappings.Talk:
          buttonsList = Settings.Instance.BtnTalk;
          break;
        case ButtonConfigUI.Mappings.QuickRestart:
          buttonsList = Settings.Instance.BtnAltQuickRestart;
          break;
      }
      if (!buttonsList.Contains(btn))
      {
        if (buttonsList.Count >= 4)
          buttonsList.RemoveAt(0);
        buttonsList.Add(btn);
        if (buttonsList != Settings.Instance.BtnJump)
          Settings.Instance.BtnJump.Remove(btn);
        if (buttonsList != Settings.Instance.BtnDash && buttonsList != Settings.Instance.BtnTalk)
          Settings.Instance.BtnDash.Remove(btn);
        if (buttonsList != Settings.Instance.BtnGrab)
          Settings.Instance.BtnGrab.Remove(btn);
        if (buttonsList != Settings.Instance.BtnTalk && buttonsList != Settings.Instance.BtnDash)
          Settings.Instance.BtnTalk.Remove(btn);
        if (buttonsList != Settings.Instance.BtnAltQuickRestart)
          Settings.Instance.BtnAltQuickRestart.Remove(btn);
      }
      Celeste.Input.Initialize();
      this.Reload(this.Selection);
    }

    private void ForceRemap(ButtonConfigUI.Mappings mapping)
    {
      List<Buttons> buttonsList = new List<Buttons>();
      buttonsList.AddRange((IEnumerable<Buttons>) this.all);
      using (List<Buttons>.Enumerator enumerator = Settings.Instance.BtnJump.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Buttons current = enumerator.Current;
          buttonsList.Remove(current);
        }
      }
      using (List<Buttons>.Enumerator enumerator = Settings.Instance.BtnDash.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Buttons current = enumerator.Current;
          buttonsList.Remove(current);
        }
      }
      using (List<Buttons>.Enumerator enumerator = Settings.Instance.BtnGrab.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Buttons current = enumerator.Current;
          buttonsList.Remove(current);
        }
      }
      using (List<Buttons>.Enumerator enumerator = Settings.Instance.BtnTalk.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Buttons current = enumerator.Current;
          buttonsList.Remove(current);
        }
      }
      using (List<Buttons>.Enumerator enumerator = Settings.Instance.BtnAltQuickRestart.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Buttons current = enumerator.Current;
          buttonsList.Remove(current);
        }
      }
      this.remappingKey = mapping;
      if (buttonsList.Count > 0)
      {
        this.AddRemap(buttonsList[0]);
      }
      else
      {
        Buttons button;
        if (this.TrySteal(Settings.Instance.BtnJump, out button))
          this.AddRemap(button);
        else if (this.TrySteal(Settings.Instance.BtnDash, out button))
          this.AddRemap(button);
        else if (this.TrySteal(Settings.Instance.BtnGrab, out button))
          this.AddRemap(button);
        else if (this.TrySteal(Settings.Instance.BtnTalk, out button))
          this.AddRemap(button);
        else if (this.TrySteal(Settings.Instance.BtnAltQuickRestart, out button))
          this.AddRemap(button);
      }
      this.closingDelay = 0.5f;
    }

    private bool TrySteal(List<Buttons> list, out Buttons button)
    {
      if (list.Count > 1)
      {
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref button = (int) list[0];
        list.RemoveAt(0);
        return true;
      }
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(int&) ref button = 4096;
      return false;
    }

    private string Label(ButtonConfigUI.Mappings mapping)
    {
      return Dialog.Clean("KEY_CONFIG_" + mapping.ToString(), (Language) null);
    }

    public override void Update()
    {
      base.Update();
      this.Focused = !this.closing && (double) this.inputDelay <= 0.0 && !this.waitingForController && !this.remapping;
      if (!this.closing)
      {
        if (!Celeste.Input.GuiInputController())
          this.waitingForController = true;
        else if (this.waitingForController)
        {
          this.Reload(-1);
          this.waitingForController = false;
        }
        if (Celeste.Input.MenuCancel.Pressed && !this.remapping)
          this.OnCancel();
      }
      if ((double) this.inputDelay > 0.0 && !this.remapping)
        this.inputDelay -= Engine.DeltaTime;
      this.remappingEase = Calc.Approach(this.remappingEase, this.remapping ? 1f : 0.0f, Engine.DeltaTime * 4f);
      if ((double) this.remappingEase >= 1.0 && this.remapping)
      {
        if (Celeste.Input.ESC.Pressed || (double) this.timeout <= 0.0 || !Celeste.Input.GuiInputController())
        {
          this.remapping = false;
          this.Focused = true;
        }
        else
        {
          GamePadState currentState = MInput.GamePads[Celeste.Input.Gamepad].CurrentState;
          GamePadState previousState = MInput.GamePads[Celeste.Input.Gamepad].PreviousState;
          using (List<Buttons>.Enumerator enumerator = this.all.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Buttons current = enumerator.Current;
              if (((GamePadState) ref currentState).IsButtonDown(current) && !((GamePadState) ref previousState).IsButtonDown(current))
              {
                this.AddRemap(current);
                break;
              }
            }
          }
        }
        this.timeout -= Engine.DeltaTime;
      }
      this.closingDelay -= Engine.DeltaTime;
      this.Alpha = Calc.Approach(this.Alpha, !this.closing || (double) this.closingDelay > 0.0 ? 1f : 0.0f, Engine.DeltaTime * 8f);
      if (!this.closing || (double) this.Alpha > 0.0)
        return;
      this.Close();
    }

    public override void Render()
    {
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.op_Multiply(Color.get_Black(), Ease.CubeOut(this.Alpha)));
      Vector2 position = Vector2.op_Multiply(new Vector2(1920f, 1080f), 0.5f);
      if (Celeste.Input.GuiInputController())
      {
        base.Render();
        if ((double) this.remappingEase <= 0.0)
          return;
        Draw.Rect(-10f, -10f, 1940f, 1100f, Color.op_Multiply(Color.op_Multiply(Color.get_Black(), 0.95f), Ease.CubeInOut(this.remappingEase)));
        ActiveFont.Draw(Dialog.Get("BTN_CONFIG_CHANGING", (Language) null), Vector2.op_Addition(position, new Vector2(0.0f, -8f)), new Vector2(0.5f, 1f), Vector2.op_Multiply(Vector2.get_One(), 0.7f), Color.op_Multiply(Color.get_LightGray(), Ease.CubeIn(this.remappingEase)));
        ActiveFont.Draw(this.Label(this.remappingKey), Vector2.op_Addition(position, new Vector2(0.0f, 8f)), new Vector2(0.5f, 0.0f), Vector2.op_Multiply(Vector2.get_One(), 2f), Color.op_Multiply(Color.get_White(), Ease.CubeIn(this.remappingEase)));
      }
      else
        ActiveFont.Draw(Dialog.Clean("BTN_CONFIG_NOCONTROLLER", (Language) null), position, new Vector2(0.5f, 0.5f), Vector2.get_One(), Color.op_Multiply(Color.get_White(), Ease.CubeOut(this.Alpha)));
    }

    private enum Mappings
    {
      Jump,
      Dash,
      Grab,
      Talk,
      QuickRestart,
    }

    public class Info : TextMenu.Item
    {
      private List<object> info = new List<object>();

      public Info()
      {
        string[] strArray = Dialog.Clean("BTN_CONFIG_INFO", (Language) null).Split('|');
        if (strArray.Length != 3)
          return;
        this.info.Add((object) strArray[0]);
        this.info.Add((object) Celeste.Input.MenuConfirm);
        this.info.Add((object) strArray[1]);
        this.info.Add((object) Celeste.Input.MenuJournal);
        this.info.Add((object) strArray[2]);
      }

      public override float LeftWidth()
      {
        return 100f;
      }

      public override float Height()
      {
        return ActiveFont.LineHeight * 2f;
      }

      public override void Render(Vector2 position, bool highlighted)
      {
        Color color1 = Color.op_Multiply(Color.get_Gray(), Ease.CubeOut(this.Container.Alpha));
        Color strokeColor = Color.op_Multiply(Color.get_Black(), Ease.CubeOut(this.Container.Alpha));
        Color color2 = Color.op_Multiply(Color.get_White(), Ease.CubeOut(this.Container.Alpha));
        float num = 0.0f;
        for (int index = 0; index < this.info.Count; ++index)
        {
          if (this.info[index] is string)
          {
            string text = this.info[index] as string;
            num += (float) (ActiveFont.Measure(text).X * 0.600000023841858);
          }
          else if (this.info[index] is VirtualButton)
          {
            MTexture mtexture = Celeste.Input.GuiButton(this.info[index] as VirtualButton, "controls/keyboard/oemquestion");
            num += (float) mtexture.Width * 0.6f;
          }
        }
        Vector2 position1 = Vector2.op_Addition(position, Vector2.op_Division(new Vector2(this.Container.Width - num, 0.0f), 2f));
        for (int index = 0; index < this.info.Count; ++index)
        {
          if (this.info[index] is string)
          {
            string text = this.info[index] as string;
            ActiveFont.DrawOutline(text, position1, new Vector2(0.0f, 0.5f), Vector2.op_Multiply(Vector2.get_One(), 0.6f), color1, 2f, strokeColor);
            ref __Null local = ref position1.X;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local = ^(float&) ref local + (float) (ActiveFont.Measure(text).X * 0.600000023841858);
          }
          else if (this.info[index] is VirtualButton)
          {
            MTexture mtexture = Celeste.Input.GuiButton(this.info[index] as VirtualButton, "controls/keyboard/oemquestion");
            mtexture.DrawJustified(position1, new Vector2(0.0f, 0.5f), color2, 0.6f);
            ref __Null local = ref position1.X;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local = ^(float&) ref local + (float) mtexture.Width * 0.6f;
          }
        }
      }
    }
  }
}
