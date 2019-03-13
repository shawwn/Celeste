// Decompiled with JetBrains decompiler
// Type: Celeste.DisconnectedControllerUI
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  public class DisconnectedControllerUI
  {
    private float fade = 0.0f;
    private bool closing;

    public DisconnectedControllerUI()
    {
      Celeste.DisconnectUI = this;
      Engine.OverloadGameLoop = new Action(this.Update);
    }

    private void OnClose()
    {
      Celeste.DisconnectUI = (DisconnectedControllerUI) null;
      Engine.OverloadGameLoop = (Action) null;
    }

    public void Update()
    {
      bool disabled = MInput.Disabled;
      MInput.Disabled = false;
      this.fade = Calc.Approach(this.fade, this.closing ? 0.0f : 1f, Engine.DeltaTime * 8f);
      if (!this.closing)
      {
        int gamepadIndex = -1;
        if (Input.AnyGamepadConfirmPressed(out gamepadIndex))
        {
          Input.Gamepad = gamepadIndex;
          this.closing = true;
        }
      }
      else if ((double) this.fade <= 0.0)
        this.OnClose();
      MInput.Disabled = disabled;
    }

    public void Render()
    {
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, (Effect) null, Engine.ScreenMatrix);
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * this.fade * 0.8f);
      ActiveFont.DrawOutline(Dialog.Clean("XB1_RECONNECT_CONTROLLER", (Language) null), Celeste.TargetCenter, new Vector2(0.5f, 0.5f), Vector2.One, Color.White * this.fade, 2f, Color.Black * this.fade * this.fade);
      Input.GuiButton(Input.MenuConfirm, "controls/keyboard/oemquestion").DrawCentered(Celeste.TargetCenter + new Vector2(0.0f, 128f), Color.White * this.fade);
      Draw.SpriteBatch.End();
    }

    private static bool IsGamepadConnected()
    {
      return MInput.GamePads[Input.Gamepad].Attached;
    }

    private static bool RequiresGamepad()
    {
      if (Engine.Scene == null || Engine.Scene is GameLoader || Engine.Scene is OverworldLoader)
        return false;
      Overworld scene = Engine.Scene as Overworld;
      return scene == null || !(scene.Current is OuiTitleScreen);
    }

    public static void CheckGamepadDisconnect()
    {
      if (Celeste.DisconnectUI != null || !DisconnectedControllerUI.RequiresGamepad() || DisconnectedControllerUI.IsGamepadConnected())
        return;
      DisconnectedControllerUI disconnectedControllerUi = new DisconnectedControllerUI();
    }
  }
}

