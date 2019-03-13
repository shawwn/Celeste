// Decompiled with JetBrains decompiler
// Type: Celeste.Strawberry
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
  public class Strawberry : Entity
  {
    private float wobble = 0.0f;
    private float collectTimer = 0.0f;
    private bool collected = false;
    public static ParticleType P_Glow;
    public static ParticleType P_GhostGlow;
    public static ParticleType P_GoldGlow;
    public static ParticleType P_WingsBurst;
    public EntityID ID;
    private Sprite sprite;
    public Follower Follower;
    private Wiggler wiggler;
    private Wiggler rotateWiggler;
    private BloomPoint bloom;
    private VertexLight light;
    private Tween lightTween;
    private Vector2 start;
    private bool isGhostBerry;
    private bool flyingAway;
    private float flapSpeed;
    public bool WaitingOnSeeds;
    public List<StrawberrySeed> Seeds;

    public bool Winged { get; private set; }

    public bool Golden { get; private set; }

    private string gotSeedFlag
    {
      get
      {
        return "collected_seeds_of_" + this.ID.ToString();
      }
    }

    public Strawberry(EntityData data, Vector2 offset, EntityID gid)
    {
      this.ID = gid;
      this.Position = this.start = data.Position + offset;
      this.Winged = data.Bool("winged", false) || data.Name == "memorialTextController";
      this.Golden = data.Name == "memorialTextController" || data.Name == "goldenBerry";
      this.isGhostBerry = SaveData.Instance.CheckStrawberry(this.ID);
      this.Depth = -100;
      this.Collider = (Collider) new Hitbox(14f, 14f, -7f, -7f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Add((Component) new MirrorReflection());
      this.Add((Component) (this.Follower = new Follower(this.ID, (Action) null, new Action(this.OnLoseLeader))));
      this.Follower.FollowDelay = 0.3f;
      if (this.Winged)
        this.Add((Component) new DashListener()
        {
          OnDash = new Action<Vector2>(this.OnDash)
        });
      if (data.Nodes == null || (uint) data.Nodes.Length <= 0U)
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
        this.sprite = this.Golden ? GFX.SpriteBank.Create("goldghostberry") : GFX.SpriteBank.Create("ghostberry");
        this.sprite.Color = Color.White * 0.8f;
      }
      else
        this.sprite = !this.Golden ? GFX.SpriteBank.Create("strawberry") : GFX.SpriteBank.Create("goldberry");
      this.Add((Component) this.sprite);
      if (this.Winged)
        this.sprite.Play("flap", false, false);
      this.sprite.OnFrameChange = new Action<string>(this.OnAnimate);
      this.Add((Component) (this.wiggler = Wiggler.Create(0.4f, 4f, (Action<float>) (v => this.sprite.Scale = Vector2.One * (float) (1.0 + (double) v * 0.349999994039536)), false, false)));
      this.Add((Component) (this.rotateWiggler = Wiggler.Create(0.5f, 4f, (Action<float>) (v => this.sprite.Rotation = (float) ((double) v * 30.0 * (Math.PI / 180.0))), false, false)));
      this.Add((Component) (this.bloom = new BloomPoint(this.Golden || this.isGhostBerry ? 0.5f : 1f, 12f)));
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
      if ((double) (scene as Level).Session.BloomBaseAdd <= 0.100000001490116)
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
            else if (entity.OnSafeGround)
              flag = true;
          }
          if (flag)
          {
            this.collectTimer += Engine.DeltaTime;
            if ((double) this.collectTimer > 0.150000005960464)
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
      this.SceneAs<Level>().ParticlesFG.Emit(!this.isGhostBerry ? (!this.Golden ? Strawberry.P_Glow : Strawberry.P_GoldGlow) : Strawberry.P_GhostGlow, this.Position + Calc.Random.Range(-Vector2.One * 6f, Vector2.One * 6f));
    }

    private void OnDash(Vector2 dir)
    {
      if (this.flyingAway || !this.Winged || this.WaitingOnSeeds)
        return;
      this.Depth = -1000000;
      this.Add((Component) new Coroutine(this.FlyAwayRoutine(), true));
      this.flyingAway = true;
    }

    private bool IsFirstStrawberry
    {
      get
      {
        for (int index = this.Follower.FollowIndex - 1; index >= 0; --index)
        {
          Strawberry entity = this.Follower.Leader.Followers[index].Entity as Strawberry;
          if (entity != null && !entity.Golden)
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
      if (this.sprite.CurrentAnimationFrame != (!(id == "flap") ? (!this.Golden ? 35 : 30) : 25))
        return;
      this.lightTween.Start();
      if (!this.collected && (this.CollideCheck<FakeWall>() || this.CollideCheck<Solid>()))
      {
        Audio.Play("event:/game/general/strawberry_pulse", this.Position);
        this.SceneAs<Level>().Displacement.AddBurst(this.Position, 0.6f, 4f, 28f, 0.1f, (Ease.Easer) null, (Ease.Easer) null);
      }
      else
      {
        Audio.Play("event:/game/general/strawberry_pulse", this.Position);
        this.SceneAs<Level>().Displacement.AddBurst(this.Position, 0.6f, 4f, 28f, 0.2f, (Ease.Easer) null, (Ease.Easer) null);
      }
    }

    public void OnPlayer(Player player)
    {
      if (this.Follower.Leader != null || this.collected || this.WaitingOnSeeds)
        return;
      if (this.Winged)
      {
        Level level = this.SceneAs<Level>();
        this.Winged = false;
        this.sprite.Rate = 0.0f;
        Alarm.Set((Entity) this, this.Follower.FollowDelay, (Action) (() =>
        {
          this.sprite.Rate = 1f;
          this.sprite.Play("idle", false, false);
          level.Particles.Emit(Strawberry.P_WingsBurst, 8, this.Position + new Vector2(8f, 0.0f), new Vector2(4f, 2f));
          level.Particles.Emit(Strawberry.P_WingsBurst, 8, this.Position - new Vector2(8f, 0.0f), new Vector2(4f, 2f));
        }), Alarm.AlarmMode.Oneshot);
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
      SaveData.Instance.AddStrawberry(this.ID, this.Golden);
      Session session = (this.Scene as Level).Session;
      session.DoNotLoad.Add(this.ID);
      session.Strawberries.Add(this.ID);
      session.UpdateLevelStartDashes();
      this.Add((Component) new Coroutine(this.CollectRoutine(collectIndex), true));
    }

    private IEnumerator FlyAwayRoutine()
    {
      this.rotateWiggler.Start();
      this.flapSpeed = -200f;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.25f, true);
      tween.OnUpdate = (Action<Tween>) (t => this.flapSpeed = MathHelper.Lerp(-200f, 0.0f, t.Eased));
      this.Add((Component) tween);
      yield return (object) 0.1f;
      Audio.Play("event:/game/general/strawberry_laugh", this.Position);
      yield return (object) 0.2f;
      if (!this.Follower.HasLeader)
        Audio.Play("event:/game/general/strawberry_flyaway", this.Position);
      tween.Stop();
      tween = Tween.Create(Tween.TweenMode.Oneshot, (Ease.Easer) null, 0.5f, true);
      tween.OnUpdate = (Action<Tween>) (t => this.flapSpeed = MathHelper.Lerp(0.0f, -200f, t.Eased));
      this.Add((Component) tween);
    }

    private IEnumerator CollectRoutine(int collectIndex)
    {
      Level level = this.Scene as Level;
      this.Tag = (int) Tags.TransitionUpdate;
      int color = this.isGhostBerry ? 1 : (this.Golden ? 2 : 0);
      Audio.Play("event:/game/general/strawberry_get", this.Position, "colour", (float) color, "count", (float) collectIndex);
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      this.sprite.Play("collect", false, false);
      while (this.sprite.Animating)
        yield return (object) null;
      level.Displacement.AddBurst(this.Position, 0.3f, 16f, 24f, 0.3f, (Ease.Easer) null, (Ease.Easer) null);
      this.Scene.Add((Entity) new StrawberryPoints(this.Position, this.isGhostBerry, collectIndex));
      this.RemoveSelf();
    }

    private void OnLoseLeader()
    {
      if (this.collected)
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
      }), Alarm.AlarmMode.Oneshot);
    }

    public void CollectedSeeds()
    {
      this.WaitingOnSeeds = false;
      this.Visible = true;
      this.Collidable = true;
      this.bloom.Visible = this.light.Visible = true;
      (this.Scene as Level).Session.SetFlag(this.gotSeedFlag, true);
    }
  }
}

