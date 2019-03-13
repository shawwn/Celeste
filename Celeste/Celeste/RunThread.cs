// Decompiled with JetBrains decompiler
// Type: Celeste.RunThread
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;
using System;
using System.Threading;

namespace Celeste
{
  public static class RunThread
  {
    public static void Start(Action method, string name, bool highPriority = false)
    {
      Thread thread = new Thread((ThreadStart) (() => RunThread.RunThreadWithLogging(method)));
      thread.Name = name;
      thread.IsBackground = true;
      if (highPriority)
        thread.Priority = ThreadPriority.Highest;
      thread.Start();
    }

    private static void RunThreadWithLogging(Action method)
    {
      try
      {
        method();
      }
      catch (Exception ex)
      {
        ErrorLog.Write(ex);
        ErrorLog.Open();
        Engine.Instance.Exit();
      }
    }
  }
}
