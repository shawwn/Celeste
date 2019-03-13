// Decompiled with JetBrains decompiler
// Type: Celeste.DustStaticSpinner
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class DustStaticSpinner : Entity
  {
    private float offset = Calc.Random.NextFloat();
    public static ParticleType P_Move;
    public const float ParticleInterval = 0.02f;
    public DustGraphic Sprite;

    public DustStaticSpinner(Vector2 position, bool attachToSolid, bool ignoreSolids = false)
      : base(position)
    {
      this.Collider = (Collider) new ColliderList(new Collider[2]
      {
        (Collider) new Monocle.Circle(6f, 0.0f, 0.0f),
        (Collider) new Hitbox(16f, 4f, -8f, -3f)
      });
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Add((Component) new HoldableCollider(new Action<Holdable>(this.OnHoldable), (Collider) null));
      this.Add((Component) new LedgeBlocker((Func<Player, bool>) null));
      this.Add((Component) (this.Sprite = new DustGraphic(ignoreSolids, true, true)));
      this.Depth = -50;
      if (!attachToSolid)
        return;
      this.Add((Component) new StaticMover()
      {
        OnShake = new Action<Vector2>(this.OnShake),
        SolidChecker = new Func<Solid, bool>(this.IsRiding)
      });
    }

    public DustStaticSpinner(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Bool("attachToSolid", false), false)
    {
    }

    public void ForceInstantiate()
    {
      this.Sprite.AddDustNodesIfInCamera();
    }

    public override void Update()
    {
      base.Update();
      if (!this.Scene.OnInterval(0.05f, this.offset) || !this.Sprite.Estableshed)
        return;
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity != null)
        this.Collidable = (double) Math.Abs(entity.X - this.X) < 128.0 && (double) Math.Abs(entity.Y - this.Y) < 128.0;
    }

    private void OnShake(Vector2 pos)
    {
      this.Sprite.Position = pos;
    }

    private bool IsRiding(Solid solid)
    {
      return this.CollideCheck((Entity) solid);
    }

    private void OnPlayer(Player player)
    {
      player.Die((player.Position - this.Position).SafeNormalize(), false, true);
      this.Sprite.OnHitPlayer();
    }

    private void OnHoldable(Holdable h)
    {
      h.HitSpinner((Entity) this);
    }
  }
}

