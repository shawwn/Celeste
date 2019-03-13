// Decompiled with JetBrains decompiler
// Type: Celeste.NPC05_Theo_Entrance
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class NPC05_Theo_Entrance : NPC
  {
    public NPC05_Theo_Entrance(Vector2 position)
      : base(position)
    {
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("theo")));
      this.IdleAnim = "idle";
      this.MoveAnim = "walk";
      this.Maxspeed = 48f;
      this.Add((Component) (this.Light = new VertexLight(-Vector2.UnitY * 12f, Color.White, 1f, 32, 64)));
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if (this.Session.GetFlag("entrance"))
        this.RemoveSelf();
      else
        scene.Add((Entity) new CS05_Entrance((NPC) this));
    }
  }
}

