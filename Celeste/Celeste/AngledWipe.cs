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
        this.vertexBuffer[index].Color = ScreenWipe.WipeColor;
    }

    public override void Render(Scene scene)
    {
      float num1 = 183.3333f;
      float num2 = -64f;
      float num3 = 1984f;
      for (int index1 = 0; index1 < 6; ++index1)
      {
        int index2 = index1 * 6;
        float x = num2;
        float y = (float) ((double) index1 * (double) num1 - 10.0);
        float num4 = 0.0f;
        float num5 = (float) index1 / 6f;
        float num6 = (float) ((this.WipeIn ? 1.0 - (double) num5 : (double) num5) * 0.300000011920929);
        if ((double) this.Percent > (double) num6)
          num4 = Math.Min(1f, (float) (((double) this.Percent - (double) num6) / 0.699999988079071));
        if (this.WipeIn)
          num4 = 1f - num4;
        float num7 = num3 * num4;
        this.vertexBuffer[index2].Position = new Vector3(x, y, 0.0f);
        this.vertexBuffer[index2 + 1].Position = new Vector3(x + num7, y, 0.0f);
        this.vertexBuffer[index2 + 2].Position = new Vector3(x, y + num1, 0.0f);
        this.vertexBuffer[index2 + 3].Position = new Vector3(x + num7, y, 0.0f);
        this.vertexBuffer[index2 + 4].Position = new Vector3((float) ((double) x + (double) num7 + 64.0), y + num1, 0.0f);
        this.vertexBuffer[index2 + 5].Position = new Vector3(x, y + num1, 0.0f);
      }
      if (this.WipeIn)
      {
        for (int index = 0; index < this.vertexBuffer.Length; ++index)
        {
          this.vertexBuffer[index].Position.X = 1920f - this.vertexBuffer[index].Position.X;
          this.vertexBuffer[index].Position.Y = 1080f - this.vertexBuffer[index].Position.Y;
        }
      }
      ScreenWipe.DrawPrimitives(this.vertexBuffer);
    }
  }
}

