// Decompiled with JetBrains decompiler
// Type: Celeste.Seeker
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(false)]
  public class Seeker : Actor
  {
    public static readonly Color TrailColor = Calc.HexToColor("99e550");
    private static Seeker.PatrolPoint[] patrolChoices = new Seeker.PatrolPoint[3];
    private int facing = 1;
    private int spriteFacing = 1;
    private HashSet<string> flipAnimations = new HashSet<string>()
    {
      "flipMouth",
      "flipEyes",
      "skid"
    };
    private float spottedTurnDelay = 0.0f;
    public static ParticleType P_Attack;
    public static ParticleType P_HitWall;
    public static ParticleType P_Stomp;
    public static ParticleType P_Regen;
    public static ParticleType P_BreakOut;
    private const int StIdle = 0;
    private const int StPatrol = 1;
    private const int StSpotted = 2;
    private const int StAttack = 3;
    private const int StStunned = 4;
    private const int StSkidding = 5;
    private const int StRegenerate = 6;
    private const int StReturned = 7;
    private const int size = 12;
    private const int bounceWidth = 16;
    private const int bounceHeight = 4;
    private const float Accel = 600f;
    private const float WallCollideStunThreshold = 100f;
    private const float StunXSpeed = 100f;
    private const float BounceSpeed = 200f;
    private const float SightDistSq = 25600f;
    private const float ExplodeRadius = 40f;
    private Hitbox physicsHitbox;
    private Hitbox breakWallsHitbox;
    private Hitbox attackHitbox;
    private Hitbox bounceHitbox;
    private Monocle.Circle pushRadius;
    private Monocle.Circle breakWallsRadius;
    private StateMachine State;
    private Vector2 lastSpottedAt;
    private Vector2 lastPathTo;
    private bool spotted;
    private bool canSeePlayer;
    private Collision onCollideH;
    private Collision onCollideV;
    private Random random;
    private Vector2 lastPosition;
    private Shaker shaker;
    private Wiggler scaleWiggler;
    private bool lastPathFound;
    private List<Vector2> path;
    private int pathIndex;
    private Vector2[] patrolPoints;
    private SineWave idleSineX;
    private SineWave idleSineY;
    public VertexLight Light;
    private bool dead;
    private SoundSource boopedSfx;
    private SoundSource aggroSfx;
    private SoundSource reviveSfx;
    private Sprite sprite;
    private string nextSprite;
    private HoldableCollider theo;
    public Vector2 Speed;
    private const float FarDistSq = 12544f;
    private const float IdleAccel = 200f;
    private const float IdleSpeed = 50f;
    private const float PatrolSpeed = 25f;
    private const int PatrolChoices = 3;
    private const float PatrolWaitTime = 0.4f;
    private float patrolWaitTimer;
    private const float SpottedTargetSpeed = 60f;
    private const float SpottedFarSpeed = 90f;
    private const float SpottedMaxYDist = 24f;
    private const float AttackMinXDist = 16f;
    private const float SpottedLosePlayerTime = 0.6f;
    private const float SpottedMinAttackTime = 0.2f;
    private float spottedLosePlayerTimer;
    private const float AttackWindUpSpeed = -60f;
    private const float AttackWindUpTime = 0.3f;
    private const float AttackStartSpeed = 180f;
    private const float AttackTargetSpeed = 260f;
    private const float AttackAccel = 300f;
    private const float DirectionDotThreshold = 0.4f;
    private const int AttackTargetUpShift = 2;
    private const float AttackMaxRotateRadians = 0.6108652f;
    private float attackSpeed;
    private bool attackWindUp;
    private const float StunnedAccel = 150f;
    private const float StunTime = 0.8f;
    private const float SkiddingAccel = 200f;
    private const float StrongSkiddingAccel = 400f;
    private const float StrongSkiddingTime = 0.08f;
    private bool strongSkid;

    public Seeker(Vector2 position, Vector2[] patrolPoints)
      : base(position)
    {
      this.Depth = -200;
      this.patrolPoints = patrolPoints;
      this.lastPosition = position;
      this.Collider = (Collider) (this.physicsHitbox = new Hitbox(6f, 6f, -3f, -3f));
      this.breakWallsHitbox = new Hitbox(6f, 14f, -3f, -7f);
      this.attackHitbox = new Hitbox(12f, 8f, -6f, -2f);
      this.bounceHitbox = new Hitbox(16f, 6f, -8f, -8f);
      this.pushRadius = new Monocle.Circle(40f, 0.0f, 0.0f);
      this.breakWallsRadius = new Monocle.Circle(16f, 0.0f, 0.0f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnAttackPlayer), (Collider) this.attackHitbox, (Collider) null));
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnBouncePlayer), (Collider) this.bounceHitbox, (Collider) null));
      this.Add((Component) (this.shaker = new Shaker(false, (Action<Vector2>) null)));
      this.Add((Component) (this.State = new StateMachine(10)));
      this.State.SetCallbacks(0, new Func<int>(this.IdleUpdate), new Func<IEnumerator>(this.IdleCoroutine), (Action) null, (Action) null);
      this.State.SetCallbacks(1, new Func<int>(this.PatrolUpdate), (Func<IEnumerator>) null, new Action(this.PatrolBegin), (Action) null);
      this.State.SetCallbacks(2, new Func<int>(this.SpottedUpdate), new Func<IEnumerator>(this.SpottedCoroutine), new Action(this.SpottedBegin), (Action) null);
      this.State.SetCallbacks(3, new Func<int>(this.AttackUpdate), new Func<IEnumerator>(this.AttackCoroutine), new Action(this.AttackBegin), (Action) null);
      this.State.SetCallbacks(4, new Func<int>(this.StunnedUpdate), new Func<IEnumerator>(this.StunnedCoroutine), (Action) null, (Action) null);
      this.State.SetCallbacks(5, new Func<int>(this.SkiddingUpdate), new Func<IEnumerator>(this.SkiddingCoroutine), new Action(this.SkiddingBegin), new Action(this.SkiddingEnd));
      this.State.SetCallbacks(6, new Func<int>(this.RegenerateUpdate), new Func<IEnumerator>(this.RegenerateCoroutine), new Action(this.RegenerateBegin), new Action(this.RegenerateEnd));
      this.State.SetCallbacks(7, (Func<int>) null, new Func<IEnumerator>(this.ReturnedCoroutine), (Action) null, (Action) null);
      this.onCollideH = new Collision(this.OnCollideH);
      this.onCollideV = new Collision(this.OnCollideV);
      this.Add((Component) (this.idleSineX = new SineWave(0.5f)));
      this.Add((Component) (this.idleSineY = new SineWave(0.7f)));
      this.Add((Component) (this.Light = new VertexLight(Color.White, 1f, 32, 64)));
      this.Add((Component) (this.theo = new HoldableCollider(new Action<Holdable>(this.OnHoldable), (Collider) this.attackHitbox)));
      this.Add((Component) new MirrorReflection());
      this.path = new List<Vector2>();
      this.IgnoreJumpThrus = true;
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("seeker")));
      this.sprite.OnLastFrame = (Action<string>) (f =>
      {
        if (!this.flipAnimations.Contains(f) || this.spriteFacing == this.facing)
          return;
        this.spriteFacing = this.facing;
        if (this.nextSprite != null)
        {
          this.sprite.Play(this.nextSprite, false, false);
          this.nextSprite = (string) null;
        }
      });
      this.sprite.OnChange = (Action<string, string>) ((last, next) =>
      {
        this.nextSprite = (string) null;
        this.sprite.OnLastFrame(last);
      });
      this.SquishCallback = (Collision) (d =>
      {
        if (this.dead || this.TrySquishWiggle(d))
          return;
        Entity entity = new Entity(this.Position);
        entity.Add((Component) new DeathEffect(Color.HotPink, new Vector2?(this.Center - this.Position))
        {
          OnEnd = (Action) (() => entity.RemoveSelf())
        });
        entity.Depth = -1000000;
        this.Scene.Add(entity);
        Audio.Play("event:/game/05_mirror_temple/seeker_death", this.Position);
        this.RemoveSelf();
        this.dead = true;
      });
      this.scaleWiggler = Wiggler.Create(0.8f, 2f, (Action<float>) null, false, false);
      this.Add((Component) this.scaleWiggler);
      this.Add((Component) (this.boopedSfx = new SoundSource()));
      this.Add((Component) (this.aggroSfx = new SoundSource()));
      this.Add((Component) (this.reviveSfx = new SoundSource()));
    }

    public Seeker(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.NodesOffset(offset))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.random = new Random(this.SceneAs<Level>().Session.LevelData.LoadSeed);
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity == null || (double) this.X == (double) entity.X)
        this.SnapFacing(1f);
      else
        this.SnapFacing((float) Math.Sign(entity.X - this.X));
    }

    public override bool IsRiding(JumpThru jumpThru)
    {
      return false;
    }

    public override bool IsRiding(Solid solid)
    {
      return false;
    }

    public bool Attacking
    {
      get
      {
        return this.State.State == 3 && !this.attackWindUp;
      }
    }

    public bool Spotted
    {
      get
      {
        return this.State.State == 3 || this.State.State == 2;
      }
    }

    public bool Regenerating
    {
      get
      {
        return this.State.State == 6;
      }
    }

    private void OnAttackPlayer(Player player)
    {
      if (this.State.State != 4)
      {
        player.Die((player.Center - this.Position).SafeNormalize(), false, true);
      }
      else
      {
        Collider collider = this.Collider;
        this.Collider = (Collider) this.bounceHitbox;
        player.PointBounce(this.Center);
        this.Speed = (this.Center - player.Center).SafeNormalize(100f);
        this.scaleWiggler.Start();
        this.Collider = collider;
      }
    }

    private void OnBouncePlayer(Player player)
    {
      Collider collider = this.Collider;
      this.Collider = (Collider) this.attackHitbox;
      if (this.CollideCheck((Entity) player))
      {
        this.OnAttackPlayer(player);
      }
      else
      {
        player.Bounce(this.Top);
        this.GotBouncedOn((Entity) player);
      }
      this.Collider = collider;
    }

    private void GotBouncedOn(Entity entity)
    {
      Celeste.Freeze(0.15f);
      this.Speed = (this.Center - entity.Center).SafeNormalize(200f);
      this.State.State = 6;
      this.sprite.Scale = new Vector2(1.4f, 0.6f);
      this.SceneAs<Level>().Particles.Emit(Seeker.P_Stomp, 8, this.Center - Vector2.UnitY * 5f, new Vector2(6f, 3f));
    }

    public void HitSpring()
    {
      this.Speed.Y = -150f;
    }

    private bool CanSeePlayer(Player player)
    {
      if (player == null || this.State.State != 2 && !this.SceneAs<Level>().InsideCamera(this.Center, 0.0f) && (double) Vector2.DistanceSquared(this.Center, player.Center) > 25600.0)
        return false;
      Vector2 vector2 = (player.Center - this.Center).Perpendicular().SafeNormalize(2f);
      return !this.Scene.CollideCheck<Solid>(this.Center + vector2, player.Center + vector2) && !this.Scene.CollideCheck<Solid>(this.Center - vector2, player.Center - vector2);
    }

    public override void Update()
    {
      this.Light.Alpha = Calc.Approach(this.Light.Alpha, 1f, Engine.DeltaTime * 2f);
      foreach (Entity entity in this.Scene.Tracker.GetEntities<SeekerBarrier>())
        entity.Collidable = true;
      this.sprite.Scale.X = Calc.Approach(this.sprite.Scale.X, 1f, 2f * Engine.DeltaTime);
      this.sprite.Scale.Y = Calc.Approach(this.sprite.Scale.Y, 1f, 2f * Engine.DeltaTime);
      if (this.State.State == 6)
      {
        this.canSeePlayer = false;
      }
      else
      {
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        this.canSeePlayer = this.CanSeePlayer(entity);
        if (this.canSeePlayer)
        {
          this.spotted = true;
          this.lastSpottedAt = entity.Center;
        }
      }
      if (this.lastPathTo != this.lastSpottedAt)
      {
        this.lastPathTo = this.lastSpottedAt;
        this.pathIndex = 0;
        this.lastPathFound = this.SceneAs<Level>().Pathfinder.Find(ref this.path, this.Center, this.FollowTarget, true, false);
      }
      base.Update();
      this.lastPosition = this.Position;
      this.MoveH(this.Speed.X * Engine.DeltaTime, this.onCollideH, (Solid) null);
      this.MoveV(this.Speed.Y * Engine.DeltaTime, this.onCollideV, (Solid) null);
      Level level = this.SceneAs<Level>();
      double left1 = (double) this.Left;
      Rectangle bounds = level.Bounds;
      double left2 = (double) bounds.Left;
      if (left1 < left2 && (double) this.Speed.X < 0.0)
      {
        bounds = level.Bounds;
        this.Left = (float) bounds.Left;
        this.onCollideH(CollisionData.Empty);
      }
      else
      {
        double right1 = (double) this.Right;
        bounds = level.Bounds;
        double right2 = (double) bounds.Right;
        if (right1 > right2 && (double) this.Speed.X > 0.0)
        {
          bounds = level.Bounds;
          this.Right = (float) bounds.Right;
          this.onCollideH(CollisionData.Empty);
        }
      }
      double top = (double) this.Top;
      bounds = level.Bounds;
      double num = (double) (bounds.Top - 8);
      if (top < num && (double) this.Speed.Y < 0.0)
      {
        bounds = level.Bounds;
        this.Top = (float) (bounds.Top - 8);
        this.onCollideV(CollisionData.Empty);
      }
      else
      {
        double bottom1 = (double) this.Bottom;
        bounds = level.Bounds;
        double bottom2 = (double) bounds.Bottom;
        if (bottom1 > bottom2 && (double) this.Speed.Y > 0.0)
        {
          bounds = level.Bounds;
          this.Bottom = (float) bounds.Bottom;
          this.onCollideV(CollisionData.Empty);
        }
      }
      foreach (SeekerCollider component in this.Scene.Tracker.GetComponents<SeekerCollider>())
        component.Check(this);
      if (this.State.State == 3 && (double) this.Speed.X > 0.0)
      {
        this.bounceHitbox.Width = 16f;
        this.bounceHitbox.Position.X = -10f;
      }
      else if (this.State.State == 3 && (double) this.Speed.Y < 0.0)
      {
        this.bounceHitbox.Width = 16f;
        this.bounceHitbox.Position.X = -6f;
      }
      else
      {
        this.bounceHitbox.Width = 12f;
        this.bounceHitbox.Position.X = -6f;
      }
      foreach (Entity entity in this.Scene.Tracker.GetEntities<SeekerBarrier>())
        entity.Collidable = false;
    }

    private void TurnFacing(float dir, string gotoSprite = null)
    {
      if ((double) dir != 0.0)
        this.facing = Math.Sign(dir);
      if (this.spriteFacing != this.facing)
      {
        if (this.State.State == 5)
          this.sprite.Play("skid", false, false);
        else if (this.State.State == 3 || this.State.State == 2)
          this.sprite.Play("flipMouth", false, false);
        else
          this.sprite.Play("flipEyes", false, false);
        this.nextSprite = gotoSprite;
      }
      else
      {
        if (gotoSprite == null)
          return;
        this.sprite.Play(gotoSprite, false, false);
      }
    }

    private void SnapFacing(float dir)
    {
      if ((double) dir == 0.0)
        return;
      this.spriteFacing = this.facing = Math.Sign(dir);
    }

    private void OnHoldable(Holdable holdable)
    {
      if (this.State.State != 6 && holdable.Dangerous(this.theo))
      {
        holdable.HitSeeker(this);
        this.State.State = 4;
        this.Speed = (this.Center - holdable.Entity.Center).SafeNormalize(120f);
        this.scaleWiggler.Start();
      }
      else
      {
        if (this.State.State != 3 && this.State.State != 5 || !holdable.IsHeld)
          return;
        holdable.Swat(this.theo, Math.Sign(this.Speed.X));
        this.State.State = 4;
        this.Speed = (this.Center - holdable.Entity.Center).SafeNormalize(120f);
        this.scaleWiggler.Start();
      }
    }

    public override void Render()
    {
      Vector2 position = this.Position;
      this.Position = this.Position + this.shaker.Value;
      Vector2 scale = this.sprite.Scale;
      Sprite sprite = this.sprite;
      sprite.Scale = sprite.Scale * (float) (1.0 - 0.300000011920929 * (double) this.scaleWiggler.Value);
      this.sprite.Scale.X *= (float) this.spriteFacing;
      base.Render();
      this.Position = position;
      this.sprite.Scale = scale;
    }

    public override void DebugRender(Camera camera)
    {
      Collider collider = this.Collider;
      this.Collider = (Collider) this.attackHitbox;
      this.attackHitbox.Render(camera, Color.Red);
      this.Collider = (Collider) this.bounceHitbox;
      this.bounceHitbox.Render(camera, Color.Aqua);
      this.Collider = collider;
    }

    private void SlammedIntoWall(CollisionData data)
    {
      float direction;
      float x;
      if ((double) data.Direction.X > 0.0)
      {
        direction = 3.141593f;
        x = this.Right;
      }
      else
      {
        direction = 0.0f;
        x = this.Left;
      }
      this.SceneAs<Level>().Particles.Emit(Seeker.P_HitWall, 12, new Vector2(x, this.Y), Vector2.UnitY * 4f, direction);
      if (data.Hit is DashSwitch)
      {
        int num = (int) (data.Hit as DashSwitch).OnDashCollide((Player) null, Vector2.UnitX * (float) Math.Sign(this.Speed.X));
      }
      this.Collider = (Collider) this.breakWallsHitbox;
      foreach (TempleCrackedBlock entity in this.Scene.Tracker.GetEntities<TempleCrackedBlock>())
      {
        if (this.CollideCheck((Entity) entity, this.Position + Vector2.UnitX * (float) Math.Sign(this.Speed.X)))
          entity.Break(this.Center);
      }
      this.Collider = (Collider) this.physicsHitbox;
      this.SceneAs<Level>().DirectionalShake(Vector2.UnitX * (float) Math.Sign(this.Speed.X), 0.3f);
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      this.Speed.X = (float) Math.Sign(this.Speed.X) * -100f;
      this.Speed.Y *= 0.4f;
      this.sprite.Scale.X = 0.6f;
      this.sprite.Scale.Y = 1.4f;
      this.shaker.ShakeFor(0.5f, false);
      this.scaleWiggler.Start();
      this.State.State = 4;
      if (data.Hit is SeekerBarrier)
      {
        (data.Hit as SeekerBarrier).OnReflectSeeker();
        Audio.Play("event:/game/05_mirror_temple/seeker_hit_lightwall", this.Position);
      }
      else
        Audio.Play("event:/game/05_mirror_temple/seeker_hit_normal", this.Position);
    }

    private void OnCollideH(CollisionData data)
    {
      if (this.State.State == 3 && data.Hit != null)
      {
        int num = Math.Sign(this.Speed.X);
        if (!this.CollideCheck<Solid>(this.Position + new Vector2((float) num, 4f)) && !this.MoveVExact(4, (Collision) null, (Solid) null) || !this.CollideCheck<Solid>(this.Position + new Vector2((float) num, -4f)) && !this.MoveVExact(-4, (Collision) null, (Solid) null))
          return;
      }
      if ((this.State.State == 3 || this.State.State == 5) && (double) Math.Abs(this.Speed.X) >= 100.0)
        this.SlammedIntoWall(data);
      else
        this.Speed.X *= -0.2f;
    }

    private void OnCollideV(CollisionData data)
    {
      if (this.State.State == 3)
        this.Speed.Y *= -0.6f;
      else
        this.Speed.Y *= -0.2f;
    }

    private Vector2 FollowTarget
    {
      get
      {
        return this.lastSpottedAt - Vector2.UnitY * 2f;
      }
    }

    private void CreateTrail()
    {
      Vector2 scale = this.sprite.Scale;
      Sprite sprite = this.sprite;
      sprite.Scale = sprite.Scale * (float) (1.0 - 0.300000011920929 * (double) this.scaleWiggler.Value);
      this.sprite.Scale.X *= (float) this.spriteFacing;
      TrailManager.Add((Entity) this, Seeker.TrailColor, 0.5f);
      this.sprite.Scale = scale;
    }

    private int IdleUpdate()
    {
      if (this.canSeePlayer)
        return 2;
      Vector2 target = Vector2.Zero;
      if (this.spotted && (double) Vector2.DistanceSquared(this.Center, this.FollowTarget) > 64.0)
      {
        float speedMagnitude = this.GetSpeedMagnitude(50f);
        target = !this.lastPathFound ? (this.FollowTarget - this.Center).SafeNormalize(speedMagnitude) : this.GetPathSpeed(speedMagnitude);
      }
      if (target == Vector2.Zero)
      {
        target.X = this.idleSineX.Value * 6f;
        target.Y = this.idleSineY.Value * 6f;
      }
      this.Speed = Calc.Approach(this.Speed, target, 200f * Engine.DeltaTime);
      if ((double) this.Speed.LengthSquared() > 400.0)
        this.TurnFacing(this.Speed.X, (string) null);
      if (this.spriteFacing == this.facing)
        this.sprite.Play("idle", false, false);
      return 0;
    }

    private IEnumerator IdleCoroutine()
    {
      if (this.patrolPoints != null && this.patrolPoints.Length != 0 && this.spotted)
      {
        while ((double) Vector2.DistanceSquared(this.Center, this.FollowTarget) > 64.0)
          yield return (object) null;
        yield return (object) 0.3f;
        this.State.State = 1;
      }
    }

    private Vector2 GetPathSpeed(float magnitude)
    {
      if (this.pathIndex >= this.path.Count)
        return Vector2.Zero;
      if ((double) Vector2.DistanceSquared(this.Center, this.path[this.pathIndex]) >= 36.0)
        return (this.path[this.pathIndex] - this.Center).SafeNormalize(magnitude);
      ++this.pathIndex;
      return this.GetPathSpeed(magnitude);
    }

    private float GetSpeedMagnitude(float baseMagnitude)
    {
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity == null)
        return baseMagnitude;
      if ((double) Vector2.DistanceSquared(this.Center, entity.Center) > 12544.0)
        return baseMagnitude * 3f;
      return baseMagnitude * 1.5f;
    }

    private void PatrolBegin()
    {
      this.State.State = this.ChoosePatrolTarget();
      this.patrolWaitTimer = 0.0f;
    }

    private int PatrolUpdate()
    {
      if (this.canSeePlayer)
        return 2;
      if ((double) this.patrolWaitTimer > 0.0)
      {
        this.patrolWaitTimer -= Engine.DeltaTime;
        if ((double) this.patrolWaitTimer <= 0.0)
          return this.ChoosePatrolTarget();
      }
      else if ((double) Vector2.DistanceSquared(this.Center, this.lastSpottedAt) < 144.0)
        this.patrolWaitTimer = 0.4f;
      float speedMagnitude = this.GetSpeedMagnitude(25f);
      this.Speed = Calc.Approach(this.Speed, !this.lastPathFound ? (this.FollowTarget - this.Center).SafeNormalize(speedMagnitude) : this.GetPathSpeed(speedMagnitude), 600f * Engine.DeltaTime);
      if ((double) this.Speed.LengthSquared() > 100.0)
        this.TurnFacing(this.Speed.X, (string) null);
      if (this.spriteFacing == this.facing)
        this.sprite.Play("search", false, false);
      return 1;
    }

    private int ChoosePatrolTarget()
    {
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity == null)
        return 0;
      for (int index = 0; index < 3; ++index)
        Seeker.patrolChoices[index].Distance = 0.0f;
      int val2 = 0;
      foreach (Vector2 patrolPoint in this.patrolPoints)
      {
        if ((double) Vector2.DistanceSquared(this.Center, patrolPoint) >= 576.0)
        {
          float num = Vector2.DistanceSquared(patrolPoint, entity.Center);
          for (int index1 = 0; index1 < 3; ++index1)
          {
            if ((double) num < (double) Seeker.patrolChoices[index1].Distance || (double) Seeker.patrolChoices[index1].Distance <= 0.0)
            {
              ++val2;
              for (int index2 = 2; index2 > index1; --index2)
              {
                Seeker.patrolChoices[index2].Distance = Seeker.patrolChoices[index2 - 1].Distance;
                Seeker.patrolChoices[index2].Point = Seeker.patrolChoices[index2 - 1].Point;
              }
              Seeker.patrolChoices[index1].Distance = num;
              Seeker.patrolChoices[index1].Point = patrolPoint;
              break;
            }
          }
        }
      }
      if (val2 <= 0)
        return 0;
      this.lastSpottedAt = Seeker.patrolChoices[this.random.Next(Math.Min(3, val2))].Point;
      this.lastPathTo = this.lastSpottedAt;
      this.pathIndex = 0;
      this.lastPathFound = this.SceneAs<Level>().Pathfinder.Find(ref this.path, this.Center, this.FollowTarget, true, false);
      return 1;
    }

    private void SpottedBegin()
    {
      this.aggroSfx.Play("event:/game/05_mirror_temple/seeker_aggro", (string) null, 0.0f);
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity != null)
        this.TurnFacing(entity.X - this.X, "spot");
      this.spottedLosePlayerTimer = 0.6f;
      this.spottedTurnDelay = 1f;
    }

    private int SpottedUpdate()
    {
      if (!this.canSeePlayer)
      {
        this.spottedLosePlayerTimer -= Engine.DeltaTime;
        if ((double) this.spottedLosePlayerTimer < 0.0)
          return 0;
      }
      else
        this.spottedLosePlayerTimer = 0.6f;
      float speedMagnitude = this.GetSpeedMagnitude(60f);
      Vector2 vector2_1 = !this.lastPathFound ? (this.FollowTarget - this.Center).SafeNormalize(speedMagnitude) : this.GetPathSpeed(speedMagnitude);
      if ((double) Vector2.DistanceSquared(this.Center, this.FollowTarget) < 2500.0 && (double) this.Y < (double) this.FollowTarget.Y)
      {
        float num = vector2_1.Angle();
        if ((double) this.Y < (double) this.FollowTarget.Y - 2.0)
          num = Calc.AngleLerp(num, 1.570796f, 0.5f);
        else if ((double) this.Y > (double) this.FollowTarget.Y + 2.0)
          num = Calc.AngleLerp(num, -1.570796f, 0.5f);
        vector2_1 = Calc.AngleToVector(num, 60f);
        Vector2 vector2_2 = Vector2.UnitX * (float) Math.Sign(this.X - this.lastSpottedAt.X) * 48f;
        if ((double) Math.Abs(this.X - this.lastSpottedAt.X) < 36.0 && !this.CollideCheck<Solid>(this.Position + vector2_2) && !this.CollideCheck<Solid>(this.lastSpottedAt + vector2_2))
          vector2_1.X = (float) (Math.Sign(this.X - this.lastSpottedAt.X) * 60);
      }
      this.Speed = Calc.Approach(this.Speed, vector2_1, 600f * Engine.DeltaTime);
      this.spottedTurnDelay -= Engine.DeltaTime;
      if ((double) this.spottedTurnDelay <= 0.0)
        this.TurnFacing(this.Speed.X, "spotted");
      return 2;
    }

    private IEnumerator SpottedCoroutine()
    {
      yield return (object) 0.2f;
      while (!this.CanAttack())
        yield return (object) null;
      this.State.State = 3;
    }

    private bool CanAttack()
    {
      if ((double) Math.Abs(this.Y - this.lastSpottedAt.Y) > 24.0 || (double) Math.Abs(this.X - this.lastSpottedAt.X) < 16.0)
        return false;
      Vector2 vector2 = (this.FollowTarget - this.Center).SafeNormalize();
      return (double) Vector2.Dot(-Vector2.UnitY, vector2) <= 0.5 && (double) Vector2.Dot(Vector2.UnitY, vector2) <= 0.5 && !this.CollideCheck<Solid>(this.Position + Vector2.UnitX * (float) Math.Sign(this.lastSpottedAt.X - this.X) * 24f);
    }

    private void AttackBegin()
    {
      Audio.Play("event:/game/05_mirror_temple/seeker_dash", this.Position);
      this.attackWindUp = true;
      this.attackSpeed = -60f;
      this.Speed = (this.FollowTarget - this.Center).SafeNormalize(-60f);
    }

    private int AttackUpdate()
    {
      if (!this.attackWindUp)
      {
        Vector2 vector1 = (this.FollowTarget - this.Center).SafeNormalize();
        if ((double) Vector2.Dot(this.Speed.SafeNormalize(), vector1) < 0.400000005960464)
          return 5;
        this.attackSpeed = Calc.Approach(this.attackSpeed, 260f, 300f * Engine.DeltaTime);
        this.Speed = this.Speed.RotateTowards(vector1.Angle(), 7f * (float) Math.PI / 36f * Engine.DeltaTime).SafeNormalize(this.attackSpeed);
        if (this.Scene.OnInterval(0.04f))
        {
          Vector2 vector2 = (-this.Speed).SafeNormalize();
          this.SceneAs<Level>().Particles.Emit(Seeker.P_Attack, 2, this.Position + vector2 * 4f, Vector2.One * 4f, vector2.Angle());
        }
        if (this.Scene.OnInterval(0.06f))
          this.CreateTrail();
      }
      return 3;
    }

    private IEnumerator AttackCoroutine()
    {
      this.TurnFacing(this.lastSpottedAt.X - this.X, "windUp");
      yield return (object) 0.3f;
      this.attackWindUp = false;
      this.attackSpeed = 180f;
      this.Speed = (this.lastSpottedAt - Vector2.UnitY * 2f - this.Center).SafeNormalize(180f);
      this.SnapFacing(this.Speed.X);
    }

    private int StunnedUpdate()
    {
      this.Speed = Calc.Approach(this.Speed, Vector2.Zero, 150f * Engine.DeltaTime);
      return 4;
    }

    private IEnumerator StunnedCoroutine()
    {
      yield return (object) 0.8f;
      this.State.State = 0;
    }

    private void SkiddingBegin()
    {
      Audio.Play("event:/game/05_mirror_temple/seeker_dash_turn", this.Position);
      this.strongSkid = false;
      this.TurnFacing((float) -this.facing, (string) null);
    }

    private int SkiddingUpdate()
    {
      this.Speed = Calc.Approach(this.Speed, Vector2.Zero, (this.strongSkid ? 400f : 200f) * Engine.DeltaTime);
      if ((double) this.Speed.LengthSquared() >= 400.0)
        return 5;
      return this.canSeePlayer ? 2 : 0;
    }

    private IEnumerator SkiddingCoroutine()
    {
      yield return (object) 0.08f;
      this.strongSkid = true;
    }

    private void SkiddingEnd()
    {
      this.spriteFacing = this.facing;
    }

    private void RegenerateBegin()
    {
      Audio.Play("event:/game/general/thing_booped", this.Position);
      this.boopedSfx.Play("event:/game/05_mirror_temple/seeker_booped", (string) null, 0.0f);
      this.sprite.Play("takeHit", false, false);
      this.Collidable = false;
      this.State.Locked = true;
      this.Light.StartRadius = 16f;
      this.Light.EndRadius = 32f;
    }

    private void RegenerateEnd()
    {
      this.reviveSfx.Play("event:/game/05_mirror_temple/seeker_revive", (string) null, 0.0f);
      this.Collidable = true;
      this.Light.StartRadius = 32f;
      this.Light.EndRadius = 64f;
    }

    private int RegenerateUpdate()
    {
      this.Speed.X = Calc.Approach(this.Speed.X, 0.0f, 150f * Engine.DeltaTime);
      this.Speed = Calc.Approach(this.Speed, Vector2.Zero, 150f * Engine.DeltaTime);
      return 6;
    }

    private IEnumerator RegenerateCoroutine()
    {
      yield return (object) 1f;
      this.shaker.On = true;
      yield return (object) 0.2f;
      this.sprite.Play("pulse", false, false);
      yield return (object) 0.5f;
      this.sprite.Play("recover", false, false);
      Seeker.RecoverBlast.Spawn(this.Position);
      yield return (object) 0.15f;
      this.Collider = (Collider) this.pushRadius;
      Player player = this.CollideFirst<Player>();
      if (player != null && !this.Scene.CollideCheck<Solid>(this.Position, player.Center))
        player.ExplodeLaunch(this.Position, true);
      TheoCrystal theo = this.CollideFirst<TheoCrystal>();
      if (theo != null && !this.Scene.CollideCheck<Solid>(this.Position, theo.Center))
        theo.ExplodeLaunch(this.Position);
      foreach (TempleCrackedBlock entity in this.Scene.Tracker.GetEntities<TempleCrackedBlock>())
      {
        TempleCrackedBlock wall = entity;
        if (this.CollideCheck((Entity) wall))
          wall.Break(this.Position);
        wall = (TempleCrackedBlock) null;
      }
      foreach (TouchSwitch entity in this.Scene.Tracker.GetEntities<TouchSwitch>())
      {
        TouchSwitch sw = entity;
        if (this.CollideCheck((Entity) sw))
          sw.TurnOn();
        sw = (TouchSwitch) null;
      }
      this.Collider = (Collider) this.physicsHitbox;
      Level level = this.SceneAs<Level>();
      level.Displacement.AddBurst(this.Position, 0.4f, 12f, 36f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
      level.Displacement.AddBurst(this.Position, 0.4f, 24f, 48f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
      level.Displacement.AddBurst(this.Position, 0.4f, 36f, 60f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
      for (float i = 0.0f; (double) i < 6.28318548202515; i += 0.1745329f)
      {
        Vector2 at = this.Center + Calc.AngleToVector(i + Calc.Random.Range(-1f * (float) Math.PI / 90f, (float) Math.PI / 90f), (float) Calc.Random.Range(12, 18));
        level.Particles.Emit(Seeker.P_Regen, at, i);
        at = new Vector2();
      }
      player = (Player) null;
      theo = (TheoCrystal) null;
      level = (Level) null;
      this.shaker.On = false;
      this.State.Locked = false;
      this.State.State = 7;
    }

    private IEnumerator ReturnedCoroutine()
    {
      yield return (object) 0.3f;
      this.State.State = 0;
    }

    private struct PatrolPoint
    {
      public Vector2 Point;
      public float Distance;
    }

    [Pooled]
    private class RecoverBlast : Entity
    {
      private Sprite sprite;

      public override void Added(Scene scene)
      {
        base.Added(scene);
        this.Depth = -199;
        if (this.sprite == null)
        {
          this.Add((Component) (this.sprite = GFX.SpriteBank.Create("seekerShockWave")));
          this.sprite.OnLastFrame = (Action<string>) (a => this.RemoveSelf());
        }
        this.sprite.Play("shockwave", true, false);
      }

      public static void Spawn(Vector2 position)
      {
        Seeker.RecoverBlast recoverBlast = Engine.Pooler.Create<Seeker.RecoverBlast>();
        recoverBlast.Position = position;
        Engine.Scene.Add((Entity) recoverBlast);
      }
    }
  }
}

