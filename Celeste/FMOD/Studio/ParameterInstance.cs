// Decompiled with JetBrains decompiler
// Type: FMOD.Studio.ParameterInstance
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
  public class ParameterInstance : HandleBase
  {
    public RESULT getDescription(out PARAMETER_DESCRIPTION description)
    {
      description = new PARAMETER_DESCRIPTION();
      PARAMETER_DESCRIPTION_INTERNAL description1;
      RESULT description2 = ParameterInstance.FMOD_Studio_ParameterInstance_GetDescription(this.rawPtr, out description1);
      if (description2 != RESULT.OK)
        return description2;
      description1.assign(out description);
      return description2;
    }

    public RESULT getValue(out float value)
    {
      return ParameterInstance.FMOD_Studio_ParameterInstance_GetValue(this.rawPtr, out value);
    }

    public RESULT setValue(float value)
    {
      return ParameterInstance.FMOD_Studio_ParameterInstance_SetValue(this.rawPtr, value);
    }

    [DllImport("fmodstudio")]
    private static extern bool FMOD_Studio_ParameterInstance_IsValid(IntPtr parameter);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_ParameterInstance_GetDescription(
      IntPtr parameter,
      out PARAMETER_DESCRIPTION_INTERNAL description);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_ParameterInstance_GetValue(
      IntPtr parameter,
      out float value);

    [DllImport("fmodstudio")]
    private static extern RESULT FMOD_Studio_ParameterInstance_SetValue(
      IntPtr parameter,
      float value);

    public ParameterInstance(IntPtr raw)
      : base(raw)
    {
    }

    protected override bool isValidInternal()
    {
      return ParameterInstance.FMOD_Studio_ParameterInstance_IsValid(this.rawPtr);
    }
  }
}
