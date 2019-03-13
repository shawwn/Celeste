// Decompiled with JetBrains decompiler
// Type: Monocle.Collider
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using System;

namespace Monocle
{
  public abstract class Collider
  {
    public Vector2 Position;

    public Entity Entity { get; private set; }

    internal virtual void Added(Entity entity)
    {
      this.Entity = entity;
    }

    internal virtual void Removed()
    {
      this.Entity = (Entity) null;
    }

    public bool Collide(Entity entity)
    {
      return this.Collide(entity.Collider);
    }

    public bool Collide(Collider collider)
    {
      if (collider is Hitbox)
        return this.Collide(collider as Hitbox);
      if (collider is Grid)
        return this.Collide(collider as Grid);
      if (collider is ColliderList)
        return this.Collide(collider as ColliderList);
      if (collider is Circle)
        return this.Collide(collider as Circle);
      throw new Exception("Collisions against the collider type are not implemented!");
    }

    public abstract bool Collide(Vector2 point);

    public abstract bool Collide(Rectangle rect);

    public abstract bool Collide(Vector2 from, Vector2 to);

    public abstract bool Collide(Hitbox hitbox);

    public abstract bool Collide(Grid grid);

    public abstract bool Collide(Circle circle);

    public abstract bool Collide(ColliderList list);

    public abstract Collider Clone();

    public abstract void Render(Camera camera, Color color);

    public abstract float Width { get; set; }

    public abstract float Height { get; set; }

    public abstract float Top { get; set; }

    public abstract float Bottom { get; set; }

    public abstract float Left { get; set; }

    public abstract float Right { get; set; }

    public void CenterOrigin()
    {
      this.Position.X = (float) (-(double) this.Width / 2.0);
      this.Position.Y = (float) (-(double) this.Height / 2.0);
    }

    public float CenterX
    {
      get
      {
        return this.Left + this.Width / 2f;
      }
      set
      {
        this.Left = value - this.Width / 2f;
      }
    }

    public float CenterY
    {
      get
      {
        return this.Top + this.Height / 2f;
      }
      set
      {
        this.Top = value - this.Height / 2f;
      }
    }

    public Vector2 TopLeft
    {
      get
      {
        return new Vector2(this.Left, this.Top);
      }
      set
      {
        this.Left = value.X;
        this.Top = value.Y;
      }
    }

    public Vector2 TopCenter
    {
      get
      {
        return new Vector2(this.CenterX, this.Top);
      }
      set
      {
        this.CenterX = value.X;
        this.Top = value.Y;
      }
    }

    public Vector2 TopRight
    {
      get
      {
        return new Vector2(this.Right, this.Top);
      }
      set
      {
        this.Right = value.X;
        this.Top = value.Y;
      }
    }

    public Vector2 CenterLeft
    {
      get
      {
        return new Vector2(this.Left, this.CenterY);
      }
      set
      {
        this.Left = value.X;
        this.CenterY = value.Y;
      }
    }

    public Vector2 Center
    {
      get
      {
        return new Vector2(this.CenterX, this.CenterY);
      }
      set
      {
        this.CenterX = value.X;
        this.CenterY = value.Y;
      }
    }

    public Vector2 Size
    {
      get
      {
        return new Vector2(this.Width, this.Height);
      }
    }

    public Vector2 HalfSize
    {
      get
      {
        return this.Size * 0.5f;
      }
    }

    public Vector2 CenterRight
    {
      get
      {
        return new Vector2(this.Right, this.CenterY);
      }
      set
      {
        this.Right = value.X;
        this.CenterY = value.Y;
      }
    }

    public Vector2 BottomLeft
    {
      get
      {
        return new Vector2(this.Left, this.Bottom);
      }
      set
      {
        this.Left = value.X;
        this.Bottom = value.Y;
      }
    }

    public Vector2 BottomCenter
    {
      get
      {
        return new Vector2(this.CenterX, this.Bottom);
      }
      set
      {
        this.CenterX = value.X;
        this.Bottom = value.Y;
      }
    }

    public Vector2 BottomRight
    {
      get
      {
        return new Vector2(this.Right, this.Bottom);
      }
      set
      {
        this.Right = value.X;
        this.Bottom = value.Y;
      }
    }

    public void Render(Camera camera)
    {
      this.Render(camera, Color.Red);
    }

    public Vector2 AbsolutePosition
    {
      get
      {
        if (this.Entity != null)
          return this.Entity.Position + this.Position;
        return this.Position;
      }
    }

    public float AbsoluteX
    {
      get
      {
        if (this.Entity != null)
          return this.Entity.Position.X + this.Position.X;
        return this.Position.X;
      }
    }

    public float AbsoluteY
    {
      get
      {
        if (this.Entity != null)
          return this.Entity.Position.Y + this.Position.Y;
        return this.Position.Y;
      }
    }

    public float AbsoluteTop
    {
      get
      {
        if (this.Entity != null)
          return this.Top + this.Entity.Position.Y;
        return this.Top;
      }
    }

    public float AbsoluteBottom
    {
      get
      {
        if (this.Entity != null)
          return this.Bottom + this.Entity.Position.Y;
        return this.Bottom;
      }
    }

    public float AbsoluteLeft
    {
      get
      {
        if (this.Entity != null)
          return this.Left + this.Entity.Position.X;
        return this.Left;
      }
    }

    public float AbsoluteRight
    {
      get
      {
        if (this.Entity != null)
          return this.Right + this.Entity.Position.X;
        return this.Right;
      }
    }

    public Rectangle Bounds
    {
      get
      {
        return new Rectangle((int) this.AbsoluteLeft, (int) this.AbsoluteTop, (int) this.Width, (int) this.Height);
      }
    }
  }
}

