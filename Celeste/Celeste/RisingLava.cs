// Decompiled with JetBrains decompiler
// Type: Celeste.RisingLava
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class RisingLava : Entity
  {
    public static Color[] Hot = new Color[3]
    {
      Calc.HexToColor("ff8933"),
      Calc.HexToColor("f25e29"),
      Calc.HexToColor("d01c01")
    };
    public static Color[] Cold = new Color[3]
    {
      Calc.HexToColor("33ffe7"),
      Calc.HexToColor("4ca2eb"),
      Calc.HexToColor("0151d0")
    };
    private float delay = 0.0f;
    private const float Speed = -30f;
    private bool intro;
    private bool iceMode;
    private bool waiting;
    private float lerp;
    private LavaRect bottomRect;
    private SoundSource loopSfx;

    public RisingLava(bool intro)
    {
      this.intro = intro;
      this.Depth = -1000000;
      this.Collider = (Collider) new Hitbox(340f, 120f, 0.0f, 0.0f);
      this.Visible = false;
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Add((Component) new CoreModeListener(new Action<Session.CoreModes>(this.OnChangeMode)));
      this.Add((Component) (this.loopSfx = new SoundSource()));
      this.Add((Component) (this.bottomRect = new LavaRect(400f, 200f, 4)));
      this.bottomRect.Position = new Vector2(-40f, 0.0f);
      this.bottomRect.OnlyMode = LavaRect.OnlyModes.OnlyTop;
      this.bottomRect.SmallWaveAmplitude = 2f;
    }

    public RisingLava(EntityData data, Vector2 offset)
      : this(data.Bool(nameof (intro), false))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.X = (float) (this.SceneAs<Level>().Bounds.Left - 10);
      this.Y = (float) (this.SceneAs<Level>().Bounds.Bottom + 16);
      this.iceMode = this.SceneAs<Level>().Session.CoreMode == Session.CoreModes.Cold;
      this.loopSfx.Play("event:/game/09_core/rising_threat", "room_state", this.iceMode ? 1f : 0.0f);
      this.loopSfx.Position = new Vector2(this.Width / 2f, 0.0f);
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (this.intro)
      {
        this.waiting = true;
      }
      else
      {
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity != null && entity.JustRespawned)
          this.waiting = true;
      }
      if (!this.intro)
        return;
      this.Visible = true;
    }

    private void OnChangeMode(Session.CoreModes mode)
    {
      this.iceMode = mode == Session.CoreModes.Cold;
      this.loopSfx.Param("room_state", this.iceMode ? 1f : 0.0f);
    }

    private void OnPlayer(Player player)
    {
      if (SaveData.Instance.Assists.Invincible)
      {
        if ((double) this.delay > 0.0)
          return;
        float from = this.Y;
        float to = this.Y + 48f;
        player.Speed.Y = -200f;
        player.RefillDash();
        Tween.Set((Entity) this, Tween.TweenMode.Oneshot, 0.4f, Ease.CubeOut, (Action<Tween>) (t => this.Y = MathHelper.Lerp(from, to, t.Eased)), (Action<Tween>) null);
        this.delay = 0.5f;
        this.loopSfx.Param("rising", 0.0f);
        Audio.Play("event:/game/general/assist_screenbottom", player.Position);
      }
      else
        player.Die(-Vector2.UnitY, false, true);
    }

    public override void Update()
    {
      this.delay -= Engine.DeltaTime;
      this.X = this.SceneAs<Level>().Camera.X;
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      base.Update();
      this.Visible = true;
      if (this.waiting)
      {
        this.loopSfx.Param("rising", 0.0f);
        if (!this.intro && entity != null && entity.JustRespawned)
          this.Y = Calc.Approach(this.Y, entity.Y + 32f, 32f * Engine.DeltaTime);
        if ((!this.iceMode || !this.intro) && (entity == null || !entity.JustRespawned))
          this.waiting = false;
      }
      else
      {
        float num1 = this.SceneAs<Level>().Camera.Bottom - 12f;
        if ((double) this.Top > (double) num1 + 96.0)
          this.Top = num1 + 96f;
        float num2 = (double) this.Top <= (double) num1 ? Calc.ClampedMap(num1 - this.Top, 0.0f, 32f, 1f, 0.5f) : Calc.ClampedMap(this.Top - num1, 0.0f, 96f, 1f, 2f);
        if ((double) this.delay <= 0.0)
        {
          this.loopSfx.Param("rising", 1f);
          this.Y += -30f * num2 * Engine.DeltaTime;
        }
      }
      this.lerp = Calc.Approach(this.lerp, this.iceMode ? 1f : 0.0f, Engine.DeltaTime * 4f);
      this.bottomRect.SurfaceColor = Color.Lerp(RisingLava.Hot[0], RisingLava.Cold[0], this.lerp);
      this.bottomRect.EdgeColor = Color.Lerp(RisingLava.Hot[1], RisingLava.Cold[1], this.lerp);
      this.bottomRect.CenterColor = Color.Lerp(RisingLava.Hot[2], RisingLava.Cold[2], this.lerp);
      this.bottomRect.Spikey = this.lerp * 5f;
      this.bottomRect.UpdateMultiplier = (float) ((1.0 - (double) this.lerp) * 2.0);
      this.bottomRect.Fade = this.iceMode ? 128f : 32f;
    }
  }
}

