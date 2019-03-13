// Decompiled with JetBrains decompiler
// Type: Celeste.FakeHeart
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class FakeHeart : Entity
  {
    private const float RespawnTime = 3f;
    private Sprite sprite;
    private ParticleType shineParticle;
    public Wiggler ScaleWiggler;
    private Wiggler moveWiggler;
    private Vector2 moveWiggleDir;
    private BloomPoint bloom;
    private VertexLight light;
    private HoldableCollider crystalCollider;
    private float timer;
    private float bounceSfxDelay;
    private float respawnTimer;

    public FakeHeart(Vector2 position)
      : base(position)
    {
      this.Add((Component) (this.crystalCollider = new HoldableCollider(new Action<Holdable>(this.OnHoldable), (Collider) null)));
      this.Add((Component) new MirrorReflection());
    }

    public FakeHeart(EntityData data, Vector2 offset)
      : this(data.Position + offset)
    {
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      AreaMode areaMode = Calc.Random.Choose<AreaMode>(AreaMode.Normal, AreaMode.BSide, AreaMode.CSide);
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("heartgem" + (object) (int) areaMode)));
      this.sprite.Play("spin", false, false);
      this.sprite.OnLoop = (Action<string>) (anim =>
      {
        if (!this.Visible || !(anim == "spin"))
          return;
        Audio.Play("event:/game/general/crystalheart_pulse", this.Position);
        this.ScaleWiggler.Start();
        (this.Scene as Level).Displacement.AddBurst(this.Position, 0.35f, 8f, 48f, 0.25f, (Ease.Easer) null, (Ease.Easer) null);
      });
      this.Collider = (Collider) new Hitbox(16f, 16f, -8f, -8f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Add((Component) (this.ScaleWiggler = Wiggler.Create(0.5f, 4f, (Action<float>) (f => this.sprite.Scale = Vector2.One * (float) (1.0 + (double) f * 0.25)), false, false)));
      this.Add((Component) (this.bloom = new BloomPoint(0.75f, 16f)));
      Color color;
      switch (areaMode)
      {
        case AreaMode.Normal:
          color = Color.Aqua;
          this.shineParticle = HeartGem.P_BlueShine;
          break;
        case AreaMode.BSide:
          color = Color.Red;
          this.shineParticle = HeartGem.P_RedShine;
          break;
        default:
          color = Color.Gold;
          this.shineParticle = HeartGem.P_GoldShine;
          break;
      }
      this.Add((Component) (this.light = new VertexLight(Color.Lerp(color, Color.White, 0.5f), 1f, 32, 64)));
      this.moveWiggler = Wiggler.Create(0.8f, 2f, (Action<float>) null, false, false);
      this.moveWiggler.StartZero = true;
      this.Add((Component) this.moveWiggler);
    }

    public override void Update()
    {
      this.bounceSfxDelay -= Engine.DeltaTime;
      this.timer += Engine.DeltaTime;
      this.sprite.Position = Vector2.UnitY * (float) Math.Sin((double) this.timer * 2.0) * 2f + this.moveWiggleDir * this.moveWiggler.Value * -8f;
      if ((double) this.respawnTimer > 0.0)
      {
        this.respawnTimer -= Engine.DeltaTime;
        if ((double) this.respawnTimer <= 0.0)
        {
          this.Collidable = this.Visible = true;
          this.ScaleWiggler.Start();
        }
      }
      base.Update();
      if (!this.Visible || !this.Scene.OnInterval(0.1f))
        return;
      this.SceneAs<Level>().Particles.Emit(this.shineParticle, 1, this.Center, Vector2.One * 8f);
    }

    public void OnHoldable(Holdable h)
    {
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (!this.Visible || !h.Dangerous(this.crystalCollider))
        return;
      this.Collect(entity, h.GetSpeed().Angle());
    }

    public void OnPlayer(Player player)
    {
      if (!this.Visible || (this.Scene as Level).Frozen)
        return;
      if (player.DashAttacking)
      {
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        this.Collect(player, player.Speed.Angle());
      }
      else
      {
        if ((double) this.bounceSfxDelay <= 0.0)
        {
          Audio.Play("event:/game/general/crystalheart_bounce", this.Position);
          this.bounceSfxDelay = 0.1f;
        }
        player.PointBounce(this.Center);
        this.moveWiggler.Start();
        this.ScaleWiggler.Start();
        this.moveWiggleDir = (this.Center - player.Center).SafeNormalize(Vector2.UnitY);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      }
    }

    private void Collect(Player player, float angle)
    {
      if (!this.Collidable)
        return;
      this.Collidable = this.Visible = false;
      this.respawnTimer = 3f;
      Celeste.Freeze(0.05f);
      this.SceneAs<Level>().Shake(0.3f);
      SlashFx.Burst(this.Position, angle);
      if (player != null)
        player.RefillDash();
    }
  }
}

