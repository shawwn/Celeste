// Decompiled with JetBrains decompiler
// Type: FMOD.StringMarshalHelper
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
  internal class StringMarshalHelper
  {
    internal static void NativeToBuilder(StringBuilder builder, IntPtr nativeMem)
    {
      byte[] numArray = new byte[builder.Capacity];
      Marshal.Copy(nativeMem, numArray, 0, builder.Capacity);
      int count = Array.IndexOf<byte>(numArray, (byte) 0);
      if (count <= 0)
        return;
      string str = Encoding.UTF8.GetString(numArray, 0, count);
      builder.Append(str);
    }
  }
}
