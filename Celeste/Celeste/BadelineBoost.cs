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
      this.Add((Component) (this.light = new VertexLight(Color.get_White(), 0.7f, 12, 20)));
      this.Add((Component) (this.bloom = new BloomPoint(0.5f, 12f)));
      this.Add((Component) (this.wiggler = Wiggler.Create(0.4f, 3f, (Action<float>) (f => this.sprite.Scale = Vector2.op_Multiply(Vector2.get_One(), (float) (1.0 + (double) this.wiggler.Value * 0.400000005960464))), false, false)));
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
      BadelineBoost badelineBoost = this;
      badelineBoost.holding = player;
      badelineBoost.travelling = true;
      ++badelineBoost.nodeIndex;
      badelineBoost.sprite.Visible = false;
      badelineBoost.sprite.Position = Vector2.get_Zero();
      badelineBoost.Collidable = false;
      bool finalBoost = badelineBoost.nodeIndex >= badelineBoost.nodes.Length;
      Level level = badelineBoost.Scene as Level;
      if (!finalBoost)
        Audio.Play("event:/char/badeline/booster_begin", badelineBoost.Position);
      else
        Audio.Play("event:/char/badeline/booster_final", badelineBoost.Position);
      player.StateMachine.State = 11;
      player.DummyAutoAnimate = false;
      player.DummyGravity = false;
      player.Dashes = 1;
      player.RefillStamina();
      player.Speed = Vector2.get_Zero();
      int num = Math.Sign(player.X - badelineBoost.X);
      if (num == 0)
        num = -1;
      BadelineDummy badeline = new BadelineDummy(badelineBoost.Position);
      badelineBoost.Scene.Add((Entity) badeline);
      player.Facing = (Facings) -num;
      badeline.Sprite.Scale.X = (__Null) (double) num;
      Vector2 playerFrom = player.Position;
      Vector2 playerTo = Vector2.op_Addition(badelineBoost.Position, new Vector2((float) (num * 4), -3f));
      Vector2 badelineFrom = badeline.Position;
      Vector2 badelineTo = Vector2.op_Addition(badelineBoost.Position, new Vector2((float) (-num * 4), 3f));
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.2f)
      {
        Vector2 vector2 = Vector2.Lerp(playerFrom, playerTo, p);
        if (player.Scene != null)
          player.MoveToX((float) vector2.X, (Collision) null);
        if (player.Scene != null)
          player.MoveToY((float) vector2.Y, (Collision) null);
        badeline.Position = Vector2.Lerp(badelineFrom, badelineTo, p);
        yield return (object) null;
      }
      playerFrom = (Vector2) null;
      playerTo = (Vector2) null;
      badelineFrom = (Vector2) null;
      badelineTo = (Vector2) null;
      if (finalBoost)
      {
        Vector2 screenSpaceFocusPoint;
        ((Vector2) ref screenSpaceFocusPoint).\u002Ector(Calc.Clamp(player.X - level.Camera.X, 120f, 200f), Calc.Clamp(player.Y - level.Camera.Y, 60f, 120f));
        badelineBoost.Add((Component) new Coroutine(level.ZoomTo(screenSpaceFocusPoint, 1.5f, 0.18f), true));
        Engine.TimeRate = 0.5f;
      }
      else
        Audio.Play("event:/char/badeline/booster_throw", badelineBoost.Position);
      badeline.Sprite.Play("boost", false, false);
      yield return (object) 0.1f;
      if (!player.Dead)
        player.MoveV(5f, (Collision) null, (Solid) null);
      yield return (object) 0.1f;
      badelineBoost.Add((Component) Alarm.Create(Alarm.AlarmMode.Oneshot, (Action) (() =>
      {
        if (player.Dashes < player.Inventory.Dashes)
          ++player.Dashes;
        this.Scene.Remove((Entity) badeline);
        (this.Scene as Level).Displacement.AddBurst(badeline.Position, 0.25f, 8f, 32f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
      }), 0.15f, true));
      (badelineBoost.Scene as Level).Shake(0.3f);
      badelineBoost.holding = (Player) null;
      if (!finalBoost)
      {
        player.BadelineBoostLaunch(badelineBoost.CenterX);
        Vector2 from = badelineBoost.Position;
        Vector2 to = badelineBoost.nodes[badelineBoost.nodeIndex];
        float duration = Vector2.Distance(from, to) / 320f;
        badelineBoost.stretch.Visible = true;
        badelineBoost.stretch.Rotation = Vector2.op_Subtraction(to, from).Angle();
        Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, duration, true);
        tween.OnUpdate = (Action<Tween>) (t =>
        {
          this.Position = Vector2.Lerp(from, to, t.Eased);
          this.stretch.Scale.X = (__Null) (1.0 + (double) Calc.YoYo(t.Eased) * 2.0);
          this.stretch.Scale.Y = (__Null) (1.0 - (double) Calc.YoYo(t.Eased) * 0.75);
          if ((double) t.Eased >= 0.899999976158142 || !this.Scene.OnInterval(0.03f))
            return;
          TrailManager.Add((Entity) this, Player.TwoDashesHairColor, 0.5f);
          level.ParticlesFG.Emit(BadelineBoost.P_Move, 1, this.Center, Vector2.op_Multiply(Vector2.get_One(), 4f));
        });
        tween.OnComplete = (Action<Tween>) (t =>
        {
          this.travelling = false;
          this.stretch.Visible = false;
          this.sprite.Visible = true;
          this.Collidable = true;
          Audio.Play("event:/char/badeline/booster_reappear", this.Position);
        });
        badelineBoost.Add((Component) tween);
        badelineBoost.relocateSfx.Play("event:/char/badeline/booster_relocate", (string) null, 0.0f);
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
        level.DirectionalShake(Vector2.op_UnaryNegation(Vector2.get_UnitY()), 0.3f);
        level.Displacement.AddBurst(badelineBoost.Center, 0.4f, 8f, 32f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
      }
      else
      {
        Engine.FreezeTimer = 0.1f;
        yield return (object) null;
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
        level.Flash(Color.op_Multiply(Color.get_White(), 0.5f), true);
        level.DirectionalShake(Vector2.op_UnaryNegation(Vector2.get_UnitY()), 0.6f);
        level.Displacement.AddBurst(badelineBoost.Center, 0.6f, 8f, 64f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
        level.ResetZoom();
        level.Entities.FindFirst<SpeedrunTimerDisplay>().StayOnscreenFor = 1f;
        player.SummitLaunch(badelineBoost.X);
        Engine.TimeRate = 1f;
        badelineBoost.Finish();
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
        this.SceneAs<Level>().ParticlesBG.Emit(BadelineBoost.P_Ambience, 1, this.Center, Vector2.op_Multiply(Vector2.get_One(), 3f));
      if (this.holding != null)
        this.holding.Speed = Vector2.get_Zero();
      if (!this.travelling)
      {
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity != null)
        {
          Vector2 vector2 = Vector2.op_Subtraction(entity.Center, this.Position);
          float num = Calc.ClampedMap(((Vector2) ref vector2).Length(), 16f, 64f, 12f, 0.0f);
          this.sprite.Position = Calc.Approach(this.sprite.Position, Vector2.op_Multiply(Vector2.op_Subtraction(entity.Center, this.Position).SafeNormalize(), num), 32f * Engine.DeltaTime);
        }
      }
      this.light.Visible = this.bloom.Visible = this.sprite.Visible || this.stretch.Visible;
      base.Update();
    }

    private void Finish()
    {
      this.SceneAs<Level>().Displacement.AddBurst(this.Center, 0.5f, 24f, 96f, 0.4f, (Ease.Easer) null, (Ease.Easer) null);
      this.SceneAs<Level>().Particles.Emit(BadelineOldsite.P_Vanish, 12, this.Center, Vector2.op_Multiply(Vector2.get_One(), 6f));
      this.SceneAs<Level>().CameraLockMode = Level.CameraLockModes.None;
      this.SceneAs<Level>().CameraOffset = new Vector2(0.0f, -16f);
      this.RemoveSelf();
    }
  }
}
