// Decompiled with JetBrains decompiler
// Type: Celeste.AngledWipe
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  public class AngledWipe : ScreenWipe
  {
    private VertexPositionColor[] vertexBuffer = new VertexPositionColor[36];
    private const int rows = 6;
    private const float angleSize = 64f;

    public AngledWipe(Scene scene, bool wipeIn, Action onComplete = null)
      : base(scene, wipeIn, onComplete)
    {
      for (int index = 0; index < this.vertexBuffer.Length; ++index)
        this.vertexBuffer[index].Color = (__Null) ScreenWipe.WipeColor;
    }

    public override void Render(Scene scene)
    {
      float num1 = 183.3333f;
      float num2 = -64f;
      float num3 = 1984f;
      for (int index1 = 0; index1 < 6; ++index1)
      {
        int index2 = index1 * 6;
        float num4 = num2;
        float num5 = (float) ((double) index1 * (double) num1 - 10.0);
        float num6 = 0.0f;
        float num7 = (float) index1 / 6f;
        float num8 = (float) ((this.WipeIn ? 1.0 - (double) num7 : (double) num7) * 0.300000011920929);
        if ((double) this.Percent > (double) num8)
          num6 = Math.Min(1f, (float) (((double) this.Percent - (double) num8) / 0.699999988079071));
        if (this.WipeIn)
          num6 = 1f - num6;
        float num9 = num3 * num6;
        this.vertexBuffer[index2].Position = (__Null) new Vector3(num4, num5, 0.0f);
        this.vertexBuffer[index2 + 1].Position = (__Null) new Vector3(num4 + num9, num5, 0.0f);
        this.vertexBuffer[index2 + 2].Position = (__Null) new Vector3(num4, num5 + num1, 0.0f);
        this.vertexBuffer[index2 + 3].Position = (__Null) new Vector3(num4 + num9, num5, 0.0f);
        this.vertexBuffer[index2 + 4].Position = (__Null) new Vector3((float) ((double) num4 + (double) num9 + 64.0), num5 + num1, 0.0f);
        this.vertexBuffer[index2 + 5].Position = (__Null) new Vector3(num4, num5 + num1, 0.0f);
      }
      if (this.WipeIn)
      {
        for (int index = 0; index < this.vertexBuffer.Length; ++index)
        {
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          (^(Vector3&) ref this.vertexBuffer[index].Position).X = (__Null) (1920.0 - (^(Vector3&) ref this.vertexBuffer[index].Position).X);
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          (^(Vector3&) ref this.vertexBuffer[index].Position).Y = (__Null) (1080.0 - (^(Vector3&) ref this.vertexBuffer[index].Position).Y);
        }
      }
      ScreenWipe.DrawPrimitives(this.vertexBuffer);
    }
  }
}
