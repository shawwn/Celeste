// Decompiled with JetBrains decompiler
// Type: Monocle.Image
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;

namespace Monocle
{
  public class Image : GraphicsComponent
  {
    public MTexture Texture;

    public Image(MTexture texture)
      : base(false)
    {
      this.Texture = texture;
    }

    internal Image(MTexture texture, bool active)
      : base(active)
    {
      this.Texture = texture;
    }

    public override void Render()
    {
      if (this.Texture == null)
        return;
      this.Texture.Draw(this.RenderPosition, this.Origin, this.Color, this.Scale, this.Rotation, this.Effects);
    }

    public virtual float Width
    {
      get
      {
        return (float) this.Texture.Width;
      }
    }

    public virtual float Height
    {
      get
      {
        return (float) this.Texture.Height;
      }
    }

    public Image SetOrigin(float x, float y)
    {
      this.Origin.X = (__Null) (double) x;
      this.Origin.Y = (__Null) (double) y;
      return this;
    }

    public Image CenterOrigin()
    {
      this.Origin.X = (__Null) ((double) this.Width / 2.0);
      this.Origin.Y = (__Null) ((double) this.Height / 2.0);
      return this;
    }

    public Image JustifyOrigin(Vector2 at)
    {
      this.Origin.X = (__Null) ((double) this.Width * at.X);
      this.Origin.Y = (__Null) ((double) this.Height * at.Y);
      return this;
    }

    public Image JustifyOrigin(float x, float y)
    {
      this.Origin.X = (__Null) ((double) this.Width * (double) x);
      this.Origin.Y = (__Null) ((double) this.Height * (double) y);
      return this;
    }
  }
}
