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
          if (index1 != 0 || index2 != 0)
          {
            for (int index3 = 1; index3 >= -1; index3 -= 2)
            {
              for (int index4 = 1; index4 >= -1; index4 -= 2)
              {
                Vector2 vector2;
                ((Vector2) ref vector2).\u002Ector((float) (index1 * index3), (float) (index2 * index4));
                if (!this.CollideCheck<Solid>(Vector2.op_Addition(this.Position, vector2)))
                {
                  this.Position = Vector2.op_Addition(this.Position, vector2);
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
      return this.CollideCheckOutside((Entity) jumpThru, Vector2.op_Addition(this.Position, Vector2.get_UnitY()));
    }

    public virtual bool IsRiding(Solid solid)
    {
      return this.CollideCheck((Entity) solid, Vector2.op_Addition(this.Position, Vector2.get_UnitY()));
    }

    public bool OnGround(int downCheck = 1)
    {
      if (this.CollideCheck<Solid>(Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.get_UnitY(), (float) downCheck))))
        return true;
      if (!this.IgnoreJumpThrus)
        return this.CollideCheckOutside<JumpThru>(Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.get_UnitY(), (float) downCheck)));
      return false;
    }

    public bool OnGround(Vector2 at, int downCheck = 1)
    {
      Vector2 position = this.Position;
      this.Position = at;
      int num = this.OnGround(downCheck) ? 1 : 0;
      this.Position = position;
      return num != 0;
    }

    public Vector2 ExactPosition
    {
      get
      {
        return Vector2.op_Addition(this.Position, this.movementCounter);
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
      this.movementCounter.X = (__Null) 0.0;
    }

    public void ZeroRemainderY()
    {
      this.movementCounter.Y = (__Null) 0.0;
    }

    public override void Update()
    {
      base.Update();
      this.LiftSpeed = Vector2.get_Zero();
      if ((double) this.liftSpeedTimer <= 0.0)
        return;
      this.liftSpeedTimer -= Engine.DeltaTime;
      if ((double) this.liftSpeedTimer > 0.0)
        return;
      this.lastLiftSpeed = Vector2.get_Zero();
    }

    public Vector2 LiftSpeed
    {
      set
      {
        this.currentLiftSpeed = value;
        if (!Vector2.op_Inequality(value, Vector2.get_Zero()) || (double) this.LiftSpeedGraceTime <= 0.0)
          return;
        this.lastLiftSpeed = value;
        this.liftSpeedTimer = this.LiftSpeedGraceTime;
      }
      get
      {
        if (Vector2.op_Equality(this.currentLiftSpeed, Vector2.get_Zero()))
          return this.lastLiftSpeed;
        return this.currentLiftSpeed;
      }
    }

    public void ResetLiftSpeed()
    {
      this.currentLiftSpeed = this.lastLiftSpeed = Vector2.get_Zero();
      this.liftSpeedTimer = 0.0f;
    }

    public bool MoveH(float moveH, Collision onCollide = null, Solid pusher = null)
    {
      ref __Null local1 = ref this.movementCounter.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 + moveH;
      int moveH1 = (int) Math.Round((double) this.movementCounter.X, MidpointRounding.ToEven);
      if (moveH1 == 0)
        return false;
      ref __Null local2 = ref this.movementCounter.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 - (float) moveH1;
      return this.MoveHExact(moveH1, onCollide, pusher);
    }

    public bool MoveV(float moveV, Collision onCollide = null, Solid pusher = null)
    {
      ref __Null local1 = ref this.movementCounter.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 + moveV;
      int moveV1 = (int) Math.Round((double) this.movementCounter.Y, MidpointRounding.ToEven);
      if (moveV1 == 0)
        return false;
      ref __Null local2 = ref this.movementCounter.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 - (float) moveV1;
      return this.MoveVExact(moveV1, onCollide, pusher);
    }

    public bool MoveHExact(int moveH, Collision onCollide = null, Solid pusher = null)
    {
      Vector2 vector2 = Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.get_UnitX(), (float) moveH));
      int num1 = Math.Sign(moveH);
      int num2 = 0;
      while (moveH != 0)
      {
        Solid solid = this.CollideFirst<Solid>(Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.get_UnitX(), (float) num1)));
        if (solid != null)
        {
          this.movementCounter.X = (__Null) 0.0;
          if (onCollide != null)
            onCollide(new CollisionData()
            {
              Direction = Vector2.op_Multiply(Vector2.get_UnitX(), (float) num1),
              Moved = Vector2.op_Multiply(Vector2.get_UnitX(), (float) num2),
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
      Vector2 vector2 = Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.get_UnitY(), (float) moveV));
      int num1 = Math.Sign(moveV);
      int num2 = 0;
      while (moveV != 0)
      {
        Platform platform1 = (Platform) this.CollideFirst<Solid>(Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.get_UnitY(), (float) num1)));
        if (platform1 != null)
        {
          this.movementCounter.Y = (__Null) 0.0;
          if (onCollide != null)
            onCollide(new CollisionData()
            {
              Direction = Vector2.op_Multiply(Vector2.get_UnitY(), (float) num1),
              Moved = Vector2.op_Multiply(Vector2.get_UnitY(), (float) num2),
              TargetPosition = vector2,
              Hit = platform1,
              Pusher = pusher
            });
          return true;
        }
        if (moveV > 0 && !this.IgnoreJumpThrus)
        {
          Platform platform2 = (Platform) this.CollideFirstOutside<JumpThru>(Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.get_UnitY(), (float) num1)));
          if (platform2 != null)
          {
            this.movementCounter.Y = (__Null) 0.0;
            if (onCollide != null)
              onCollide(new CollisionData()
              {
                Direction = Vector2.op_Multiply(Vector2.get_UnitY(), (float) num1),
                Moved = Vector2.op_Multiply(Vector2.get_UnitY(), (float) num2),
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
      this.MoveToX(Calc.Approach((float) this.ExactPosition.X, targetX, maxAmount), onCollide);
    }

    public void MoveTowardsY(float targetY, float maxAmount, Collision onCollide = null)
    {
      this.MoveToY(Calc.Approach((float) this.ExactPosition.Y, targetY, maxAmount), onCollide);
    }

    public void MoveToX(float toX, Collision onCollide = null)
    {
      this.MoveH(toX - (float) this.ExactPosition.X, onCollide, (Solid) null);
    }

    public void MoveToY(float toY, Collision onCollide = null)
    {
      this.MoveV(toY - (float) this.ExactPosition.Y, onCollide, (Solid) null);
    }

    public void NaiveMove(Vector2 amount)
    {
      this.movementCounter = Vector2.op_Addition(this.movementCounter, amount);
      int num1 = (int) Math.Round((double) this.movementCounter.X);
      int num2 = (int) Math.Round((double) this.movementCounter.Y);
      this.Position = Vector2.op_Addition(this.Position, new Vector2((float) num1, (float) num2));
      this.movementCounter = Vector2.op_Subtraction(this.movementCounter, new Vector2((float) num1, (float) num2));
    }
  }
}
