// Decompiled with JetBrains decompiler
// Type: Celeste.Puffer
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class Puffer : Actor
  {
    private const float RespawnTime = 2.5f;
    private const float RespawnMoveTime = 0.5f;
    private const float BounceSpeed = 200f;
    private const float ExplodeRadius = 40f;
    private const float DetectRadius = 32f;
    private const float StunnedAccel = 320f;
    private const float AlertedRadius = 60f;
    private const float CantExplodeTime = 0.5f;
    private Sprite sprite;
    private Puffer.States state;
    private Vector2 startPosition;
    private Vector2 anchorPosition;
    private Vector2 lastSpeedPosition;
    private Vector2 lastSinePosition;
    private Monocle.Circle pushRadius;
    private Monocle.Circle breakWallsRadius;
    private Monocle.Circle detectRadius;
    private SineWave idleSine;
    private Vector2 hitSpeed;
    private float goneTimer;
    private float cannotHitTimer;
    private Collision onCollideV;
    private Collision onCollideH;
    private float alertTimer;
    private Wiggler bounceWiggler;
    private Wiggler inflateWiggler;
    private Vector2 scale;
    private SimpleCurve returnCurve;
    private float cantExplodeTimer;
    private Vector2 lastPlayerPos;
    private float playerAliveFade;
    private Vector2 facing = Vector2.One;
    private float eyeSpin;

    public Puffer(Vector2 position, bool faceRight)
      : base(position)
    {
      this.Collider = (Collider) new Hitbox(12f, 10f, -6f, -5f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) new Hitbox(14f, 12f, -7f, -7f)));
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("pufferFish")));
      this.sprite.Play("idle");
      if (!faceRight)
        this.facing.X = -1f;
      this.idleSine = new SineWave(0.5f);
      this.idleSine.Randomize();
      this.Add((Component) this.idleSine);
      this.anchorPosition = this.Position;
      this.Position = this.Position + new Vector2(this.idleSine.Value * 3f, this.idleSine.ValueOverTwo * 2f);
      this.state = Puffer.States.Idle;
      this.startPosition = this.lastSinePosition = this.lastSpeedPosition = this.Position;
      this.pushRadius = new Monocle.Circle(40f);
      this.detectRadius = new Monocle.Circle(32f);
      this.breakWallsRadius = new Monocle.Circle(16f);
      this.onCollideV = new Collision(this.OnCollideV);
      this.onCollideH = new Collision(this.OnCollideH);
      this.scale = Vector2.One;
      this.bounceWiggler = Wiggler.Create(0.6f, 2.5f, (Action<float>) (v => this.sprite.Rotation = (float) ((double) v * 20.0 * (Math.PI / 180.0))));
      this.Add((Component) this.bounceWiggler);
      this.inflateWiggler = Wiggler.Create(0.6f, 2f);
      this.Add((Component) this.inflateWiggler);
    }

    public Puffer(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Bool("right"))
    {
    }

    public override bool IsRiding(JumpThru jumpThru) => false;

    public override bool IsRiding(Solid solid) => false;

    protected override void OnSquish(CollisionData data)
    {
      this.Explode();
      this.GotoGone();
    }

    private void OnCollideH(CollisionData data) => this.hitSpeed.X *= -0.8f;

    private void OnCollideV(CollisionData data)
    {
      if ((double) data.Direction.Y <= 0.0)
        return;
      for (int index1 = -1; index1 <= 1; index1 += 2)
      {
        for (int index2 = 1; index2 <= 2; ++index2)
        {
          Vector2 at = this.Position + Vector2.UnitX * (float) index2 * (float) index1;
          if (!this.CollideCheck<Solid>(at) && !this.OnGround(at))
          {
            this.Position = at;
            return;
          }
        }
      }
      this.hitSpeed.Y *= -0.2f;
    }

    private void GotoIdle()
    {
      if (this.state == Puffer.States.Gone)
      {
        this.Position = this.startPosition;
        this.cantExplodeTimer = 0.5f;
        this.sprite.Play("recover");
        Audio.Play("event:/new_content/game/10_farewell/puffer_reform", this.Position);
      }
      this.lastSinePosition = this.lastSpeedPosition = this.anchorPosition = this.Position;
      this.hitSpeed = Vector2.Zero;
      this.idleSine.Reset();
      this.state = Puffer.States.Idle;
    }

    private void GotoHit(Vector2 from)
    {
      this.scale = new Vector2(1.2f, 0.8f);
      this.hitSpeed = Vector2.UnitY * 200f;
      this.state = Puffer.States.Hit;
      this.bounceWiggler.Start();
      this.Alert(true, false);
      Audio.Play("event:/new_content/game/10_farewell/puffer_boop", this.Position);
    }

    private void GotoHitSpeed(Vector2 speed)
    {
      this.hitSpeed = speed;
      this.state = Puffer.States.Hit;
    }

    private void GotoGone()
    {
      Vector2 control = this.Position + (this.startPosition - this.Position) * 0.5f;
      if ((double) (this.startPosition - this.Position).LengthSquared() > 100.0)
      {
        if ((double) Math.Abs(this.Position.Y - this.startPosition.Y) > (double) Math.Abs(this.Position.X - this.startPosition.X))
        {
          if ((double) this.Position.X > (double) this.startPosition.X)
            control += Vector2.UnitX * -24f;
          else
            control += Vector2.UnitX * 24f;
        }
        else if ((double) this.Position.Y > (double) this.startPosition.Y)
          control += Vector2.UnitY * -24f;
        else
          control += Vector2.UnitY * 24f;
      }
      this.returnCurve = new SimpleCurve(this.Position, this.startPosition, control);
      this.Collidable = false;
      this.goneTimer = 2.5f;
      this.state = Puffer.States.Gone;
    }

    private void Explode()
    {
      Collider collider = this.Collider;
      this.Collider = (Collider) this.pushRadius;
      Audio.Play("event:/new_content/game/10_farewell/puffer_splode", this.Position);
      this.sprite.Play("explode");
      Player player = this.CollideFirst<Player>();
      if (player != null && !this.Scene.CollideCheck<Solid>(this.Position, player.Center))
        player.ExplodeLaunch(this.Position, false, true);
      TheoCrystal theoCrystal = this.CollideFirst<TheoCrystal>();
      if (theoCrystal != null && !this.Scene.CollideCheck<Solid>(this.Position, theoCrystal.Center))
        theoCrystal.ExplodeLaunch(this.Position);
      foreach (TempleCrackedBlock entity in this.Scene.Tracker.GetEntities<TempleCrackedBlock>())
      {
        if (this.CollideCheck((Entity) entity))
          entity.Break(this.Position);
      }
      foreach (TouchSwitch entity in this.Scene.Tracker.GetEntities<TouchSwitch>())
      {
        if (this.CollideCheck((Entity) entity))
          entity.TurnOn();
      }
      foreach (FloatingDebris entity in this.Scene.Tracker.GetEntities<FloatingDebris>())
      {
        if (this.CollideCheck((Entity) entity))
          entity.OnExplode(this.Position);
      }
      this.Collider = collider;
      Level level = this.SceneAs<Level>();
      level.Shake();
      level.Displacement.AddBurst(this.Position, 0.4f, 12f, 36f, 0.5f);
      level.Displacement.AddBurst(this.Position, 0.4f, 24f, 48f, 0.5f);
      level.Displacement.AddBurst(this.Position, 0.4f, 36f, 60f, 0.5f);
      for (float direction = 0.0f; (double) direction < 6.2831854820251465; direction += 0.17453292f)
      {
        Vector2 position = this.Center + Calc.AngleToVector(direction + Calc.Random.Range(-1f * (float) Math.PI / 90f, (float) Math.PI / 90f), (float) Calc.Random.Range(12, 18));
        level.Particles.Emit(Seeker.P_Regen, position, direction);
      }
    }

    public override void Render()
    {
      this.sprite.Scale = this.scale * (float) (1.0 + (double) this.inflateWiggler.Value * 0.4000000059604645);
      Sprite sprite1 = this.sprite;
      sprite1.Scale = sprite1.Scale * this.facing;
      bool flag1 = false;
      if (this.sprite.CurrentAnimationID != "hidden" && this.sprite.CurrentAnimationID != "explode" && this.sprite.CurrentAnimationID != "recover")
        flag1 = true;
      else if (this.sprite.CurrentAnimationID == "explode" && this.sprite.CurrentAnimationFrame <= 1)
        flag1 = true;
      else if (this.sprite.CurrentAnimationID == "recover" && this.sprite.CurrentAnimationFrame >= 4)
        flag1 = true;
      if (flag1)
        this.sprite.DrawSimpleOutline();
      float num1 = this.playerAliveFade * Calc.ClampedMap((this.Position - this.lastPlayerPos).Length(), 128f, 96f);
      if ((double) num1 > 0.0 && this.state != Puffer.States.Gone)
      {
        bool flag2 = false;
        Vector2 lastPlayerPos = this.lastPlayerPos;
        if ((double) lastPlayerPos.Y < (double) this.Y)
        {
          lastPlayerPos.Y = this.Y - (float) (((double) lastPlayerPos.Y - (double) this.Y) * 0.5);
          lastPlayerPos.X += lastPlayerPos.X - this.X;
          flag2 = true;
        }
        float radiansB = (lastPlayerPos - this.Position).Angle();
        for (int index = 0; index < 28; ++index)
        {
          float num2 = (float) Math.Sin((double) this.Scene.TimeActive * 0.5) * 0.02f;
          float num3 = Calc.Map((float) index / 28f + num2, 0.0f, 1f, -1f * (float) Math.PI / 30f, 3.2463126f) + (float) ((double) this.bounceWiggler.Value * 20.0 * (Math.PI / 180.0));
          Vector2 vector = Calc.AngleToVector(num3, 1f);
          Vector2 start = this.Position + vector * 32f;
          float t = Calc.ClampedMap(Calc.AbsAngleDiff(num3, radiansB), 1.5707964f, 0.17453292f);
          float num4 = Ease.CubeOut(t) * 0.8f * num1;
          if ((double) num4 > 0.0)
          {
            if (index == 0 || index == 27)
            {
              Draw.Line(start, start - vector * 10f, Color.White * num4);
            }
            else
            {
              Vector2 vector2_1 = vector * (float) Math.Sin((double) this.Scene.TimeActive * 2.0 + (double) index * 0.6000000238418579);
              if (index % 2 == 0)
                vector2_1 *= -1f;
              Vector2 vector2_2 = start + vector2_1;
              if (!flag2 && (double) Calc.AbsAngleDiff(num3, radiansB) <= 0.1745329201221466)
                Draw.Line(vector2_2, vector2_2 - vector * 3f, Color.White * num4);
              else
                Draw.Point(vector2_2, Color.White * num4);
            }
          }
        }
      }
      base.Render();
      if (this.sprite.CurrentAnimationID == "alerted")
      {
        Vector2 from = this.Position + new Vector2(3f, (double) this.facing.X < 0.0 ? -5f : -4f) * this.sprite.Scale;
        Vector2 vector = Calc.AngleToVector(Calc.Angle(from, this.lastPlayerPos + new Vector2(0.0f, -4f)) + (float) ((double) this.eyeSpin * 6.2831854820251465 * 2.0), 1f);
        Vector2 vector2 = from + new Vector2((float) Math.Round((double) vector.X), (float) Math.Round((double) Calc.ClampedMap(vector.Y, -1f, 1f, -1f, 2f)));
        Draw.Rect(vector2.X, vector2.Y, 1f, 1f, Color.Black);
      }
      Sprite sprite2 = this.sprite;
      sprite2.Scale = sprite2.Scale / this.facing;
    }

    public override void Update()
    {
      base.Update();
      this.eyeSpin = Calc.Approach(this.eyeSpin, 0.0f, Engine.DeltaTime * 1.5f);
      this.scale = Calc.Approach(this.scale, Vector2.One, 1f * Engine.DeltaTime);
      if ((double) this.cannotHitTimer > 0.0)
        this.cannotHitTimer -= Engine.DeltaTime;
      if (this.state != Puffer.States.Gone && (double) this.cantExplodeTimer > 0.0)
        this.cantExplodeTimer -= Engine.DeltaTime;
      if ((double) this.alertTimer > 0.0)
        this.alertTimer -= Engine.DeltaTime;
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity == null)
      {
        this.playerAliveFade = Calc.Approach(this.playerAliveFade, 0.0f, 1f * Engine.DeltaTime);
      }
      else
      {
        this.playerAliveFade = Calc.Approach(this.playerAliveFade, 1f, 1f * Engine.DeltaTime);
        this.lastPlayerPos = entity.Center;
      }
      switch (this.state)
      {
        case Puffer.States.Idle:
          if (this.Position != this.lastSinePosition)
            this.anchorPosition += this.Position - this.lastSinePosition;
          Vector2 vector2 = this.anchorPosition + new Vector2(this.idleSine.Value * 3f, this.idleSine.ValueOverTwo * 2f);
          this.MoveToX(vector2.X);
          this.MoveToY(vector2.Y);
          this.lastSinePosition = this.Position;
          if (this.ProximityExplodeCheck())
          {
            this.Explode();
            this.GotoGone();
            break;
          }
          if (this.AlertedCheck())
            this.Alert(false, true);
          else if (this.sprite.CurrentAnimationID == "alerted" && (double) this.alertTimer <= 0.0)
          {
            Audio.Play("event:/new_content/game/10_farewell/puffer_shrink", this.Position);
            this.sprite.Play("unalert");
          }
          using (List<Component>.Enumerator enumerator = this.Scene.Tracker.GetComponents<PufferCollider>().GetEnumerator())
          {
            while (enumerator.MoveNext())
              ((PufferCollider) enumerator.Current).Check(this);
            break;
          }
        case Puffer.States.Hit:
          this.lastSpeedPosition = this.Position;
          this.MoveH(this.hitSpeed.X * Engine.DeltaTime, this.onCollideH);
          this.MoveV(this.hitSpeed.Y * Engine.DeltaTime, new Collision(this.OnCollideV));
          this.anchorPosition = this.Position;
          this.hitSpeed.X = Calc.Approach(this.hitSpeed.X, 0.0f, 150f * Engine.DeltaTime);
          this.hitSpeed = Calc.Approach(this.hitSpeed, Vector2.Zero, 320f * Engine.DeltaTime);
          if (this.ProximityExplodeCheck())
          {
            this.Explode();
            this.GotoGone();
            break;
          }
          if ((double) this.Top >= (double) (this.SceneAs<Level>().Bounds.Bottom + 5))
          {
            this.sprite.Play("hidden");
            this.GotoGone();
            break;
          }
          foreach (PufferCollider component in this.Scene.Tracker.GetComponents<PufferCollider>())
            component.Check(this);
          if (!(this.hitSpeed == Vector2.Zero))
            break;
          this.ZeroRemainderX();
          this.ZeroRemainderY();
          this.GotoIdle();
          break;
        case Puffer.States.Gone:
          float goneTimer = this.goneTimer;
          this.goneTimer -= Engine.DeltaTime;
          if ((double) this.goneTimer <= 0.5)
          {
            if ((double) goneTimer > 0.5 && (double) this.returnCurve.GetLengthParametric(8) > 8.0)
              Audio.Play("event:/new_content/game/10_farewell/puffer_return", this.Position);
            this.Position = this.returnCurve.GetPoint(Ease.CubeInOut(Calc.ClampedMap(this.goneTimer, 0.5f, 0.0f)));
          }
          if ((double) this.goneTimer > 0.0)
            break;
          this.Visible = this.Collidable = true;
          this.GotoIdle();
          break;
      }
    }

    public bool HitSpring(Spring spring)
    {
      switch (spring.Orientation)
      {
        case Spring.Orientations.WallLeft:
          if ((double) this.hitSpeed.X > 60.0)
            return false;
          this.facing.X = 1f;
          this.GotoHitSpeed(280f * Vector2.UnitX);
          this.MoveTowardsY(spring.CenterY, 4f);
          this.bounceWiggler.Start();
          this.Alert(true, false);
          return true;
        case Spring.Orientations.WallRight:
          if ((double) this.hitSpeed.X < -60.0)
            return false;
          this.facing.X = -1f;
          this.GotoHitSpeed(280f * -Vector2.UnitX);
          this.MoveTowardsY(spring.CenterY, 4f);
          this.bounceWiggler.Start();
          this.Alert(true, false);
          return true;
        default:
          if ((double) this.hitSpeed.Y < 0.0)
            return false;
          this.GotoHitSpeed(224f * -Vector2.UnitY);
          this.MoveTowardsX(spring.CenterX, 4f);
          this.bounceWiggler.Start();
          this.Alert(true, false);
          return true;
      }
    }

    private bool ProximityExplodeCheck()
    {
      if ((double) this.cantExplodeTimer > 0.0)
        return false;
      bool flag = false;
      Collider collider = this.Collider;
      this.Collider = (Collider) this.detectRadius;
      Player player;
      if ((player = this.CollideFirst<Player>()) != null && (double) player.CenterY >= (double) this.Y + (double) collider.Bottom - 4.0 && !this.Scene.CollideCheck<Solid>(this.Position, player.Center))
        flag = true;
      this.Collider = collider;
      return flag;
    }

    private bool AlertedCheck()
    {
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      return entity != null && (double) (entity.Center - this.Center).Length() < 60.0;
    }

    private void Alert(bool restart, bool playSfx)
    {
      if (this.sprite.CurrentAnimationID == "idle")
      {
        if (playSfx)
          Audio.Play("event:/new_content/game/10_farewell/puffer_expand", this.Position);
        this.sprite.Play("alert");
        this.inflateWiggler.Start();
      }
      else if (restart && playSfx)
        Audio.Play("event:/new_content/game/10_farewell/puffer_expand", this.Position);
      this.alertTimer = 2f;
    }

    private void OnPlayer(Player player)
    {
      if (this.state == Puffer.States.Gone || (double) this.cantExplodeTimer > 0.0)
        return;
      if ((double) this.cannotHitTimer <= 0.0)
      {
        if ((double) player.Bottom > (double) this.lastSpeedPosition.Y + 3.0)
        {
          this.Explode();
          this.GotoGone();
        }
        else
        {
          player.Bounce(this.Top);
          this.GotoHit(player.Center);
          this.MoveToX(this.anchorPosition.X);
          this.idleSine.Reset();
          this.anchorPosition = this.lastSinePosition = this.Position;
          this.eyeSpin = 1f;
        }
      }
      this.cannotHitTimer = 0.1f;
    }

    private enum States
    {
      Idle,
      Hit,
      Gone,
    }
  }
}
