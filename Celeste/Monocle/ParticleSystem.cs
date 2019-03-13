// Decompiled with JetBrains decompiler
// Type: Monocle.ParticleSystem
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using System;

namespace Monocle
{
  public class ParticleSystem : Entity
  {
    private Particle[] particles;
    private int nextSlot;

    public ParticleSystem(int depth, int maxParticles)
    {
      this.particles = new Particle[maxParticles];
      this.Depth = depth;
    }

    public void Clear()
    {
      for (int index = 0; index < this.particles.Length; ++index)
        this.particles[index].Active = false;
    }

    public void ClearRect(Rectangle rect, bool inside)
    {
      for (int index = 0; index < this.particles.Length; ++index)
      {
        Vector2 position = this.particles[index].Position;
        if (((double) position.X > (double) rect.Left && (double) position.Y > (double) rect.Top && (double) position.X < (double) rect.Right && (double) position.Y < (double) rect.Bottom) == inside)
          this.particles[index].Active = false;
      }
    }

    public override void Update()
    {
      for (int index = 0; index < this.particles.Length; ++index)
      {
        if (this.particles[index].Active)
          this.particles[index].Update(new float?());
      }
    }

    public override void Render()
    {
      foreach (Particle particle in this.particles)
      {
        if (particle.Active)
          particle.Render();
      }
    }

    public void Render(float alpha)
    {
      foreach (Particle particle in this.particles)
      {
        if (particle.Active)
          particle.Render(alpha);
      }
    }

    public void Simulate(float duration, float interval, Action<ParticleSystem> emitter)
    {
      float num1 = 0.016f;
      for (float num2 = 0.0f; (double) num2 < (double) duration; num2 += num1)
      {
        if ((int) (((double) num2 - (double) num1) / (double) interval) < (int) ((double) num2 / (double) interval))
          emitter(this);
        for (int index = 0; index < this.particles.Length; ++index)
        {
          if (this.particles[index].Active)
            this.particles[index].Update(new float?(num1));
        }
      }
    }

    public void Add(Particle particle)
    {
      this.particles[this.nextSlot] = particle;
      this.nextSlot = (this.nextSlot + 1) % this.particles.Length;
    }

    public void Emit(ParticleType type, Vector2 position)
    {
      type.Create(ref this.particles[this.nextSlot], position);
      this.nextSlot = (this.nextSlot + 1) % this.particles.Length;
    }

    public void Emit(ParticleType type, Vector2 position, float direction)
    {
      type.Create(ref this.particles[this.nextSlot], position, direction);
      this.nextSlot = (this.nextSlot + 1) % this.particles.Length;
    }

    public void Emit(ParticleType type, Vector2 position, Color color)
    {
      type.Create(ref this.particles[this.nextSlot], position, color);
      this.nextSlot = (this.nextSlot + 1) % this.particles.Length;
    }

    public void Emit(ParticleType type, Vector2 position, Color color, float direction)
    {
      type.Create(ref this.particles[this.nextSlot], position, color, direction);
      this.nextSlot = (this.nextSlot + 1) % this.particles.Length;
    }

    public void Emit(ParticleType type, int amount, Vector2 position, Vector2 positionRange)
    {
      for (int index = 0; index < amount; ++index)
        this.Emit(type, Calc.Random.Range(position - positionRange, position + positionRange));
    }

    public void Emit(
      ParticleType type,
      int amount,
      Vector2 position,
      Vector2 positionRange,
      float direction)
    {
      for (int index = 0; index < amount; ++index)
        this.Emit(type, Calc.Random.Range(position - positionRange, position + positionRange), direction);
    }

    public void Emit(
      ParticleType type,
      int amount,
      Vector2 position,
      Vector2 positionRange,
      Color color)
    {
      for (int index = 0; index < amount; ++index)
        this.Emit(type, Calc.Random.Range(position - positionRange, position + positionRange), color);
    }

    public void Emit(
      ParticleType type,
      int amount,
      Vector2 position,
      Vector2 positionRange,
      Color color,
      float direction)
    {
      for (int index = 0; index < amount; ++index)
        this.Emit(type, Calc.Random.Range(position - positionRange, position + positionRange), color, direction);
    }

    public void Emit(
      ParticleType type,
      Entity track,
      int amount,
      Vector2 position,
      Vector2 positionRange,
      float direction)
    {
      for (int index = 0; index < amount; ++index)
      {
        type.Create(ref this.particles[this.nextSlot], track, Calc.Random.Range(position - positionRange, position + positionRange), direction, type.Color);
        this.nextSlot = (this.nextSlot + 1) % this.particles.Length;
      }
    }
  }
}

