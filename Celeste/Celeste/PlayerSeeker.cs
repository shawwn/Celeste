// Decompiled with JetBrains decompiler
// Type: Celeste.PlayerSeeker
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class PlayerSeeker : Actor
  {
    private Facings facing;
    private Sprite sprite;
    private Vector2 speed;
    private bool enabled;
    private float dashTimer;
    private Vector2 dashDirection;
    private float trailTimerA;
    private float trailTimerB;
    private Shaker shaker;

    public PlayerSeeker(EntityData data, Vector2 offset)
      : base(Vector2.op_Addition(data.Position, offset))
    {
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("seeker")));
      this.sprite.Play("statue", false, false);
      this.sprite.OnLastFrame = (Action<string>) (a =>
      {
        if (!(a == "flipMouth") && !(a == "flipEyes"))
          return;
        this.facing = (Facings) -(int) this.facing;
      });
      this.Collider = (Collider) new Hitbox(10f, 10f, -5f, -5f);
      this.Add((Component) new MirrorReflection());
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Add((Component) new VertexLight(Color.get_White(), 1f, 32, 64));
      this.facing = Facings.Right;
      this.Add((Component) (this.shaker = new Shaker(false, (Action<Vector2>) null)));
      this.Add((Component) new Coroutine(this.IntroSequence(), true));
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      Level level = scene as Level;
      level.Session.ColorGrade = "templevoid";
      level.ScreenPadding = 32f;
      level.CanRetry = false;
    }

    private IEnumerator IntroSequence()
    {
      PlayerSeeker playerSeeker = this;
      Level level = playerSeeker.Scene as Level;
      yield return (object) null;
      Glitch.Value = 0.05f;
      level.Tracker.GetEntity<Player>()?.StartTempleMirrorVoidSleep();
      yield return (object) 3f;
      Vector2 from = level.Camera.Position;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, 2f, true);
      tween.OnUpdate = (Action<Tween>) (f => level.Camera.Position = Vector2.op_Addition(from, Vector2.op_Multiply(Vector2.op_Subtraction(this.CameraTarget, from), f.Eased)));
      playerSeeker.Add((Component) tween);
      yield return (object) 2f;
      playerSeeker.shaker.ShakeFor(0.5f, false);
      playerSeeker.BreakOutParticles();
      Input.Rumble(RumbleStrength.Light, RumbleLength.Long);
      yield return (object) 1f;
      playerSeeker.shaker.ShakeFor(0.5f, false);
      playerSeeker.BreakOutParticles();
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Long);
      yield return (object) 1f;
      playerSeeker.BreakOutParticles();
      Audio.Play("event:/game/05_mirror_temple/seeker_statue_break", playerSeeker.Position);
      playerSeeker.shaker.ShakeFor(1f, false);
      playerSeeker.sprite.Play("hatch", false, false);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.FullSecond);
      playerSeeker.enabled = true;
      yield return (object) 0.8f;
      playerSeeker.BreakOutParticles();
      yield return (object) 0.7f;
    }

    private void BreakOutParticles()
    {
      Level level = this.SceneAs<Level>();
      for (float direction = 0.0f; (double) direction < 6.28318548202515; direction += 0.1745329f)
      {
        Vector2 position = Vector2.op_Addition(this.Center, Calc.AngleToVector(direction + Calc.Random.Range(-1f * (float) Math.PI / 90f, (float) Math.PI / 90f), (float) Calc.Random.Range(12, 20)));
        level.Particles.Emit(Seeker.P_BreakOut, position, direction);
      }
    }

    private void OnPlayer(Player player)
    {
      if (player.Dead)
        return;
      Leader.StoreStrawberries(player.Leader);
      PlayerDeadBody playerDeadBody = player.Die(Vector2.op_Subtraction(player.Position, this.Position).SafeNormalize(), true, false);
      playerDeadBody.DeathAction = new Action(this.End);
      playerDeadBody.ActionDelay = 0.3f;
      Engine.TimeRate = 0.25f;
    }

    private void End()
    {
      Level level = this.Scene as Level;
      level.OnEndOfFrame += (Action) (() =>
      {
        Glitch.Value = 0.0f;
        Distort.Anxiety = 0.0f;
        Engine.TimeRate = 1f;
        level.Session.ColorGrade = (string) null;
        level.UnloadLevel();
        level.CanRetry = true;
        level.Session.Level = "c-00";
        Session session = level.Session;
        Level level1 = level;
        Rectangle bounds = level.Bounds;
        double left = (double) ((Rectangle) ref bounds).get_Left();
        bounds = level.Bounds;
        double top = (double) ((Rectangle) ref bounds).get_Top();
        Vector2 from = new Vector2((float) left, (float) top);
        Vector2? nullable = new Vector2?(level1.GetSpawnPoint(from));
        session.RespawnPoint = nullable;
        level.LoadLevel(Player.IntroTypes.WakeUp, false);
        Leader.RestoreStrawberries(level.Tracker.GetEntity<Player>().Leader);
      });
    }

    public override void Update()
    {
      foreach (Entity entity in this.Scene.Tracker.GetEntities<SeekerBarrier>())
        entity.Collidable = true;
      Level scene = this.Scene as Level;
      base.Update();
      this.sprite.Scale.X = (__Null) (double) Calc.Approach((float) this.sprite.Scale.X, 1f, 2f * Engine.DeltaTime);
      this.sprite.Scale.Y = (__Null) (double) Calc.Approach((float) this.sprite.Scale.Y, 1f, 2f * Engine.DeltaTime);
      if (this.enabled && this.sprite.CurrentAnimationID != "hatch")
      {
        if ((double) this.dashTimer > 0.0)
        {
          this.speed = Calc.Approach(this.speed, Vector2.get_Zero(), 800f * Engine.DeltaTime);
          this.dashTimer -= Engine.DeltaTime;
          if ((double) this.dashTimer <= 0.0)
            this.sprite.Play("spotted", false, false);
          if ((double) this.trailTimerA > 0.0)
          {
            this.trailTimerA -= Engine.DeltaTime;
            if ((double) this.trailTimerA <= 0.0)
              this.CreateTrail();
          }
          if ((double) this.trailTimerB > 0.0)
          {
            this.trailTimerB -= Engine.DeltaTime;
            if ((double) this.trailTimerB <= 0.0)
              this.CreateTrail();
          }
          if (this.Scene.OnInterval(0.04f))
          {
            Vector2 vector = this.speed.SafeNormalize();
            this.SceneAs<Level>().Particles.Emit(Seeker.P_Attack, 2, Vector2.op_Addition(this.Position, Vector2.op_Multiply(vector, 4f)), Vector2.op_Multiply(Vector2.get_One(), 4f), vector.Angle());
          }
        }
        else
        {
          Vector2 vector2 = Input.Aim.Value.SafeNormalize();
          this.speed = Vector2.op_Addition(this.speed, Vector2.op_Multiply(Vector2.op_Multiply(vector2, 600f), Engine.DeltaTime));
          float val = ((Vector2) ref this.speed).Length();
          if ((double) val > 120.0)
            this.speed = this.speed.SafeNormalize(Calc.Approach(val, 120f, Engine.DeltaTime * 700f));
          if (vector2.Y == 0.0)
            this.speed.Y = (__Null) (double) Calc.Approach((float) this.speed.Y, 0.0f, 400f * Engine.DeltaTime);
          if (vector2.X == 0.0)
            this.speed.X = (__Null) (double) Calc.Approach((float) this.speed.X, 0.0f, 400f * Engine.DeltaTime);
          if ((double) ((Vector2) ref vector2).Length() > 0.0 && this.sprite.CurrentAnimationID == "idle")
          {
            scene.Displacement.AddBurst(this.Position, 0.5f, 8f, 32f, 1f, (Ease.Easer) null, (Ease.Easer) null);
            this.sprite.Play("spotted", false, false);
            Audio.Play("event:/game/05_mirror_temple/seeker_playercontrolstart");
          }
          int num1 = Math.Sign((int) this.facing);
          int num2 = Math.Sign((float) this.speed.X);
          if (num2 != 0 && num1 != num2 && (Math.Sign((float) Input.Aim.Value.X) == Math.Sign((float) this.speed.X) && (double) Math.Abs((float) this.speed.X) > 20.0) && (this.sprite.CurrentAnimationID != "flipMouth" && this.sprite.CurrentAnimationID != "flipEyes"))
            this.sprite.Play("flipMouth", false, false);
          if (Input.Dash.Pressed)
            this.Dash(Input.Aim.Value.EightWayNormal());
        }
        this.MoveH((float) this.speed.X * Engine.DeltaTime, new Collision(this.OnCollide), (Solid) null);
        this.MoveV((float) this.speed.Y * Engine.DeltaTime, new Collision(this.OnCollide), (Solid) null);
        Vector2 position = this.Position;
        double x = (double) (float) scene.Bounds.X;
        double y = (double) (float) scene.Bounds.Y;
        Rectangle bounds = scene.Bounds;
        double right = (double) ((Rectangle) ref bounds).get_Right();
        bounds = scene.Bounds;
        double bottom = (double) ((Rectangle) ref bounds).get_Bottom();
        this.Position = position.Clamp((float) x, (float) y, (float) right, (float) bottom);
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity != null)
        {
          Vector2 vector2 = Vector2.op_Subtraction(this.Position, entity.Position);
          float val = ((Vector2) ref vector2).Length();
          if ((double) val < 200.0 && entity.Sprite.CurrentAnimationID == "asleep")
          {
            entity.Sprite.Rate = 2f;
            entity.Sprite.Play("wakeUp", false, false);
          }
          else if ((double) val < 100.0 && entity.Sprite.CurrentAnimationID != "wakeUp")
          {
            entity.Sprite.Rate = 1f;
            entity.Sprite.Play("runFast", false, false);
            entity.Facing = (double) this.X > (double) entity.X ? Facings.Left : Facings.Right;
          }
          if ((double) val < 50.0 && (double) this.dashTimer <= 0.0)
            this.Dash(Vector2.op_Subtraction(entity.Center, this.Center).SafeNormalize());
          Engine.TimeRate = Calc.ClampedMap(val, 60f, 220f, 0.5f, 1f);
          Camera camera1 = scene.Camera;
          Vector2 cameraTarget = this.CameraTarget;
          Camera camera2 = camera1;
          camera2.Position = Vector2.op_Addition(camera2.Position, Vector2.op_Multiply(Vector2.op_Subtraction(cameraTarget, camera1.Position), 1f - (float) Math.Pow(0.00999999977648258, (double) Engine.DeltaTime)));
          Distort.Anxiety = Calc.ClampedMap(val, 0.0f, 200f, 0.25f, 0.0f) + Calc.Random.NextFloat(0.05f);
          Distort.AnxietyOrigin = Vector2.op_Division(Vector2.op_Subtraction(new Vector2(entity.X, scene.Camera.Top), scene.Camera.Position), new Vector2(320f, 180f));
        }
        else
          Engine.TimeRate = Calc.Approach(Engine.TimeRate, 1f, 1f * Engine.DeltaTime);
      }
      foreach (Entity entity in this.Scene.Tracker.GetEntities<SeekerBarrier>())
        entity.Collidable = false;
    }

    private void CreateTrail()
    {
      Vector2 scale = this.sprite.Scale;
      ref __Null local = ref this.sprite.Scale.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local = ^(float&) ref local * (float) this.facing;
      TrailManager.Add((Entity) this, Seeker.TrailColor, 1f);
      this.sprite.Scale = scale;
    }

    private void OnCollide(CollisionData data)
    {
      if ((double) this.dashTimer <= 0.0)
      {
        if (data.Direction.X != 0.0)
          this.speed.X = (__Null) 0.0;
        if (data.Direction.Y == 0.0)
          return;
        this.speed.Y = (__Null) 0.0;
      }
      else
      {
        float direction;
        Vector2 position;
        Vector2 positionRange;
        if (data.Direction.X > 0.0)
        {
          direction = 3.141593f;
          ((Vector2) ref position).\u002Ector(this.Right, this.Y);
          positionRange = Vector2.op_Multiply(Vector2.get_UnitY(), 4f);
        }
        else if (data.Direction.X < 0.0)
        {
          direction = 0.0f;
          ((Vector2) ref position).\u002Ector(this.Left, this.Y);
          positionRange = Vector2.op_Multiply(Vector2.get_UnitY(), 4f);
        }
        else if (data.Direction.Y > 0.0)
        {
          direction = -1.570796f;
          ((Vector2) ref position).\u002Ector(this.X, this.Bottom);
          positionRange = Vector2.op_Multiply(Vector2.get_UnitX(), 4f);
        }
        else
        {
          direction = 1.570796f;
          ((Vector2) ref position).\u002Ector(this.X, this.Top);
          positionRange = Vector2.op_Multiply(Vector2.get_UnitX(), 4f);
        }
        this.SceneAs<Level>().Particles.Emit(Seeker.P_HitWall, 12, position, positionRange, direction);
        if (data.Hit is SeekerBarrier)
        {
          (data.Hit as SeekerBarrier).OnReflectSeeker();
          Audio.Play("event:/game/05_mirror_temple/seeker_hit_lightwall", this.Position);
        }
        else
          Audio.Play("event:/game/05_mirror_temple/seeker_hit_normal", this.Position);
        if (data.Direction.X != 0.0)
        {
          ref __Null local = ref this.speed.X;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local * -0.8f;
          this.sprite.Scale = new Vector2(0.6f, 1.4f);
        }
        else if (data.Direction.Y != 0.0)
        {
          ref __Null local = ref this.speed.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local * -0.8f;
          this.sprite.Scale = new Vector2(1.4f, 0.6f);
        }
        if (!(data.Hit is TempleCrackedBlock))
          return;
        Celeste.Celeste.Freeze(0.15f);
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
        (data.Hit as TempleCrackedBlock).Break(this.Position);
      }
    }

    private void Dash(Vector2 dir)
    {
      if ((double) this.dashTimer <= 0.0)
      {
        this.CreateTrail();
        this.trailTimerA = 0.1f;
        this.trailTimerB = 0.25f;
      }
      this.dashTimer = 0.3f;
      this.dashDirection = dir;
      if (Vector2.op_Equality(this.dashDirection, Vector2.get_Zero()))
        this.dashDirection.X = (__Null) (double) Math.Sign((int) this.facing);
      if (this.dashDirection.X != 0.0)
        this.facing = (Facings) Math.Sign((float) this.dashDirection.X);
      this.speed = Vector2.op_Multiply(this.dashDirection, 400f);
      this.sprite.Play("attacking", false, false);
      this.SceneAs<Level>().DirectionalShake(this.dashDirection, 0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      Audio.Play("event:/game/05_mirror_temple/seeker_dash", this.Position);
      if (this.dashDirection.X == 0.0)
        this.sprite.Scale = new Vector2(0.6f, 1.4f);
      else
        this.sprite.Scale = new Vector2(1.4f, 0.6f);
    }

    public Vector2 CameraTarget
    {
      get
      {
        Rectangle bounds = (this.Scene as Level).Bounds;
        return Vector2.op_Addition(this.Position, new Vector2(-160f, -90f)).Clamp((float) ((Rectangle) ref bounds).get_Left(), (float) ((Rectangle) ref bounds).get_Top(), (float) (((Rectangle) ref bounds).get_Right() - 320), (float) (((Rectangle) ref bounds).get_Bottom() - 180));
      }
    }

    public override void Render()
    {
      if (SaveData.Instance.Assists.InvisibleMotion && this.enabled && (double) ((Vector2) ref this.speed).LengthSquared() > 100.0)
        return;
      Vector2 position = this.Position;
      this.Position = Vector2.op_Addition(this.Position, this.shaker.Value);
      Vector2 scale = this.sprite.Scale;
      ref __Null local = ref this.sprite.Scale.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local = ^(float&) ref local * (float) this.facing;
      base.Render();
      this.Position = position;
      this.sprite.Scale = scale;
    }
  }
}
