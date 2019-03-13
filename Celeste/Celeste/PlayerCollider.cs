// Decompiled with JetBrains decompiler
// Type: Celeste.PlayerCollider
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class PlayerCollider : Component
  {
    public Action<Player> OnCollide;
    public Collider Collider;
    public Collider FeatherCollider;

    public PlayerCollider(Action<Player> onCollide, Collider collider = null, Collider featherCollider = null)
      : base(false, false)
    {
      this.OnCollide = onCollide;
      this.Collider = collider;
      this.FeatherCollider = featherCollider;
    }

    public bool Check(Player player)
    {
      Collider collider1 = this.Collider;
      if (this.FeatherCollider != null && player.StateMachine.State == 19)
        collider1 = this.FeatherCollider;
      if (collider1 == null)
      {
        if (!player.CollideCheck(this.Entity))
          return false;
        this.OnCollide(player);
        return true;
      }
      Collider collider2 = this.Entity.Collider;
      this.Entity.Collider = collider1;
      bool flag = player.CollideCheck(this.Entity);
      this.Entity.Collider = collider2;
      if (!flag)
        return false;
      this.OnCollide(player);
      return true;
    }

    public override void DebugRender(Camera camera)
    {
      if (this.Collider == null)
        return;
      Collider collider = this.Entity.Collider;
      this.Entity.Collider = this.Collider;
      this.Collider.Render(camera, Color.HotPink);
      this.Entity.Collider = collider;
    }
  }
}

