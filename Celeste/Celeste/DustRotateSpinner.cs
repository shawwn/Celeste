// Decompiled with JetBrains decompiler
// Type: Celeste.DustRotateSpinner
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class DustRotateSpinner : RotateSpinner
  {
    private DustGraphic dusty;

    public DustRotateSpinner(EntityData data, Vector2 offset)
      : base(data, offset)
    {
      this.Add((Component) (this.dusty = new DustGraphic(true, false, false)));
    }

    public override void Update()
    {
      base.Update();
      if (!this.Moving)
        return;
      DustGraphic dusty1 = this.dusty;
      DustGraphic dusty2 = this.dusty;
      double angle = (double) this.Angle;
      double num = 1.57079637050629 * (this.Clockwise ? 1.0 : -1.0);
      Vector2 vector;
      Vector2 vector2_1 = vector = Calc.AngleToVector((float) (angle + num), 1f);
      dusty2.EyeTargetDirection = vector;
      Vector2 vector2_2 = vector2_1;
      dusty1.EyeDirection = vector2_2;
    }

    public override void OnPlayer(Player player)
    {
      base.OnPlayer(player);
      this.dusty.OnHitPlayer();
    }
  }
}
