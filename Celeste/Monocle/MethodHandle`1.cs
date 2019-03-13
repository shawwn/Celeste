// Decompiled with JetBrains decompiler
// Type: Monocle.MethodHandle`1
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System.Reflection;

namespace Monocle
{
  public class MethodHandle<T> where T : Entity
  {
    private MethodInfo info;

    public MethodHandle(string methodName)
    {
      this.info = typeof (T).GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic);
    }

    public void Call(T instance)
    {
      this.info.Invoke((object) instance, (object[]) null);
    }
  }
}
