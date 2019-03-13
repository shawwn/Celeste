// Decompiled with JetBrains decompiler
// Type: Celeste.Distort
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
  public static class Distort
  {
    private static float anxiety = 0.0f;
    private static float gamerate = 1f;
    private static float waterSine = 0.0f;
    public static float WaterSineDirection = 1f;
    private static float waterCameraY = 0.0f;
    private static float waterAlpha = 1f;
    private static Vector2 anxietyOrigin;

    public static Vector2 AnxietyOrigin
    {
      get
      {
        return Distort.anxietyOrigin;
      }
      set
      {
        GFX.FxDistort.Parameters["anxietyOrigin"].SetValue(Distort.anxietyOrigin = value);
      }
    }

    public static float Anxiety
    {
      get
      {
        return Distort.anxiety;
      }
      set
      {
        Distort.anxiety = value;
        GFX.FxDistort.Parameters["anxiety"].SetValue(!Settings.Instance.DisableFlashes ? Distort.anxiety : 0.0f);
      }
    }

    public static float GameRate
    {
      get
      {
        return Distort.gamerate;
      }
      set
      {
        GFX.FxDistort.Parameters["gamerate"].SetValue(Distort.gamerate = value);
      }
    }

    public static float WaterSine
    {
      get
      {
        return Distort.waterSine;
      }
      set
      {
        GFX.FxDistort.Parameters["waterSine"].SetValue(Distort.waterSine = Distort.WaterSineDirection * value);
      }
    }

    public static float WaterCameraY
    {
      get
      {
        return Distort.waterCameraY;
      }
      set
      {
        GFX.FxDistort.Parameters["waterCameraY"].SetValue(Distort.waterCameraY = value);
      }
    }

    public static float WaterAlpha
    {
      get
      {
        return Distort.waterAlpha;
      }
      set
      {
        GFX.FxDistort.Parameters["waterAlpha"].SetValue(Distort.waterAlpha = value);
      }
    }

    public static void Render(Texture2D source, Texture2D map, bool hasDistortion)
    {
      Effect fxDistort = GFX.FxDistort;
      if (fxDistort != null && (((double) Distort.anxiety > 0.0 ? 1 : ((double) Distort.gamerate < 1.0 ? 1 : 0)) | (hasDistortion ? 1 : 0)) != 0)
      {
        int num = (double) Distort.anxiety > 0.0 ? 1 : ((double) Distort.gamerate < 1.0 ? 1 : 0);
        fxDistort.CurrentTechnique = num == 0 ? fxDistort.Techniques["Displace"] : fxDistort.Techniques[nameof (Distort)];
        Engine.Graphics.GraphicsDevice.Textures[1] = (Texture) map;
        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, fxDistort);
        Draw.SpriteBatch.Draw(source, Vector2.Zero, Color.White);
        Draw.SpriteBatch.End();
      }
      else
      {
        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, (Effect) null);
        Draw.SpriteBatch.Draw(source, Vector2.Zero, Color.White);
        Draw.SpriteBatch.End();
      }
    }
  }
}

