// Decompiled with JetBrains decompiler
// Type: Monocle.SineWave
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

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

    public SineWave(float frequency, float offset = 0.0f)
      : this()
    {
      this.Frequency = frequency;
      this.Counter = offset;
    }

    public override void Update()
    {
      this.Counter += (float) (6.2831854820251465 * (double) this.Frequency * (double) this.Rate * (this.UseRawDeltaTime ? (double) Engine.RawDeltaTime : (double) Engine.DeltaTime));
      if (this.OnUpdate == null)
        return;
      this.OnUpdate(this.Value);
    }

    public float ValueOffset(float offset) => (float) Math.Sin((double) this.counter + (double) offset);

    public SineWave Randomize()
    {
      this.Counter = (float) ((double) Calc.Random.NextFloat() * 6.2831854820251465 * 2.0);
      return this;
    }

    public void Reset() => this.Counter = 0.0f;

    public void StartUp() => this.Counter = 1.5707964f;

    public void StartDown() => this.Counter = 4.712389f;

    public float Counter
    {
      get => this.counter;
      set
      {
        this.counter = (float) (((double) value + 25.132741928100586) % 25.132741928100586);
        this.Value = (float) Math.Sin((double) this.counter);
        this.ValueOverTwo = (float) Math.Sin((double) this.counter / 2.0);
        this.TwoValue = (float) Math.Sin((double) this.counter * 2.0);
      }
    }
  }
}
