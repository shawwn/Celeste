// Decompiled with JetBrains decompiler
// Type: Celeste.Booster
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class Booster : Entity
  {
    public static readonly Vector2 playerOffset = new Vector2(0.0f, -2f);
    private const float RespawnTime = 1f;
    public static ParticleType P_Burst;
    public static ParticleType P_BurstRed;
    private Sprite sprite;
    private Entity outline;
    private Wiggler wiggler;
    private BloomPoint bloom;
    private VertexLight light;
    private Coroutine dashRoutine;
    private DashListener dashListener;
    private ParticleType particleType;
    private bool boostingPlayer;
    private float respawnTimer;
    private float cannotUseTimer;
    private bool red;
    private SoundSource loopingSfx;

    public Booster(Vector2 position, bool red)
      : base(position)
    {
      this.Depth = -8500;
      this.Collider = (Collider) new Monocle.Circle(10f, 0.0f, 2f);
      this.red = red;
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create(red ? "boosterRed" : "booster")));
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Add((Component) (this.light = new VertexLight(Color.White, 1f, 16, 32)));
      this.Add((Component) (this.bloom = new BloomPoint(0.1f, 16f)));
      this.Add((Component) (this.wiggler = Wiggler.Create(0.5f, 4f, (Action<float>) (f => this.sprite.Scale = Vector2.One * (float) (1.0 + (double) f * 0.25)), false, false)));
      this.Add((Component) (this.dashRoutine = new Coroutine(false)));
      this.Add((Component) (this.dashListener = new DashListener()));
      this.Add((Component) new MirrorReflection());
      this.Add((Component) (this.loopingSfx = new SoundSource()));
      this.dashListener.OnDash = new Action<Vector2>(this.OnPlayerDashed);
      this.particleType = red ? Booster.P_BurstRed : Booster.P_Burst;
    }

    public Booster(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Bool(nameof (red), false))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      Monocle.Image image = new Monocle.Image(GFX.Game["objects/booster/outline"]);
      image.CenterOrigin();
      image.Color = Color.White * 0.75f;
      this.outline = new Entity(this.Position);
      this.outline.Depth = 8999;
      this.outline.Visible = false;
      this.outline.Add((Component) image);
      this.outline.Add((Component) new MirrorReflection());
      scene.Add(this.outline);
    }

    private void OnPlayer(Player player)
    {
      if ((double) this.respawnTimer > 0.0 || (double) this.cannotUseTimer > 0.0 || this.boostingPlayer)
        return;
      this.cannotUseTimer = 0.45f;
      if (this.red)
        player.RedBoost(this);
      else
        player.Boost(this);
      Audio.Play(this.red ? "event:/game/05_mirror_temple/redbooster_enter" : "event:/game/04_cliffside/greenbooster_enter", this.Position);
      this.wiggler.Start();
      this.sprite.Play("inside", false, false);
      this.sprite.FlipX = player.Facing == Facings.Left;
    }

    public void PlayerBoosted(Player player, Vector2 direction)
    {
      Audio.Play(this.red ? "event:/game/05_mirror_temple/redbooster_dash" : "event:/game/04_cliffside/greenbooster_dash", this.Position);
      if (this.red)
      {
        this.loopingSfx.Play("event:/game/05_mirror_temple/redbooster_move", (string) null, 0.0f);
        this.loopingSfx.DisposeOnTransition = false;
      }
      this.boostingPlayer = true;
      this.Tag = (int) Tags.Persistent | (int) Tags.TransitionUpdate;
      this.sprite.Play("spin", false, false);
      this.sprite.FlipX = player.Facing == Facings.Left;
      this.outline.Visible = true;
      this.wiggler.Start();
      this.dashRoutine.Replace(this.BoostRoutine(player, direction));
    }

    private IEnumerator BoostRoutine(Player player, Vector2 dir)
    {
      float angle = (-dir).Angle();
      while ((player.StateMachine.State == 2 || player.StateMachine.State == 5) && this.boostingPlayer)
      {
        this.sprite.RenderPosition = player.Center + Booster.playerOffset;
        this.loopingSfx.Position = this.sprite.Position;
        if (this.Scene.OnInterval(0.02f))
          (this.Scene as Level).ParticlesBG.Emit(this.particleType, 2, player.Center - dir * 3f + new Vector2(0.0f, -2f), new Vector2(3f, 3f), angle);
        yield return (object) null;
      }
      this.PlayerReleased();
      if (player.StateMachine.State == 4)
        this.sprite.Visible = false;
      while (this.SceneAs<Level>().Transitioning)
        yield return (object) null;
      this.Tag = 0;
    }

    public void OnPlayerDashed(Vector2 direction)
    {
      if (!this.boostingPlayer)
        return;
      this.boostingPlayer = false;
    }

    public void PlayerReleased()
    {
      Audio.Play(this.red ? "event:/game/05_mirror_temple/redbooster_end" : "event:/game/04_cliffside/greenbooster_end", this.sprite.RenderPosition);
      this.sprite.Play("pop", false, false);
      this.cannotUseTimer = 0.0f;
      this.respawnTimer = 1f;
      this.boostingPlayer = false;
      this.wiggler.Stop();
      this.loopingSfx.Stop(true);
    }

    public void PlayerDied()
    {
      if (!this.boostingPlayer)
        return;
      this.PlayerReleased();
      this.dashRoutine.Active = false;
      this.Tag = 0;
    }

    public void Respawn()
    {
      Audio.Play(this.red ? "event:/game/05_mirror_temple/redbooster_reappear" : "event:/game/04_cliffside/greenbooster_reappear", this.Position);
      this.sprite.Position = Vector2.Zero;
      this.sprite.Play("loop", true, false);
      this.wiggler.Start();
      this.sprite.Visible = true;
      this.outline.Visible = false;
    }

    public override void Update()
    {
      base.Update();
      if ((double) this.cannotUseTimer > 0.0)
        this.cannotUseTimer -= Engine.DeltaTime;
      if ((double) this.respawnTimer > 0.0)
      {
        this.respawnTimer -= Engine.DeltaTime;
        if ((double) this.respawnTimer <= 0.0)
          this.Respawn();
      }
      if (!this.dashRoutine.Active && (double) this.respawnTimer <= 0.0)
      {
        Vector2 target = Vector2.Zero;
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity != null && this.CollideCheck((Entity) entity))
          target = entity.Center + Booster.playerOffset - this.Position;
        this.sprite.Position = Calc.Approach(this.sprite.Position, target, 80f * Engine.DeltaTime);
      }
      if (!(this.sprite.CurrentAnimationID == "inside") || this.boostingPlayer || this.CollideCheck<Player>())
        return;
      this.sprite.Play("loop", false, false);
    }

    public override void Render()
    {
      if (this.sprite.CurrentAnimationID != "pop" && this.sprite.Visible)
        this.sprite.DrawOutline(1);
      base.Render();
    }
  }
}

