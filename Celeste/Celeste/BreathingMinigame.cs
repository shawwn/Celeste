// Decompiled with JetBrains decompiler
// Type: Celeste.BreathingMinigame
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class BreathingMinigame : Entity
  {
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
    public bool Completed;
    public bool Pausing;
    private bool winnable;
    private float boxAlpha;
    private float featherAlpha;
    private float bgAlpha;
    private float feather;
    private float speed;
    private float stablizedTimer;
    private float currentTargetBounds = 160f;
    private float currentTargetCenter;
    private float speedMultiplier = 1f;
    private float insideTargetTimer;
    private bool boxEnabled;
    private float trailSpeed;
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
    private float textAlpha;
    private VirtualRenderTarget featherBuffer;
    private VirtualRenderTarget smokeBuffer;
    private VirtualRenderTarget tempBuffer;
    private float timer;
    private BreathingMinigame.Particle[] particles;
    private MTexture particleTexture = OVR.Atlas["snow"].GetSubtexture(1, 1, 254, 254);
    private float particleSpeed;
    private float particleAlpha;

    private Vector2 screenCenter => new Vector2(1920f, 1080f) / 2f;

    public BreathingMinigame(bool winnable = true, BreathingRumbler rumbler = null)
    {
      this.rumbler = rumbler;
      this.winnable = winnable;
      this.Tag = (int) Tags.HUD;
      this.Depth = 100;
      this.Add((Component) (this.featherSprite = GFX.GuiSpriteBank.Create(nameof (feather))));
      this.featherSprite.Position = this.screenCenter + Vector2.UnitY * (this.feather - 128f);
      this.Add((Component) new Coroutine(this.Routine()));
      this.Add((Component) (this.featherWave = new SineWave(0.25f)));
      this.Add((Component) new BeforeRenderHook(new Action(this.BeforeRender)));
      this.particles = new BreathingMinigame.Particle[50];
      for (int index = 0; index < this.particles.Length; ++index)
        this.particles[index].Reset();
      this.particleSpeed = 120f;
    }

    public IEnumerator Routine()
    {
      BreathingMinigame breathingMinigame = this;
      breathingMinigame.insideTargetTimer = 1f;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        yield return (object) null;
        if ((double) p > 1.0)
          p = 1f;
        breathingMinigame.bgAlpha = p * 0.65f;
      }
      if (breathingMinigame.winnable)
      {
        yield return (object) breathingMinigame.ShowText(1);
        yield return (object) breathingMinigame.FadeGameIn();
        yield return (object) breathingMinigame.ShowText(2);
        yield return (object) breathingMinigame.ShowText(3);
        yield return (object) breathingMinigame.ShowText(4);
        yield return (object) breathingMinigame.ShowText(5);
      }
      else
        yield return (object) breathingMinigame.FadeGameIn();
      breathingMinigame.Add((Component) new Coroutine(breathingMinigame.FadeBoxIn()));
      float activeBounds = 450f;
      while ((double) breathingMinigame.stablizedTimer < 30.0)
      {
        float t = breathingMinigame.stablizedTimer / 30f;
        bool flag = Input.Jump.Check || Input.Dash.Check || (double) Input.Aim.Value.Y < 0.0;
        if (breathingMinigame.winnable)
        {
          Audio.SetMusicParam("calm", t);
          Audio.SetMusicParam("gondola_idle", t);
        }
        else
        {
          Level scene = breathingMinigame.Scene as Level;
          if (!breathingMinigame.losing)
          {
            float num = t / 0.4f;
            scene.Session.Audio.Music.Layer(1, num);
            scene.Session.Audio.Music.Layer(3, 1f - num);
            scene.Session.Audio.Apply();
          }
          else
          {
            scene.Session.Audio.Music.Layer(1, 1f - breathingMinigame.losingTimer);
            scene.Session.Audio.Music.Layer(3, breathingMinigame.losingTimer);
            scene.Session.Audio.Apply();
          }
        }
        if (!breathingMinigame.winnable && breathingMinigame.losing)
        {
          if (Calc.BetweenInterval(breathingMinigame.losingTimer * 10f, 0.5f))
            flag = !flag;
          activeBounds = (float) (450.0 - (double) Ease.CubeIn(breathingMinigame.losingTimer) * 200.0);
        }
        if (flag)
        {
          if ((double) breathingMinigame.feather > -(double) activeBounds)
            breathingMinigame.speed -= 280f * Engine.DeltaTime;
          breathingMinigame.particleSpeed -= 2800f * Engine.DeltaTime;
        }
        else
        {
          if ((double) breathingMinigame.feather < (double) activeBounds)
            breathingMinigame.speed += 280f * Engine.DeltaTime;
          breathingMinigame.particleSpeed += 2800f * Engine.DeltaTime;
        }
        breathingMinigame.speed = Calc.Clamp(breathingMinigame.speed, -200f, 200f);
        if ((double) breathingMinigame.feather > (double) activeBounds && (double) breathingMinigame.speedMultiplier == 0.0 && (double) breathingMinigame.speed > 0.0)
          breathingMinigame.speed = 0.0f;
        if ((double) breathingMinigame.feather < (double) activeBounds && (double) breathingMinigame.speedMultiplier == 0.0 && (double) breathingMinigame.speed < 0.0)
          breathingMinigame.speed = 0.0f;
        breathingMinigame.particleSpeed = Calc.Clamp(breathingMinigame.particleSpeed, -1600f, 120f);
        breathingMinigame.speedMultiplier = Calc.Approach(breathingMinigame.speedMultiplier, (double) breathingMinigame.feather < -(double) activeBounds && (double) breathingMinigame.speed < 0.0 || (double) breathingMinigame.feather > (double) activeBounds && (double) breathingMinigame.speed > 0.0 ? 0.0f : 1f, Engine.DeltaTime * 4f);
        breathingMinigame.currentTargetBounds = Calc.Approach(breathingMinigame.currentTargetBounds, (float) (160.0 + -60.0 * (double) t), Engine.DeltaTime * 16f);
        breathingMinigame.feather += breathingMinigame.speed * breathingMinigame.speedMultiplier * Engine.DeltaTime;
        if (breathingMinigame.boxEnabled)
        {
          breathingMinigame.currentTargetCenter = (float) (-(double) breathingMinigame.sine.Value * 300.0) * MathHelper.Lerp(1f, 0.0f, Ease.CubeIn(t));
          float num1 = breathingMinigame.currentTargetCenter - breathingMinigame.currentTargetBounds;
          float num2 = breathingMinigame.currentTargetCenter + breathingMinigame.currentTargetBounds;
          if ((double) breathingMinigame.feather > (double) num1 && (double) breathingMinigame.feather < (double) num2)
          {
            breathingMinigame.insideTargetTimer += Engine.DeltaTime;
            if ((double) breathingMinigame.insideTargetTimer > 0.20000000298023224)
              breathingMinigame.stablizedTimer += Engine.DeltaTime;
            if (breathingMinigame.rumbler != null)
              breathingMinigame.rumbler.Strength = (float) (0.30000001192092896 * (1.0 - (double) t));
          }
          else
          {
            if ((double) breathingMinigame.insideTargetTimer > 0.20000000298023224)
              breathingMinigame.stablizedTimer = Math.Max(0.0f, breathingMinigame.stablizedTimer - 0.5f);
            if ((double) breathingMinigame.stablizedTimer > 0.0)
              breathingMinigame.stablizedTimer -= 0.5f * Engine.DeltaTime;
            breathingMinigame.insideTargetTimer = 0.0f;
            if (breathingMinigame.rumbler != null)
              breathingMinigame.rumbler.Strength = (float) (0.5 * (1.0 - (double) t));
          }
        }
        else if (breathingMinigame.rumbler != null)
          breathingMinigame.rumbler.Strength = 0.2f;
        float target = (float) (0.6499999761581421 + (double) Math.Min(1f, t / 0.8f) * 0.3500000238418579);
        breathingMinigame.bgAlpha = Calc.Approach(breathingMinigame.bgAlpha, target, Engine.DeltaTime);
        breathingMinigame.featherSprite.Position = breathingMinigame.screenCenter + Vector2.UnitY * (breathingMinigame.feather - 128f);
        breathingMinigame.featherSprite.Play((double) breathingMinigame.insideTargetTimer > 0.0 || !breathingMinigame.boxEnabled ? "hover" : "flutter");
        breathingMinigame.particleAlpha = Calc.Approach(breathingMinigame.particleAlpha, 1f, Engine.DeltaTime);
        if (!breathingMinigame.winnable && (double) breathingMinigame.stablizedTimer > 12.0)
          breathingMinigame.losing = true;
        if (breathingMinigame.losing)
        {
          breathingMinigame.losingTimer += Engine.DeltaTime / 5f;
          if ((double) breathingMinigame.losingTimer > 1.0)
            break;
        }
        yield return (object) null;
      }
      if (!breathingMinigame.winnable)
      {
        breathingMinigame.Pausing = true;
        while (breathingMinigame.Pausing)
        {
          if (breathingMinigame.rumbler != null)
            breathingMinigame.rumbler.Strength = Calc.Approach(breathingMinigame.rumbler.Strength, 1f, 2f * Engine.DeltaTime);
          Sprite featherSprite = breathingMinigame.featherSprite;
          featherSprite.Position = featherSprite.Position + (breathingMinigame.screenCenter - breathingMinigame.featherSprite.Position) * (1f - (float) Math.Pow(0.009999999776482582, (double) Engine.DeltaTime));
          breathingMinigame.boxAlpha -= Engine.DeltaTime * 10f;
          breathingMinigame.particleAlpha = breathingMinigame.boxAlpha;
          yield return (object) null;
        }
        breathingMinigame.losing = false;
        breathingMinigame.losingTimer = 0.0f;
        yield return (object) breathingMinigame.PopFeather();
      }
      else
      {
        breathingMinigame.bgAlpha = 1f;
        if (breathingMinigame.rumbler != null)
        {
          breathingMinigame.rumbler.RemoveSelf();
          breathingMinigame.rumbler = (BreathingRumbler) null;
        }
        while ((double) breathingMinigame.boxAlpha > 0.0)
        {
          yield return (object) null;
          breathingMinigame.boxAlpha -= Engine.DeltaTime;
          breathingMinigame.particleAlpha = breathingMinigame.boxAlpha;
        }
        breathingMinigame.particleAlpha = 0.0f;
        yield return (object) 2f;
        for (; (double) breathingMinigame.featherAlpha > 0.0; breathingMinigame.featherAlpha -= Engine.DeltaTime)
          yield return (object) null;
        yield return (object) 1f;
      }
      breathingMinigame.Completed = true;
      for (; (double) breathingMinigame.bgAlpha > 0.0; breathingMinigame.bgAlpha -= Engine.DeltaTime * (breathingMinigame.winnable ? 1f : 10f))
        yield return (object) null;
      breathingMinigame.RemoveSelf();
    }

    private IEnumerator ShowText(int num)
    {
      yield return (object) this.FadeTextTo(0.0f);
      this.text = Dialog.Clean("CH4_GONDOLA_FEATHER_" + (object) num);
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
      BreathingMinigame breathingMinigame = this;
      yield return (object) (float) (breathingMinigame.winnable ? 5.0 : 2.0);
      while ((double) Math.Abs(breathingMinigame.feather) > 300.0)
        yield return (object) null;
      breathingMinigame.boxEnabled = true;
      breathingMinigame.Add((Component) (breathingMinigame.sine = new SineWave(0.12f)));
      while ((double) breathingMinigame.boxAlpha < 1.0)
      {
        breathingMinigame.boxAlpha += Engine.DeltaTime;
        yield return (object) null;
      }
      breathingMinigame.boxAlpha = 1f;
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
      BreathingMinigame breathingMinigame = this;
      Audio.Play("event:/game/06_reflection/badeline_feather_slice");
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      if (breathingMinigame.rumbler != null)
      {
        breathingMinigame.rumbler.RemoveSelf();
        breathingMinigame.rumbler = (BreathingRumbler) null;
      }
      breathingMinigame.featherSprite.Rotation = 0.0f;
      breathingMinigame.featherSprite.Play("hover");
      breathingMinigame.featherSprite.CenterOrigin();
      breathingMinigame.featherSprite.Y += breathingMinigame.featherSprite.Height / 2f;
      yield return (object) 0.25f;
      breathingMinigame.featherSlice = new Monocle.Image(GFX.Gui["feather/slice"]);
      breathingMinigame.featherSlice.CenterOrigin();
      breathingMinigame.featherSlice.Position = breathingMinigame.featherSprite.Position;
      breathingMinigame.featherSlice.Rotation = Calc.Angle(new Vector2(96f, 165f), new Vector2(140f, 112f));
      float p;
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 8f)
      {
        breathingMinigame.featherSlice.Scale.X = (float) ((0.25 + (double) Calc.YoYo(p) * 0.75) * 8.0);
        breathingMinigame.featherSlice.Scale.Y = (float) ((0.5 + (1.0 - (double) Calc.YoYo(p)) * 0.5) * 8.0);
        breathingMinigame.featherSlice.Position = breathingMinigame.featherSprite.Position + Vector2.Lerp(new Vector2(128f, (float) sbyte.MinValue), new Vector2((float) sbyte.MinValue, 128f), p);
        yield return (object) null;
      }
      breathingMinigame.featherSlice.Visible = false;
      (breathingMinigame.Scene as Level).Shake();
      (breathingMinigame.Scene as Level).Flash(Color.White);
      breathingMinigame.featherSprite.Visible = false;
      breathingMinigame.featherHalfLeft = new Monocle.Image(GFX.Gui["feather/feather_half0"]);
      breathingMinigame.featherHalfLeft.CenterOrigin();
      breathingMinigame.featherHalfRight = new Monocle.Image(GFX.Gui["feather/feather_half1"]);
      breathingMinigame.featherHalfRight.CenterOrigin();
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        breathingMinigame.featherHalfLeft.Position = breathingMinigame.featherSprite.Position + Vector2.Lerp(Vector2.Zero, new Vector2((float) sbyte.MinValue, -32f), p);
        breathingMinigame.featherHalfRight.Position = breathingMinigame.featherSprite.Position + Vector2.Lerp(Vector2.Zero, new Vector2(128f, 32f), p);
        breathingMinigame.featherAlpha = 1f - p;
        yield return (object) null;
      }
    }

    public override void Update()
    {
      this.timer += Engine.DeltaTime;
      this.trailSpeed = Calc.Approach(this.trailSpeed, this.speed, (float) ((double) Engine.DeltaTime * 200.0 * 8.0));
      if (this.featherWave != null)
        this.featherSprite.Rotation = (float) ((double) this.featherWave.Value * 0.25 + 0.10000000149011612);
      for (int index = 0; index < this.particles.Length; ++index)
      {
        this.particles[index].Position.Y += this.particles[index].Speed * this.particleSpeed * Engine.DeltaTime;
        if ((double) this.particleSpeed > -400.0)
          this.particles[index].Position.X += (float) (((double) this.particleSpeed + 400.0) * Math.Sin((double) this.particles[index].Sin) * 0.10000000149011612) * Engine.DeltaTime;
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
        this.featherBuffer = VirtualContent.CreateRenderTarget("breathing-minigame-a", width, height);
        this.smokeBuffer = VirtualContent.CreateRenderTarget("breathing-minigame-b", width / 2, height / 2);
        this.tempBuffer = VirtualContent.CreateRenderTarget("breathing-minigame-c", width / 2, height / 2);
      }
      Engine.Graphics.GraphicsDevice.SetRenderTarget(this.featherBuffer.Target);
      Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
      Matrix scale = Matrix.CreateScale((float) this.featherBuffer.Width / 1920f);
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, scale);
      if (this.losing)
      {
        Sprite featherSprite = this.featherSprite;
        featherSprite.Position = featherSprite.Position + new Vector2((float) Calc.Random.Range(-1, 1), (float) Calc.Random.Range(-1, 1)).SafeNormalize() * this.losingTimer * 10f;
        this.featherSprite.Rotation += (float) ((double) Calc.Random.Range(-1, 1) * (double) this.losingTimer * 0.10000000149011612);
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
      GaussianBlur.Blur((Texture2D) this.smokeBuffer.Target, this.tempBuffer, this.smokeBuffer);
    }

    public override void Render()
    {
      Color color = (double) this.insideTargetTimer > 0.20000000298023224 ? Color.White : Color.White * 0.6f;
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * this.bgAlpha);
      if (this.Scene is Level scene && (scene.FrozenOrPaused || scene.RetryPlayerCorpse != null || scene.SkippingCutscene))
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
        vector2_1 = new Vector2(Math.Min(1f, (float) (1.0 / (-(double) this.particleSpeed * 0.004000000189989805))), Math.Max(1f, (float) (1.0 * -(double) this.particleSpeed * 0.004000000189989805)));
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
        this.Sin = Calc.Random.NextFloat(6.2831855f);
      }
    }
  }
}
