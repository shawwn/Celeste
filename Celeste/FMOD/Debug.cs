// Decompiled with JetBrains decompiler
// Type: FMOD.Debug
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System.Runtime.InteropServices;

namespace FMOD
{
  public class Debug
  {
    public static RESULT Initialize(
      DEBUG_FLAGS flags,
      DEBUG_MODE mode,
      DEBUG_CALLBACK callback,
      string filename)
    {
      return Debug.FMOD_Debug_Initialize(flags, mode, callback, filename);
    }

    [DllImport("fmod")]
    private static extern RESULT FMOD_Debug_Initialize(
      DEBUG_FLAGS flags,
      DEBUG_MODE mode,
      DEBUG_CALLBACK callback,
      string filename);
  }
}
