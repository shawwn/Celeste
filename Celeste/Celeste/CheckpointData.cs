// Decompiled with JetBrains decompiler
// Type: Celeste.CheckpointData
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System.Collections.Generic;

namespace Celeste
{
  public class CheckpointData
  {
    public string Level;
    public string Name;
    public bool Dreaming;
    public int Strawberries;
    public PlayerInventory? Inventory;
    public AudioState AudioState;
    public HashSet<string> Flags;
    public Session.CoreModes? CoreMode;

    public CheckpointData(
      string level,
      string name,
      PlayerInventory? inventory = null,
      bool dreaming = false,
      AudioState audioState = null)
    {
      this.Level = level;
      this.Name = name;
      this.Inventory = inventory;
      this.Dreaming = dreaming;
      this.AudioState = audioState;
      this.CoreMode = new Session.CoreModes?();
    }
  }
}
