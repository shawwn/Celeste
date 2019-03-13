// Decompiled with JetBrains decompiler
// Type: Celeste.BirdTutorialGui
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class BirdTutorialGui : Entity
  {
    public float Scale = 0.0f;
    private float buttonPadding = 8f;
    private Color bgColor = Calc.HexToColor("061526");
    private Color lineColor = new Color(1f, 1f, 1f);
    private Color textColor = Calc.HexToColor("6179e2");
    public Entity Entity;
    public bool Open;
    private object info;
    private List<object> controls;
    private float controlsWidth;
    private float infoWidth;
    private float infoHeight;

    public BirdTutorialGui(Entity entity, Vector2 position, object info, params object[] controls)
    {
      this.AddTag((int) Tags.HUD);
      this.Entity = entity;
      this.Position = position;
      this.info = info;
      this.controls = new List<object>((IEnumerable<object>) controls);
      if (info is string)
      {
        this.infoWidth = ActiveFont.Measure((string) info).X;
        this.infoHeight = ActiveFont.LineHeight;
      }
      else if (info is MTexture)
      {
        this.infoWidth = (float) ((MTexture) info).Width;
        this.infoHeight = (float) ((MTexture) info).Height;
      }
      this.controlsWidth = 0.0f;
      foreach (object control in controls)
      {
        if (control is VirtualButton)
          this.controlsWidth += (float) Input.GuiButton((VirtualButton) control, "controls/keyboard/oemquestion").Width + this.buttonPadding * 2f;
        else if (control is Vector2)
          this.controlsWidth += (float) Input.GuiDirection((Vector2) control).Width + this.buttonPadding * 2f;
        else if (control is string)
          this.controlsWidth += ActiveFont.Measure(control.ToString()).X;
        else if (control is MTexture)
          this.controlsWidth += (float) ((MTexture) control).Width;
      }
    }

    public override void Update()
    {
      this.Scale = Calc.Approach(this.Scale, this.Open ? 1f : 0.0f, Engine.RawDeltaTime * 8f);
      base.Update();
    }

    public override void Render()
    {
      Level scene = this.Scene as Level;
      if (scene.FrozenOrPaused || scene.RetryPlayerCorpse != null || (double) this.Scale <= 0.0)
        return;
      Vector2 vector2_1 = this.Entity.Position + this.Position - this.SceneAs<Level>().Camera.Position.Floor();
      if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
        vector2_1.X = 320f - vector2_1.X;
      vector2_1.X *= 6f;
      vector2_1.Y *= 6f;
      float lineHeight = ActiveFont.LineHeight;
      float width1 = (Math.Max(this.controlsWidth, this.infoWidth) + 64f) * this.Scale;
      float height = (float) ((double) this.infoHeight + (double) lineHeight + 32.0);
      float x1 = vector2_1.X - width1 / 2f;
      float y = (float) ((double) vector2_1.Y - (double) height - 32.0);
      Draw.Rect(x1 - 6f, y - 6f, width1 + 12f, height + 12f, this.lineColor);
      Draw.Rect(x1, y, width1, height, this.bgColor);
      for (int index = 0; index <= 36; ++index)
      {
        float width2 = (float) (73 - index * 2) * this.Scale;
        Draw.Rect(vector2_1.X - width2 / 2f, y + height + (float) index, width2, 1f, this.lineColor);
        if ((double) width2 > 12.0)
          Draw.Rect((float) ((double) vector2_1.X - (double) width2 / 2.0 + 6.0), y + height + (float) index, width2 - 12f, 1f, this.bgColor);
      }
      if ((double) width1 <= 3.0)
        return;
      Vector2 position = new Vector2(vector2_1.X, y + 16f);
      if (this.info is string)
        ActiveFont.Draw((string) this.info, position, new Vector2(0.5f, 0.0f), new Vector2(this.Scale, 1f), this.textColor);
      else if (this.info is MTexture)
        ((MTexture) this.info).DrawJustified(position, new Vector2(0.5f, 0.0f), Color.White, new Vector2(this.Scale, 1f));
      position.Y += this.infoHeight + lineHeight * 0.5f;
      Vector2 vector2_2 = new Vector2((float) (-(double) this.controlsWidth / 2.0), 0.0f);
      foreach (object control in this.controls)
      {
        if (control is VirtualButton)
        {
          MTexture mtexture = Input.GuiButton((VirtualButton) control, "controls/keyboard/oemquestion");
          vector2_2.X += this.buttonPadding;
          mtexture.Draw(position, new Vector2(-vector2_2.X, (float) (mtexture.Height / 2)), Color.White, new Vector2(this.Scale, 1f));
          vector2_2.X += (float) mtexture.Width + this.buttonPadding;
        }
        else if (control is Vector2)
        {
          Vector2 direction = (Vector2) control;
          if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
            direction.X = -direction.X;
          MTexture mtexture = Input.GuiDirection(direction);
          vector2_2.X += this.buttonPadding;
          mtexture.Draw(position, new Vector2(-vector2_2.X, (float) (mtexture.Height / 2)), Color.White, new Vector2(this.Scale, 1f));
          vector2_2.X += (float) mtexture.Width + this.buttonPadding;
        }
        else if (control is string)
        {
          string text = control.ToString();
          float x2 = ActiveFont.Measure(text).X;
          ActiveFont.Draw(text, position + new Vector2(1f, 2f), new Vector2(-vector2_2.X / x2, 0.5f), new Vector2(this.Scale, 1f), this.textColor);
          ActiveFont.Draw(text, position + new Vector2(1f, -2f), new Vector2(-vector2_2.X / x2, 0.5f), new Vector2(this.Scale, 1f), Color.White);
          vector2_2.X += x2 + 1f;
        }
        else if (control is MTexture)
        {
          MTexture mtexture = (MTexture) control;
          mtexture.Draw(position, new Vector2(-vector2_2.X, (float) (mtexture.Height / 2)), Color.White, new Vector2(this.Scale, 1f));
          vector2_2.X += (float) mtexture.Width;
        }
      }
    }
  }
}

