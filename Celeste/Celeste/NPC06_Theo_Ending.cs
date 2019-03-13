// Decompiled with JetBrains decompiler
// Type: Celeste.NPC06_Theo_Ending
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class NPC06_Theo_Ending : NPC
  {
    private float speedY = 0.0f;

    public NPC06_Theo_Ending(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("theo")));
      this.IdleAnim = "idle";
      this.MoveAnim = "run";
      this.Maxspeed = 72f;
      this.MoveY = false;
      this.Visible = false;
      this.Add((Component) (this.Light = new VertexLight(new Vector2(0.0f, -8f), Color.White, 1f, 16, 32)));
      this.SetupTheoSpriteSounds();
    }

    public override void Update()
    {
      base.Update();
      if (!this.CollideCheck<Solid>(this.Position + new Vector2(0.0f, 1f)))
      {
        this.speedY += 400f * Engine.DeltaTime;
        this.Position.Y += this.speedY * Engine.DeltaTime;
      }
      else
        this.speedY = 0.0f;
    }
  }
}

