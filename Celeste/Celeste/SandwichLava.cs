// Decompiled with JetBrains decompiler
// Type: Celeste.SandwichLava
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class SandwichLava : Entity
  {
    private bool leaving = false;
    private float delay = 0.0f;
    private const float TopOffset = -160f;
    private const float Speed = 20f;
    public bool Waiting;
    private bool iceMode;
    private float startX;
    private float lerp;
    private float transitionStartY;
    private LavaRect bottomRect;
    private LavaRect topRect;
    private bool persistent;
    private SoundSource loopSfx;

    private float centerY
    {
      get
      {
        return (float) this.SceneAs<Level>().Bounds.Bottom - 10f;
      }
    }

    public SandwichLava(float startX)
    {
      this.startX = startX;
      this.Depth = -1000000;
      this.Collider = (Collider) new ColliderList(new Collider[2]
      {
        (Collider) new Hitbox(340f, 120f, 0.0f, 0.0f),
        (Collider) new Hitbox(340f, 120f, 0.0f, -280f)
      });
      this.Visible = false;
      this.Add((Component) (this.loopSfx = new SoundSource()));
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Add((Component) new CoreModeListener(new Action<Session.CoreModes>(this.OnChangeMode)));
      this.Add((Component) (this.bottomRect = new LavaRect(400f, 200f, 4)));
      this.bottomRect.Position = new Vector2(-40f, 0.0f);
      this.bottomRect.OnlyMode = LavaRect.OnlyModes.OnlyTop;
      this.bottomRect.SmallWaveAmplitude = 2f;
      this.Add((Component) (this.topRect = new LavaRect(400f, 200f, 4)));
      this.topRect.Position = new Vector2(-40f, -360f);
      this.topRect.OnlyMode = LavaRect.OnlyModes.OnlyBottom;
      this.topRect.SmallWaveAmplitude = 2f;
      this.topRect.BigWaveAmplitude = this.bottomRect.BigWaveAmplitude = 2f;
      this.topRect.CurveAmplitude = this.bottomRect.CurveAmplitude = 4f;
      this.Add((Component) new TransitionListener()
      {
        OnOutBegin = (Action) (() =>
        {
          this.transitionStartY = this.Y;
          if (!this.persistent || this.Scene == null || this.Scene.Entities.FindAll<SandwichLava>().Count > 1)
            return;
          this.Leave();
        }),
        OnOut = (Action<float>) (f =>
        {
          if (this.Scene != null)
          {
            this.X = (this.Scene as Level).Camera.X;
            if (!this.leaving)
              this.Y = MathHelper.Lerp(this.transitionStartY, this.centerY, f);
          }
          if (!((double) f > 0.949999988079071 & this.leaving))
            return;
          this.RemoveSelf();
        })
      });
    }

    public SandwichLava(EntityData data, Vector2 offset)
      : this(data.Position.X + offset.X)
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.X = (float) (this.SceneAs<Level>().Bounds.Left - 10);
      this.Y = this.centerY;
      this.iceMode = this.SceneAs<Level>().Session.CoreMode == Session.CoreModes.Cold;
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity != null && (entity.JustRespawned || (double) entity.X < (double) this.startX))
        this.Waiting = true;
      List<SandwichLava> all = this.Scene.Entities.FindAll<SandwichLava>();
      bool flag = false;
      if (!this.persistent && all.Count >= 2)
      {
        SandwichLava sandwichLava = all[0] == this ? all[1] : all[0];
        if (!sandwichLava.leaving)
        {
          sandwichLava.startX = this.startX;
          sandwichLava.Waiting = true;
          this.RemoveSelf();
          flag = true;
        }
      }
      if (flag)
        return;
      this.persistent = true;
      this.Tag = (int) Tags.Persistent;
      if ((scene as Level).LastIntroType != Player.IntroTypes.Respawn)
      {
        this.topRect.Position.Y -= 60f;
        this.bottomRect.Position.Y += 60f;
      }
      else
        this.Visible = true;
      this.loopSfx.Play("event:/game/09_core/rising_threat", "room_state", this.iceMode ? 1f : 0.0f);
      this.loopSfx.Position = new Vector2(this.Width / 2f, 0.0f);
    }

    private void OnChangeMode(Session.CoreModes mode)
    {
      this.iceMode = mode == Session.CoreModes.Cold;
      this.loopSfx.Param("room_state", this.iceMode ? 1f : 0.0f);
    }

    private void OnPlayer(Player player)
    {
      if (this.Waiting)
        return;
      if (SaveData.Instance.Assists.Invincible)
      {
        if ((double) this.delay <= 0.0)
        {
          int num = (double) player.Y > (double) this.Y + (double) this.bottomRect.Position.Y - 32.0 ? 1 : -1;
          float from = this.Y;
          float to = this.Y + (float) (num * 48);
          player.Speed.Y = (float) (-num * 200);
          if (num > 0)
            player.RefillDash();
          Tween.Set((Entity) this, Tween.TweenMode.Oneshot, 0.4f, Ease.CubeOut, (Action<Tween>) (t => this.Y = MathHelper.Lerp(from, to, t.Eased)), (Action<Tween>) null);
          this.delay = 0.5f;
          this.loopSfx.Param("rising", 0.0f);
          Audio.Play("event:/game/general/assist_screenbottom", player.Position);
        }
      }
      else
        player.Die(-Vector2.UnitY, false, true);
    }

    public void Leave()
    {
      this.AddTag((int) Tags.TransitionUpdate);
      this.leaving = true;
      this.Collidable = false;
      Alarm.Set((Entity) this, 2f, (Action) (() => this.RemoveSelf()), Alarm.AlarmMode.Oneshot);
    }

    public override void Update()
    {
      this.X = (this.Scene as Level).Camera.X;
      this.delay -= Engine.DeltaTime;
      base.Update();
      this.Visible = true;
      if (this.Waiting)
      {
        this.Y = Calc.Approach(this.Y, this.centerY, 128f * Engine.DeltaTime);
        this.loopSfx.Param("rising", 0.0f);
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity != null && (double) entity.X >= (double) this.startX && !entity.JustRespawned && entity.StateMachine.State != 11)
          this.Waiting = false;
      }
      else if (!this.leaving && (double) this.delay <= 0.0)
      {
        this.loopSfx.Param("rising", 1f);
        if (this.iceMode)
          this.Y += 20f * Engine.DeltaTime;
        else
          this.Y -= 20f * Engine.DeltaTime;
      }
      this.topRect.Position.Y = Calc.Approach(this.topRect.Position.Y, (float) (-160.0 - (double) this.topRect.Height + (this.leaving ? -512.0 : 0.0)), (this.leaving ? 256f : 64f) * Engine.DeltaTime);
      this.bottomRect.Position.Y = Calc.Approach(this.bottomRect.Position.Y, this.leaving ? 512f : 0.0f, (this.leaving ? 256f : 64f) * Engine.DeltaTime);
      this.lerp = Calc.Approach(this.lerp, this.iceMode ? 1f : 0.0f, Engine.DeltaTime * 4f);
      this.bottomRect.SurfaceColor = Color.Lerp(RisingLava.Hot[0], RisingLava.Cold[0], this.lerp);
      this.bottomRect.EdgeColor = Color.Lerp(RisingLava.Hot[1], RisingLava.Cold[1], this.lerp);
      this.bottomRect.CenterColor = Color.Lerp(RisingLava.Hot[2], RisingLava.Cold[2], this.lerp);
      this.bottomRect.Spikey = this.lerp * 5f;
      this.bottomRect.UpdateMultiplier = (float) ((1.0 - (double) this.lerp) * 2.0);
      this.bottomRect.Fade = this.iceMode ? 128f : 32f;
      this.topRect.SurfaceColor = this.bottomRect.SurfaceColor;
      this.topRect.EdgeColor = this.bottomRect.EdgeColor;
      this.topRect.CenterColor = this.bottomRect.CenterColor;
      this.topRect.Spikey = this.bottomRect.Spikey;
      this.topRect.UpdateMultiplier = this.bottomRect.UpdateMultiplier;
      this.topRect.Fade = this.bottomRect.Fade;
    }
  }
}

