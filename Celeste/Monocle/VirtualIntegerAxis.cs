// Decompiled with JetBrains decompiler
// Type: Monocle.VirtualIntegerAxis
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

namespace Monocle
{
  public class VirtualIntegerAxis : VirtualInput
  {
    public Binding Positive;
    public Binding Negative;
    public Binding PositiveAlt;
    public Binding NegativeAlt;
    public float Threshold;
    public int GamepadIndex;
    public VirtualInput.OverlapBehaviors OverlapBehavior;
    public bool Inverted;
    public int Value;
    public int PreviousValue;
    private bool turned;

    public VirtualIntegerAxis()
    {
    }

    public VirtualIntegerAxis(
      Binding negative,
      Binding positive,
      int gamepadIndex,
      float threshold,
      VirtualInput.OverlapBehaviors overlapBehavior = VirtualInput.OverlapBehaviors.TakeNewer)
    {
      this.Positive = positive;
      this.Negative = negative;
      this.Threshold = threshold;
      this.GamepadIndex = gamepadIndex;
      this.OverlapBehavior = overlapBehavior;
    }

    public VirtualIntegerAxis(
      Binding negative,
      Binding negativeAlt,
      Binding positive,
      Binding positiveAlt,
      int gamepadIndex,
      float threshold,
      VirtualInput.OverlapBehaviors overlapBehavior = VirtualInput.OverlapBehaviors.TakeNewer)
    {
      this.Positive = positive;
      this.Negative = negative;
      this.PositiveAlt = positiveAlt;
      this.NegativeAlt = negativeAlt;
      this.Threshold = threshold;
      this.GamepadIndex = gamepadIndex;
      this.OverlapBehavior = overlapBehavior;
    }

    public override void Update()
    {
      this.PreviousValue = this.Value;
      if (MInput.Disabled)
        return;
      bool flag1 = (double) this.Positive.Axis(this.GamepadIndex, this.Threshold) > 0.0 || this.PositiveAlt != null && (double) this.PositiveAlt.Axis(this.GamepadIndex, this.Threshold) > 0.0;
      bool flag2 = (double) this.Negative.Axis(this.GamepadIndex, this.Threshold) > 0.0 || this.NegativeAlt != null && (double) this.NegativeAlt.Axis(this.GamepadIndex, this.Threshold) > 0.0;
      if (flag1 & flag2)
      {
        switch (this.OverlapBehavior)
        {
          case VirtualInput.OverlapBehaviors.CancelOut:
            this.Value = 0;
            break;
          case VirtualInput.OverlapBehaviors.TakeOlder:
            this.Value = this.PreviousValue;
            break;
          case VirtualInput.OverlapBehaviors.TakeNewer:
            if (!this.turned)
            {
              this.Value *= -1;
              this.turned = true;
              break;
            }
            break;
        }
      }
      else if (flag1)
      {
        this.turned = false;
        this.Value = 1;
      }
      else if (flag2)
      {
        this.turned = false;
        this.Value = -1;
      }
      else
      {
        this.turned = false;
        this.Value = 0;
      }
      if (!this.Inverted)
        return;
      this.Value = -this.Value;
    }

    public static implicit operator float(VirtualIntegerAxis axis) => (float) axis.Value;
  }
}
