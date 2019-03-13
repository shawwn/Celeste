// Decompiled with JetBrains decompiler
// Type: Celeste.Flagline
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class Flagline : Component
  {
    public float ClothDroopAmount = 0.6f;
    private Color[] colors;
    private Color[] highlights;
    private Color lineColor;
    private Color pinColor;
    private Flagline.Cloth[] clothes;
    private float waveTimer;
    public Vector2 To;

    public Vector2 From
    {
      get
      {
        return this.Entity.Position;
      }
    }

    public Flagline(
      Vector2 to,
      Color lineColor,
      Color pinColor,
      Color[] colors,
      int minFlagHeight,
      int maxFlagHeight,
      int minFlagLength,
      int maxFlagLength,
      int minSpace,
      int maxSpace)
      : base(true, true)
    {
      this.To = to;
      this.colors = colors;
      this.lineColor = lineColor;
      this.pinColor = pinColor;
      this.waveTimer = Calc.Random.NextFloat() * 6.283185f;
      this.highlights = new Color[colors.Length];
      for (int index = 0; index < colors.Length; ++index)
        this.highlights[index] = Color.Lerp(colors[index], Color.get_White(), 0.1f);
      this.clothes = new Flagline.Cloth[10];
      for (int index = 0; index < this.clothes.Length; ++index)
        this.clothes[index] = new Flagline.Cloth()
        {
          Color = Calc.Random.Next(colors.Length),
          Height = Calc.Random.Next(minFlagHeight, maxFlagHeight),
          Length = Calc.Random.Next(minFlagLength, maxFlagLength),
          Step = Calc.Random.Next(minSpace, maxSpace)
        };
    }

    public override void Update()
    {
      this.waveTimer += Engine.DeltaTime;
      base.Update();
    }

    public override void Render()
    {
      Vector2 begin = this.From.X < this.To.X ? this.From : this.To;
      Vector2 end = this.From.X < this.To.X ? this.To : this.From;
      Vector2 vector2_1 = Vector2.op_Subtraction(begin, end);
      float num1 = ((Vector2) ref vector2_1).Length();
      float num2 = num1 / 8f;
      SimpleCurve simpleCurve1 = new SimpleCurve(begin, end, Vector2.op_Addition(Vector2.op_Division(Vector2.op_Addition(end, begin), 2f), Vector2.op_Multiply(Vector2.get_UnitY(), num2 + (float) (Math.Sin((double) this.waveTimer) * (double) num2 * 0.300000011920929))));
      Vector2 vector2_2 = begin;
      float percent = 0.0f;
      int num3 = 0;
      bool flag = false;
      while ((double) percent < 1.0)
      {
        Flagline.Cloth clothe = this.clothes[num3 % this.clothes.Length];
        percent += (flag ? (float) clothe.Length : (float) clothe.Step) / num1;
        Vector2 point1 = simpleCurve1.GetPoint(percent);
        Draw.Line(vector2_2, point1, this.lineColor);
        if ((double) percent < 1.0 & flag)
        {
          float num4 = (float) clothe.Length * this.ClothDroopAmount;
          SimpleCurve simpleCurve2 = new SimpleCurve(vector2_2, point1, Vector2.op_Addition(Vector2.op_Division(Vector2.op_Addition(vector2_2, point1), 2f), new Vector2(0.0f, num4 + (float) (Math.Sin((double) this.waveTimer * 2.0 + (double) percent) * (double) num4 * 0.400000005960464))));
          Vector2 vector2_3 = vector2_2;
          for (float num5 = 1f; (double) num5 <= (double) clothe.Length; ++num5)
          {
            Vector2 point2 = simpleCurve2.GetPoint(num5 / (float) clothe.Length);
            if (point2.X != vector2_3.X)
            {
              Draw.Rect((float) vector2_3.X, (float) vector2_3.Y, (float) (point2.X - vector2_3.X + 1.0), (float) clothe.Height, this.colors[clothe.Color]);
              vector2_3 = point2;
            }
          }
          Draw.Rect((float) vector2_2.X, (float) vector2_2.Y, 1f, (float) clothe.Height, this.highlights[clothe.Color]);
          Draw.Rect((float) point1.X, (float) point1.Y, 1f, (float) clothe.Height, this.highlights[clothe.Color]);
          Draw.Rect((float) vector2_2.X, (float) (vector2_2.Y - 1.0), 1f, 3f, this.pinColor);
          Draw.Rect((float) point1.X, (float) (point1.Y - 1.0), 1f, 3f, this.pinColor);
          ++num3;
        }
        vector2_2 = point1;
        flag = !flag;
      }
    }

    private struct Cloth
    {
      public int Color;
      public int Height;
      public int Length;
      public int Step;
    }
  }
}
