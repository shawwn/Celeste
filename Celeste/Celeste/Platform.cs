// Decompiled with JetBrains decompiler
// Type: Celeste.Platform
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(true)]
  public abstract class Platform : Entity
  {
    private Vector2 movementCounter;
    private Vector2 shakeAmount;
    private bool shaking;
    private float shakeTimer;
    protected List<StaticMover> staticMovers = new List<StaticMover>();
    public Vector2 LiftSpeed;
    public bool Safe;
    public bool BlockWaterfalls = true;
    public int SurfaceSoundIndex = 8;
    public int SurfaceSoundPriority;
    public DashCollision OnDashCollide;
    public Action<Vector2> OnCollide;

    public Vector2 Shake => this.shakeAmount;

    public Hitbox Hitbox => this.Collider as Hitbox;

    public Vector2 ExactPosition => this.Position + this.movementCounter;

    public Platform(Vector2 position, bool safe)
      : base(position)
    {
      this.Safe = safe;
      this.Depth = -9000;
    }

    public void ClearRemainder() => this.movementCounter = Vector2.Zero;

    public override void Update()
    {
      base.Update();
      this.LiftSpeed = Vector2.Zero;
      if (!this.shaking)
        return;
      if (this.Scene.OnInterval(0.04f))
      {
        Vector2 shakeAmount = this.shakeAmount;
        this.shakeAmount = Calc.Random.ShakeVector();
        this.OnShake(this.shakeAmount - shakeAmount);
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
      if (!(this.shakeAmount != Vector2.Zero))
        return;
      this.OnShake(-this.shakeAmount);
      this.shakeAmount = Vector2.Zero;
    }

    public virtual void OnShake(Vector2 amount) => this.ShakeStaticMovers(amount);

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

    public virtual void OnStaticMoverTrigger(StaticMover sm)
    {
    }

    public virtual int GetLandSoundIndex(Entity entity) => this.SurfaceSoundIndex;

    public virtual int GetWallSoundIndex(Player player, int side) => this.SurfaceSoundIndex;

    public virtual int GetStepSoundIndex(Entity entity) => this.SurfaceSoundIndex;

    public void MoveH(float moveH)
    {
      this.LiftSpeed.X = (double) Engine.DeltaTime != 0.0 ? moveH / Engine.DeltaTime : 0.0f;
      this.movementCounter.X += moveH;
      int move = (int) Math.Round((double) this.movementCounter.X);
      if (move == 0)
        return;
      this.movementCounter.X -= (float) move;
      this.MoveHExact(move);
    }

    public void MoveH(float moveH, float liftSpeedH)
    {
      this.LiftSpeed.X = liftSpeedH;
      this.movementCounter.X += moveH;
      int move = (int) Math.Round((double) this.movementCounter.X);
      if (move == 0)
        return;
      this.movementCounter.X -= (float) move;
      this.MoveHExact(move);
    }

    public void MoveV(float moveV)
    {
      this.LiftSpeed.Y = (double) Engine.DeltaTime != 0.0 ? moveV / Engine.DeltaTime : 0.0f;
      this.movementCounter.Y += moveV;
      int move = (int) Math.Round((double) this.movementCounter.Y);
      if (move == 0)
        return;
      this.movementCounter.Y -= (float) move;
      this.MoveVExact(move);
    }

    public void MoveV(float moveV, float liftSpeedV)
    {
      this.LiftSpeed.Y = liftSpeedV;
      this.movementCounter.Y += moveV;
      int move = (int) Math.Round((double) this.movementCounter.Y);
      if (move == 0)
        return;
      this.movementCounter.Y -= (float) move;
      this.MoveVExact(move);
    }

    public void MoveToX(float x) => this.MoveH(x - this.ExactPosition.X);

    public void MoveToX(float x, float liftSpeedX) => this.MoveH(x - this.ExactPosition.X, liftSpeedX);

    public void MoveToY(float y) => this.MoveV(y - this.ExactPosition.Y);

    public void MoveToY(float y, float liftSpeedY) => this.MoveV(y - this.ExactPosition.Y, liftSpeedY);

    public void MoveTo(Vector2 position)
    {
      this.MoveToX(position.X);
      this.MoveToY(position.Y);
    }

    public void MoveTo(Vector2 position, Vector2 liftSpeed)
    {
      this.MoveToX(position.X, liftSpeed.X);
      this.MoveToY(position.Y, liftSpeed.Y);
    }

    public void MoveTowardsX(float x, float amount) => this.MoveToX(Calc.Approach(this.ExactPosition.X, x, amount));

    public void MoveTowardsY(float y, float amount) => this.MoveToY(Calc.Approach(this.ExactPosition.Y, y, amount));

    public abstract void MoveHExact(int move);

    public abstract void MoveVExact(int move);

    public void MoveToNaive(Vector2 position)
    {
      this.MoveToXNaive(position.X);
      this.MoveToYNaive(position.Y);
    }

    public void MoveToXNaive(float x) => this.MoveHNaive(x - this.ExactPosition.X);

    public void MoveToYNaive(float y) => this.MoveVNaive(y - this.ExactPosition.Y);

    public void MoveHNaive(float moveH)
    {
      this.LiftSpeed.X = (double) Engine.DeltaTime != 0.0 ? moveH / Engine.DeltaTime : 0.0f;
      this.movementCounter.X += moveH;
      int num = (int) Math.Round((double) this.movementCounter.X);
      if (num == 0)
        return;
      this.movementCounter.X -= (float) num;
      this.X += (float) num;
      this.MoveStaticMovers(Vector2.UnitX * (float) num);
    }

    public void MoveVNaive(float moveV)
    {
      this.LiftSpeed.Y = (double) Engine.DeltaTime != 0.0 ? moveV / Engine.DeltaTime : 0.0f;
      this.movementCounter.Y += moveV;
      int num = (int) Math.Round((double) this.movementCounter.Y);
      if (num == 0)
        return;
      this.movementCounter.Y -= (float) num;
      this.Y += (float) num;
      this.MoveStaticMovers(Vector2.UnitY * (float) num);
    }

    public bool MoveHCollideSolids(
      float moveH,
      bool thruDashBlocks,
      Action<Vector2, Vector2, Platform> onCollide = null)
    {
      this.LiftSpeed.X = (double) Engine.DeltaTime != 0.0 ? moveH / Engine.DeltaTime : 0.0f;
      this.movementCounter.X += moveH;
      int moveH1 = (int) Math.Round((double) this.movementCounter.X);
      if (moveH1 == 0)
        return false;
      this.movementCounter.X -= (float) moveH1;
      return this.MoveHExactCollideSolids(moveH1, thruDashBlocks, onCollide);
    }

    public bool MoveVCollideSolids(
      float moveV,
      bool thruDashBlocks,
      Action<Vector2, Vector2, Platform> onCollide = null)
    {
      this.LiftSpeed.Y = (double) Engine.DeltaTime != 0.0 ? moveV / Engine.DeltaTime : 0.0f;
      this.movementCounter.Y += moveV;
      int moveV1 = (int) Math.Round((double) this.movementCounter.Y);
      if (moveV1 == 0)
        return false;
      this.movementCounter.Y -= (float) moveV1;
      return this.MoveVExactCollideSolids(moveV1, thruDashBlocks, onCollide);
    }

    public bool MoveHCollideSolidsAndBounds(
      Level level,
      float moveH,
      bool thruDashBlocks,
      Action<Vector2, Vector2, Platform> onCollide = null)
    {
      this.LiftSpeed.X = (double) Engine.DeltaTime != 0.0 ? moveH / Engine.DeltaTime : 0.0f;
      this.movementCounter.X += moveH;
      int moveH1 = (int) Math.Round((double) this.movementCounter.X);
      if (moveH1 == 0)
        return false;
      this.movementCounter.X -= (float) moveH1;
      double num1 = (double) this.Left + (double) moveH1;
      Rectangle bounds = level.Bounds;
      double left = (double) bounds.Left;
      bool flag;
      if (num1 < left)
      {
        flag = true;
        bounds = level.Bounds;
        moveH1 = bounds.Left - (int) this.Left;
      }
      else
      {
        double num2 = (double) this.Right + (double) moveH1;
        bounds = level.Bounds;
        double right = (double) bounds.Right;
        if (num2 > right)
        {
          flag = true;
          bounds = level.Bounds;
          moveH1 = bounds.Right - (int) this.Right;
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
      Action<Vector2, Vector2, Platform> onCollide = null,
      bool checkBottom = true)
    {
      this.LiftSpeed.Y = (double) Engine.DeltaTime != 0.0 ? moveV / Engine.DeltaTime : 0.0f;
      this.movementCounter.Y += moveV;
      int moveV1 = (int) Math.Round((double) this.movementCounter.Y);
      if (moveV1 == 0)
        return false;
      this.movementCounter.Y -= (float) moveV1;
      int num = level.Bounds.Bottom + 32;
      bool flag;
      if ((double) this.Top + (double) moveV1 < (double) level.Bounds.Top)
      {
        flag = true;
        moveV1 = level.Bounds.Top - (int) this.Top;
      }
      else if (checkBottom && (double) this.Bottom + (double) moveV1 > (double) num)
      {
        flag = true;
        moveV1 = num - (int) this.Bottom;
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
            if (this.CollideCheck((Entity) entity, this.Position + Vector2.UnitX * (float) num))
            {
              entity.Break(this.Center, Vector2.UnitX * (float) num);
              this.SceneAs<Level>().Shake(0.2f);
              Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }
          }
        }
        solid = this.CollideFirst<Solid>(this.Position + Vector2.UnitX * (float) num);
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
        onCollide(Vector2.UnitX * (float) num, Vector2.UnitX * (float) move, (Platform) solid);
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
            if (this.CollideCheck((Entity) entity, this.Position + Vector2.UnitY * (float) num))
            {
              entity.Break(this.Center, Vector2.UnitY * (float) num);
              this.SceneAs<Level>().Shake(0.2f);
              Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }
          }
        }
        platform = (Platform) this.CollideFirst<Solid>(this.Position + Vector2.UnitY * (float) num);
        if (platform == null)
        {
          if (moveV > 0)
          {
            platform = (Platform) this.CollideFirstOutside<JumpThru>(this.Position + Vector2.UnitY * (float) num);
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
        onCollide(Vector2.UnitY * (float) num, Vector2.UnitY * (float) move, platform);
      return platform != null;
    }
  }
}
