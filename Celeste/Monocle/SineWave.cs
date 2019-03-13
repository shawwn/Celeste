// Decompiled with JetBrains decompiler
// Type: Monocle.SineWave
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace Monocle
{
  public class SineWave : Component
  {
    public float Frequency = 1f;
    public float Rate = 1f;
    public Action<float> OnUpdate;
    public bool UseRawDeltaTime;
    private float counter;

    public float Value { get; private set; }

    public float ValueOverTwo { get; private set; }

    public float TwoValue { get; private set; }

    public SineWave()
      : base(true, false)
    {
    }

    public SineWave(float frequency)
      : this()
    {
      this.Frequency = frequency;
    }

    public override void Update()
    {
      this.Counter += (float) (6.28318548202515 * (double) this.Frequency * (double) this.Rate * (this.UseRawDeltaTime ? (double) Engine.RawDeltaTime : (double) Engine.DeltaTime));
      if (this.OnUpdate == null)
        return;
      this.OnUpdate(this.Value);
    }

    public float ValueOffset(float offset)
    {
      return (float) Math.Sin((double) this.counter + (double) offset);
    }

    public SineWave Randomize()
    {
      this.Counter = (float) ((double) Calc.Random.NextFloat() * 6.28318548202515 * 2.0);
      return this;
    }

    public void Reset()
    {
      this.Counter = 0.0f;
    }

    public void StartUp()
    {
      this.Counter = 1.570796f;
    }

    public void StartDown()
    {
      this.Counter = 4.712389f;
    }

    public float Counter
    {
      get
      {
        return this.counter;
      }
      set
      {
        this.counter = (float) (((double) value + 25.1327419281006) % 25.1327419281006);
        this.Value = (float) Math.Sin((double) this.counter);
        this.ValueOverTwo = (float) Math.Sin((double) this.counter / 2.0);
        this.TwoValue = (float) Math.Sin((double) this.counter * 2.0);
      }
    }
  }
}
