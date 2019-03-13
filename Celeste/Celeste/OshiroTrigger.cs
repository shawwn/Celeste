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
        this.Scene.Add((Entity) new AngryOshiro(new Vector2((float) (level.Bounds.Left - 32), (float) (level.Bounds.Top + level.Bounds.Height / 2)), false));
        this.RemoveSelf();
      }
      else
      {
        AngryOshiro entity = this.Scene.Tracker.GetEntity<AngryOshiro>();
        if (entity != null)
          entity.Leave();
        this.RemoveSelf();
      }
    }
  }
}

