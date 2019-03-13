// Decompiled with JetBrains decompiler
// Type: Celeste.BreathingMinigame
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class BreathingMinigame : Entity
  {
    public bool Completed = false;
    public bool Pausing = false;
    private float boxAlpha = 0.0f;
    private float featherAlpha = 0.0f;
    private float bgAlpha = 0.0f;
    private float feather = 0.0f;
    private float speed = 0.0f;
    private float stablizedTimer = 0.0f;
    private float currentTargetBounds = 160f;
    private float currentTargetCenter = 0.0f;
    private float speedMultiplier = 1f;
    private float insideTargetTimer = 0.0f;
    private bool boxEnabled = false;
    private float trailSpeed = 0.0f;
    private float textAlpha = 0.0f;
    private float timer = 0.0f;
    private MTexture particleTexture = GFX.Overworld["snow"].GetSubtexture(1, 1, 254, 254, (MTexture) null);
    private float particleAlpha = 0.0f;
    private const float StablizeDuration = 30f;
    private const float StablizeLossRate = 0.5f;
    private const float StablizeIncreaseDelay = 0.2f;
    private const float StablizeLossPenalty = 0.5f;
    private const float Acceleration = 280f;
    private const float Gravity = 280f;
    private const float Maxspeed = 200f;
    private const float Bounds = 450f;
    private const float BGFadeStart = 0.65f;
    private const float featherSpriteOffset = -128f;
    private const float FadeBoxInMargin = 300f;
    private const float TargetSineAmplitude = 300f;
    private const float TargetSineFreq = 0.25f;
    private const float TargetBoundsAtStart = 160f;
    private const float TargetBoundsAtEnd = 100f;
    public const float MaxRumble = 0.5f;
    private const float PercentBeforeStartLosing = 0.4f;
    private const float LoseDuration = 5f;
    private bool winnable;
    private bool losing;
    private float losingTimer;
    private Sprite featherSprite;
    private Monocle.Image featherSlice;
    private Monocle.Image featherHalfLeft;
    private Monocle.Image featherHalfRight;
    private SineWave sine;
    private SineWave featherWave;
    private BreathingRumbler rumbler;
    private string text;
    private VirtualRenderTarget featherBuffer;
    private VirtualRenderTarget smokeBuffer;
    private VirtualRenderTarget tempBuffer;
    private BreathingMinigame.Particle[] particles;
    private float particleSpeed;

    private Vector2 screenCenter
    {
      get
      {
        return new Vector2(1920f, 1080f) / 2f;
      }
    }

    public BreathingMinigame(bool winnable = true, BreathingRumbler rumbler = null)
    {
      this.rumbler = rumbler;
      this.winnable = winnable;
      this.Tag = (int) Tags.HUD;
      this.Depth = 100;
      this.Add((Component) (this.featherSprite = GFX.GuiSpriteBank.Create(nameof (feather))));
      this.featherSprite.Position = this.screenCenter + Vector2.UnitY * (this.feather - 128f);
      this.Add((Component) new Coroutine(this.Routine(), true));
      this.Add((Component) (this.featherWave = new SineWave(0.25f)));
      this.Add((Component) new BeforeRenderHook(new Action(this.BeforeRender)));
      this.particles = new BreathingMinigame.Particle[50];
      for (int index = 0; index < this.particles.Length; ++index)
        this.particles[index].Reset();
      this.particleSpeed = 120f;
    }

    public IEnumerator Routine()
    {
      this.insideTargetTimer = 1f;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        yield return (object) null;
        if ((double) p > 1.0)
          p = 1f;
        this.bgAlpha = p * 0.65f;
      }
      if (this.winnable)
      {
        yield return (object) this.ShowText(1);
        yield return (object) this.FadeGameIn();
        yield return (object) this.ShowText(2);
        yield return (object) this.ShowText(3);
        yield return (object) this.ShowText(4);
        yield return (object) this.ShowText(5);
      }
      else
        yield return (object) this.FadeGameIn();
      this.Add((Component) new Coroutine(this.FadeBoxIn(), true));
      float activeBounds = 450f;
      while ((double) this.stablizedTimer < 30.0)
      {
        float percent = this.stablizedTimer / 30f;
        bool pullUp = Input.Jump.Check || Input.Dash.Check || (double) Input.Aim.Value.Y < 0.0;
        if (this.winnable)
        {
          Audio.SetMusicParam("calm", percent);
          Audio.SetMusicParam("gondola_idle", percent);
        }
        else
        {
          Level level = this.Scene as Level;
          if (!this.losing)
          {
            float percentToLosing = percent / 0.4f;
            level.Session.Audio.Music.Layer(1, percentToLosing);
            level.Session.Audio.Music.Layer(3, 1f - percentToLosing);
            level.Session.Audio.Apply();
          }
          else
          {
            level.Session.Audio.Music.Layer(1, 1f - this.losingTimer);
            level.Session.Audio.Music.Layer(3, this.losingTimer);
            level.Session.Audio.Apply();
          }
          level = (Level) null;
        }
        if (!this.winnable && this.losing)
        {
          if (Calc.BetweenInterval(this.losingTimer * 10f, 0.5f))
            pullUp = !pullUp;
          activeBounds = (float) (450.0 - (double) Ease.CubeIn(this.losingTimer) * 200.0);
        }
        if (pullUp)
        {
          if ((double) this.feather > -(double) activeBounds)
            this.speed -= 280f * Engine.DeltaTime;
          this.particleSpeed -= 2800f * Engine.DeltaTime;
        }
        else
        {
          if ((double) this.feather < (double) activeBounds)
            this.speed += 280f * Engine.DeltaTime;
          this.particleSpeed += 2800f * Engine.DeltaTime;
        }
        this.speed = Calc.Clamp(this.speed, -200f, 200f);
        if ((double) this.feather > (double) activeBounds && (double) this.speedMultiplier == 0.0 && (double) this.speed > 0.0)
          this.speed = 0.0f;
        if ((double) this.feather < (double) activeBounds && (double) this.speedMultiplier == 0.0 && (double) this.speed < 0.0)
          this.speed = 0.0f;
        this.particleSpeed = Calc.Clamp(this.particleSpeed, -1600f, 120f);
        this.speedMultiplier = Calc.Approach(this.speedMultiplier, (double) this.feather < -(double) activeBounds && (double) this.speed < 0.0 || (double) this.feather > (double) activeBounds && (double) this.speed > 0.0 ? 0.0f : 1f, Engine.DeltaTime * 4f);
        this.currentTargetBounds = Calc.Approach(this.currentTargetBounds, (float) (160.0 + -60.0 * (double) percent), Engine.DeltaTime * 16f);
        this.feather += this.speed * this.speedMultiplier * Engine.DeltaTime;
        if (this.boxEnabled)
        {
          this.currentTargetCenter = (float) (-(double) this.sine.Value * 300.0) * MathHelper.Lerp(1f, 0.0f, Ease.CubeIn(percent));
          float top = this.currentTargetCenter - this.currentTargetBounds;
          float bottom = this.currentTargetCenter + this.currentTargetBounds;
          if ((double) this.feather > (double) top && (double) this.feather < (double) bottom)
          {
            this.insideTargetTimer += Engine.DeltaTime;
            if ((double) this.insideTargetTimer > 0.200000002980232)
              this.stablizedTimer += Engine.DeltaTime;
            if (this.rumbler != null)
              this.rumbler.Strength = (float) (0.300000011920929 * (1.0 - (double) percent));
          }
          else
          {
            if ((double) this.insideTargetTimer > 0.200000002980232)
              this.stablizedTimer = Math.Max(0.0f, this.stablizedTimer - 0.5f);
            if ((double) this.stablizedTimer > 0.0)
              this.stablizedTimer -= 0.5f * Engine.DeltaTime;
            this.insideTargetTimer = 0.0f;
            if (this.rumbler != null)
              this.rumbler.Strength = (float) (0.5 * (1.0 - (double) percent));
          }
        }
        else if (this.rumbler != null)
          this.rumbler.Strength = 0.2f;
        float fadeTarget = (float) (0.649999976158142 + (double) Math.Min(1f, percent / 0.8f) * 0.350000023841858);
        this.bgAlpha = Calc.Approach(this.bgAlpha, fadeTarget, Engine.DeltaTime);
        this.featherSprite.Position = this.screenCenter + Vector2.UnitY * (this.feather - 128f);
        this.featherSprite.Play((double) this.insideTargetTimer > 0.0 || !this.boxEnabled ? "hover" : "flutter", false, false);
        this.particleAlpha = Calc.Approach(this.particleAlpha, 1f, Engine.DeltaTime);
        if (!this.winnable && (double) this.stablizedTimer > 12.0)
          this.losing = true;
        if (this.losing)
        {
          this.losingTimer += Engine.DeltaTime / 5f;
          if ((double) this.losingTimer > 1.0)
            break;
        }
        yield return (object) null;
      }
      if (!this.winnable)
      {
        this.Pausing = true;
        while (this.Pausing)
        {
          if (this.rumbler != null)
            this.rumbler.Strength = Calc.Approach(this.rumbler.Strength, 1f, 2f * Engine.DeltaTime);
          Sprite featherSprite = this.featherSprite;
          featherSprite.Position = featherSprite.Position + (this.screenCenter - this.featherSprite.Position) * (1f - (float) Math.Pow(0.00999999977648258, (double) Engine.DeltaTime));
          this.boxAlpha -= Engine.DeltaTime * 10f;
          this.particleAlpha = this.boxAlpha;
          yield return (object) null;
        }
        this.losing = false;
        this.losingTimer = 0.0f;
        yield return (object) this.PopFeather();
      }
      else
      {
        this.bgAlpha = 1f;
        if (this.rumbler != null)
        {
          this.rumbler.RemoveSelf();
          this.rumbler = (BreathingRumbler) null;
        }
        while ((double) this.boxAlpha > 0.0)
        {
          yield return (object) null;
          this.boxAlpha -= Engine.DeltaTime;
          this.particleAlpha = this.boxAlpha;
        }
        this.particleAlpha = 0.0f;
        yield return (object) 2f;
        for (; (double) this.featherAlpha > 0.0; this.featherAlpha -= Engine.DeltaTime)
          yield return (object) null;
        yield return (object) 1f;
      }
      this.Completed = true;
      for (; (double) this.bgAlpha > 0.0; this.bgAlpha -= Engine.DeltaTime * (this.winnable ? 1f : 10f))
        yield return (object) null;
      this.RemoveSelf();
    }

    private IEnumerator ShowText(int num)
    {
      yield return (object) this.FadeTextTo(0.0f);
      this.text = Dialog.Clean("CH4_GONDOLA_FEATHER_" + (object) num, (Language) null);
      yield return (object) 0.1f;
      yield return (object) this.FadeTextTo(1f);
      while (!Input.MenuConfirm.Pressed)
        yield return (object) null;
      yield return (object) this.FadeTextTo(0.0f);
    }

    private IEnumerator FadeGameIn()
    {
      while ((double) this.featherAlpha < 1.0)
      {
        this.featherAlpha += Engine.DeltaTime;
        yield return (object) null;
      }
      this.featherAlpha = 1f;
    }

    private IEnumerator FadeBoxIn()
    {
      yield return (object) (float) (this.winnable ? 5.0 : 2.0);
      while ((double) Math.Abs(this.feather) > 300.0)
        yield return (object) null;
      this.boxEnabled = true;
      this.Add((Component) (this.sine = new SineWave(0.12f)));
      while ((double) this.boxAlpha < 1.0)
      {
        this.boxAlpha += Engine.DeltaTime;
        yield return (object) null;
      }
      this.boxAlpha = 1f;
    }

    private IEnumerator FadeTextTo(float v)
    {
      if ((double) this.textAlpha != (double) v)
      {
        float from = this.textAlpha;
        for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
        {
          yield return (object) null;
          this.textAlpha = from + (v - from) * p;
        }
        this.textAlpha = v;
      }
    }

    private IEnumerator PopFeather()
    {
      Audio.Play("event:/game/06_reflection/badeline_feather_slice");
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      if (this.rumbler != null)
      {
        this.rumbler.RemoveSelf();
        this.rumbler = (BreathingRumbler) null;
      }
      this.featherSprite.Rotation = 0.0f;
      this.featherSprite.Play("hover", false, false);
      this.featherSprite.CenterOrigin();
      this.featherSprite.Y += this.featherSprite.Height / 2f;
      yield return (object) 0.25f;
      this.featherSlice = new Monocle.Image(GFX.Gui["feather/slice"]);
      this.featherSlice.CenterOrigin();
      this.featherSlice.Position = this.featherSprite.Position;
      this.featherSlice.Rotation = Calc.Angle(new Vector2(96f, 165f), new Vector2(140f, 112f));
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 8f)
      {
        this.featherSlice.Scale.X = (float) ((0.25 + (double) Calc.YoYo(p) * 0.75) * 8.0);
        this.featherSlice.Scale.Y = (float) ((0.5 + (1.0 - (double) Calc.YoYo(p)) * 0.5) * 8.0);
        this.featherSlice.Position = this.featherSprite.Position + Vector2.Lerp(new Vector2(128f, (float) sbyte.MinValue), new Vector2((float) sbyte.MinValue, 128f), p);
        yield return (object) null;
      }
      this.featherSlice.Visible = false;
      (this.Scene as Level).Shake(0.3f);
      (this.Scene as Level).Flash(Color.White, false);
      this.featherSprite.Visible = false;
      this.featherHalfLeft = new Monocle.Image(GFX.Gui["feather/feather_half0"]);
      this.featherHalfLeft.CenterOrigin();
      this.featherHalfRight = new Monocle.Image(GFX.Gui["feather/feather_half1"]);
      this.featherHalfRight.CenterOrigin();
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        this.featherHalfLeft.Position = this.featherSprite.Position + Vector2.Lerp(Vector2.Zero, new Vector2((float) sbyte.MinValue, -32f), p);
        this.featherHalfRight.Position = this.featherSprite.Position + Vector2.Lerp(Vector2.Zero, new Vector2(128f, 32f), p);
        this.featherAlpha = 1f - p;
        yield return (object) null;
      }
    }

    public override void Update()
    {
      this.timer += Engine.DeltaTime;
      this.trailSpeed = Calc.Approach(this.trailSpeed, this.speed, (float) ((double) Engine.DeltaTime * 200.0 * 8.0));
      if (this.featherWave != null)
        this.featherSprite.Rotation = (float) ((double) this.featherWave.Value * 0.25 + 0.100000001490116);
      for (int index = 0; index < this.particles.Length; ++index)
      {
        this.particles[index].Position.Y += this.particles[index].Speed * this.particleSpeed * Engine.DeltaTime;
        if ((double) this.particleSpeed > -400.0)
          this.particles[index].Position.X += (float) (((double) this.particleSpeed + 400.0) * Math.Sin((double) this.particles[index].Sin) * 0.100000001490116) * Engine.DeltaTime;
        this.particles[index].Sin += Engine.DeltaTime;
        if ((double) this.particles[index].Position.Y < (double) sbyte.MinValue || (double) this.particles[index].Position.Y > 1208.0)
        {
          this.particles[index].Reset();
          this.particles[index].Position.Y = (double) this.particleSpeed >= 0.0 ? (float) sbyte.MinValue : 1208f;
        }
      }
      base.Update();
    }

    public void BeforeRender()
    {
      if (this.featherBuffer == null)
      {
        int width = Math.Min(1920, Engine.ViewWidth);
        int height = Math.Min(1080, Engine.ViewHeight);
        this.featherBuffer = VirtualContent.CreateRenderTarget("breathing-minigame-a", width, height, false, true, 0);
        this.smokeBuffer = VirtualContent.CreateRenderTarget("breathing-minigame-b", width / 2, height / 2, false, true, 0);
        this.tempBuffer = VirtualContent.CreateRenderTarget("breathing-minigame-c", width / 2, height / 2, false, true, 0);
      }
      Engine.Graphics.GraphicsDevice.SetRenderTarget(this.featherBuffer.Target);
      Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
      Matrix scale = Matrix.CreateScale((float) this.featherBuffer.Width / 1920f);
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, scale);
      if (this.losing)
      {
        Sprite featherSprite = this.featherSprite;
        featherSprite.Position = featherSprite.Position + new Vector2((float) Calc.Random.Range(-1, 1), (float) Calc.Random.Range(-1, 1)).SafeNormalize() * this.losingTimer * 10f;
        this.featherSprite.Rotation += (float) ((double) Calc.Random.Range(-1, 1) * (double) this.losingTimer * 0.100000001490116);
      }
      this.featherSprite.Color = Color.White * this.featherAlpha;
      if (this.featherSprite.Visible)
        this.featherSprite.Render();
      if (this.featherSlice != null && this.featherSlice.Visible)
        this.featherSlice.Render();
      if (this.featherHalfLeft != null && this.featherHalfLeft.Visible)
      {
        this.featherHalfLeft.Color = Color.White * this.featherAlpha;
        this.featherHalfRight.Color = Color.White * this.featherAlpha;
        this.featherHalfLeft.Render();
        this.featherHalfRight.Render();
      }
      Draw.SpriteBatch.End();
      Engine.Graphics.GraphicsDevice.SetRenderTarget(this.smokeBuffer.Target);
      Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
      MagicGlow.Render((Texture2D) this.featherBuffer.Target, this.timer, (float) (-(double) this.trailSpeed / 200.0 * 2.0), Matrix.CreateScale(0.5f));
      GaussianBlur.Blur((Texture2D) this.smokeBuffer.Target, this.tempBuffer, this.smokeBuffer, 0.0f, true, GaussianBlur.Samples.Nine, 1f, GaussianBlur.Direction.Both, 1f);
    }

    public override void Render()
    {
      Color color = (double) this.insideTargetTimer > 0.200000002980232 ? Color.White : Color.White * 0.6f;
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * this.bgAlpha);
      Level scene = this.Scene as Level;
      if (scene != null && (scene.FrozenOrPaused || scene.RetryPlayerCorpse != null || scene.SkippingCutscene))
        return;
      MTexture mtexture1 = GFX.Gui["feather/border"];
      MTexture mtexture2 = GFX.Gui["feather/box"];
      Vector2 scale1 = new Vector2((float) ((double) mtexture1.Width * 2.0 - 32.0) / (float) mtexture2.Width, (float) ((double) this.currentTargetBounds * 2.0 - 32.0) / (float) mtexture2.Height);
      mtexture2.DrawCentered(this.screenCenter + new Vector2(0.0f, this.currentTargetCenter), Color.White * this.boxAlpha * 0.25f, scale1);
      mtexture1.Draw(this.screenCenter + new Vector2((float) -mtexture1.Width, this.currentTargetCenter - this.currentTargetBounds), Vector2.Zero, color * this.boxAlpha, Vector2.One);
      mtexture1.Draw(this.screenCenter + new Vector2((float) mtexture1.Width, this.currentTargetCenter + this.currentTargetBounds), Vector2.Zero, color * this.boxAlpha, new Vector2(-1f, -1f));
      if (this.featherBuffer != null && !this.featherBuffer.IsDisposed)
      {
        float scale2 = 1920f / (float) this.featherBuffer.Width;
        Draw.SpriteBatch.Draw((Texture2D) this.smokeBuffer.Target, Vector2.Zero, new Rectangle?(this.smokeBuffer.Bounds), Color.White * 0.3f, 0.0f, Vector2.Zero, scale2 * 2f, SpriteEffects.None, 0.0f);
        Draw.SpriteBatch.Draw((Texture2D) this.featherBuffer.Target, Vector2.Zero, new Rectangle?(this.featherBuffer.Bounds), Color.White, 0.0f, Vector2.Zero, scale2, SpriteEffects.None, 0.0f);
      }
      Vector2 vector2_1 = new Vector2(1f, 1f);
      if ((double) this.particleSpeed < 0.0)
        vector2_1 = new Vector2(Math.Min(1f, (float) (1.0 / (-(double) this.particleSpeed * 0.00400000018998981))), Math.Max(1f, (float) (1.0 * -(double) this.particleSpeed * 0.00400000018998981)));
      for (int index = 0; index < this.particles.Length; ++index)
        this.particleTexture.DrawCentered(this.particles[index].Position, Color.White * (0.5f * this.particleAlpha), this.particles[index].Scale * vector2_1);
      if (!string.IsNullOrEmpty(this.text) && (double) this.textAlpha > 0.0)
        ActiveFont.Draw(this.text, new Vector2(960f, 920f), new Vector2(0.5f, 0.5f), Vector2.One, Color.White * this.textAlpha);
      if (string.IsNullOrEmpty(this.text) || (double) this.textAlpha < 1.0)
        return;
      Vector2 vector2_2 = ActiveFont.Measure(this.text);
      Vector2 position = new Vector2((float) ((1920.0 + (double) vector2_2.X) / 2.0 + 40.0), (float) (920.0 + (double) vector2_2.Y / 2.0 - 16.0)) + new Vector2(0.0f, (double) this.timer % 1.0 < 0.25 ? 6f : 0.0f);
      GFX.Gui["textboxbutton"].DrawCentered(position);
    }

    public override void Removed(Scene scene)
    {
      this.Dispose();
      base.Removed(scene);
    }

    public override void SceneEnd(Scene scene)
    {
      this.Dispose();
      base.SceneEnd(scene);
    }

    private void Dispose()
    {
      if (this.featherBuffer == null || this.featherBuffer.IsDisposed)
        return;
      this.featherBuffer.Dispose();
      this.featherBuffer = (VirtualRenderTarget) null;
      this.smokeBuffer.Dispose();
      this.smokeBuffer = (VirtualRenderTarget) null;
      this.tempBuffer.Dispose();
      this.tempBuffer = (VirtualRenderTarget) null;
    }

    private struct Particle
    {
      public Vector2 Position;
      public float Speed;
      public float Scale;
      public float Sin;

      public void Reset()
      {
        float num = Calc.Random.NextFloat();
        float val = num * (num * num * num);
        this.Position = new Vector2(Calc.Random.NextFloat() * 1920f, Calc.Random.NextFloat() * 1080f);
        this.Scale = Calc.Map(val, 0.0f, 1f, 0.05f, 0.8f);
        this.Speed = this.Scale * Calc.Random.Range(2f, 8f);
        this.Sin = Calc.Random.NextFloat(6.283185f);
      }
    }
  }
}

