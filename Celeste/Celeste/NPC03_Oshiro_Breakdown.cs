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
      this.Add((Component) (this.Light = new VertexLight(Vector2.op_Multiply(Vector2.op_UnaryNegation(Vector2.get_UnitY()), 16f), Color.get_White(), 1f, 32, 64)));
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
      if (this.talked || entity == null)
        return;
      double x1 = (double) entity.X;
      Rectangle bounds = this.Level.Bounds;
      double num1 = (double) (((Rectangle) ref bounds).get_Left() + 370);
      if (x1 <= num1 && entity.OnSafeGround)
      {
        double y1 = (double) entity.Y;
        bounds = this.Level.Bounds;
        double y2 = (double) (float) ((Rectangle) ref bounds).get_Center().Y;
        if (y1 < y2)
          goto label_4;
      }
      double x2 = (double) entity.X;
      bounds = this.Level.Bounds;
      double num2 = (double) (((Rectangle) ref bounds).get_Left() + 320);
      if (x2 > num2)
        return;
label_4:
      this.Scene.Add((Entity) new CS03_OshiroBreakdown(entity, (NPC) this));
      this.talked = true;
    }
  }
}
