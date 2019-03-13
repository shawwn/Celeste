// Decompiled with JetBrains decompiler
// Type: Celeste.BloomPoint
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  [Tracked(false)]
  public class BloomPoint : Component
  {
    public Vector2 Position = Vector2.Zero;
    public float Alpha = 1f;
    public float Radius = 8f;

    public float X
    {
      get
      {
        return this.Position.X;
      }
      set
      {
        this.Position.X = value;
      }
    }

    public float Y
    {
      get
      {
        return this.Position.Y;
      }
      set
      {
        this.Position.Y = value;
      }
    }

    public BloomPoint(float alpha, float radius)
      : base(false, true)
    {
      this.Alpha = alpha;
      this.Radius = radius;
    }

    public BloomPoint(Vector2 position, float alpha, float radius)
      : base(false, true)
    {
      this.Position = position;
      this.Alpha = alpha;
      this.Radius = radius;
    }
  }
}

