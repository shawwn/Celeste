// Decompiled with JetBrains decompiler
// Type: Celeste.ChangeRespawnTrigger
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class ChangeRespawnTrigger : Trigger
  {
    public Vector2 Target;

    public ChangeRespawnTrigger(EntityData data, Vector2 offset)
      : base(data, offset)
    {
      this.Collider = (Collider) new Hitbox((float) data.Width, (float) data.Height, 0.0f, 0.0f);
      this.Target = data.Nodes == null || (uint) data.Nodes.Length <= 0U ? this.Center : data.Nodes[0] + offset;
      this.Visible = this.Active = false;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.Target = this.SceneAs<Level>().GetSpawnPoint(this.Target);
    }

    public override void OnEnter(Player player)
    {
      base.OnEnter(player);
      if (this.Scene.CollideCheck<Solid>(this.Target + Vector2.UnitY * -4f))
        return;
      Session session = (this.Scene as Level).Session;
      session.HitCheckpoint = true;
      session.RespawnPoint = new Vector2?(this.Target);
      session.UpdateLevelStartDashes();
    }
  }
}

