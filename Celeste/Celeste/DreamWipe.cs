// Decompiled with JetBrains decompiler
// Type: Celeste.DreamWipe
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  public class DreamWipe : ScreenWipe
  {
    private readonly int circleColumns = 15;
    private readonly int circleRows = 8;
    private const int circleSegments = 32;
    private const float circleFillSpeed = 400f;
    private static DreamWipe.Circle[] circles;
    private static VertexPositionColor[] vertexBuffer;

    public DreamWipe(Scene scene, bool wipeIn, Action onComplete = null)
      : base(scene, wipeIn, onComplete)
    {
      if (DreamWipe.vertexBuffer == null)
        DreamWipe.vertexBuffer = new VertexPositionColor[(this.circleColumns + 2) * (this.circleRows + 2) * 32 * 3];
      if (DreamWipe.circles == null)
        DreamWipe.circles = new DreamWipe.Circle[(this.circleColumns + 2) * (this.circleRows + 2)];
      for (int index = 0; index < DreamWipe.vertexBuffer.Length; ++index)
        DreamWipe.vertexBuffer[index].Color = ScreenWipe.WipeColor;
      int num1 = 1920 / this.circleColumns;
      int num2 = 1080 / this.circleRows;
      int num3 = 0;
      int index1 = 0;
      for (; num3 < this.circleColumns + 2; ++num3)
      {
        for (int index2 = 0; index2 < this.circleRows + 2; ++index2)
        {
          DreamWipe.circles[index1].Position = new Vector2(((float) (num3 - 1) + 0.2f + Calc.Random.NextFloat(0.6f)) * (float) num1, ((float) (index2 - 1) + 0.2f + Calc.Random.NextFloat(0.6f)) * (float) num2);
          DreamWipe.circles[index1].Delay = Calc.Random.NextFloat(0.05f) + (float) ((this.WipeIn ? (double) (this.circleColumns - num3) : (double) num3) * 0.0179999992251396);
          DreamWipe.circles[index1].Radius = this.WipeIn ? (float) (400.0 * ((double) this.Duration - (double) DreamWipe.circles[index1].Delay)) : 0.0f;
          ++index1;
        }
      }
    }

    public override void Update(Scene scene)
    {
      base.Update(scene);
      for (int index = 0; index < DreamWipe.circles.Length; ++index)
      {
        if (!this.WipeIn)
        {
          DreamWipe.circles[index].Delay -= Engine.DeltaTime;
          if ((double) DreamWipe.circles[index].Delay <= 0.0)
            DreamWipe.circles[index].Radius += Engine.DeltaTime * 400f;
        }
        else if ((double) DreamWipe.circles[index].Radius > 0.0)
          DreamWipe.circles[index].Radius -= Engine.DeltaTime * 400f;
        else
          DreamWipe.circles[index].Radius = 0.0f;
      }
    }

    public override void Render(Scene scene)
    {
      int num1 = 0;
      for (int index1 = 0; index1 < DreamWipe.circles.Length; ++index1)
      {
        DreamWipe.Circle circle = DreamWipe.circles[index1];
        Vector2 vector2 = new Vector2(1f, 0.0f);
        for (float num2 = 0.0f; (double) num2 < 32.0; ++num2)
        {
          Vector2 vector = Calc.AngleToVector((float) (((double) num2 + 1.0) / 32.0 * 6.28318548202515), 1f);
          VertexPositionColor[] vertexBuffer1 = DreamWipe.vertexBuffer;
          int index2 = num1;
          int num3 = index2 + 1;
          vertexBuffer1[index2].Position = new Vector3(circle.Position, 0.0f);
          VertexPositionColor[] vertexBuffer2 = DreamWipe.vertexBuffer;
          int index3 = num3;
          int num4 = index3 + 1;
          vertexBuffer2[index3].Position = new Vector3(circle.Position + vector2 * circle.Radius, 0.0f);
          VertexPositionColor[] vertexBuffer3 = DreamWipe.vertexBuffer;
          int index4 = num4;
          num1 = index4 + 1;
          vertexBuffer3[index4].Position = new Vector3(circle.Position + vector * circle.Radius, 0.0f);
          vector2 = vector;
        }
      }
      ScreenWipe.DrawPrimitives(DreamWipe.vertexBuffer);
    }

    private struct Circle
    {
      public Vector2 Position;
      public float Radius;
      public float Delay;
    }
  }
}

