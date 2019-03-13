// Decompiled with JetBrains decompiler
// Type: Celeste.HiresSnow
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

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
      this.overlay = GFX.Overworld[nameof (overlay)];
      this.snow = GFX.Overworld[nameof (snow)].GetSubtexture(1, 1, 254, 254, (MTexture) null);
      this.particles = new HiresSnow.Particle[50];
      this.Reset();
    }

    public void Reset()
    {
      for (int index = 0; index < this.particles.Length; ++index)
      {
        this.particles[index].Reset(this.Direction);
        this.particles[index].Position.X = (__Null) (double) Calc.Random.NextFloat((float) Engine.Width);
        this.particles[index].Position.Y = (__Null) (double) Calc.Random.NextFloat((float) Engine.Height);
      }
    }

    public override void Update(Scene scene)
    {
      base.Update(scene);
      if (this.AttachAlphaTo != null)
        this.Alpha = this.AttachAlphaTo.Percent;
      for (int index = 0; index < this.particles.Length; ++index)
      {
        ref Vector2 local1 = ref this.particles[index].Position;
        local1 = Vector2.op_Addition(local1, Vector2.op_Multiply(Vector2.op_Multiply(this.Direction, this.particles[index].Speed), Engine.DeltaTime));
        ref __Null local2 = ref this.particles[index].Position.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local2 = ^(float&) ref local2 + (float) (Math.Sin((double) this.particles[index].Sin) * 100.0) * Engine.DeltaTime;
        this.particles[index].Sin += Engine.DeltaTime;
        if (this.particles[index].Position.X < (double) sbyte.MinValue || this.particles[index].Position.X > (double) (Engine.Width + 128) || (this.particles[index].Position.Y < (double) sbyte.MinValue || this.particles[index].Position.Y > (double) (Engine.Height + 128)))
          this.particles[index].Reset(this.Direction);
      }
      this.timer += Engine.DeltaTime;
    }

    public override void Render(Scene scene)
    {
      float num1 = Calc.Clamp(((Vector2) ref this.Direction).Length(), 0.0f, 20f);
      float num2 = 0.0f;
      Vector2 one = Vector2.get_One();
      bool flag = (double) num1 > 1.0;
      if (flag)
      {
        num2 = this.Direction.Angle();
        ((Vector2) ref one).\u002Ector(num1, (float) (0.200000002980232 + (1.0 - (double) num1 / 20.0) * 0.800000011920929));
      }
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.Additive, (SamplerState) SamplerState.LinearWrap, (DepthStencilState) null, (RasterizerState) null, (Effect) null, Engine.ScreenMatrix);
      float num3 = this.Alpha * this.ParticleAlpha;
      for (int index = 0; index < this.particles.Length; ++index)
      {
        Color color = this.particles[index].Color;
        float rotation = this.particles[index].Rotation;
        if ((double) num3 < 1.0)
          color = Color.op_Multiply(color, num3);
        this.snow.DrawCentered(this.particles[index].Position, color, Vector2.op_Multiply(one, this.particles[index].Scale), flag ? num2 : rotation);
      }
      Draw.SpriteBatch.Draw(this.overlay.Texture.Texture, Vector2.get_Zero(), new Rectangle?(new Rectangle(-(int) (this.timer * 32f % (float) this.overlay.Width), -(int) (this.timer * 20f % (float) this.overlay.Height), 1920, 1080)), Color.op_Multiply(Color.get_White(), this.Alpha * this.overlayAlpha));
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
        if (direction.X < 0.0)
          this.Position = new Vector2((float) (Engine.Width + 128), Calc.Random.NextFloat((float) Engine.Height));
        else if (direction.X > 0.0)
          this.Position = new Vector2((float) sbyte.MinValue, Calc.Random.NextFloat((float) Engine.Height));
        else if (direction.Y > 0.0)
          this.Position = new Vector2(Calc.Random.NextFloat((float) Engine.Width), (float) sbyte.MinValue);
        else if (direction.Y < 0.0)
          this.Position = new Vector2(Calc.Random.NextFloat((float) Engine.Width), (float) (Engine.Height + 128));
        this.Sin = Calc.Random.NextFloat(6.283185f);
        this.Rotation = Calc.Random.NextFloat(6.283185f);
        this.Color = Color.Lerp(Color.get_White(), Color.get_Transparent(), val * 0.8f);
      }
    }
  }
}
