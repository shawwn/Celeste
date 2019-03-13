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
    public bool Hovering;
    private float hoveringTimer;

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
      this.Hair.Color = Color.Lerp(BadelineOldsite.HairColor, Color.get_White(), (float) index / 6f);
      this.Hair.Border = Color.get_Black();
      this.Add((Component) this.Hair);
      this.Add((Component) this.Sprite);
      this.Visible = false;
      this.followBehindTime = 1.55f;
      this.followBehindIndexDelay = 0.4f * (float) index;
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
    }

    public BadelineOldsite(EntityData data, Vector2 offset, int index)
      : this(Vector2.op_Addition(data.Position, offset), index)
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
      BadelineOldsite badelineOldsite = this;
      badelineOldsite.Hovering = true;
      while ((badelineOldsite.player = badelineOldsite.Scene.Tracker.GetEntity<Player>()) == null || badelineOldsite.player.JustRespawned)
        yield return (object) null;
      Vector2 to = badelineOldsite.player.Position;
      yield return (object) badelineOldsite.followBehindIndexDelay;
      if (!badelineOldsite.Visible)
        badelineOldsite.PopIntoExistance(0.5f);
      badelineOldsite.Sprite.Play("fallSlow", false, false);
      badelineOldsite.Hair.Visible = true;
      badelineOldsite.Hovering = false;
      if (level.Session.Area.Mode == AreaMode.Normal)
      {
        level.Session.Audio.Music.Event = "event:/music/lvl2/chase";
        level.Session.Audio.Apply();
      }
      yield return (object) badelineOldsite.TweenToPlayer(to);
      badelineOldsite.Collidable = true;
      badelineOldsite.following = true;
      badelineOldsite.Add((Component) (badelineOldsite.occlude = new LightOcclude(1f)));
      if (level.Session.Level == "2")
        badelineOldsite.Add((Component) new Coroutine(badelineOldsite.StopChasing(), true));
    }

    private IEnumerator TweenToPlayer(Vector2 to)
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      BadelineOldsite badelineOldsite = this;
      if (num != 0)
      {
        if (num != 1)
          return false;
        // ISSUE: reference to a compiler-generated field
        this.\u003C\u003E1__state = -1;
        return false;
      }
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      Audio.Play("event:/char/badeline/level_entry", badelineOldsite.Position, "chaser_count", (float) badelineOldsite.index);
      Vector2 from = badelineOldsite.Position;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, badelineOldsite.followBehindTime - 0.1f, true);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        this.Position = Vector2.Lerp(from, to, t.Eased);
        if (to.X != from.X)
          this.Sprite.Scale.X = (__Null) ((double) Math.Abs((float) this.Sprite.Scale.X) * (double) Math.Sign((float) (to.X - from.X)));
        this.Trail();
      });
      badelineOldsite.Add((Component) tween);
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) tween.Duration;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }

    private IEnumerator StopChasing()
    {
      BadelineOldsite badelineOldsite = this;
      Level level = badelineOldsite.SceneAs<Level>();
      int boundsRight = level.Bounds.X + 148;
      int boundsBottom = level.Bounds.Y + 168 + 184;
      while ((double) badelineOldsite.X != (double) boundsRight || (double) badelineOldsite.Y != (double) boundsBottom)
      {
        yield return (object) null;
        if ((double) badelineOldsite.X > (double) boundsRight)
          badelineOldsite.X = (float) boundsRight;
        if ((double) badelineOldsite.Y > (double) boundsBottom)
          badelineOldsite.Y = (float) boundsBottom;
      }
      badelineOldsite.following = false;
      badelineOldsite.ignorePlayerAnim = true;
      badelineOldsite.Sprite.Play("laugh", false, false);
      badelineOldsite.Sprite.Scale.X = (__Null) 1.0;
      yield return (object) 1f;
      Audio.Play("event:/char/badeline/disappear", badelineOldsite.Position);
      level.Displacement.AddBurst(badelineOldsite.Center, 0.5f, 24f, 96f, 0.4f, (Ease.Easer) null, (Ease.Easer) null);
      level.Particles.Emit(BadelineOldsite.P_Vanish, 12, badelineOldsite.Center, Vector2.op_Multiply(Vector2.get_One(), 6f));
      badelineOldsite.RemoveSelf();
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
          if (!this.ignorePlayerAnim && chaseState.Animation != this.Sprite.CurrentAnimationID && (chaseState.Animation != null && this.Sprite.Has(chaseState.Animation)))
            this.Sprite.Play(chaseState.Animation, true, false);
          if (!this.ignorePlayerAnim)
            this.Sprite.Scale.X = (__Null) ((double) Math.Abs((float) this.Sprite.Scale.X) * (double) chaseState.Facing);
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
      if (this.Sprite.Scale.X != 0.0)
        this.Hair.Facing = (Facings) Math.Sign((float) this.Sprite.Scale.X);
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
      player.Die(Vector2.op_Subtraction(player.Position, this.Position).SafeNormalize(), false, true);
    }

    private void Die()
    {
      this.RemoveSelf();
    }

    private void PopIntoExistance(float duration)
    {
      this.Visible = true;
      this.Sprite.Scale = Vector2.get_Zero();
      this.Sprite.Color = Color.get_Transparent();
      this.Hair.Visible = true;
      this.Hair.Alpha = 0.0f;
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, duration, true);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        this.Sprite.Scale = Vector2.op_Multiply(Vector2.get_One(), t.Eased);
        this.Sprite.Color = Color.op_Multiply(Color.get_White(), t.Eased);
        this.Hair.Alpha = t.Eased;
      });
      this.Add((Component) tween);
    }

    private bool OnGround(int dist = 1)
    {
      for (int index = 1; index <= dist; ++index)
      {
        if (this.CollideCheck<Solid>(Vector2.op_Addition(this.Position, new Vector2(0.0f, (float) index))))
          return true;
      }
      return false;
    }
  }
}
