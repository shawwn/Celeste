// Decompiled with JetBrains decompiler
// Type: Celeste.MountainCamera
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public struct MountainCamera
  {
    public Vector3 Position;
    public Vector3 Target;
    public Quaternion Rotation;

    public MountainCamera(Vector3 pos, Vector3 target)
    {
      this.Position = pos;
      this.Target = target;
      this.Rotation = new Quaternion().LookAt(this.Position, this.Target, Vector3.Up);
    }

    public void LookAt(Vector3 pos)
    {
      this.Target = pos;
      this.Rotation = new Quaternion().LookAt(this.Position, this.Target, Vector3.Up);
    }
  }
}

