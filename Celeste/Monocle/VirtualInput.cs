// Decompiled with JetBrains decompiler
// Type: Monocle.VirtualInput
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

namespace Monocle
{
    public abstract class VirtualInput
    {
        public VirtualInput() => MInput.VirtualInputs.Add(this);

        public void Deregister() => MInput.VirtualInputs.Remove(this);

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