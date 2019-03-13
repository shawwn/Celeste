// Decompiled with JetBrains decompiler
// Type: Monocle.Shaker
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using System;

namespace Monocle
{
  public class Shaker : Component
  {
    public float Interval = 0.05f;
    public Vector2 Value;
    public float Timer;
    public bool RemoveOnFinish;
    public Action<Vector2> OnShake;
    private bool on;

    public Shaker(bool on = true, Action<Vector2> onShake = null)
      : base(true, false)
    {
      this.on = on;
      this.OnShake = onShake;
    }

    public Shaker(float time, bool removeOnFinish, Action<Vector2> onShake = null)
      : this(true, onShake)
    {
      this.Timer = time;
      this.RemoveOnFinish = removeOnFinish;
    }

    public bool On
    {
      get
      {
        return this.on;
      }
      set
      {
        this.on = value;
        if (this.on)
          return;
        this.Timer = 0.0f;
        if (this.Value != Vector2.Zero)
        {
          this.Value = Vector2.Zero;
          if (this.OnShake != null)
            this.OnShake(Vector2.Zero);
        }
      }
    }

    public Shaker ShakeFor(float seconds, bool removeOnFinish)
    {
      this.on = true;
      this.Timer = seconds;
      this.RemoveOnFinish = removeOnFinish;
      return this;
    }

    public override void Update()
    {
      if (this.on && (double) this.Timer > 0.0)
      {
        this.Timer -= Engine.DeltaTime;
        if ((double) this.Timer <= 0.0)
        {
          this.on = false;
          this.Value = Vector2.Zero;
          if (this.OnShake != null)
            this.OnShake(Vector2.Zero);
          if (!this.RemoveOnFinish)
            return;
          this.RemoveSelf();
          return;
        }
      }
      if (!this.on || !this.Scene.OnInterval(this.Interval))
        return;
      this.Value = Calc.Random.ShakeVector();
      if (this.OnShake != null)
        this.OnShake(this.Value);
    }
  }
}

