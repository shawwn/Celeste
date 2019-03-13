// Decompiled with JetBrains decompiler
// Type: Monocle.QuaternionExt
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;

namespace Monocle
{
  public static class QuaternionExt
  {
    public static Quaternion Conjugated(this Quaternion q)
    {
      Quaternion quaternion = q;
      ((Quaternion) ref quaternion).Conjugate();
      return quaternion;
    }

    public static Quaternion LookAt(
      this Quaternion q,
      Vector3 from,
      Vector3 to,
      Vector3 up)
    {
      return Quaternion.CreateFromRotationMatrix(Matrix.CreateLookAt(from, to, up));
    }

    public static Quaternion LookAt(this Quaternion q, Vector3 direction, Vector3 up)
    {
      return Quaternion.CreateFromRotationMatrix(Matrix.CreateLookAt(Vector3.get_Zero(), direction, up));
    }

    public static Vector3 Forward(this Quaternion q)
    {
      return Vector3.Transform(Vector3.get_Forward(), q.Conjugated());
    }

    public static Vector3 Left(this Quaternion q)
    {
      return Vector3.Transform(Vector3.get_Left(), q.Conjugated());
    }

    public static Vector3 Up(this Quaternion q)
    {
      return Vector3.Transform(Vector3.get_Up(), q.Conjugated());
    }
  }
}
