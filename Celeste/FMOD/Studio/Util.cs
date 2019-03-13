// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.Util
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
  public class Util
  {
    public static RESULT ParseID(string idString, out Guid id)
    {
      return Util.FMOD_Studio_ParseID(Encoding.UTF8.GetBytes(idString + "\0"), out id);
    }

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_ParseID(byte[] idString, out Guid id);
  }
}
