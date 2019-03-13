// Decompiled with JetBrains decompiler
// Type: Celeste.SafeGroundBlocker
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  [Tracked(false)]
  public class SafeGroundBlocker : Component
  {
    public bool Blocking = true;
    public Collider CheckWith;

    public SafeGroundBlocker(Collider checkWith = null)
      : base(false, false)
    {
      this.CheckWith = checkWith;
    }

    public bool Check(Player player)
    {
      if (!this.Blocking)
        return false;
      Collider collider = this.Entity.Collider;
      if (this.CheckWith != null)
        this.Entity.Collider = this.CheckWith;
      bool flag = player.CollideCheck(this.Entity);
      this.Entity.Collider = collider;
      return flag;
    }

    public override void DebugRender(Camera camera)
    {
      Collider collider = this.Entity.Collider;
      if (this.CheckWith != null)
        this.Entity.Collider = this.CheckWith;
      this.Entity.Collider.Render(camera, Color.Aqua);
      this.Entity.Collider = collider;
    }
  }
}

