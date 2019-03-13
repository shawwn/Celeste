// Decompiled with JetBrains decompiler
// Type: Celeste.FlyFeather
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
  public class FlyFeather : Entity
  {
    public static ParticleType P_Collect;
    public static ParticleType P_Boost;
    public static ParticleType P_Flying;
    public static ParticleType P_Respawn;
    private const float RespawnTime = 3f;
    private Sprite sprite;
    private Monocle.Image outline;
    private Wiggler wiggler;
    private BloomPoint bloom;
    private VertexLight light;
    private Level level;
    private SineWave sine;
    private bool shielded;
    private bool singleUse;
    private Wiggler shieldRadiusWiggle;
    private Wiggler moveWiggle;
    private Vector2 moveWiggleDir;
    private float respawnTimer;

    public FlyFeather(Vector2 position, bool shielded, bool singleUse)
      : base(position)
    {
      this.shielded = shielded;
      this.singleUse = singleUse;
      this.Collider = (Collider) new Hitbox(20f, 20f, -10f, -10f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("flyFeather")));
      this.Add((Component) (this.wiggler = Wiggler.Create(1f, 4f, (Action<float>) (v => this.sprite.Scale = Vector2.One * (float) (1.0 + (double) v * 0.200000002980232)), false, false)));
      this.Add((Component) (this.bloom = new BloomPoint(0.5f, 20f)));
      this.Add((Component) (this.light = new VertexLight(Color.White, 1f, 16, 48)));
      this.Add((Component) (this.sine = new SineWave(0.6f).Randomize()));
      this.Add((Component) (this.outline = new Monocle.Image(GFX.Game["objects/flyFeather/outline"])));
      this.outline.CenterOrigin();
      this.outline.Visible = false;
      this.shieldRadiusWiggle = Wiggler.Create(0.5f, 4f, (Action<float>) null, false, false);
      this.Add((Component) this.shieldRadiusWiggle);
      this.moveWiggle = Wiggler.Create(0.8f, 2f, (Action<float>) null, false, false);
      this.moveWiggle.StartZero = true;
      this.Add((Component) this.moveWiggle);
      this.UpdateY();
    }

    public FlyFeather(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Bool(nameof (shielded), false), data.Bool(nameof (singleUse), false))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.level = this.SceneAs<Level>();
    }

    public override void Update()
    {
      base.Update();
      if ((double) this.respawnTimer > 0.0)
      {
        this.respawnTimer -= Engine.DeltaTime;
        if ((double) this.respawnTimer <= 0.0)
          this.Respawn();
      }
      this.UpdateY();
      this.light.Alpha = Calc.Approach(this.light.Alpha, this.sprite.Visible ? 1f : 0.0f, 4f * Engine.DeltaTime);
      this.bloom.Alpha = this.light.Alpha * 0.8f;
    }

    public override void Render()
    {
      base.Render();
      if (!this.shielded || !this.sprite.Visible)
        return;
      Draw.Circle(this.Position + this.sprite.Position, (float) (10.0 - (double) this.shieldRadiusWiggle.Value * 2.0), Color.White, 3);
    }

    private void Respawn()
    {
      if (this.Collidable)
        return;
      this.outline.Visible = false;
      this.Collidable = true;
      this.sprite.Visible = true;
      this.wiggler.Start();
      Audio.Play("event:/game/06_reflection/feather_reappear", this.Position);
      this.level.ParticlesFG.Emit(FlyFeather.P_Respawn, 16, this.Position, Vector2.One * 2f);
    }

    private void UpdateY()
    {
      this.sprite.X = 0.0f;
      this.sprite.Y = this.bloom.Y = this.sine.Value * 2f;
      Sprite sprite = this.sprite;
      sprite.Position = sprite.Position + this.moveWiggleDir * this.moveWiggle.Value * -8f;
    }

    private void OnPlayer(Player player)
    {
      Vector2 speed = player.Speed;
      if (this.shielded && !player.DashAttacking)
      {
        player.PointBounce(this.Center);
        this.moveWiggle.Start();
        this.shieldRadiusWiggle.Start();
        this.moveWiggleDir = (this.Center - player.Center).SafeNormalize(Vector2.UnitY);
        Audio.Play("event:/game/06_reflection/feather_bubble_bounce", this.Position);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      }
      else
      {
        bool flag = player.StateMachine.State == 19;
        if (!player.StartStarFly())
          return;
        if (!flag)
          Audio.Play(this.shielded ? "event:/game/06_reflection/feather_bubble_get" : "event:/game/06_reflection/feather_get", this.Position);
        else
          Audio.Play(this.shielded ? "event:/game/06_reflection/feather_bubble_renew" : "event:/game/06_reflection/feather_renew", this.Position);
        this.Collidable = false;
        this.Add((Component) new Coroutine(this.CollectRoutine(player, speed), true));
        if (!this.singleUse)
        {
          this.outline.Visible = true;
          this.respawnTimer = 3f;
        }
      }
    }

    private IEnumerator CollectRoutine(Player player, Vector2 playerSpeed)
    {
      this.level.Shake(0.3f);
      this.sprite.Visible = false;
      yield return (object) 0.05f;
      float angle = 0.0f;
      angle = !(playerSpeed != Vector2.Zero) ? (this.Position - player.Center).Angle() : playerSpeed.Angle();
      this.level.ParticlesFG.Emit(FlyFeather.P_Collect, 10, this.Position, Vector2.One * 6f);
      SlashFx.Burst(this.Position, angle);
    }
  }
}

