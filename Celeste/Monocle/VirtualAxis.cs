// Decompiled with JetBrains decompiler
// Type: Monocle.VirtualAxis
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Monocle
{
  public class VirtualAxis : VirtualInput
  {
    public List<VirtualAxis.Node> Nodes;

    public float Value { get; private set; }

    public float PreviousValue { get; private set; }

    public VirtualAxis()
    {
      this.Nodes = new List<VirtualAxis.Node>();
    }

    public VirtualAxis(params VirtualAxis.Node[] nodes)
    {
      this.Nodes = new List<VirtualAxis.Node>((IEnumerable<VirtualAxis.Node>) nodes);
    }

    public override void Update()
    {
      foreach (VirtualInputNode node in this.Nodes)
        node.Update();
      this.PreviousValue = this.Value;
      this.Value = 0.0f;
      if (MInput.Disabled)
        return;
      foreach (VirtualAxis.Node node in this.Nodes)
      {
        float num = node.Value;
        if ((double) num != 0.0)
        {
          this.Value = num;
          break;
        }
      }
    }

    public static implicit operator float(VirtualAxis axis)
    {
      return axis.Value;
    }

    public abstract class Node : VirtualInputNode
    {
      public abstract float Value { get; }
    }

    public class PadLeftStickX : VirtualAxis.Node
    {
      public int GamepadIndex;
      public float Deadzone;

      public PadLeftStickX(int gamepadIndex, float deadzone)
      {
        this.GamepadIndex = gamepadIndex;
        this.Deadzone = deadzone;
      }

      public override float Value
      {
        get
        {
          return Calc.SignThreshold((float) MInput.GamePads[this.GamepadIndex].GetLeftStick().X, this.Deadzone);
        }
      }
    }

    public class PadLeftStickY : VirtualAxis.Node
    {
      public int GamepadIndex;
      public float Deadzone;

      public PadLeftStickY(int gamepadIndex, float deadzone)
      {
        this.GamepadIndex = gamepadIndex;
        this.Deadzone = deadzone;
      }

      public override float Value
      {
        get
        {
          return Calc.SignThreshold((float) MInput.GamePads[this.GamepadIndex].GetLeftStick().Y, this.Deadzone);
        }
      }
    }

    public class PadRightStickX : VirtualAxis.Node
    {
      public int GamepadIndex;
      public float Deadzone;

      public PadRightStickX(int gamepadIndex, float deadzone)
      {
        this.GamepadIndex = gamepadIndex;
        this.Deadzone = deadzone;
      }

      public override float Value
      {
        get
        {
          return Calc.SignThreshold((float) MInput.GamePads[this.GamepadIndex].GetRightStick().X, this.Deadzone);
        }
      }
    }

    public class PadRightStickY : VirtualAxis.Node
    {
      public int GamepadIndex;
      public float Deadzone;

      public PadRightStickY(int gamepadIndex, float deadzone)
      {
        this.GamepadIndex = gamepadIndex;
        this.Deadzone = deadzone;
      }

      public override float Value
      {
        get
        {
          return Calc.SignThreshold((float) MInput.GamePads[this.GamepadIndex].GetRightStick().Y, this.Deadzone);
        }
      }
    }

    public class PadDpadLeftRight : VirtualAxis.Node
    {
      public int GamepadIndex;

      public PadDpadLeftRight(int gamepadIndex)
      {
        this.GamepadIndex = gamepadIndex;
      }

      public override float Value
      {
        get
        {
          if (MInput.GamePads[this.GamepadIndex].DPadRightCheck)
            return 1f;
          return MInput.GamePads[this.GamepadIndex].DPadLeftCheck ? -1f : 0.0f;
        }
      }
    }

    public class PadDpadUpDown : VirtualAxis.Node
    {
      public int GamepadIndex;

      public PadDpadUpDown(int gamepadIndex)
      {
        this.GamepadIndex = gamepadIndex;
      }

      public override float Value
      {
        get
        {
          if (MInput.GamePads[this.GamepadIndex].DPadDownCheck)
            return 1f;
          return MInput.GamePads[this.GamepadIndex].DPadUpCheck ? -1f : 0.0f;
        }
      }
    }

    public class KeyboardKeys : VirtualAxis.Node
    {
      public VirtualInput.OverlapBehaviors OverlapBehavior;
      public Keys Positive;
      public Keys Negative;
      private float value;
      private bool turned;

      public KeyboardKeys(
        VirtualInput.OverlapBehaviors overlapBehavior,
        Keys negative,
        Keys positive)
      {
        this.OverlapBehavior = overlapBehavior;
        this.Negative = negative;
        this.Positive = positive;
      }

      public override void Update()
      {
        if (MInput.Keyboard.Check(this.Positive))
        {
          if (MInput.Keyboard.Check(this.Negative))
          {
            switch (this.OverlapBehavior)
            {
              case VirtualInput.OverlapBehaviors.TakeOlder:
                break;
              case VirtualInput.OverlapBehaviors.TakeNewer:
                if (this.turned)
                  break;
                this.value *= -1f;
                this.turned = true;
                break;
              default:
                this.value = 0.0f;
                break;
            }
          }
          else
          {
            this.turned = false;
            this.value = 1f;
          }
        }
        else if (MInput.Keyboard.Check(this.Negative))
        {
          this.turned = false;
          this.value = -1f;
        }
        else
        {
          this.turned = false;
          this.value = 0.0f;
        }
      }

      public override float Value
      {
        get
        {
          return this.value;
        }
      }
    }
  }
}
