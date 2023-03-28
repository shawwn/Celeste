// Decompiled with JetBrains decompiler
// Type: Celeste.LightningBreakerBox
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class LightningBreakerBox : Solid
  {
    public static ParticleType P_Smash;
    public static ParticleType P_Sparks;
    private Sprite sprite;
    private SineWave sine;
    private Vector2 start;
    private float sink;
    private int health = 2;
    private bool flag;
    private float shakeCounter;
    private string music;
    private int musicProgress = -1;
    private bool musicStoreInSession;
    private Vector2 bounceDir;
    private Wiggler bounce;
    private Shaker shaker;
    private bool makeSparks;
    private bool smashParticles;
    private Coroutine pulseRoutine;
    private SoundSource firstHitSfx;
    private bool spikesLeft;
    private bool spikesRight;
    private bool spikesUp;
    private bool spikesDown;

    public LightningBreakerBox(Vector2 position, bool flipX)
      : base(position, 32f, 32f, true)
    {
      this.SurfaceSoundIndex = 9;
      this.start = this.Position;
      this.sprite = GFX.SpriteBank.Create("breakerBox");
      this.sprite.OnLastFrame += (Action<string>) (anim =>
      {
        switch (anim)
        {
          case "break":
            this.Visible = false;
            break;
          case "open":
            this.makeSparks = true;
            break;
        }
      });
      this.sprite.Position = new Vector2(this.Width, this.Height) / 2f;
      this.sprite.FlipX = flipX;
      this.Add((Component) this.sprite);
      this.sine = new SineWave(0.5f);
      this.Add((Component) this.sine);
      this.bounce = Wiggler.Create(1f, 0.5f);
      this.bounce.StartZero = false;
      this.Add((Component) this.bounce);
      this.Add((Component) (this.shaker = new Shaker(false)));
      this.OnDashCollide = new DashCollision(this.Dashed);
    }

    public LightningBreakerBox(EntityData e, Vector2 levelOffset)
      : this(e.Position + levelOffset, e.Bool("flipX"))
    {
      this.flag = e.Bool(nameof (flag));
      this.music = e.Attr(nameof (music), (string) null);
      this.musicProgress = e.Int("music_progress", -1);
      this.musicStoreInSession = e.Bool("music_session");
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      this.spikesUp = this.CollideCheck<Spikes>(this.Position - Vector2.UnitY);
      this.spikesDown = this.CollideCheck<Spikes>(this.Position + Vector2.UnitY);
      this.spikesLeft = this.CollideCheck<Spikes>(this.Position - Vector2.UnitX);
      this.spikesRight = this.CollideCheck<Spikes>(this.Position + Vector2.UnitX);
    }

    public DashCollisionResults Dashed(Player player, Vector2 dir)
    {
      if (!SaveData.Instance.Assists.Invincible && (dir == Vector2.UnitX && this.spikesLeft || dir == -Vector2.UnitX && this.spikesRight || dir == Vector2.UnitY && this.spikesUp || dir == -Vector2.UnitY && this.spikesDown))
        return DashCollisionResults.NormalCollision;
      (this.Scene as Level).DirectionalShake(dir);
      this.sprite.Scale = new Vector2((float) (1.0 + (double) Math.Abs(dir.Y) * 0.4000000059604645 - (double) Math.Abs(dir.X) * 0.4000000059604645), (float) (1.0 + (double) Math.Abs(dir.X) * 0.4000000059604645 - (double) Math.Abs(dir.Y) * 0.4000000059604645));
      --this.health;
      if (this.health > 0)
      {
        this.Add((Component) (this.firstHitSfx = new SoundSource("event:/new_content/game/10_farewell/fusebox_hit_1")));
        Celeste.Freeze(0.1f);
        this.shakeCounter = 0.2f;
        this.shaker.On = true;
        this.bounceDir = dir;
        this.bounce.Start();
        this.smashParticles = true;
        this.Pulse();
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      }
      else
      {
        if (this.firstHitSfx != null)
          this.firstHitSfx.Stop();
        Audio.Play("event:/new_content/game/10_farewell/fusebox_hit_2", this.Position);
        Celeste.Freeze(0.2f);
        player.RefillDash();
        this.Break();
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
        this.SmashParticles(dir.Perpendicular());
        this.SmashParticles(-dir.Perpendicular());
      }
      return DashCollisionResults.Rebound;
    }

    private void SmashParticles(Vector2 dir)
    {
      float direction;
      Vector2 position;
      Vector2 positionRange;
      int num;
      if (dir == Vector2.UnitX)
      {
        direction = 0.0f;
        position = this.CenterRight - Vector2.UnitX * 12f;
        positionRange = Vector2.UnitY * (this.Height - 6f) * 0.5f;
        num = (int) ((double) this.Height / 8.0) * 4;
      }
      else if (dir == -Vector2.UnitX)
      {
        direction = 3.1415927f;
        position = this.CenterLeft + Vector2.UnitX * 12f;
        positionRange = Vector2.UnitY * (this.Height - 6f) * 0.5f;
        num = (int) ((double) this.Height / 8.0) * 4;
      }
      else if (dir == Vector2.UnitY)
      {
        direction = 1.5707964f;
        position = this.BottomCenter - Vector2.UnitY * 12f;
        positionRange = Vector2.UnitX * (this.Width - 6f) * 0.5f;
        num = (int) ((double) this.Width / 8.0) * 4;
      }
      else
      {
        direction = -1.5707964f;
        position = this.TopCenter + Vector2.UnitY * 12f;
        positionRange = Vector2.UnitX * (this.Width - 6f) * 0.5f;
        num = (int) ((double) this.Width / 8.0) * 4;
      }
      int amount = num + 2;
      this.SceneAs<Level>().Particles.Emit(LightningBreakerBox.P_Smash, amount, position, positionRange, direction);
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if (!this.flag || !(this.Scene as Level).Session.GetFlag("disable_lightning"))
        return;
      this.RemoveSelf();
    }

    public override void Update()
    {
      base.Update();
      if (this.makeSparks && this.Scene.OnInterval(0.03f))
        this.SceneAs<Level>().ParticlesFG.Emit(LightningBreakerBox.P_Sparks, 1, this.Center, Vector2.One * 12f);
      if ((double) this.shakeCounter > 0.0)
      {
        this.shakeCounter -= Engine.DeltaTime;
        if ((double) this.shakeCounter <= 0.0)
        {
          this.shaker.On = false;
          this.sprite.Scale = Vector2.One * 1.2f;
          this.sprite.Play("open");
        }
      }
      if (this.Collidable)
      {
        this.sink = Calc.Approach(this.sink, this.HasPlayerRider() ? 1f : 0.0f, 2f * Engine.DeltaTime);
        this.sine.Rate = MathHelper.Lerp(1f, 0.5f, this.sink);
        Vector2 start = this.start;
        start.Y += (float) ((double) this.sink * 6.0 + (double) this.sine.Value * (double) MathHelper.Lerp(4f, 2f, this.sink));
        Vector2 vector2 = start + this.bounce.Value * this.bounceDir * 12f;
        this.MoveToX(vector2.X);
        this.MoveToY(vector2.Y);
        if (this.smashParticles)
        {
          this.smashParticles = false;
          this.SmashParticles(this.bounceDir.Perpendicular());
          this.SmashParticles(-this.bounceDir.Perpendicular());
        }
      }
      this.sprite.Scale.X = Calc.Approach(this.sprite.Scale.X, 1f, Engine.DeltaTime * 4f);
      this.sprite.Scale.Y = Calc.Approach(this.sprite.Scale.Y, 1f, Engine.DeltaTime * 4f);
      this.LiftSpeed = Vector2.Zero;
    }

    public override void Render()
    {
      Vector2 position = this.sprite.Position;
      Sprite sprite = this.sprite;
      sprite.Position = sprite.Position + this.shaker.Value;
      base.Render();
      this.sprite.Position = position;
    }

    private void Pulse()
    {
      this.pulseRoutine = new Coroutine(Lightning.PulseRoutine(this.SceneAs<Level>()));
      this.Add((Component) this.pulseRoutine);
    }

    private void Break()
    {
      Session session = (this.Scene as Level).Session;
      RumbleTrigger.ManuallyTrigger(this.Center.X, 1.2f);
      this.Tag = (int) Tags.Persistent;
      this.shakeCounter = 0.0f;
      this.shaker.On = false;
      this.sprite.Play("break");
      this.Collidable = false;
      this.DestroyStaticMovers();
      if (this.flag)
        session.SetFlag("disable_lightning");
      if (this.musicStoreInSession)
      {
        if (!string.IsNullOrEmpty(this.music))
          session.Audio.Music.Event = SFX.EventnameByHandle(this.music);
        if (this.musicProgress >= 0)
          session.Audio.Music.SetProgress(this.musicProgress);
        session.Audio.Apply();
      }
      else
      {
        if (!string.IsNullOrEmpty(this.music))
          Audio.SetMusic(SFX.EventnameByHandle(this.music), false);
        if (this.musicProgress >= 0)
          Audio.SetMusicParam("progress", (float) this.musicProgress);
        if (!string.IsNullOrEmpty(this.music) && (HandleBase) Audio.CurrentMusicEventInstance != (HandleBase) null)
        {
          int num = (int) Audio.CurrentMusicEventInstance.start();
        }
      }
      if (this.pulseRoutine != null)
        this.pulseRoutine.Active = false;
      this.Add((Component) new Coroutine(Lightning.RemoveRoutine(this.SceneAs<Level>(), new Action(((Entity) this).RemoveSelf))));
    }
  }
}
