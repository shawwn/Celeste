// Decompiled with JetBrains decompiler
// Type: Celeste.BladeRotateSpinner
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class BladeRotateSpinner : RotateSpinner
  {
    public Sprite Sprite;

    public BladeRotateSpinner(EntityData data, Vector2 offset)
      : base(data, offset)
    {
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("templeBlade")));
      this.Sprite.Play("idle", false, false);
      this.Depth = -50;
      this.Add((Component) new MirrorReflection());
    }

    public override void Update()
    {
      base.Update();
      if (!this.Scene.OnInterval(1f))
        return;
      this.Sprite.Play("spin", false, false);
    }
  }
}
