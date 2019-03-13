// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.USER_PROPERTY_INTERNAL
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace FMOD.Studio
{
  internal struct USER_PROPERTY_INTERNAL
  {
    private IntPtr name;
    private USER_PROPERTY_TYPE type;
    private Union_IntBoolFloatString value;

    public USER_PROPERTY createPublic()
    {
      USER_PROPERTY userProperty = new USER_PROPERTY();
      userProperty.name = MarshallingHelper.stringFromNativeUtf8(this.name);
      userProperty.type = this.type;
      switch (this.type)
      {
        case USER_PROPERTY_TYPE.INTEGER:
          userProperty.intvalue = this.value.intvalue;
          break;
        case USER_PROPERTY_TYPE.BOOLEAN:
          userProperty.boolvalue = this.value.boolvalue;
          break;
        case USER_PROPERTY_TYPE.FLOAT:
          userProperty.floatvalue = this.value.floatvalue;
          break;
        case USER_PROPERTY_TYPE.STRING:
          userProperty.stringvalue = MarshallingHelper.stringFromNativeUtf8(this.value.stringvalue);
          break;
      }
      return userProperty;
    }
  }
}
