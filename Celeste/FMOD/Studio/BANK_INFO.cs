// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.BANK_INFO
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD.Studio
{
  public struct BANK_INFO
  {
    public int size;
    public IntPtr userdata;
    public int userdatalength;
    public FILE_OPENCALLBACK opencallback;
    public FILE_CLOSECALLBACK closecallback;
    public FILE_READCALLBACK readcallback;
    public FILE_SEEKCALLBACK seekcallback;
  }
}
