// Decompiled with JetBrains decompiler
// Type: Celeste.StardustFG
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class StardustFG : Backdrop
  {
    private static readonly Color[] colors = new Color[3]
    {
      Calc.HexToColor("4cccef"),
      Calc.HexToColor("f243bd"),
      Calc.HexToColor("42f1dd")
    };
    private StardustFG.Particle[] particles = new StardustFG.Particle[50];
    private float fade;
    private Vector2 scale = Vector2.One;

    public StardustFG()
    {
      for (int i = 0; i < this.particles.Length; ++i)
        this.Reset(i, Calc.Random.NextFloat());
    }

    private void Reset(int i, float p)
    {
      this.particles[i].Percent = p;
      this.particles[i].Position = new Vector2((float) Calc.Random.Range(0, 320), (float) Calc.Random.Range(0, 180));
      this.particles[i].Speed = (float) Calc.Random.Range(4, 14);
      this.particles[i].Spin = Calc.Random.Range(0.25f, 18.849556f);
      this.particles[i].Duration = Calc.Random.Range(1f, 4f);
      this.particles[i].Direction = Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), 1f);
      this.particles[i].Color = Calc.Random.Next(StardustFG.colors.Length);
    }

    public override void Update(Scene scene)
    {
      base.Update(scene);
      Level level = scene as Level;
      int num = (double) level.Wind.Y == 0.0 ? 1 : 0;
      Vector2 vector2 = Vector2.Zero;
      if (num != 0)
      {
        this.scale.X = Math.Max(1f, Math.Abs(level.Wind.X) / 100f);
        this.scale.Y = 1f;
        vector2 = new Vector2(level.Wind.X, 0.0f);
      }
      else
      {
        this.scale.X = 1f;
        this.scale.Y = Math.Max(1f, Math.Abs(level.Wind.Y) / 40f);
        vector2 = new Vector2(0.0f, level.Wind.Y * 2f);
      }
      for (int i = 0; i < this.particles.Length; ++i)
      {
        if ((double) this.particles[i].Percent >= 1.0)
          this.Reset(i, 0.0f);
        this.particles[i].Percent += Engine.DeltaTime / this.particles[i].Duration;
        this.particles[i].Position += (this.particles[i].Direction * this.particles[i].Speed + vector2) * Engine.DeltaTime;
        this.particles[i].Direction.Rotate(this.particles[i].Spin * Engine.DeltaTime);
      }
      this.fade = Calc.Approach(this.fade, this.Visible ? 1f : 0.0f, Engine.DeltaTime);
    }

    public override void Render(Scene level)
    {
      if ((double) this.fade <= 0.0)
        return;
      Camera camera = (level as Level).Camera;
      for (int index = 0; index < this.particles.Length; ++index)
      {
        Vector2 position = new Vector2();
        position.X = this.mod(this.particles[index].Position.X - camera.X, 320f);
        position.Y = this.mod(this.particles[index].Position.Y - camera.Y, 180f);
        float percent = this.particles[index].Percent;
        float num = ((double) percent >= 0.699999988079071 ? Calc.ClampedMap(percent, 0.7f, 1f, 1f, 0.0f) : Calc.ClampedMap(percent, 0.0f, 0.3f)) * this.FadeAlphaMultiplier;
        Draw.Rect(position, this.scale.X, this.scale.Y, StardustFG.colors[this.particles[index].Color] * (this.fade * num));
      }
    }

    private float mod(float x, float m) => (x % m + m) % m;

    private struct Particle
    {
      public Vector2 Position;
      public float Percent;
      public float Duration;
      public Vector2 Direction;
      public float Speed;
      public float Spin;
      public int Color;
    }
  }
}
