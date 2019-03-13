// Decompiled with JetBrains decompiler
// Type: Celeste.Poem
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  public class Poem : Entity
  {
    private static readonly Color[] colors = new Color[3]
    {
      Calc.HexToColor("8cc7fa"),
      Calc.HexToColor("ff668a"),
      Calc.HexToColor("fffc24")
    };
    public float Alpha = 1f;
    private Poem.Particle[] particles = new Poem.Particle[80];
    private const float textScale = 1.5f;
    public Sprite Heart;
    private float timer;
    private string text;
    private bool disposed;
    private VirtualRenderTarget poem;
    private VirtualRenderTarget smoke;
    private VirtualRenderTarget temp;

    public Color Color { get; private set; }

    public Poem(string text, AreaMode mode)
    {
      if (text != null)
        this.text = ActiveFont.FontSize.AutoNewline(text, 1024);
      this.Color = Poem.colors[(int) mode];
      this.Heart = GFX.GuiSpriteBank.Create("heartgem" + (object) (int) mode);
      this.Heart.Play("spin", false, false);
      this.Heart.Position = Vector2.op_Multiply(new Vector2(1920f, 1080f), 0.5f);
      this.Heart.Color = Color.op_Multiply(Color.get_White(), mode == AreaMode.CSide ? 1f : 0.6f);
      int width = Math.Min(1920, Engine.ViewWidth);
      int height = Math.Min(1080, Engine.ViewHeight);
      this.poem = VirtualContent.CreateRenderTarget("poem-a", width, height, false, true, 0);
      this.smoke = VirtualContent.CreateRenderTarget("poem-b", width / 2, height / 2, false, true, 0);
      this.temp = VirtualContent.CreateRenderTarget("poem-c", width / 2, height / 2, false, true, 0);
      this.Tag = (int) Tags.HUD | (int) Tags.FrozenUpdate;
      this.Add((Component) new BeforeRenderHook(new Action(this.BeforeRender)));
      for (int index = 0; index < this.particles.Length; ++index)
        this.particles[index].Reset(Calc.Random.NextFloat());
    }

    public void Dispose()
    {
      if (this.disposed)
        return;
      this.poem.Dispose();
      this.smoke.Dispose();
      this.temp.Dispose();
      this.RemoveSelf();
      this.disposed = true;
    }

    private void DrawPoem(Vector2 offset, Color color)
    {
      MTexture mtexture = GFX.Gui["poemside"];
      float num = (float) (ActiveFont.Measure(this.text).X * 1.5);
      Vector2 position = Vector2.op_Addition(new Vector2(960f, 540f), offset);
      mtexture.DrawCentered(Vector2.op_Subtraction(position, Vector2.op_Multiply(Vector2.get_UnitX(), (float) ((double) num / 2.0 + 64.0))), color);
      ActiveFont.Draw(this.text, position, new Vector2(0.5f, 0.5f), Vector2.op_Multiply(Vector2.get_One(), 1.5f), color);
      mtexture.DrawCentered(Vector2.op_Addition(position, Vector2.op_Multiply(Vector2.get_UnitX(), (float) ((double) num / 2.0 + 64.0))), color);
    }

    public override void Update()
    {
      this.timer += Engine.DeltaTime;
      for (int index = 0; index < this.particles.Length; ++index)
      {
        this.particles[index].Percent += Engine.DeltaTime / this.particles[index].Duration;
        if ((double) this.particles[index].Percent > 1.0)
          this.particles[index].Reset(0.0f);
      }
      this.Heart.Update();
    }

    public void BeforeRender()
    {
      if (this.disposed)
        return;
      Engine.Graphics.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) this.poem);
      Engine.Graphics.get_GraphicsDevice().Clear(Color.get_Transparent());
      Matrix scale = Matrix.CreateScale((float) this.poem.Width / 1920f);
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, scale);
      MTexture mtexture = GFX.Overworld["snow"];
      for (int index = 0; index < this.particles.Length; ++index)
      {
        Poem.Particle particle = this.particles[index];
        float num1 = Ease.SineIn(particle.Percent);
        Vector2 position = Vector2.op_Addition(this.Heart.Position, Vector2.op_Multiply(Vector2.op_Multiply(particle.Direction, 1f - num1), 1920f));
        float num2 = (float) (1.0 + (double) num1 * 2.0);
        float num3 = (float) (0.25 * (0.25 + (1.0 - (double) num1) * 0.75));
        float num4 = 1f - num1;
        mtexture.DrawCentered(position, Color.op_Multiply(this.Color, num4), new Vector2(num2, num3), Vector2.op_UnaryNegation(particle.Direction).Angle());
      }
      this.Heart.Render();
      if (!string.IsNullOrEmpty(this.text))
      {
        this.DrawPoem(new Vector2(-2f, 0.0f), Color.get_Black());
        this.DrawPoem(new Vector2(2f, 0.0f), Color.get_Black());
        this.DrawPoem(new Vector2(0.0f, -2f), Color.get_Black());
        this.DrawPoem(new Vector2(0.0f, 2f), Color.get_Black());
        this.DrawPoem(Vector2.get_Zero(), this.Color);
      }
      Draw.SpriteBatch.End();
      Engine.Graphics.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) this.smoke);
      Engine.Graphics.get_GraphicsDevice().Clear(Color.get_Transparent());
      MagicGlow.Render((Texture2D) (RenderTarget2D) this.poem, this.timer, -1f, Matrix.CreateScale(0.5f));
      GaussianBlur.Blur((Texture2D) (RenderTarget2D) this.smoke, this.temp, this.smoke, 0.0f, true, GaussianBlur.Samples.Nine, 1f, GaussianBlur.Direction.Both, 1f);
    }

    public override void Render()
    {
      if (this.disposed || this.Scene.Paused)
        return;
      float num = 1920f / (float) this.poem.Width;
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.smoke, Vector2.get_Zero(), new Rectangle?(this.smoke.Bounds), Color.op_Multiply(Color.op_Multiply(Color.get_White(), 0.3f), this.Alpha), 0.0f, Vector2.get_Zero(), num * 2f, (SpriteEffects) 0, 0.0f);
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.poem, Vector2.get_Zero(), new Rectangle?(this.poem.Bounds), Color.op_Multiply(Color.get_White(), this.Alpha), 0.0f, Vector2.get_Zero(), num, (SpriteEffects) 0, 0.0f);
    }

    public override void Removed(Scene scene)
    {
      base.Removed(scene);
      this.Dispose();
    }

    public override void SceneEnd(Scene scene)
    {
      base.SceneEnd(scene);
      this.Dispose();
    }

    private struct Particle
    {
      public Vector2 Direction;
      public float Percent;
      public float Duration;

      public void Reset(float percent)
      {
        this.Direction = Calc.AngleToVector(Calc.Random.NextFloat(6.283185f), 1f);
        this.Percent = percent;
        this.Duration = (float) (0.5 + (double) Calc.Random.NextFloat() * 0.5);
      }
    }
  }
}
