// Decompiled with JetBrains decompiler
// Type: Celeste.Key
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class Key : Entity
  {
    public static ParticleType P_Shimmer;
    public static ParticleType P_Insert;
    public static ParticleType P_Collect;
    public EntityID ID;
    public bool IsUsed;
    public bool StartedUsing;
    private Follower follower;
    private Sprite sprite;
    private Wiggler wiggler;
    private VertexLight light;
    private ParticleEmitter shimmerParticles;
    private float wobble;
    private bool wobbleActive;
    private Tween tween;
    private Alarm alarm;
    private Vector2[] nodes;

    public bool Turning { get; private set; }

    public Key(Vector2 position, EntityID id, Vector2[] nodes)
      : base(position)
    {
      this.ID = id;
      this.Collider = (Collider) new Hitbox(12f, 12f, -6f, -6f);
      this.nodes = nodes;
      this.Add((Component) (this.follower = new Follower(id, (Action) null, (Action) null)));
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Add((Component) new MirrorReflection());
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("key")));
      this.sprite.CenterOrigin();
      this.sprite.Play("idle", false, false);
      this.Add((Component) new TransitionListener()
      {
        OnOut = (Action<float>) (f =>
        {
          this.StartedUsing = false;
          if (this.IsUsed)
            return;
          if (this.tween != null)
          {
            this.tween.RemoveSelf();
            this.tween = (Tween) null;
          }
          if (this.alarm != null)
          {
            this.alarm.RemoveSelf();
            this.alarm = (Alarm) null;
          }
          this.Turning = false;
          this.Visible = true;
          this.sprite.Visible = true;
          this.sprite.Rate = 1f;
          this.sprite.Scale = Vector2.One;
          this.sprite.Play("idle", false, false);
          this.sprite.Rotation = 0.0f;
          this.wiggler.Stop();
          this.follower.MoveTowardsLeader = true;
        })
      });
      this.Add((Component) (this.wiggler = Wiggler.Create(0.4f, 4f, (Action<float>) (v => this.sprite.Scale = Vector2.One * (float) (1.0 + (double) v * 0.349999994039536)), false, false)));
      this.Add((Component) (this.light = new VertexLight(Color.White, 1f, 32, 48)));
    }

    public Key(EntityData data, Vector2 offset, EntityID id)
      : this(data.Position + offset, id, data.NodesOffset(offset))
    {
    }

    public Key(Player player, EntityID id)
      : this(player.Position + new Vector2((float) (-12 * (int) player.Facing), -8f), id, (Vector2[]) null)
    {
      player.Leader.GainFollower(this.follower);
      this.Collidable = false;
      this.Depth = -1000000;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.Add((Component) (this.shimmerParticles = new ParticleEmitter((scene as Level).ParticlesFG, Key.P_Shimmer, Vector2.Zero, new Vector2(6f, 6f), 1, 0.1f)));
      this.shimmerParticles.SimulateCycle();
    }

    public override void Update()
    {
      if (this.wobbleActive)
      {
        this.wobble += Engine.DeltaTime * 4f;
        this.sprite.Y = (float) Math.Sin((double) this.wobble);
      }
      base.Update();
    }

    private void OnPlayer(Player player)
    {
      this.SceneAs<Level>().Particles.Emit(Key.P_Collect, 10, this.Position, Vector2.One * 3f);
      Audio.Play("event:/game/general/key_get", this.Position);
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      player.Leader.GainFollower(this.follower);
      this.Collidable = false;
      Session session = this.SceneAs<Level>().Session;
      session.DoNotLoad.Add(this.ID);
      session.Keys.Add(this.ID);
      session.UpdateLevelStartDashes();
      this.wiggler.Start();
      this.Depth = -1000000;
      if (this.nodes == null || this.nodes.Length < 2)
        return;
      this.Add((Component) new Coroutine(this.NodeRoutine(player), true));
    }

    private IEnumerator NodeRoutine(Player player)
    {
      yield return (object) 0.3f;
      if (!player.Dead)
      {
        Audio.Play("event:/game/general/cassette_bubblereturn", this.SceneAs<Level>().Camera.Position + new Vector2(160f, 90f));
        player.StartCassetteFly(this.nodes[1], this.nodes[0]);
      }
    }

    public void RegisterUsed()
    {
      this.IsUsed = true;
      if (this.follower.Leader != null)
        this.follower.Leader.LoseFollower(this.follower);
      this.SceneAs<Level>().Session.Keys.Remove(this.ID);
    }

    public IEnumerator UseRoutine(Vector2 target)
    {
      this.Turning = true;
      this.follower.MoveTowardsLeader = false;
      this.wiggler.Start();
      this.wobbleActive = false;
      this.sprite.Y = 0.0f;
      Vector2 from = this.Position;
      SimpleCurve curve = new SimpleCurve(from, target, (target + from) / 2f + new Vector2(0.0f, -48f));
      this.tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 1f, true);
      this.tween.OnUpdate = (Action<Tween>) (t =>
      {
        this.Position = curve.GetPoint(t.Eased);
        this.sprite.Rate = (float) (1.0 + (double) t.Eased * 2.0);
      });
      this.Add((Component) this.tween);
      yield return (object) this.tween.Wait();
      this.tween = (Tween) null;
      while (this.sprite.CurrentAnimationFrame != 4)
        yield return (object) null;
      this.shimmerParticles.Active = false;
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      for (int i = 0; i < 16; ++i)
        this.SceneAs<Level>().ParticlesFG.Emit(Key.P_Insert, this.Center, 0.3926991f * (float) i);
      this.sprite.Play("enter", false, false);
      yield return (object) 0.3f;
      this.tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, 0.3f, true);
      this.tween.OnUpdate = (Action<Tween>) (t => this.sprite.Rotation = t.Eased * 1.570796f);
      this.Add((Component) this.tween);
      yield return (object) this.tween.Wait();
      this.tween = (Tween) null;
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      this.alarm = Alarm.Set((Entity) this, 1f, (Action) (() =>
      {
        this.alarm = (Alarm) null;
        this.tween = Tween.Create(Tween.TweenMode.Oneshot, (Ease.Easer) null, 1f, true);
        this.tween.OnUpdate = (Action<Tween>) (t => this.light.Alpha = 1f - t.Eased);
        this.tween.OnComplete = (Action<Tween>) (t => this.RemoveSelf());
        this.Add((Component) this.tween);
      }), Alarm.AlarmMode.Oneshot);
      yield return (object) 0.2f;
      for (int i = 0; i < 8; ++i)
        this.SceneAs<Level>().ParticlesFG.Emit(Key.P_Insert, this.Center, 0.7853982f * (float) i);
      this.sprite.Visible = false;
      this.Turning = false;
    }
  }
}

