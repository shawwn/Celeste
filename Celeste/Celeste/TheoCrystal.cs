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
    private float hardVerticalHitSoundCooldown = 0.0f;
    private float tutorialTimer = 0.0f;
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
    private BirdTutorialGui tutorialGui;

    public TheoCrystal(Vector2 position)
      : base(position)
    {
      this.previousPosition = position;
      this.Depth = 100;
      this.Collider = (Collider) new Hitbox(8f, 10f, -4f, -10f);
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("theo_crystal")));
      this.sprite.Scale.X = -1f;
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
      this.Add((Component) new VertexLight(this.Collider.Center, Color.White, 1f, 32, 64));
      this.Tag = (int) Tags.TransitionUpdate;
      this.Add((Component) new MirrorReflection());
    }

    public TheoCrystal(EntityData e, Vector2 offset)
      : this(e.Position + offset)
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
          this.prevLiftSpeed = Vector2.Zero;
        }
        else
        {
          if (this.OnGround(1))
          {
            this.Speed.X = Calc.Approach(this.Speed.X, this.OnGround(this.Position + Vector2.UnitX * 3f, 1) ? (this.OnGround(this.Position - Vector2.UnitX * 3f, 1) ? 0.0f : -20f) : 20f, 800f * Engine.DeltaTime);
            Vector2 liftSpeed = this.LiftSpeed;
            if (liftSpeed == Vector2.Zero && this.prevLiftSpeed != Vector2.Zero)
            {
              this.Speed = this.prevLiftSpeed;
              this.prevLiftSpeed = Vector2.Zero;
              this.Speed.Y = Math.Min(this.Speed.Y * 0.6f, 0.0f);
              if ((double) this.Speed.X != 0.0 && (double) this.Speed.Y == 0.0)
                this.Speed.Y = -60f;
              if ((double) this.Speed.Y < 0.0)
                this.noGravityTimer = 0.15f;
            }
            else
            {
              this.prevLiftSpeed = liftSpeed;
              if ((double) liftSpeed.Y < 0.0 && (double) this.Speed.Y < 0.0)
                this.Speed.Y = 0.0f;
            }
          }
          else if (this.Hold.ShouldHaveGravity)
          {
            float num1 = 800f;
            if ((double) Math.Abs(this.Speed.Y) <= 30.0)
              num1 *= 0.5f;
            float num2 = 350f;
            if ((double) this.Speed.Y < 0.0)
              num2 *= 0.5f;
            this.Speed.X = Calc.Approach(this.Speed.X, 0.0f, num2 * Engine.DeltaTime);
            if ((double) this.noGravityTimer > 0.0)
              this.noGravityTimer -= Engine.DeltaTime;
            else
              this.Speed.Y = Calc.Approach(this.Speed.Y, 200f, num1 * Engine.DeltaTime);
          }
          this.previousPosition = this.ExactPosition;
          this.MoveH(this.Speed.X * Engine.DeltaTime, this.onCollideH, (Solid) null);
          this.MoveV(this.Speed.Y * Engine.DeltaTime, this.onCollideV, (Solid) null);
          double x1 = (double) this.Center.X;
          Rectangle bounds = this.Level.Bounds;
          double right1 = (double) bounds.Right;
          if (x1 > right1)
          {
            this.MoveH(32f * Engine.DeltaTime, (Collision) null, (Solid) null);
            double num = (double) this.Left - 8.0;
            bounds = this.Level.Bounds;
            double right2 = (double) bounds.Right;
            if (num > right2)
              this.RemoveSelf();
          }
          else
          {
            double left1 = (double) this.Left;
            bounds = this.Level.Bounds;
            double left2 = (double) bounds.Left;
            if (left1 < left2)
            {
              bounds = this.Level.Bounds;
              this.Left = (float) bounds.Left;
              this.Speed.X *= -0.4f;
            }
            else
            {
              double top1 = (double) this.Top;
              bounds = this.Level.Bounds;
              double num = (double) (bounds.Top - 4);
              if (top1 < num)
              {
                bounds = this.Level.Bounds;
                this.Top = (float) (bounds.Top + 4);
                this.Speed.Y = 0.0f;
              }
              else
              {
                double bottom1 = (double) this.Bottom;
                bounds = this.Level.Bounds;
                double bottom2 = (double) bounds.Bottom;
                if (bottom1 > bottom2 && SaveData.Instance.Assists.Invincible)
                {
                  bounds = this.Level.Bounds;
                  this.Bottom = (float) bounds.Bottom;
                  this.Speed.Y = -300f;
                  Audio.Play("event:/game/general/assist_screenbottom", this.Position);
                }
                else
                {
                  double top2 = (double) this.Top;
                  bounds = this.Level.Bounds;
                  double bottom3 = (double) bounds.Bottom;
                  if (top2 > bottom3)
                    this.Die();
                }
              }
            }
          }
          double x2 = (double) this.X;
          bounds = this.Level.Bounds;
          double num3 = (double) (bounds.Left + 10);
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
        if (!this.OnPedestal && !this.Hold.IsHeld && this.OnGround(1) && this.Level.Session.GetFlag("foundTheoInCrystal"))
          this.tutorialTimer += Engine.DeltaTime;
        else
          this.tutorialTimer = 0.0f;
        this.tutorialGui.Open = (double) this.tutorialTimer > 0.25;
      }
    }

    public IEnumerator Shatter()
    {
      this.shattering = true;
      BloomPoint bloom = new BloomPoint(0.0f, 32f);
      VertexLight light = new VertexLight(Color.AliceBlue, 0.0f, 64, 200);
      this.Add((Component) bloom);
      this.Add((Component) light);
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        this.Position = this.Position + this.Speed * (1f - p) * Engine.DeltaTime;
        this.Level.ZoomFocusPoint = this.TopCenter - this.Level.Camera.Position;
        light.Alpha = p;
        bloom.Alpha = p;
        yield return (object) null;
      }
      yield return (object) 0.5f;
      this.Level.Shake(0.3f);
      this.sprite.Play("shatter", false, false);
      yield return (object) 1f;
      this.Level.Shake(0.3f);
    }

    public void ExplodeLaunch(Vector2 from)
    {
      if (this.Hold.IsHeld)
        return;
      this.Speed = (this.Center - from).SafeNormalize(120f);
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
      return !this.Hold.IsHeld && this.Speed != Vector2.Zero && this.hitSeeker != holdableCollider;
    }

    public void HitSeeker(Seeker seeker)
    {
      if (!this.Hold.IsHeld)
        this.Speed = (this.Center - seeker.Center).SafeNormalize(120f);
      Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_side", this.Position);
    }

    public void HitSpinner(Entity spinner)
    {
      if (this.Hold.IsHeld || (!(this.Speed == Vector2.Zero) || !(this.LiftSpeed == Vector2.Zero) || !(this.previousPosition == this.ExactPosition) || !this.OnGround(1)))
        return;
      int num = Math.Sign(this.X - spinner.X);
      if (num == 0)
        num = 1;
      this.Speed.X = (float) num * 120f;
      this.Speed.Y = -30f;
    }

    public bool HitSpring(Spring spring)
    {
      if (!this.Hold.IsHeld)
      {
        if (spring.Orientation == Spring.Orientations.Floor && (double) this.Speed.Y >= 0.0)
        {
          this.Speed.X *= 0.5f;
          this.Speed.Y = -160f;
          this.noGravityTimer = 0.15f;
          return true;
        }
        if (spring.Orientation == Spring.Orientations.WallLeft && (double) this.Speed.X <= 0.0)
        {
          this.MoveTowardsY(spring.CenterY + 5f, 4f, (Collision) null);
          this.Speed.X = 220f;
          this.Speed.Y = -80f;
          this.noGravityTimer = 0.1f;
          return true;
        }
        if (spring.Orientation == Spring.Orientations.WallRight && (double) this.Speed.X >= 0.0)
        {
          this.MoveTowardsY(spring.CenterY + 5f, 4f, (Collision) null);
          this.Speed.X = -220f;
          this.Speed.Y = -80f;
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
        int num = (int) (data.Hit as DashSwitch).OnDashCollide((Player) null, Vector2.UnitX * (float) Math.Sign(this.Speed.X));
      }
      Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_side", this.Position);
      if ((double) Math.Abs(this.Speed.X) > 100.0)
        this.ImpactParticles(data.Direction);
      this.Speed.X *= -0.4f;
    }

    private void OnCollideV(CollisionData data)
    {
      if (data.Hit is DashSwitch)
      {
        int num = (int) (data.Hit as DashSwitch).OnDashCollide((Player) null, Vector2.UnitY * (float) Math.Sign(this.Speed.Y));
      }
      if ((double) this.Speed.Y > 0.0)
      {
        if ((double) this.hardVerticalHitSoundCooldown <= 0.0)
        {
          Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_ground", this.Position, "crystal_velocity", Calc.ClampedMap(this.Speed.Y, 0.0f, 200f, 0.0f, 1f));
          this.hardVerticalHitSoundCooldown = 0.5f;
        }
        else
          Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_ground", this.Position, "crystal_velocity", 0.0f);
      }
      if ((double) this.Speed.Y > 160.0)
        this.ImpactParticles(data.Direction);
      if ((double) this.Speed.Y > 140.0 && !(data.Hit is SwapBlock) && !(data.Hit is DashSwitch))
        this.Speed.Y *= -0.6f;
      else
        this.Speed.Y = 0.0f;
    }

    private void ImpactParticles(Vector2 dir)
    {
      float direction;
      Vector2 position;
      Vector2 positionRange;
      if ((double) dir.X > 0.0)
      {
        direction = 3.141593f;
        position = new Vector2(this.Right, this.Y - 4f);
        positionRange = Vector2.UnitY * 6f;
      }
      else if ((double) dir.X < 0.0)
      {
        direction = 0.0f;
        position = new Vector2(this.Left, this.Y - 4f);
        positionRange = Vector2.UnitY * 6f;
      }
      else if ((double) dir.Y > 0.0)
      {
        direction = -1.570796f;
        position = new Vector2(this.X, this.Bottom);
        positionRange = Vector2.UnitX * 6f;
      }
      else
      {
        direction = 1.570796f;
        position = new Vector2(this.X, this.Top);
        positionRange = Vector2.UnitX * 6f;
      }
      this.Level.Particles.Emit(TheoCrystal.P_Impact, 12, position, positionRange, direction);
    }

    public override bool IsRiding(Solid solid)
    {
      return (double) this.Speed.Y == 0.0 && base.IsRiding(solid);
    }

    protected override void OnSquish(CollisionData data)
    {
      if (this.TrySquishWiggle(data) || SaveData.Instance.Assists.Invincible)
        return;
      this.Die();
    }

    private void OnPickup()
    {
      this.Speed = Vector2.Zero;
      this.AddTag((int) Tags.Persistent);
    }

    private void OnRelease(Vector2 force)
    {
      this.RemoveTag((int) Tags.Persistent);
      if ((double) force.X != 0.0 && (double) force.Y == 0.0)
        force.Y = -0.4f;
      this.Speed = force * 200f;
      if (!(this.Speed != Vector2.Zero))
        return;
      this.noGravityTimer = 0.1f;
    }

    public void Die()
    {
      if (this.dead)
        return;
      this.dead = true;
      Player entity = this.Level.Tracker.GetEntity<Player>();
      if (entity != null)
        entity.Die(-Vector2.UnitX * (float) entity.Facing, false, true);
      Audio.Play("event:/char/madeline/death", this.Position);
      this.Add((Component) new DeathEffect(Color.ForestGreen, new Vector2?(this.Center - this.Position)));
      this.sprite.Visible = false;
      this.Depth = -1000000;
      this.AllowPushing = false;
    }
  }
}

