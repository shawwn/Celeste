// Decompiled with JetBrains decompiler
// Type: Celeste.Glider
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class Glider : Actor
  {
    public static ParticleType P_Glide;
    public static ParticleType P_GlideUp;
    public static ParticleType P_Platform;
    public static ParticleType P_Glow;
    public static ParticleType P_Expand;
    private const float HighFrictionTime = 0.5f;
    public Vector2 Speed;
    public Holdable Hold;
    private Level level;
    private Collision onCollideH;
    private Collision onCollideV;
    private Vector2 prevLiftSpeed;
    private Vector2 startPos;
    private float noGravityTimer;
    private float highFrictionTimer;
    private bool bubble;
    private bool tutorial;
    private bool destroyed;
    private Sprite sprite;
    private Wiggler wiggler;
    private SineWave platformSine;
    private SoundSource fallingSfx;
    private BirdTutorialGui tutorialGui;

    public Glider(Vector2 position, bool bubble, bool tutorial)
      : base(position)
    {
      this.bubble = bubble;
      this.tutorial = tutorial;
      this.startPos = this.Position;
      this.Collider = (Collider) new Hitbox(8f, 10f, -4f, -10f);
      this.onCollideH = new Collision(this.OnCollideH);
      this.onCollideV = new Collision(this.OnCollideV);
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("glider")));
      this.Add((Component) (this.wiggler = Wiggler.Create(0.25f, 4f)));
      this.Depth = -5;
      this.Add((Component) (this.Hold = new Holdable(0.3f)));
      this.Hold.PickupCollider = (Collider) new Hitbox(20f, 22f, -10f, -16f);
      this.Hold.SlowFall = true;
      this.Hold.SlowRun = false;
      this.Hold.OnPickup = new Action(this.OnPickup);
      this.Hold.OnRelease = new Action<Vector2>(this.OnRelease);
      this.Hold.SpeedGetter = (Func<Vector2>) (() => this.Speed);
      this.Hold.OnHitSpring = new Func<Spring, bool>(this.HitSpring);
      this.platformSine = new SineWave(0.3f);
      this.Add((Component) this.platformSine);
      this.fallingSfx = new SoundSource();
      this.Add((Component) this.fallingSfx);
      this.Add((Component) new WindMover(new Action<Vector2>(this.WindMode)));
    }

    public Glider(EntityData e, Vector2 offset)
      : this(e.Position + offset, e.Bool(nameof (bubble)), e.Bool(nameof (tutorial)))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.level = this.SceneAs<Level>();
      if (!this.tutorial)
        return;
      this.tutorialGui = new BirdTutorialGui((Entity) this, new Vector2(0.0f, -24f), (object) Dialog.Clean("tutorial_carry"), new object[2]
      {
        (object) Dialog.Clean("tutorial_hold"),
        (object) BirdTutorialGui.ButtonPrompt.Grab
      });
      this.tutorialGui.Open = true;
      this.Scene.Add((Entity) this.tutorialGui);
    }

    public override void Update()
    {
      if (this.Scene.OnInterval(0.05f))
        this.level.Particles.Emit(Glider.P_Glow, 1, this.Center + Vector2.UnitY * -9f, new Vector2(10f, 4f));
      this.sprite.Rotation = Calc.Approach(this.sprite.Rotation, !this.Hold.IsHeld ? 0.0f : (!this.Hold.Holder.OnGround() ? Calc.ClampedMap(this.Hold.Holder.Speed.X, -300f, 300f, 1.0471976f, -1.0471976f) : Calc.ClampedMap(this.Hold.Holder.Speed.X, -300f, 300f, 0.6981317f, -0.6981317f)), 3.1415927f * Engine.DeltaTime);
      if (this.Hold.IsHeld && !this.Hold.Holder.OnGround() && (this.sprite.CurrentAnimationID == "fall" || this.sprite.CurrentAnimationID == "fallLoop"))
      {
        if (!this.fallingSfx.Playing)
        {
          Audio.Play("event:/new_content/game/10_farewell/glider_engage", this.Position);
          this.fallingSfx.Play("event:/new_content/game/10_farewell/glider_movement");
        }
        Vector2 speed = this.Hold.Holder.Speed;
        this.fallingSfx.Param("glider_speed", Calc.Map(new Vector2(speed.X * 0.5f, (double) speed.Y < 0.0 ? speed.Y * 2f : speed.Y).Length(), 0.0f, 120f, newMax: 0.7f));
      }
      else
        this.fallingSfx.Stop();
      base.Update();
      if (!this.destroyed)
      {
        foreach (SeekerBarrier entity in this.Scene.Tracker.GetEntities<SeekerBarrier>())
        {
          entity.Collidable = true;
          int num = this.CollideCheck((Entity) entity) ? 1 : 0;
          entity.Collidable = false;
          if (num != 0)
          {
            this.destroyed = true;
            this.Collidable = false;
            if (this.Hold.IsHeld)
            {
              Vector2 speed = this.Hold.Holder.Speed;
              this.Hold.Holder.Drop();
              this.Speed = speed * 0.333f;
              Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }
            this.Add((Component) new Coroutine(this.DestroyAnimationRoutine()));
            return;
          }
        }
        if (this.Hold.IsHeld)
          this.prevLiftSpeed = Vector2.Zero;
        else if (!this.bubble)
        {
          if ((double) this.highFrictionTimer > 0.0)
            this.highFrictionTimer -= Engine.DeltaTime;
          if (this.OnGround())
          {
            this.Speed.X = Calc.Approach(this.Speed.X, this.OnGround(this.Position + Vector2.UnitX * 3f) ? (this.OnGround(this.Position - Vector2.UnitX * 3f) ? 0.0f : -20f) : 20f, 800f * Engine.DeltaTime);
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
            float num = 200f;
            if ((double) this.Speed.Y >= -30.0)
              num *= 0.5f;
            this.Speed.X = Calc.Approach(this.Speed.X, 0.0f, ((double) this.Speed.Y >= 0.0 ? ((double) this.highFrictionTimer > 0.0 ? 10f : 40f) : 40f) * Engine.DeltaTime);
            if ((double) this.noGravityTimer > 0.0)
              this.noGravityTimer -= Engine.DeltaTime;
            else
              this.Speed.Y = (double) this.level.Wind.Y >= 0.0 ? Calc.Approach(this.Speed.Y, 30f, num * Engine.DeltaTime) : Calc.Approach(this.Speed.Y, 0.0f, num * Engine.DeltaTime);
          }
          this.MoveH(this.Speed.X * Engine.DeltaTime, this.onCollideH);
          this.MoveV(this.Speed.Y * Engine.DeltaTime, this.onCollideV);
          Rectangle bounds;
          if ((double) this.Left < (double) this.level.Bounds.Left)
          {
            bounds = this.level.Bounds;
            this.Left = (float) bounds.Left;
            this.OnCollideH(new CollisionData()
            {
              Direction = -Vector2.UnitX
            });
          }
          else
          {
            double right1 = (double) this.Right;
            bounds = this.level.Bounds;
            double right2 = (double) bounds.Right;
            if (right1 > right2)
            {
              bounds = this.level.Bounds;
              this.Right = (float) bounds.Right;
              this.OnCollideH(new CollisionData()
              {
                Direction = Vector2.UnitX
              });
            }
          }
          double top1 = (double) this.Top;
          bounds = this.level.Bounds;
          double top2 = (double) bounds.Top;
          if (top1 < top2)
          {
            bounds = this.level.Bounds;
            this.Top = (float) bounds.Top;
            this.OnCollideV(new CollisionData()
            {
              Direction = -Vector2.UnitY
            });
          }
          else
          {
            double top3 = (double) this.Top;
            bounds = this.level.Bounds;
            double num = (double) (bounds.Bottom + 16);
            if (top3 > num)
            {
              this.RemoveSelf();
              return;
            }
          }
          this.Hold.CheckAgainstColliders();
        }
        else
          this.Position = this.startPos + Vector2.UnitY * this.platformSine.Value * 1f;
        Vector2 one = Vector2.One;
        if (!this.Hold.IsHeld)
        {
          if ((double) this.level.Wind.Y < 0.0)
            this.PlayOpen();
          else
            this.sprite.Play("idle");
        }
        else if ((double) this.Hold.Holder.Speed.Y > 20.0 || (double) this.level.Wind.Y < 0.0)
        {
          if (this.level.OnInterval(0.04f))
          {
            if ((double) this.level.Wind.Y < 0.0)
              this.level.ParticlesBG.Emit(Glider.P_GlideUp, 1, this.Position - Vector2.UnitY * 20f, new Vector2(6f, 4f));
            else
              this.level.ParticlesBG.Emit(Glider.P_Glide, 1, this.Position - Vector2.UnitY * 10f, new Vector2(6f, 4f));
          }
          this.PlayOpen();
          if (Input.GliderMoveY.Value > 0)
          {
            one.X = 0.7f;
            one.Y = 1.4f;
          }
          else if (Input.GliderMoveY.Value < 0)
          {
            one.X = 1.2f;
            one.Y = 0.8f;
          }
          Input.Rumble(RumbleStrength.Climb, RumbleLength.Short);
        }
        else
          this.sprite.Play("held");
        this.sprite.Scale.Y = Calc.Approach(this.sprite.Scale.Y, one.Y, Engine.DeltaTime * 2f);
        this.sprite.Scale.X = Calc.Approach(this.sprite.Scale.X, (float) Math.Sign(this.sprite.Scale.X) * one.X, Engine.DeltaTime * 2f);
        if (this.tutorialGui == null)
          return;
        this.tutorialGui.Open = this.tutorial && !this.Hold.IsHeld && (this.OnGround(4) || this.bubble);
      }
      else
        this.Position = this.Position + this.Speed * Engine.DeltaTime;
    }

    private void PlayOpen()
    {
      if (!(this.sprite.CurrentAnimationID != "fall") || !(this.sprite.CurrentAnimationID != "fallLoop"))
        return;
      this.sprite.Play("fall");
      this.sprite.Scale = new Vector2(1.5f, 0.6f);
      this.level.Particles.Emit(Glider.P_Expand, 16, this.Center + (Vector2.UnitY * -12f).Rotate(this.sprite.Rotation), new Vector2(8f, 3f), this.sprite.Rotation - 1.5707964f);
      if (!this.Hold.IsHeld)
        return;
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
    }

    public override void Render()
    {
      if (!this.destroyed)
        this.sprite.DrawSimpleOutline();
      base.Render();
      if (!this.bubble)
        return;
      for (int num = 0; num < 24; ++num)
        Draw.Point(this.Position + this.PlatformAdd(num), this.PlatformColor(num));
    }

    private void WindMode(Vector2 wind)
    {
      if (this.Hold.IsHeld)
        return;
      if ((double) wind.X != 0.0)
        this.MoveH(wind.X * 0.5f);
      if ((double) wind.Y == 0.0)
        return;
      this.MoveV(wind.Y);
    }

    private Vector2 PlatformAdd(int num) => new Vector2((float) (num - 12), (float) ((int) Math.Round(Math.Sin((double) this.Scene.TimeActive + (double) num * 0.20000000298023224) * 1.7999999523162842) - 5));

    private Color PlatformColor(int num) => num <= 1 || num >= 22 ? Color.White * 0.4f : Color.White * 0.8f;

    private void OnCollideH(CollisionData data)
    {
      if (data.Hit is DashSwitch)
      {
        int num = (int) (data.Hit as DashSwitch).OnDashCollide((Player) null, Vector2.UnitX * (float) Math.Sign(this.Speed.X));
      }
      if ((double) this.Speed.X < 0.0)
        Audio.Play("event:/new_content/game/10_farewell/glider_wallbounce_left", this.Position);
      else
        Audio.Play("event:/new_content/game/10_farewell/glider_wallbounce_right", this.Position);
      this.Speed.X *= -1f;
      this.sprite.Scale = new Vector2(0.8f, 1.2f);
    }

    private void OnCollideV(CollisionData data)
    {
      if ((double) Math.Abs(this.Speed.Y) > 8.0)
      {
        this.sprite.Scale = new Vector2(1.2f, 0.8f);
        Audio.Play("event:/new_content/game/10_farewell/glider_land", this.Position);
      }
      if ((double) this.Speed.Y < 0.0)
        this.Speed.Y *= -0.5f;
      else
        this.Speed.Y = 0.0f;
    }

    private void OnPickup()
    {
      if (this.bubble)
      {
        for (int num = 0; num < 24; ++num)
          this.level.Particles.Emit(Glider.P_Platform, this.Position + this.PlatformAdd(num), this.PlatformColor(num));
      }
      this.AllowPushing = false;
      this.Speed = Vector2.Zero;
      this.AddTag((int) Tags.Persistent);
      this.highFrictionTimer = 0.5f;
      this.bubble = false;
      this.wiggler.Start();
      this.tutorial = false;
    }

    private void OnRelease(Vector2 force)
    {
      if ((double) force.X == 0.0)
        Audio.Play("event:/new_content/char/madeline/glider_drop", this.Position);
      this.AllowPushing = true;
      this.RemoveTag((int) Tags.Persistent);
      force.Y *= 0.5f;
      if ((double) force.X != 0.0 && (double) force.Y == 0.0)
        force.Y = -0.4f;
      this.Speed = force * 100f;
      this.wiggler.Start();
    }

    protected override void OnSquish(CollisionData data)
    {
      if (this.TrySquishWiggle(data))
        return;
      this.RemoveSelf();
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
          this.wiggler.Start();
          return true;
        }
        if (spring.Orientation == Spring.Orientations.WallLeft && (double) this.Speed.X <= 0.0)
        {
          this.MoveTowardsY(spring.CenterY + 5f, 4f);
          this.Speed.X = 160f;
          this.Speed.Y = -80f;
          this.noGravityTimer = 0.1f;
          this.wiggler.Start();
          return true;
        }
        if (spring.Orientation == Spring.Orientations.WallRight && (double) this.Speed.X >= 0.0)
        {
          this.MoveTowardsY(spring.CenterY + 5f, 4f);
          this.Speed.X = -160f;
          this.Speed.Y = -80f;
          this.noGravityTimer = 0.1f;
          this.wiggler.Start();
          return true;
        }
      }
      return false;
    }

    // class DestroyAnimation : IEnumerator
    // {
    //   private int __1__state = 0;
    //   private object __2__current;
    //   private Glider glider;
    //
    //   public DestroyAnimation(Glider glider)
    //   {
    //     this.glider = glider;
    //   }
    //
    //   
    //   public bool MoveNext()
    //   {
    //     // ISSUE: reference to a compiler-generated field
    //     int num = this.__1__state;
    //     if (num != 0)
    //     {
    //       if (num != 1)
    //         return false;
    //       // ISSUE: reference to a compiler-generated field
    //       this.__1__state = -1;
    //       this.glider.RemoveSelf();
    //       return false;
    //     }
    //     // ISSUE: reference to a compiler-generated field
    //     this.__1__state = -1;
    //     Audio.Play("event:/new_content/game/10_farewell/glider_emancipate", this.glider.Position);
    //     this.glider.sprite.Play("death");
    //     // ISSUE: reference to a compiler-generated field
    //     this.__2__current = (object) 1f;
    //     // ISSUE: reference to a compiler-generated field
    //     this.__1__state = 1;
    //     return true;
    //   }
    //
    //   public void Reset()
    //   {
    //     throw new NotImplementedException();
    //   }
    //
    //   public object Current
    //   {
    //     get { return this.__2__current; }
    //   }
    // };

    // private IEnumerator DestroyAnimationRoutine()
    // {
    //   // ISSUE: reference to a compiler-generated field
    //   int num = this.__1__state;
    //   Glider glider = this;
    //   if (num != 0)
    //   {
    //     if (num != 1)
    //       return false;
    //     // ISSUE: reference to a compiler-generated field
    //     this.__1__state = -1;
    //     glider.RemoveSelf();
    //     return false;
    //   }
    //   // ISSUE: reference to a compiler-generated field
    //   this.__1__state = -1;
    //   Audio.Play("event:/new_content/game/10_farewell/glider_emancipate", glider.Position);
    //   glider.sprite.Play("death");
    //   // ISSUE: reference to a compiler-generated field
    //   this.__2__current = (object) 1f;
    //   // ISSUE: reference to a compiler-generated field
    //   this.__1__state = 1;
    //   return true;
    // }

    private IEnumerator DestroyAnimationRoutine()
    {
      Audio.Play("event:/new_content/game/10_farewell/glider_emancipate", Position);
      sprite.Play("death");
      yield return 1f;
    }
  }
}
