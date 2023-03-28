// Decompiled with JetBrains decompiler
// Type: Celeste.Starfield
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class Starfield : Backdrop
  {
    public const int StepSize = 32;
    public const int Steps = 15;
    public const float MinDist = 4f;
    public const float MaxDist = 24f;
    public float FlowSpeed;
    public List<float> YNodes = new List<float>();
    public Starfield.Star[] Stars = new Starfield.Star[128];

    public Starfield(Color color, float speed = 1f)
    {
      this.Color = color;
      this.FlowSpeed = speed;
      float num1 = Calc.Random.NextFloat(180f);
      int num2 = 0;
      while (num2 < 15)
      {
        this.YNodes.Add(num1);
        ++num2;
        num1 += (float) Calc.Random.Choose<int>(-1, 1) * (16f + Calc.Random.NextFloat(24f));
      }
      for (int index = 0; index < 4; ++index)
        this.YNodes[this.YNodes.Count - 1 - index] = Calc.LerpClamp(this.YNodes[this.YNodes.Count - 1 - index], this.YNodes[0], (float) (1.0 - (double) index / 4.0));
      List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("particles/starfield/");
      for (int index1 = 0; index1 < this.Stars.Length; ++index1)
      {
        float num3 = Calc.Random.NextFloat(1f);
        this.Stars[index1].NodeIndex = Calc.Random.Next(this.YNodes.Count - 1);
        this.Stars[index1].NodePercent = Calc.Random.NextFloat(1f);
        this.Stars[index1].Distance = (float) (4.0 + (double) num3 * 20.0);
        this.Stars[index1].Sine = Calc.Random.NextFloat(6.2831855f);
        this.Stars[index1].Position = this.GetTargetOfStar(ref this.Stars[index1]);
        this.Stars[index1].Color = Color.Lerp(this.Color, Color.Transparent, num3 * 0.5f);
        int index2 = (int) Calc.Clamp(Ease.CubeIn(1f - num3) * (float) atlasSubtextures.Count, 0.0f, (float) (atlasSubtextures.Count - 1));
        this.Stars[index1].Texture = atlasSubtextures[index2];
      }
    }

    public override void Update(Scene scene)
    {
      base.Update(scene);
      for (int index = 0; index < this.Stars.Length; ++index)
        this.UpdateStar(ref this.Stars[index]);
    }

    private void UpdateStar(ref Starfield.Star star)
    {
      star.Sine += Engine.DeltaTime * this.FlowSpeed;
      star.NodePercent += Engine.DeltaTime * 0.25f * this.FlowSpeed;
      if ((double) star.NodePercent >= 1.0)
      {
        --star.NodePercent;
        ++star.NodeIndex;
        if (star.NodeIndex >= this.YNodes.Count - 1)
        {
          star.NodeIndex = 0;
          star.Position.X -= 448f;
        }
      }
      star.Position += (this.GetTargetOfStar(ref star) - star.Position) / 50f;
    }

    private Vector2 GetTargetOfStar(ref Starfield.Star star)
    {
      Vector2 vector2_1 = new Vector2((float) (star.NodeIndex * 32), this.YNodes[star.NodeIndex]);
      Vector2 vector2_2 = new Vector2((float) ((star.NodeIndex + 1) * 32), this.YNodes[star.NodeIndex + 1]);
      Vector2 vector2_3 = vector2_1 + (vector2_2 - vector2_1) * star.NodePercent;
      Vector2 vector2_4 = (vector2_2 - vector2_1).SafeNormalize();
      Vector2 vector2_5 = new Vector2(-vector2_4.Y, vector2_4.X) * star.Distance * (float) Math.Sin((double) star.Sine);
      return vector2_3 + vector2_5;
    }

    public override void Render(Scene scene)
    {
      Vector2 position1 = (scene as Level).Camera.Position;
      for (int index = 0; index < this.Stars.Length; ++index)
      {
        Vector2 position2 = new Vector2()
        {
          X = this.Mod(this.Stars[index].Position.X - position1.X * this.Scroll.X, 448f) - 64f,
          Y = this.Mod(this.Stars[index].Position.Y - position1.Y * this.Scroll.Y, 212f) - 16f
        };
        this.Stars[index].Texture.DrawCentered(position2, this.Stars[index].Color * this.FadeAlphaMultiplier);
      }
    }

    private float Mod(float x, float m) => (x % m + m) % m;

    public struct Star
    {
      public MTexture Texture;
      public Vector2 Position;
      public Color Color;
      public int NodeIndex;
      public float NodePercent;
      public float Distance;
      public float Sine;
    }
  }
}
