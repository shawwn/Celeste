// Decompiled with JetBrains decompiler
// Type: Celeste.Actor
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(true)]
  public class Actor : Entity
  {
    public bool AllowPushing = true;
    public float LiftSpeedGraceTime = 0.16f;
    public Collision SquishCallback;
    public bool TreatNaive;
    private Vector2 movementCounter;
    public bool IgnoreJumpThrus;
    private Vector2 currentLiftSpeed;
    private Vector2 lastLiftSpeed;
    private float liftSpeedTimer;

    public Actor(Vector2 position)
      : base(position)
    {
      this.SquishCallback = new Collision(this.OnSquish);
    }

    protected virtual void OnSquish(CollisionData data)
    {
      if (this.TrySquishWiggle(data))
        return;
      this.RemoveSelf();
    }

    protected bool TrySquishWiggle(CollisionData data)
    {
      data.Pusher.Collidable = true;
      for (int index1 = 0; index1 <= 3; ++index1)
      {
        for (int index2 = 0; index2 <= 3; ++index2)
        {
          if (index1 != 0 || (uint) index2 > 0U)
          {
            for (int index3 = 1; index3 >= -1; index3 -= 2)
            {
              for (int index4 = 1; index4 >= -1; index4 -= 2)
              {
                Vector2 vector2 = new Vector2((float) (index1 * index3), (float) (index2 * index4));
                if (!this.CollideCheck<Solid>(this.Position + vector2))
                {
                  this.Position = this.Position + vector2;
                  data.Pusher.Collidable = false;
                  return true;
                }
              }
            }
          }
        }
      }
      data.Pusher.Collidable = false;
      return false;
    }

    public virtual bool IsRiding(JumpThru jumpThru)
    {
      if (this.IgnoreJumpThrus)
        return false;
      return this.CollideCheckOutside((Entity) jumpThru, this.Position + Vector2.UnitY);
    }

    public virtual bool IsRiding(Solid solid)
    {
      return this.CollideCheck((Entity) solid, this.Position + Vector2.UnitY);
    }

    public bool OnGround(int downCheck = 1)
    {
      return this.CollideCheck<Solid>(this.Position + Vector2.UnitY * (float) downCheck) || !this.IgnoreJumpThrus && this.CollideCheckOutside<JumpThru>(this.Position + Vector2.UnitY * (float) downCheck);
    }

    public bool OnGround(Vector2 at, int downCheck = 1)
    {
      Vector2 position = this.Position;
      this.Position = at;
      bool flag = this.OnGround(downCheck);
      this.Position = position;
      return flag;
    }

    public Vector2 ExactPosition
    {
      get
      {
        return this.Position + this.movementCounter;
      }
    }

    public Vector2 PositionRemainder
    {
      get
      {
        return this.movementCounter;
      }
    }

    public void ZeroRemainderX()
    {
      this.movementCounter.X = 0.0f;
    }

    public void ZeroRemainderY()
    {
      this.movementCounter.Y = 0.0f;
    }

    public override void Update()
    {
      base.Update();
      this.LiftSpeed = Vector2.Zero;
      if ((double) this.liftSpeedTimer <= 0.0)
        return;
      this.liftSpeedTimer -= Engine.DeltaTime;
      if ((double) this.liftSpeedTimer <= 0.0)
        this.lastLiftSpeed = Vector2.Zero;
    }

    public Vector2 LiftSpeed
    {
      set
      {
        this.currentLiftSpeed = value;
        if (!(value != Vector2.Zero) || (double) this.LiftSpeedGraceTime <= 0.0)
          return;
        this.lastLiftSpeed = value;
        this.liftSpeedTimer = this.LiftSpeedGraceTime;
      }
      get
      {
        if (this.currentLiftSpeed == Vector2.Zero)
          return this.lastLiftSpeed;
        return this.currentLiftSpeed;
      }
    }

    public void ResetLiftSpeed()
    {
      this.currentLiftSpeed = this.lastLiftSpeed = Vector2.Zero;
      this.liftSpeedTimer = 0.0f;
    }

    public bool MoveH(float moveH, Collision onCollide = null, Solid pusher = null)
    {
      this.movementCounter.X += moveH;
      int moveH1 = (int) Math.Round((double) this.movementCounter.X, MidpointRounding.ToEven);
      if ((uint) moveH1 <= 0U)
        return false;
      this.movementCounter.X -= (float) moveH1;
      return this.MoveHExact(moveH1, onCollide, pusher);
    }

    public bool MoveV(float moveV, Collision onCollide = null, Solid pusher = null)
    {
      this.movementCounter.Y += moveV;
      int moveV1 = (int) Math.Round((double) this.movementCounter.Y, MidpointRounding.ToEven);
      if ((uint) moveV1 <= 0U)
        return false;
      this.movementCounter.Y -= (float) moveV1;
      return this.MoveVExact(moveV1, onCollide, pusher);
    }

    public bool MoveHExact(int moveH, Collision onCollide = null, Solid pusher = null)
    {
      Vector2 vector2 = this.Position + Vector2.UnitX * (float) moveH;
      int num1 = Math.Sign(moveH);
      int num2 = 0;
      while ((uint) moveH > 0U)
      {
        Solid solid = this.CollideFirst<Solid>(this.Position + Vector2.UnitX * (float) num1);
        if (solid != null)
        {
          this.movementCounter.X = 0.0f;
          if (onCollide != null)
            onCollide(new CollisionData()
            {
              Direction = Vector2.UnitX * (float) num1,
              Moved = Vector2.UnitX * (float) num2,
              TargetPosition = vector2,
              Hit = (Platform) solid,
              Pusher = pusher
            });
          return true;
        }
        num2 += num1;
        moveH -= num1;
        this.X += (float) num1;
      }
      return false;
    }

    public bool MoveVExact(int moveV, Collision onCollide = null, Solid pusher = null)
    {
      Vector2 vector2 = this.Position + Vector2.UnitY * (float) moveV;
      int num1 = Math.Sign(moveV);
      int num2 = 0;
      while ((uint) moveV > 0U)
      {
        Platform platform1 = (Platform) this.CollideFirst<Solid>(this.Position + Vector2.UnitY * (float) num1);
        if (platform1 != null)
        {
          this.movementCounter.Y = 0.0f;
          if (onCollide != null)
            onCollide(new CollisionData()
            {
              Direction = Vector2.UnitY * (float) num1,
              Moved = Vector2.UnitY * (float) num2,
              TargetPosition = vector2,
              Hit = platform1,
              Pusher = pusher
            });
          return true;
        }
        if (moveV > 0 && !this.IgnoreJumpThrus)
        {
          Platform platform2 = (Platform) this.CollideFirstOutside<JumpThru>(this.Position + Vector2.UnitY * (float) num1);
          if (platform2 != null)
          {
            this.movementCounter.Y = 0.0f;
            if (onCollide != null)
              onCollide(new CollisionData()
              {
                Direction = Vector2.UnitY * (float) num1,
                Moved = Vector2.UnitY * (float) num2,
                TargetPosition = vector2,
                Hit = platform2,
                Pusher = pusher
              });
            return true;
          }
        }
        num2 += num1;
        moveV -= num1;
        this.Y += (float) num1;
      }
      return false;
    }

    public void MoveTowardsX(float targetX, float maxAmount, Collision onCollide = null)
    {
      this.MoveToX(Calc.Approach(this.ExactPosition.X, targetX, maxAmount), onCollide);
    }

    public void MoveTowardsY(float targetY, float maxAmount, Collision onCollide = null)
    {
      this.MoveToY(Calc.Approach(this.ExactPosition.Y, targetY, maxAmount), onCollide);
    }

    public void MoveToX(float toX, Collision onCollide = null)
    {
      this.MoveH(toX - this.ExactPosition.X, onCollide, (Solid) null);
    }

    public void MoveToY(float toY, Collision onCollide = null)
    {
      this.MoveV(toY - this.ExactPosition.Y, onCollide, (Solid) null);
    }

    public void NaiveMove(Vector2 amount)
    {
      this.movementCounter += amount;
      int num1 = (int) Math.Round((double) this.movementCounter.X);
      int num2 = (int) Math.Round((double) this.movementCounter.Y);
      this.Position = this.Position + new Vector2((float) num1, (float) num2);
      this.movementCounter -= new Vector2((float) num1, (float) num2);
    }
  }
}

