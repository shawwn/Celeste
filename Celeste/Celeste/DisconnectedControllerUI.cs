// Decompiled with JetBrains decompiler
// Type: Celeste.DisconnectedControllerUI
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  public class DisconnectedControllerUI
  {
    private float fade;
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
      int num = MInput.Disabled ? 1 : 0;
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
      MInput.Disabled = num != 0;
    }

    public void Render()
    {
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, (Effect) null, Engine.ScreenMatrix);
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * this.fade * 0.8f);
      ActiveFont.DrawOutline(Dialog.Clean("XB1_RECONNECT_CONTROLLER"), Celeste.TargetCenter, new Vector2(0.5f, 0.5f), Vector2.One, Color.White * this.fade, 2f, Color.Black * this.fade * this.fade);
      Input.GuiButton(Input.MenuConfirm).DrawCentered(Celeste.TargetCenter + new Vector2(0.0f, 128f), Color.White * this.fade);
      Draw.SpriteBatch.End();
    }

    private static bool IsGamepadConnected() => MInput.GamePads[Input.Gamepad].Attached;

    private static bool RequiresGamepad()
    {
      switch (Engine.Scene)
      {
        case null:
        case GameLoader _:
        case OverworldLoader _:
          return false;
        case Overworld overworld:
          if (overworld.Current is OuiTitleScreen)
            return false;
          break;
      }
      return true;
    }

    public static void CheckGamepadDisconnect()
    {
      if (Celeste.DisconnectUI != null || !DisconnectedControllerUI.RequiresGamepad() || DisconnectedControllerUI.IsGamepadConnected())
        return;
      DisconnectedControllerUI disconnectedControllerUi = new DisconnectedControllerUI();
    }
  }
}
