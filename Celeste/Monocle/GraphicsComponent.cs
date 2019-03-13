// Decompiled with JetBrains decompiler
// Type: Monocle.GraphicsComponent
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
  public abstract class GraphicsComponent : Component
  {
    public Vector2 Scale = Vector2.One;
    public Color Color = Color.White;
    public SpriteEffects Effects = SpriteEffects.None;
    public Vector2 Position;
    public Vector2 Origin;
    public float Rotation;

    public GraphicsComponent(bool active)
      : base(active, true)
    {
    }

    public float X
    {
      get
      {
        return this.Position.X;
      }
      set
      {
        this.Position.X = value;
      }
    }

    public float Y
    {
      get
      {
        return this.Position.Y;
      }
      set
      {
        this.Position.Y = value;
      }
    }

    public bool FlipX
    {
      get
      {
        return (this.Effects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally;
      }
      set
      {
        this.Effects = value ? this.Effects | SpriteEffects.FlipHorizontally : this.Effects & ~SpriteEffects.FlipHorizontally;
      }
    }

    public bool FlipY
    {
      get
      {
        return (this.Effects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically;
      }
      set
      {
        this.Effects = value ? this.Effects | SpriteEffects.FlipVertically : this.Effects & ~SpriteEffects.FlipVertically;
      }
    }

    public Vector2 RenderPosition
    {
      get
      {
        return (this.Entity == null ? Vector2.Zero : this.Entity.Position) + this.Position;
      }
      set
      {
        this.Position = value - (this.Entity == null ? Vector2.Zero : this.Entity.Position);
      }
    }

    public void DrawOutline(int offset = 1)
    {
      this.DrawOutline(Color.Black, offset);
    }

    public void DrawOutline(Color color, int offset = 1)
    {
      Vector2 position = this.Position;
      Color color1 = this.Color;
      this.Color = color;
      for (int index1 = -1; index1 < 2; ++index1)
      {
        for (int index2 = -1; index2 < 2; ++index2)
        {
          if (index1 != 0 || (uint) index2 > 0U)
          {
            this.Position = position + new Vector2((float) (index1 * offset), (float) (index2 * offset));
            this.Render();
          }
        }
      }
      this.Position = position;
      this.Color = color1;
    }
  }
}

