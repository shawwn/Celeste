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
    public Vector2 Scale = Vector2.get_One();
    public Color Color = Color.get_White();
    public Vector2 Position;
    public Vector2 Origin;
    public float Rotation;
    public SpriteEffects Effects;

    public GraphicsComponent(bool active)
      : base(active, true)
    {
    }

    public float X
    {
      get
      {
        return (float) this.Position.X;
      }
      set
      {
        this.Position.X = (__Null) (double) value;
      }
    }

    public float Y
    {
      get
      {
        return (float) this.Position.Y;
      }
      set
      {
        this.Position.Y = (__Null) (double) value;
      }
    }

    public bool FlipX
    {
      get
      {
        return (this.Effects & 1) == 1;
      }
      set
      {
        this.Effects = value ? (SpriteEffects) (this.Effects | 1) : (SpriteEffects) (this.Effects & -2);
      }
    }

    public bool FlipY
    {
      get
      {
        return (this.Effects & 2) == 2;
      }
      set
      {
        this.Effects = value ? (SpriteEffects) (this.Effects | 2) : (SpriteEffects) (this.Effects & -3);
      }
    }

    public Vector2 RenderPosition
    {
      get
      {
        return Vector2.op_Addition(this.Entity == null ? Vector2.get_Zero() : this.Entity.Position, this.Position);
      }
      set
      {
        this.Position = Vector2.op_Subtraction(value, this.Entity == null ? Vector2.get_Zero() : this.Entity.Position);
      }
    }

    public void DrawOutline(int offset = 1)
    {
      this.DrawOutline(Color.get_Black(), offset);
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
          if (index1 != 0 || index2 != 0)
          {
            this.Position = Vector2.op_Addition(position, new Vector2((float) (index1 * offset), (float) (index2 * offset)));
            this.Render();
          }
        }
      }
      this.Position = position;
      this.Color = color1;
    }
  }
}
