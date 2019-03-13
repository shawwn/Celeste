// Decompiled with JetBrains decompiler
// Type: Celeste.WindWipe
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  public class WindWipe : ScreenWipe
  {
    private int t;
    private int columns;
    private int rows;
    private VertexPositionColor[] vertexBuffer;

    public WindWipe(Scene scene, bool wipeIn, Action onComplete = null)
      : base(scene, wipeIn, onComplete)
    {
      this.t = 40;
      this.columns = 1920 / this.t + 1;
      this.rows = 1080 / this.t + 1;
      this.vertexBuffer = new VertexPositionColor[this.columns * this.rows * 6];
      for (int index = 0; index < this.vertexBuffer.Length; ++index)
        this.vertexBuffer[index].Color = ScreenWipe.WipeColor;
    }

    public override void Render(Scene scene)
    {
      float num1 = (float) (this.columns * this.rows);
      int index1 = 0;
      for (int index2 = 0; index2 < this.columns; ++index2)
      {
        for (int index3 = 0; index3 < this.rows; ++index3)
        {
          int num2 = this.WipeIn ? this.columns - index2 - 1 : index2;
          float val1_1 = (float) ((double) ((index3 + num2 % 2) % 2 * (this.rows + index3 / 2) + (index3 + num2 % 2 + 1) % 2 * (index3 / 2) + num2 * this.rows) / (double) num1 * 0.5);
          float val1_2 = val1_1 + 300f / num1;
          float num3 = (float) (((double) Math.Max(val1_1, Math.Min(val1_2, this.WipeIn ? 1f - this.Percent : this.Percent)) - (double) val1_1) / ((double) val1_2 - (double) val1_1));
          float x1 = ((float) index2 - 0.5f) * (float) this.t;
          float y1 = (float) (((double) index3 - 0.5) * (double) this.t - (double) this.t * 0.5 * (double) num3);
          float x2 = x1 + (float) this.t;
          float y2 = y1 + (float) this.t * num3;
          this.vertexBuffer[index1].Position = new Vector3(x1, y1, 0.0f);
          this.vertexBuffer[index1 + 1].Position = new Vector3(x2, y1, 0.0f);
          this.vertexBuffer[index1 + 2].Position = new Vector3(x1, y2, 0.0f);
          this.vertexBuffer[index1 + 3].Position = new Vector3(x2, y1, 0.0f);
          this.vertexBuffer[index1 + 4].Position = new Vector3(x2, y2, 0.0f);
          this.vertexBuffer[index1 + 5].Position = new Vector3(x1, y2, 0.0f);
          index1 += 6;
        }
      }
      ScreenWipe.DrawPrimitives(this.vertexBuffer);
    }
  }
}

