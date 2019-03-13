// Decompiled with JetBrains decompiler
// Type: Celeste.ColorGrade
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
  public static class ColorGrade
  {
    public static bool Enabled = true;
    private static float percent = 0.0f;
    private static MTexture from;
    private static MTexture to;

    public static Effect Effect
    {
      get
      {
        return GFX.FxColorGrading;
      }
    }

    public static void Set(MTexture grade)
    {
      ColorGrade.Set(grade, grade, 0.0f);
    }

    public static void Set(MTexture fromTex, MTexture toTex, float p)
    {
      if (!ColorGrade.Enabled || fromTex == null || toTex == null)
      {
        ColorGrade.from = GFX.ColorGrades["none"];
        ColorGrade.to = GFX.ColorGrades["none"];
      }
      else
      {
        ColorGrade.from = fromTex;
        ColorGrade.to = toTex;
      }
      ColorGrade.percent = Calc.Clamp(p, 0.0f, 1f);
      if (ColorGrade.from == ColorGrade.to || (double) ColorGrade.percent <= 0.0)
      {
        ColorGrade.Effect.CurrentTechnique = ColorGrade.Effect.Techniques["ColorGradeSingle"];
        Engine.Graphics.GraphicsDevice.Textures[1] = (Texture) ColorGrade.from.Texture.Texture;
      }
      else if ((double) ColorGrade.percent >= 1.0)
      {
        ColorGrade.Effect.CurrentTechnique = ColorGrade.Effect.Techniques["ColorGradeSingle"];
        Engine.Graphics.GraphicsDevice.Textures[1] = (Texture) ColorGrade.to.Texture.Texture;
      }
      else
      {
        ColorGrade.Effect.CurrentTechnique = ColorGrade.Effect.Techniques[nameof (ColorGrade)];
        ColorGrade.Effect.Parameters["percent"].SetValue(ColorGrade.percent);
        Engine.Graphics.GraphicsDevice.Textures[1] = (Texture) ColorGrade.from.Texture.Texture;
        Engine.Graphics.GraphicsDevice.Textures[2] = (Texture) ColorGrade.to.Texture.Texture;
      }
    }

    public static float Percent
    {
      get
      {
        return ColorGrade.percent;
      }
      set
      {
        ColorGrade.Set(ColorGrade.from, ColorGrade.to, value);
      }
    }
  }
}

