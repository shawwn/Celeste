// Decompiled with JetBrains decompiler
// Type: Monocle.CounterSet`1
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Collections.Generic;

namespace Monocle
{
  public class CounterSet<T> : Component
  {
    private Dictionary<T, float> counters;
    private float timer;

    public CounterSet()
      : base(true, false)
    {
      this.counters = new Dictionary<T, float>();
    }

    public float this[T index]
    {
      get
      {
        float num;
        if (this.counters.TryGetValue(index, out num))
          return Math.Max(num - this.timer, 0.0f);
        return 0.0f;
      }
      set
      {
        this.counters[index] = this.timer + value;
      }
    }

    public bool Check(T index)
    {
      float num;
      if (this.counters.TryGetValue(index, out num))
        return (double) num - (double) this.timer > 0.0;
      return false;
    }

    public override void Update()
    {
      this.timer += Engine.DeltaTime;
    }
  }
}
