// Decompiled with JetBrains decompiler
// Type: Monocle.VirtualJoystick
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;

namespace Monocle
{
  public class VirtualJoystick : VirtualInput
  {
    public Binding Up;
    public Binding Down;
    public Binding Left;
    public Binding Right;
    public Binding UpAlt;
    public Binding DownAlt;
    public Binding LeftAlt;
    public Binding RightAlt;
    public float Threshold;
    public int GamepadIndex;
    public VirtualInput.OverlapBehaviors OverlapBehavior;
    public bool InvertedX;
    public bool InvertedY;
    private Vector2 value;
    private Vector2 previousValue;
    private bool hTurned;
    private bool vTurned;

    public Vector2 Value { get; private set; }

    public Vector2 PreviousValue { get; private set; }

    public VirtualJoystick(
      Binding up,
      Binding down,
      Binding left,
      Binding right,
      int gamepadIndex,
      float threshold,
      VirtualInput.OverlapBehaviors overlapBehavior = VirtualInput.OverlapBehaviors.TakeNewer)
    {
      this.Up = up;
      this.Down = down;
      this.Left = left;
      this.Right = right;
      this.GamepadIndex = gamepadIndex;
      this.Threshold = threshold;
      this.OverlapBehavior = overlapBehavior;
    }

    public VirtualJoystick(
      Binding up,
      Binding upAlt,
      Binding down,
      Binding downAlt,
      Binding left,
      Binding leftAlt,
      Binding right,
      Binding rightAlt,
      int gamepadIndex,
      float threshold,
      VirtualInput.OverlapBehaviors overlapBehavior = VirtualInput.OverlapBehaviors.TakeNewer)
    {
      this.Up = up;
      this.Down = down;
      this.Left = left;
      this.Right = right;
      this.UpAlt = upAlt;
      this.DownAlt = downAlt;
      this.LeftAlt = leftAlt;
      this.RightAlt = rightAlt;
      this.GamepadIndex = gamepadIndex;
      this.Threshold = threshold;
      this.OverlapBehavior = overlapBehavior;
    }

    public override void Update()
    {
      this.previousValue = this.value;
      if (!MInput.Disabled)
      {
        Vector2 zero = this.value;
        float num1 = this.Right.Axis(this.GamepadIndex, 0.0f);
        float num2 = this.Left.Axis(this.GamepadIndex, 0.0f);
        float num3 = this.Down.Axis(this.GamepadIndex, 0.0f);
        float num4 = this.Up.Axis(this.GamepadIndex, 0.0f);
        if ((double) num1 == 0.0 && this.RightAlt != null)
          num1 = this.RightAlt.Axis(this.GamepadIndex, 0.0f);
        if ((double) num2 == 0.0 && this.LeftAlt != null)
          num2 = this.LeftAlt.Axis(this.GamepadIndex, 0.0f);
        if ((double) num3 == 0.0 && this.DownAlt != null)
          num3 = this.DownAlt.Axis(this.GamepadIndex, 0.0f);
        if ((double) num4 == 0.0 && this.UpAlt != null)
          num4 = this.UpAlt.Axis(this.GamepadIndex, 0.0f);
        if ((double) num1 > (double) num2)
          num2 = 0.0f;
        else if ((double) num2 > (double) num1)
          num1 = 0.0f;
        if ((double) num3 > (double) num4)
          num4 = 0.0f;
        else if ((double) num4 > (double) num3)
          num3 = 0.0f;
        if ((double) num1 != 0.0 && (double) num2 != 0.0)
        {
          switch (this.OverlapBehavior)
          {
            case VirtualInput.OverlapBehaviors.CancelOut:
              zero.X = 0.0f;
              break;
            case VirtualInput.OverlapBehaviors.TakeOlder:
              if ((double) zero.X > 0.0)
              {
                zero.X = num1;
                break;
              }
              if ((double) zero.X < 0.0)
              {
                zero.X = num2;
                break;
              }
              break;
            case VirtualInput.OverlapBehaviors.TakeNewer:
              if (!this.hTurned)
              {
                if ((double) zero.X > 0.0)
                  zero.X = -num2;
                else if ((double) zero.X < 0.0)
                  zero.X = num1;
                this.hTurned = true;
                break;
              }
              if ((double) zero.X > 0.0)
              {
                zero.X = num1;
                break;
              }
              if ((double) zero.X < 0.0)
              {
                zero.X = -num2;
                break;
              }
              break;
          }
        }
        else if ((double) num1 != 0.0)
        {
          this.hTurned = false;
          zero.X = num1;
        }
        else if ((double) num2 != 0.0)
        {
          this.hTurned = false;
          zero.X = -num2;
        }
        else
        {
          this.hTurned = false;
          zero.X = 0.0f;
        }
        if ((double) num3 != 0.0 && (double) num4 != 0.0)
        {
          switch (this.OverlapBehavior)
          {
            case VirtualInput.OverlapBehaviors.CancelOut:
              zero.Y = 0.0f;
              break;
            case VirtualInput.OverlapBehaviors.TakeOlder:
              if ((double) zero.Y > 0.0)
              {
                zero.Y = num3;
                break;
              }
              if ((double) zero.Y < 0.0)
              {
                zero.Y = -num4;
                break;
              }
              break;
            case VirtualInput.OverlapBehaviors.TakeNewer:
              if (!this.vTurned)
              {
                if ((double) zero.Y > 0.0)
                  zero.Y = -num4;
                else if ((double) zero.Y < 0.0)
                  zero.Y = num3;
                this.vTurned = true;
                break;
              }
              if ((double) zero.Y > 0.0)
              {
                zero.Y = num3;
                break;
              }
              if ((double) zero.Y < 0.0)
              {
                zero.Y = -num4;
                break;
              }
              break;
          }
        }
        else if ((double) num3 != 0.0)
        {
          this.vTurned = false;
          zero.Y = num3;
        }
        else if ((double) num4 != 0.0)
        {
          this.vTurned = false;
          zero.Y = -num4;
        }
        else
        {
          this.vTurned = false;
          zero.Y = 0.0f;
        }
        if ((double) zero.Length() < (double) this.Threshold)
          zero = Vector2.Zero;
        this.value = zero;
      }
      this.Value = new Vector2(this.InvertedX ? this.value.X * -1f : this.value.X, this.InvertedY ? this.value.Y * -1f : this.value.Y);
      this.PreviousValue = new Vector2(this.InvertedX ? this.previousValue.X * -1f : this.previousValue.X, this.InvertedY ? this.previousValue.Y * -1f : this.previousValue.Y);
    }

    public static implicit operator Vector2(VirtualJoystick joystick) => joystick.Value;
  }
}
