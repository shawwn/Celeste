// Decompiled with JetBrains decompiler
// Type: Celeste.MountainWipe
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  public class MountainWipe : ScreenWipe
  {
    private VertexPositionColor[] vertexBuffer = new VertexPositionColor[9];

    public MountainWipe(Scene scene, bool wipeIn, Action onComplete = null)
      : base(scene, wipeIn, onComplete)
    {
      for (int index = 0; index < this.vertexBuffer.Length; ++index)
        this.vertexBuffer[index].Color = ScreenWipe.WipeColor;
    }

    public override void Render(Scene scene)
    {
      float percent = this.Percent;
      int num = 1080;
      Vector2 vector2_1 = new Vector2(960f, (float) num - (float) (num * 2) * percent);
      Vector2 vector2_2 = new Vector2(-10f, (float) (num * 2) * (1f - percent));
      Vector2 vector2_3 = new Vector2((float) this.Right, (float) (num * 2) * (1f - percent));
      if (!this.WipeIn)
      {
        this.vertexBuffer[0].Position = new Vector3(vector2_1, 0.0f);
        this.vertexBuffer[1].Position = new Vector3(vector2_2, 0.0f);
        this.vertexBuffer[2].Position = new Vector3(vector2_3, 0.0f);
        this.vertexBuffer[3].Position = new Vector3(vector2_2, 0.0f);
        this.vertexBuffer[4].Position = new Vector3(vector2_3, 0.0f);
        this.vertexBuffer[5].Position = new Vector3(vector2_2.X, (float) ((double) vector2_2.Y + (double) num + 10.0), 0.0f);
        this.vertexBuffer[6].Position = new Vector3(vector2_3, 0.0f);
        this.vertexBuffer[8].Position = new Vector3(vector2_3.X, (float) ((double) vector2_3.Y + (double) num + 10.0), 0.0f);
        this.vertexBuffer[7].Position = new Vector3(vector2_2.X, (float) ((double) vector2_2.Y + (double) num + 10.0), 0.0f);
      }
      else
      {
        this.vertexBuffer[0].Position = new Vector3(vector2_2.X, (float) ((double) vector2_1.Y - (double) num - 10.0), 0.0f);
        this.vertexBuffer[1].Position = new Vector3(vector2_3.X, (float) ((double) vector2_1.Y - (double) num - 10.0), 0.0f);
        this.vertexBuffer[2].Position = new Vector3(vector2_1, 0.0f);
        this.vertexBuffer[3].Position = new Vector3(vector2_2.X, (float) ((double) vector2_1.Y - (double) num - 10.0), 0.0f);
        this.vertexBuffer[4].Position = new Vector3(vector2_1, 0.0f);
        this.vertexBuffer[5].Position = new Vector3(vector2_2, 0.0f);
        this.vertexBuffer[6].Position = new Vector3(vector2_3.X, (float) ((double) vector2_1.Y - (double) num - 10.0), 0.0f);
        this.vertexBuffer[7].Position = new Vector3(vector2_3, 0.0f);
        this.vertexBuffer[8].Position = new Vector3(vector2_1, 0.0f);
      }
      ScreenWipe.DrawPrimitives(this.vertexBuffer);
    }
  }
}

