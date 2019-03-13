// Decompiled with JetBrains decompiler
// Type: Celeste.AngryOshiro
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
  public class AngryOshiro : Entity
  {
    private static readonly float[] ChaseWaitTimes = new float[5]
    {
      1f,
      2f,
      3f,
      2f,
      3f
    };
    private bool canControlTimeRate = true;
    private float yApproachSpeed = 100f;
    private const int StChase = 0;
    private const int StChargeUp = 1;
    private const int StAttack = 2;
    private const int StDummy = 3;
    private const int StWaiting = 4;
    private const int StHurt = 5;
    private const float HitboxBackRange = 4f;
    public Sprite Sprite;
    private Sprite lightning;
    private bool lightningVisible;
    private VertexLight light;
    private Level level;
    private SineWave sine;
    private float cameraXOffset;
    private StateMachine state;
    private int attackIndex;
    private float targetAnxiety;
    private float anxietySpeed;
    private bool easeBackFromRightEdge;
    private bool fromCutscene;
    private bool doRespawnAnim;
    private bool leaving;
    private Shaker shaker;
    private PlayerCollider bounceCollider;
    private Vector2 colliderTargetPosition;
    private SoundSource prechargeSfx;
    private SoundSource chargeSfx;
    private bool hasEnteredSfx;
    private const float minCameraOffsetX = -48f;
    private const float yApproachTargetSpeed = 100f;
    private float attackSpeed;
    private const float HurtXSpeed = 100f;
    private const float HurtYSpeed = 200f;

    public AngryOshiro(Vector2 position, bool fromCutscene)
      : base(position)
    {
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("oshiro_boss")));
      this.Sprite.Play("idle", false, false);
      this.Add((Component) (this.lightning = GFX.SpriteBank.Create("oshiro_boss_lightning")));
      this.lightning.Visible = false;
      this.lightning.OnFinish = (Action<string>) (s => this.lightningVisible = false);
      this.Collider = (Collider) new Monocle.Circle(14f, 0.0f, 0.0f);
      this.Collider.Position = this.colliderTargetPosition = new Vector2(3f, 4f);
      this.Add((Component) (this.sine = new SineWave(0.5f)));
      this.Add((Component) (this.bounceCollider = new PlayerCollider(new Action<Player>(this.OnPlayerBounce), (Collider) new Hitbox(28f, 6f, -11f, -11f), (Collider) null)));
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Depth = -12500;
      this.Visible = false;
      this.Add((Component) (this.light = new VertexLight(Color.White, 1f, 32, 64)));
      this.Add((Component) (this.shaker = new Shaker(false, (Action<Vector2>) null)));
      this.state = new StateMachine(10);
      this.state.SetCallbacks(0, new Func<int>(this.ChaseUpdate), new Func<IEnumerator>(this.ChaseCoroutine), new Action(this.ChaseBegin), (Action) null);
      this.state.SetCallbacks(1, new Func<int>(this.ChargeUpUpdate), new Func<IEnumerator>(this.ChargeUpCoroutine), (Action) null, new Action(this.ChargeUpEnd));
      this.state.SetCallbacks(2, new Func<int>(this.AttackUpdate), new Func<IEnumerator>(this.AttackCoroutine), new Action(this.AttackBegin), new Action(this.AttackEnd));
      this.state.SetCallbacks(3, (Func<int>) null, (Func<IEnumerator>) null, (Action) null, (Action) null);
      this.state.SetCallbacks(4, new Func<int>(this.WaitingUpdate), (Func<IEnumerator>) null, (Action) null, (Action) null);
      this.state.SetCallbacks(5, new Func<int>(this.HurtUpdate), (Func<IEnumerator>) null, new Action(this.HurtBegin), (Action) null);
      this.Add((Component) this.state);
      if (fromCutscene)
        this.yApproachSpeed = 0.0f;
      this.fromCutscene = fromCutscene;
      this.Add((Component) new TransitionListener()
      {
        OnOutBegin = (Action) (() =>
        {
          if ((double) this.X > (double) this.level.Bounds.Left + (double) this.Sprite.Width / 2.0)
            this.Visible = false;
          else
            this.easeBackFromRightEdge = true;
        }),
        OnOut = (Action<float>) (f =>
        {
          this.lightning.Update();
          if (!this.easeBackFromRightEdge)
            return;
          this.X -= 128f * Engine.RawDeltaTime;
        })
      });
      this.Add((Component) (this.prechargeSfx = new SoundSource()));
      this.Add((Component) (this.chargeSfx = new SoundSource()));
      Distort.AnxietyOrigin = new Vector2(1f, 0.5f);
    }

    public AngryOshiro(EntityData data, Vector2 offset)
      : this(data.Position + offset, false)
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.level = this.SceneAs<Level>();
      if (this.level.Session.GetFlag("oshiroEnding") || !this.level.Session.GetFlag("oshiro_resort_roof") && this.level.Session.Level.Equals("roof00"))
        this.RemoveSelf();
      if (this.state.State != 3 && !this.fromCutscene)
        this.state.State = 4;
      if (!this.fromCutscene)
      {
        this.Y = this.TargetY;
        this.cameraXOffset = -48f;
      }
      else
        this.cameraXOffset = this.X - this.level.Camera.Left;
    }

    private float TargetY
    {
      get
      {
        Player entity = this.level.Tracker.GetEntity<Player>();
        if (entity != null)
          return MathHelper.Clamp(entity.CenterY, (float) (this.level.Bounds.Top + 8), (float) (this.level.Bounds.Bottom - 8));
        return this.Y;
      }
    }

    private void OnPlayer(Player player)
    {
      if (this.state.State == 5 || (double) this.CenterX >= (double) player.CenterX + 4.0 && !(this.Sprite.CurrentAnimationID != "respawn"))
        return;
      player.Die((player.Center - this.Center).SafeNormalize(Vector2.UnitX), false, true);
    }

    private void OnPlayerBounce(Player player)
    {
      if (this.state.State != 2 || (double) player.Bottom > (double) this.Top + 6.0)
        return;
      Audio.Play("event:/game/general/thing_booped", this.Position);
      Celeste.Freeze(0.2f);
      player.Bounce(this.Top + 2f);
      this.state.State = 5;
      this.prechargeSfx.Stop(true);
      this.chargeSfx.Stop(true);
    }

    public override void Update()
    {
      base.Update();
      this.Sprite.Scale.X = Calc.Approach(this.Sprite.Scale.X, 1f, 0.6f * Engine.DeltaTime);
      this.Sprite.Scale.Y = Calc.Approach(this.Sprite.Scale.Y, 1f, 0.6f * Engine.DeltaTime);
      if (!this.doRespawnAnim)
        this.Visible = (double) this.X > (double) this.level.Bounds.Left - (double) this.Width / 2.0;
      this.yApproachSpeed = Calc.Approach(this.yApproachSpeed, 100f, 300f * Engine.DeltaTime);
      if (this.state.State != 3 && this.canControlTimeRate)
      {
        if (this.state.State == 2 && (double) this.attackSpeed > 200.0)
        {
          Player entity = this.Scene.Tracker.GetEntity<Player>();
          Engine.TimeRate = entity == null || entity.Dead || (double) this.CenterX >= (double) entity.CenterX + 4.0 ? 1f : MathHelper.Lerp(Calc.ClampedMap(entity.CenterX - this.CenterX, 30f, 80f, 0.5f, 1f), 1f, Calc.ClampedMap(Math.Abs(entity.CenterY - this.CenterY), 32f, 48f, 0.0f, 1f));
        }
        else
          Engine.TimeRate = 1f;
        Distort.GameRate = Calc.Approach(Distort.GameRate, Calc.Map(Engine.TimeRate, 0.5f, 1f, 0.0f, 1f), Engine.DeltaTime * 8f);
        Distort.Anxiety = Calc.Approach(Distort.Anxiety, this.targetAnxiety, this.anxietySpeed * Engine.DeltaTime);
      }
      else
      {
        Distort.GameRate = 1f;
        Distort.Anxiety = 0.0f;
      }
    }

    public void StopControllingTime()
    {
      this.canControlTimeRate = false;
    }

    public override void Render()
    {
      if (this.lightningVisible)
      {
        this.lightning.RenderPosition = new Vector2(this.level.Camera.Left - 2f, this.Top + 16f);
        this.lightning.Render();
      }
      this.Sprite.Position = this.shaker.Value * 2f;
      base.Render();
    }

    public void Leave()
    {
      this.leaving = true;
    }

    public void Squish()
    {
      this.Sprite.Scale = new Vector2(1.3f, 0.5f);
      this.shaker.ShakeFor(0.5f, false);
    }

    private void ChaseBegin()
    {
      this.Sprite.Play("idle", false, false);
    }

    private int ChaseUpdate()
    {
      if (!this.hasEnteredSfx && (double) this.cameraXOffset >= -16.0 && !this.doRespawnAnim)
      {
        Audio.Play("event:/char/oshiro/boss_enter_screen", this.Position);
        this.hasEnteredSfx = true;
      }
      if (this.doRespawnAnim && (double) this.cameraXOffset >= 0.0)
      {
        this.Collider.Position.X = -48f;
        this.Visible = true;
        this.Sprite.Play("respawn", false, false);
        this.doRespawnAnim = false;
        if (this.Scene.Tracker.GetEntity<Player>() != null)
          Audio.Play("event:/char/oshiro/boss_reform", this.Position);
      }
      this.cameraXOffset = Calc.Approach(this.cameraXOffset, 20f, 80f * Engine.DeltaTime);
      this.X = this.level.Camera.Left + this.cameraXOffset;
      this.Collider.Position.X = Calc.Approach(this.Collider.Position.X, this.colliderTargetPosition.X, Engine.DeltaTime * 128f);
      this.Collidable = this.Visible;
      if (this.level.Tracker.GetEntity<Player>() != null && this.Sprite.CurrentAnimationID != "respawn")
        this.CenterY = Calc.Approach(this.CenterY, this.TargetY, this.yApproachSpeed * Engine.DeltaTime);
      return 0;
    }

    private IEnumerator ChaseCoroutine()
    {
      if ((uint) this.level.Session.Area.Mode > 0U)
      {
        yield return (object) 1f;
      }
      else
      {
        yield return (object) AngryOshiro.ChaseWaitTimes[this.attackIndex];
        ++this.attackIndex;
        this.attackIndex %= AngryOshiro.ChaseWaitTimes.Length;
      }
      this.prechargeSfx.Play("event:/char/oshiro/boss_precharge", (string) null, 0.0f);
      this.Sprite.Play("charge", false, false);
      yield return (object) 0.7f;
      if (this.Scene.Tracker.GetEntity<Player>() != null)
      {
        Alarm.Set((Entity) this, 0.216f, (Action) (() => this.chargeSfx.Play("event:/char/oshiro/boss_charge", (string) null, 0.0f)), Alarm.AlarmMode.Oneshot);
        this.state.State = 1;
      }
      else
        this.Sprite.Play("idle", false, false);
    }

    private int ChargeUpUpdate()
    {
      if (this.level.OnInterval(0.05f))
        this.Sprite.Position = Calc.Random.ShakeVector();
      this.cameraXOffset = Calc.Approach(this.cameraXOffset, 0.0f, 40f * Engine.DeltaTime);
      this.X = this.level.Camera.Left + this.cameraXOffset;
      Player entity = this.level.Tracker.GetEntity<Player>();
      if (entity != null)
        this.CenterY = Calc.Approach(this.CenterY, MathHelper.Clamp(entity.CenterY, (float) (this.level.Bounds.Top + 8), (float) (this.level.Bounds.Bottom - 8)), 30f * Engine.DeltaTime);
      return 1;
    }

    private void ChargeUpEnd()
    {
      this.Sprite.Position = Vector2.Zero;
    }

    private IEnumerator ChargeUpCoroutine()
    {
      Celeste.Freeze(0.05f);
      Distort.Anxiety = 0.3f;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      this.lightningVisible = true;
      this.lightning.Play("once", true, false);
      yield return (object) 0.3f;
      Player player = this.Scene.Tracker.GetEntity<Player>();
      this.state.State = player == null ? 0 : 2;
    }

    private void AttackBegin()
    {
      this.attackSpeed = 0.0f;
      this.targetAnxiety = 0.3f;
      this.anxietySpeed = 4f;
      this.level.DirectionalShake(Vector2.UnitX, 0.3f);
    }

    private void AttackEnd()
    {
      this.targetAnxiety = 0.0f;
      this.anxietySpeed = 0.5f;
    }

    private int AttackUpdate()
    {
      this.X += this.attackSpeed * Engine.DeltaTime;
      this.attackSpeed = Calc.Approach(this.attackSpeed, 500f, 2000f * Engine.DeltaTime);
      if ((double) this.X >= (double) this.level.Camera.Right + 48.0)
      {
        if (this.leaving)
        {
          this.RemoveSelf();
          return 2;
        }
        this.X = this.level.Camera.Left - 48f;
        this.cameraXOffset = -48f;
        this.doRespawnAnim = true;
        this.Visible = false;
        return 0;
      }
      Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
      if (this.Scene.OnInterval(0.05f))
        TrailManager.Add((Entity) this, Color.Red * 0.6f, 0.5f);
      return 2;
    }

    private IEnumerator AttackCoroutine()
    {
      yield return (object) 0.1f;
      this.targetAnxiety = 0.0f;
      this.anxietySpeed = 0.5f;
    }

    public bool DummyMode
    {
      get
      {
        return this.state.State == 3;
      }
    }

    public void EnterDummyMode()
    {
      this.state.State = 3;
    }

    public void LeaveDummyMode()
    {
      this.state.State = 0;
    }

    private int WaitingUpdate()
    {
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      return entity != null && entity.Speed != Vector2.Zero && (double) entity.X > (double) (this.level.Bounds.Left + 48) ? 0 : 4;
    }

    private void HurtBegin()
    {
      this.Sprite.Play("hurt", true, false);
    }

    private int HurtUpdate()
    {
      this.X += 100f * Engine.DeltaTime;
      this.Y += 200f * Engine.DeltaTime;
      if ((double) this.Top <= (double) (this.level.Bounds.Bottom + 20))
        return 5;
      if (this.leaving)
      {
        this.RemoveSelf();
        return 5;
      }
      this.X = this.level.Camera.Left - 48f;
      this.cameraXOffset = -48f;
      this.doRespawnAnim = true;
      this.Visible = false;
      return 0;
    }
  }
}

