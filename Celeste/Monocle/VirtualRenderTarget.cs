// Decompiled with JetBrains decompiler
// Type: Monocle.VirtualRenderTarget
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
  public class VirtualRenderTarget : VirtualAsset
  {
    public RenderTarget2D Target;
    public int MultiSampleCount;

    public bool Depth { get; private set; }

    public bool Preserve { get; private set; }

    public bool IsDisposed
    {
      get
      {
        if (this.Target != null && !((GraphicsResource) this.Target).get_IsDisposed())
          return ((GraphicsResource) this.Target).get_GraphicsDevice().get_IsDisposed();
        return true;
      }
    }

    public Rectangle Bounds
    {
      get
      {
        return ((Texture2D) this.Target).get_Bounds();
      }
    }

    internal VirtualRenderTarget(
      string name,
      int width,
      int height,
      int multiSampleCount,
      bool depth,
      bool preserve)
    {
      this.Name = name;
      this.Width = width;
      this.Height = height;
      this.MultiSampleCount = multiSampleCount;
      this.Depth = depth;
      this.Preserve = preserve;
      this.Reload();
    }

    internal override void Unload()
    {
      if (this.Target == null || ((GraphicsResource) this.Target).get_IsDisposed())
        return;
      ((GraphicsResource) this.Target).Dispose();
    }

    internal override void Reload()
    {
      this.Unload();
      this.Target = new RenderTarget2D(Engine.Instance.get_GraphicsDevice(), this.Width, this.Height, false, (SurfaceFormat) 0, this.Depth ? (DepthFormat) 3 : (DepthFormat) 0, this.MultiSampleCount, this.Preserve ? (RenderTargetUsage) 1 : (RenderTargetUsage) 0);
    }

    public override void Dispose()
    {
      this.Unload();
      this.Target = (RenderTarget2D) null;
      VirtualContent.Remove((VirtualAsset) this);
    }

    public static implicit operator RenderTarget2D(VirtualRenderTarget target)
    {
      return target.Target;
    }
  }
}
