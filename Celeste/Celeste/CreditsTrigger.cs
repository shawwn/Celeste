// Decompiled with JetBrains decompiler
// Type: Celeste.CreditsTrigger
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  [Tracked(false)]
  public class CreditsTrigger : Trigger
  {
    public string Event;

    public CreditsTrigger(EntityData data, Vector2 offset)
      : base(data, offset)
    {
      this.Event = data.Attr("event", "");
    }

    public override void OnEnter(Player player)
    {
      this.Triggered = true;
      if (CS07_Credits.Instance == null)
        return;
      CS07_Credits.Instance.Event = this.Event;
    }
  }
}
