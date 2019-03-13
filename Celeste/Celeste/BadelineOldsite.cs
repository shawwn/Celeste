// Decompiled with JetBrains decompiler
// Type: Celeste.BadelineOldsite
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
  [Tracked(false)]
  public class BadelineOldsite : Entity
  {
    public static readonly Color HairColor = Calc.HexToColor("9B3FB5");
    public bool Hovering = false;
    private float hoveringTimer = 0.0f;
    private Dictionary<string, SoundSource> loopingSounds = new Dictionary<string, SoundSource>();
    private List<SoundSource> inactiveLoopingSounds = new List<SoundSource>();
    public static ParticleType P_Vanish;
    public PlayerSprite Sprite;
    public PlayerHair Hair;
    private LightOcclude occlude;
    private bool ignorePlayerAnim;
    private int index;
    private Player player;
    private bool following;
    private float followBehindTime;
    private float followBehindIndexDelay;

    public BadelineOldsite(Vector2 position, int index)
      : base(position)
    {
      this.index = index;
      this.Depth = -1;
      this.Collider = (Collider) new Hitbox(6f, 6f, -3f, -7f);
      this.Collidable = false;
      this.Sprite = new PlayerSprite(PlayerSpriteMode.Badeline);
      this.Sprite.Play("fallSlow", true, false);
      this.Hair = new PlayerHair(this.Sprite);
      this.Hair.Color = Color.Lerp(BadelineOldsite.HairColor, Color.White, (float) index / 6f);
      this.Hair.Border = Color.Black;
      this.Add((Component) this.Hair);
      this.Add((Component) this.Sprite);
      this.Visible = false;
      this.followBehindTime = 1.55f;
      this.followBehindIndexDelay = 0.4f * (float) index;
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
    }

    public BadelineOldsite(EntityData data, Vector2 offset, int index)
      : this(data.Position + offset, index)
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      Session session = this.SceneAs<Level>().Session;
      if (session.GetLevelFlag("11") && session.Area.Mode == AreaMode.Normal)
        this.RemoveSelf();
      else if (!session.GetLevelFlag("3") && session.Area.Mode == AreaMode.Normal)
        this.RemoveSelf();
      else if (!session.GetFlag("evil_maddy_intro") && session.Level == "3" && session.Area.Mode == AreaMode.Normal)
      {
        this.Hovering = false;
        this.Visible = true;
        this.Hair.Visible = false;
        this.Sprite.Play("pretendDead", false, false);
        if (session.Area.Mode == AreaMode.Normal)
        {
          session.Audio.Music.Event = (string) null;
          session.Audio.Apply();
        }
        this.Scene.Add((Entity) new CS02_BadelineIntro(this));
      }
      else
        this.Add((Component) new Coroutine(this.StartChasingRoutine(this.Scene as Level), true));
    }

    public IEnumerator StartChasingRoutine(Level level)
    {
      this.Hovering = true;
      while ((this.player = this.Scene.Tracker.GetEntity<Player>()) == null || this.player.JustRespawned)
        yield return (object) null;
      Vector2 to = this.player.Position;
      yield return (object) this.followBehindIndexDelay;
      if (!this.Visible)
        this.PopIntoExistance(0.5f);
      this.Sprite.Play("fallSlow", false, false);
      this.Hair.Visible = true;
      this.Hovering = false;
      if (level.Session.Area.Mode == AreaMode.Normal)
      {
        level.Session.Audio.Music.Event = "event:/music/lvl2/chase";
        level.Session.Audio.Apply();
      }
      yield return (object) this.TweenToPlayer(to);
      this.Collidable = true;
      this.following = true;
      this.Add((Component) (this.occlude = new LightOcclude(1f)));
      if (level.Session.Level == "2")
        this.Add((Component) new Coroutine(this.StopChasing(), true));
    }

    private IEnumerator TweenToPlayer(Vector2 to)
    {
      Audio.Play("event:/char/badeline/level_entry", this.Position, "chaser_count", (float) this.index);
      Vector2 from = this.Position;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, this.followBehindTime - 0.1f, true);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        this.Position = Vector2.Lerp(from, to, t.Eased);
        if ((double) to.X != (double) from.X)
          this.Sprite.Scale.X = Math.Abs(this.Sprite.Scale.X) * (float) Math.Sign(to.X - from.X);
        this.Trail();
      });
      this.Add((Component) tween);
      yield return (object) tween.Duration;
    }

    private IEnumerator StopChasing()
    {
      Level level = this.SceneAs<Level>();
      int boundsRight = level.Bounds.X + 148;
      int boundsBottom = level.Bounds.Y + 168 + 184;
      while ((double) this.X != (double) boundsRight || (double) this.Y != (double) boundsBottom)
      {
        yield return (object) null;
        if ((double) this.X > (double) boundsRight)
          this.X = (float) boundsRight;
        if ((double) this.Y > (double) boundsBottom)
          this.Y = (float) boundsBottom;
      }
      this.following = false;
      this.ignorePlayerAnim = true;
      this.Sprite.Play("laugh", false, false);
      this.Sprite.Scale.X = 1f;
      yield return (object) 1f;
      Audio.Play("event:/char/badeline/disappear", this.Position);
      level.Displacement.AddBurst(this.Center, 0.5f, 24f, 96f, 0.4f, (Ease.Easer) null, (Ease.Easer) null);
      level.Particles.Emit(BadelineOldsite.P_Vanish, 12, this.Center, Vector2.One * 6f);
      this.RemoveSelf();
    }

    public override void Update()
    {
      if (this.player != null && this.player.Dead)
      {
        this.Sprite.Play("laugh", false, false);
        this.Sprite.X = (float) (Math.Sin((double) this.hoveringTimer) * 4.0);
        this.Hovering = true;
        this.hoveringTimer += Engine.DeltaTime * 2f;
        this.Depth = -12500;
        foreach (KeyValuePair<string, SoundSource> loopingSound in this.loopingSounds)
          loopingSound.Value.Stop(true);
        this.Trail();
      }
      else
      {
        Player.ChaserState chaseState;
        if (this.following && this.player.GetChasePosition(this.Scene.TimeActive, this.followBehindTime + this.followBehindIndexDelay, out chaseState))
        {
          this.Position = Calc.Approach(this.Position, chaseState.Position, 500f * Engine.DeltaTime);
          if (!this.ignorePlayerAnim && chaseState.Animation != this.Sprite.CurrentAnimationID && chaseState.Animation != null && this.Sprite.Has(chaseState.Animation))
            this.Sprite.Play(chaseState.Animation, true, false);
          if (!this.ignorePlayerAnim)
            this.Sprite.Scale.X = Math.Abs(this.Sprite.Scale.X) * (float) chaseState.Facing;
          for (int index = 0; index < chaseState.Sounds; ++index)
          {
            if (chaseState[index].Action == Player.ChaserStateSound.Actions.Oneshot)
              Audio.Play(chaseState[index].Event, this.Position, chaseState[index].Parameter, chaseState[index].ParameterValue, "chaser_count", (float) this.index);
            else if (chaseState[index].Action == Player.ChaserStateSound.Actions.Loop && !this.loopingSounds.ContainsKey(chaseState[index].Event))
            {
              SoundSource soundSource;
              if (this.inactiveLoopingSounds.Count > 0)
              {
                soundSource = this.inactiveLoopingSounds[0];
                this.inactiveLoopingSounds.RemoveAt(0);
              }
              else
                this.Add((Component) (soundSource = new SoundSource()));
              soundSource.Play(chaseState[index].Event, "chaser_count", (float) this.index);
              this.loopingSounds.Add(chaseState[index].Event, soundSource);
            }
            else if (chaseState[index].Action == Player.ChaserStateSound.Actions.Stop)
            {
              SoundSource soundSource = (SoundSource) null;
              if (this.loopingSounds.TryGetValue(chaseState[index].Event, out soundSource))
              {
                soundSource.Stop(true);
                this.loopingSounds.Remove(chaseState[index].Event);
                this.inactiveLoopingSounds.Add(soundSource);
              }
            }
          }
          this.Depth = chaseState.Depth;
          this.Trail();
        }
      }
      if ((double) this.Sprite.Scale.X != 0.0)
        this.Hair.Facing = (Facings) Math.Sign(this.Sprite.Scale.X);
      if (this.Hovering)
      {
        this.hoveringTimer += Engine.DeltaTime;
        this.Sprite.Y = (float) (Math.Sin((double) this.hoveringTimer * 2.0) * 4.0);
      }
      else
        this.Sprite.Y = Calc.Approach(this.Sprite.Y, 0.0f, Engine.DeltaTime * 4f);
      if (this.occlude != null)
        this.occlude.Visible = !this.CollideCheck<Solid>();
      base.Update();
    }

    private void Trail()
    {
      if (!this.Scene.OnInterval(0.1f))
        return;
      TrailManager.Add((Entity) this, Player.NormalHairColor, 1f);
    }

    private void OnPlayer(Player player)
    {
      player.Die((player.Position - this.Position).SafeNormalize(), false, true);
    }

    private void Die()
    {
      this.RemoveSelf();
    }

    private void PopIntoExistance(float duration)
    {
      this.Visible = true;
      this.Sprite.Scale = Vector2.Zero;
      this.Sprite.Color = Color.Transparent;
      this.Hair.Visible = true;
      this.Hair.Alpha = 0.0f;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, duration, true);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        this.Sprite.Scale = Vector2.One * t.Eased;
        this.Sprite.Color = Color.White * t.Eased;
        this.Hair.Alpha = t.Eased;
      });
      this.Add((Component) tween);
    }

    private bool OnGround(int dist = 1)
    {
      for (int index = 1; index <= dist; ++index)
      {
        if (this.CollideCheck<Solid>(this.Position + new Vector2(0.0f, (float) index)))
          return true;
      }
      return false;
    }
  }
}

