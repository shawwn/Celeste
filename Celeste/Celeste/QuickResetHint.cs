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
      Buttons buttons1 = (Buttons) 256;
      Buttons buttons2 = (Buttons) 512;
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
        (object) Celeste.Input.FirstButton(Celeste.Input.Pause)
      };
      this.keyboardList = new List<object>()
      {
        (object) this.textStart,
        (object) this.textPress,
        (object) Celeste.Input.FirstKey(Celeste.Input.QuickRestart)
      };
    }

    public override void Render()
    {
      List<object> objectList = Celeste.Input.GuiInputController() ? this.controllerList : this.keyboardList;
      float num1 = 0.0f;
      foreach (object obj in objectList)
      {
        if (obj is string)
          num1 += (float) ActiveFont.Measure(obj as string).X;
        else if (obj is Buttons)
          num1 += (float) Celeste.Input.GuiSingleButton((Buttons) obj, "controls/keyboard/oemquestion").Width + 16f;
        else if (obj is Keys)
          num1 += (float) Celeste.Input.GuiKey((Keys) obj, "controls/keyboard/oemquestion").Width + 16f;
      }
      float num2 = num1 * 0.75f;
      Vector2 position;
      ((Vector2) ref position).\u002Ector((float) ((1920.0 - (double) num2) / 2.0), 980f);
      foreach (object obj in objectList)
      {
        if (obj is string)
        {
          ActiveFont.DrawOutline(obj as string, position, new Vector2(0.0f, 0.5f), Vector2.op_Multiply(Vector2.get_One(), 0.75f), Color.get_LightGray(), 2f, Color.get_Black());
          ref __Null local = ref position.X;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local + (float) (ActiveFont.Measure(obj as string).X * 0.75);
        }
        else if (obj is Buttons)
        {
          MTexture mtexture = Celeste.Input.GuiSingleButton((Buttons) obj, "controls/keyboard/oemquestion");
          mtexture.DrawJustified(Vector2.op_Addition(position, new Vector2((float) (((double) mtexture.Width + 16.0) * 0.75 * 0.5), 0.0f)), new Vector2(0.5f, 0.5f), Color.get_White(), 0.75f);
          ref __Null local = ref position.X;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local + (float) (((double) mtexture.Width + 16.0) * 0.75);
        }
        else if (obj is Keys)
        {
          MTexture mtexture = Celeste.Input.GuiKey((Keys) obj, "controls/keyboard/oemquestion");
          mtexture.DrawJustified(Vector2.op_Addition(position, new Vector2((float) (((double) mtexture.Width + 16.0) * 0.75 * 0.5), 0.0f)), new Vector2(0.5f, 0.5f), Color.get_White(), 0.75f);
          ref __Null local = ref position.X;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local + (float) (((double) mtexture.Width + 16.0) * 0.75);
        }
      }
    }
  }
}
