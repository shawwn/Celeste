// Decompiled with JetBrains decompiler
// Type: Celeste.MagicGlow
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
  public static class MagicGlow
  {
    public static void Render(Texture2D texture, float noiseEase, float direction, Matrix matrix)
    {
      GFX.FxMagicGlow.get_Parameters().get_Item("alpha").SetValue(0.5f);
      GFX.FxMagicGlow.get_Parameters().get_Item("pixel").SetValue(Vector2.op_Multiply(new Vector2(1f / (float) texture.get_Width(), 1f / (float) texture.get_Height()), 3f));
      GFX.FxMagicGlow.get_Parameters().get_Item("noiseSample").SetValue(new Vector2(1f, 0.5f));
      GFX.FxMagicGlow.get_Parameters().get_Item("noiseDistort").SetValue(new Vector2(1f, 1f));
      GFX.FxMagicGlow.get_Parameters().get_Item(nameof (noiseEase)).SetValue(noiseEase * 0.05f);
      GFX.FxMagicGlow.get_Parameters().get_Item(nameof (direction)).SetValue(-direction);
      Engine.Graphics.get_GraphicsDevice().get_Textures().set_Item(1, (Texture) GFX.MagicGlowNoise.Texture);
      Engine.Graphics.get_GraphicsDevice().get_SamplerStates().set_Item(1, (SamplerState) SamplerState.LinearWrap);
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) null, GFX.FxMagicGlow, matrix);
      Draw.SpriteBatch.Draw(texture, Vector2.get_Zero(), Color.get_White());
      Draw.SpriteBatch.End();
    }
  }
}
