// Decompiled with JetBrains decompiler
// Type: Celeste.RunLengthEncoding
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System.Collections.Generic;
using System.Text;

namespace Celeste
{
  public static class RunLengthEncoding
  {
    public static byte[] Encode(string str)
    {
      List<byte> byteList = new List<byte>();
      for (int index = 0; index < str.Length; ++index)
      {
        byte num = 1;
        char ch;
        for (ch = str[index]; index + 1 < str.Length && (int) str[index + 1] == (int) ch && num < byte.MaxValue; ++index)
          ++num;
        byteList.Add(num);
        byteList.Add((byte) ch);
      }
      return byteList.ToArray();
    }

    public static string Decode(byte[] bytes)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < bytes.Length; index += 2)
      {
        byte num = bytes[index];
        char ch = (char) bytes[index + 1];
        for (; num > (byte) 0; --num)
          stringBuilder.Append(ch);
      }
      return stringBuilder.ToString();
    }
  }
}
