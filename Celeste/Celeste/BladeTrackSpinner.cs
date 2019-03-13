// Decompiled with JetBrains decompiler
// Type: Celeste.BladeTrackSpinner
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class BladeTrackSpinner : TrackSpinner
  {
    public Sprite Sprite;
    private bool hasStarted;

    public BladeTrackSpinner(EntityData data, Vector2 offset)
      : base(data, offset)
    {
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("templeBlade")));
      this.Sprite.Play("idle", false, false);
      this.Depth = -50;
      this.Add((Component) new MirrorReflection());
    }

    public override void OnTrackStart()
    {
      this.Sprite.Play("spin", false, false);
      if (this.hasStarted)
        Audio.Play("event:/game/05_mirror_temple/bladespinner_spin", this.Position);
      this.hasStarted = true;
    }
  }
}
