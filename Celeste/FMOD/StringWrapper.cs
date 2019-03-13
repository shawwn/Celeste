// Decompiled with JetBrains decompiler
// Type: FMOD.StringWrapper
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
  public struct StringWrapper
  {
    private IntPtr nativeUtf8Ptr;

    public static implicit operator string(StringWrapper fstring)
    {
      if (fstring.nativeUtf8Ptr == IntPtr.Zero)
        return "";
      int length = 0;
      while (Marshal.ReadByte(fstring.nativeUtf8Ptr, length) != (byte) 0)
        ++length;
      if (length <= 0)
        return "";
      byte[] numArray = new byte[length];
      Marshal.Copy(fstring.nativeUtf8Ptr, numArray, 0, length);
      return Encoding.UTF8.GetString(numArray, 0, length);
    }
  }
}
