// Decompiled with JetBrains decompiler
// Type: Celeste.Glitch
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
  public static class Glitch
  {
    public static float Value;

    public static void Apply(VirtualRenderTarget source, float timer, float seed, float amplitude)
    {
      if ((double) Glitch.Value <= 0.0 || Settings.Instance.DisableFlashes)
        return;
      Effect fxGlitch = GFX.FxGlitch;
      Vector2 vector2;
      ref Vector2 local = ref vector2;
      Viewport viewport = Engine.Graphics.get_GraphicsDevice().get_Viewport();
      double width = (double) ((Viewport) ref viewport).get_Width();
      viewport = Engine.Graphics.get_GraphicsDevice().get_Viewport();
      double height = (double) ((Viewport) ref viewport).get_Height();
      ((Vector2) ref local).\u002Ector((float) width, (float) height);
      fxGlitch.get_Parameters().get_Item("dimensions").SetValue(vector2);
      fxGlitch.get_Parameters().get_Item(nameof (amplitude)).SetValue(amplitude);
      fxGlitch.get_Parameters().get_Item("minimum").SetValue(-1f);
      fxGlitch.get_Parameters().get_Item("glitch").SetValue(Glitch.Value);
      fxGlitch.get_Parameters().get_Item(nameof (timer)).SetValue(timer);
      fxGlitch.get_Parameters().get_Item(nameof (seed)).SetValue(seed);
      VirtualRenderTarget tempA = GameplayBuffers.TempA;
      Engine.Instance.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) tempA);
      Engine.Instance.get_GraphicsDevice().Clear(Color.get_Transparent());
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.PointClamp, (DepthStencilState) DepthStencilState.Default, (RasterizerState) RasterizerState.CullNone, fxGlitch);
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) source, Vector2.get_Zero(), Color.get_White());
      Draw.SpriteBatch.End();
      Engine.Instance.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) source);
      Engine.Instance.get_GraphicsDevice().Clear(Color.get_Transparent());
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.PointClamp, (DepthStencilState) DepthStencilState.Default, (RasterizerState) RasterizerState.CullNone, fxGlitch);
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) tempA, Vector2.get_Zero(), Color.get_White());
      Draw.SpriteBatch.End();
    }
  }
}
