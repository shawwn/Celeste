// Decompiled with JetBrains decompiler
// Type: Celeste.Overlay
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste
{
  public class Overlay : Entity
  {
    public float Fade;
    public bool XboxOverlay;

    public Overlay()
    {
      this.Tag = (int) Tags.HUD;
      this.Depth = -100000;
    }

    public override void Added(Scene scene)
    {
      IOverlayHandler overlayHandler = scene as IOverlayHandler;
      if (overlayHandler != null)
        overlayHandler.Overlay = this;
      base.Added(scene);
    }

    public override void Removed(Scene scene)
    {
      IOverlayHandler overlayHandler = scene as IOverlayHandler;
      if (overlayHandler != null && overlayHandler.Overlay == this)
        overlayHandler.Overlay = (Overlay) null;
      base.Removed(scene);
    }

    public IEnumerator FadeIn()
    {
      for (; (double) this.Fade < 1.0; this.Fade += Engine.DeltaTime * 4f)
        yield return (object) null;
      this.Fade = 1f;
    }

    public IEnumerator FadeOut()
    {
      for (; (double) this.Fade > 0.0; this.Fade -= Engine.DeltaTime * 4f)
        yield return (object) null;
    }

    public void RenderFade()
    {
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * Ease.CubeInOut(this.Fade) * 0.95f);
    }
  }
}

