﻿// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.SYSTEM_CALLBACK
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD.Studio
{
  public delegate RESULT SYSTEM_CALLBACK(
    IntPtr systemraw,
    SYSTEM_CALLBACK_TYPE type,
    IntPtr parameters,
    IntPtr userdata);
}
