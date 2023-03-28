// Decompiled with JetBrains decompiler
// Type: Celeste.DisperseImage
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class DisperseImage : Entity
  {
    private List<DisperseImage.Particle> particles = new List<DisperseImage.Particle>();
    private Vector2 scale;

    public DisperseImage(
      Vector2 position,
      Vector2 direction,
      Vector2 origin,
      Vector2 scale,
      MTexture texture)
    {
      this.Position = position;
      this.scale = new Vector2(Math.Abs(scale.X), Math.Abs(scale.Y));
      float num = direction.Angle();
      for (int x = 0; x < texture.Width; ++x)
      {
        for (int y = 0; y < texture.Height; ++y)
          this.particles.Add(new DisperseImage.Particle()
          {
            Position = position + scale * (new Vector2((float) x, (float) y) - origin),
            Direction = Calc.AngleToVector(num + Calc.Random.Range(-0.2f, 0.2f), 1f),
            Sin = Calc.Random.NextFloat(6.2831855f),
            Speed = Calc.Random.Range(0.0f, 4f),
            Alpha = 1f,
            Percent = 0.0f,
            Duration = Calc.Random.Range(1f, 3f),
            Image = new MTexture(texture, x, y, 1, 1)
          });
      }
    }

    public override void Update()
    {
      bool flag = false;
      foreach (DisperseImage.Particle particle in this.particles)
      {
        particle.Percent += Engine.DeltaTime / particle.Duration;
        particle.Position += particle.Direction * particle.Speed * Engine.DeltaTime;
        particle.Position += (float) Math.Sin((double) particle.Sin) * particle.Direction.Perpendicular() * particle.Percent * 4f * Engine.DeltaTime;
        particle.Speed += Engine.DeltaTime * (float) (4.0 + (double) particle.Percent * 80.0);
        particle.Sin += Engine.DeltaTime * 4f;
        if ((double) particle.Percent < 1.0)
          flag = true;
      }
      if (flag)
        return;
      this.RemoveSelf();
    }

    public override void Render()
    {
      foreach (DisperseImage.Particle particle in this.particles)
        particle.Image.Draw(particle.Position, Vector2.Zero, Color.White * (1f - particle.Percent), this.scale);
    }

    private class Particle
    {
      public Vector2 Position;
      public Vector2 Direction;
      public float Speed;
      public float Sin;
      public float Alpha;
      public float Percent;
      public float Duration;
      public MTexture Image;
    }
  }
}
