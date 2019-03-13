// Decompiled with JetBrains decompiler
// Type: Monocle.VirtualJoystick
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Monocle
{
  public class VirtualJoystick : VirtualInput
  {
    public List<VirtualJoystick.Node> Nodes;
    public bool Normalized;
    public float? SnapSlices;
    public bool InvertedX;
    public bool InvertedY;

    public Vector2 Value { get; private set; }

    public Vector2 PreviousValue { get; private set; }

    public VirtualJoystick(bool normalized)
    {
      this.Nodes = new List<VirtualJoystick.Node>();
      this.Normalized = normalized;
    }

    public VirtualJoystick(bool normalized, params VirtualJoystick.Node[] nodes)
    {
      this.Nodes = new List<VirtualJoystick.Node>((IEnumerable<VirtualJoystick.Node>) nodes);
      this.Normalized = normalized;
    }

    public override void Update()
    {
      foreach (VirtualInputNode node in this.Nodes)
        node.Update();
      this.PreviousValue = this.Value;
      this.Value = Vector2.get_Zero();
      if (MInput.Disabled)
        return;
      foreach (VirtualJoystick.Node node in this.Nodes)
      {
        Vector2 vec = node.Value;
        if (Vector2.op_Inequality(vec, Vector2.get_Zero()))
        {
          if (this.Normalized)
          {
            if (this.SnapSlices.HasValue)
              vec = vec.SnappedNormal(this.SnapSlices.Value);
            else
              ((Vector2) ref vec).Normalize();
          }
          else if (this.SnapSlices.HasValue)
            vec = vec.Snapped(this.SnapSlices.Value);
          if (this.InvertedX)
          {
            ref __Null local = ref vec.X;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local = ^(float&) ref local * -1f;
          }
          if (this.InvertedY)
          {
            ref __Null local = ref vec.Y;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local = ^(float&) ref local * -1f;
          }
          this.Value = vec;
          break;
        }
      }
    }

    public static implicit operator Vector2(VirtualJoystick joystick)
    {
      return joystick.Value;
    }

    public abstract class Node : VirtualInputNode
    {
      public abstract Vector2 Value { get; }
    }

    public class PadLeftStick : VirtualJoystick.Node
    {
      public int GamepadIndex;
      public float Deadzone;

      public PadLeftStick(int gamepadIndex, float deadzone)
      {
        this.GamepadIndex = gamepadIndex;
        this.Deadzone = deadzone;
      }

      public override Vector2 Value
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].GetLeftStick(this.Deadzone);
        }
      }
    }

    public class PadRightStick : VirtualJoystick.Node
    {
      public int GamepadIndex;
      public float Deadzone;

      public PadRightStick(int gamepadIndex, float deadzone)
      {
        this.GamepadIndex = gamepadIndex;
        this.Deadzone = deadzone;
      }

      public override Vector2 Value
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].GetRightStick(this.Deadzone);
        }
      }
    }

    public class PadDpad : VirtualJoystick.Node
    {
      public int GamepadIndex;

      public PadDpad(int gamepadIndex)
      {
        this.GamepadIndex = gamepadIndex;
      }

      public override Vector2 Value
      {
        get
        {
          Vector2 zero = Vector2.get_Zero();
          if (MInput.GamePads[this.GamepadIndex].DPadRightCheck)
            zero.X = (__Null) 1.0;
          else if (MInput.GamePads[this.GamepadIndex].DPadLeftCheck)
            zero.X = (__Null) -1.0;
          if (MInput.GamePads[this.GamepadIndex].DPadDownCheck)
            zero.Y = (__Null) 1.0;
          else if (MInput.GamePads[this.GamepadIndex].DPadUpCheck)
            zero.Y = (__Null) -1.0;
          return zero;
        }
      }
    }

    public class KeyboardKeys : VirtualJoystick.Node
    {
      public VirtualInput.OverlapBehaviors OverlapBehavior;
      public Keys Left;
      public Keys Right;
      public Keys Up;
      public Keys Down;
      private bool turnedX;
      private bool turnedY;
      private Vector2 value;

      public KeyboardKeys(
        VirtualInput.OverlapBehaviors overlapBehavior,
        Keys left,
        Keys right,
        Keys up,
        Keys down)
      {
        this.OverlapBehavior = overlapBehavior;
        this.Left = left;
        this.Right = right;
        this.Up = up;
        this.Down = down;
      }

      public override void Update()
      {
        if (MInput.Keyboard.Check(this.Left))
        {
          if (MInput.Keyboard.Check(this.Right))
          {
            switch (this.OverlapBehavior)
            {
              case VirtualInput.OverlapBehaviors.TakeOlder:
                break;
              case VirtualInput.OverlapBehaviors.TakeNewer:
                if (!this.turnedX)
                {
                  ref __Null local = ref this.value.X;
                  // ISSUE: cast to a reference type
                  // ISSUE: explicit reference operation
                  // ISSUE: cast to a reference type
                  // ISSUE: explicit reference operation
                  ^(float&) ref local = ^(float&) ref local * -1f;
                  this.turnedX = true;
                  break;
                }
                break;
              default:
                this.value.X = (__Null) 0.0;
                break;
            }
          }
          else
          {
            this.turnedX = false;
            this.value.X = (__Null) -1.0;
          }
        }
        else if (MInput.Keyboard.Check(this.Right))
        {
          this.turnedX = false;
          this.value.X = (__Null) 1.0;
        }
        else
        {
          this.turnedX = false;
          this.value.X = (__Null) 0.0;
        }
        if (MInput.Keyboard.Check(this.Up))
        {
          if (MInput.Keyboard.Check(this.Down))
          {
            switch (this.OverlapBehavior)
            {
              case VirtualInput.OverlapBehaviors.TakeOlder:
                break;
              case VirtualInput.OverlapBehaviors.TakeNewer:
                if (this.turnedY)
                  break;
                ref __Null local1 = ref this.value.Y;
                // ISSUE: cast to a reference type
                // ISSUE: explicit reference operation
                // ISSUE: cast to a reference type
                // ISSUE: explicit reference operation
                ^(float&) ref local1 = ^(float&) ref local1 * -1f;
                this.turnedY = true;
                break;
              default:
                this.value.Y = (__Null) 0.0;
                break;
            }
          }
          else
          {
            this.turnedY = false;
            this.value.Y = (__Null) -1.0;
          }
        }
        else if (MInput.Keyboard.Check(this.Down))
        {
          this.turnedY = false;
          this.value.Y = (__Null) 1.0;
        }
        else
        {
          this.turnedY = false;
          this.value.Y = (__Null) 0.0;
        }
      }

      public override Vector2 Value
      {
        get
        {
          return this.value;
        }
      }
    }
  }
}
