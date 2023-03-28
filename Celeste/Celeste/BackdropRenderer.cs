// Decompiled with JetBrains decompiler
// Type: Celeste.BackdropRenderer
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  public class BackdropRenderer : Monocle.Renderer
  {
    public Matrix Matrix = Matrix.Identity;
    public List<Backdrop> Backdrops = new List<Backdrop>();
    public float Fade;
    public Color FadeColor = Color.Black;
    private bool usingSpritebatch;

    public override void BeforeRender(Scene scene)
    {
      foreach (Backdrop backdrop in this.Backdrops)
        backdrop.BeforeRender(scene);
    }

    public override void Update(Scene scene)
    {
      foreach (Backdrop backdrop in this.Backdrops)
        backdrop.Update(scene);
    }

    public void Ended(Scene scene)
    {
      foreach (Backdrop backdrop in this.Backdrops)
        backdrop.Ended(scene);
    }

    public T Get<T>() where T : class
    {
      foreach (Backdrop backdrop in this.Backdrops)
      {
        if (backdrop is T)
          return backdrop as T;
      }
      return default (T);
    }

    public IEnumerable<T> GetEach<T>() where T : class
    {
      foreach (Backdrop backdrop in this.Backdrops)
      {
        if (backdrop is T)
          yield return backdrop as T;
      }
    }

    public IEnumerable<T> GetEach<T>(string tag) where T : class
    {
      foreach (Backdrop backdrop in this.Backdrops)
      {
        if (backdrop is T && backdrop.Tags.Contains(tag))
          yield return backdrop as T;
      }
    }

    public void StartSpritebatch(BlendState blendState)
    {
      if (!this.usingSpritebatch)
        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, blendState, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, (Effect) null, this.Matrix);
      this.usingSpritebatch = true;
    }

    public void EndSpritebatch()
    {
      if (this.usingSpritebatch)
        Draw.SpriteBatch.End();
      this.usingSpritebatch = false;
    }

    public override void Render(Scene scene)
    {
      BlendState blendState = BlendState.AlphaBlend;
      foreach (Backdrop backdrop in this.Backdrops)
      {
        if (backdrop.Visible)
        {
          if (backdrop is Parallax && (backdrop as Parallax).BlendState != blendState)
          {
            this.EndSpritebatch();
            blendState = (backdrop as Parallax).BlendState;
          }
          if (backdrop.UseSpritebatch && !this.usingSpritebatch)
            this.StartSpritebatch(blendState);
          if (!backdrop.UseSpritebatch && this.usingSpritebatch)
            this.EndSpritebatch();
          backdrop.Render(scene);
        }
      }
      if ((double) this.Fade > 0.0)
        Draw.Rect(-10f, -10f, 340f, 200f, this.FadeColor * this.Fade);
      this.EndSpritebatch();
    }

    public void Remove<T>() where T : Backdrop
    {
      for (int index = this.Backdrops.Count - 1; index >= 0; --index)
      {
        if (this.Backdrops[index].GetType() == typeof (T))
          this.Backdrops.RemoveAt(index);
      }
    }
  }
}
