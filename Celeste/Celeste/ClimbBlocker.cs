// Decompiled with JetBrains decompiler
// Type: Celeste.ClimbBlocker
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  [Tracked(false)]
  public class ClimbBlocker : Component
  {
    public bool Blocking = true;

    public ClimbBlocker()
      : base(false, false)
    {
    }

    public static bool Check(Scene scene, Entity entity, Vector2 at)
    {
      Vector2 position = entity.Position;
      entity.Position = at;
      int num = ClimbBlocker.Check(scene, entity) ? 1 : 0;
      entity.Position = position;
      return num != 0;
    }

    public static bool Check(Scene scene, Entity entity)
    {
      foreach (ClimbBlocker component in scene.Tracker.GetComponents<ClimbBlocker>())
      {
        if (component.Blocking && entity.CollideCheck(component.Entity))
          return true;
      }
      return false;
    }
  }
}
