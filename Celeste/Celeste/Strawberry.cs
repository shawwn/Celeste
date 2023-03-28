// Decompiled with JetBrains decompiler
// Type: Celeste.Strawberry
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
  public class Strawberry : Entity
  {
    public static ParticleType P_Glow;
    public static ParticleType P_GhostGlow;
    public static ParticleType P_GoldGlow;
    public static ParticleType P_MoonGlow;
    public static ParticleType P_WingsBurst;
    public EntityID ID;
    private Sprite sprite;
    public Follower Follower;
    private Wiggler wiggler;
    private Wiggler rotateWiggler;
    private BloomPoint bloom;
    private VertexLight light;
    private Tween lightTween;
    private float wobble;
    private Vector2 start;
    private float collectTimer;
    private bool collected;
    private bool isGhostBerry;
    private bool flyingAway;
    private float flapSpeed;
    public bool ReturnHomeWhenLost = true;
    public bool WaitingOnSeeds;
    public List<StrawberrySeed> Seeds;

    public bool Winged { get; private set; }

    public bool Golden { get; private set; }

    public bool Moon { get; private set; }

    private string gotSeedFlag => "collected_seeds_of_" + this.ID.ToString();

    public Strawberry(EntityData data, Vector2 offset, EntityID gid)
    {
      this.ID = gid;
      this.Position = this.start = data.Position + offset;
      this.Winged = data.Bool("winged") || data.Name == "memorialTextController";
      this.Golden = data.Name == "memorialTextController" || data.Name == "goldenBerry";
      this.Moon = data.Bool("moon");
      this.isGhostBerry = SaveData.Instance.CheckStrawberry(this.ID);
      this.Depth = -100;
      this.Collider = (Collider) new Hitbox(14f, 14f, -7f, -7f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer)));
      this.Add((Component) new MirrorReflection());
      this.Add((Component) (this.Follower = new Follower(this.ID, onLoseLeader: new Action(this.OnLoseLeader))));
      this.Follower.FollowDelay = 0.3f;
      if (this.Winged)
        this.Add((Component) new DashListener()
        {
          OnDash = new Action<Vector2>(this.OnDash)
        });
      if (data.Nodes == null || data.Nodes.Length == 0)
        return;
      this.Seeds = new List<StrawberrySeed>();
      for (int index = 0; index < data.Nodes.Length; ++index)
        this.Seeds.Add(new StrawberrySeed(this, offset + data.Nodes[index], index, this.isGhostBerry));
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if (SaveData.Instance.CheckStrawberry(this.ID))
      {
        this.sprite = !this.Moon ? (!this.Golden ? GFX.SpriteBank.Create("ghostberry") : GFX.SpriteBank.Create("goldghostberry")) : GFX.SpriteBank.Create("moonghostberry");
        this.sprite.Color = Color.White * 0.8f;
      }
      else
        this.sprite = !this.Moon ? (!this.Golden ? GFX.SpriteBank.Create("strawberry") : GFX.SpriteBank.Create("goldberry")) : GFX.SpriteBank.Create("moonberry");
      this.Add((Component) this.sprite);
      if (this.Winged)
        this.sprite.Play("flap");
      this.sprite.OnFrameChange = new Action<string>(this.OnAnimate);
      this.Add((Component) (this.wiggler = Wiggler.Create(0.4f, 4f, (Action<float>) (v => this.sprite.Scale = Vector2.One * (float) (1.0 + (double) v * 0.3499999940395355)))));
      this.Add((Component) (this.rotateWiggler = Wiggler.Create(0.5f, 4f, (Action<float>) (v => this.sprite.Rotation = (float) ((double) v * 30.0 * (Math.PI / 180.0))))));
      this.Add((Component) (this.bloom = new BloomPoint(this.Golden || this.Moon || this.isGhostBerry ? 0.5f : 1f, 12f)));
      this.Add((Component) (this.light = new VertexLight(Color.White, 1f, 16, 24)));
      this.Add((Component) (this.lightTween = this.light.CreatePulseTween()));
      if (this.Seeds != null && this.Seeds.Count > 0 && !(scene as Level).Session.GetFlag(this.gotSeedFlag))
      {
        foreach (StrawberrySeed seed in this.Seeds)
          scene.Add((Entity) seed);
        this.Visible = false;
        this.Collidable = false;
        this.WaitingOnSeeds = true;
        this.bloom.Visible = this.light.Visible = false;
      }
      if ((double) (scene as Level).Session.BloomBaseAdd <= 0.10000000149011612)
        return;
      this.bloom.Alpha *= 0.5f;
    }

    public override void Update()
    {
      if (this.WaitingOnSeeds)
        return;
      if (!this.collected)
      {
        if (!this.Winged)
        {
          this.wobble += Engine.DeltaTime * 4f;
          this.sprite.Y = this.bloom.Y = this.light.Y = (float) Math.Sin((double) this.wobble) * 2f;
        }
        int followIndex = this.Follower.FollowIndex;
        if (this.Follower.Leader != null && (double) this.Follower.DelayTimer <= 0.0 && this.IsFirstStrawberry)
        {
          Player entity = this.Follower.Leader.Entity as Player;
          bool flag = false;
          if (entity != null && entity.Scene != null && !entity.StrawberriesBlocked)
          {
            if (this.Golden)
            {
              if (entity.CollideCheck<GoldBerryCollectTrigger>() || (this.Scene as Level).Completed)
                flag = true;
            }
            else if (entity.OnSafeGround && (!this.Moon || entity.StateMachine.State != 13))
              flag = true;
          }
          if (flag)
          {
            this.collectTimer += Engine.DeltaTime;
            if ((double) this.collectTimer > 0.15000000596046448)
              this.OnCollect();
          }
          else
            this.collectTimer = Math.Min(this.collectTimer, 0.0f);
        }
        else
        {
          if (followIndex > 0)
            this.collectTimer = -0.15f;
          if (this.Winged)
          {
            this.Y += this.flapSpeed * Engine.DeltaTime;
            if (this.flyingAway)
            {
              if ((double) this.Y < (double) (this.SceneAs<Level>().Bounds.Top - 16))
                this.RemoveSelf();
            }
            else
            {
              this.flapSpeed = Calc.Approach(this.flapSpeed, 20f, 170f * Engine.DeltaTime);
              if ((double) this.Y < (double) this.start.Y - 5.0)
                this.Y = this.start.Y - 5f;
              else if ((double) this.Y > (double) this.start.Y + 5.0)
                this.Y = this.start.Y + 5f;
            }
          }
        }
      }
      base.Update();
      if (this.Follower.Leader == null || !this.Scene.OnInterval(0.08f))
        return;
      ParticleType type = !this.isGhostBerry ? (!this.Golden ? (!this.Moon ? Strawberry.P_Glow : Strawberry.P_MoonGlow) : Strawberry.P_GoldGlow) : Strawberry.P_GhostGlow;
      this.SceneAs<Level>().ParticlesFG.Emit(type, this.Position + Calc.Random.Range(-Vector2.One * 6f, Vector2.One * 6f));
    }

    private void OnDash(Vector2 dir)
    {
      if (this.flyingAway || !this.Winged || this.WaitingOnSeeds)
        return;
      this.Depth = -1000000;
      this.Add((Component) new Coroutine(this.FlyAwayRoutine()));
      this.flyingAway = true;
    }

    private bool IsFirstStrawberry
    {
      get
      {
        for (int index = this.Follower.FollowIndex - 1; index >= 0; --index)
        {
          if (this.Follower.Leader.Followers[index].Entity is Strawberry entity && !entity.Golden)
            return false;
        }
        return true;
      }
    }

    private void OnAnimate(string id)
    {
      if (!this.flyingAway && id == "flap" && this.sprite.CurrentAnimationFrame % 9 == 4)
      {
        Audio.Play("event:/game/general/strawberry_wingflap", this.Position);
        this.flapSpeed = -50f;
      }
      if (this.sprite.CurrentAnimationFrame != (!(id == "flap") ? (!this.Golden ? (!this.Moon ? 35 : 30) : 30) : 25))
        return;
      this.lightTween.Start();
      if (!this.collected && (this.CollideCheck<FakeWall>() || this.CollideCheck<Solid>()))
      {
        Audio.Play("event:/game/general/strawberry_pulse", this.Position);
        this.SceneAs<Level>().Displacement.AddBurst(this.Position, 0.6f, 4f, 28f, 0.1f);
      }
      else
      {
        Audio.Play("event:/game/general/strawberry_pulse", this.Position);
        this.SceneAs<Level>().Displacement.AddBurst(this.Position, 0.6f, 4f, 28f, 0.2f);
      }
    }

    public void OnPlayer(Player player)
    {
      if (this.Follower.Leader != null || this.collected || this.WaitingOnSeeds)
        return;
      this.ReturnHomeWhenLost = true;
      if (this.Winged)
      {
        Level level = this.SceneAs<Level>();
        this.Winged = false;
        this.sprite.Rate = 0.0f;
        Alarm.Set((Entity) this, this.Follower.FollowDelay, (Action) (() =>
        {
          this.sprite.Rate = 1f;
          this.sprite.Play("idle");
          level.Particles.Emit(Strawberry.P_WingsBurst, 8, this.Position + new Vector2(8f, 0.0f), new Vector2(4f, 2f));
          level.Particles.Emit(Strawberry.P_WingsBurst, 8, this.Position - new Vector2(8f, 0.0f), new Vector2(4f, 2f));
        }));
      }
      if (this.Golden)
        (this.Scene as Level).Session.GrabbedGolden = true;
      Audio.Play(this.isGhostBerry ? "event:/game/general/strawberry_blue_touch" : "event:/game/general/strawberry_touch", this.Position);
      player.Leader.GainFollower(this.Follower);
      this.wiggler.Start();
      this.Depth = -1000000;
    }

    public void OnCollect()
    {
      if (this.collected)
        return;
      int collectIndex = 0;
      this.collected = true;
      if (this.Follower.Leader != null)
      {
        Player entity = this.Follower.Leader.Entity as Player;
        collectIndex = entity.StrawberryCollectIndex;
        ++entity.StrawberryCollectIndex;
        entity.StrawberryCollectResetTimer = 2.5f;
        this.Follower.Leader.LoseFollower(this.Follower);
      }
      if (this.Moon)
        Achievements.Register(Achievement.WOW);
      SaveData.Instance.AddStrawberry(this.ID, this.Golden);
      Session session = (this.Scene as Level).Session;
      session.DoNotLoad.Add(this.ID);
      session.Strawberries.Add(this.ID);
      session.UpdateLevelStartDashes();
      this.Add((Component) new Coroutine(this.CollectRoutine(collectIndex)));
    }

    private IEnumerator FlyAwayRoutine()
    {
      Strawberry strawberry = this;
      strawberry.rotateWiggler.Start();
      strawberry.flapSpeed = -200f;
      Tween tween1 = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.25f, true);
      // ISSUE: reference to a compiler-generated method
      // tween1.OnUpdate = new Action<Tween>(strawberry.\u003CFlyAwayRoutine\u003Eb__46_0);
      tween1.OnUpdate = (Action<Tween>) (t => this.flapSpeed = MathHelper.Lerp(-200f, 0.0f, t.Eased));
      strawberry.Add((Component) tween1);
      yield return (object) 0.1f;
      Audio.Play("event:/game/general/strawberry_laugh", strawberry.Position);
      yield return (object) 0.2f;
      if (!strawberry.Follower.HasLeader)
        Audio.Play("event:/game/general/strawberry_flyaway", strawberry.Position);
      Tween tween2 = Tween.Create(Tween.TweenMode.Oneshot, duration: 0.5f, start: true);
      // ISSUE: reference to a compiler-generated method
      // tween2.OnUpdate = new Action<Tween>(strawberry.\u003CFlyAwayRoutine\u003Eb__46_1);
      tween2.OnUpdate = (Action<Tween>) (t => this.flapSpeed = MathHelper.Lerp(0.0f, -200f, t.Eased));
      strawberry.Add((Component) tween2);
    }

    private IEnumerator CollectRoutine(int collectIndex)
    {
      Strawberry strawberry = this;
      Scene scene = strawberry.Scene;
      strawberry.Tag = (int) Tags.TransitionUpdate;
      strawberry.Depth = -2000010;
      int num = 0;
      if (strawberry.Moon)
        num = 3;
      else if (strawberry.isGhostBerry)
        num = 1;
      else if (strawberry.Golden)
        num = 2;
      Audio.Play("event:/game/general/strawberry_get", strawberry.Position, "colour", (float) num, "count", (float) collectIndex);
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      strawberry.sprite.Play("collect");
      while (strawberry.sprite.Animating)
        yield return (object) null;
      strawberry.Scene.Add((Entity) new StrawberryPoints(strawberry.Position, strawberry.isGhostBerry, collectIndex, strawberry.Moon));
      strawberry.RemoveSelf();
    }

    private void OnLoseLeader()
    {
      if (this.collected || !this.ReturnHomeWhenLost)
        return;
      Alarm.Set((Entity) this, 0.15f, (Action) (() =>
      {
        Vector2 vector = (this.start - this.Position).SafeNormalize();
        float val = Vector2.Distance(this.Position, this.start);
        float num = Calc.ClampedMap(val, 16f, 120f, 16f, 96f);
        SimpleCurve curve = new SimpleCurve(this.Position, this.start, this.start + vector * 16f + vector.Perpendicular() * num * (float) Calc.Random.Choose<int>(1, -1));
        Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineOut, MathHelper.Max(val / 100f, 0.4f), true);
        tween.OnUpdate = (Action<Tween>) (f => this.Position = curve.GetPoint(f.Eased));
        tween.OnComplete = (Action<Tween>) (f => this.Depth = 0);
        this.Add((Component) tween);
      }));
    }

    public void CollectedSeeds()
    {
      this.WaitingOnSeeds = false;
      this.Visible = true;
      this.Collidable = true;
      this.bloom.Visible = this.light.Visible = true;
      (this.Scene as Level).Session.SetFlag(this.gotSeedFlag);
    }
  }
}
