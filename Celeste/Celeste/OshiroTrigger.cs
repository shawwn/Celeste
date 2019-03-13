// Decompiled with JetBrains decompiler
// Type: Celeste.OshiroTrigger
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class OshiroTrigger : Trigger
  {
    public bool State;

    public OshiroTrigger(EntityData data, Vector2 offset)
      : base(data, offset)
    {
      this.State = data.Bool("state", true);
    }

    public override void OnEnter(Player player)
    {
      base.OnEnter(player);
      if (this.State)
      {
        Level level = this.SceneAs<Level>();
        Vector2 position;
        ref Vector2 local = ref position;
        Rectangle bounds1 = level.Bounds;
        double num1 = (double) (((Rectangle) ref bounds1).get_Left() - 32);
        Rectangle bounds2 = level.Bounds;
        double num2 = (double) (((Rectangle) ref bounds2).get_Top() + level.Bounds.Height / 2);
        ((Vector2) ref local).\u002Ector((float) num1, (float) num2);
        this.Scene.Add((Entity) new AngryOshiro(position, false));
        this.RemoveSelf();
      }
      else
      {
        this.Scene.Tracker.GetEntity<AngryOshiro>()?.Leave();
        this.RemoveSelf();
      }
    }
  }
}
