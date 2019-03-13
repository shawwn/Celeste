// Decompiled with JetBrains decompiler
// Type: Monocle.Circle
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;

namespace Monocle
{
  public class Circle : Collider
  {
    public float Radius;

    public Circle(float radius, float x = 0.0f, float y = 0.0f)
    {
      this.Radius = radius;
      this.Position.X = (__Null) (double) x;
      this.Position.Y = (__Null) (double) y;
    }

    public override float Width
    {
      get
      {
        return this.Radius * 2f;
      }
      set
      {
        this.Radius = value / 2f;
      }
    }

    public override float Height
    {
      get
      {
        return this.Radius * 2f;
      }
      set
      {
        this.Radius = value / 2f;
      }
    }

    public override float Left
    {
      get
      {
        return (float) this.Position.X - this.Radius;
      }
      set
      {
        this.Position.X = (__Null) ((double) value + (double) this.Radius);
      }
    }

    public override float Top
    {
      get
      {
        return (float) this.Position.Y - this.Radius;
      }
      set
      {
        this.Position.Y = (__Null) ((double) value + (double) this.Radius);
      }
    }

    public override float Right
    {
      get
      {
        return (float) this.Position.X + this.Radius;
      }
      set
      {
        this.Position.X = (__Null) ((double) value - (double) this.Radius);
      }
    }

    public override float Bottom
    {
      get
      {
        return (float) this.Position.Y + this.Radius;
      }
      set
      {
        this.Position.Y = (__Null) ((double) value - (double) this.Radius);
      }
    }

    public override Collider Clone()
    {
      return (Collider) new Circle(this.Radius, (float) this.Position.X, (float) this.Position.Y);
    }

    public override void Render(Camera camera, Color color)
    {
      Draw.Circle(this.AbsolutePosition, this.Radius, color, 4);
    }

    public override bool Collide(Vector2 point)
    {
      return Collide.CircleToPoint(this.AbsolutePosition, this.Radius, point);
    }

    public override bool Collide(Rectangle rect)
    {
      return Collide.RectToCircle(rect, this.AbsolutePosition, this.Radius);
    }

    public override bool Collide(Vector2 from, Vector2 to)
    {
      return Collide.CircleToLine(this.AbsolutePosition, this.Radius, from, to);
    }

    public override bool Collide(Circle circle)
    {
      return (double) Vector2.DistanceSquared(this.AbsolutePosition, circle.AbsolutePosition) < ((double) this.Radius + (double) circle.Radius) * ((double) this.Radius + (double) circle.Radius);
    }

    public override bool Collide(Hitbox hitbox)
    {
      return hitbox.Collide(this);
    }

    public override bool Collide(Grid grid)
    {
      return grid.Collide(this);
    }

    public override bool Collide(ColliderList list)
    {
      return list.Collide(this);
    }
  }
}
