// Decompiled with JetBrains decompiler
// Type: Celeste.WaterInteraction
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class WaterInteraction : Component
  {
    public static ParticleType P_Drip;
    public Func<bool> IsDashing;
    public float DrippingTimer;
    public float DrippingOffset;

    public WaterInteraction(Func<bool> isDashing)
      : base(false, false)
    {
      this.IsDashing = isDashing;
    }

    public override void Update()
    {
      if ((double) this.DrippingTimer <= 0.0)
        return;
      this.DrippingTimer -= Engine.DeltaTime;
      if (!this.Scene.OnInterval(0.1f))
        return;
      float num1 = this.Entity.Left - 2f + Calc.Random.NextFloat(this.Entity.Width + 4f);
      float num2 = this.Entity.Top + this.DrippingOffset + Calc.Random.NextFloat(this.Entity.Height - this.DrippingOffset);
      (this.Scene as Level).ParticlesFG.Emit(WaterInteraction.P_Drip, new Vector2(num1, num2));
    }
  }
}
