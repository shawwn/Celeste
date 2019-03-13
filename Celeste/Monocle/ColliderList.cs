// Decompiled with JetBrains decompiler
// Type: Monocle.ColliderList
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Monocle
{
  public class ColliderList : Collider
  {
    public Collider[] colliders { get; private set; }

    public ColliderList(params Collider[] colliders)
    {
      this.colliders = colliders;
    }

    public void Add(params Collider[] toAdd)
    {
      Collider[] colliderArray = new Collider[this.colliders.Length + toAdd.Length];
      for (int index = 0; index < this.colliders.Length; ++index)
        colliderArray[index] = this.colliders[index];
      for (int index = 0; index < toAdd.Length; ++index)
      {
        colliderArray[index + this.colliders.Length] = toAdd[index];
        toAdd[index].Added(this.Entity);
      }
      this.colliders = colliderArray;
    }

    public void Remove(params Collider[] toRemove)
    {
      Collider[] colliderArray = new Collider[this.colliders.Length - toRemove.Length];
      int index = 0;
      foreach (Collider collider in this.colliders)
      {
        if (!((IEnumerable<Collider>) toRemove).Contains<Collider>(collider))
        {
          colliderArray[index] = collider;
          ++index;
        }
      }
      this.colliders = colliderArray;
    }

    internal override void Added(Entity entity)
    {
      base.Added(entity);
      foreach (Collider collider in this.colliders)
        collider.Added(entity);
    }

    internal override void Removed()
    {
      base.Removed();
      foreach (Collider collider in this.colliders)
        collider.Removed();
    }

    public override float Width
    {
      get
      {
        return this.Right - this.Left;
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    public override float Height
    {
      get
      {
        return this.Bottom - this.Top;
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    public override float Left
    {
      get
      {
        float left = this.colliders[0].Left;
        for (int index = 1; index < this.colliders.Length; ++index)
        {
          if ((double) this.colliders[index].Left < (double) left)
            left = this.colliders[index].Left;
        }
        return left;
      }
      set
      {
        float num = value - this.Left;
        foreach (Collider collider in this.colliders)
          this.Position.X += num;
      }
    }

    public override float Right
    {
      get
      {
        float right = this.colliders[0].Right;
        for (int index = 1; index < this.colliders.Length; ++index)
        {
          if ((double) this.colliders[index].Right > (double) right)
            right = this.colliders[index].Right;
        }
        return right;
      }
      set
      {
        float num = value - this.Right;
        foreach (Collider collider in this.colliders)
          this.Position.X += num;
      }
    }

    public override float Top
    {
      get
      {
        float top = this.colliders[0].Top;
        for (int index = 1; index < this.colliders.Length; ++index)
        {
          if ((double) this.colliders[index].Top < (double) top)
            top = this.colliders[index].Top;
        }
        return top;
      }
      set
      {
        float num = value - this.Top;
        foreach (Collider collider in this.colliders)
          this.Position.Y += num;
      }
    }

    public override float Bottom
    {
      get
      {
        float bottom = this.colliders[0].Bottom;
        for (int index = 1; index < this.colliders.Length; ++index)
        {
          if ((double) this.colliders[index].Bottom > (double) bottom)
            bottom = this.colliders[index].Bottom;
        }
        return bottom;
      }
      set
      {
        float num = value - this.Bottom;
        foreach (Collider collider in this.colliders)
          this.Position.Y += num;
      }
    }

    public override Collider Clone()
    {
      Collider[] colliderArray = new Collider[this.colliders.Length];
      for (int index = 0; index < this.colliders.Length; ++index)
        colliderArray[index] = this.colliders[index].Clone();
      return (Collider) new ColliderList(colliderArray);
    }

    public override void Render(Camera camera, Color color)
    {
      foreach (Collider collider in this.colliders)
        collider.Render(camera, color);
    }

    public override bool Collide(Vector2 point)
    {
      foreach (Collider collider in this.colliders)
      {
        if (collider.Collide(point))
          return true;
      }
      return false;
    }

    public override bool Collide(Rectangle rect)
    {
      foreach (Collider collider in this.colliders)
      {
        if (collider.Collide(rect))
          return true;
      }
      return false;
    }

    public override bool Collide(Vector2 from, Vector2 to)
    {
      foreach (Collider collider in this.colliders)
      {
        if (collider.Collide(from, to))
          return true;
      }
      return false;
    }

    public override bool Collide(Hitbox hitbox)
    {
      foreach (Collider collider in this.colliders)
      {
        if (collider.Collide(hitbox))
          return true;
      }
      return false;
    }

    public override bool Collide(Grid grid)
    {
      foreach (Collider collider in this.colliders)
      {
        if (collider.Collide(grid))
          return true;
      }
      return false;
    }

    public override bool Collide(Circle circle)
    {
      foreach (Collider collider in this.colliders)
      {
        if (collider.Collide(circle))
          return true;
      }
      return false;
    }

    public override bool Collide(ColliderList list)
    {
      foreach (Collider collider in this.colliders)
      {
        if (collider.Collide(list))
          return true;
      }
      return false;
    }
  }
}

