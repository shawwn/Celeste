// Decompiled with JetBrains decompiler
// Type: Celeste.ScreenWipe
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public abstract class ScreenWipe : Monocle.Renderer
  {
    public static Color WipeColor = Color.Black;
    public float Duration = 0.5f;
    public Scene Scene;
    public bool WipeIn;
    public float Percent;
    public Action OnComplete;
    public bool Completed;
    public float EndTimer;
    private bool ending;
    public const int Left = -10;
    public const int Top = -10;

    public int Right
    {
      get
      {
        return 1930;
      }
    }

    public int Bottom
    {
      get
      {
        return 1090;
      }
    }

    public ScreenWipe(Scene scene, bool wipeIn, Action onComplete = null)
    {
      this.Scene = scene;
      this.WipeIn = wipeIn;
      if (this.Scene is Level)
        (this.Scene as Level).Wipe = this;
      this.Scene.Add((Monocle.Renderer) this);
      this.OnComplete = onComplete;
    }

    public IEnumerator Wait()
    {
      while ((double) this.Percent < 1.0)
        yield return (object) null;
    }

    public override void Update(Scene scene)
    {
      if (!this.Completed)
      {
        if ((double) this.Percent < 1.0)
          this.Percent = Calc.Approach(this.Percent, 1f, Engine.RawDeltaTime / this.Duration);
        else if ((double) this.EndTimer > 0.0)
          this.EndTimer -= Engine.RawDeltaTime;
        else
          this.Completed = true;
      }
      else
      {
        if (this.ending)
          return;
        this.ending = true;
        scene.Remove((Monocle.Renderer) this);
        if (scene is Level && (scene as Level).Wipe == this)
          (scene as Level).Wipe = (ScreenWipe) null;
        if (this.OnComplete != null)
          this.OnComplete();
      }
    }

    public virtual void Cancel()
    {
      this.Scene.Remove((Monocle.Renderer) this);
      if (!(this.Scene is Level))
        return;
      (this.Scene as Level).Wipe = (ScreenWipe) null;
    }

    public static void DrawPrimitives(VertexPositionColor[] vertices)
    {
      GFX.DrawVertices<VertexPositionColor>(Matrix.CreateScale((float) Engine.Graphics.GraphicsDevice.Viewport.Width / 1920f), vertices, vertices.Length, (Effect) null, (BlendState) null);
    }
  }
}

