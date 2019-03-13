// Decompiled with JetBrains decompiler
// Type: Celeste.Payphone
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  [Tracked(false)]
  public class Payphone : Entity
  {
    private float lightFlickerTimer = 0.0f;
    private float lightFlickerFor = 0.1f;
    public static ParticleType P_Snow;
    public static ParticleType P_SnowB;
    public bool Broken;
    public Sprite Sprite;
    public Monocle.Image Blink;
    private VertexLight light;
    private BloomPoint bloom;
    private int lastFrame;
    private SoundSource buzzSfx;

    public Payphone(Vector2 pos)
      : base(pos)
    {
      this.Depth = 1;
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("payphone")));
      this.Sprite.Play("idle", false, false);
      this.Add((Component) (this.Blink = new Monocle.Image(GFX.Game["cutscenes/payphone/blink"])));
      this.Blink.Origin = this.Sprite.Origin;
      this.Blink.Visible = false;
      this.Add((Component) (this.light = new VertexLight(new Vector2(-6f, -45f), Color.White, 1f, 8, 96)));
      this.light.Spotlight = true;
      this.light.SpotlightDirection = new Vector2(0.0f, 1f).Angle();
      this.Add((Component) (this.bloom = new BloomPoint(new Vector2(-6f, -45f), 0.8f, 8f)));
      this.Add((Component) (this.buzzSfx = new SoundSource()));
      this.buzzSfx.Play("event:/env/local/02_old_site/phone_lamp", (string) null, 0.0f);
      this.buzzSfx.Param("on", 1f);
    }

    public override void Update()
    {
      base.Update();
      if (!this.Broken)
      {
        this.lightFlickerTimer -= Engine.DeltaTime;
        if ((double) this.lightFlickerTimer <= 0.0)
        {
          if (this.Scene.OnInterval(0.025f))
          {
            bool flag = (double) Calc.Random.NextFloat() > 0.5;
            this.light.Visible = flag;
            this.bloom.Visible = flag;
            this.Blink.Visible = !flag;
            this.buzzSfx.Param("on", flag ? 1f : 0.0f);
          }
          if ((double) this.lightFlickerTimer < -(double) this.lightFlickerFor)
          {
            this.lightFlickerTimer = Calc.Random.Choose<float>(0.4f, 0.6f, 0.8f, 1f);
            this.lightFlickerFor = Calc.Random.Choose<float>(0.1f, 0.2f, 0.05f);
            this.light.Visible = true;
            this.bloom.Visible = true;
            this.Blink.Visible = false;
            this.buzzSfx.Param("on", 1f);
          }
        }
      }
      else
      {
        this.Blink.Visible = this.bloom.Visible = this.light.Visible = false;
        this.buzzSfx.Param("on", 0.0f);
      }
      if (this.Sprite.CurrentAnimationID == "eat" && this.Sprite.CurrentAnimationFrame == 5 && this.lastFrame != this.Sprite.CurrentAnimationFrame)
      {
        Level level = this.SceneAs<Level>();
        level.ParticlesFG.Emit(Payphone.P_Snow, 10, level.Camera.Position + new Vector2(236f, 152f), new Vector2(10f, 0.0f));
        level.ParticlesFG.Emit(Payphone.P_SnowB, 8, level.Camera.Position + new Vector2(236f, 152f), new Vector2(6f, 0.0f));
        level.DirectionalShake(Vector2.UnitY, 0.3f);
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      }
      if (this.Sprite.CurrentAnimationID == "eat" && this.Sprite.CurrentAnimationFrame == this.Sprite.CurrentAnimationTotalFrames - 5 && this.lastFrame != this.Sprite.CurrentAnimationFrame)
        Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      this.lastFrame = this.Sprite.CurrentAnimationFrame;
    }
  }
}

