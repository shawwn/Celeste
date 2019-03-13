// Decompiled with JetBrains decompiler
// Type: Celeste.PlayerInventory
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace Celeste
{
  [Serializable]
  public struct PlayerInventory
  {
    public static readonly PlayerInventory Prologue = new PlayerInventory(0, false, true, false);
    public static readonly PlayerInventory Default = new PlayerInventory(1, true, true, false);
    public static readonly PlayerInventory OldSite = new PlayerInventory(1, false, true, false);
    public static readonly PlayerInventory CH6End = new PlayerInventory(2, true, true, false);
    public static readonly PlayerInventory TheSummit = new PlayerInventory(2, true, false, false);
    public static readonly PlayerInventory Core = new PlayerInventory(2, true, true, true);
    public static readonly PlayerInventory Farewell = new PlayerInventory(1, true, false, false);
    public int Dashes;
    public bool DreamDash;
    public bool Backpack;
    public bool NoRefills;

    public PlayerInventory(int dashes = 1, bool dreamDash = true, bool backpack = true, bool noRefills = false)
    {
      this.Dashes = dashes;
      this.DreamDash = dreamDash;
      this.Backpack = backpack;
      this.NoRefills = noRefills;
    }
  }
}
