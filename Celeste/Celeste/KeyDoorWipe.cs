// Decompiled with JetBrains decompiler
// Type: Celeste.KeyDoorWipe
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  public class KeyDoorWipe : ScreenWipe
  {
    private VertexPositionColor[] vertex = new VertexPositionColor[57];

    public KeyDoorWipe(Scene scene, bool wipeIn, Action onComplete = null)
      : base(scene, wipeIn, onComplete)
    {
      for (int index = 0; index < this.vertex.Length; ++index)
        this.vertex[index].Color = ScreenWipe.WipeColor;
    }

    public override void Render(Scene scene)
    {
      int num1 = 1090;
      int num2 = 540;
      float num3 = this.WipeIn ? 1f - this.Percent : this.Percent;
      float num4 = Ease.SineInOut(Math.Min(1f, num3 / 0.5f));
      float num5 = Ease.SineInOut(1f - Calc.Clamp((float) (((double) num3 - 0.5) / 0.300000011920929), 0.0f, 1f));
      float num6 = num4;
      float num7 = (float) (1.0 + (1.0 - (double) num4) * 0.5);
      float x1 = 960f * num4;
      float x2 = 128f * num5 * num6;
      float y1 = 128f * num5 * num7;
      float y2 = (float) num2 - (float) num2 * 0.3f * num5 * num7;
      float y3 = (float) num2 + (float) num2 * 0.5f * num5 * num7;
      float angleRadians1 = 0.0f;
      int num8 = 0;
      VertexPositionColor[] vertex1 = this.vertex;
      int index1 = num8;
      int num9 = index1 + 1;
      vertex1[index1].Position = new Vector3(-10f, -10f, 0.0f);
      VertexPositionColor[] vertex2 = this.vertex;
      int index2 = num9;
      int num10 = index2 + 1;
      vertex2[index2].Position = new Vector3(x1, -10f, 0.0f);
      VertexPositionColor[] vertex3 = this.vertex;
      int index3 = num10;
      int num11 = index3 + 1;
      vertex3[index3].Position = new Vector3(x1, y2 - y1, 0.0f);
      for (int index4 = 1; index4 <= 8; ++index4)
      {
        float angleRadians2 = (float) (-1.57079637050629 - (double) (index4 - 1) / 8.0 * 1.57079637050629);
        angleRadians1 = (float) (-1.57079637050629 - (double) index4 / 8.0 * 1.57079637050629);
        VertexPositionColor[] vertex4 = this.vertex;
        int index5 = num11;
        int num12 = index5 + 1;
        vertex4[index5].Position = new Vector3(-10f, -10f, 0.0f);
        VertexPositionColor[] vertex5 = this.vertex;
        int index6 = num12;
        int num13 = index6 + 1;
        vertex5[index6].Position = new Vector3(new Vector2(x1, y2) + Calc.AngleToVector(angleRadians2, 1f) * new Vector2(x2, y1), 0.0f);
        VertexPositionColor[] vertex6 = this.vertex;
        int index7 = num13;
        num11 = index7 + 1;
        vertex6[index7].Position = new Vector3(new Vector2(x1, y2) + Calc.AngleToVector(angleRadians1, 1f) * new Vector2(x2, y1), 0.0f);
      }
      VertexPositionColor[] vertex7 = this.vertex;
      int index8 = num11;
      int num14 = index8 + 1;
      vertex7[index8].Position = new Vector3(-10f, -10f, 0.0f);
      VertexPositionColor[] vertex8 = this.vertex;
      int index9 = num14;
      int num15 = index9 + 1;
      vertex8[index9].Position = new Vector3(x1 - x2, y2, 0.0f);
      VertexPositionColor[] vertex9 = this.vertex;
      int index10 = num15;
      int num16 = index10 + 1;
      vertex9[index10].Position = new Vector3(-10f, (float) num1, 0.0f);
      for (int index4 = 1; index4 <= 6; ++index4)
      {
        float angleRadians2 = (float) (3.14159274101257 - (double) (index4 - 1) / 8.0 * 1.57079637050629);
        angleRadians1 = (float) (3.14159274101257 - (double) index4 / 8.0 * 1.57079637050629);
        VertexPositionColor[] vertex4 = this.vertex;
        int index5 = num16;
        int num12 = index5 + 1;
        vertex4[index5].Position = new Vector3(-10f, (float) num1, 0.0f);
        VertexPositionColor[] vertex5 = this.vertex;
        int index6 = num12;
        int num13 = index6 + 1;
        vertex5[index6].Position = new Vector3(new Vector2(x1, y2) + Calc.AngleToVector(angleRadians2, 1f) * new Vector2(x2, y1), 0.0f);
        VertexPositionColor[] vertex6 = this.vertex;
        int index7 = num13;
        num16 = index7 + 1;
        vertex6[index7].Position = new Vector3(new Vector2(x1, y2) + Calc.AngleToVector(angleRadians1, 1f) * new Vector2(x2, y1), 0.0f);
      }
      VertexPositionColor[] vertex10 = this.vertex;
      int index11 = num16;
      int num17 = index11 + 1;
      vertex10[index11].Position = new Vector3(-10f, (float) num1, 0.0f);
      VertexPositionColor[] vertex11 = this.vertex;
      int index12 = num17;
      int num18 = index12 + 1;
      vertex11[index12].Position = new Vector3(new Vector2(x1, y2) + Calc.AngleToVector(angleRadians1, 1f) * new Vector2(x2, y1), 0.0f);
      VertexPositionColor[] vertex12 = this.vertex;
      int index13 = num18;
      int num19 = index13 + 1;
      vertex12[index13].Position = new Vector3(x1 - x2 * 0.8f, y3, 0.0f);
      VertexPositionColor[] vertex13 = this.vertex;
      int index14 = num19;
      int num20 = index14 + 1;
      vertex13[index14].Position = new Vector3(-10f, (float) num1, 0.0f);
      VertexPositionColor[] vertex14 = this.vertex;
      int index15 = num20;
      int num21 = index15 + 1;
      vertex14[index15].Position = new Vector3(x1 - x2 * 0.8f, y3, 0.0f);
      VertexPositionColor[] vertex15 = this.vertex;
      int index16 = num21;
      int num22 = index16 + 1;
      vertex15[index16].Position = new Vector3(x1, y3, 0.0f);
      VertexPositionColor[] vertex16 = this.vertex;
      int index17 = num22;
      int num23 = index17 + 1;
      vertex16[index17].Position = new Vector3(-10f, (float) num1, 0.0f);
      VertexPositionColor[] vertex17 = this.vertex;
      int index18 = num23;
      int num24 = index18 + 1;
      vertex17[index18].Position = new Vector3(x1, y3, 0.0f);
      VertexPositionColor[] vertex18 = this.vertex;
      int index19 = num24;
      int num25 = index19 + 1;
      vertex18[index19].Position = new Vector3(x1, (float) num1, 0.0f);
      ScreenWipe.DrawPrimitives(this.vertex);
      for (int index4 = 0; index4 < this.vertex.Length; ++index4)
        this.vertex[index4].Position.X = 1920f - this.vertex[index4].Position.X;
      ScreenWipe.DrawPrimitives(this.vertex);
    }
  }
}

