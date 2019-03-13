// Decompiled with JetBrains decompiler
// Type: Celeste.OldSiteChaseMusicHandler
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;

namespace Celeste
{
  public class OldSiteChaseMusicHandler : Entity
  {
    public OldSiteChaseMusicHandler()
    {
      this.Tag = (int) Tags.TransitionUpdate | (int) Tags.Global;
    }

    public override void Update()
    {
      base.Update();
      int num1 = 1150;
      int num2 = 2832;
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity == null || !(Audio.CurrentMusic == "event:/music/lvl2/chase"))
        return;
      Audio.SetMusicParam("escape", (entity.X - (float) num1) / (float) (num2 - num1));
    }
  }
}
