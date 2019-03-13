// Decompiled with JetBrains decompiler
// Type: Celeste.BadelineBoost
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class BadelineBoost : Entity
  {
    public static ParticleType P_Ambience;
    public static ParticleType P_Move;
    private const float MoveSpeed = 320f;
    private Sprite sprite;
    private Monocle.Image stretch;
    private Wiggler wiggler;
    private VertexLight light;
    private BloomPoint bloom;
    private Vector2[] nodes;
    private int nodeIndex;
    private bool travelling;
    private Player holding;
    private SoundSource relocateSfx;

    public BadelineBoost(Vector2[] nodes, bool lockCamera)
      : base(nodes[0])
    {
      this.Depth = -1000000;
      this.nodes = nodes;
      this.Collider = (Collider) new Monocle.Circle(16f, 0.0f, 0.0f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("badelineBoost")));
      this.Add((Component) (this.stretch = new Monocle.Image(GFX.Game["objects/badelineboost/stretch"])));
      this.stretch.Visible = false;
      this.stretch.CenterOrigin();
      this.Add((Component) (this.light = new VertexLight(Color.White, 0.7f, 12, 20)));
      this.Add((Component) (this.bloom = new BloomPoint(0.5f, 12f)));
      this.Add((Component) (this.wiggler = Wiggler.Create(0.4f, 3f, (Action<float>) (f => this.sprite.Scale = Vector2.One * (float) (1.0 + (double) this.wiggler.Value * 0.400000005960464)), false, false)));
      if (lockCamera)
        this.Add((Component) new CameraLocker(Level.CameraLockModes.BoostSequence, 0.0f, 160f));
      this.Add((Component) (this.relocateSfx = new SoundSource()));
    }

    public BadelineBoost(EntityData data, Vector2 offset)
      : this(data.NodesWithPosition(offset), data.Bool("lockCamera", true))
    {
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (!this.CollideCheck<FakeWall>())
        return;
      this.Depth = -12500;
    }

    private void OnPlayer(Player player)
    {
      this.Add((Component) new Coroutine(this.BoostRoutine(player), true));
    }

    private IEnumerator BoostRoutine(Player player)
    {
      this.holding = player;
      this.travelling = true;
      ++this.nodeIndex;
      this.sprite.Visible = false;
      this.sprite.Position = Vector2.Zero;
      this.Collidable = false;
      bool finalBoost = this.nodeIndex >= this.nodes.Length;
      Level level = this.Scene as Level;
      if (!finalBoost)
        Audio.Play("event:/char/badeline/booster_begin", this.Position);
      else
        Audio.Play("event:/char/badeline/booster_final", this.Position);
      player.StateMachine.State = Player.StDummy;
      player.DummyAutoAnimate = false;
      player.DummyGravity = false;
      player.Dashes = 1;
      player.RefillStamina();
      player.Speed = Vector2.Zero;
      int side = Math.Sign(player.X - this.X);
      if (side == 0)
        side = -1;
      BadelineDummy badeline = new BadelineDummy(this.Position);
      this.Scene.Add((Entity) badeline);
      player.Facing = ToFacing.Convert(-side);
      badeline.Sprite.Scale.X = (float) side;
      Vector2 playerFrom = player.Position;
      Vector2 playerTo = this.Position + new Vector2((float) (side * 4), -3f);
      Vector2 badelineFrom = badeline.Position;
      Vector2 badelineTo = this.Position + new Vector2((float) (-side * 4), 3f);
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.2f)
      {
        Vector2 target = Vector2.Lerp(playerFrom, playerTo, p);
        if (player.Scene != null)
          player.MoveToX(target.X, (Collision) null);
        if (player.Scene != null)
          player.MoveToY(target.Y, (Collision) null);
        badeline.Position = Vector2.Lerp(badelineFrom, badelineTo, p);
        yield return (object) null;
        target = new Vector2();
      }
      playerFrom = new Vector2();
      playerTo = new Vector2();
      badelineFrom = new Vector2();
      badelineTo = new Vector2();
      if (finalBoost)
      {
        Vector2 center = new Vector2(Calc.Clamp(player.X - level.Camera.X, 120f, 200f), Calc.Clamp(player.Y - level.Camera.Y, 60f, 120f));
        this.Add((Component) new Coroutine(level.ZoomTo(center, 1.5f, 0.18f), true));
        Engine.TimeRate = 0.5f;
        center = new Vector2();
      }
      else
        Audio.Play("event:/char/badeline/booster_throw", this.Position);
      badeline.Sprite.Play("boost", false, false);
      yield return (object) 0.1f;
      if (!player.Dead)
        player.MoveV(5f, (Collision) null, (Solid) null);
      yield return (object) 0.1f;
      this.Add((Component) Alarm.Create(Alarm.AlarmMode.Oneshot, (Action) (() =>
      {
        if (player.Dashes < player.Inventory.Dashes)
          ++player.Dashes;
        this.Scene.Remove((Entity) badeline);
        (this.Scene as Level).Displacement.AddBurst(badeline.Position, 0.25f, 8f, 32f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
      }), 0.15f, true));
      (this.Scene as Level).Shake(0.3f);
      this.holding = (Player) null;
      if (!finalBoost)
      {
        player.BadelineBoostLaunch(this.CenterX);
        Vector2 from = this.Position;
        Vector2 to = this.nodes[this.nodeIndex];
        float time = Vector2.Distance(from, to) / 320f;
        this.stretch.Visible = true;
        this.stretch.Rotation = (to - from).Angle();
        Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, time, true);
        tween.OnUpdate = (Action<Tween>) (t =>
        {
          this.Position = Vector2.Lerp(from, to, t.Eased);
          this.stretch.Scale.X = (float) (1.0 + (double) Calc.YoYo(t.Eased) * 2.0);
          this.stretch.Scale.Y = (float) (1.0 - (double) Calc.YoYo(t.Eased) * 0.75);
          if ((double) t.Eased >= 0.899999976158142 || !this.Scene.OnInterval(0.03f))
            return;
          TrailManager.Add((Entity) this, Player.TwoDashesHairColor, 0.5f);
          level.ParticlesFG.Emit(BadelineBoost.P_Move, 1, this.Center, Vector2.One * 4f);
        });
        tween.OnComplete = (Action<Tween>) (t =>
        {
          this.travelling = false;
          this.stretch.Visible = false;
          this.sprite.Visible = true;
          this.Collidable = true;
          Audio.Play("event:/char/badeline/booster_reappear", this.Position);
        });
        this.Add((Component) tween);
        this.relocateSfx.Play("event:/char/badeline/booster_relocate", (string) null, 0.0f);
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
        level.DirectionalShake(-Vector2.UnitY, 0.3f);
        level.Displacement.AddBurst(this.Center, 0.4f, 8f, 32f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
        tween = (Tween) null;
      }
      else
      {
        Engine.FreezeTimer = 0.1f;
        yield return (object) null;
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
        level.Flash(Color.White * 0.5f, true);
        level.DirectionalShake(-Vector2.UnitY, 0.6f);
        level.Displacement.AddBurst(this.Center, 0.6f, 8f, 64f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
        level.ResetZoom();
        level.Entities.FindFirst<SpeedrunTimerDisplay>().StayOnscreenFor = 1f;
        player.SummitLaunch(this.X);
        Engine.TimeRate = 1f;
        this.Finish();
      }
    }

    public void Wiggle()
    {
      this.wiggler.Start();
      (this.Scene as Level).Displacement.AddBurst(this.Position, 0.3f, 4f, 16f, 0.25f, (Ease.Easer) null, (Ease.Easer) null);
      Audio.Play("event:/game/general/crystalheart_pulse", this.Position);
    }

    public override void Update()
    {
      if (this.sprite.Visible && this.Scene.OnInterval(0.05f))
        this.SceneAs<Level>().ParticlesBG.Emit(BadelineBoost.P_Ambience, 1, this.Center, Vector2.One * 3f);
      if (this.holding != null)
        this.holding.Speed = Vector2.Zero;
      if (!this.travelling)
      {
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity != null)
        {
          float num = Calc.ClampedMap((entity.Center - this.Position).Length(), 16f, 64f, 12f, 0.0f);
          this.sprite.Position = Calc.Approach(this.sprite.Position, (entity.Center - this.Position).SafeNormalize() * num, 32f * Engine.DeltaTime);
        }
      }
      this.light.Visible = this.bloom.Visible = this.sprite.Visible || this.stretch.Visible;
      base.Update();
    }

    private void Finish()
    {
      this.SceneAs<Level>().Displacement.AddBurst(this.Center, 0.5f, 24f, 96f, 0.4f, (Ease.Easer) null, (Ease.Easer) null);
      this.SceneAs<Level>().Particles.Emit(BadelineOldsite.P_Vanish, 12, this.Center, Vector2.One * 6f);
      this.SceneAs<Level>().CameraLockMode = Level.CameraLockModes.None;
      this.SceneAs<Level>().CameraOffset = new Vector2(0.0f, -16f);
      this.RemoveSelf();
    }
  }
}

