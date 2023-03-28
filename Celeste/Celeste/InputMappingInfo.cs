// Decompiled with JetBrains decompiler
// Type: Celeste.InputMappingInfo
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  public class InputMappingInfo : TextMenu.Item
  {
    private List<object> info = new List<object>();
    private bool controllerMode;
    private float borderEase;
    private bool fixedPosition;

    public InputMappingInfo(bool controllerMode)
    {
      string[] strArray = Dialog.Clean("BTN_CONFIG_INFO").Split('|');
      if (strArray.Length == 3)
      {
        this.info.Add((object) strArray[0]);
        this.info.Add((object) Input.MenuConfirm);
        this.info.Add((object) strArray[1]);
        this.info.Add((object) Input.MenuJournal);
        this.info.Add((object) strArray[2]);
      }
      this.controllerMode = controllerMode;
      this.AboveAll = true;
    }

    public override float LeftWidth() => 100f;

    public override float Height() => ActiveFont.LineHeight * 2f;

    public override void Update()
    {
      this.borderEase = Calc.Approach(this.borderEase, this.fixedPosition ? 1f : 0.0f, Engine.DeltaTime * 4f);
      base.Update();
    }

    public override void Render(Vector2 position, bool highlighted)
    {
      this.fixedPosition = false;
      if ((double) position.Y < 100.0)
      {
        this.fixedPosition = true;
        position.Y = 100f;
      }
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
          VirtualButton button = this.info[index] as VirtualButton;
          if (this.controllerMode)
          {
            MTexture mtexture = Input.GuiButton(button, Input.PrefixMode.Attached);
            num += (float) mtexture.Width * 0.6f;
          }
          else if (button.Binding.Keyboard.Count > 0)
          {
            MTexture mtexture = Input.GuiKey(button.Binding.Keyboard[0]);
            num += (float) mtexture.Width * 0.6f;
          }
          else
          {
            MTexture mtexture = Input.GuiKey(Keys.None);
            num += (float) mtexture.Width * 0.6f;
          }
        }
      }
      Vector2 position1 = position + new Vector2(this.Container.Width - num, 0.0f) / 2f;
      if ((double) this.borderEase > 0.0)
      {
        Draw.HollowRect(position1.X - 22f, position1.Y - 42f, num + 44f, 84f, Color.White * Ease.CubeOut(this.Container.Alpha) * this.borderEase);
        Draw.HollowRect(position1.X - 21f, position1.Y - 41f, num + 42f, 82f, Color.White * Ease.CubeOut(this.Container.Alpha) * this.borderEase);
        Draw.Rect(position1.X - 20f, position1.Y - 40f, num + 40f, 80f, Color.Black * Ease.CubeOut(this.Container.Alpha));
      }
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
          VirtualButton button = this.info[index] as VirtualButton;
          if (this.controllerMode)
          {
            MTexture mtexture = Input.GuiButton(button, Input.PrefixMode.Attached);
            mtexture.DrawJustified(position1, new Vector2(0.0f, 0.5f), color2, 0.6f);
            position1.X += (float) mtexture.Width * 0.6f;
          }
          else if (button.Binding.Keyboard.Count > 0)
          {
            MTexture mtexture = Input.GuiKey(button.Binding.Keyboard[0]);
            mtexture.DrawJustified(position1, new Vector2(0.0f, 0.5f), color2, 0.6f);
            position1.X += (float) mtexture.Width * 0.6f;
          }
          else
          {
            MTexture mtexture = Input.GuiKey(Keys.None);
            mtexture.DrawJustified(position1, new Vector2(0.0f, 0.5f), color2, 0.6f);
            position1.X += (float) mtexture.Width * 0.6f;
          }
        }
      }
    }
  }
}
