// Decompiled with JetBrains decompiler
// Type: Celeste.TheoCrystal
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  [Tracked(false)]
  public class TheoCrystal : Actor
  {
    public static ParticleType P_Impact;
    public Vector2 Speed;
    public bool OnPedestal;
    public Holdable Hold;
    private Sprite sprite;
    private bool dead;
    private Level Level;
    private Collision onCollideH;
    private Collision onCollideV;
    private float noGravityTimer;
    private Vector2 prevLiftSpeed;
    private Vector2 previousPosition;
    private HoldableCollider hitSeeker;
    private float swatTimer;
    private bool shattering;
    private float hardVerticalHitSoundCooldown;
    private BirdTutorialGui tutorialGui;
    private float tutorialTimer;

    public TheoCrystal(Vector2 position)
      : base(position)
    {
      this.previousPosition = position;
      this.Depth = 100;
      this.Collider = (Collider) new Hitbox(8f, 10f, -4f, -10f);
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("theo_crystal")));
      this.sprite.Scale.X = (__Null) -1.0;
      this.Add((Component) (this.Hold = new Holdable()));
      this.Hold.PickupCollider = (Collider) new Hitbox(16f, 22f, -8f, -16f);
      this.Hold.SlowRun = true;
      this.Hold.OnPickup = new Action(this.OnPickup);
      this.Hold.OnRelease = new Action<Vector2>(this.OnRelease);
      this.Hold.DangerousCheck = new Func<HoldableCollider, bool>(this.Dangerous);
      this.Hold.OnHitSeeker = new Action<Seeker>(this.HitSeeker);
      this.Hold.OnSwat = new Action<HoldableCollider, int>(this.Swat);
      this.Hold.OnHitSpring = new Func<Spring, bool>(this.HitSpring);
      this.Hold.OnHitSpinner = new Action<Entity>(this.HitSpinner);
      this.Hold.SpeedGetter = (Func<Vector2>) (() => this.Speed);
      this.onCollideH = new Collision(this.OnCollideH);
      this.onCollideV = new Collision(this.OnCollideV);
      this.LiftSpeedGraceTime = 0.1f;
      this.Add((Component) new VertexLight(this.Collider.Center, Color.get_White(), 1f, 32, 64));
      this.Tag = (int) Tags.TransitionUpdate;
      this.Add((Component) new MirrorReflection());
    }

    public TheoCrystal(EntityData e, Vector2 offset)
      : this(Vector2.op_Addition(e.Position, offset))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.Level = this.SceneAs<Level>();
      foreach (TheoCrystal entity in this.Level.Tracker.GetEntities<TheoCrystal>())
      {
        if (entity != this && entity.Hold.IsHeld)
          this.RemoveSelf();
      }
      if (!(this.Level.Session.Level == "e-00"))
        return;
      this.tutorialGui = new BirdTutorialGui((Entity) this, new Vector2(0.0f, -24f), (object) Dialog.Clean("tutorial_carry", (Language) null), new object[2]
      {
        (object) Dialog.Clean("tutorial_hold", (Language) null),
        (object) Input.Grab
      });
      this.tutorialGui.Open = false;
      this.Scene.Add((Entity) this.tutorialGui);
    }

    public override void Update()
    {
      base.Update();
      if (this.shattering || this.dead)
        return;
      if ((double) this.swatTimer > 0.0)
        this.swatTimer -= Engine.DeltaTime;
      this.hardVerticalHitSoundCooldown -= Engine.DeltaTime;
      if (this.OnPedestal)
      {
        this.Depth = 8999;
      }
      else
      {
        this.Depth = 100;
        if (this.Hold.IsHeld)
        {
          this.prevLiftSpeed = Vector2.get_Zero();
        }
        else
        {
          if (this.OnGround(1))
          {
            this.Speed.X = (__Null) (double) Calc.Approach((float) this.Speed.X, this.OnGround(Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.get_UnitX(), 3f)), 1) ? (this.OnGround(Vector2.op_Subtraction(this.Position, Vector2.op_Multiply(Vector2.get_UnitX(), 3f)), 1) ? 0.0f : -20f) : 20f, 800f * Engine.DeltaTime);
            Vector2 liftSpeed = this.LiftSpeed;
            if (Vector2.op_Equality(liftSpeed, Vector2.get_Zero()) && Vector2.op_Inequality(this.prevLiftSpeed, Vector2.get_Zero()))
            {
              this.Speed = this.prevLiftSpeed;
              this.prevLiftSpeed = Vector2.get_Zero();
              this.Speed.Y = (__Null) (double) Math.Min((float) (this.Speed.Y * 0.600000023841858), 0.0f);
              if (this.Speed.X != 0.0 && this.Speed.Y == 0.0)
                this.Speed.Y = (__Null) -60.0;
              if (this.Speed.Y < 0.0)
                this.noGravityTimer = 0.15f;
            }
            else
            {
              this.prevLiftSpeed = liftSpeed;
              if (liftSpeed.Y < 0.0 && this.Speed.Y < 0.0)
                this.Speed.Y = (__Null) 0.0;
            }
          }
          else if (this.Hold.ShouldHaveGravity)
          {
            float num1 = 800f;
            if ((double) Math.Abs((float) this.Speed.Y) <= 30.0)
              num1 *= 0.5f;
            float num2 = 350f;
            if (this.Speed.Y < 0.0)
              num2 *= 0.5f;
            this.Speed.X = (__Null) (double) Calc.Approach((float) this.Speed.X, 0.0f, num2 * Engine.DeltaTime);
            if ((double) this.noGravityTimer > 0.0)
              this.noGravityTimer -= Engine.DeltaTime;
            else
              this.Speed.Y = (__Null) (double) Calc.Approach((float) this.Speed.Y, 200f, num1 * Engine.DeltaTime);
          }
          this.previousPosition = this.ExactPosition;
          this.MoveH((float) this.Speed.X * Engine.DeltaTime, this.onCollideH, (Solid) null);
          this.MoveV((float) this.Speed.Y * Engine.DeltaTime, this.onCollideV, (Solid) null);
          // ISSUE: variable of the null type
          __Null x1 = this.Center.X;
          Rectangle bounds1 = this.Level.Bounds;
          double right1 = (double) ((Rectangle) ref bounds1).get_Right();
          if (x1 > right1)
          {
            this.MoveH(32f * Engine.DeltaTime, (Collision) null, (Solid) null);
            double num = (double) this.Left - 8.0;
            Rectangle bounds2 = this.Level.Bounds;
            double right2 = (double) ((Rectangle) ref bounds2).get_Right();
            if (num > right2)
              this.RemoveSelf();
          }
          else
          {
            double left1 = (double) this.Left;
            Rectangle bounds2 = this.Level.Bounds;
            double left2 = (double) ((Rectangle) ref bounds2).get_Left();
            if (left1 < left2)
            {
              Rectangle bounds3 = this.Level.Bounds;
              this.Left = (float) ((Rectangle) ref bounds3).get_Left();
              ref __Null local = ref this.Speed.X;
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              // ISSUE: cast to a reference type
              // ISSUE: explicit reference operation
              ^(float&) ref local = ^(float&) ref local * -0.4f;
            }
            else
            {
              double top1 = (double) this.Top;
              Rectangle bounds3 = this.Level.Bounds;
              double num = (double) (((Rectangle) ref bounds3).get_Top() - 4);
              if (top1 < num)
              {
                Rectangle bounds4 = this.Level.Bounds;
                this.Top = (float) (((Rectangle) ref bounds4).get_Top() + 4);
                this.Speed.Y = (__Null) 0.0;
              }
              else
              {
                double bottom1 = (double) this.Bottom;
                Rectangle bounds4 = this.Level.Bounds;
                double bottom2 = (double) ((Rectangle) ref bounds4).get_Bottom();
                if (bottom1 > bottom2 && SaveData.Instance.Assists.Invincible)
                {
                  Rectangle bounds5 = this.Level.Bounds;
                  this.Bottom = (float) ((Rectangle) ref bounds5).get_Bottom();
                  this.Speed.Y = (__Null) -300.0;
                  Audio.Play("event:/game/general/assist_screenbottom", this.Position);
                }
                else
                {
                  double top2 = (double) this.Top;
                  Rectangle bounds5 = this.Level.Bounds;
                  double bottom3 = (double) ((Rectangle) ref bounds5).get_Bottom();
                  if (top2 > bottom3)
                    this.Die();
                }
              }
            }
          }
          double x2 = (double) this.X;
          Rectangle bounds6 = this.Level.Bounds;
          double num3 = (double) (((Rectangle) ref bounds6).get_Left() + 10);
          if (x2 < num3)
            this.MoveH(32f * Engine.DeltaTime, (Collision) null, (Solid) null);
          Player entity = this.Scene.Tracker.GetEntity<Player>();
          TempleGate templeGate = this.CollideFirst<TempleGate>();
          if (templeGate != null && entity != null)
          {
            templeGate.Collidable = false;
            this.MoveH((float) (Math.Sign(entity.X - this.X) * 32) * Engine.DeltaTime, (Collision) null, (Solid) null);
            templeGate.Collidable = true;
          }
        }
        if (!this.dead)
          this.Hold.CheckAgainstColliders();
        if (this.hitSeeker != null && (double) this.swatTimer <= 0.0 && !this.hitSeeker.Check(this.Hold))
          this.hitSeeker = (HoldableCollider) null;
        if (this.tutorialGui == null)
          return;
        if (!this.OnPedestal && !this.Hold.IsHeld && (this.OnGround(1) && this.Level.Session.GetFlag("foundTheoInCrystal")))
          this.tutorialTimer += Engine.DeltaTime;
        else
          this.tutorialTimer = 0.0f;
        this.tutorialGui.Open = (double) this.tutorialTimer > 0.25;
      }
    }

    public IEnumerator Shatter()
    {
      TheoCrystal theoCrystal = this;
      theoCrystal.shattering = true;
      BloomPoint bloom = new BloomPoint(0.0f, 32f);
      VertexLight light = new VertexLight(Color.get_AliceBlue(), 0.0f, 64, 200);
      theoCrystal.Add((Component) bloom);
      theoCrystal.Add((Component) light);
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        theoCrystal.Position = Vector2.op_Addition(theoCrystal.Position, Vector2.op_Multiply(Vector2.op_Multiply(theoCrystal.Speed, 1f - p), Engine.DeltaTime));
        theoCrystal.Level.ZoomFocusPoint = Vector2.op_Subtraction(theoCrystal.TopCenter, theoCrystal.Level.Camera.Position);
        light.Alpha = p;
        bloom.Alpha = p;
        yield return (object) null;
      }
      yield return (object) 0.5f;
      theoCrystal.Level.Shake(0.3f);
      theoCrystal.sprite.Play("shatter", false, false);
      yield return (object) 1f;
      theoCrystal.Level.Shake(0.3f);
    }

    public void ExplodeLaunch(Vector2 from)
    {
      if (this.Hold.IsHeld)
        return;
      this.Speed = Vector2.op_Subtraction(this.Center, from).SafeNormalize(120f);
      SlashFx.Burst(this.Center, this.Speed.Angle());
    }

    public void Swat(HoldableCollider hc, int dir)
    {
      if (!this.Hold.IsHeld || this.hitSeeker != null)
        return;
      this.swatTimer = 0.1f;
      this.hitSeeker = hc;
      this.Hold.Holder.Swat(dir);
    }

    public bool Dangerous(HoldableCollider holdableCollider)
    {
      if (!this.Hold.IsHeld && Vector2.op_Inequality(this.Speed, Vector2.get_Zero()))
        return this.hitSeeker != holdableCollider;
      return false;
    }

    public void HitSeeker(Seeker seeker)
    {
      if (!this.Hold.IsHeld)
        this.Speed = Vector2.op_Subtraction(this.Center, seeker.Center).SafeNormalize(120f);
      Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_side", this.Position);
    }

    public void HitSpinner(Entity spinner)
    {
      if (this.Hold.IsHeld || !Vector2.op_Equality(this.Speed, Vector2.get_Zero()) || (!Vector2.op_Equality(this.LiftSpeed, Vector2.get_Zero()) || !Vector2.op_Equality(this.previousPosition, this.ExactPosition)) || !this.OnGround(1))
        return;
      int num = Math.Sign(this.X - spinner.X);
      if (num == 0)
        num = 1;
      this.Speed.X = (__Null) ((double) num * 120.0);
      this.Speed.Y = (__Null) -30.0;
    }

    public bool HitSpring(Spring spring)
    {
      if (!this.Hold.IsHeld)
      {
        if (spring.Orientation == Spring.Orientations.Floor && this.Speed.Y >= 0.0)
        {
          ref __Null local = ref this.Speed.X;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local * 0.5f;
          this.Speed.Y = (__Null) -160.0;
          this.noGravityTimer = 0.15f;
          return true;
        }
        if (spring.Orientation == Spring.Orientations.WallLeft && this.Speed.X <= 0.0)
        {
          this.MoveTowardsY(spring.CenterY + 5f, 4f, (Collision) null);
          this.Speed.X = (__Null) 220.0;
          this.Speed.Y = (__Null) -80.0;
          this.noGravityTimer = 0.1f;
          return true;
        }
        if (spring.Orientation == Spring.Orientations.WallRight && this.Speed.X >= 0.0)
        {
          this.MoveTowardsY(spring.CenterY + 5f, 4f, (Collision) null);
          this.Speed.X = (__Null) -220.0;
          this.Speed.Y = (__Null) -80.0;
          this.noGravityTimer = 0.1f;
          return true;
        }
      }
      return false;
    }

    private void OnCollideH(CollisionData data)
    {
      if (data.Hit is DashSwitch)
      {
        int num = (int) (data.Hit as DashSwitch).OnDashCollide((Player) null, Vector2.op_Multiply(Vector2.get_UnitX(), (float) Math.Sign((float) this.Speed.X)));
      }
      Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_side", this.Position);
      if ((double) Math.Abs((float) this.Speed.X) > 100.0)
        this.ImpactParticles(data.Direction);
      ref __Null local = ref this.Speed.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local = ^(float&) ref local * -0.4f;
    }

    private void OnCollideV(CollisionData data)
    {
      if (data.Hit is DashSwitch)
      {
        int num = (int) (data.Hit as DashSwitch).OnDashCollide((Player) null, Vector2.op_Multiply(Vector2.get_UnitY(), (float) Math.Sign((float) this.Speed.Y)));
      }
      if (this.Speed.Y > 0.0)
      {
        if ((double) this.hardVerticalHitSoundCooldown <= 0.0)
        {
          Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_ground", this.Position, "crystal_velocity", Calc.ClampedMap((float) this.Speed.Y, 0.0f, 200f, 0.0f, 1f));
          this.hardVerticalHitSoundCooldown = 0.5f;
        }
        else
          Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_ground", this.Position, "crystal_velocity", 0.0f);
      }
      if (this.Speed.Y > 160.0)
        this.ImpactParticles(data.Direction);
      if (this.Speed.Y > 140.0 && !(data.Hit is SwapBlock) && !(data.Hit is DashSwitch))
      {
        ref __Null local = ref this.Speed.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local = ^(float&) ref local * -0.6f;
      }
      else
        this.Speed.Y = (__Null) 0.0;
    }

    private void ImpactParticles(Vector2 dir)
    {
      float direction;
      Vector2 position;
      Vector2 positionRange;
      if (dir.X > 0.0)
      {
        direction = 3.141593f;
        ((Vector2) ref position).\u002Ector(this.Right, this.Y - 4f);
        positionRange = Vector2.op_Multiply(Vector2.get_UnitY(), 6f);
      }
      else if (dir.X < 0.0)
      {
        direction = 0.0f;
        ((Vector2) ref position).\u002Ector(this.Left, this.Y - 4f);
        positionRange = Vector2.op_Multiply(Vector2.get_UnitY(), 6f);
      }
      else if (dir.Y > 0.0)
      {
        direction = -1.570796f;
        ((Vector2) ref position).\u002Ector(this.X, this.Bottom);
        positionRange = Vector2.op_Multiply(Vector2.get_UnitX(), 6f);
      }
      else
      {
        direction = 1.570796f;
        ((Vector2) ref position).\u002Ector(this.X, this.Top);
        positionRange = Vector2.op_Multiply(Vector2.get_UnitX(), 6f);
      }
      this.Level.Particles.Emit(TheoCrystal.P_Impact, 12, position, positionRange, direction);
    }

    public override bool IsRiding(Solid solid)
    {
      if (this.Speed.Y == 0.0)
        return base.IsRiding(solid);
      return false;
    }

    protected override void OnSquish(CollisionData data)
    {
      if (this.TrySquishWiggle(data) || SaveData.Instance.Assists.Invincible)
        return;
      this.Die();
    }

    private void OnPickup()
    {
      this.Speed = Vector2.get_Zero();
      this.AddTag((int) Tags.Persistent);
    }

    private void OnRelease(Vector2 force)
    {
      this.RemoveTag((int) Tags.Persistent);
      if (force.X != 0.0 && force.Y == 0.0)
        force.Y = (__Null) -0.400000005960464;
      this.Speed = Vector2.op_Multiply(force, 200f);
      if (!Vector2.op_Inequality(this.Speed, Vector2.get_Zero()))
        return;
      this.noGravityTimer = 0.1f;
    }

    public void Die()
    {
      if (this.dead)
        return;
      this.dead = true;
      Player entity = this.Level.Tracker.GetEntity<Player>();
      entity?.Die(Vector2.op_Multiply(Vector2.op_UnaryNegation(Vector2.get_UnitX()), (float) entity.Facing), false, true);
      Audio.Play("event:/char/madeline/death", this.Position);
      this.Add((Component) new DeathEffect(Color.get_ForestGreen(), new Vector2?(Vector2.op_Subtraction(this.Center, this.Position))));
      this.sprite.Visible = false;
      this.Depth = -1000000;
      this.AllowPushing = false;
    }
  }
}
