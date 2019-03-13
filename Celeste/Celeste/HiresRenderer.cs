// Decompiled with JetBrains decompiler
// Type: Celeste.HiresRenderer
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
  public class HiresRenderer : Monocle.Renderer
  {
    public static VirtualRenderTarget Buffer
    {
      get
      {
        return Celeste.Celeste.HudTarget;
      }
    }

    public static bool DrawToBuffer
    {
      get
      {
        if (HiresRenderer.Buffer == null)
          return false;
        if (Engine.ViewWidth >= 1920)
          return Engine.ViewHeight < 1080;
        return true;
      }
    }

    public static void BeginRender(BlendState blend = null, SamplerState sampler = null)
    {
      if (blend == null)
        blend = (BlendState) BlendState.AlphaBlend;
      if (sampler == null)
        sampler = (SamplerState) SamplerState.LinearClamp;
      Matrix matrix = HiresRenderer.DrawToBuffer ? Matrix.get_Identity() : Engine.ScreenMatrix;
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, blend, sampler, (DepthStencilState) DepthStencilState.Default, (RasterizerState) RasterizerState.CullNone, (Effect) null, matrix);
    }

    public static void EndRender()
    {
      Draw.SpriteBatch.End();
    }

    public override void BeforeRender(Scene scene)
    {
      if (!HiresRenderer.DrawToBuffer)
        return;
      Engine.Graphics.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) HiresRenderer.Buffer);
      Engine.Graphics.get_GraphicsDevice().Clear(Color.get_Transparent());
      this.RenderContent(scene);
    }

    public override void Render(Scene scene)
    {
      if (HiresRenderer.DrawToBuffer)
      {
        Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.LinearClamp, (DepthStencilState) DepthStencilState.Default, (RasterizerState) RasterizerState.CullNone, (Effect) null, Engine.ScreenMatrix);
        Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) HiresRenderer.Buffer, new Vector2(-1f, -1f), Color.get_White());
        Draw.SpriteBatch.End();
      }
      else
        this.RenderContent(scene);
    }

    public virtual void RenderContent(Scene scene)
    {
    }
  }
}
