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
        this.vertexBuffer[index].Color = (__Null) ScreenWipe.WipeColor;
    }

    public override void Render(Scene scene)
    {
      float percent = this.Percent;
      int num = 1080;
      Vector2 vector2_1;
      ((Vector2) ref vector2_1).\u002Ector(960f, (float) num - (float) (num * 2) * percent);
      Vector2 vector2_2;
      ((Vector2) ref vector2_2).\u002Ector(-10f, (float) (num * 2) * (1f - percent));
      Vector2 vector2_3;
      ((Vector2) ref vector2_3).\u002Ector((float) this.Right, (float) (num * 2) * (1f - percent));
      if (!this.WipeIn)
      {
        this.vertexBuffer[0].Position = (__Null) new Vector3(vector2_1, 0.0f);
        this.vertexBuffer[1].Position = (__Null) new Vector3(vector2_2, 0.0f);
        this.vertexBuffer[2].Position = (__Null) new Vector3(vector2_3, 0.0f);
        this.vertexBuffer[3].Position = (__Null) new Vector3(vector2_2, 0.0f);
        this.vertexBuffer[4].Position = (__Null) new Vector3(vector2_3, 0.0f);
        this.vertexBuffer[5].Position = (__Null) new Vector3((float) vector2_2.X, (float) (vector2_2.Y + (double) num + 10.0), 0.0f);
        this.vertexBuffer[6].Position = (__Null) new Vector3(vector2_3, 0.0f);
        this.vertexBuffer[8].Position = (__Null) new Vector3((float) vector2_3.X, (float) (vector2_3.Y + (double) num + 10.0), 0.0f);
        this.vertexBuffer[7].Position = (__Null) new Vector3((float) vector2_2.X, (float) (vector2_2.Y + (double) num + 10.0), 0.0f);
      }
      else
      {
        this.vertexBuffer[0].Position = (__Null) new Vector3((float) vector2_2.X, (float) (vector2_1.Y - (double) num - 10.0), 0.0f);
        this.vertexBuffer[1].Position = (__Null) new Vector3((float) vector2_3.X, (float) (vector2_1.Y - (double) num - 10.0), 0.0f);
        this.vertexBuffer[2].Position = (__Null) new Vector3(vector2_1, 0.0f);
        this.vertexBuffer[3].Position = (__Null) new Vector3((float) vector2_2.X, (float) (vector2_1.Y - (double) num - 10.0), 0.0f);
        this.vertexBuffer[4].Position = (__Null) new Vector3(vector2_1, 0.0f);
        this.vertexBuffer[5].Position = (__Null) new Vector3(vector2_2, 0.0f);
        this.vertexBuffer[6].Position = (__Null) new Vector3((float) vector2_3.X, (float) (vector2_1.Y - (double) num - 10.0), 0.0f);
        this.vertexBuffer[7].Position = (__Null) new Vector3(vector2_3, 0.0f);
        this.vertexBuffer[8].Position = (__Null) new Vector3(vector2_1, 0.0f);
      }
      ScreenWipe.DrawPrimitives(this.vertexBuffer);
    }
  }
}
