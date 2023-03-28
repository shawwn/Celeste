// Decompiled with JetBrains decompiler
// Type: Celeste.HiresSnow
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  public class HiresSnow : Monocle.Renderer
  {
    public float Alpha = 1f;
    public float ParticleAlpha = 1f;
    public Vector2 Direction = new Vector2(-1f, 0.0f);
    public ScreenWipe AttachAlphaTo;
    private HiresSnow.Particle[] particles;
    private MTexture overlay;
    private MTexture snow;
    private float timer;
    private float overlayAlpha;

    public HiresSnow(float overlayAlpha = 0.45f)
    {
      this.overlayAlpha = overlayAlpha;
      this.overlay = OVR.Atlas[nameof (overlay)];
      this.snow = OVR.Atlas[nameof (snow)].GetSubtexture(1, 1, 254, 254);
      this.particles = new HiresSnow.Particle[50];
      this.Reset();
    }

    public void Reset()
    {
      for (int index = 0; index < this.particles.Length; ++index)
      {
        this.particles[index].Reset(this.Direction);
        this.particles[index].Position.X = Calc.Random.NextFloat((float) Engine.Width);
        this.particles[index].Position.Y = Calc.Random.NextFloat((float) Engine.Height);
      }
    }

    public override void Update(Scene scene)
    {
      base.Update(scene);
      if (this.AttachAlphaTo != null)
        this.Alpha = this.AttachAlphaTo.Percent;
      for (int index = 0; index < this.particles.Length; ++index)
      {
        this.particles[index].Position += this.Direction * this.particles[index].Speed * Engine.DeltaTime;
        this.particles[index].Position.Y += (float) (Math.Sin((double) this.particles[index].Sin) * 100.0) * Engine.DeltaTime;
        this.particles[index].Sin += Engine.DeltaTime;
        if ((double) this.particles[index].Position.X < (double) sbyte.MinValue || (double) this.particles[index].Position.X > (double) (Engine.Width + 128) || (double) this.particles[index].Position.Y < (double) sbyte.MinValue || (double) this.particles[index].Position.Y > (double) (Engine.Height + 128))
          this.particles[index].Reset(this.Direction);
      }
      this.timer += Engine.DeltaTime;
    }

    public override void Render(Scene scene)
    {
      float x = Calc.Clamp(this.Direction.Length(), 0.0f, 20f);
      float num1 = 0.0f;
      Vector2 vector2 = Vector2.One;
      bool flag = (double) x > 1.0;
      if (flag)
      {
        num1 = this.Direction.Angle();
        vector2 = new Vector2(x, (float) (0.20000000298023224 + (1.0 - (double) x / 20.0) * 0.800000011920929));
      }
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearWrap, (DepthStencilState) null, (RasterizerState) null, (Effect) null, Engine.ScreenMatrix);
      float num2 = this.Alpha * this.ParticleAlpha;
      for (int index = 0; index < this.particles.Length; ++index)
      {
        Color color = this.particles[index].Color;
        float rotation = this.particles[index].Rotation;
        if ((double) num2 < 1.0)
          color *= num2;
        this.snow.DrawCentered(this.particles[index].Position, color, vector2 * this.particles[index].Scale, flag ? num1 : rotation);
      }
      Draw.SpriteBatch.Draw(this.overlay.Texture.Texture, Vector2.Zero, new Rectangle?(new Rectangle(-(int) (this.timer * 32f % (float) this.overlay.Width), -(int) (this.timer * 20f % (float) this.overlay.Height), 1920, 1080)), Color.White * (this.Alpha * this.overlayAlpha));
      Draw.SpriteBatch.End();
    }

    private struct Particle
    {
      public float Scale;
      public Vector2 Position;
      public float Speed;
      public float Sin;
      public float Rotation;
      public Color Color;

      public void Reset(Vector2 direction)
      {
        float num = Calc.Random.NextFloat();
        float val = num * (num * num * num);
        this.Scale = Calc.Map(val, 0.0f, 1f, 0.05f, 0.8f);
        this.Speed = this.Scale * (float) Calc.Random.Range(2500, 5000);
        if ((double) direction.X < 0.0)
          this.Position = new Vector2((float) (Engine.Width + 128), Calc.Random.NextFloat((float) Engine.Height));
        else if ((double) direction.X > 0.0)
          this.Position = new Vector2((float) sbyte.MinValue, Calc.Random.NextFloat((float) Engine.Height));
        else if ((double) direction.Y > 0.0)
          this.Position = new Vector2(Calc.Random.NextFloat((float) Engine.Width), (float) sbyte.MinValue);
        else if ((double) direction.Y < 0.0)
          this.Position = new Vector2(Calc.Random.NextFloat((float) Engine.Width), (float) (Engine.Height + 128));
        this.Sin = Calc.Random.NextFloat(6.2831855f);
        this.Rotation = Calc.Random.NextFloat(6.2831855f);
        this.Color = Color.Lerp(Color.White, Color.Transparent, val * 0.8f);
      }
    }
  }
}
