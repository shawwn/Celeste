// Decompiled with JetBrains decompiler
// Type: Celeste.QuickResetHint
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  public class QuickResetHint : Entity
  {
    private string textStart;
    private string textHold;
    private string textPress;
    private List<object> controllerList;
    private List<object> keyboardList;

    public QuickResetHint()
    {
      this.Tag = (int) Tags.HUD;
      Buttons buttons1 = Buttons.LeftShoulder;
      Buttons buttons2 = Buttons.RightShoulder;
      this.textStart = Dialog.Clean("UI_QUICK_RESTART_TITLE") + " ";
      this.textHold = Dialog.Clean("UI_QUICK_RESTART_HOLD");
      this.textPress = Dialog.Clean("UI_QUICK_RESTART_PRESS");
      if (Settings.Instance.Language == "japanese")
      {
        this.controllerList = new List<object>()
        {
          (object) this.textStart,
          (object) buttons1,
          (object) buttons2,
          (object) this.textHold,
          (object) "、",
          (object) Input.FirstButton(Input.Pause),
          (object) this.textPress
        };
        this.keyboardList = new List<object>()
        {
          (object) this.textStart,
          (object) Input.FirstKey(Input.QuickRestart),
          (object) this.textPress
        };
      }
      else
      {
        this.controllerList = new List<object>()
        {
          (object) this.textStart,
          (object) this.textHold,
          (object) buttons1,
          (object) buttons2,
          (object) ",  ",
          (object) this.textPress,
          (object) Input.FirstButton(Input.Pause)
        };
        this.keyboardList = new List<object>()
        {
          (object) this.textStart,
          (object) this.textPress,
          (object) Input.FirstKey(Input.QuickRestart)
        };
      }
    }

    public override void Render()
    {
      List<object> objectList = Input.GuiInputController() ? this.controllerList : this.keyboardList;
      float num = 0.0f;
      foreach (object text in objectList)
      {
        switch (text)
        {
          case string _:
            num += ActiveFont.Measure(text as string).X;
            continue;
          case Buttons button:
            num += (float) Input.GuiSingleButton(button).Width + 16f;
            continue;
          case Keys key:
            num += (float) Input.GuiKey(key).Width + 16f;
            continue;
          default:
            continue;
        }
      }
      Vector2 position = new Vector2((float) ((1920.0 - (double) (num * 0.75f)) / 2.0), 980f);
      foreach (object text in objectList)
      {
        switch (text)
        {
          case string _:
            ActiveFont.DrawOutline(text as string, position, new Vector2(0.0f, 0.5f), Vector2.One * 0.75f, Color.LightGray, 2f, Color.Black);
            position.X += ActiveFont.Measure(text as string).X * 0.75f;
            continue;
          case Buttons button:
            MTexture mtexture1 = Input.GuiSingleButton(button);
            mtexture1.DrawJustified(position + new Vector2((float) (((double) mtexture1.Width + 16.0) * 0.75 * 0.5), 0.0f), new Vector2(0.5f, 0.5f), Color.White, 0.75f);
            position.X += (float) (((double) mtexture1.Width + 16.0) * 0.75);
            continue;
          case Keys key:
            MTexture mtexture2 = Input.GuiKey(key);
            mtexture2.DrawJustified(position + new Vector2((float) (((double) mtexture2.Width + 16.0) * 0.75 * 0.5), 0.0f), new Vector2(0.5f, 0.5f), Color.White, 0.75f);
            position.X += (float) (((double) mtexture2.Width + 16.0) * 0.75);
            continue;
          default:
            continue;
        }
      }
    }
  }
}
