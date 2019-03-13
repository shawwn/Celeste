// Decompiled with JetBrains decompiler
// Type: Celeste.Killbox
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class Killbox : Entity
  {
    public Killbox(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Collider = (Collider) new Hitbox((float) data.Width, 32f, 0.0f, 0.0f);
      this.Collidable = false;
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
    }

    private void OnPlayer(Player player)
    {
      if (SaveData.Instance.Assists.Invincible)
        player.Bounce(this.Top);
      else
        player.Die(Vector2.Zero, false, true);
    }

    public override void Update()
    {
      if (!this.Collidable)
      {
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity != null && (double) entity.Bottom < (double) this.Top - 32.0)
          this.Collidable = true;
      }
      base.Update();
    }
  }
}

