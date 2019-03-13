// Decompiled with JetBrains decompiler
// Type: Monocle.Wiggler
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Collections.Generic;

namespace Monocle
{
  public class Wiggler : Component
  {
    private static Stack<Wiggler> cache = new Stack<Wiggler>();
    public bool StartZero;
    public bool UseRawDeltaTime;
    private float sineCounter;
    private float increment;
    private float sineAdd;
    private Action<float> onChange;
    private bool removeSelfOnFinish;

    public float Counter { get; private set; }

    public float Value { get; private set; }

    public static Wiggler Create(
      float duration,
      float frequency,
      Action<float> onChange = null,
      bool start = false,
      bool removeSelfOnFinish = false)
    {
      Wiggler wiggler = Wiggler.cache.Count <= 0 ? new Wiggler() : Wiggler.cache.Pop();
      wiggler.Init(duration, frequency, onChange, start, removeSelfOnFinish);
      return wiggler;
    }

    private Wiggler()
      : base(false, false)
    {
    }

    private void Init(
      float duration,
      float frequency,
      Action<float> onChange,
      bool start,
      bool removeSelfOnFinish)
    {
      this.Counter = this.sineCounter = 0.0f;
      this.UseRawDeltaTime = false;
      this.increment = 1f / duration;
      this.sineAdd = 6.283185f * frequency;
      this.onChange = onChange;
      this.removeSelfOnFinish = removeSelfOnFinish;
      if (start)
        this.Start();
      else
        this.Active = false;
    }

    public override void Removed(Entity entity)
    {
      base.Removed(entity);
      Wiggler.cache.Push(this);
    }

    public void Start()
    {
      this.Counter = 1f;
      if (this.StartZero)
      {
        this.sineCounter = 1.570796f;
        this.Value = 0.0f;
        if (this.onChange != null)
          this.onChange(0.0f);
      }
      else
      {
        this.sineCounter = 0.0f;
        this.Value = 1f;
        if (this.onChange != null)
          this.onChange(1f);
      }
      this.Active = true;
    }

    public void Start(float duration, float frequency)
    {
      this.increment = 1f / duration;
      this.sineAdd = 6.283185f * frequency;
      this.Start();
    }

    public void Stop()
    {
      this.Active = false;
    }

    public void StopAndClear()
    {
      this.Stop();
      this.Value = 0.0f;
    }

    public override void Update()
    {
      if (this.UseRawDeltaTime)
      {
        this.sineCounter += this.sineAdd * Engine.RawDeltaTime;
        this.Counter -= this.increment * Engine.RawDeltaTime;
      }
      else
      {
        this.sineCounter += this.sineAdd * Engine.DeltaTime;
        this.Counter -= this.increment * Engine.DeltaTime;
      }
      if ((double) this.Counter <= 0.0)
      {
        this.Counter = 0.0f;
        this.Active = false;
        if (this.removeSelfOnFinish)
          this.RemoveSelf();
      }
      this.Value = (float) Math.Cos((double) this.sineCounter) * this.Counter;
      if (this.onChange == null)
        return;
      this.onChange(this.Value);
    }
  }
}
