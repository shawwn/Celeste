// Decompiled with JetBrains decompiler
// Type: Celeste.MirrorSurface
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class MirrorSurface : Component
  {
    public Action OnRender;
    private Vector2 reflectionOffset;

    public Vector2 ReflectionOffset
    {
      get
      {
        return this.reflectionOffset;
      }
      set
      {
        this.reflectionOffset = value;
        this.ReflectionColor = new Color((float) (0.5 + (double) Calc.Clamp((float) (this.reflectionOffset.X / 32.0), -1f, 1f) * 0.5), (float) (0.5 + (double) Calc.Clamp((float) (this.reflectionOffset.Y / 32.0), -1f, 1f) * 0.5), 0.0f, 1f);
      }
    }

    public Color ReflectionColor { get; private set; }

    public MirrorSurface(Action onRender = null)
      : base(false, true)
    {
      this.OnRender = onRender;
    }
  }
}
