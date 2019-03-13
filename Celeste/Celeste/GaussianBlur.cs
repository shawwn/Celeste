// Decompiled with JetBrains decompiler
// Type: Celeste.GaussianBlur
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
  public static class GaussianBlur
  {
    private static string[] techniques = new string[3]
    {
      "GaussianBlur3",
      "GaussianBlur5",
      "GaussianBlur9"
    };

    public static Texture2D Blur(
      Texture2D texture,
      VirtualRenderTarget temp,
      VirtualRenderTarget output,
      float fade = 0.0f,
      bool clear = true,
      GaussianBlur.Samples samples = GaussianBlur.Samples.Nine,
      float sampleScale = 1f,
      GaussianBlur.Direction direction = GaussianBlur.Direction.Both,
      float alpha = 1f)
    {
      Effect fxGaussianBlur = GFX.FxGaussianBlur;
      string technique = GaussianBlur.techniques[(int) samples];
      if (fxGaussianBlur == null)
        return texture;
      fxGaussianBlur.set_CurrentTechnique(fxGaussianBlur.get_Techniques().get_Item(technique));
      fxGaussianBlur.get_Parameters().get_Item(nameof (fade)).SetValue(fade);
      fxGaussianBlur.get_Parameters().get_Item("pixel").SetValue(Vector2.op_Multiply(new Vector2(1f / (float) temp.Width, 0.0f), sampleScale));
      Engine.Instance.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) temp);
      if (clear)
        Engine.Instance.get_GraphicsDevice().Clear(Color.get_Transparent());
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.PointClamp, (DepthStencilState) DepthStencilState.Default, (RasterizerState) RasterizerState.CullNone, direction != GaussianBlur.Direction.Vertical ? fxGaussianBlur : (Effect) null);
      Draw.SpriteBatch.Draw(texture, new Rectangle(0, 0, temp.Width, temp.Height), Color.get_White());
      Draw.SpriteBatch.End();
      fxGaussianBlur.get_Parameters().get_Item("pixel").SetValue(Vector2.op_Multiply(new Vector2(0.0f, 1f / (float) output.Height), sampleScale));
      Engine.Instance.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) output);
      if (clear)
        Engine.Instance.get_GraphicsDevice().Clear(Color.get_Transparent());
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.PointClamp, (DepthStencilState) DepthStencilState.Default, (RasterizerState) RasterizerState.CullNone, direction != GaussianBlur.Direction.Horizontal ? fxGaussianBlur : (Effect) null);
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) temp, new Rectangle(0, 0, output.Width, output.Height), Color.get_White());
      Draw.SpriteBatch.End();
      return (Texture2D) (RenderTarget2D) output;
    }

    public enum Samples
    {
      Three,
      Five,
      Nine,
    }

    public enum Direction
    {
      Both,
      Horizontal,
      Vertical,
    }
  }
}
