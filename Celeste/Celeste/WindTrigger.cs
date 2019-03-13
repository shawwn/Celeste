// Decompiled with JetBrains decompiler
// Type: Celeste.WindTrigger
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  [Tracked(false)]
  public class WindTrigger : Trigger
  {
    public WindController.Patterns Pattern;

    public WindTrigger(EntityData data, Vector2 offset)
      : base(data, offset)
    {
      this.Pattern = data.Enum<WindController.Patterns>("pattern", WindController.Patterns.None);
    }

    public override void OnEnter(Player player)
    {
      base.OnEnter(player);
      WindController first = this.Scene.Entities.FindFirst<WindController>();
      if (first == null)
        this.Scene.Add((Entity) new WindController(this.Pattern));
      else
        first.SetPattern(this.Pattern);
    }
  }
}
