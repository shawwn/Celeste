// Decompiled with JetBrains decompiler
// Type: Celeste.HeartWipe
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  public class HeartWipe : ScreenWipe
  {
    private VertexPositionColor[] vertex = new VertexPositionColor[111];

    public HeartWipe(Scene scene, bool wipeIn, Action onComplete = null)
      : base(scene, wipeIn, onComplete)
    {
      for (int index = 0; index < this.vertex.Length; ++index)
        this.vertex[index].Color = ScreenWipe.WipeColor;
    }

    public override void Render(Scene scene)
    {
      float num1 = (float) (((this.WipeIn ? (double) this.Percent : 1.0 - (double) this.Percent) - 0.200000002980232) / 0.800000011920929);
      if ((double) num1 <= 0.0)
      {
        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, Engine.ScreenMatrix);
        Draw.Rect(-1f, -1f, (float) (Engine.Width + 2), (float) (Engine.Height + 2), ScreenWipe.WipeColor);
        Draw.SpriteBatch.End();
      }
      else
      {
        Vector2 vector2_1 = new Vector2((float) Engine.Width, (float) Engine.Height) / 2f;
        float length = (float) Engine.Width * 0.75f * num1;
        float num2 = (float) Engine.Width * num1;
        float num3 = -0.25f;
        float num4 = -1.570796f;
        Vector2 vector2_2 = vector2_1 + new Vector2((float) -Math.Cos((double) num3) * length, (float) (-(double) length / 2.0));
        int num5 = 0;
        for (int index1 = 1; index1 <= 16; ++index1)
        {
          float angleRadians1 = num3 + (float) (((double) num4 - (double) num3) * ((double) (index1 - 1) / 16.0));
          float angleRadians2 = num3 + (float) (((double) num4 - (double) num3) * ((double) index1 / 16.0));
          VertexPositionColor[] vertex1 = this.vertex;
          int index2 = num5;
          int num6 = index2 + 1;
          vertex1[index2].Position = new Vector3(vector2_1.X, -num2, 0.0f);
          VertexPositionColor[] vertex2 = this.vertex;
          int index3 = num6;
          int num7 = index3 + 1;
          vertex2[index3].Position = new Vector3(vector2_2 + Calc.AngleToVector(angleRadians1, length), 0.0f);
          VertexPositionColor[] vertex3 = this.vertex;
          int index4 = num7;
          num5 = index4 + 1;
          vertex3[index4].Position = new Vector3(vector2_2 + Calc.AngleToVector(angleRadians2, length), 0.0f);
        }
        VertexPositionColor[] vertex4 = this.vertex;
        int index5 = num5;
        int num8 = index5 + 1;
        vertex4[index5].Position = new Vector3(vector2_1.X, -num2, 0.0f);
        VertexPositionColor[] vertex5 = this.vertex;
        int index6 = num8;
        int num9 = index6 + 1;
        vertex5[index6].Position = new Vector3(vector2_2 + new Vector2(0.0f, -length), 0.0f);
        VertexPositionColor[] vertex6 = this.vertex;
        int index7 = num9;
        int num10 = index7 + 1;
        vertex6[index7].Position = new Vector3(-num2, -num2, 0.0f);
        VertexPositionColor[] vertex7 = this.vertex;
        int index8 = num10;
        int num11 = index8 + 1;
        vertex7[index8].Position = new Vector3(-num2, -num2, 0.0f);
        VertexPositionColor[] vertex8 = this.vertex;
        int index9 = num11;
        int num12 = index9 + 1;
        vertex8[index9].Position = new Vector3(vector2_2 + new Vector2(0.0f, -length), 0.0f);
        VertexPositionColor[] vertex9 = this.vertex;
        int index10 = num12;
        int num13 = index10 + 1;
        vertex9[index10].Position = new Vector3(-num2, vector2_2.Y, 0.0f);
        float num14 = 2.356194f;
        for (int index1 = 1; index1 <= 16; ++index1)
        {
          float angleRadians1 = (float) (-1.57079637050629 - (double) (index1 - 1) / 16.0 * (double) num14);
          float angleRadians2 = (float) (-1.57079637050629 - (double) index1 / 16.0 * (double) num14);
          VertexPositionColor[] vertex1 = this.vertex;
          int index2 = num13;
          int num6 = index2 + 1;
          vertex1[index2].Position = new Vector3(-num2, vector2_2.Y, 0.0f);
          VertexPositionColor[] vertex2 = this.vertex;
          int index3 = num6;
          int num7 = index3 + 1;
          vertex2[index3].Position = new Vector3(vector2_2 + Calc.AngleToVector(angleRadians1, length), 0.0f);
          VertexPositionColor[] vertex3 = this.vertex;
          int index4 = num7;
          num13 = index4 + 1;
          vertex3[index4].Position = new Vector3(vector2_2 + Calc.AngleToVector(angleRadians2, length), 0.0f);
        }
        Vector2 vector2_3 = vector2_2 + Calc.AngleToVector(-1.570796f - num14, length);
        Vector2 vector2_4 = vector2_1 + new Vector2(0.0f, length * 1.8f);
        VertexPositionColor[] vertex10 = this.vertex;
        int index11 = num13;
        int num15 = index11 + 1;
        vertex10[index11].Position = new Vector3(-num2, vector2_2.Y, 0.0f);
        VertexPositionColor[] vertex11 = this.vertex;
        int index12 = num15;
        int num16 = index12 + 1;
        vertex11[index12].Position = new Vector3(vector2_3, 0.0f);
        VertexPositionColor[] vertex12 = this.vertex;
        int index13 = num16;
        int num17 = index13 + 1;
        vertex12[index13].Position = new Vector3(-num2, (float) Engine.Height + num2, 0.0f);
        VertexPositionColor[] vertex13 = this.vertex;
        int index14 = num17;
        int num18 = index14 + 1;
        vertex13[index14].Position = new Vector3(-num2, (float) Engine.Height + num2, 0.0f);
        VertexPositionColor[] vertex14 = this.vertex;
        int index15 = num18;
        int num19 = index15 + 1;
        vertex14[index15].Position = new Vector3(vector2_3, 0.0f);
        VertexPositionColor[] vertex15 = this.vertex;
        int index16 = num19;
        int num20 = index16 + 1;
        vertex15[index16].Position = new Vector3(vector2_4, 0.0f);
        VertexPositionColor[] vertex16 = this.vertex;
        int index17 = num20;
        int num21 = index17 + 1;
        vertex16[index17].Position = new Vector3(-num2, (float) Engine.Height + num2, 0.0f);
        VertexPositionColor[] vertex17 = this.vertex;
        int index18 = num21;
        int num22 = index18 + 1;
        vertex17[index18].Position = new Vector3(vector2_4, 0.0f);
        VertexPositionColor[] vertex18 = this.vertex;
        int index19 = num22;
        int num23 = index19 + 1;
        vertex18[index19].Position = new Vector3(vector2_1.X, (float) Engine.Height + num2, 0.0f);
        ScreenWipe.DrawPrimitives(this.vertex);
        for (int index1 = 0; index1 < this.vertex.Length; ++index1)
          this.vertex[index1].Position.X = 1920f - this.vertex[index1].Position.X;
        ScreenWipe.DrawPrimitives(this.vertex);
      }
    }
  }
}

