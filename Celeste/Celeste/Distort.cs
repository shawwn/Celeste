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
        GFX.FxDistort.get_Parameters().get_Item("anxietyOrigin").SetValue(Distort.anxietyOrigin = value);
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
        GFX.FxDistort.get_Parameters().get_Item("anxiety").SetValue(!Settings.Instance.DisableFlashes ? Distort.anxiety : 0.0f);
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
        GFX.FxDistort.get_Parameters().get_Item("gamerate").SetValue(Distort.gamerate = value);
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
        GFX.FxDistort.get_Parameters().get_Item("waterSine").SetValue(Distort.waterSine = Distort.WaterSineDirection * value);
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
        GFX.FxDistort.get_Parameters().get_Item("waterCameraY").SetValue(Distort.waterCameraY = value);
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
        GFX.FxDistort.get_Parameters().get_Item("waterAlpha").SetValue(Distort.waterAlpha = value);
      }
    }

    public static void Render(Texture2D source, Texture2D map, bool hasDistortion)
    {
      Effect fxDistort = GFX.FxDistort;
      if (fxDistort != null && (((double) Distort.anxiety > 0.0 ? 1 : ((double) Distort.gamerate < 1.0 ? 1 : 0)) | (hasDistortion ? 1 : 0)) != 0)
      {
        if ((double) Distort.anxiety > 0.0 || (double) Distort.gamerate < 1.0)
          fxDistort.set_CurrentTechnique(fxDistort.get_Techniques().get_Item(nameof (Distort)));
        else
          fxDistort.set_CurrentTechnique(fxDistort.get_Techniques().get_Item("Displace"));
        Engine.Graphics.get_GraphicsDevice().get_Textures().set_Item(1, (Texture) map);
        Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.PointClamp, (DepthStencilState) DepthStencilState.Default, (RasterizerState) RasterizerState.CullNone, fxDistort);
        Draw.SpriteBatch.Draw(source, Vector2.get_Zero(), Color.get_White());
        Draw.SpriteBatch.End();
      }
      else
      {
        Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.PointClamp, (DepthStencilState) DepthStencilState.Default, (RasterizerState) RasterizerState.CullNone, (Effect) null);
        Draw.SpriteBatch.Draw(source, Vector2.get_Zero(), Color.get_White());
        Draw.SpriteBatch.End();
      }
    }
  }
}
