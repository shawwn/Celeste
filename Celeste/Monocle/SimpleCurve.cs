// Decompiled with JetBrains decompiler
// Type: Monocle.SimpleCurve
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;

namespace Monocle
{
  public struct SimpleCurve
  {
    public Vector2 Begin;
    public Vector2 End;
    public Vector2 Control;

    public SimpleCurve(Vector2 begin, Vector2 end, Vector2 control)
    {
      this.Begin = begin;
      this.End = end;
      this.Control = control;
    }

    public void DoubleControl()
    {
      this.Control += this.Control - (this.Begin + (this.End - this.Begin) / 2f);
    }

    public Vector2 GetPoint(float percent)
    {
      float num = 1f - percent;
      return num * num * this.Begin + 2f * num * percent * this.Control + percent * percent * this.End;
    }

    public float GetLengthParametric(int resolution)
    {
      Vector2 vector2 = this.Begin;
      float num = 0.0f;
      for (int index = 1; index <= resolution; ++index)
      {
        Vector2 point = this.GetPoint((float) index / (float) resolution);
        num += (point - vector2).Length();
        vector2 = point;
      }
      return num;
    }

    public void Render(Vector2 offset, Color color, int resolution)
    {
      Vector2 start = offset + this.Begin;
      for (int index = 1; index <= resolution; ++index)
      {
        Vector2 end = offset + this.GetPoint((float) index / (float) resolution);
        Draw.Line(start, end, color);
        start = end;
      }
    }

    public void Render(Vector2 offset, Color color, int resolution, float thickness)
    {
      Vector2 start = offset + this.Begin;
      for (int index = 1; index <= resolution; ++index)
      {
        Vector2 end = offset + this.GetPoint((float) index / (float) resolution);
        Draw.Line(start, end, color, thickness);
        start = end;
      }
    }

    public void Render(Color color, int resolution)
    {
      this.Render(Vector2.Zero, color, resolution);
    }

    public void Render(Color color, int resolution, float thickness)
    {
      this.Render(Vector2.Zero, color, resolution, thickness);
    }
  }
}

