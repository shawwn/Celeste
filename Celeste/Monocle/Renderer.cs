// Decompiled with JetBrains decompiler
// Type: Monocle.Renderer
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

namespace Monocle
{
  public abstract class Renderer
  {
    public bool Visible = true;

    public virtual void Update(Scene scene)
    {
    }

    public virtual void BeforeRender(Scene scene)
    {
    }

    public virtual void Render(Scene scene)
    {
    }

    public virtual void AfterRender(Scene scene)
    {
    }
  }
}
