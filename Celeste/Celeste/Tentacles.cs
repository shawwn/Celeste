// Decompiled with JetBrains decompiler
// Type: Celeste.Tentacles
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  public class Tentacles : Backdrop
  {
    private float hideTimer = 5f;
    private const int NodesPerTentacle = 10;
    private Tentacles.Side side;
    private float width;
    private Vector2 origin;
    private Vector2 outwards;
    private float outwardsOffset;
    private VertexPositionColor[] vertices;
    private int vertexCount;
    private Tentacles.Tentacle[] tentacles;
    private int tentacleCount;

    public Tentacles(Tentacles.Side side, Color color, float outwardsOffset)
    {
      this.side = side;
      this.outwardsOffset = outwardsOffset;
      this.UseSpritebatch = false;
      switch (side)
      {
        case Tentacles.Side.Right:
          this.outwards = new Vector2(-1f, 0.0f);
          this.width = 180f;
          this.origin = new Vector2(320f, 90f);
          break;
        case Tentacles.Side.Left:
          this.outwards = new Vector2(1f, 0.0f);
          this.width = 180f;
          this.origin = new Vector2(0.0f, 90f);
          break;
        case Tentacles.Side.Top:
          this.outwards = new Vector2(0.0f, 1f);
          this.width = 320f;
          this.origin = new Vector2(160f, 0.0f);
          break;
        case Tentacles.Side.Bottom:
          this.outwards = new Vector2(0.0f, -1f);
          this.width = 320f;
          this.origin = new Vector2(160f, 180f);
          break;
      }
      float num = 0.0f;
      this.tentacles = new Tentacles.Tentacle[100];
      for (int index = 0; index < this.tentacles.Length && (double) num < (double) this.width + 40.0; ++index)
      {
        this.tentacles[index].Length = Calc.Random.NextFloat();
        this.tentacles[index].Offset = Calc.Random.NextFloat();
        this.tentacles[index].Step = Calc.Random.NextFloat();
        this.tentacles[index].Position = -200f;
        this.tentacles[index].Approach = Calc.Random.NextFloat();
        num += this.tentacles[index].Width = 6f + Calc.Random.NextFloat(20f);
        ++this.tentacleCount;
      }
      this.vertices = new VertexPositionColor[this.tentacleCount * 11 * 6];
      for (int index = 0; index < this.vertices.Length; ++index)
        this.vertices[index].Color = color;
    }

    public override void Update(Scene scene)
    {
      bool flag = this.IsVisible(scene as Level);
      float num1 = 0.0f;
      if (flag)
      {
        Camera camera = (scene as Level).Camera;
        Player entity = scene.Tracker.GetEntity<Player>();
        if (entity != null)
        {
          if (this.side == Tentacles.Side.Right)
            num1 = (float) (320.0 - ((double) entity.X - (double) camera.X) - 160.0);
          else if (this.side == Tentacles.Side.Bottom)
            num1 = (float) (180.0 - ((double) entity.Y - (double) camera.Y) - 180.0);
        }
        this.hideTimer = 0.0f;
      }
      else
      {
        num1 = -200f;
        this.hideTimer += Engine.DeltaTime;
      }
      float num2 = num1 + this.outwardsOffset;
      this.Visible = (double) this.hideTimer < 5.0;
      if (!this.Visible)
        return;
      Vector2 vector2_1 = -this.outwards.Perpendicular();
      int num3 = 0;
      Vector2 vector2_2 = this.origin - vector2_1 * (float) ((double) this.width / 2.0 + 2.0);
      for (int index1 = 0; index1 < this.tentacleCount; ++index1)
      {
        Vector2 vector2_3 = vector2_2 + vector2_1 * this.tentacles[index1].Width * 0.5f;
        this.tentacles[index1].Position += (float) (((double) num2 - (double) this.tentacles[index1].Position) * (1.0 - Math.Pow(0.5 * (0.5 + (double) this.tentacles[index1].Approach * 0.5), (double) Engine.DeltaTime)));
        Vector2 vector2_4 = (float) ((double) this.tentacles[index1].Position + Math.Sin((double) scene.TimeActive + (double) this.tentacles[index1].Offset * 4.0) * 8.0 + (double) (this.origin - vector2_3).Length() * 0.699999988079071) * this.outwards;
        Vector2 vector2_5 = vector2_3 + vector2_4;
        float num4 = (float) (2.0 + (double) this.tentacles[index1].Length * 8.0);
        Vector2 vector2_6 = vector2_5;
        Vector2 vector2_7 = vector2_1 * this.tentacles[index1].Width * 0.5f;
        VertexPositionColor[] vertices1 = this.vertices;
        int index2 = num3;
        int num5 = index2 + 1;
        vertices1[index2].Position = new Vector3(vector2_3 + vector2_7, 0.0f);
        VertexPositionColor[] vertices2 = this.vertices;
        int index3 = num5;
        int num6 = index3 + 1;
        vertices2[index3].Position = new Vector3(vector2_3 - vector2_7, 0.0f);
        VertexPositionColor[] vertices3 = this.vertices;
        int index4 = num6;
        int num7 = index4 + 1;
        vertices3[index4].Position = new Vector3(vector2_5 - vector2_7, 0.0f);
        VertexPositionColor[] vertices4 = this.vertices;
        int index5 = num7;
        int num8 = index5 + 1;
        vertices4[index5].Position = new Vector3(vector2_5 - vector2_7, 0.0f);
        VertexPositionColor[] vertices5 = this.vertices;
        int index6 = num8;
        int num9 = index6 + 1;
        vertices5[index6].Position = new Vector3(vector2_3 + vector2_7, 0.0f);
        VertexPositionColor[] vertices6 = this.vertices;
        int index7 = num9;
        num3 = index7 + 1;
        vertices6[index7].Position = new Vector3(vector2_5 + vector2_7, 0.0f);
        for (int index8 = 1; index8 < 10; ++index8)
        {
          float num10 = (float) ((double) scene.TimeActive * (double) this.tentacles[index1].Offset * Math.Pow(1.10000002384186, (double) index8) * 2.0);
          float num11 = (float) ((double) this.tentacles[index1].Offset * 3.0 + (double) index8 * (0.100000001490116 + (double) this.tentacles[index1].Step * 0.200000002980232) + (double) num4 * (double) index8 * 0.100000001490116);
          float num12 = (float) (2.0 + 4.0 * ((double) index8 / 10.0));
          Vector2 vector2_8 = (float) Math.Sin((double) num10 + (double) num11) * vector2_1 * num12;
          float num13 = (float) ((1.0 - (double) index8 / 10.0) * (double) this.tentacles[index1].Width * 0.5);
          Vector2 vector2_9 = vector2_6 + this.outwards * num4 + vector2_8;
          Vector2 vector2_10 = (vector2_6 - vector2_9).SafeNormalize().Perpendicular() * num13;
          VertexPositionColor[] vertices7 = this.vertices;
          int index9 = num3;
          int num14 = index9 + 1;
          vertices7[index9].Position = new Vector3(vector2_6 + vector2_7, 0.0f);
          VertexPositionColor[] vertices8 = this.vertices;
          int index10 = num14;
          int num15 = index10 + 1;
          vertices8[index10].Position = new Vector3(vector2_6 - vector2_7, 0.0f);
          VertexPositionColor[] vertices9 = this.vertices;
          int index11 = num15;
          int num16 = index11 + 1;
          vertices9[index11].Position = new Vector3(vector2_9 - vector2_10, 0.0f);
          VertexPositionColor[] vertices10 = this.vertices;
          int index12 = num16;
          int num17 = index12 + 1;
          vertices10[index12].Position = new Vector3(vector2_9 - vector2_10, 0.0f);
          VertexPositionColor[] vertices11 = this.vertices;
          int index13 = num17;
          int num18 = index13 + 1;
          vertices11[index13].Position = new Vector3(vector2_6 + vector2_7, 0.0f);
          VertexPositionColor[] vertices12 = this.vertices;
          int index14 = num18;
          num3 = index14 + 1;
          vertices12[index14].Position = new Vector3(vector2_9 + vector2_10, 0.0f);
          vector2_6 = vector2_9;
          vector2_7 = vector2_10;
        }
        vector2_2 = vector2_3 + vector2_1 * this.tentacles[index1].Width * 0.5f;
      }
      this.vertexCount = num3;
    }

    public override void Render(Scene scene)
    {
      if (this.vertexCount <= 0)
        return;
      GFX.DrawVertices<VertexPositionColor>(Matrix.Identity, this.vertices, this.vertexCount, (Effect) null, (BlendState) null);
    }

    public enum Side
    {
      Right,
      Left,
      Top,
      Bottom,
    }

    private struct Tentacle
    {
      public float Length;
      public float Offset;
      public float Step;
      public float Position;
      public float Approach;
      public float Width;
    }
  }
}

