// Decompiled with JetBrains decompiler
// Type: Celeste.QuickResetHint
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

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
      this.textStart = Dialog.Clean("UI_QUICK_RESTART_TITLE", (Language) null) + " ";
      this.textHold = Dialog.Clean("UI_QUICK_RESTART_HOLD", (Language) null);
      this.textPress = Dialog.Clean("UI_QUICK_RESTART_PRESS", (Language) null);
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

    public override void Render()
    {
      List<object> objectList = Input.GuiInputController() ? this.controllerList : this.keyboardList;
      float num = 0.0f;
      foreach (object obj in objectList)
      {
        if (obj is string)
          num += ActiveFont.Measure(obj as string).X;
        else if (obj is Buttons)
          num += (float) Input.GuiSingleButton((Buttons) obj, "controls/keyboard/oemquestion").Width + 16f;
        else if (obj is Keys)
          num += (float) Input.GuiKey((Keys) obj, "controls/keyboard/oemquestion").Width + 16f;
      }
      Vector2 position = new Vector2((float) ((1920.0 - (double) (num * 0.75f)) / 2.0), 980f);
      foreach (object obj in objectList)
      {
        if (obj is string)
        {
          ActiveFont.DrawOutline(obj as string, position, new Vector2(0.0f, 0.5f), Vector2.One * 0.75f, Color.LightGray, 2f, Color.Black);
          position.X += ActiveFont.Measure(obj as string).X * 0.75f;
        }
        else if (obj is Buttons)
        {
          MTexture mtexture = Input.GuiSingleButton((Buttons) obj, "controls/keyboard/oemquestion");
          mtexture.DrawJustified(position + new Vector2((float) (((double) mtexture.Width + 16.0) * 0.75 * 0.5), 0.0f), new Vector2(0.5f, 0.5f), Color.White, 0.75f);
          position.X += (float) (((double) mtexture.Width + 16.0) * 0.75);
        }
        else if (obj is Keys)
        {
          MTexture mtexture = Input.GuiKey((Keys) obj, "controls/keyboard/oemquestion");
          mtexture.DrawJustified(position + new Vector2((float) (((double) mtexture.Width + 16.0) * 0.75 * 0.5), 0.0f), new Vector2(0.5f, 0.5f), Color.White, 0.75f);
          position.X += (float) (((double) mtexture.Width + 16.0) * 0.75);
        }
      }
    }
  }
}

