// Decompiled with JetBrains decompiler
// Type: Celeste.ChimneySmokeFx
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class ChimneySmokeFx
  {
    public static void Burst(Vector2 position, float direction, int count, ParticleSystem system = null)
    {
      Vector2 vector = Calc.AngleToVector(direction - 1.570796f, 2f);
      vector.X = (__Null) (double) Math.Abs((float) vector.X);
      vector.Y = (__Null) (double) Math.Abs((float) vector.Y);
      if (system == null)
        system = (Engine.Scene as Level).ParticlesFG;
      for (int index = 0; index < count; ++index)
        system.Emit(Calc.Random.Choose<ParticleType>(new ParticleType[1]
        {
          ParticleTypes.Chimney
        }), Vector2.op_Addition(position, Calc.Random.Range(Vector2.op_UnaryNegation(vector), vector)), direction);
    }
  }
}
