// Decompiled with JetBrains decompiler
// Type: Monocle.Hitbox
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;

namespace Monocle
{
  public class Hitbox : Collider
  {
    private float width;
    private float height;

    public Hitbox(float width, float height, float x = 0.0f, float y = 0.0f)
    {
      this.width = width;
      this.height = height;
      this.Position.X = x;
      this.Position.Y = y;
    }

    public override float Width
    {
      get
      {
        return this.width;
      }
      set
      {
        this.width = value;
      }
    }

    public override float Height
    {
      get
      {
        return this.height;
      }
      set
      {
        this.height = value;
      }
    }

    public override float Left
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

    public override float Top
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

    public override float Right
    {
      get
      {
        return this.Position.X + this.Width;
      }
      set
      {
        this.Position.X = value - this.Width;
      }
    }

    public override float Bottom
    {
      get
      {
        return this.Position.Y + this.Height;
      }
      set
      {
        this.Position.Y = value - this.Height;
      }
    }

    public bool Intersects(Hitbox hitbox)
    {
      return (double) this.AbsoluteLeft < (double) hitbox.AbsoluteRight && (double) this.AbsoluteRight > (double) hitbox.AbsoluteLeft && (double) this.AbsoluteBottom > (double) hitbox.AbsoluteTop && (double) this.AbsoluteTop < (double) hitbox.AbsoluteBottom;
    }

    public bool Intersects(float x, float y, float width, float height)
    {
      return (double) this.AbsoluteRight > (double) x && (double) this.AbsoluteBottom > (double) y && (double) this.AbsoluteLeft < (double) x + (double) width && (double) this.AbsoluteTop < (double) y + (double) height;
    }

    public override Collider Clone()
    {
      return (Collider) new Hitbox(this.width, this.height, this.Position.X, this.Position.Y);
    }

    public override void Render(Camera camera, Color color)
    {
      Draw.HollowRect(this.AbsoluteX, this.AbsoluteY, this.Width, this.Height, color);
    }

    public void SetFromRectangle(Rectangle rect)
    {
      this.Position = new Vector2((float) rect.X, (float) rect.Y);
      this.Width = (float) rect.Width;
      this.Height = (float) rect.Height;
    }

    public void Set(float x, float y, float w, float h)
    {
      this.Position = new Vector2(x, y);
      this.Width = w;
      this.Height = h;
    }

    public void GetTopEdge(out Vector2 from, out Vector2 to)
    {
      from.X = this.AbsoluteLeft;
      to.X = this.AbsoluteRight;
      from.Y = to.Y = this.AbsoluteTop;
    }

    public void GetBottomEdge(out Vector2 from, out Vector2 to)
    {
      from.X = this.AbsoluteLeft;
      to.X = this.AbsoluteRight;
      from.Y = to.Y = this.AbsoluteBottom;
    }

    public void GetLeftEdge(out Vector2 from, out Vector2 to)
    {
      from.Y = this.AbsoluteTop;
      to.Y = this.AbsoluteBottom;
      from.X = to.X = this.AbsoluteLeft;
    }

    public void GetRightEdge(out Vector2 from, out Vector2 to)
    {
      from.Y = this.AbsoluteTop;
      to.Y = this.AbsoluteBottom;
      from.X = to.X = this.AbsoluteRight;
    }

    public override bool Collide(Vector2 point)
    {
      return Monocle.Collide.RectToPoint(this.AbsoluteLeft, this.AbsoluteTop, this.Width, this.Height, point);
    }

    public override bool Collide(Rectangle rect)
    {
      return (double) this.AbsoluteRight > (double) rect.Left && (double) this.AbsoluteBottom > (double) rect.Top && (double) this.AbsoluteLeft < (double) rect.Right && (double) this.AbsoluteTop < (double) rect.Bottom;
    }

    public override bool Collide(Vector2 from, Vector2 to)
    {
      return Monocle.Collide.RectToLine(this.AbsoluteLeft, this.AbsoluteTop, this.Width, this.Height, from, to);
    }

    public override bool Collide(Hitbox hitbox)
    {
      return this.Intersects(hitbox);
    }

    public override bool Collide(Grid grid)
    {
      return grid.Collide(this.Bounds);
    }

    public override bool Collide(Circle circle)
    {
      return Monocle.Collide.RectToCircle(this.AbsoluteLeft, this.AbsoluteTop, this.Width, this.Height, circle.AbsolutePosition, circle.Radius);
    }

    public override bool Collide(ColliderList list)
    {
      return list.Collide(this);
    }
  }
}

