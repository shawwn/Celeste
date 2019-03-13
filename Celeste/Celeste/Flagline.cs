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
    private float waveTimer = 0.0f;
    public float ClothDroopAmount = 0.6f;
    private Color[] colors;
    private Color[] highlights;
    private Color lineColor;
    private Color pinColor;
    private Flagline.Cloth[] clothes;
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
        this.highlights[index] = Color.Lerp(colors[index], Color.White, 0.1f);
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
      Vector2 begin = (double) this.From.X < (double) this.To.X ? this.From : this.To;
      Vector2 end = (double) this.From.X < (double) this.To.X ? this.To : this.From;
      float num1 = (begin - end).Length();
      float num2 = num1 / 8f;
      SimpleCurve simpleCurve1 = new SimpleCurve(begin, end, (end + begin) / 2f + Vector2.UnitY * (num2 + (float) (Math.Sin((double) this.waveTimer) * (double) num2 * 0.300000011920929)));
      Vector2 vector2_1 = begin;
      float percent = 0.0f;
      int num3 = 0;
      bool flag = false;
      while ((double) percent < 1.0)
      {
        Flagline.Cloth clothe = this.clothes[num3 % this.clothes.Length];
        percent += (flag ? (float) clothe.Length : (float) clothe.Step) / num1;
        Vector2 point1 = simpleCurve1.GetPoint(percent);
        Draw.Line(vector2_1, point1, this.lineColor);
        if ((double) percent < 1.0 & flag)
        {
          float num4 = (float) clothe.Length * this.ClothDroopAmount;
          SimpleCurve simpleCurve2 = new SimpleCurve(vector2_1, point1, (vector2_1 + point1) / 2f + new Vector2(0.0f, num4 + (float) (Math.Sin((double) this.waveTimer * 2.0 + (double) percent) * (double) num4 * 0.400000005960464)));
          Vector2 vector2_2 = vector2_1;
          for (float num5 = 1f; (double) num5 <= (double) clothe.Length; ++num5)
          {
            Vector2 point2 = simpleCurve2.GetPoint(num5 / (float) clothe.Length);
            if ((double) point2.X != (double) vector2_2.X)
            {
              Draw.Rect(vector2_2.X, vector2_2.Y, (float) ((double) point2.X - (double) vector2_2.X + 1.0), (float) clothe.Height, this.colors[clothe.Color]);
              vector2_2 = point2;
            }
          }
          Draw.Rect(vector2_1.X, vector2_1.Y, 1f, (float) clothe.Height, this.highlights[clothe.Color]);
          Draw.Rect(point1.X, point1.Y, 1f, (float) clothe.Height, this.highlights[clothe.Color]);
          Draw.Rect(vector2_1.X, vector2_1.Y - 1f, 1f, 3f, this.pinColor);
          Draw.Rect(point1.X, point1.Y - 1f, 1f, 3f, this.pinColor);
          ++num3;
        }
        vector2_1 = point1;
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

