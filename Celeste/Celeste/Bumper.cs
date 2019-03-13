// Decompiled with JetBrains decompiler
// Type: Celeste.Bumper
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class Bumper : Entity
  {
    public static ParticleType P_Ambience;
    public static ParticleType P_Launch;
    public static ParticleType P_FireAmbience;
    public static ParticleType P_FireHit;
    private const float RespawnTime = 0.6f;
    private const float MoveCycleTime = 1.818182f;
    private const float SineCycleFreq = 0.44f;
    private Sprite sprite;
    private Sprite spriteEvil;
    private VertexLight light;
    private BloomPoint bloom;
    private Vector2? node;
    private bool goBack;
    private Vector2 anchor;
    private SineWave sine;
    private float respawnTimer;
    private bool fireMode;
    private Wiggler hitWiggler;
    private Vector2 hitDir;

    public Bumper(Vector2 position, Vector2? node)
      : base(position)
    {
      this.Collider = (Collider) new Monocle.Circle(12f, 0.0f, 0.0f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Add((Component) (this.sine = new SineWave(0.44f).Randomize()));
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("bumper")));
      this.Add((Component) (this.spriteEvil = GFX.SpriteBank.Create("bumper_evil")));
      this.spriteEvil.Visible = false;
      this.Add((Component) (this.light = new VertexLight(Color.Teal, 1f, 16, 32)));
      this.Add((Component) (this.bloom = new BloomPoint(0.5f, 16f)));
      this.node = node;
      this.anchor = this.Position;
      if (node.HasValue)
      {
        Vector2 start = this.Position;
        Vector2 end = node.Value;
        Tween tween = Tween.Create(Tween.TweenMode.Looping, Ease.CubeInOut, 1.818182f, true);
        tween.OnUpdate = (Action<Tween>) (t =>
        {
          if (this.goBack)
            this.anchor = Vector2.Lerp(end, start, t.Eased);
          else
            this.anchor = Vector2.Lerp(start, end, t.Eased);
        });
        tween.OnComplete = (Action<Tween>) (t => this.goBack = !this.goBack);
        this.Add((Component) tween);
      }
      this.UpdatePosition();
      this.Add((Component) (this.hitWiggler = Wiggler.Create(1.2f, 2f, (Action<float>) (v => this.spriteEvil.Position = this.hitDir * this.hitWiggler.Value * 8f), false, false)));
      this.Add((Component) new CoreModeListener(new Action<Session.CoreModes>(this.OnChangeMode)));
    }

    public Bumper(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.FirstNodeNullable(new Vector2?(offset)))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.fireMode = this.SceneAs<Level>().CoreMode == Session.CoreModes.Hot;
      this.spriteEvil.Visible = this.fireMode;
      this.sprite.Visible = !this.fireMode;
    }

    private void OnChangeMode(Session.CoreModes coreMode)
    {
      this.fireMode = coreMode == Session.CoreModes.Hot;
      this.spriteEvil.Visible = this.fireMode;
      this.sprite.Visible = !this.fireMode;
    }

    private void UpdatePosition()
    {
      this.Position = this.anchor + new Vector2(this.sine.Value * 3f, this.sine.ValueOverTwo * 2f);
    }

    public override void Update()
    {
      base.Update();
      if ((double) this.respawnTimer > 0.0)
      {
        this.respawnTimer -= Engine.DeltaTime;
        if ((double) this.respawnTimer <= 0.0)
        {
          this.light.Visible = true;
          this.bloom.Visible = true;
          this.sprite.Play("on", false, false);
          this.spriteEvil.Play("on", false, false);
          if (!this.fireMode)
            Audio.Play("event:/game/06_reflection/pinballbumper_reset", this.Position);
        }
      }
      else if (this.Scene.OnInterval(0.05f))
      {
        float angleRadians = Calc.Random.NextAngle();
        ParticleType type = this.fireMode ? Bumper.P_FireAmbience : Bumper.P_Ambience;
        float direction = this.fireMode ? -1.570796f : angleRadians;
        float length = this.fireMode ? 12f : 8f;
        this.SceneAs<Level>().Particles.Emit(type, 1, this.Center + Calc.AngleToVector(angleRadians, length), Vector2.One * 2f, direction);
      }
      this.UpdatePosition();
    }

    private void OnPlayer(Player player)
    {
      if (this.fireMode)
      {
        if (SaveData.Instance.Assists.Invincible)
          return;
        Vector2 vector2 = (player.Center - this.Center).SafeNormalize();
        this.hitDir = -vector2;
        this.hitWiggler.Start();
        Audio.Play("event:/game/09_core/hotpinball_activate", this.Position);
        this.respawnTimer = 0.6f;
        player.Die(vector2, false, true);
        this.SceneAs<Level>().Particles.Emit(Bumper.P_FireHit, 12, this.Center + vector2 * 12f, Vector2.One * 3f, vector2.Angle());
      }
      else
      {
        if ((double) this.respawnTimer > 0.0)
          return;
        if ((this.Scene as Level).Session.Area.ID == 9)
          Audio.Play("event:/game/09_core/pinballbumper_hit", this.Position);
        else
          Audio.Play("event:/game/06_reflection/pinballbumper_hit", this.Position);
        this.respawnTimer = 0.6f;
        Vector2 vector2 = player.ExplodeLaunch(this.Position, false);
        this.sprite.Play("hit", true, false);
        this.spriteEvil.Play("hit", true, false);
        this.light.Visible = false;
        this.bloom.Visible = false;
        this.SceneAs<Level>().DirectionalShake(vector2, 0.15f);
        this.SceneAs<Level>().Displacement.AddBurst(this.Center, 0.3f, 8f, 32f, 0.8f, (Ease.Easer) null, (Ease.Easer) null);
        this.SceneAs<Level>().Particles.Emit(Bumper.P_Launch, 12, this.Center + vector2 * 12f, Vector2.One * 3f, vector2.Angle());
      }
    }
  }
}

