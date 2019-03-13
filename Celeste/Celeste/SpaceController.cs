// Decompiled with JetBrains decompiler
// Type: Celeste.SpaceController
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;

namespace Celeste
{
  public class SpaceController : Entity
  {
    private Level level;

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.level = this.SceneAs<Level>();
    }

    public override void Update()
    {
      base.Update();
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity == null)
        return;
      if ((double) entity.Top > (double) this.level.Camera.Bottom + 12.0)
      {
        entity.Bottom = this.level.Camera.Top - 4f;
      }
      else
      {
        if ((double) entity.Bottom >= (double) this.level.Camera.Top - 4.0)
          return;
        entity.Top = this.level.Camera.Bottom + 12f;
      }
    }
  }
}
