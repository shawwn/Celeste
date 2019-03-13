// Decompiled with JetBrains decompiler
// Type: Monocle.VirtualAsset
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

namespace Monocle
{
  public abstract class VirtualAsset
  {
    public string Name { get; internal set; }

    public int Width { get; internal set; }

    public int Height { get; internal set; }

    internal virtual void Unload()
    {
    }

    internal virtual void Reload()
    {
    }

    public virtual void Dispose()
    {
    }
  }
}
