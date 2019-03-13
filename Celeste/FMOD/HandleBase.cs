// Decompiled with JetBrains decompiler
// Type: FMOD.HandleBase
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD
{
  public class HandleBase
  {
    protected IntPtr rawPtr;

    public HandleBase(IntPtr newPtr)
    {
      this.rawPtr = newPtr;
    }

    public bool isValid()
    {
      return this.rawPtr != IntPtr.Zero;
    }

    public IntPtr getRaw()
    {
      return this.rawPtr;
    }

    public override bool Equals(object obj)
    {
      return this.Equals(obj as HandleBase);
    }

    public bool Equals(HandleBase p)
    {
      if ((object) p != null)
        return this.rawPtr == p.rawPtr;
      return false;
    }

    public override int GetHashCode()
    {
      return this.rawPtr.ToInt32();
    }

    public static bool operator ==(HandleBase a, HandleBase b)
    {
      if ((object) a == (object) b)
        return true;
      if ((object) a == null || (object) b == null)
        return false;
      return a.rawPtr == b.rawPtr;
    }

    public static bool operator !=(HandleBase a, HandleBase b)
    {
      return !(a == b);
    }
  }
}
