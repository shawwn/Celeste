// Decompiled with JetBrains decompiler
// Type: Celeste.HudRenderer
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
  public class HudRenderer : HiresRenderer
  {
    public float BackgroundFade = 0.0f;

    public override void RenderContent(Scene scene)
    {
      if (!scene.Entities.HasVisibleEntities((int) Tags.HUD) && (double) this.BackgroundFade <= 0.0)
        return;
      HiresRenderer.BeginRender((BlendState) null, (SamplerState) null);
      if ((double) this.BackgroundFade > 0.0)
        Draw.Rect(-1f, -1f, 1922f, 1082f, Color.Black * this.BackgroundFade * 0.7f);
      scene.Entities.RenderOnly((int) Tags.HUD);
      HiresRenderer.EndRender();
    }
  }
}

