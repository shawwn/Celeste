// Decompiled with JetBrains decompiler
// Type: Celeste.StaminaDisplay
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class StaminaDisplay : Component
  {
    private Player player;
    private float drawStamina;
    private float displayTimer;
    private Level level;

    public StaminaDisplay()
      : base(true, false)
    {
    }

    public override void Added(Entity entity)
    {
      base.Added(entity);
      this.level = this.SceneAs<Level>();
      this.player = this.EntityAs<Player>();
      this.drawStamina = this.player.Stamina;
    }

    public override void Update()
    {
      base.Update();
      this.drawStamina = Calc.Approach(this.drawStamina, this.player.Stamina, 300f * Engine.DeltaTime);
      if ((double) this.drawStamina < 110.0 && (double) this.drawStamina > 0.0)
      {
        this.displayTimer = 0.75f;
      }
      else
      {
        if ((double) this.displayTimer <= 0.0)
          return;
        this.displayTimer -= Engine.DeltaTime;
      }
    }

    public void RenderHUD()
    {
      if ((double) this.displayTimer <= 0.0)
        return;
      Vector2 vector2 = Vector2.op_Multiply(this.level.Camera.CameraToScreen(Vector2.op_Addition(this.player.Position, new Vector2(0.0f, -18f))), 6f);
      Color color = (double) this.drawStamina >= 20.0 ? Color.get_Lime() : Color.get_Red();
      Draw.Rect((float) (vector2.X - 48.0 - 1.0), (float) (vector2.Y - 6.0 - 1.0), 98f, 14f, Color.get_Black());
      Draw.Rect((float) (vector2.X - 48.0), (float) (vector2.Y - 6.0), (float) (96.0 * ((double) this.drawStamina / 110.0)), 12f, color);
    }
  }
}
