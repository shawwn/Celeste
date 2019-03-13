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
        return this.Target == null || this.Target.IsDisposed || this.Target.GraphicsDevice.IsDisposed;
      }
    }

    public Rectangle Bounds
    {
      get
      {
        return this.Target.Bounds;
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
      if (this.Target == null || this.Target.IsDisposed)
        return;
      this.Target.Dispose();
    }

    internal override void Reload()
    {
      this.Unload();
      this.Target = new RenderTarget2D(Engine.Instance.GraphicsDevice, this.Width, this.Height, false, SurfaceFormat.Color, this.Depth ? DepthFormat.Depth24Stencil8 : DepthFormat.None, this.MultiSampleCount, this.Preserve ? RenderTargetUsage.PreserveContents : RenderTargetUsage.DiscardContents);
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

