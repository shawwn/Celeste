// Decompiled with JetBrains decompiler
// Type: Celeste.BreathingRumbler
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;

namespace Celeste
{
  public class BreathingRumbler : Entity
  {
    public float Strength = 0.2f;
    private const float MaxRumble = 0.25f;
    private float currentRumble;

    public BreathingRumbler()
    {
      this.currentRumble = this.Strength;
    }

    public override void Update()
    {
      base.Update();
      this.currentRumble = Calc.Approach(this.currentRumble, this.Strength, 2f * Engine.DeltaTime);
      if ((double) this.currentRumble <= 0.0)
        return;
      Input.RumbleSpecific(this.currentRumble * 0.25f, 0.05f);
    }
  }
}
