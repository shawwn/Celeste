// Decompiled with JetBrains decompiler
// Type: Celeste.NPC03_Oshiro_Breakdown
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class NPC03_Oshiro_Breakdown : NPC
  {
    private bool talked;

    public NPC03_Oshiro_Breakdown(Vector2 position)
      : base(position)
    {
      this.Add((Component) (this.Sprite = (Sprite) new OshiroSprite(1)));
      this.Add((Component) (this.Light = new VertexLight(-Vector2.UnitY * 16f, Color.White, 1f, 32, 64)));
      this.MoveAnim = "move";
      this.IdleAnim = "idle";
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if (!this.Session.GetFlag("oshiro_breakdown"))
        return;
      this.RemoveSelf();
    }

    public override void Update()
    {
      base.Update();
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (this.talked || entity == null || ((double) entity.X > (double) (this.Level.Bounds.Left + 370) || !entity.OnSafeGround || (double) entity.Y >= (double) this.Level.Bounds.Center.Y) && (double) entity.X > (double) (this.Level.Bounds.Left + 320))
        return;
      this.Scene.Add((Entity) new CS03_OshiroBreakdown(entity, (NPC) this));
      this.talked = true;
    }
  }
}

