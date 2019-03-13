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
    public static float Value = 0.0f;

    public static void Apply(VirtualRenderTarget source, float timer, float seed, float amplitude)
    {
      if ((double) Glitch.Value <= 0.0 || Settings.Instance.DisableFlashes)
        return;
      Effect fxGlitch = GFX.FxGlitch;
      Vector2 vector2 = new Vector2();
      ref Vector2 local = ref vector2;
      Viewport viewport = Engine.Graphics.GraphicsDevice.Viewport;
      double width = (double) viewport.Width;
      viewport = Engine.Graphics.GraphicsDevice.Viewport;
      double height = (double) viewport.Height;
      local = new Vector2((float) width, (float) height);
      fxGlitch.Parameters["dimensions"].SetValue(vector2);
      fxGlitch.Parameters[nameof (amplitude)].SetValue(amplitude);
      fxGlitch.Parameters["minimum"].SetValue(-1f);
      fxGlitch.Parameters["glitch"].SetValue(Glitch.Value);
      fxGlitch.Parameters[nameof (timer)].SetValue(timer);
      fxGlitch.Parameters[nameof (seed)].SetValue(seed);
      VirtualRenderTarget tempA = GameplayBuffers.TempA;
      Engine.Instance.GraphicsDevice.SetRenderTarget((RenderTarget2D) tempA);
      Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, fxGlitch);
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) source, Vector2.Zero, Color.White);
      Draw.SpriteBatch.End();
      Engine.Instance.GraphicsDevice.SetRenderTarget((RenderTarget2D) source);
      Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, fxGlitch);
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) tempA, Vector2.Zero, Color.White);
      Draw.SpriteBatch.End();
    }
  }
}

