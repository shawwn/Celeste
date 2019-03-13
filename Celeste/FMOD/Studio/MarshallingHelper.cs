// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.MarshallingHelper
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
  internal class MarshallingHelper
  {
    public static int stringLengthUtf8(IntPtr nativeUtf8)
    {
      int ofs = 0;
      while (Marshal.ReadByte(nativeUtf8, ofs) != (byte) 0)
        ++ofs;
      return ofs;
    }

    public static string stringFromNativeUtf8(IntPtr nativeUtf8)
    {
      int count = MarshallingHelper.stringLengthUtf8(nativeUtf8);
      if (count == 0)
        return string.Empty;
      byte[] numArray = new byte[count];
      Marshal.Copy(nativeUtf8, numArray, 0, numArray.Length);
      return Encoding.UTF8.GetString(numArray, 0, count);
    }
  }
}
