// Decompiled with JetBrains decompiler
// Type: Celeste.Petals
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class Petals : Backdrop
  {
    private static readonly Color[] colors = new Color[1]
    {
      Calc.HexToColor("ff3aa3")
    };
    private Petals.Particle[] particles = new Petals.Particle[40];
    private float fade;

    public Petals()
    {
      for (int i = 0; i < this.particles.Length; ++i)
        this.Reset(i);
    }

    private void Reset(int i)
    {
      this.particles[i].Position = new Vector2((float) Calc.Random.Range(0, 352), (float) Calc.Random.Range(0, 212));
      this.particles[i].Speed = Calc.Random.Range(6f, 16f);
      this.particles[i].Spin = Calc.Random.Range(8f, 12f) * 0.2f;
      this.particles[i].Color = Calc.Random.Next(Petals.colors.Length);
      this.particles[i].RotationCounter = Calc.Random.NextAngle();
      this.particles[i].MaxRotate = Calc.Random.Range(0.3f, 0.6f) * 1.570796f;
    }

    public override void Update(Scene scene)
    {
      base.Update(scene);
      for (int index = 0; index < this.particles.Length; ++index)
      {
        this.particles[index].Position.Y += this.particles[index].Speed * Engine.DeltaTime;
        this.particles[index].RotationCounter += this.particles[index].Spin * Engine.DeltaTime;
      }
      this.fade = Calc.Approach(this.fade, this.Visible ? 1f : 0.0f, Engine.DeltaTime);
    }

    public override void Render(Scene level)
    {
      if ((double) this.fade <= 0.0)
        return;
      Camera camera = (level as Level).Camera;
      MTexture mtexture = GFX.Game["particles/petal"];
      for (int index = 0; index < this.particles.Length; ++index)
      {
        Vector2 vector2 = new Vector2();
        vector2.X = this.Mod(this.particles[index].Position.X - camera.X, 352f) - 16f;
        vector2.Y = this.Mod(this.particles[index].Position.Y - camera.Y, 212f) - 16f;
        float angleRadians = (float) (1.57079637050629 + Math.Sin((double) this.particles[index].RotationCounter * (double) this.particles[index].MaxRotate) * 1.0);
        Vector2 position = vector2 + Calc.AngleToVector(angleRadians, 4f);
        mtexture.DrawCentered(position, Petals.colors[this.particles[index].Color] * this.fade, 1f, angleRadians - 0.8f);
      }
    }

    private float Mod(float x, float m)
    {
      return (x % m + m) % m;
    }

    private struct Particle
    {
      public Vector2 Position;
      public float Speed;
      public float Spin;
      public float MaxRotate;
      public int Color;
      public float RotationCounter;
    }
  }
}

