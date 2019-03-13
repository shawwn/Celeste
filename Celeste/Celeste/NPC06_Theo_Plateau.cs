// Decompiled with JetBrains decompiler
// Type: Celeste.NPC06_Theo_Plateau
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class NPC06_Theo_Plateau : NPC
  {
    private float speedY;

    public NPC06_Theo_Plateau(EntityData data, Vector2 offset)
      : base(Vector2.op_Addition(data.Position, offset))
    {
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("theo")));
      this.IdleAnim = "idle";
      this.MoveAnim = "walk";
      this.Maxspeed = 48f;
      this.MoveY = false;
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      this.Scene.Add((Entity) new CS06_Campfire((NPC) this, this.Scene.Tracker.GetEntity<Player>()));
      this.Add((Component) (this.Light = new VertexLight(new Vector2(0.0f, -6f), Color.get_White(), 1f, 16, 48)));
    }

    public override void Update()
    {
      base.Update();
      if (!this.CollideCheck<Solid>(Vector2.op_Addition(this.Position, new Vector2(0.0f, 1f))))
      {
        this.speedY += 400f * Engine.DeltaTime;
        ref __Null local = ref this.Position.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local = ^(float&) ref local + this.speedY * Engine.DeltaTime;
      }
      else
        this.speedY = 0.0f;
    }
  }
}
