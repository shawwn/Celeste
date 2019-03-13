// Decompiled with JetBrains decompiler
// Type: Monocle.VirtualInput
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

namespace Monocle
{
  public abstract class VirtualInput
  {
    public VirtualInput()
    {
      MInput.VirtualInputs.Add(this);
    }

    public void Deregister()
    {
      MInput.VirtualInputs.Remove(this);
    }

    public abstract void Update();

    public enum OverlapBehaviors
    {
      CancelOut,
      TakeOlder,
      TakeNewer,
    }

    public enum ThresholdModes
    {
      LargerThan,
      LessThan,
      EqualTo,
    }
  }
}
