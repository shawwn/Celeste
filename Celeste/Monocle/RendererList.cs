// Decompiled with JetBrains decompiler
// Type: Monocle.RendererList
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System.Collections.Generic;

namespace Monocle
{
  public class RendererList
  {
    public List<Renderer> Renderers;
    private List<Renderer> adding;
    private List<Renderer> removing;
    private Scene scene;

    internal RendererList(Scene scene)
    {
      this.scene = scene;
      this.Renderers = new List<Renderer>();
      this.adding = new List<Renderer>();
      this.removing = new List<Renderer>();
    }

    internal void UpdateLists()
    {
      if (this.adding.Count > 0)
      {
        foreach (Renderer renderer in this.adding)
          this.Renderers.Add(renderer);
      }
      this.adding.Clear();
      if (this.removing.Count > 0)
      {
        foreach (Renderer renderer in this.removing)
          this.Renderers.Remove(renderer);
      }
      this.removing.Clear();
    }

    internal void Update()
    {
      foreach (Renderer renderer in this.Renderers)
        renderer.Update(this.scene);
    }

    internal void BeforeRender()
    {
      for (int index = 0; index < this.Renderers.Count; ++index)
      {
        if (this.Renderers[index].Visible)
        {
          Draw.Renderer = this.Renderers[index];
          this.Renderers[index].BeforeRender(this.scene);
        }
      }
    }

    internal void Render()
    {
      for (int index = 0; index < this.Renderers.Count; ++index)
      {
        if (this.Renderers[index].Visible)
        {
          Draw.Renderer = this.Renderers[index];
          this.Renderers[index].Render(this.scene);
        }
      }
    }

    internal void AfterRender()
    {
      for (int index = 0; index < this.Renderers.Count; ++index)
      {
        if (this.Renderers[index].Visible)
        {
          Draw.Renderer = this.Renderers[index];
          this.Renderers[index].AfterRender(this.scene);
        }
      }
    }

    public void MoveToFront(Renderer renderer)
    {
      this.Renderers.Remove(renderer);
      this.Renderers.Add(renderer);
    }

    public void Add(Renderer renderer)
    {
      this.adding.Add(renderer);
    }

    public void Remove(Renderer renderer)
    {
      this.removing.Add(renderer);
    }
  }
}
