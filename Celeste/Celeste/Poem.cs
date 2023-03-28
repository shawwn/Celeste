// Decompiled with JetBrains decompiler
// Type: Celeste.Poem
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  public class Poem : Entity
  {
    private const float textScale = 1.5f;
    private static readonly Color[] colors = new Color[4]
    {
      Calc.HexToColor("8cc7fa"),
      Calc.HexToColor("ff668a"),
      Calc.HexToColor("fffc24"),
      Calc.HexToColor("ffffff")
    };
    public float Alpha = 1f;
    public float TextAlpha = 1f;
    public Vector2 Offset;
    public Sprite Heart;
    public float ParticleSpeed = 1f;
    public float Shake;
    private float timer;
    private string text;
    private bool disposed;
    private VirtualRenderTarget poem;
    private VirtualRenderTarget smoke;
    private VirtualRenderTarget temp;
    private Poem.Particle[] particles = new Poem.Particle[80];

    public Color Color { get; private set; }

    public Poem(string text, int heartIndex, float heartAlpha)
    {
      if (text != null)
        this.text = ActiveFont.FontSize.AutoNewline(text, 1024);
      this.Color = Poem.colors[heartIndex];
      this.Heart = GFX.GuiSpriteBank.Create("heartgem" + (object) heartIndex);
      this.Heart.Play("spin");
      this.Heart.Position = new Vector2(1920f, 1080f) * 0.5f;
      this.Heart.Color = Color.White * heartAlpha;
      int width = Math.Min(1920, Engine.ViewWidth);
      int height = Math.Min(1080, Engine.ViewHeight);
      this.poem = VirtualContent.CreateRenderTarget("poem-a", width, height);
      this.smoke = VirtualContent.CreateRenderTarget("poem-b", width / 2, height / 2);
      this.temp = VirtualContent.CreateRenderTarget("poem-c", width / 2, height / 2);
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
      float num = ActiveFont.Measure(this.text).X * 1.5f;
      Vector2 position = new Vector2(960f, 540f) + offset;
      mtexture.DrawCentered(position - Vector2.UnitX * (float) ((double) num / 2.0 + 64.0), color);
      ActiveFont.Draw(this.text, position, new Vector2(0.5f, 0.5f), Vector2.One * 1.5f, color);
      mtexture.DrawCentered(position + Vector2.UnitX * (float) ((double) num / 2.0 + 64.0), color);
    }

    public override void Update()
    {
      this.timer += Engine.DeltaTime;
      for (int index = 0; index < this.particles.Length; ++index)
      {
        this.particles[index].Percent += Engine.DeltaTime / this.particles[index].Duration * this.ParticleSpeed;
        if ((double) this.particles[index].Percent > 1.0)
          this.particles[index].Reset(0.0f);
      }
      this.Heart.Update();
    }

    public void BeforeRender()
    {
      if (this.disposed)
        return;
      Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) this.poem);
      Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
      Matrix scale = Matrix.CreateScale((float) this.poem.Width / 1920f);
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, scale);
      this.Heart.Position = this.Offset + new Vector2(1920f, 1080f) * 0.5f;
      this.Heart.Scale = Vector2.One * (float) (1.0 + (double) this.Shake * 0.10000000149011612);
      MTexture atla = OVR.Atlas["snow"];
      for (int index = 0; index < this.particles.Length; ++index)
      {
        Poem.Particle particle = this.particles[index];
        float num1 = Ease.SineIn(particle.Percent);
        Vector2 position = this.Heart.Position + particle.Direction * (1f - num1) * 1920f;
        float x = (float) (1.0 + (double) num1 * 2.0);
        float y = (float) (0.25 * (0.25 + (1.0 - (double) num1) * 0.75));
        float num2 = 1f - num1;
        atla.DrawCentered(position, this.Color * num2, new Vector2(x, y), (-particle.Direction).Angle());
      }
      Sprite heart = this.Heart;
      heart.Position = heart.Position + new Vector2(Calc.Random.Range(-1f, 1f), Calc.Random.Range(-1f, 1f)) * 16f * this.Shake;
      this.Heart.Render();
      if (!string.IsNullOrEmpty(this.text))
      {
        this.DrawPoem(this.Offset + new Vector2(-2f, 0.0f), Color.Black * this.TextAlpha);
        this.DrawPoem(this.Offset + new Vector2(2f, 0.0f), Color.Black * this.TextAlpha);
        this.DrawPoem(this.Offset + new Vector2(0.0f, -2f), Color.Black * this.TextAlpha);
        this.DrawPoem(this.Offset + new Vector2(0.0f, 2f), Color.Black * this.TextAlpha);
        this.DrawPoem(this.Offset + Vector2.Zero, this.Color * this.TextAlpha);
      }
      Draw.SpriteBatch.End();
      Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) this.smoke);
      Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
      MagicGlow.Render((Texture2D) (RenderTarget2D) this.poem, this.timer, -1f, Matrix.CreateScale(0.5f));
      GaussianBlur.Blur((Texture2D) (RenderTarget2D) this.smoke, this.temp, this.smoke);
    }

    public override void Render()
    {
      if (this.disposed || this.Scene.Paused)
        return;
      float scale = 1920f / (float) this.poem.Width;
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.smoke, Vector2.Zero, new Rectangle?(this.smoke.Bounds), Color.White * 0.3f * this.Alpha, 0.0f, Vector2.Zero, scale * 2f, SpriteEffects.None, 0.0f);
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.poem, Vector2.Zero, new Rectangle?(this.poem.Bounds), Color.White * this.Alpha, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
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
        this.Direction = Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), 1f);
        this.Percent = percent;
        this.Duration = (float) (0.5 + (double) Calc.Random.NextFloat() * 0.5);
      }
    }
  }
}
