// Decompiled with JetBrains decompiler
// Type: FMOD.DEBUG_CALLBACK
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

namespace FMOD
{
  public delegate RESULT DEBUG_CALLBACK(
    DEBUG_FLAGS flags,
    string file,
    int line,
    string func,
    string message);
}
