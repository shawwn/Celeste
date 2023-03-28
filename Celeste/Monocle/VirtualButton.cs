// Decompiled with JetBrains decompiler
// Type: Monocle.VirtualButton
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework.Input;

namespace Monocle
{
  public class VirtualButton : VirtualInput
  {
    public Binding Binding;
    public float Threshold;
    public float BufferTime;
    public int GamepadIndex;
    public Keys? DebugOverridePressed;
    private float firstRepeatTime;
    private float multiRepeatTime;
    private float bufferCounter;
    private float repeatCounter;
    private bool canRepeat;
    private bool consumed;

    public bool Repeating { get; private set; }

    public VirtualButton(
      Binding binding,
      int gamepadIndex,
      float bufferTime,
      float triggerThreshold)
    {
      this.Binding = binding;
      this.GamepadIndex = gamepadIndex;
      this.BufferTime = bufferTime;
      this.Threshold = triggerThreshold;
    }

    public VirtualButton()
    {
    }

    public void SetRepeat(float repeatTime) => this.SetRepeat(repeatTime, repeatTime);

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
      if (this.Binding.Pressed(this.GamepadIndex, this.Threshold))
      {
        this.bufferCounter = this.BufferTime;
        flag = true;
      }
      else if (this.Binding.Check(this.GamepadIndex, this.Threshold))
        flag = true;
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

    public bool Check => !MInput.Disabled && this.Binding.Check(this.GamepadIndex, this.Threshold);

    public bool Pressed
    {
      get
      {
        if (this.DebugOverridePressed.HasValue && MInput.Keyboard.Check(this.DebugOverridePressed.Value))
          return true;
        if (MInput.Disabled || this.consumed)
          return false;
        return (double) this.bufferCounter > 0.0 || this.Repeating || this.Binding.Pressed(this.GamepadIndex, this.Threshold);
      }
    }

    public bool Released => !MInput.Disabled && this.Binding.Released(this.GamepadIndex, this.Threshold);

    public void ConsumeBuffer() => this.bufferCounter = 0.0f;

    public void ConsumePress()
    {
      this.bufferCounter = 0.0f;
      this.consumed = true;
    }

    public static implicit operator bool(VirtualButton button) => button.Check;
  }
}
