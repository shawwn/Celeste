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
      this.Position.X = x;
      this.Position.Y = y;
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
        return this.Position.X - this.Radius;
      }
      set
      {
        this.Position.X = value + this.Radius;
      }
    }

    public override float Top
    {
      get
      {
        return this.Position.Y - this.Radius;
      }
      set
      {
        this.Position.Y = value + this.Radius;
      }
    }

    public override float Right
    {
      get
      {
        return this.Position.X + this.Radius;
      }
      set
      {
        this.Position.X = value - this.Radius;
      }
    }

    public override float Bottom
    {
      get
      {
        return this.Position.Y + this.Radius;
      }
      set
      {
        this.Position.Y = value - this.Radius;
      }
    }

    public override Collider Clone()
    {
      return (Collider) new Circle(this.Radius, this.Position.X, this.Position.Y);
    }

    public override void Render(Camera camera, Color color)
    {
      Draw.Circle(this.AbsolutePosition, this.Radius, color, 4);
    }

    public override bool Collide(Vector2 point)
    {
      return Monocle.Collide.CircleToPoint(this.AbsolutePosition, this.Radius, point);
    }

    public override bool Collide(Rectangle rect)
    {
      return Monocle.Collide.RectToCircle(rect, this.AbsolutePosition, this.Radius);
    }

    public override bool Collide(Vector2 from, Vector2 to)
    {
      return Monocle.Collide.CircleToLine(this.AbsolutePosition, this.Radius, from, to);
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

