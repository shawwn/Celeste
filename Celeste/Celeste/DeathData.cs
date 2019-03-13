// Decompiled with JetBrains decompiler
// Type: Celeste.DeathData
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class DeathData
  {
    public Vector2 Position;
    public int Amount;

    public DeathData(Vector2 position)
    {
      this.Position = position;
      this.Amount = 1;
    }

    public DeathData(DeathData old, Vector2 add)
    {
      this.Position = Vector2.Lerp(old.Position, add, 1f / (float) (old.Amount + 1));
      this.Amount = old.Amount + 1;
    }

    public bool CombinesWith(Vector2 position)
    {
      return (double) Vector2.DistanceSquared(this.Position, position) <= 100.0;
    }

    public void Render()
    {
      float num1 = Math.Min(0.7f, (float) (0.300000011920929 + 0.100000001490116 * (double) this.Amount));
      int num2 = Math.Min(6, this.Amount + 1);
      Draw.Rect(this.Position.X - (float) num2, this.Position.Y - (float) num2, (float) (num2 * 2), (float) (num2 * 2), Color.Red * num1);
    }
  }
}

