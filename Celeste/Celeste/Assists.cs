// Decompiled with JetBrains decompiler
// Type: Celeste.Assists
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;
using System;

namespace Celeste
{
  [Serializable]
  public struct Assists
  {
    public int GameSpeed;
    public bool Invincible;
    public Assists.DashModes DashMode;
    public bool InfiniteStamina;
    public bool MirrorMode;
    public bool ThreeSixtyDashing;
    public bool InvisibleMotion;
    public bool NoGrabbing;
    public bool LowFriction;
    public bool SuperDashing;
    public bool Hiccups;
    public bool PlayAsBadeline;

    public static Assists Default
    {
      get
      {
        return new Assists() { GameSpeed = 10 };
      }
    }

    public void EnfornceAssistMode()
    {
      this.GameSpeed = Calc.Clamp(this.GameSpeed, 5, 10);
      this.MirrorMode = false;
      this.ThreeSixtyDashing = false;
      this.InvisibleMotion = false;
      this.NoGrabbing = false;
      this.LowFriction = false;
      this.SuperDashing = false;
      this.Hiccups = false;
      this.PlayAsBadeline = false;
    }

    public enum DashModes
    {
      Normal,
      Two,
      Infinite,
    }
  }
}
