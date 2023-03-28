// Decompiled with JetBrains decompiler
// Type: Celeste.RunThread
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Monocle;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Celeste
{
    public static class RunThread
    {
        private static List<Thread> threads = new List<Thread>();

        public static void Start(Action method, string name, bool highPriority = false)
        {
            Thread thread = new Thread((ThreadStart) (() => RunThread.RunThreadWithLogging(method)));
            lock (RunThread.threads)
                RunThread.threads.Add(thread);
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
                Console.WriteLine(ex.ToString());
                ErrorLog.Write(ex);
                ErrorLog.Open();
                Engine.Instance.Exit();
            }
            finally
            {
                lock (RunThread.threads)
                    RunThread.threads.Remove(Thread.CurrentThread);
            }
        }

        public static void WaitAll()
        {
            label_0:
            Thread thread;
            lock (RunThread.threads)
            {
                if (RunThread.threads.Count == 0)
                    return;
                thread = RunThread.threads[0];
            }
            while (thread.IsAlive)
                Engine.Instance.GraphicsDevice.Present();
            goto label_0;
        }
    }
}