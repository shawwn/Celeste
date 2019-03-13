// Decompiled with JetBrains decompiler
// Type: Celeste.EffectCutout
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  [Tracked(false)]
  public class EffectCutout : Component
  {
    public float Alpha = 1f;
    private Rectangle lastSize;
    private bool lastVisible;
    private float lastAlpha;

    public int Left
    {
      get
      {
        return (int) this.Entity.Collider.AbsoluteLeft;
      }
    }

    public int Right
    {
      get
      {
        return (int) this.Entity.Collider.AbsoluteRight;
      }
    }

    public int Top
    {
      get
      {
        return (int) this.Entity.Collider.AbsoluteTop;
      }
    }

    public int Bottom
    {
      get
      {
        return (int) this.Entity.Collider.AbsoluteBottom;
      }
    }

    public Rectangle Bounds
    {
      get
      {
        return this.Entity.Collider.Bounds;
      }
    }

    public EffectCutout()
      : base(true, true)
    {
    }

    public override void Update()
    {
      bool flag = this.Visible && this.Entity.Visible;
      Rectangle bounds = this.Bounds;
      if (!(this.lastSize != bounds) && (double) this.lastAlpha == (double) this.Alpha && this.lastVisible == flag)
        return;
      this.MakeLightsDirty();
      this.lastSize = bounds;
      this.lastAlpha = this.Alpha;
      this.lastVisible = flag;
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
      Rectangle bounds = this.Bounds;
      foreach (VertexLight component in this.Entity.Scene.Tracker.GetComponents<VertexLight>())
      {
        if (!component.Dirty)
        {
          Rectangle rectangle = new Rectangle((int) ((double) component.Center.X - (double) component.EndRadius), (int) ((double) component.Center.Y - (double) component.EndRadius), (int) component.EndRadius * 2, (int) component.EndRadius * 2);
          if (bounds.Intersects(rectangle) || this.lastSize.Intersects(rectangle))
            component.Dirty = true;
        }
      }
    }
  }
}

