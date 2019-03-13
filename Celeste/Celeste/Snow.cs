// Decompiled with JetBrains decompiler
// Type: Celeste.Snow
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class Snow : Backdrop
  {
    public static readonly Color[] ForegroundColors = new Color[2]
    {
      Color.White,
      Color.CornflowerBlue
    };
    public static readonly Color[] BackgroundColors = new Color[2]
    {
      new Color(0.2f, 0.2f, 0.2f, 1f),
      new Color(0.1f, 0.2f, 0.5f, 1f)
    };
    public float Alpha = 1f;
    private float visibleFade = 1f;
    private float linearFade = 1f;
    private Snow.Particle[] particles = new Snow.Particle[60];
    private Color[] colors;
    private Color[] blendedColors;

    public Snow(bool foreground)
    {
      this.colors = foreground ? Snow.ForegroundColors : Snow.BackgroundColors;
      this.blendedColors = new Color[this.colors.Length];
      int num1 = foreground ? 120 : 40;
      int num2 = foreground ? 300 : 100;
      for (int index = 0; index < this.particles.Length; ++index)
        this.particles[index].Init(this.colors.Length, (float) num1, (float) num2);
    }

    public override void Update(Scene scene)
    {
      base.Update(scene);
      this.visibleFade = Calc.Approach(this.visibleFade, this.IsVisible(scene as Level) ? 1f : 0.0f, Engine.DeltaTime * 2f);
      if (this.FadeX != null)
        this.linearFade = this.FadeX.Value((scene as Level).Camera.X + 160f);
      for (int index = 0; index < this.particles.Length; ++index)
      {
        this.particles[index].Position.X -= this.particles[index].Speed * Engine.DeltaTime;
        this.particles[index].Position.Y += (float) (Math.Sin((double) this.particles[index].Sin) * (double) this.particles[index].Speed * 0.200000002980232) * Engine.DeltaTime;
        this.particles[index].Sin += Engine.DeltaTime;
      }
    }

    public override void Render(Scene scene)
    {
      if ((double) this.Alpha <= 0.0 || (double) this.visibleFade <= 0.0 || (double) this.linearFade <= 0.0)
        return;
      for (int index = 0; index < this.blendedColors.Length; ++index)
        this.blendedColors[index] = this.colors[index] * (this.Alpha * this.visibleFade * this.linearFade);
      Camera camera = (scene as Level).Camera;
      for (int index = 0; index < this.particles.Length; ++index)
      {
        Vector2 position = new Vector2(this.mod(this.particles[index].Position.X - camera.X, 320f), this.mod(this.particles[index].Position.Y - camera.Y, 180f));
        Color blendedColor = this.blendedColors[this.particles[index].Color];
        Draw.Pixel.DrawCentered(position, blendedColor);
      }
    }

    private float mod(float x, float m)
    {
      return (x % m + m) % m;
    }

    private struct Particle
    {
      public Vector2 Position;
      public int Color;
      public float Speed;
      public float Sin;

      public void Init(int maxColors, float speedMin, float speedMax)
      {
        this.Position = new Vector2(Calc.Random.NextFloat(320f), Calc.Random.NextFloat(180f));
        this.Color = Calc.Random.Next(maxColors);
        this.Speed = Calc.Random.Range(speedMin, speedMax);
        this.Sin = Calc.Random.NextFloat(6.283185f);
      }
    }
  }
}

