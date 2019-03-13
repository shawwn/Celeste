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
    private Sprite animation = (Sprite) null;
    private List<string> options = new List<string>();
    private List<string> animations = new List<string>();
    private Vector2 topleft = new Vector2(64f, 64f);
    private string currentPortrait;

    public PreviewPortrait(float scroll = 64f)
    {
      foreach (KeyValuePair<string, SpriteData> keyValuePair in GFX.PortraitsSpriteBank.SpriteData)
      {
        if (keyValuePair.Key.StartsWith("portrait"))
          this.options.Add(keyValuePair.Key);
      }
      this.topleft.Y = scroll;
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
      this.topleft.Y += (float) MInput.Mouse.WheelDelta * Engine.DeltaTime * ActiveFont.LineHeight;
      if (!MInput.Keyboard.Pressed(Keys.F1))
        return;
      Celeste.ReloadPortraits();
      Engine.Scene = (Scene) new PreviewPortrait(this.topleft.Y);
    }

    public Vector2 Mouse
    {
      get
      {
        return Vector2.Transform(new Vector2((float) MInput.Mouse.CurrentState.X, (float) MInput.Mouse.CurrentState.Y), Matrix.Invert(Engine.ScreenMatrix));
      }
    }

    public override void Render()
    {
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, (DepthStencilState) null, RasterizerState.CullNone, (Effect) null, Engine.ScreenMatrix);
      Draw.Rect(0.0f, 0.0f, 960f, 1080f, Color.DarkSlateGray * 0.25f);
      if (this.animation != null)
      {
        this.animation.Scale = Vector2.One;
        this.animation.Position = new Vector2(1440f, 540f);
        this.animation.Render();
        int i = 0;
        foreach (string animation in this.animations)
        {
          Color color = Color.Gray;
          if (this.MouseOverOption(i))
            color = Color.White;
          else if (this.animation.CurrentAnimationID == animation)
            color = Color.Yellow;
          ActiveFont.Draw(animation, this.topleft + new Vector2(0.0f, (float) i * ActiveFont.LineHeight), color);
          ++i;
        }
        if (!string.IsNullOrEmpty(this.animation.CurrentAnimationID))
        {
          string[] strArray = this.animation.CurrentAnimationID.Split('_');
          if (strArray.Length > 1)
            ActiveFont.Draw(this.currentPortrait + " " + strArray[1], new Vector2(1440f, 1016f), new Vector2(0.5f, 1f), Vector2.One, Color.White);
        }
      }
      else
      {
        int i = 0;
        foreach (string option in this.options)
        {
          ActiveFont.Draw(option, this.topleft + new Vector2(0.0f, (float) i * ActiveFont.LineHeight), this.MouseOverOption(i) ? Color.White : Color.Gray);
          ++i;
        }
      }
      Draw.Rect(this.Mouse.X - 12f, this.Mouse.Y - 4f, 24f, 8f, Color.Red);
      Draw.Rect(this.Mouse.X - 4f, this.Mouse.Y - 12f, 8f, 24f, Color.Red);
      Draw.SpriteBatch.End();
    }

    private bool MouseOverOption(int i)
    {
      return (double) this.Mouse.X > (double) this.topleft.X && (double) this.Mouse.Y > (double) this.topleft.Y + (double) i * (double) ActiveFont.LineHeight && (double) MInput.Mouse.X < 960.0 && (double) this.Mouse.Y < (double) this.topleft.Y + (double) (i + 1) * (double) ActiveFont.LineHeight;
    }
  }
}

