// Decompiled with JetBrains decompiler
// Type: Celeste.Billboard
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(true)]
  public class Billboard : Component
  {
    public Color Color = Color.White;
    public Vector2 Size = Vector2.One;
    public Vector2 Scale = Vector2.One;
    public MTexture Texture;
    public Vector3 Position;
    public Action BeforeRender;

    public Billboard(
      MTexture texture,
      Vector3 position,
      Vector2? size = null,
      Color? color = null,
      Vector2? scale = null)
      : base(true, true)
    {
      this.Texture = texture;
      this.Position = position;
      this.Size = size.HasValue ? size.Value : Vector2.One;
      this.Color = color.HasValue ? color.Value : Color.White;
      this.Scale = scale.HasValue ? scale.Value : Vector2.One;
    }
  }
}

