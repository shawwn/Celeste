// Decompiled with JetBrains decompiler
// Type: Celeste.Dust
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public static class Dust
  {
    public static void Burst(Vector2 position, float direction, int count = 1)
    {
      Vector2 vector = Calc.AngleToVector(direction - 1.570796f, 4f);
      vector.X = (__Null) (double) Math.Abs((float) vector.X);
      vector.Y = (__Null) (double) Math.Abs((float) vector.Y);
      Level scene = Engine.Scene as Level;
      for (int index = 0; index < count; ++index)
        scene.Particles.Emit(Calc.Random.Choose<ParticleType>(new ParticleType[1]
        {
          ParticleTypes.Dust
        }), Vector2.op_Addition(position, Calc.Random.Range(Vector2.op_UnaryNegation(vector), vector)), direction);
    }

    public static void BurstFG(Vector2 position, float direction, int count = 1, float range = 4f)
    {
      Vector2 vector = Calc.AngleToVector(direction - 1.570796f, range);
      vector.X = (__Null) (double) Math.Abs((float) vector.X);
      vector.Y = (__Null) (double) Math.Abs((float) vector.Y);
      Level scene = Engine.Scene as Level;
      for (int index = 0; index < count; ++index)
        scene.ParticlesFG.Emit(Calc.Random.Choose<ParticleType>(new ParticleType[1]
        {
          ParticleTypes.Dust
        }), Vector2.op_Addition(position, Calc.Random.Range(Vector2.op_UnaryNegation(vector), vector)), direction);
    }
  }
}
