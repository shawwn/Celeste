// Decompiled with JetBrains decompiler
// Type: Celeste.Holdable
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class Holdable : Component
  {
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
    public bool SlowRun = true;
    public bool SlowFall;
    private float cannotHoldDelay;
    private Vector2 startPos;
    private float gravityTimer;
    private float cannotHoldTimer;
    private int idleDepth;

    public Player Holder { get; private set; }

    public Holdable(float cannotHoldDelay = 0.1f)
      : base(true, false)
    {
      this.cannotHoldDelay = cannotHoldDelay;
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
      this.idleDepth = this.Entity.Depth;
      this.Entity.Depth = player.Depth - 1;
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
        if ((double) force.X != 0.0)
        {
          bool flag = false;
          int num1 = Math.Sign(force.X);
          int num2 = 0;
          while (!flag && num2++ < 10)
          {
            if (!this.Entity.CollideCheck<Solid>(this.Entity.Position + (float) (num1 * num2) * Vector2.UnitX))
              flag = true;
          }
          if (flag)
            this.Entity.X += (float) (num1 * num2);
        }
        while (this.Entity.CollideCheck<Solid>())
          this.Entity.Position += Vector2.UnitY;
      }
      this.Entity.Depth = this.idleDepth;
      this.Holder = (Player) null;
      this.gravityTimer = 0.1f;
      this.cannotHoldTimer = this.cannotHoldDelay;
      if (this.OnRelease == null)
        return;
      this.OnRelease(force);
    }

    public bool IsHeld => this.Holder != null;

    public bool ShouldHaveGravity => (double) this.gravityTimer <= 0.0;

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
      this.Entity.Collider.Render(camera, Color.Pink);
      this.Entity.Collider = collider;
    }

    public bool Dangerous(HoldableCollider hc) => this.DangerousCheck != null && this.DangerousCheck(hc);

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

    public bool HitSpring(Spring spring) => this.OnHitSpring != null && this.OnHitSpring(spring);

    public void HitSpinner(Entity spinner)
    {
      if (this.OnHitSpinner == null)
        return;
      this.OnHitSpinner(spinner);
    }

    public Vector2 GetSpeed() => this.SpeedGetter != null ? this.SpeedGetter() : Vector2.Zero;
  }
}
