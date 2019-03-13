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
    private float remappingEase = 0.0f;
    private float inputDelay = 0.0f;
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
    private bool remapping;
    private float timeout;
    private ButtonConfigUI.Mappings remappingKey;
    private bool closing;
    private float closingDelay;
    private bool waitingForController;

    public ButtonConfigUI()
    {
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
      this.Position.Y = this.ScrollTargetY;
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
        Input.Initialize();
        this.Reload(this.Selection);
      });
      this.Add((TextMenu.Item) button);
      if (index < 0)
        return;
      this.Selection = index;
    }

    private void Remap(ButtonConfigUI.Mappings mapping)
    {
      if (!Input.GuiInputController())
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
      Input.Initialize();
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
      Input.Initialize();
      this.Reload(this.Selection);
    }

    private void ForceRemap(ButtonConfigUI.Mappings mapping)
    {
      List<Buttons> buttonsList = new List<Buttons>();
      buttonsList.AddRange((IEnumerable<Buttons>) this.all);
      foreach (Buttons buttons in Settings.Instance.BtnJump)
        buttonsList.Remove(buttons);
      foreach (Buttons buttons in Settings.Instance.BtnDash)
        buttonsList.Remove(buttons);
      foreach (Buttons buttons in Settings.Instance.BtnGrab)
        buttonsList.Remove(buttons);
      foreach (Buttons buttons in Settings.Instance.BtnTalk)
        buttonsList.Remove(buttons);
      foreach (Buttons buttons in Settings.Instance.BtnAltQuickRestart)
        buttonsList.Remove(buttons);
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
        button = list[0];
        list.RemoveAt(0);
        return true;
      }
      button = Buttons.A;
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
        if (!Input.GuiInputController())
          this.waitingForController = true;
        else if (this.waitingForController)
        {
          this.Reload(-1);
          this.waitingForController = false;
        }
        if (Input.MenuCancel.Pressed && !this.remapping)
          this.OnCancel();
      }
      if ((double) this.inputDelay > 0.0 && !this.remapping)
        this.inputDelay -= Engine.DeltaTime;
      this.remappingEase = Calc.Approach(this.remappingEase, this.remapping ? 1f : 0.0f, Engine.DeltaTime * 4f);
      if ((double) this.remappingEase >= 1.0 && this.remapping)
      {
        if (Input.ESC.Pressed || (double) this.timeout <= 0.0 || !Input.GuiInputController())
        {
          this.remapping = false;
          this.Focused = true;
        }
        else
        {
          GamePadState currentState = MInput.GamePads[Input.Gamepad].CurrentState;
          GamePadState previousState = MInput.GamePads[Input.Gamepad].PreviousState;
          foreach (Buttons buttons in this.all)
          {
            if (currentState.IsButtonDown(buttons) && !previousState.IsButtonDown(buttons))
            {
              this.AddRemap(buttons);
              break;
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
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * Ease.CubeOut(this.Alpha));
      Vector2 position = new Vector2(1920f, 1080f) * 0.5f;
      if (Input.GuiInputController())
      {
        base.Render();
        if ((double) this.remappingEase <= 0.0)
          return;
        Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * 0.95f * Ease.CubeInOut(this.remappingEase));
        ActiveFont.Draw(Dialog.Get("BTN_CONFIG_CHANGING", (Language) null), position + new Vector2(0.0f, -8f), new Vector2(0.5f, 1f), Vector2.One * 0.7f, Color.LightGray * Ease.CubeIn(this.remappingEase));
        ActiveFont.Draw(this.Label(this.remappingKey), position + new Vector2(0.0f, 8f), new Vector2(0.5f, 0.0f), Vector2.One * 2f, Color.White * Ease.CubeIn(this.remappingEase));
      }
      else
        ActiveFont.Draw(Dialog.Clean("BTN_CONFIG_NOCONTROLLER", (Language) null), position, new Vector2(0.5f, 0.5f), Vector2.One, Color.White * Ease.CubeOut(this.Alpha));
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
        this.info.Add((object) Input.MenuConfirm);
        this.info.Add((object) strArray[1]);
        this.info.Add((object) Input.MenuJournal);
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
        Color color1 = Color.Gray * Ease.CubeOut(this.Container.Alpha);
        Color strokeColor = Color.Black * Ease.CubeOut(this.Container.Alpha);
        Color color2 = Color.White * Ease.CubeOut(this.Container.Alpha);
        float num = 0.0f;
        for (int index = 0; index < this.info.Count; ++index)
        {
          if (this.info[index] is string)
          {
            string text = this.info[index] as string;
            num += ActiveFont.Measure(text).X * 0.6f;
          }
          else if (this.info[index] is VirtualButton)
          {
            MTexture mtexture = Input.GuiButton(this.info[index] as VirtualButton, "controls/keyboard/oemquestion");
            num += (float) mtexture.Width * 0.6f;
          }
        }
        Vector2 position1 = position + new Vector2(this.Container.Width - num, 0.0f) / 2f;
        for (int index = 0; index < this.info.Count; ++index)
        {
          if (this.info[index] is string)
          {
            string text = this.info[index] as string;
            ActiveFont.DrawOutline(text, position1, new Vector2(0.0f, 0.5f), Vector2.One * 0.6f, color1, 2f, strokeColor);
            position1.X += ActiveFont.Measure(text).X * 0.6f;
          }
          else if (this.info[index] is VirtualButton)
          {
            MTexture mtexture = Input.GuiButton(this.info[index] as VirtualButton, "controls/keyboard/oemquestion");
            mtexture.DrawJustified(position1, new Vector2(0.0f, 0.5f), color2, 0.6f);
            position1.X += (float) mtexture.Width * 0.6f;
          }
        }
      }
    }
  }
}

