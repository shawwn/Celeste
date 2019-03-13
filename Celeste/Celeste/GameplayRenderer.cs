// Decompiled with JetBrains decompiler
// Type: Celeste.GameplayRenderer
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
  public class GameplayRenderer : Monocle.Renderer
  {
    public Camera Camera;
    private static GameplayRenderer instance;

    public GameplayRenderer()
    {
      GameplayRenderer.instance = this;
      this.Camera = new Camera(320, 180);
    }

    public static void Begin()
    {
      Draw.SpriteBatch.Begin((SpriteSortMode) 0, (BlendState) BlendState.AlphaBlend, (SamplerState) SamplerState.PointClamp, (DepthStencilState) DepthStencilState.None, (RasterizerState) RasterizerState.CullNone, (Effect) null, GameplayRenderer.instance.Camera.Matrix);
    }

    public override void Render(Scene scene)
    {
      GameplayRenderer.Begin();
      scene.Entities.RenderExcept((int) Tags.HUD);
      if (Engine.Commands.Open)
        scene.Entities.DebugRender(this.Camera);
      GameplayRenderer.End();
    }

    public static void End()
    {
      Draw.SpriteBatch.End();
    }
  }
}
