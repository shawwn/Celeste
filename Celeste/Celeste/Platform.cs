// Decompiled with JetBrains decompiler
// Type: Celeste.Platform
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(true)]
  public abstract class Platform : Entity
  {
    protected List<StaticMover> staticMovers = new List<StaticMover>();
    public bool BlockWaterfalls = true;
    public int SurfaceSoundIndex = 8;
    private Vector2 movementCounter;
    private Vector2 shakeAmount;
    private bool shaking;
    private float shakeTimer;
    public Vector2 LiftSpeed;
    public bool Safe;
    public int SurfaceSoundPriority;
    public DashCollision OnDashCollide;
    public Action<Vector2> OnCollide;

    public Vector2 Shake
    {
      get
      {
        return this.shakeAmount;
      }
    }

    public Hitbox Hitbox
    {
      get
      {
        return this.Collider as Hitbox;
      }
    }

    public Vector2 ExactPosition
    {
      get
      {
        return Vector2.op_Addition(this.Position, this.movementCounter);
      }
    }

    public Platform(Vector2 position, bool safe)
      : base(position)
    {
      this.Safe = safe;
      this.Depth = -9000;
    }

    public void ClearRemainder()
    {
      this.movementCounter = Vector2.get_Zero();
    }

    public override void Update()
    {
      base.Update();
      this.LiftSpeed = Vector2.get_Zero();
      if (!this.shaking)
        return;
      if (this.Scene.OnInterval(0.04f))
      {
        Vector2 shakeAmount = this.shakeAmount;
        this.shakeAmount = Calc.Random.ShakeVector();
        this.OnShake(Vector2.op_Subtraction(this.shakeAmount, shakeAmount));
      }
      if ((double) this.shakeTimer <= 0.0)
        return;
      this.shakeTimer -= Engine.DeltaTime;
      if ((double) this.shakeTimer > 0.0)
        return;
      this.shaking = false;
      this.StopShaking();
    }

    public void StartShaking(float time = 0.0f)
    {
      this.shaking = true;
      this.shakeTimer = time;
    }

    public void StopShaking()
    {
      this.shaking = false;
      if (!Vector2.op_Inequality(this.shakeAmount, Vector2.get_Zero()))
        return;
      this.OnShake(Vector2.op_UnaryNegation(this.shakeAmount));
      this.shakeAmount = Vector2.get_Zero();
    }

    public virtual void OnShake(Vector2 amount)
    {
      this.ShakeStaticMovers(amount);
    }

    public void ShakeStaticMovers(Vector2 amount)
    {
      foreach (StaticMover staticMover in this.staticMovers)
        staticMover.Shake(amount);
    }

    public void MoveStaticMovers(Vector2 amount)
    {
      foreach (StaticMover staticMover in this.staticMovers)
        staticMover.Move(amount);
    }

    public void DestroyStaticMovers()
    {
      foreach (StaticMover staticMover in this.staticMovers)
        staticMover.Destroy();
      this.staticMovers.Clear();
    }

    public void DisableStaticMovers()
    {
      foreach (StaticMover staticMover in this.staticMovers)
        staticMover.Disable();
    }

    public void EnableStaticMovers()
    {
      foreach (StaticMover staticMover in this.staticMovers)
        staticMover.Enable();
    }

    public virtual void OnStaticMoverTrigger()
    {
    }

    public virtual int GetLandSoundIndex(Entity entity)
    {
      return this.SurfaceSoundIndex;
    }

    public virtual int GetWallSoundIndex(Player player, int side)
    {
      return this.SurfaceSoundIndex;
    }

    public virtual int GetStepSoundIndex(Entity entity)
    {
      return this.SurfaceSoundIndex;
    }

    public void MoveH(float moveH)
    {
      this.LiftSpeed.X = (double) Engine.DeltaTime != 0.0 ? (__Null) ((double) moveH / (double) Engine.DeltaTime) : (__Null) 0.0;
      ref __Null local1 = ref this.movementCounter.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 + moveH;
      int move = (int) Math.Round((double) this.movementCounter.X);
      if (move == 0)
        return;
      ref __Null local2 = ref this.movementCounter.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 - (float) move;
      this.MoveHExact(move);
    }

    public void MoveH(float moveH, float liftSpeedH)
    {
      this.LiftSpeed.X = (__Null) (double) liftSpeedH;
      ref __Null local1 = ref this.movementCounter.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 + moveH;
      int move = (int) Math.Round((double) this.movementCounter.X);
      if (move == 0)
        return;
      ref __Null local2 = ref this.movementCounter.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 - (float) move;
      this.MoveHExact(move);
    }

    public void MoveV(float moveV)
    {
      this.LiftSpeed.Y = (double) Engine.DeltaTime != 0.0 ? (__Null) ((double) moveV / (double) Engine.DeltaTime) : (__Null) 0.0;
      ref __Null local1 = ref this.movementCounter.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 + moveV;
      int move = (int) Math.Round((double) this.movementCounter.Y);
      if (move == 0)
        return;
      ref __Null local2 = ref this.movementCounter.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 - (float) move;
      this.MoveVExact(move);
    }

    public void MoveV(float moveV, float liftSpeedV)
    {
      this.LiftSpeed.Y = (__Null) (double) liftSpeedV;
      ref __Null local1 = ref this.movementCounter.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 + moveV;
      int move = (int) Math.Round((double) this.movementCounter.Y);
      if (move == 0)
        return;
      ref __Null local2 = ref this.movementCounter.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 - (float) move;
      this.MoveVExact(move);
    }

    public void MoveToX(float x)
    {
      this.MoveH(x - (float) this.ExactPosition.X);
    }

    public void MoveToX(float x, float liftSpeedX)
    {
      this.MoveH(x - (float) this.ExactPosition.X, liftSpeedX);
    }

    public void MoveToY(float y)
    {
      this.MoveV(y - (float) this.ExactPosition.Y);
    }

    public void MoveToY(float y, float liftSpeedY)
    {
      this.MoveV(y - (float) this.ExactPosition.Y, liftSpeedY);
    }

    public void MoveTo(Vector2 position)
    {
      this.MoveToX((float) position.X);
      this.MoveToY((float) position.Y);
    }

    public void MoveTo(Vector2 position, Vector2 liftSpeed)
    {
      this.MoveToX((float) position.X, (float) liftSpeed.X);
      this.MoveToY((float) position.Y, (float) liftSpeed.Y);
    }

    public void MoveTowardsX(float x, float amount)
    {
      this.MoveToX(Calc.Approach((float) this.ExactPosition.X, x, amount));
    }

    public void MoveTowardsY(float y, float amount)
    {
      this.MoveToY(Calc.Approach((float) this.ExactPosition.Y, y, amount));
    }

    public abstract void MoveHExact(int move);

    public abstract void MoveVExact(int move);

    public void MoveToNaive(Vector2 position)
    {
      this.MoveToXNaive((float) position.X);
      this.MoveToYNaive((float) position.Y);
    }

    public void MoveToXNaive(float x)
    {
      this.MoveHNaive(x - (float) this.ExactPosition.X);
    }

    public void MoveToYNaive(float y)
    {
      this.MoveVNaive(y - (float) this.ExactPosition.Y);
    }

    public void MoveHNaive(float moveH)
    {
      this.LiftSpeed.X = (double) Engine.DeltaTime != 0.0 ? (__Null) ((double) moveH / (double) Engine.DeltaTime) : (__Null) 0.0;
      ref __Null local1 = ref this.movementCounter.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 + moveH;
      int num = (int) Math.Round((double) this.movementCounter.X);
      if (num == 0)
        return;
      ref __Null local2 = ref this.movementCounter.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 - (float) num;
      this.X += (float) num;
      this.MoveStaticMovers(Vector2.op_Multiply(Vector2.get_UnitX(), (float) num));
    }

    public void MoveVNaive(float moveV)
    {
      this.LiftSpeed.Y = (double) Engine.DeltaTime != 0.0 ? (__Null) ((double) moveV / (double) Engine.DeltaTime) : (__Null) 0.0;
      ref __Null local1 = ref this.movementCounter.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 + moveV;
      int num = (int) Math.Round((double) this.movementCounter.Y);
      if (num == 0)
        return;
      ref __Null local2 = ref this.movementCounter.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 - (float) num;
      this.Y += (float) num;
      this.MoveStaticMovers(Vector2.op_Multiply(Vector2.get_UnitY(), (float) num));
    }

    public bool MoveHCollideSolids(
      float moveH,
      bool thruDashBlocks,
      Action<Vector2, Vector2, Platform> onCollide = null)
    {
      this.LiftSpeed.X = (double) Engine.DeltaTime != 0.0 ? (__Null) ((double) moveH / (double) Engine.DeltaTime) : (__Null) 0.0;
      ref __Null local1 = ref this.movementCounter.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 + moveH;
      int moveH1 = (int) Math.Round((double) this.movementCounter.X);
      if (moveH1 == 0)
        return false;
      ref __Null local2 = ref this.movementCounter.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 - (float) moveH1;
      return this.MoveHExactCollideSolids(moveH1, thruDashBlocks, onCollide);
    }

    public bool MoveVCollideSolids(
      float moveV,
      bool thruDashBlocks,
      Action<Vector2, Vector2, Platform> onCollide = null)
    {
      this.LiftSpeed.Y = (double) Engine.DeltaTime != 0.0 ? (__Null) ((double) moveV / (double) Engine.DeltaTime) : (__Null) 0.0;
      ref __Null local1 = ref this.movementCounter.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 + moveV;
      int moveV1 = (int) Math.Round((double) this.movementCounter.Y);
      if (moveV1 == 0)
        return false;
      ref __Null local2 = ref this.movementCounter.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 - (float) moveV1;
      return this.MoveVExactCollideSolids(moveV1, thruDashBlocks, onCollide);
    }

    public bool MoveHCollideSolidsAndBounds(
      Level level,
      float moveH,
      bool thruDashBlocks,
      Action<Vector2, Vector2, Platform> onCollide = null)
    {
      this.LiftSpeed.X = (double) Engine.DeltaTime != 0.0 ? (__Null) ((double) moveH / (double) Engine.DeltaTime) : (__Null) 0.0;
      ref __Null local1 = ref this.movementCounter.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 + moveH;
      int moveH1 = (int) Math.Round((double) this.movementCounter.X);
      if (moveH1 == 0)
        return false;
      ref __Null local2 = ref this.movementCounter.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 - (float) moveH1;
      double num1 = (double) this.Left + (double) moveH1;
      Rectangle bounds = level.Bounds;
      double left = (double) ((Rectangle) ref bounds).get_Left();
      bool flag;
      if (num1 < left)
      {
        flag = true;
        bounds = level.Bounds;
        moveH1 = ((Rectangle) ref bounds).get_Left() - (int) this.Left;
      }
      else
      {
        double num2 = (double) this.Right + (double) moveH1;
        bounds = level.Bounds;
        double right = (double) ((Rectangle) ref bounds).get_Right();
        if (num2 > right)
        {
          flag = true;
          bounds = level.Bounds;
          moveH1 = ((Rectangle) ref bounds).get_Right() - (int) this.Right;
        }
        else
          flag = false;
      }
      return this.MoveHExactCollideSolids(moveH1, thruDashBlocks, onCollide) | flag;
    }

    public bool MoveVCollideSolidsAndBounds(
      Level level,
      float moveV,
      bool thruDashBlocks,
      Action<Vector2, Vector2, Platform> onCollide = null)
    {
      this.LiftSpeed.Y = (double) Engine.DeltaTime != 0.0 ? (__Null) ((double) moveV / (double) Engine.DeltaTime) : (__Null) 0.0;
      ref __Null local1 = ref this.movementCounter.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 + moveV;
      int moveV1 = (int) Math.Round((double) this.movementCounter.Y);
      if (moveV1 == 0)
        return false;
      ref __Null local2 = ref this.movementCounter.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 - (float) moveV1;
      Rectangle bounds1 = level.Bounds;
      int num1 = ((Rectangle) ref bounds1).get_Bottom() + 32;
      double num2 = (double) this.Top + (double) moveV1;
      Rectangle bounds2 = level.Bounds;
      double top = (double) ((Rectangle) ref bounds2).get_Top();
      bool flag;
      if (num2 < top)
      {
        flag = true;
        Rectangle bounds3 = level.Bounds;
        moveV1 = ((Rectangle) ref bounds3).get_Top() - (int) this.Top;
      }
      else if ((double) this.Bottom + (double) moveV1 > (double) num1)
      {
        flag = true;
        moveV1 = num1 - (int) this.Bottom;
      }
      else
        flag = false;
      return this.MoveVExactCollideSolids(moveV1, thruDashBlocks, onCollide) | flag;
    }

    public bool MoveHExactCollideSolids(
      int moveH,
      bool thruDashBlocks,
      Action<Vector2, Vector2, Platform> onCollide = null)
    {
      float x = this.X;
      int num = Math.Sign(moveH);
      int move = 0;
      Solid solid = (Solid) null;
      while (moveH != 0)
      {
        if (thruDashBlocks)
        {
          foreach (DashBlock entity in this.Scene.Tracker.GetEntities<DashBlock>())
          {
            if (this.CollideCheck((Entity) entity, Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.get_UnitX(), (float) num))))
            {
              entity.Break(this.Center, Vector2.op_Multiply(Vector2.get_UnitX(), (float) num), true);
              this.SceneAs<Level>().Shake(0.2f);
              Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }
          }
        }
        solid = this.CollideFirst<Solid>(Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.get_UnitX(), (float) num)));
        if (solid == null)
        {
          move += num;
          moveH -= num;
          this.X += (float) num;
        }
        else
          break;
      }
      this.X = x;
      this.MoveHExact(move);
      if (solid != null && onCollide != null)
        onCollide(Vector2.op_Multiply(Vector2.get_UnitX(), (float) num), Vector2.op_Multiply(Vector2.get_UnitX(), (float) move), (Platform) solid);
      return solid != null;
    }

    public bool MoveVExactCollideSolids(
      int moveV,
      bool thruDashBlocks,
      Action<Vector2, Vector2, Platform> onCollide = null)
    {
      float y = this.Y;
      int num = Math.Sign(moveV);
      int move = 0;
      Platform platform = (Platform) null;
      while (moveV != 0)
      {
        if (thruDashBlocks)
        {
          foreach (DashBlock entity in this.Scene.Tracker.GetEntities<DashBlock>())
          {
            if (this.CollideCheck((Entity) entity, Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.get_UnitY(), (float) num))))
            {
              entity.Break(this.Center, Vector2.op_Multiply(Vector2.get_UnitY(), (float) num), true);
              this.SceneAs<Level>().Shake(0.2f);
              Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }
          }
        }
        platform = (Platform) this.CollideFirst<Solid>(Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.get_UnitY(), (float) num)));
        if (platform == null)
        {
          if (moveV > 0)
          {
            platform = (Platform) this.CollideFirstOutside<JumpThru>(Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.get_UnitY(), (float) num)));
            if (platform != null)
              break;
          }
          move += num;
          moveV -= num;
          this.Y += (float) num;
        }
        else
          break;
      }
      this.Y = y;
      this.MoveVExact(move);
      if (platform != null && onCollide != null)
        onCollide(Vector2.op_Multiply(Vector2.get_UnitY(), (float) num), Vector2.op_Multiply(Vector2.get_UnitY(), (float) move), platform);
      return platform != null;
    }
  }
}
