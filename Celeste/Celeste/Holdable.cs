// Decompiled with JetBrains decompiler
// Type: Celeste.Holdable
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class Holdable : Component
  {
    public bool SlowRun = true;
    public Collider PickupCollider;
    public Action OnPickup;
    public Action<Vector2> OnCarry;
    public Action<Vector2> OnRelease;
    public Func<HoldableCollider, bool> DangerousCheck;
    public Action<Seeker> OnHitSeeker;
    public Action<HoldableCollider, int> OnSwat;
    public Func<Spring, bool> OnHitSpring;
    public Action<Entity> OnHitSpinner;
    public Func<Vector2> SpeedGetter;
    private Vector2 startPos;
    private float gravityTimer;
    private float cannotHoldTimer;

    public Player Holder { get; private set; }

    public Holdable()
      : base(true, false)
    {
    }

    public bool Check(Player player)
    {
      Collider collider = this.Entity.Collider;
      if (this.PickupCollider != null)
        this.Entity.Collider = this.PickupCollider;
      int num = player.CollideCheck(this.Entity) ? 1 : 0;
      this.Entity.Collider = collider;
      return num != 0;
    }

    public override void Added(Entity entity)
    {
      base.Added(entity);
      this.startPos = this.Entity.Position;
    }

    public override void EntityRemoved(Scene scene)
    {
      base.EntityRemoved(scene);
      if (this.Holder == null || this.Holder == null)
        return;
      this.Holder.Holding = (Holdable) null;
    }

    public bool Pickup(Player player)
    {
      if ((double) this.cannotHoldTimer > 0.0 || this.Scene == null || this.Entity.Scene == null)
        return false;
      this.Entity.Visible = true;
      this.Holder = player;
      if (this.OnPickup != null)
        this.OnPickup();
      return true;
    }

    public void Carry(Vector2 position)
    {
      if (this.OnCarry != null)
        this.OnCarry(position);
      else
        this.Entity.Position = position;
    }

    public void Release(Vector2 force)
    {
      if (this.Entity.CollideCheck<Solid>())
      {
        if (force.X != 0.0)
        {
          bool flag = false;
          int num1 = Math.Sign((float) force.X);
          int num2 = 0;
          while (!flag && num2++ < 10)
          {
            if (!this.Entity.CollideCheck<Solid>(Vector2.op_Addition(this.Entity.Position, Vector2.op_Multiply((float) (num1 * num2), Vector2.get_UnitX()))))
              flag = true;
          }
          if (flag)
            this.Entity.X += (float) (num1 * num2);
        }
        while (this.Entity.CollideCheck<Solid>())
        {
          Entity entity = this.Entity;
          entity.Position = Vector2.op_Addition(entity.Position, Vector2.get_UnitY());
        }
      }
      this.Holder = (Player) null;
      this.gravityTimer = 0.1f;
      this.cannotHoldTimer = 0.1f;
      if (this.OnRelease == null)
        return;
      this.OnRelease(force);
    }

    public bool IsHeld
    {
      get
      {
        return this.Holder != null;
      }
    }

    public bool ShouldHaveGravity
    {
      get
      {
        return (double) this.gravityTimer <= 0.0;
      }
    }

    public override void Update()
    {
      base.Update();
      if ((double) this.cannotHoldTimer > 0.0)
        this.cannotHoldTimer -= Engine.DeltaTime;
      if ((double) this.gravityTimer <= 0.0)
        return;
      this.gravityTimer -= Engine.DeltaTime;
    }

    public void CheckAgainstColliders()
    {
      foreach (HoldableCollider component in this.Scene.Tracker.GetComponents<HoldableCollider>())
      {
        if (component.Check(this))
          component.OnCollide(this);
      }
    }

    public override void DebugRender(Camera camera)
    {
      base.DebugRender(camera);
      if (this.PickupCollider == null)
        return;
      Collider collider = this.Entity.Collider;
      this.Entity.Collider = this.PickupCollider;
      this.Entity.Collider.Render(camera, Color.get_Pink());
      this.Entity.Collider = collider;
    }

    public bool Dangerous(HoldableCollider hc)
    {
      if (this.DangerousCheck == null)
        return false;
      return this.DangerousCheck(hc);
    }

    public void HitSeeker(Seeker seeker)
    {
      if (this.OnHitSeeker == null)
        return;
      this.OnHitSeeker(seeker);
    }

    public void Swat(HoldableCollider hc, int dir)
    {
      if (this.OnSwat == null)
        return;
      this.OnSwat(hc, dir);
    }

    public bool HitSpring(Spring spring)
    {
      if (this.OnHitSpring != null)
        return this.OnHitSpring(spring);
      return false;
    }

    public void HitSpinner(Entity spinner)
    {
      if (this.OnHitSpinner == null)
        return;
      this.OnHitSpinner(spinner);
    }

    public Vector2 GetSpeed()
    {
      if (this.SpeedGetter != null)
        return this.SpeedGetter();
      return Vector2.get_Zero();
    }
  }
}
