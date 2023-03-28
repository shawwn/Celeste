// Decompiled with JetBrains decompiler
// Type: Celeste.StatsForStadia
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using System;
using System.Collections.Generic;

namespace Celeste
{
    public static class StatsForStadia
    {
        private static Dictionary<StadiaStat, string> statToString = new Dictionary<StadiaStat, string>();
        private static bool ready;

        public static void MakeRequest()
        {
        }

        public static void Increment(StadiaStat stat, int increment = 1)
        {
        }

        public static void SetIfLarger(StadiaStat stat, int value)
        {
        }

        public static void BeginFrame(IntPtr handle)
        {
        }
    }
}