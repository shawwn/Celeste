// Decompiled with JetBrains decompiler
// Type: Monocle.VirtualButton
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Monocle
{
  public class VirtualButton : VirtualInput
  {
    public List<VirtualButton.Node> Nodes;
    public float BufferTime;
    public Keys? DebugOverridePressed;
    private float firstRepeatTime;
    private float multiRepeatTime;
    private float bufferCounter;
    private float repeatCounter;
    private bool canRepeat;
    private bool consumed;

    public bool Repeating { get; private set; }

    public VirtualButton(float bufferTime)
    {
      this.Nodes = new List<VirtualButton.Node>();
      this.BufferTime = bufferTime;
    }

    public VirtualButton()
      : this(0.0f)
    {
    }

    public VirtualButton(float bufferTime, params VirtualButton.Node[] nodes)
    {
      this.Nodes = new List<VirtualButton.Node>((IEnumerable<VirtualButton.Node>) nodes);
      this.BufferTime = bufferTime;
    }

    public VirtualButton(params VirtualButton.Node[] nodes)
      : this(0.0f, nodes)
    {
    }

    public void SetRepeat(float repeatTime)
    {
      this.SetRepeat(repeatTime, repeatTime);
    }

    public void SetRepeat(float firstRepeatTime, float multiRepeatTime)
    {
      this.firstRepeatTime = firstRepeatTime;
      this.multiRepeatTime = multiRepeatTime;
      this.canRepeat = (double) this.firstRepeatTime > 0.0;
      if (this.canRepeat)
        return;
      this.Repeating = false;
    }

    public override void Update()
    {
      this.consumed = false;
      this.bufferCounter -= Engine.DeltaTime;
      bool flag = false;
      foreach (VirtualButton.Node node in this.Nodes)
      {
        node.Update();
        if (node.Pressed)
        {
          this.bufferCounter = this.BufferTime;
          flag = true;
        }
        else if (node.Check)
          flag = true;
      }
      if (!flag)
      {
        this.Repeating = false;
        this.repeatCounter = 0.0f;
        this.bufferCounter = 0.0f;
      }
      else
      {
        if (!this.canRepeat)
          return;
        this.Repeating = false;
        if ((double) this.repeatCounter == 0.0)
        {
          this.repeatCounter = this.firstRepeatTime;
        }
        else
        {
          this.repeatCounter -= Engine.DeltaTime;
          if ((double) this.repeatCounter > 0.0)
            return;
          this.Repeating = true;
          this.repeatCounter = this.multiRepeatTime;
        }
      }
    }

    public bool Check
    {
      get
      {
        if (MInput.Disabled)
          return false;
        foreach (VirtualButton.Node node in this.Nodes)
        {
          if (node.Check)
            return true;
        }
        return false;
      }
    }

    public bool Pressed
    {
      get
      {
        if (this.DebugOverridePressed.HasValue && MInput.Keyboard.Check(this.DebugOverridePressed.Value))
          return true;
        if (MInput.Disabled || this.consumed)
          return false;
        if ((double) this.bufferCounter > 0.0 || this.Repeating)
          return true;
        foreach (VirtualButton.Node node in this.Nodes)
        {
          if (node.Pressed)
            return true;
        }
        return false;
      }
    }

    public bool Released
    {
      get
      {
        if (MInput.Disabled)
          return false;
        foreach (VirtualButton.Node node in this.Nodes)
        {
          if (node.Released)
            return true;
        }
        return false;
      }
    }

    public void ConsumeBuffer()
    {
      this.bufferCounter = 0.0f;
    }

    public void ConsumePress()
    {
      this.bufferCounter = 0.0f;
      this.consumed = true;
    }

    public static implicit operator bool(VirtualButton button)
    {
      return button.Check;
    }

    public abstract class Node : VirtualInputNode
    {
      public abstract bool Check { get; }

      public abstract bool Pressed { get; }

      public abstract bool Released { get; }
    }

    public class KeyboardKey : VirtualButton.Node
    {
      public Keys Key;

      public KeyboardKey(Keys key)
      {
        this.Key = key;
      }

      public override bool Check
      {
        get
        {
          return MInput.Keyboard.Check(this.Key);
        }
      }

      public override bool Pressed
      {
        get
        {
          return MInput.Keyboard.Pressed(this.Key);
        }
      }

      public override bool Released
      {
        get
        {
          return MInput.Keyboard.Released(this.Key);
        }
      }
    }

    public class PadButton : VirtualButton.Node
    {
      public int GamepadIndex;
      public Buttons Button;

      public PadButton(int gamepadIndex, Buttons button)
      {
        this.GamepadIndex = gamepadIndex;
        this.Button = button;
      }

      public override bool Check
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].Check(this.Button);
        }
      }

      public override bool Pressed
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].Pressed(this.Button);
        }
      }

      public override bool Released
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].Released(this.Button);
        }
      }
    }

    public class PadLeftStickRight : VirtualButton.Node
    {
      public int GamepadIndex;
      public float Deadzone;

      public PadLeftStickRight(int gamepadindex, float deadzone)
      {
        this.GamepadIndex = gamepadindex;
        this.Deadzone = deadzone;
      }

      public override bool Check
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].LeftStickRightCheck(this.Deadzone);
        }
      }

      public override bool Pressed
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].LeftStickRightPressed(this.Deadzone);
        }
      }

      public override bool Released
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].LeftStickRightReleased(this.Deadzone);
        }
      }
    }

    public class PadLeftStickLeft : VirtualButton.Node
    {
      public int GamepadIndex;
      public float Deadzone;

      public PadLeftStickLeft(int gamepadindex, float deadzone)
      {
        this.GamepadIndex = gamepadindex;
        this.Deadzone = deadzone;
      }

      public override bool Check
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].LeftStickLeftCheck(this.Deadzone);
        }
      }

      public override bool Pressed
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].LeftStickLeftPressed(this.Deadzone);
        }
      }

      public override bool Released
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].LeftStickLeftReleased(this.Deadzone);
        }
      }
    }

    public class PadLeftStickUp : VirtualButton.Node
    {
      public int GamepadIndex;
      public float Deadzone;

      public PadLeftStickUp(int gamepadindex, float deadzone)
      {
        this.GamepadIndex = gamepadindex;
        this.Deadzone = deadzone;
      }

      public override bool Check
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].LeftStickUpCheck(this.Deadzone);
        }
      }

      public override bool Pressed
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].LeftStickUpPressed(this.Deadzone);
        }
      }

      public override bool Released
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].LeftStickUpReleased(this.Deadzone);
        }
      }
    }

    public class PadLeftStickDown : VirtualButton.Node
    {
      public int GamepadIndex;
      public float Deadzone;

      public PadLeftStickDown(int gamepadindex, float deadzone)
      {
        this.GamepadIndex = gamepadindex;
        this.Deadzone = deadzone;
      }

      public override bool Check
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].LeftStickDownCheck(this.Deadzone);
        }
      }

      public override bool Pressed
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].LeftStickDownPressed(this.Deadzone);
        }
      }

      public override bool Released
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].LeftStickDownReleased(this.Deadzone);
        }
      }
    }

    public class PadRightStickRight : VirtualButton.Node
    {
      public int GamepadIndex;
      public float Deadzone;

      public PadRightStickRight(int gamepadindex, float deadzone)
      {
        this.GamepadIndex = gamepadindex;
        this.Deadzone = deadzone;
      }

      public override bool Check
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].RightStickRightCheck(this.Deadzone);
        }
      }

      public override bool Pressed
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].RightStickRightPressed(this.Deadzone);
        }
      }

      public override bool Released
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].RightStickRightReleased(this.Deadzone);
        }
      }
    }

    public class PadRightStickLeft : VirtualButton.Node
    {
      public int GamepadIndex;
      public float Deadzone;

      public PadRightStickLeft(int gamepadindex, float deadzone)
      {
        this.GamepadIndex = gamepadindex;
        this.Deadzone = deadzone;
      }

      public override bool Check
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].RightStickLeftCheck(this.Deadzone);
        }
      }

      public override bool Pressed
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].RightStickLeftPressed(this.Deadzone);
        }
      }

      public override bool Released
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].RightStickLeftReleased(this.Deadzone);
        }
      }
    }

    public class PadRightStickUp : VirtualButton.Node
    {
      public int GamepadIndex;
      public float Deadzone;

      public PadRightStickUp(int gamepadindex, float deadzone)
      {
        this.GamepadIndex = gamepadindex;
        this.Deadzone = deadzone;
      }

      public override bool Check
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].RightStickUpCheck(this.Deadzone);
        }
      }

      public override bool Pressed
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].RightStickUpPressed(this.Deadzone);
        }
      }

      public override bool Released
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].RightStickUpReleased(this.Deadzone);
        }
      }
    }

    public class PadRightStickDown : VirtualButton.Node
    {
      public int GamepadIndex;
      public float Deadzone;

      public PadRightStickDown(int gamepadindex, float deadzone)
      {
        this.GamepadIndex = gamepadindex;
        this.Deadzone = deadzone;
      }

      public override bool Check
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].RightStickDownCheck(this.Deadzone);
        }
      }

      public override bool Pressed
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].RightStickDownPressed(this.Deadzone);
        }
      }

      public override bool Released
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].RightStickDownReleased(this.Deadzone);
        }
      }
    }

    public class PadLeftTrigger : VirtualButton.Node
    {
      public int GamepadIndex;
      public float Threshold;

      public PadLeftTrigger(int gamepadIndex, float threshold)
      {
        this.GamepadIndex = gamepadIndex;
        this.Threshold = threshold;
      }

      public override bool Check
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].LeftTriggerCheck(this.Threshold);
        }
      }

      public override bool Pressed
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].LeftTriggerPressed(this.Threshold);
        }
      }

      public override bool Released
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].LeftTriggerReleased(this.Threshold);
        }
      }
    }

    public class PadRightTrigger : VirtualButton.Node
    {
      public int GamepadIndex;
      public float Threshold;

      public PadRightTrigger(int gamepadIndex, float threshold)
      {
        this.GamepadIndex = gamepadIndex;
        this.Threshold = threshold;
      }

      public override bool Check
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].RightTriggerCheck(this.Threshold);
        }
      }

      public override bool Pressed
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].RightTriggerPressed(this.Threshold);
        }
      }

      public override bool Released
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].RightTriggerReleased(this.Threshold);
        }
      }
    }

    public class PadDPadRight : VirtualButton.Node
    {
      public int GamepadIndex;

      public PadDPadRight(int gamepadIndex)
      {
        this.GamepadIndex = gamepadIndex;
      }

      public override bool Check
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].DPadRightCheck;
        }
      }

      public override bool Pressed
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].DPadRightPressed;
        }
      }

      public override bool Released
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].DPadRightReleased;
        }
      }
    }

    public class PadDPadLeft : VirtualButton.Node
    {
      public int GamepadIndex;

      public PadDPadLeft(int gamepadIndex)
      {
        this.GamepadIndex = gamepadIndex;
      }

      public override bool Check
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].DPadLeftCheck;
        }
      }

      public override bool Pressed
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].DPadLeftPressed;
        }
      }

      public override bool Released
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].DPadLeftReleased;
        }
      }
    }

    public class PadDPadUp : VirtualButton.Node
    {
      public int GamepadIndex;

      public PadDPadUp(int gamepadIndex)
      {
        this.GamepadIndex = gamepadIndex;
      }

      public override bool Check
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].DPadUpCheck;
        }
      }

      public override bool Pressed
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].DPadUpPressed;
        }
      }

      public override bool Released
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].DPadUpReleased;
        }
      }
    }

    public class PadDPadDown : VirtualButton.Node
    {
      public int GamepadIndex;

      public PadDPadDown(int gamepadIndex)
      {
        this.GamepadIndex = gamepadIndex;
      }

      public override bool Check
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].DPadDownCheck;
        }
      }

      public override bool Pressed
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].DPadDownPressed;
        }
      }

      public override bool Released
      {
        get
        {
          return MInput.GamePads[this.GamepadIndex].DPadDownReleased;
        }
      }
    }

    public class MouseLeftButton : VirtualButton.Node
    {
      public override bool Check
      {
        get
        {
          return MInput.Mouse.CheckLeftButton;
        }
      }

      public override bool Pressed
      {
        get
        {
          return MInput.Mouse.PressedLeftButton;
        }
      }

      public override bool Released
      {
        get
        {
          return MInput.Mouse.ReleasedLeftButton;
        }
      }
    }

    public class MouseRightButton : VirtualButton.Node
    {
      public override bool Check
      {
        get
        {
          return MInput.Mouse.CheckRightButton;
        }
      }

      public override bool Pressed
      {
        get
        {
          return MInput.Mouse.PressedRightButton;
        }
      }

      public override bool Released
      {
        get
        {
          return MInput.Mouse.ReleasedRightButton;
        }
      }
    }

    public class MouseMiddleButton : VirtualButton.Node
    {
      public override bool Check
      {
        get
        {
          return MInput.Mouse.CheckMiddleButton;
        }
      }

      public override bool Pressed
      {
        get
        {
          return MInput.Mouse.PressedMiddleButton;
        }
      }

      public override bool Released
      {
        get
        {
          return MInput.Mouse.ReleasedMiddleButton;
        }
      }
    }

    public class VirtualAxisTrigger : VirtualButton.Node
    {
      public VirtualInput.ThresholdModes Mode;
      public float Threshold;
      private VirtualAxis axis;

      public VirtualAxisTrigger(
        VirtualAxis axis,
        VirtualInput.ThresholdModes mode,
        float threshold)
      {
        this.axis = axis;
        this.Mode = mode;
        this.Threshold = threshold;
      }

      public override bool Check
      {
        get
        {
          if (this.Mode == VirtualInput.ThresholdModes.LargerThan)
            return (double) this.axis.Value >= (double) this.Threshold;
          if (this.Mode == VirtualInput.ThresholdModes.LessThan)
            return (double) this.axis.Value <= (double) this.Threshold;
          return (double) this.axis.Value == (double) this.Threshold;
        }
      }

      public override bool Pressed
      {
        get
        {
          if (this.Mode == VirtualInput.ThresholdModes.LargerThan)
          {
            if ((double) this.axis.Value >= (double) this.Threshold)
              return (double) this.axis.PreviousValue < (double) this.Threshold;
            return false;
          }
          if (this.Mode == VirtualInput.ThresholdModes.LessThan)
          {
            if ((double) this.axis.Value <= (double) this.Threshold)
              return (double) this.axis.PreviousValue > (double) this.Threshold;
            return false;
          }
          if ((double) this.axis.Value == (double) this.Threshold)
            return (double) this.axis.PreviousValue != (double) this.Threshold;
          return false;
        }
      }

      public override bool Released
      {
        get
        {
          if (this.Mode == VirtualInput.ThresholdModes.LargerThan)
          {
            if ((double) this.axis.Value < (double) this.Threshold)
              return (double) this.axis.PreviousValue >= (double) this.Threshold;
            return false;
          }
          if (this.Mode == VirtualInput.ThresholdModes.LessThan)
          {
            if ((double) this.axis.Value > (double) this.Threshold)
              return (double) this.axis.PreviousValue <= (double) this.Threshold;
            return false;
          }
          if ((double) this.axis.Value != (double) this.Threshold)
            return (double) this.axis.PreviousValue == (double) this.Threshold;
          return false;
        }
      }

      public enum Modes
      {
        LargerThan,
        LessThan,
        Equals,
      }
    }

    public class VirtualIntegerAxisTrigger : VirtualButton.Node
    {
      public VirtualInput.ThresholdModes Mode;
      public int Threshold;
      private VirtualIntegerAxis axis;

      public VirtualIntegerAxisTrigger(
        VirtualIntegerAxis axis,
        VirtualInput.ThresholdModes mode,
        int threshold)
      {
        this.axis = axis;
        this.Mode = mode;
        this.Threshold = threshold;
      }

      public override bool Check
      {
        get
        {
          if (this.Mode == VirtualInput.ThresholdModes.LargerThan)
            return this.axis.Value >= this.Threshold;
          if (this.Mode == VirtualInput.ThresholdModes.LessThan)
            return this.axis.Value <= this.Threshold;
          return this.axis.Value == this.Threshold;
        }
      }

      public override bool Pressed
      {
        get
        {
          if (this.Mode == VirtualInput.ThresholdModes.LargerThan)
          {
            if (this.axis.Value >= this.Threshold)
              return this.axis.PreviousValue < this.Threshold;
            return false;
          }
          if (this.Mode == VirtualInput.ThresholdModes.LessThan)
          {
            if (this.axis.Value <= this.Threshold)
              return this.axis.PreviousValue > this.Threshold;
            return false;
          }
          if (this.axis.Value == this.Threshold)
            return this.axis.PreviousValue != this.Threshold;
          return false;
        }
      }

      public override bool Released
      {
        get
        {
          if (this.Mode == VirtualInput.ThresholdModes.LargerThan)
          {
            if (this.axis.Value < this.Threshold)
              return this.axis.PreviousValue >= this.Threshold;
            return false;
          }
          if (this.Mode == VirtualInput.ThresholdModes.LessThan)
          {
            if (this.axis.Value > this.Threshold)
              return this.axis.PreviousValue <= this.Threshold;
            return false;
          }
          if (this.axis.Value != this.Threshold)
            return this.axis.PreviousValue == this.Threshold;
          return false;
        }
      }

      public enum Modes
      {
        LargerThan,
        LessThan,
        Equals,
      }
    }

    public class VirtualJoystickXTrigger : VirtualButton.Node
    {
      public VirtualInput.ThresholdModes Mode;
      public float Threshold;
      private VirtualJoystick joystick;

      public VirtualJoystickXTrigger(
        VirtualJoystick joystick,
        VirtualInput.ThresholdModes mode,
        float threshold)
      {
        this.joystick = joystick;
        this.Mode = mode;
        this.Threshold = threshold;
      }

      public override bool Check
      {
        get
        {
          if (this.Mode == VirtualInput.ThresholdModes.LargerThan)
            return this.joystick.Value.X >= (double) this.Threshold;
          if (this.Mode == VirtualInput.ThresholdModes.LessThan)
            return this.joystick.Value.X <= (double) this.Threshold;
          return this.joystick.Value.X == (double) this.Threshold;
        }
      }

      public override bool Pressed
      {
        get
        {
          if (this.Mode == VirtualInput.ThresholdModes.LargerThan)
          {
            if (this.joystick.Value.X >= (double) this.Threshold)
              return this.joystick.PreviousValue.X < (double) this.Threshold;
            return false;
          }
          if (this.Mode == VirtualInput.ThresholdModes.LessThan)
          {
            if (this.joystick.Value.X <= (double) this.Threshold)
              return this.joystick.PreviousValue.X > (double) this.Threshold;
            return false;
          }
          if (this.joystick.Value.X == (double) this.Threshold)
            return this.joystick.PreviousValue.X != (double) this.Threshold;
          return false;
        }
      }

      public override bool Released
      {
        get
        {
          if (this.Mode == VirtualInput.ThresholdModes.LargerThan)
          {
            if (this.joystick.Value.X < (double) this.Threshold)
              return this.joystick.PreviousValue.X >= (double) this.Threshold;
            return false;
          }
          if (this.Mode == VirtualInput.ThresholdModes.LessThan)
          {
            if (this.joystick.Value.X > (double) this.Threshold)
              return this.joystick.PreviousValue.X <= (double) this.Threshold;
            return false;
          }
          if (this.joystick.Value.X != (double) this.Threshold)
            return this.joystick.PreviousValue.X == (double) this.Threshold;
          return false;
        }
      }

      public enum Modes
      {
        LargerThan,
        LessThan,
        Equals,
      }
    }

    public class VirtualJoystickYTrigger : VirtualButton.Node
    {
      public VirtualInput.ThresholdModes Mode;
      public float Threshold;
      private VirtualJoystick joystick;

      public VirtualJoystickYTrigger(
        VirtualJoystick joystick,
        VirtualInput.ThresholdModes mode,
        float threshold)
      {
        this.joystick = joystick;
        this.Mode = mode;
        this.Threshold = threshold;
      }

      public override bool Check
      {
        get
        {
          if (this.Mode == VirtualInput.ThresholdModes.LargerThan)
            return this.joystick.Value.X >= (double) this.Threshold;
          if (this.Mode == VirtualInput.ThresholdModes.LessThan)
            return this.joystick.Value.X <= (double) this.Threshold;
          return this.joystick.Value.X == (double) this.Threshold;
        }
      }

      public override bool Pressed
      {
        get
        {
          if (this.Mode == VirtualInput.ThresholdModes.LargerThan)
          {
            if (this.joystick.Value.X >= (double) this.Threshold)
              return this.joystick.PreviousValue.X < (double) this.Threshold;
            return false;
          }
          if (this.Mode == VirtualInput.ThresholdModes.LessThan)
          {
            if (this.joystick.Value.X <= (double) this.Threshold)
              return this.joystick.PreviousValue.X > (double) this.Threshold;
            return false;
          }
          if (this.joystick.Value.X == (double) this.Threshold)
            return this.joystick.PreviousValue.X != (double) this.Threshold;
          return false;
        }
      }

      public override bool Released
      {
        get
        {
          if (this.Mode == VirtualInput.ThresholdModes.LargerThan)
          {
            if (this.joystick.Value.X < (double) this.Threshold)
              return this.joystick.PreviousValue.X >= (double) this.Threshold;
            return false;
          }
          if (this.Mode == VirtualInput.ThresholdModes.LessThan)
          {
            if (this.joystick.Value.X > (double) this.Threshold)
              return this.joystick.PreviousValue.X <= (double) this.Threshold;
            return false;
          }
          if (this.joystick.Value.X != (double) this.Threshold)
            return this.joystick.PreviousValue.X == (double) this.Threshold;
          return false;
        }
      }
    }
  }
}
