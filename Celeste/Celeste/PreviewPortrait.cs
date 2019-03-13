// Decompiled with JetBrains decompiler
// Type: Celeste.PreviewPortrait
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Celeste
{
  public class PreviewPortrait : Scene
  {
    private List<string> options = new List<string>();
    private List<string> animations = new List<string>();
    private Vector2 topleft = new Vector2(64f, 64f);
    private Sprite animation;
    private string currentPortrait;

    public PreviewPortrait(float scroll = 64f)
    {
      foreach (KeyValuePair<string, SpriteData> keyValuePair in GFX.PortraitsSpriteBank.SpriteData)
      {
        if (keyValuePair.Key.StartsWith("portrait"))
          this.options.Add(keyValuePair.Key);
      }
      this.topleft.Y = (__Null) (double) scroll;
    }

    public override void Update()
    {
      if (this.animation != null)
      {
        this.animation.Update();
        if (MInput.Mouse.PressedLeftButton)
        {
          for (int i = 0; i < this.animations.Count; ++i)
          {
            if (this.MouseOverOption(i))
            {
              if (i == 0)
              {
                this.animation = (Sprite) null;
                break;
              }
              this.animation.Play(this.animations[i], false, false);
              break;
            }
          }
        }
      }
      else if (MInput.Mouse.PressedLeftButton)
      {
        for (int i = 0; i < this.options.Count; ++i)
        {
          if (this.MouseOverOption(i))
          {
            this.currentPortrait = this.options[i].Split('_')[1];
            this.animation = GFX.PortraitsSpriteBank.Create(this.options[i]);
            this.animations.Clear();
            this.animations.Add("<-BACK");
            XmlElement xml1 = GFX.PortraitsSpriteBank.SpriteData[this.options[i]].Sources[0].XML;
            foreach (XmlElement xml2 in xml1.GetElementsByTagName("Anim"))
              this.animations.Add(xml2.Attr("id"));
            IEnumerator enumerator = xml1.GetElementsByTagName("Loop").GetEnumerator();
            try
            {
              while (enumerator.MoveNext())
                this.animations.Add(((XmlElement) enumerator.Current).Attr("id"));
              break;
            }
            finally
            {
              (enumerator as IDisposable)?.Dispose();
            }
          }
        }
      }
      ref __Null local = ref this.topleft.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local = ^(float&) ref local + (float) MInput.Mouse.WheelDelta * Engine.DeltaTime * ActiveFont.LineHeight;
      if (!MInput.Keyboard.Pressed((Keys) 112))
        return;
      Celeste.Celeste.ReloadPortraits();
      Engine.Scene = (Scene) new PreviewPortrait((float) this.topleft.Y);
    }

    public Vector2 Mouse
    {
      get
      {
        return Vector2.Transform(new Vector2((float) ((MouseState) ref MInput.Mouse.CurrentState).get_X(), (float) ((MouseState) ref MInput.Mouse.CurrentState).get_Y()), Matrix.Invert(Engine.ScreenMatrix));
      }
    }

    public override void Render()
    {
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) RasterizerState.CullNone, (Effect) null, Engine.ScreenMatrix);
      Draw.Rect(0.0f, 0.0f, 960f, 1080f, Color.op_Multiply(Color.get_DarkSlateGray(), 0.25f));
      if (this.animation != null)
      {
        this.animation.Scale = Vector2.get_One();
        this.animation.Position = new Vector2(1440f, 540f);
        this.animation.Render();
        int i = 0;
        foreach (string animation in this.animations)
        {
          Color color = Color.get_Gray();
          if (this.MouseOverOption(i))
            color = Color.get_White();
          else if (this.animation.CurrentAnimationID == animation)
            color = Color.get_Yellow();
          ActiveFont.Draw(animation, Vector2.op_Addition(this.topleft, new Vector2(0.0f, (float) i * ActiveFont.LineHeight)), color);
          ++i;
        }
        if (!string.IsNullOrEmpty(this.animation.CurrentAnimationID))
        {
          string[] strArray = this.animation.CurrentAnimationID.Split('_');
          if (strArray.Length > 1)
            ActiveFont.Draw(this.currentPortrait + " " + strArray[1], new Vector2(1440f, 1016f), new Vector2(0.5f, 1f), Vector2.get_One(), Color.get_White());
        }
      }
      else
      {
        int i = 0;
        foreach (string option in this.options)
        {
          ActiveFont.Draw(option, Vector2.op_Addition(this.topleft, new Vector2(0.0f, (float) i * ActiveFont.LineHeight)), this.MouseOverOption(i) ? Color.get_White() : Color.get_Gray());
          ++i;
        }
      }
      Draw.Rect((float) (this.Mouse.X - 12.0), (float) (this.Mouse.Y - 4.0), 24f, 8f, Color.get_Red());
      Draw.Rect((float) (this.Mouse.X - 4.0), (float) (this.Mouse.Y - 12.0), 8f, 24f, Color.get_Red());
      Draw.SpriteBatch.End();
    }

    private bool MouseOverOption(int i)
    {
      if (this.Mouse.X > this.topleft.X && this.Mouse.Y > this.topleft.Y + (double) i * (double) ActiveFont.LineHeight && (double) MInput.Mouse.X < 960.0)
        return this.Mouse.Y < this.topleft.Y + (double) (i + 1) * (double) ActiveFont.LineHeight;
      return false;
    }
  }
}
