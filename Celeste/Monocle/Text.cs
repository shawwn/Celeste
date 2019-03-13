// Decompiled with JetBrains decompiler
// Type: Monocle.Text
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
  public class Text : GraphicsComponent
  {
    private SpriteFont font;
    private string text;
    private Text.HorizontalAlign horizontalOrigin;
    private Text.VerticalAlign verticalOrigin;
    private Vector2 size;

    public Text(
      SpriteFont font,
      string text,
      Vector2 position,
      Color color,
      Text.HorizontalAlign horizontalAlign = Text.HorizontalAlign.Center,
      Text.VerticalAlign verticalAlign = Text.VerticalAlign.Center)
      : base(false)
    {
      this.font = font;
      this.text = text;
      this.Position = position;
      this.Color = color;
      this.horizontalOrigin = horizontalAlign;
      this.verticalOrigin = verticalAlign;
      this.UpdateSize();
    }

    public Text(
      SpriteFont font,
      string text,
      Vector2 position,
      Text.HorizontalAlign horizontalAlign = Text.HorizontalAlign.Center,
      Text.VerticalAlign verticalAlign = Text.VerticalAlign.Center)
      : this(font, text, position, Color.White, horizontalAlign, verticalAlign)
    {
    }

    public SpriteFont Font
    {
      get
      {
        return this.font;
      }
      set
      {
        this.font = value;
        this.UpdateSize();
      }
    }

    public string DrawText
    {
      get
      {
        return this.text;
      }
      set
      {
        this.text = value;
        this.UpdateSize();
      }
    }

    public Text.HorizontalAlign HorizontalOrigin
    {
      get
      {
        return this.horizontalOrigin;
      }
      set
      {
        this.horizontalOrigin = value;
        this.UpdateCentering();
      }
    }

    public Text.VerticalAlign VerticalOrigin
    {
      get
      {
        return this.verticalOrigin;
      }
      set
      {
        this.verticalOrigin = value;
        this.UpdateCentering();
      }
    }

    public float Width
    {
      get
      {
        return this.size.X;
      }
    }

    public float Height
    {
      get
      {
        return this.size.Y;
      }
    }

    private void UpdateSize()
    {
      this.size = this.font.MeasureString(this.text);
      this.UpdateCentering();
    }

    private void UpdateCentering()
    {
      if (this.horizontalOrigin == Text.HorizontalAlign.Left)
        this.Origin.X = 0.0f;
      else if (this.horizontalOrigin == Text.HorizontalAlign.Center)
        this.Origin.X = this.size.X / 2f;
      else
        this.Origin.X = this.size.X;
      if (this.verticalOrigin == Text.VerticalAlign.Top)
        this.Origin.Y = 0.0f;
      else if (this.verticalOrigin == Text.VerticalAlign.Center)
        this.Origin.Y = this.size.Y / 2f;
      else
        this.Origin.Y = this.size.Y;
      this.Origin = this.Origin.Floor();
    }

    public override void Render()
    {
      Draw.SpriteBatch.DrawString(this.font, this.text, this.RenderPosition, this.Color, this.Rotation, this.Origin, this.Scale, this.Effects, 0.0f);
    }

    public enum HorizontalAlign
    {
      Left,
      Center,
      Right,
    }

    public enum VerticalAlign
    {
      Top,
      Center,
      Bottom,
    }
  }
}

