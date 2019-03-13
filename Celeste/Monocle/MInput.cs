// Decompiled with JetBrains decompiler
// Type: Monocle.MInput
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Monocle
{
  public static class MInput
  {
    public static bool Active = true;
    public static bool Disabled = false;
    internal static List<VirtualInput> VirtualInputs;

    public static MInput.KeyboardData Keyboard { get; private set; }

    public static MInput.MouseData Mouse { get; private set; }

    public static MInput.GamePadData[] GamePads { get; private set; }

    internal static void Initialize()
    {
      MInput.Keyboard = new MInput.KeyboardData();
      MInput.Mouse = new MInput.MouseData();
      MInput.GamePads = new MInput.GamePadData[4];
      for (int playerIndex = 0; playerIndex < 4; ++playerIndex)
        MInput.GamePads[playerIndex] = new MInput.GamePadData(playerIndex);
      MInput.VirtualInputs = new List<VirtualInput>();
    }

    internal static void Shutdown()
    {
      foreach (MInput.GamePadData gamePad in MInput.GamePads)
        gamePad.StopRumble();
    }

    internal static void Update()
    {
      if (Engine.Instance.IsActive && MInput.Active)
      {
        if (Engine.Commands.Open)
        {
          MInput.Keyboard.UpdateNull();
          MInput.Mouse.UpdateNull();
        }
        else
        {
          MInput.Keyboard.Update();
          MInput.Mouse.Update();
        }
        for (int index = 0; index < 4; ++index)
          MInput.GamePads[index].Update();
      }
      else
      {
        MInput.Keyboard.UpdateNull();
        MInput.Mouse.UpdateNull();
        for (int index = 0; index < 4; ++index)
          MInput.GamePads[index].UpdateNull();
      }
      MInput.UpdateVirtualInputs();
    }

    public static void UpdateNull()
    {
      MInput.Keyboard.UpdateNull();
      MInput.Mouse.UpdateNull();
      for (int index = 0; index < 4; ++index)
        MInput.GamePads[index].UpdateNull();
      MInput.UpdateVirtualInputs();
    }

    private static void UpdateVirtualInputs()
    {
      foreach (VirtualInput virtualInput in MInput.VirtualInputs)
        virtualInput.Update();
    }

    public static void RumbleFirst(float strength, float time)
    {
      MInput.GamePads[0].Rumble(strength, time);
    }

    public static int Axis(bool negative, bool positive, int bothValue)
    {
      if (negative)
      {
        if (positive)
          return bothValue;
        return -1;
      }
      return positive ? 1 : 0;
    }

    public static int Axis(float axisValue, float deadzone)
    {
      if ((double) Math.Abs(axisValue) >= (double) deadzone)
        return Math.Sign(axisValue);
      return 0;
    }

    public static int Axis(
      bool negative,
      bool positive,
      int bothValue,
      float axisValue,
      float deadzone)
    {
      int num = MInput.Axis(axisValue, deadzone);
      if (num == 0)
        num = MInput.Axis(negative, positive, bothValue);
      return num;
    }

    public class KeyboardData
    {
      public KeyboardState PreviousState;
      public KeyboardState CurrentState;

      internal KeyboardData()
      {
      }

      internal void Update()
      {
        this.PreviousState = this.CurrentState;
        this.CurrentState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
      }

      internal void UpdateNull()
      {
        this.PreviousState = this.CurrentState;
        this.CurrentState = new KeyboardState();
      }

      public bool Check(Keys key)
      {
        if (MInput.Disabled)
          return false;
        return this.CurrentState.IsKeyDown(key);
      }

      public bool Pressed(Keys key)
      {
        if (MInput.Disabled)
          return false;
        return this.CurrentState.IsKeyDown(key) && !this.PreviousState.IsKeyDown(key);
      }

      public bool Released(Keys key)
      {
        if (MInput.Disabled)
          return false;
        return !this.CurrentState.IsKeyDown(key) && this.PreviousState.IsKeyDown(key);
      }

      public bool Check(Keys keyA, Keys keyB)
      {
        return this.Check(keyA) || this.Check(keyB);
      }

      public bool Pressed(Keys keyA, Keys keyB)
      {
        return this.Pressed(keyA) || this.Pressed(keyB);
      }

      public bool Released(Keys keyA, Keys keyB)
      {
        return this.Released(keyA) || this.Released(keyB);
      }

      public bool Check(Keys keyA, Keys keyB, Keys keyC)
      {
        return this.Check(keyA) || this.Check(keyB) || this.Check(keyC);
      }

      public bool Pressed(Keys keyA, Keys keyB, Keys keyC)
      {
        return this.Pressed(keyA) || this.Pressed(keyB) || this.Pressed(keyC);
      }

      public bool Released(Keys keyA, Keys keyB, Keys keyC)
      {
        return this.Released(keyA) || this.Released(keyB) || this.Released(keyC);
      }

      public int AxisCheck(Keys negative, Keys positive)
      {
        if (this.Check(negative))
          return this.Check(positive) ? 0 : -1;
        return this.Check(positive) ? 1 : 0;
      }

      public int AxisCheck(Keys negative, Keys positive, int both)
      {
        if (this.Check(negative))
        {
          if (this.Check(positive))
            return both;
          return -1;
        }
        return this.Check(positive) ? 1 : 0;
      }
    }

    public class MouseData
    {
      public MouseState PreviousState;
      public MouseState CurrentState;

      internal MouseData()
      {
        this.PreviousState = new MouseState();
        this.CurrentState = new MouseState();
      }

      internal void Update()
      {
        this.PreviousState = this.CurrentState;
        this.CurrentState = Microsoft.Xna.Framework.Input.Mouse.GetState();
      }

      internal void UpdateNull()
      {
        this.PreviousState = this.CurrentState;
        this.CurrentState = new MouseState();
      }

      public bool CheckLeftButton
      {
        get
        {
          return this.CurrentState.LeftButton == ButtonState.Pressed;
        }
      }

      public bool CheckRightButton
      {
        get
        {
          return this.CurrentState.RightButton == ButtonState.Pressed;
        }
      }

      public bool CheckMiddleButton
      {
        get
        {
          return this.CurrentState.MiddleButton == ButtonState.Pressed;
        }
      }

      public bool PressedLeftButton
      {
        get
        {
          return this.CurrentState.LeftButton == ButtonState.Pressed && this.PreviousState.LeftButton == ButtonState.Released;
        }
      }

      public bool PressedRightButton
      {
        get
        {
          return this.CurrentState.RightButton == ButtonState.Pressed && this.PreviousState.RightButton == ButtonState.Released;
        }
      }

      public bool PressedMiddleButton
      {
        get
        {
          return this.CurrentState.MiddleButton == ButtonState.Pressed && this.PreviousState.MiddleButton == ButtonState.Released;
        }
      }

      public bool ReleasedLeftButton
      {
        get
        {
          return this.CurrentState.LeftButton == ButtonState.Released && this.PreviousState.LeftButton == ButtonState.Pressed;
        }
      }

      public bool ReleasedRightButton
      {
        get
        {
          return this.CurrentState.RightButton == ButtonState.Released && this.PreviousState.RightButton == ButtonState.Pressed;
        }
      }

      public bool ReleasedMiddleButton
      {
        get
        {
          return this.CurrentState.MiddleButton == ButtonState.Released && this.PreviousState.MiddleButton == ButtonState.Pressed;
        }
      }

      public int Wheel
      {
        get
        {
          return this.CurrentState.ScrollWheelValue;
        }
      }

      public int WheelDelta
      {
        get
        {
          return this.CurrentState.ScrollWheelValue - this.PreviousState.ScrollWheelValue;
        }
      }

      public bool WasMoved
      {
        get
        {
          return this.CurrentState.X != this.PreviousState.X || this.CurrentState.Y != this.PreviousState.Y;
        }
      }

      public float X
      {
        get
        {
          return this.Position.X;
        }
        set
        {
          this.Position = new Vector2(value, this.Position.Y);
        }
      }

      public float Y
      {
        get
        {
          return this.Position.Y;
        }
        set
        {
          this.Position = new Vector2(this.Position.X, value);
        }
      }

      public Vector2 Position
      {
        get
        {
          return Vector2.Transform(new Vector2((float) this.CurrentState.X, (float) this.CurrentState.Y), Matrix.Invert(Engine.ScreenMatrix));
        }
        set
        {
          Vector2 vector2 = Vector2.Transform(value, Engine.ScreenMatrix);
          Microsoft.Xna.Framework.Input.Mouse.SetPosition((int) Math.Round((double) vector2.X), (int) Math.Round((double) vector2.Y));
        }
      }
    }

    public class GamePadData
    {
      public readonly PlayerIndex PlayerIndex;
      public GamePadState PreviousState;
      public GamePadState CurrentState;
      public bool Attached;
      private float rumbleStrength;
      private float rumbleTime;

      internal GamePadData(int playerIndex)
      {
        this.PlayerIndex = (PlayerIndex) Calc.Clamp(playerIndex, 0, 3);
      }

      public void Update()
      {
        this.PreviousState = this.CurrentState;
        this.CurrentState = GamePad.GetState(this.PlayerIndex);
        this.Attached = this.CurrentState.IsConnected;
        if ((double) this.rumbleTime <= 0.0)
          return;
        this.rumbleTime -= Engine.DeltaTime;
        if ((double) this.rumbleTime <= 0.0)
          GamePad.SetVibration(this.PlayerIndex, 0.0f, 0.0f);
      }

      public void UpdateNull()
      {
        this.PreviousState = this.CurrentState;
        this.CurrentState = new GamePadState();
        this.Attached = GamePad.GetState(this.PlayerIndex).IsConnected;
        if ((double) this.rumbleTime > 0.0)
          this.rumbleTime -= Engine.DeltaTime;
        GamePad.SetVibration(this.PlayerIndex, 0.0f, 0.0f);
      }

      public void Rumble(float strength, float time)
      {
        if ((double) this.rumbleTime > 0.0 && (double) strength <= (double) this.rumbleStrength && ((double) strength != (double) this.rumbleStrength || (double) time <= (double) this.rumbleTime))
          return;
        GamePad.SetVibration(this.PlayerIndex, strength, strength);
        this.rumbleStrength = strength;
        this.rumbleTime = time;
      }

      public void StopRumble()
      {
        GamePad.SetVibration(this.PlayerIndex, 0.0f, 0.0f);
        this.rumbleTime = 0.0f;
      }

      public bool Check(Buttons button)
      {
        if (MInput.Disabled)
          return false;
        return this.CurrentState.IsButtonDown(button);
      }

      public bool Pressed(Buttons button)
      {
        if (MInput.Disabled)
          return false;
        return this.CurrentState.IsButtonDown(button) && this.PreviousState.IsButtonUp(button);
      }

      public bool Released(Buttons button)
      {
        if (MInput.Disabled)
          return false;
        return this.CurrentState.IsButtonUp(button) && this.PreviousState.IsButtonDown(button);
      }

      public bool Check(Buttons buttonA, Buttons buttonB)
      {
        return this.Check(buttonA) || this.Check(buttonB);
      }

      public bool Pressed(Buttons buttonA, Buttons buttonB)
      {
        return this.Pressed(buttonA) || this.Pressed(buttonB);
      }

      public bool Released(Buttons buttonA, Buttons buttonB)
      {
        return this.Released(buttonA) || this.Released(buttonB);
      }

      public bool Check(Buttons buttonA, Buttons buttonB, Buttons buttonC)
      {
        return this.Check(buttonA) || this.Check(buttonB) || this.Check(buttonC);
      }

      public bool Pressed(Buttons buttonA, Buttons buttonB, Buttons buttonC)
      {
        return this.Pressed(buttonA) || this.Pressed(buttonB) || this.Check(buttonC);
      }

      public bool Released(Buttons buttonA, Buttons buttonB, Buttons buttonC)
      {
        return this.Released(buttonA) || this.Released(buttonB) || this.Check(buttonC);
      }

      public Vector2 GetLeftStick()
      {
        Vector2 left = this.CurrentState.ThumbSticks.Left;
        left.Y = -left.Y;
        return left;
      }

      public Vector2 GetLeftStick(float deadzone)
      {
        Vector2 vector2 = this.CurrentState.ThumbSticks.Left;
        if ((double) vector2.LengthSquared() < (double) deadzone * (double) deadzone)
          vector2 = Vector2.Zero;
        else
          vector2.Y = -vector2.Y;
        return vector2;
      }

      public Vector2 GetRightStick()
      {
        Vector2 right = this.CurrentState.ThumbSticks.Right;
        right.Y = -right.Y;
        return right;
      }

      public Vector2 GetRightStick(float deadzone)
      {
        Vector2 vector2 = this.CurrentState.ThumbSticks.Right;
        if ((double) vector2.LengthSquared() < (double) deadzone * (double) deadzone)
          vector2 = Vector2.Zero;
        else
          vector2.Y = -vector2.Y;
        return vector2;
      }

      public bool LeftStickLeftCheck(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Left.X <= -(double) deadzone;
      }

      public bool LeftStickLeftPressed(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Left.X <= -(double) deadzone && (double) this.PreviousState.ThumbSticks.Left.X > -(double) deadzone;
      }

      public bool LeftStickLeftReleased(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Left.X > -(double) deadzone && (double) this.PreviousState.ThumbSticks.Left.X <= -(double) deadzone;
      }

      public bool LeftStickRightCheck(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Left.X >= (double) deadzone;
      }

      public bool LeftStickRightPressed(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Left.X >= (double) deadzone && (double) this.PreviousState.ThumbSticks.Left.X < (double) deadzone;
      }

      public bool LeftStickRightReleased(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Left.X < (double) deadzone && (double) this.PreviousState.ThumbSticks.Left.X >= (double) deadzone;
      }

      public bool LeftStickDownCheck(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Left.Y <= -(double) deadzone;
      }

      public bool LeftStickDownPressed(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Left.Y <= -(double) deadzone && (double) this.PreviousState.ThumbSticks.Left.Y > -(double) deadzone;
      }

      public bool LeftStickDownReleased(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Left.Y > -(double) deadzone && (double) this.PreviousState.ThumbSticks.Left.Y <= -(double) deadzone;
      }

      public bool LeftStickUpCheck(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Left.Y >= (double) deadzone;
      }

      public bool LeftStickUpPressed(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Left.Y >= (double) deadzone && (double) this.PreviousState.ThumbSticks.Left.Y < (double) deadzone;
      }

      public bool LeftStickUpReleased(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Left.Y < (double) deadzone && (double) this.PreviousState.ThumbSticks.Left.Y >= (double) deadzone;
      }

      public float LeftStickHorizontal(float deadzone)
      {
        float x = this.CurrentState.ThumbSticks.Left.X;
        if ((double) Math.Abs(x) < (double) deadzone)
          return 0.0f;
        return x;
      }

      public float LeftStickVertical(float deadzone)
      {
        float y = this.CurrentState.ThumbSticks.Left.Y;
        if ((double) Math.Abs(y) < (double) deadzone)
          return 0.0f;
        return -y;
      }

      public bool RightStickLeftCheck(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Right.X <= -(double) deadzone;
      }

      public bool RightStickLeftPressed(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Right.X <= -(double) deadzone && (double) this.PreviousState.ThumbSticks.Right.X > -(double) deadzone;
      }

      public bool RightStickLeftReleased(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Right.X > -(double) deadzone && (double) this.PreviousState.ThumbSticks.Right.X <= -(double) deadzone;
      }

      public bool RightStickRightCheck(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Right.X >= (double) deadzone;
      }

      public bool RightStickRightPressed(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Right.X >= (double) deadzone && (double) this.PreviousState.ThumbSticks.Right.X < (double) deadzone;
      }

      public bool RightStickRightReleased(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Right.X < (double) deadzone && (double) this.PreviousState.ThumbSticks.Right.X >= (double) deadzone;
      }

      public bool RightStickUpCheck(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Right.Y <= -(double) deadzone;
      }

      public bool RightStickUpPressed(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Right.Y <= -(double) deadzone && (double) this.PreviousState.ThumbSticks.Right.Y > -(double) deadzone;
      }

      public bool RightStickUpReleased(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Right.Y > -(double) deadzone && (double) this.PreviousState.ThumbSticks.Right.Y <= -(double) deadzone;
      }

      public bool RightStickDownCheck(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Right.Y >= (double) deadzone;
      }

      public bool RightStickDownPressed(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Right.Y >= (double) deadzone && (double) this.PreviousState.ThumbSticks.Right.Y < (double) deadzone;
      }

      public bool RightStickDownReleased(float deadzone)
      {
        return (double) this.CurrentState.ThumbSticks.Right.Y < (double) deadzone && (double) this.PreviousState.ThumbSticks.Right.Y >= (double) deadzone;
      }

      public float RightStickHorizontal(float deadzone)
      {
        float x = this.CurrentState.ThumbSticks.Right.X;
        if ((double) Math.Abs(x) < (double) deadzone)
          return 0.0f;
        return x;
      }

      public float RightStickVertical(float deadzone)
      {
        float y = this.CurrentState.ThumbSticks.Right.Y;
        if ((double) Math.Abs(y) < (double) deadzone)
          return 0.0f;
        return -y;
      }

      public int DPadHorizontal
      {
        get
        {
          return this.CurrentState.DPad.Right == ButtonState.Pressed ? 1 : (this.CurrentState.DPad.Left == ButtonState.Pressed ? -1 : 0);
        }
      }

      public int DPadVertical
      {
        get
        {
          return this.CurrentState.DPad.Down == ButtonState.Pressed ? 1 : (this.CurrentState.DPad.Up == ButtonState.Pressed ? -1 : 0);
        }
      }

      public Vector2 DPad
      {
        get
        {
          return new Vector2((float) this.DPadHorizontal, (float) this.DPadVertical);
        }
      }

      public bool DPadLeftCheck
      {
        get
        {
          return this.CurrentState.DPad.Left == ButtonState.Pressed;
        }
      }

      public bool DPadLeftPressed
      {
        get
        {
          return this.CurrentState.DPad.Left == ButtonState.Pressed && this.PreviousState.DPad.Left == ButtonState.Released;
        }
      }

      public bool DPadLeftReleased
      {
        get
        {
          return this.CurrentState.DPad.Left == ButtonState.Released && this.PreviousState.DPad.Left == ButtonState.Pressed;
        }
      }

      public bool DPadRightCheck
      {
        get
        {
          return this.CurrentState.DPad.Right == ButtonState.Pressed;
        }
      }

      public bool DPadRightPressed
      {
        get
        {
          return this.CurrentState.DPad.Right == ButtonState.Pressed && this.PreviousState.DPad.Right == ButtonState.Released;
        }
      }

      public bool DPadRightReleased
      {
        get
        {
          return this.CurrentState.DPad.Right == ButtonState.Released && this.PreviousState.DPad.Right == ButtonState.Pressed;
        }
      }

      public bool DPadUpCheck
      {
        get
        {
          return this.CurrentState.DPad.Up == ButtonState.Pressed;
        }
      }

      public bool DPadUpPressed
      {
        get
        {
          return this.CurrentState.DPad.Up == ButtonState.Pressed && this.PreviousState.DPad.Up == ButtonState.Released;
        }
      }

      public bool DPadUpReleased
      {
        get
        {
          return this.CurrentState.DPad.Up == ButtonState.Released && this.PreviousState.DPad.Up == ButtonState.Pressed;
        }
      }

      public bool DPadDownCheck
      {
        get
        {
          return this.CurrentState.DPad.Down == ButtonState.Pressed;
        }
      }

      public bool DPadDownPressed
      {
        get
        {
          return this.CurrentState.DPad.Down == ButtonState.Pressed && this.PreviousState.DPad.Down == ButtonState.Released;
        }
      }

      public bool DPadDownReleased
      {
        get
        {
          return this.CurrentState.DPad.Down == ButtonState.Released && this.PreviousState.DPad.Down == ButtonState.Pressed;
        }
      }

      public bool LeftTriggerCheck(float threshold)
      {
        if (MInput.Disabled)
          return false;
        return (double) this.CurrentState.Triggers.Left >= (double) threshold;
      }

      public bool LeftTriggerPressed(float threshold)
      {
        if (MInput.Disabled)
          return false;
        GamePadTriggers triggers = this.CurrentState.Triggers;
        int num;
        if ((double) triggers.Left >= (double) threshold)
        {
          triggers = this.PreviousState.Triggers;
          num = (double) triggers.Left < (double) threshold ? 1 : 0;
        }
        else
          num = 0;
        return num != 0;
      }

      public bool LeftTriggerReleased(float threshold)
      {
        if (MInput.Disabled)
          return false;
        GamePadTriggers triggers = this.CurrentState.Triggers;
        int num;
        if ((double) triggers.Left < (double) threshold)
        {
          triggers = this.PreviousState.Triggers;
          num = (double) triggers.Left >= (double) threshold ? 1 : 0;
        }
        else
          num = 0;
        return num != 0;
      }

      public bool RightTriggerCheck(float threshold)
      {
        if (MInput.Disabled)
          return false;
        return (double) this.CurrentState.Triggers.Right >= (double) threshold;
      }

      public bool RightTriggerPressed(float threshold)
      {
        if (MInput.Disabled)
          return false;
        GamePadTriggers triggers = this.CurrentState.Triggers;
        int num;
        if ((double) triggers.Right >= (double) threshold)
        {
          triggers = this.PreviousState.Triggers;
          num = (double) triggers.Right < (double) threshold ? 1 : 0;
        }
        else
          num = 0;
        return num != 0;
      }

      public bool RightTriggerReleased(float threshold)
      {
        if (MInput.Disabled)
          return false;
        GamePadTriggers triggers = this.CurrentState.Triggers;
        int num;
        if ((double) triggers.Right < (double) threshold)
        {
          triggers = this.PreviousState.Triggers;
          num = (double) triggers.Right >= (double) threshold ? 1 : 0;
        }
        else
          num = 0;
        return num != 0;
      }
    }
  }
}

