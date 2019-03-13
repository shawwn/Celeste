// Decompiled with JetBrains decompiler
// Type: Celeste.Plateau
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class Plateau : Solid
  {
    private Monocle.Image sprite;
    public LightOcclude Occluder;

    public Plateau(EntityData e, Vector2 offset)
      : base(e.Position + offset, 104f, 4f, true)
    {
      this.Collider.Left += 8f;
      this.Add((Component) (this.sprite = new Monocle.Image(GFX.Game["scenery/fallplateau"])));
      this.Add((Component) (this.Occluder = new LightOcclude(1f)));
      this.SurfaceSoundIndex = 23;
      this.EnableAssistModeChecks = false;
    }
  }
}

