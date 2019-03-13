// Decompiled with JetBrains decompiler
// Type: Celeste.LightOcclude
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  [Tracked(false)]
  public class LightOcclude : Component
  {
    public float Alpha = 1f;
    private Rectangle? bounds = new Rectangle?();
    public Rectangle RenderBounds;
    private Rectangle lastSize;
    private bool lastVisible;
    private float lastAlpha;

    public int Left
    {
      get
      {
        if (this.bounds.HasValue)
          return (int) this.Entity.X + this.bounds.Value.Left;
        return (int) this.Entity.Collider.AbsoluteLeft;
      }
    }

    public int Width
    {
      get
      {
        if (this.bounds.HasValue)
          return this.bounds.Value.Width;
        return (int) this.Entity.Collider.Width;
      }
    }

    public int Top
    {
      get
      {
        if (this.bounds.HasValue)
          return (int) this.Entity.Y + this.bounds.Value.Top;
        return (int) this.Entity.Collider.AbsoluteTop;
      }
    }

    public int Height
    {
      get
      {
        if (this.bounds.HasValue)
          return this.bounds.Value.Height;
        return (int) this.Entity.Collider.Height;
      }
    }

    public int Right
    {
      get
      {
        return this.Left + this.Width;
      }
    }

    public int Bottom
    {
      get
      {
        return this.Top + this.Height;
      }
    }

    public LightOcclude(float alpha = 1f)
      : base(true, true)
    {
      this.Alpha = alpha;
    }

    public LightOcclude(Rectangle bounds, float alpha = 1f)
      : this(alpha)
    {
      this.bounds = new Rectangle?(bounds);
    }

    public override void Update()
    {
      base.Update();
      bool flag = this.Visible && this.Entity.Visible;
      Rectangle rectangle = new Rectangle(this.Left, this.Top, this.Width, this.Height);
      if (!(this.lastSize != rectangle) && this.lastVisible == flag && (double) this.lastAlpha == (double) this.Alpha)
        return;
      this.MakeLightsDirty();
      this.lastVisible = flag;
      this.lastSize = rectangle;
      this.lastAlpha = this.Alpha;
    }

    public override void Removed(Entity entity)
    {
      this.MakeLightsDirty();
      base.Removed(entity);
    }

    public override void EntityRemoved(Scene scene)
    {
      this.MakeLightsDirty();
      base.EntityRemoved(scene);
    }

    private void MakeLightsDirty()
    {
      Rectangle rectangle1 = new Rectangle(this.Left, this.Top, this.Width, this.Height);
      foreach (VertexLight component in this.Entity.Scene.Tracker.GetComponents<VertexLight>())
      {
        if (!component.Dirty)
        {
          Rectangle rectangle2 = new Rectangle((int) ((double) component.Center.X - (double) component.EndRadius), (int) ((double) component.Center.Y - (double) component.EndRadius), (int) component.EndRadius * 2, (int) component.EndRadius * 2);
          if (rectangle1.Intersects(rectangle2) || this.lastSize.Intersects(rectangle2))
            component.Dirty = true;
        }
      }
    }
  }
}

