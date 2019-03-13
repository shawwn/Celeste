// Decompiled with JetBrains decompiler
// Type: Celeste.NPC05_Theo_Mirror
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class NPC05_Theo_Mirror : NPC
  {
    private bool started;

    public NPC05_Theo_Mirror(Vector2 position)
      : base(position)
    {
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("theo")));
      this.IdleAnim = "idle";
      this.MoveAnim = "walk";
      this.Visible = false;
      this.Add((Component) new MirrorReflection()
      {
        IgnoreEntityVisible = true
      });
      this.Sprite.Scale.X = 1f;
      this.Maxspeed = 48f;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if (!this.Session.GetFlag("theoInMirror"))
        return;
      this.RemoveSelf();
    }

    public override void Update()
    {
      base.Update();
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (this.started || entity == null || (double) entity.X <= (double) this.X - 64.0)
        return;
      this.started = true;
      this.Scene.Add((Entity) new CS05_TheoInMirror((NPC) this, entity));
    }
  }
}

