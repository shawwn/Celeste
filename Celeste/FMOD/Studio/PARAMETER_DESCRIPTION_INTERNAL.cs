// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.PARAMETER_DESCRIPTION_INTERNAL
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD.Studio
{
  internal struct PARAMETER_DESCRIPTION_INTERNAL
  {
    public IntPtr name;
    public int index;
    public float minimum;
    public float maximum;
    public float defaultvalue;
    public PARAMETER_TYPE type;

    public void assign(out PARAMETER_DESCRIPTION publicDesc)
    {
      publicDesc.name = MarshallingHelper.stringFromNativeUtf8(this.name);
      publicDesc.index = this.index;
      publicDesc.minimum = this.minimum;
      publicDesc.maximum = this.maximum;
      publicDesc.defaultvalue = this.defaultvalue;
      publicDesc.type = this.type;
    }
  }
}
