// Decompiled with JetBrains decompiler
// Type: Celeste.StaticMover
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class StaticMover : Component
  {
    public Func<Solid, bool> SolidChecker;
    public Func<JumpThru, bool> JumpThruChecker;
    public Action<Vector2> OnMove;
    public Action<Vector2> OnShake;
    public Action<Platform> OnAttach;
    public Action OnDestroy;
    public Action OnDisable;
    public Action OnEnable;
    public Platform Platform;

    public StaticMover()
      : base(false, false)
    {
    }

    public void Destroy()
    {
      if (this.OnDestroy != null)
        this.OnDestroy();
      else
        this.Entity.RemoveSelf();
    }

    public void Shake(Vector2 amount)
    {
      if (this.OnShake == null)
        return;
      this.OnShake(amount);
    }

    public void Move(Vector2 amount)
    {
      if (this.OnMove != null)
      {
        this.OnMove(amount);
      }
      else
      {
        Entity entity = this.Entity;
        entity.Position = Vector2.op_Addition(entity.Position, amount);
      }
    }

    public bool IsRiding(Solid solid)
    {
      if (this.SolidChecker != null)
        return this.SolidChecker(solid);
      return false;
    }

    public bool IsRiding(JumpThru jumpThru)
    {
      if (this.JumpThruChecker != null)
        return this.JumpThruChecker(jumpThru);
      return false;
    }

    public void TriggerPlatform()
    {
      if (this.Platform == null)
        return;
      this.Platform.OnStaticMoverTrigger();
    }

    public void Disable()
    {
      if (this.OnDisable != null)
        this.OnDisable();
      else
        this.Entity.Active = this.Entity.Visible = this.Entity.Collidable = false;
    }

    public void Enable()
    {
      if (this.OnEnable != null)
        this.OnEnable();
      else
        this.Entity.Active = this.Entity.Visible = this.Entity.Collidable = true;
    }
  }
}
