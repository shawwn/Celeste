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
      if (Engine.Instance.get_IsActive() && MInput.Active)
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
        this.CurrentState = (KeyboardState) null;
      }

      public bool Check(Keys key)
      {
        if (MInput.Disabled)
          return false;
        return ((KeyboardState) ref this.CurrentState).IsKeyDown(key);
      }

      public bool Pressed(Keys key)
      {
        if (MInput.Disabled || !((KeyboardState) ref this.CurrentState).IsKeyDown(key))
          return false;
        return !((KeyboardState) ref this.PreviousState).IsKeyDown(key);
      }

      public bool Released(Keys key)
      {
        if (MInput.Disabled || ((KeyboardState) ref this.CurrentState).IsKeyDown(key))
          return false;
        return ((KeyboardState) ref this.PreviousState).IsKeyDown(key);
      }

      public bool Check(Keys keyA, Keys keyB)
      {
        if (!this.Check(keyA))
          return this.Check(keyB);
        return true;
      }

      public bool Pressed(Keys keyA, Keys keyB)
      {
        if (!this.Pressed(keyA))
          return this.Pressed(keyB);
        return true;
      }

      public bool Released(Keys keyA, Keys keyB)
      {
        if (!this.Released(keyA))
          return this.Released(keyB);
        return true;
      }

      public bool Check(Keys keyA, Keys keyB, Keys keyC)
      {
        if (!this.Check(keyA) && !this.Check(keyB))
          return this.Check(keyC);
        return true;
      }

      public bool Pressed(Keys keyA, Keys keyB, Keys keyC)
      {
        if (!this.Pressed(keyA) && !this.Pressed(keyB))
          return this.Pressed(keyC);
        return true;
      }

      public bool Released(Keys keyA, Keys keyB, Keys keyC)
      {
        if (!this.Released(keyA) && !this.Released(keyB))
          return this.Released(keyC);
        return true;
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
        this.PreviousState = (MouseState) null;
        this.CurrentState = (MouseState) null;
      }

      internal void Update()
      {
        this.PreviousState = this.CurrentState;
        this.CurrentState = Microsoft.Xna.Framework.Input.Mouse.GetState();
      }

      internal void UpdateNull()
      {
        this.PreviousState = this.CurrentState;
        this.CurrentState = (MouseState) null;
      }

      public bool CheckLeftButton
      {
        get
        {
          return ((MouseState) ref this.CurrentState).get_LeftButton() == 1;
        }
      }

      public bool CheckRightButton
      {
        get
        {
          return ((MouseState) ref this.CurrentState).get_RightButton() == 1;
        }
      }

      public bool CheckMiddleButton
      {
        get
        {
          return ((MouseState) ref this.CurrentState).get_MiddleButton() == 1;
        }
      }

      public bool PressedLeftButton
      {
        get
        {
          if (((MouseState) ref this.CurrentState).get_LeftButton() == 1)
            return ((MouseState) ref this.PreviousState).get_LeftButton() == 0;
          return false;
        }
      }

      public bool PressedRightButton
      {
        get
        {
          if (((MouseState) ref this.CurrentState).get_RightButton() == 1)
            return ((MouseState) ref this.PreviousState).get_RightButton() == 0;
          return false;
        }
      }

      public bool PressedMiddleButton
      {
        get
        {
          if (((MouseState) ref this.CurrentState).get_MiddleButton() == 1)
            return ((MouseState) ref this.PreviousState).get_MiddleButton() == 0;
          return false;
        }
      }

      public bool ReleasedLeftButton
      {
        get
        {
          if (((MouseState) ref this.CurrentState).get_LeftButton() == null)
            return ((MouseState) ref this.PreviousState).get_LeftButton() == 1;
          return false;
        }
      }

      public bool ReleasedRightButton
      {
        get
        {
          if (((MouseState) ref this.CurrentState).get_RightButton() == null)
            return ((MouseState) ref this.PreviousState).get_RightButton() == 1;
          return false;
        }
      }

      public bool ReleasedMiddleButton
      {
        get
        {
          if (((MouseState) ref this.CurrentState).get_MiddleButton() == null)
            return ((MouseState) ref this.PreviousState).get_MiddleButton() == 1;
          return false;
        }
      }

      public int Wheel
      {
        get
        {
          return ((MouseState) ref this.CurrentState).get_ScrollWheelValue();
        }
      }

      public int WheelDelta
      {
        get
        {
          return ((MouseState) ref this.CurrentState).get_ScrollWheelValue() - ((MouseState) ref this.PreviousState).get_ScrollWheelValue();
        }
      }

      public bool WasMoved
      {
        get
        {
          if (((MouseState) ref this.CurrentState).get_X() == ((MouseState) ref this.PreviousState).get_X())
            return ((MouseState) ref this.CurrentState).get_Y() != ((MouseState) ref this.PreviousState).get_Y();
          return true;
        }
      }

      public float X
      {
        get
        {
          return (float) this.Position.X;
        }
        set
        {
          this.Position = new Vector2(value, (float) this.Position.Y);
        }
      }

      public float Y
      {
        get
        {
          return (float) this.Position.Y;
        }
        set
        {
          this.Position = new Vector2((float) this.Position.X, value);
        }
      }

      public Vector2 Position
      {
        get
        {
          return Vector2.Transform(new Vector2((float) ((MouseState) ref this.CurrentState).get_X(), (float) ((MouseState) ref this.CurrentState).get_Y()), Matrix.Invert(Engine.ScreenMatrix));
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
        this.Attached = ((GamePadState) ref this.CurrentState).get_IsConnected();
        if ((double) this.rumbleTime <= 0.0)
          return;
        this.rumbleTime -= Engine.DeltaTime;
        if ((double) this.rumbleTime > 0.0)
          return;
        GamePad.SetVibration(this.PlayerIndex, 0.0f, 0.0f);
      }

      public void UpdateNull()
      {
        this.PreviousState = this.CurrentState;
        this.CurrentState = (GamePadState) null;
        GamePadState state = GamePad.GetState(this.PlayerIndex);
        this.Attached = ((GamePadState) ref state).get_IsConnected();
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
        return ((GamePadState) ref this.CurrentState).IsButtonDown(button);
      }

      public bool Pressed(Buttons button)
      {
        if (MInput.Disabled || !((GamePadState) ref this.CurrentState).IsButtonDown(button))
          return false;
        return ((GamePadState) ref this.PreviousState).IsButtonUp(button);
      }

      public bool Released(Buttons button)
      {
        if (MInput.Disabled || !((GamePadState) ref this.CurrentState).IsButtonUp(button))
          return false;
        return ((GamePadState) ref this.PreviousState).IsButtonDown(button);
      }

      public bool Check(Buttons buttonA, Buttons buttonB)
      {
        if (!this.Check(buttonA))
          return this.Check(buttonB);
        return true;
      }

      public bool Pressed(Buttons buttonA, Buttons buttonB)
      {
        if (!this.Pressed(buttonA))
          return this.Pressed(buttonB);
        return true;
      }

      public bool Released(Buttons buttonA, Buttons buttonB)
      {
        if (!this.Released(buttonA))
          return this.Released(buttonB);
        return true;
      }

      public bool Check(Buttons buttonA, Buttons buttonB, Buttons buttonC)
      {
        if (!this.Check(buttonA) && !this.Check(buttonB))
          return this.Check(buttonC);
        return true;
      }

      public bool Pressed(Buttons buttonA, Buttons buttonB, Buttons buttonC)
      {
        if (!this.Pressed(buttonA) && !this.Pressed(buttonB))
          return this.Check(buttonC);
        return true;
      }

      public bool Released(Buttons buttonA, Buttons buttonB, Buttons buttonC)
      {
        if (!this.Released(buttonA) && !this.Released(buttonB))
          return this.Check(buttonC);
        return true;
      }

      public Vector2 GetLeftStick()
      {
        GamePadThumbSticks thumbSticks = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        Vector2 left = ((GamePadThumbSticks) ref thumbSticks).get_Left();
        left.Y = -left.Y;
        return left;
      }

      public Vector2 GetLeftStick(float deadzone)
      {
        GamePadThumbSticks thumbSticks = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        Vector2 vector2 = ((GamePadThumbSticks) ref thumbSticks).get_Left();
        if ((double) ((Vector2) ref vector2).LengthSquared() < (double) deadzone * (double) deadzone)
          vector2 = Vector2.get_Zero();
        else
          vector2.Y = -vector2.Y;
        return vector2;
      }

      public Vector2 GetRightStick()
      {
        GamePadThumbSticks thumbSticks = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        Vector2 right = ((GamePadThumbSticks) ref thumbSticks).get_Right();
        right.Y = -right.Y;
        return right;
      }

      public Vector2 GetRightStick(float deadzone)
      {
        GamePadThumbSticks thumbSticks = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        Vector2 vector2 = ((GamePadThumbSticks) ref thumbSticks).get_Right();
        if ((double) ((Vector2) ref vector2).LengthSquared() < (double) deadzone * (double) deadzone)
          vector2 = Vector2.get_Zero();
        else
          vector2.Y = -vector2.Y;
        return vector2;
      }

      public bool LeftStickLeftCheck(float deadzone)
      {
        GamePadThumbSticks thumbSticks = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks).get_Left().X <= -(double) deadzone;
      }

      public bool LeftStickLeftPressed(float deadzone)
      {
        GamePadThumbSticks thumbSticks1 = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        if (((GamePadThumbSticks) ref thumbSticks1).get_Left().X > -(double) deadzone)
          return false;
        GamePadThumbSticks thumbSticks2 = ((GamePadState) ref this.PreviousState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks2).get_Left().X > -(double) deadzone;
      }

      public bool LeftStickLeftReleased(float deadzone)
      {
        GamePadThumbSticks thumbSticks1 = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        if (((GamePadThumbSticks) ref thumbSticks1).get_Left().X <= -(double) deadzone)
          return false;
        GamePadThumbSticks thumbSticks2 = ((GamePadState) ref this.PreviousState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks2).get_Left().X <= -(double) deadzone;
      }

      public bool LeftStickRightCheck(float deadzone)
      {
        GamePadThumbSticks thumbSticks = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks).get_Left().X >= (double) deadzone;
      }

      public bool LeftStickRightPressed(float deadzone)
      {
        GamePadThumbSticks thumbSticks1 = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        if (((GamePadThumbSticks) ref thumbSticks1).get_Left().X < (double) deadzone)
          return false;
        GamePadThumbSticks thumbSticks2 = ((GamePadState) ref this.PreviousState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks2).get_Left().X < (double) deadzone;
      }

      public bool LeftStickRightReleased(float deadzone)
      {
        GamePadThumbSticks thumbSticks1 = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        if (((GamePadThumbSticks) ref thumbSticks1).get_Left().X >= (double) deadzone)
          return false;
        GamePadThumbSticks thumbSticks2 = ((GamePadState) ref this.PreviousState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks2).get_Left().X >= (double) deadzone;
      }

      public bool LeftStickDownCheck(float deadzone)
      {
        GamePadThumbSticks thumbSticks = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks).get_Left().Y <= -(double) deadzone;
      }

      public bool LeftStickDownPressed(float deadzone)
      {
        GamePadThumbSticks thumbSticks1 = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        if (((GamePadThumbSticks) ref thumbSticks1).get_Left().Y > -(double) deadzone)
          return false;
        GamePadThumbSticks thumbSticks2 = ((GamePadState) ref this.PreviousState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks2).get_Left().Y > -(double) deadzone;
      }

      public bool LeftStickDownReleased(float deadzone)
      {
        GamePadThumbSticks thumbSticks1 = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        if (((GamePadThumbSticks) ref thumbSticks1).get_Left().Y <= -(double) deadzone)
          return false;
        GamePadThumbSticks thumbSticks2 = ((GamePadState) ref this.PreviousState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks2).get_Left().Y <= -(double) deadzone;
      }

      public bool LeftStickUpCheck(float deadzone)
      {
        GamePadThumbSticks thumbSticks = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks).get_Left().Y >= (double) deadzone;
      }

      public bool LeftStickUpPressed(float deadzone)
      {
        GamePadThumbSticks thumbSticks1 = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        if (((GamePadThumbSticks) ref thumbSticks1).get_Left().Y < (double) deadzone)
          return false;
        GamePadThumbSticks thumbSticks2 = ((GamePadState) ref this.PreviousState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks2).get_Left().Y < (double) deadzone;
      }

      public bool LeftStickUpReleased(float deadzone)
      {
        GamePadThumbSticks thumbSticks1 = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        if (((GamePadThumbSticks) ref thumbSticks1).get_Left().Y >= (double) deadzone)
          return false;
        GamePadThumbSticks thumbSticks2 = ((GamePadState) ref this.PreviousState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks2).get_Left().Y >= (double) deadzone;
      }

      public float LeftStickHorizontal(float deadzone)
      {
        GamePadThumbSticks thumbSticks = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        float x = (float) ((GamePadThumbSticks) ref thumbSticks).get_Left().X;
        if ((double) Math.Abs(x) < (double) deadzone)
          return 0.0f;
        return x;
      }

      public float LeftStickVertical(float deadzone)
      {
        GamePadThumbSticks thumbSticks = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        float y = (float) ((GamePadThumbSticks) ref thumbSticks).get_Left().Y;
        if ((double) Math.Abs(y) < (double) deadzone)
          return 0.0f;
        return -y;
      }

      public bool RightStickLeftCheck(float deadzone)
      {
        GamePadThumbSticks thumbSticks = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks).get_Right().X <= -(double) deadzone;
      }

      public bool RightStickLeftPressed(float deadzone)
      {
        GamePadThumbSticks thumbSticks1 = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        if (((GamePadThumbSticks) ref thumbSticks1).get_Right().X > -(double) deadzone)
          return false;
        GamePadThumbSticks thumbSticks2 = ((GamePadState) ref this.PreviousState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks2).get_Right().X > -(double) deadzone;
      }

      public bool RightStickLeftReleased(float deadzone)
      {
        GamePadThumbSticks thumbSticks1 = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        if (((GamePadThumbSticks) ref thumbSticks1).get_Right().X <= -(double) deadzone)
          return false;
        GamePadThumbSticks thumbSticks2 = ((GamePadState) ref this.PreviousState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks2).get_Right().X <= -(double) deadzone;
      }

      public bool RightStickRightCheck(float deadzone)
      {
        GamePadThumbSticks thumbSticks = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks).get_Right().X >= (double) deadzone;
      }

      public bool RightStickRightPressed(float deadzone)
      {
        GamePadThumbSticks thumbSticks1 = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        if (((GamePadThumbSticks) ref thumbSticks1).get_Right().X < (double) deadzone)
          return false;
        GamePadThumbSticks thumbSticks2 = ((GamePadState) ref this.PreviousState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks2).get_Right().X < (double) deadzone;
      }

      public bool RightStickRightReleased(float deadzone)
      {
        GamePadThumbSticks thumbSticks1 = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        if (((GamePadThumbSticks) ref thumbSticks1).get_Right().X >= (double) deadzone)
          return false;
        GamePadThumbSticks thumbSticks2 = ((GamePadState) ref this.PreviousState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks2).get_Right().X >= (double) deadzone;
      }

      public bool RightStickUpCheck(float deadzone)
      {
        GamePadThumbSticks thumbSticks = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks).get_Right().Y <= -(double) deadzone;
      }

      public bool RightStickUpPressed(float deadzone)
      {
        GamePadThumbSticks thumbSticks1 = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        if (((GamePadThumbSticks) ref thumbSticks1).get_Right().Y > -(double) deadzone)
          return false;
        GamePadThumbSticks thumbSticks2 = ((GamePadState) ref this.PreviousState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks2).get_Right().Y > -(double) deadzone;
      }

      public bool RightStickUpReleased(float deadzone)
      {
        GamePadThumbSticks thumbSticks1 = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        if (((GamePadThumbSticks) ref thumbSticks1).get_Right().Y <= -(double) deadzone)
          return false;
        GamePadThumbSticks thumbSticks2 = ((GamePadState) ref this.PreviousState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks2).get_Right().Y <= -(double) deadzone;
      }

      public bool RightStickDownCheck(float deadzone)
      {
        GamePadThumbSticks thumbSticks = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks).get_Right().Y >= (double) deadzone;
      }

      public bool RightStickDownPressed(float deadzone)
      {
        GamePadThumbSticks thumbSticks1 = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        if (((GamePadThumbSticks) ref thumbSticks1).get_Right().Y < (double) deadzone)
          return false;
        GamePadThumbSticks thumbSticks2 = ((GamePadState) ref this.PreviousState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks2).get_Right().Y < (double) deadzone;
      }

      public bool RightStickDownReleased(float deadzone)
      {
        GamePadThumbSticks thumbSticks1 = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        if (((GamePadThumbSticks) ref thumbSticks1).get_Right().Y >= (double) deadzone)
          return false;
        GamePadThumbSticks thumbSticks2 = ((GamePadState) ref this.PreviousState).get_ThumbSticks();
        return ((GamePadThumbSticks) ref thumbSticks2).get_Right().Y >= (double) deadzone;
      }

      public float RightStickHorizontal(float deadzone)
      {
        GamePadThumbSticks thumbSticks = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        float x = (float) ((GamePadThumbSticks) ref thumbSticks).get_Right().X;
        if ((double) Math.Abs(x) < (double) deadzone)
          return 0.0f;
        return x;
      }

      public float RightStickVertical(float deadzone)
      {
        GamePadThumbSticks thumbSticks = ((GamePadState) ref this.CurrentState).get_ThumbSticks();
        float y = (float) ((GamePadThumbSticks) ref thumbSticks).get_Right().Y;
        if ((double) Math.Abs(y) < (double) deadzone)
          return 0.0f;
        return -y;
      }

      public int DPadHorizontal
      {
        get
        {
          GamePadDPad dpad1 = ((GamePadState) ref this.CurrentState).get_DPad();
          if (((GamePadDPad) ref dpad1).get_Right() == 1)
            return 1;
          GamePadDPad dpad2 = ((GamePadState) ref this.CurrentState).get_DPad();
          return ((GamePadDPad) ref dpad2).get_Left() != 1 ? 0 : -1;
        }
      }

      public int DPadVertical
      {
        get
        {
          GamePadDPad dpad1 = ((GamePadState) ref this.CurrentState).get_DPad();
          if (((GamePadDPad) ref dpad1).get_Down() == 1)
            return 1;
          GamePadDPad dpad2 = ((GamePadState) ref this.CurrentState).get_DPad();
          return ((GamePadDPad) ref dpad2).get_Up() != 1 ? 0 : -1;
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
          GamePadDPad dpad = ((GamePadState) ref this.CurrentState).get_DPad();
          return ((GamePadDPad) ref dpad).get_Left() == 1;
        }
      }

      public bool DPadLeftPressed
      {
        get
        {
          GamePadDPad dpad1 = ((GamePadState) ref this.CurrentState).get_DPad();
          if (((GamePadDPad) ref dpad1).get_Left() != 1)
            return false;
          GamePadDPad dpad2 = ((GamePadState) ref this.PreviousState).get_DPad();
          return ((GamePadDPad) ref dpad2).get_Left() == 0;
        }
      }

      public bool DPadLeftReleased
      {
        get
        {
          GamePadDPad dpad1 = ((GamePadState) ref this.CurrentState).get_DPad();
          if (((GamePadDPad) ref dpad1).get_Left() != null)
            return false;
          GamePadDPad dpad2 = ((GamePadState) ref this.PreviousState).get_DPad();
          return ((GamePadDPad) ref dpad2).get_Left() == 1;
        }
      }

      public bool DPadRightCheck
      {
        get
        {
          GamePadDPad dpad = ((GamePadState) ref this.CurrentState).get_DPad();
          return ((GamePadDPad) ref dpad).get_Right() == 1;
        }
      }

      public bool DPadRightPressed
      {
        get
        {
          GamePadDPad dpad1 = ((GamePadState) ref this.CurrentState).get_DPad();
          if (((GamePadDPad) ref dpad1).get_Right() != 1)
            return false;
          GamePadDPad dpad2 = ((GamePadState) ref this.PreviousState).get_DPad();
          return ((GamePadDPad) ref dpad2).get_Right() == 0;
        }
      }

      public bool DPadRightReleased
      {
        get
        {
          GamePadDPad dpad1 = ((GamePadState) ref this.CurrentState).get_DPad();
          if (((GamePadDPad) ref dpad1).get_Right() != null)
            return false;
          GamePadDPad dpad2 = ((GamePadState) ref this.PreviousState).get_DPad();
          return ((GamePadDPad) ref dpad2).get_Right() == 1;
        }
      }

      public bool DPadUpCheck
      {
        get
        {
          GamePadDPad dpad = ((GamePadState) ref this.CurrentState).get_DPad();
          return ((GamePadDPad) ref dpad).get_Up() == 1;
        }
      }

      public bool DPadUpPressed
      {
        get
        {
          GamePadDPad dpad1 = ((GamePadState) ref this.CurrentState).get_DPad();
          if (((GamePadDPad) ref dpad1).get_Up() != 1)
            return false;
          GamePadDPad dpad2 = ((GamePadState) ref this.PreviousState).get_DPad();
          return ((GamePadDPad) ref dpad2).get_Up() == 0;
        }
      }

      public bool DPadUpReleased
      {
        get
        {
          GamePadDPad dpad1 = ((GamePadState) ref this.CurrentState).get_DPad();
          if (((GamePadDPad) ref dpad1).get_Up() != null)
            return false;
          GamePadDPad dpad2 = ((GamePadState) ref this.PreviousState).get_DPad();
          return ((GamePadDPad) ref dpad2).get_Up() == 1;
        }
      }

      public bool DPadDownCheck
      {
        get
        {
          GamePadDPad dpad = ((GamePadState) ref this.CurrentState).get_DPad();
          return ((GamePadDPad) ref dpad).get_Down() == 1;
        }
      }

      public bool DPadDownPressed
      {
        get
        {
          GamePadDPad dpad1 = ((GamePadState) ref this.CurrentState).get_DPad();
          if (((GamePadDPad) ref dpad1).get_Down() != 1)
            return false;
          GamePadDPad dpad2 = ((GamePadState) ref this.PreviousState).get_DPad();
          return ((GamePadDPad) ref dpad2).get_Down() == 0;
        }
      }

      public bool DPadDownReleased
      {
        get
        {
          GamePadDPad dpad1 = ((GamePadState) ref this.CurrentState).get_DPad();
          if (((GamePadDPad) ref dpad1).get_Down() != null)
            return false;
          GamePadDPad dpad2 = ((GamePadState) ref this.PreviousState).get_DPad();
          return ((GamePadDPad) ref dpad2).get_Down() == 1;
        }
      }

      public bool LeftTriggerCheck(float threshold)
      {
        if (MInput.Disabled)
          return false;
        GamePadTriggers triggers = ((GamePadState) ref this.CurrentState).get_Triggers();
        return (double) ((GamePadTriggers) ref triggers).get_Left() >= (double) threshold;
      }

      public bool LeftTriggerPressed(float threshold)
      {
        if (MInput.Disabled)
          return false;
        GamePadTriggers triggers1 = ((GamePadState) ref this.CurrentState).get_Triggers();
        if ((double) ((GamePadTriggers) ref triggers1).get_Left() < (double) threshold)
          return false;
        GamePadTriggers triggers2 = ((GamePadState) ref this.PreviousState).get_Triggers();
        return (double) ((GamePadTriggers) ref triggers2).get_Left() < (double) threshold;
      }

      public bool LeftTriggerReleased(float threshold)
      {
        if (MInput.Disabled)
          return false;
        GamePadTriggers triggers1 = ((GamePadState) ref this.CurrentState).get_Triggers();
        if ((double) ((GamePadTriggers) ref triggers1).get_Left() >= (double) threshold)
          return false;
        GamePadTriggers triggers2 = ((GamePadState) ref this.PreviousState).get_Triggers();
        return (double) ((GamePadTriggers) ref triggers2).get_Left() >= (double) threshold;
      }

      public bool RightTriggerCheck(float threshold)
      {
        if (MInput.Disabled)
          return false;
        GamePadTriggers triggers = ((GamePadState) ref this.CurrentState).get_Triggers();
        return (double) ((GamePadTriggers) ref triggers).get_Right() >= (double) threshold;
      }

      public bool RightTriggerPressed(float threshold)
      {
        if (MInput.Disabled)
          return false;
        GamePadTriggers triggers1 = ((GamePadState) ref this.CurrentState).get_Triggers();
        if ((double) ((GamePadTriggers) ref triggers1).get_Right() < (double) threshold)
          return false;
        GamePadTriggers triggers2 = ((GamePadState) ref this.PreviousState).get_Triggers();
        return (double) ((GamePadTriggers) ref triggers2).get_Right() < (double) threshold;
      }

      public bool RightTriggerReleased(float threshold)
      {
        if (MInput.Disabled)
          return false;
        GamePadTriggers triggers1 = ((GamePadState) ref this.CurrentState).get_Triggers();
        if ((double) ((GamePadTriggers) ref triggers1).get_Right() >= (double) threshold)
          return false;
        GamePadTriggers triggers2 = ((GamePadState) ref this.PreviousState).get_Triggers();
        return (double) ((GamePadTriggers) ref triggers2).get_Right() >= (double) threshold;
      }
    }
  }
}
