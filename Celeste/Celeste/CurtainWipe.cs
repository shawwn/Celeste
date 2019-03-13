// Decompiled with JetBrains decompiler
// Type: Celeste.CurtainWipe
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  public class CurtainWipe : ScreenWipe
  {
    private VertexPositionColor[] vertexBufferLeft = new VertexPositionColor[192];
    private VertexPositionColor[] vertexBufferRight = new VertexPositionColor[192];

    public CurtainWipe(Scene scene, bool wipeIn, Action onComplete = null)
      : base(scene, wipeIn, onComplete)
    {
      for (int index = 0; index < this.vertexBufferLeft.Length; ++index)
        this.vertexBufferLeft[index].Color = ScreenWipe.WipeColor;
    }

    public override void Render(Scene scene)
    {
      float num1 = (this.WipeIn ? Ease.CubeInOut : Ease.CubeInOut)(this.WipeIn ? 1f - this.Percent : this.Percent);
      float num2 = Math.Min(1f, num1 / 0.3f);
      float percent = Math.Max(0.0f, Math.Min(1f, (float) (((double) num1 - 0.100000001490116) / 0.899999976158142 / 0.899999976158142)));
      Vector2 begin1 = new Vector2(0.0f, 540f * num2);
      Vector2 end = new Vector2(1920f, 1592f) / 2f;
      Vector2 control = (begin1 + end) / 2f + Vector2.UnitY * 1080f * 0.25f;
      Vector2 begin2 = new Vector2((float) (896.0 + 200.0 * (double) num1), (float) (256.0 * (double) num2 - 350.0));
      Vector2 point1 = new SimpleCurve(begin1, end, control).GetPoint(percent);
      Vector2 vector2_1 = new Vector2(point1.X + 64f * num1, 1080f);
      int num3 = 0;
      VertexPositionColor[] vertexBufferLeft1 = this.vertexBufferLeft;
      int index1 = num3;
      int num4 = index1 + 1;
      vertexBufferLeft1[index1].Position = new Vector3(-10f, -10f, 0.0f);
      VertexPositionColor[] vertexBufferLeft2 = this.vertexBufferLeft;
      int index2 = num4;
      int num5 = index2 + 1;
      vertexBufferLeft2[index2].Position = new Vector3(begin2.X, -10f, 0.0f);
      VertexPositionColor[] vertexBufferLeft3 = this.vertexBufferLeft;
      int index3 = num5;
      int num6 = index3 + 1;
      vertexBufferLeft3[index3].Position = new Vector3(begin2.X, begin2.Y, 0.0f);
      VertexPositionColor[] vertexBufferLeft4 = this.vertexBufferLeft;
      int index4 = num6;
      int num7 = index4 + 1;
      vertexBufferLeft4[index4].Position = new Vector3(-10f, -10f, 0.0f);
      VertexPositionColor[] vertexBufferLeft5 = this.vertexBufferLeft;
      int index5 = num7;
      int num8 = index5 + 1;
      vertexBufferLeft5[index5].Position = new Vector3(-10f, point1.Y, 0.0f);
      VertexPositionColor[] vertexBufferLeft6 = this.vertexBufferLeft;
      int index6 = num8;
      int num9 = index6 + 1;
      vertexBufferLeft6[index6].Position = new Vector3(point1.X, point1.Y, 0.0f);
      VertexPositionColor[] vertexBufferLeft7 = this.vertexBufferLeft;
      int index7 = num9;
      int num10 = index7 + 1;
      vertexBufferLeft7[index7].Position = new Vector3(point1.X, point1.Y, 0.0f);
      VertexPositionColor[] vertexBufferLeft8 = this.vertexBufferLeft;
      int index8 = num10;
      int num11 = index8 + 1;
      vertexBufferLeft8[index8].Position = new Vector3(-10f, point1.Y, 0.0f);
      VertexPositionColor[] vertexBufferLeft9 = this.vertexBufferLeft;
      int index9 = num11;
      int num12 = index9 + 1;
      vertexBufferLeft9[index9].Position = new Vector3(-10f, 1090f, 0.0f);
      VertexPositionColor[] vertexBufferLeft10 = this.vertexBufferLeft;
      int index10 = num12;
      int num13 = index10 + 1;
      vertexBufferLeft10[index10].Position = new Vector3(point1.X, point1.Y, 0.0f);
      VertexPositionColor[] vertexBufferLeft11 = this.vertexBufferLeft;
      int index11 = num13;
      int num14 = index11 + 1;
      vertexBufferLeft11[index11].Position = new Vector3(-10f, 1090f, 0.0f);
      VertexPositionColor[] vertexBufferLeft12 = this.vertexBufferLeft;
      int index12 = num14;
      int index13 = index12 + 1;
      vertexBufferLeft12[index12].Position = new Vector3(vector2_1.X, vector2_1.Y + 10f, 0.0f);
      int num15 = index13;
      Vector2 vector2_2 = begin2;
      for (; index13 < this.vertexBufferLeft.Length; index13 += 3)
      {
        Vector2 point2 = new SimpleCurve(begin2, point1, (begin2 + point1) / 2f + new Vector2(0.0f, 384f * percent)).GetPoint((float) (index13 - num15) / (float) (this.vertexBufferLeft.Length - num15 - 3));
        this.vertexBufferLeft[index13].Position = new Vector3(-10f, -10f, 0.0f);
        this.vertexBufferLeft[index13 + 1].Position = new Vector3(vector2_2, 0.0f);
        this.vertexBufferLeft[index13 + 2].Position = new Vector3(point2, 0.0f);
        vector2_2 = point2;
      }
      for (int index14 = 0; index14 < this.vertexBufferLeft.Length; ++index14)
      {
        this.vertexBufferRight[index14] = this.vertexBufferLeft[index14];
        this.vertexBufferRight[index14].Position.X = 1920f - this.vertexBufferRight[index14].Position.X;
      }
      ScreenWipe.DrawPrimitives(this.vertexBufferLeft);
      ScreenWipe.DrawPrimitives(this.vertexBufferRight);
    }
  }
}

