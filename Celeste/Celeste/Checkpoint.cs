// Decompiled with JetBrains decompiler
// Type: Celeste.Checkpoint
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class Checkpoint : Entity
  {
    public Checkpoint(Vector2 position)
      : base(position)
    {
      this.Visible = false;
    }

    public Checkpoint(EntityData data, Vector2 offset)
      : this(data.Position + offset)
    {
    }

    public override void Update()
    {
      Level scene = this.Scene as Level;
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity == null || scene.Transitioning)
        return;
      if (!entity.CollideCheck<CheckpointBlockerTrigger>() && SaveData.Instance.SetCheckpoint(scene.Session.Area, scene.Session.Level))
        scene.AutoSave();
      this.Active = false;
    }
  }
}

