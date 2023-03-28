// Decompiled with JetBrains decompiler
// Type: Celeste.Actor
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(true)]
  public class Actor : Entity
  {
    public Collision SquishCallback;
    public bool TreatNaive;
    private Vector2 movementCounter;
    public bool IgnoreJumpThrus;
    public bool AllowPushing = true;
    public float LiftSpeedGraceTime = 0.16f;
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

    protected bool TrySquishWiggle(CollisionData data, int wiggleX = 3, int wiggleY = 3)
    {
      data.Pusher.Collidable = true;
      for (int index1 = 0; index1 <= wiggleX; ++index1)
      {
        for (int index2 = 0; index2 <= wiggleY; ++index2)
        {
          if (index1 != 0 || index2 != 0)
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
      for (int index5 = 0; index5 <= wiggleX; ++index5)
      {
        for (int index6 = 0; index6 <= wiggleY; ++index6)
        {
          if (index5 != 0 || index6 != 0)
          {
            for (int index7 = 1; index7 >= -1; index7 -= 2)
            {
              for (int index8 = 1; index8 >= -1; index8 -= 2)
              {
                Vector2 vector2 = new Vector2((float) (index5 * index7), (float) (index6 * index8));
                if (!this.CollideCheck<Solid>(data.TargetPosition + vector2))
                {
                  this.Position = data.TargetPosition + vector2;
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

    public virtual bool IsRiding(JumpThru jumpThru) => !this.IgnoreJumpThrus && this.CollideCheckOutside((Entity) jumpThru, this.Position + Vector2.UnitY);

    public virtual bool IsRiding(Solid solid) => this.CollideCheck((Entity) solid, this.Position + Vector2.UnitY);

    public bool OnGround(int downCheck = 1)
    {
      if (this.CollideCheck<Solid>(this.Position + Vector2.UnitY * (float) downCheck))
        return true;
      return !this.IgnoreJumpThrus && this.CollideCheckOutside<JumpThru>(this.Position + Vector2.UnitY * (float) downCheck);
    }

    public bool OnGround(Vector2 at, int downCheck = 1)
    {
      Vector2 position = this.Position;
      this.Position = at;
      int num = this.OnGround(downCheck) ? 1 : 0;
      this.Position = position;
      return num != 0;
    }

    public Vector2 ExactPosition => this.Position + this.movementCounter;

    public Vector2 PositionRemainder => this.movementCounter;

    public void ZeroRemainderX() => this.movementCounter.X = 0.0f;

    public void ZeroRemainderY() => this.movementCounter.Y = 0.0f;

    public override void Update()
    {
      base.Update();
      this.LiftSpeed = Vector2.Zero;
      if ((double) this.liftSpeedTimer <= 0.0)
        return;
      this.liftSpeedTimer -= Engine.DeltaTime;
      if ((double) this.liftSpeedTimer > 0.0)
        return;
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
      get => this.currentLiftSpeed == Vector2.Zero ? this.lastLiftSpeed : this.currentLiftSpeed;
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
      if (moveH1 == 0)
        return false;
      this.movementCounter.X -= (float) moveH1;
      return this.MoveHExact(moveH1, onCollide, pusher);
    }

    public bool MoveV(float moveV, Collision onCollide = null, Solid pusher = null)
    {
      this.movementCounter.Y += moveV;
      int moveV1 = (int) Math.Round((double) this.movementCounter.Y, MidpointRounding.ToEven);
      if (moveV1 == 0)
        return false;
      this.movementCounter.Y -= (float) moveV1;
      return this.MoveVExact(moveV1, onCollide, pusher);
    }

    public bool MoveHExact(int moveH, Collision onCollide = null, Solid pusher = null)
    {
      Vector2 vector2 = this.Position + Vector2.UnitX * (float) moveH;
      int num1 = Math.Sign(moveH);
      int num2 = 0;
      while (moveH != 0)
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
      while (moveV != 0)
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

    public void MoveTowardsX(float targetX, float maxAmount, Collision onCollide = null) => this.MoveToX(Calc.Approach(this.ExactPosition.X, targetX, maxAmount), onCollide);

    public void MoveTowardsY(float targetY, float maxAmount, Collision onCollide = null) => this.MoveToY(Calc.Approach(this.ExactPosition.Y, targetY, maxAmount), onCollide);

    public void MoveToX(float toX, Collision onCollide = null) => this.MoveH(toX - this.ExactPosition.X, onCollide);

    public void MoveToY(float toY, Collision onCollide = null) => this.MoveV(toY - this.ExactPosition.Y, onCollide);

    public void NaiveMove(Vector2 amount)
    {
      this.movementCounter += amount;
      int x = (int) Math.Round((double) this.movementCounter.X);
      int y = (int) Math.Round((double) this.movementCounter.Y);
      this.Position = this.Position + new Vector2((float) x, (float) y);
      this.movementCounter -= new Vector2((float) x, (float) y);
    }
  }
}
