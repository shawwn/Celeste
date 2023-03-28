// Decompiled with JetBrains decompiler
// Type: Celeste.BirdTutorialGui
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class BirdTutorialGui : Entity
  {
    public Entity Entity;
    public bool Open;
    public float Scale;
    private object info;
    private List<object> controls;
    private float controlsWidth;
    private float infoWidth;
    private float infoHeight;
    private float buttonPadding = 8f;
    private Color bgColor = Calc.HexToColor("061526");
    private Color lineColor = new Color(1f, 1f, 1f);
    private Color textColor = Calc.HexToColor("6179e2");

    public BirdTutorialGui(Entity entity, Vector2 position, object info, params object[] controls)
    {
      this.AddTag((int) Tags.HUD);
      this.Entity = entity;
      this.Position = position;
      this.info = info;
      this.controls = new List<object>((IEnumerable<object>) controls);
      switch (info)
      {
        case string _:
          this.infoWidth = ActiveFont.Measure((string) info).X;
          this.infoHeight = ActiveFont.LineHeight;
          break;
        case MTexture _:
          this.infoWidth = (float) ((MTexture) info).Width;
          this.infoHeight = (float) ((MTexture) info).Height;
          break;
      }
      this.UpdateControlsSize();
    }

    public void UpdateControlsSize()
    {
      this.controlsWidth = 0.0f;
      foreach (object control in this.controls)
      {
        switch (control)
        {
          case BirdTutorialGui.ButtonPrompt prompt:
            this.controlsWidth += (float) Input.GuiButton(BirdTutorialGui.ButtonPromptToVirtualButton(prompt)).Width + this.buttonPadding * 2f;
            continue;
          case Vector2 direction:
            this.controlsWidth += (float) Input.GuiDirection(direction).Width + this.buttonPadding * 2f;
            continue;
          case string _:
            this.controlsWidth += ActiveFont.Measure(control.ToString()).X;
            continue;
          case MTexture _:
            this.controlsWidth += (float) ((MTexture) control).Width;
            continue;
          default:
            continue;
        }
      }
    }

    public override void Update()
    {
      this.UpdateControlsSize();
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
      double x1 = (double) vector2_1.X - (double) width1 / 2.0;
      float y = (float) ((double) vector2_1.Y - (double) height - 32.0);
      Draw.Rect((float) (x1 - 6.0), y - 6f, width1 + 12f, height + 12f, this.lineColor);
      Draw.Rect((float) x1, y, width1, height, this.bgColor);
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
        switch (control)
        {
          case BirdTutorialGui.ButtonPrompt prompt:
            MTexture mtexture1 = Input.GuiButton(BirdTutorialGui.ButtonPromptToVirtualButton(prompt));
            vector2_2.X += this.buttonPadding;
            mtexture1.Draw(position, new Vector2(-vector2_2.X, (float) (mtexture1.Height / 2)), Color.White, new Vector2(this.Scale, 1f));
            vector2_2.X += (float) mtexture1.Width + this.buttonPadding;
            continue;
          case Vector2 direction:
            if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
              direction.X = -direction.X;
            MTexture mtexture2 = Input.GuiDirection(direction);
            vector2_2.X += this.buttonPadding;
            mtexture2.Draw(position, new Vector2(-vector2_2.X, (float) (mtexture2.Height / 2)), Color.White, new Vector2(this.Scale, 1f));
            vector2_2.X += (float) mtexture2.Width + this.buttonPadding;
            continue;
          case string _:
            string text = control.ToString();
            float x2 = ActiveFont.Measure(text).X;
            ActiveFont.Draw(text, position + new Vector2(1f, 2f), new Vector2(-vector2_2.X / x2, 0.5f), new Vector2(this.Scale, 1f), this.textColor);
            ActiveFont.Draw(text, position + new Vector2(1f, -2f), new Vector2(-vector2_2.X / x2, 0.5f), new Vector2(this.Scale, 1f), Color.White);
            vector2_2.X += x2 + 1f;
            continue;
          case MTexture _:
            MTexture mtexture3 = (MTexture) control;
            mtexture3.Draw(position, new Vector2(-vector2_2.X, (float) (mtexture3.Height / 2)), Color.White, new Vector2(this.Scale, 1f));
            vector2_2.X += (float) mtexture3.Width;
            continue;
          default:
            continue;
        }
      }
    }

    public static VirtualButton ButtonPromptToVirtualButton(BirdTutorialGui.ButtonPrompt prompt)
    {
      switch (prompt)
      {
        case BirdTutorialGui.ButtonPrompt.Dash:
          return Input.Dash;
        case BirdTutorialGui.ButtonPrompt.Jump:
          return Input.Jump;
        case BirdTutorialGui.ButtonPrompt.Grab:
          return Input.Grab;
        case BirdTutorialGui.ButtonPrompt.Talk:
          return Input.Talk;
        default:
          return Input.Jump;
      }
    }

    public enum ButtonPrompt
    {
      Dash,
      Jump,
      Grab,
      Talk,
    }
  }
}
