// Decompiled with JetBrains decompiler
// Type: Monocle.OutlineText
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
  public class OutlineText : Text
  {
    public Color OutlineColor = Color.Black;
    public int OutlineOffset = 1;

    public OutlineText(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      Text.HorizontalAlign horizontalAlign = Text.HorizontalAlign.Center,
      Text.VerticalAlign verticalAlign = Text.VerticalAlign.Center)
      : base(font, text, position, color, horizontalAlign, verticalAlign)
    {
    }

    public OutlineText(
      SpriteFont font,
      string text,
      Vector2 position,
      Text.HorizontalAlign horizontalAlign = Text.HorizontalAlign.Center,
      Text.VerticalAlign verticalAlign = Text.VerticalAlign.Center)
      : this(font, text, position, Color.White, horizontalAlign, verticalAlign)
    {
    }

    public OutlineText(SpriteFont font, string text)
      : this(font, text, Vector2.Zero, Color.White, Text.HorizontalAlign.Center, Text.VerticalAlign.Center)
    {
    }

    public override void Render()
    {
      for (int index1 = -1; index1 < 2; ++index1)
      {
        for (int index2 = -1; index2 < 2; ++index2)
        {
          if (index1 != 0 || (uint) index2 > 0U)
            Draw.SpriteBatch.DrawString(this.Font, this.DrawText, this.RenderPosition + new Vector2((float) (index1 * this.OutlineOffset), (float) (index2 * this.OutlineOffset)), this.OutlineColor, this.Rotation, this.Origin, this.Scale, this.Effects, 0.0f);
        }
      }
      base.Render();
    }
  }
}

