// Decompiled with JetBrains decompiler
// Type: Celeste.Booster
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class Booster : Entity
  {
    private const float RespawnTime = 1f;
    public static ParticleType P_Burst;
    public static ParticleType P_BurstRed;
    public static ParticleType P_Appear;
    public static ParticleType P_RedAppear;
    public static readonly Vector2 playerOffset = new Vector2(0.0f, -2f);
    private Sprite sprite;
    private Entity outline;
    private Wiggler wiggler;
    private BloomPoint bloom;
    private VertexLight light;
    private Coroutine dashRoutine;
    private DashListener dashListener;
    private ParticleType particleType;
    private float respawnTimer;
    private float cannotUseTimer;
    private bool red;
    private SoundSource loopingSfx;
    public bool Ch9HubBooster;
    public bool Ch9HubTransition;

    public bool BoostingPlayer { get; private set; }

    public Booster(Vector2 position, bool red)
      : base(position)
    {
      this.Depth = -8500;
      this.Collider = (Collider) new Monocle.Circle(10f, y: 2f);
      this.red = red;
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create(red ? "boosterRed" : "booster")));
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer)));
      this.Add((Component) (this.light = new VertexLight(Color.White, 1f, 16, 32)));
      this.Add((Component) (this.bloom = new BloomPoint(0.1f, 16f)));
      this.Add((Component) (this.wiggler = Wiggler.Create(0.5f, 4f, (Action<float>) (f => this.sprite.Scale = Vector2.One * (float) (1.0 + (double) f * 0.25)))));
      this.Add((Component) (this.dashRoutine = new Coroutine(false)));
      this.Add((Component) (this.dashListener = new DashListener()));
      this.Add((Component) new MirrorReflection());
      this.Add((Component) (this.loopingSfx = new SoundSource()));
      this.dashListener.OnDash = new Action<Vector2>(this.OnPlayerDashed);
      this.particleType = red ? Booster.P_BurstRed : Booster.P_Burst;
    }

    public Booster(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Bool(nameof (red)))
    {
      this.Ch9HubBooster = data.Bool("ch9_hub_booster");
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

    public void Appear()
    {
      Audio.Play(this.red ? "event:/game/05_mirror_temple/redbooster_reappear" : "event:/game/04_cliffside/greenbooster_reappear", this.Position);
      this.sprite.Play("appear");
      this.wiggler.Start();
      this.Visible = true;
      this.AppearParticles();
    }

    private void AppearParticles()
    {
      ParticleSystem particlesBg = this.SceneAs<Level>().ParticlesBG;
      for (int index = 0; index < 360; index += 30)
        particlesBg.Emit(this.red ? Booster.P_RedAppear : Booster.P_Appear, 1, this.Center, Vector2.One * 2f, (float) index * ((float) Math.PI / 180f));
    }

    private void OnPlayer(Player player)
    {
      if ((double) this.respawnTimer > 0.0 || (double) this.cannotUseTimer > 0.0 || this.BoostingPlayer)
        return;
      this.cannotUseTimer = 0.45f;
      if (this.red)
        player.RedBoost(this);
      else
        player.Boost(this);
      Audio.Play(this.red ? "event:/game/05_mirror_temple/redbooster_enter" : "event:/game/04_cliffside/greenbooster_enter", this.Position);
      this.wiggler.Start();
      this.sprite.Play("inside");
      this.sprite.FlipX = player.Facing == Facings.Left;
    }

    public void PlayerBoosted(Player player, Vector2 direction)
    {
      Audio.Play(this.red ? "event:/game/05_mirror_temple/redbooster_dash" : "event:/game/04_cliffside/greenbooster_dash", this.Position);
      if (this.red)
      {
        this.loopingSfx.Play("event:/game/05_mirror_temple/redbooster_move");
        this.loopingSfx.DisposeOnTransition = false;
      }
      if (this.Ch9HubBooster && (double) direction.Y < 0.0)
      {
        bool flag = true;
        List<LockBlock> all = this.Scene.Entities.FindAll<LockBlock>();
        if (all.Count > 0)
        {
          foreach (LockBlock lockBlock in all)
          {
            if (!lockBlock.UnlockingRegistered)
            {
              flag = false;
              break;
            }
          }
        }
        if (flag)
        {
          this.Ch9HubTransition = true;
          this.Add((Component) Alarm.Create(Alarm.AlarmMode.Oneshot, (Action) (() => this.Add((Component) new SoundSource("event:/new_content/timeline_bubble_to_remembered")
          {
            DisposeOnTransition = false
          })), 2f, true));
        }
      }
      this.BoostingPlayer = true;
      this.Tag = (int) Tags.Persistent | (int) Tags.TransitionUpdate;
      this.sprite.Play("spin");
      this.sprite.FlipX = player.Facing == Facings.Left;
      this.outline.Visible = true;
      this.wiggler.Start();
      this.dashRoutine.Replace(this.BoostRoutine(player, direction));
    }

    private IEnumerator BoostRoutine(Player player, Vector2 dir)
    {
      Booster booster = this;
      float angle = (-dir).Angle();
      while ((player.StateMachine.State == 2 || player.StateMachine.State == 5) && booster.BoostingPlayer)
      {
        booster.sprite.RenderPosition = player.Center + Booster.playerOffset;
        booster.loopingSfx.Position = booster.sprite.Position;
        if (booster.Scene.OnInterval(0.02f))
          (booster.Scene as Level).ParticlesBG.Emit(booster.particleType, 2, player.Center - dir * 3f + new Vector2(0.0f, -2f), new Vector2(3f, 3f), angle);
        yield return (object) null;
      }
      booster.PlayerReleased();
      if (player.StateMachine.State == 4)
        booster.sprite.Visible = false;
      while (booster.SceneAs<Level>().Transitioning)
        yield return (object) null;
      booster.Tag = 0;
    }

    public void OnPlayerDashed(Vector2 direction)
    {
      if (!this.BoostingPlayer)
        return;
      this.BoostingPlayer = false;
    }

    public void PlayerReleased()
    {
      Audio.Play(this.red ? "event:/game/05_mirror_temple/redbooster_end" : "event:/game/04_cliffside/greenbooster_end", this.sprite.RenderPosition);
      this.sprite.Play("pop");
      this.cannotUseTimer = 0.0f;
      this.respawnTimer = 1f;
      this.BoostingPlayer = false;
      this.wiggler.Stop();
      this.loopingSfx.Stop();
    }

    public void PlayerDied()
    {
      if (!this.BoostingPlayer)
        return;
      this.PlayerReleased();
      this.dashRoutine.Active = false;
      this.Tag = 0;
    }

    public void Respawn()
    {
      Audio.Play(this.red ? "event:/game/05_mirror_temple/redbooster_reappear" : "event:/game/04_cliffside/greenbooster_reappear", this.Position);
      this.sprite.Position = Vector2.Zero;
      this.sprite.Play("loop", true);
      this.wiggler.Start();
      this.sprite.Visible = true;
      this.outline.Visible = false;
      this.AppearParticles();
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
      if (!(this.sprite.CurrentAnimationID == "inside") || this.BoostingPlayer || this.CollideCheck<Player>())
        return;
      this.sprite.Play("loop");
    }

    public override void Render()
    {
      Vector2 position = this.sprite.Position;
      this.sprite.Position = position.Floor();
      if (this.sprite.CurrentAnimationID != "pop" && this.sprite.Visible)
        this.sprite.DrawOutline();
      base.Render();
      this.sprite.Position = position;
    }

    public override void Removed(Scene scene)
    {
      if (this.Ch9HubTransition)
      {
        Level level = scene as Level;
        foreach (Backdrop backdrop in level.Background.GetEach<Backdrop>("bright"))
        {
          backdrop.ForceVisible = false;
          backdrop.FadeAlphaMultiplier = 1f;
        }
        level.Bloom.Base = AreaData.Get((Scene) level).BloomBase + 0.25f;
        level.Session.BloomBaseAdd = 0.25f;
      }
      base.Removed(scene);
    }
  }
}
