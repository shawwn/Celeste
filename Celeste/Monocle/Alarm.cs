// Decompiled with JetBrains decompiler
// Type: Monocle.Alarm
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Collections.Generic;

namespace Monocle
{
  public class Alarm : Component
  {
    private static Stack<Alarm> cached = new Stack<Alarm>();
    public Action OnComplete;

    public Alarm.AlarmMode Mode { get; private set; }

    public float Duration { get; private set; }

    public float TimeLeft { get; private set; }

    public static Alarm Create(
      Alarm.AlarmMode mode,
      Action onComplete,
      float duration = 1f,
      bool start = false)
    {
      Alarm alarm = Alarm.cached.Count != 0 ? Alarm.cached.Pop() : new Alarm();
      alarm.Init(mode, onComplete, duration, start);
      return alarm;
    }

    public static Alarm Set(
      Entity entity,
      float duration,
      Action onComplete,
      Alarm.AlarmMode alarmMode = Alarm.AlarmMode.Oneshot)
    {
      Alarm alarm = Alarm.Create(alarmMode, onComplete, duration, true);
      entity.Add((Component) alarm);
      return alarm;
    }

    private Alarm()
      : base(false, false)
    {
    }

    private void Init(Alarm.AlarmMode mode, Action onComplete, float duration = 1f, bool start = false)
    {
      this.Mode = mode;
      this.Duration = duration;
      this.OnComplete = onComplete;
      this.Active = false;
      this.TimeLeft = 0.0f;
      if (!start)
        return;
      this.Start();
    }

    public override void Update()
    {
      this.TimeLeft -= Engine.DeltaTime;
      if ((double) this.TimeLeft > 0.0)
        return;
      this.TimeLeft = 0.0f;
      if (this.OnComplete != null)
        this.OnComplete();
      if (this.Mode == Alarm.AlarmMode.Looping)
        this.Start();
      else if (this.Mode == Alarm.AlarmMode.Oneshot)
      {
        this.RemoveSelf();
      }
      else
      {
        if ((double) this.TimeLeft > 0.0)
          return;
        this.Active = false;
      }
    }

    public override void Removed(Entity entity)
    {
      base.Removed(entity);
      Alarm.cached.Push(this);
    }

    public void Start()
    {
      this.Active = true;
      this.TimeLeft = this.Duration;
    }

    public void Start(float duration)
    {
      this.Duration = duration;
      this.Start();
    }

    public void Stop()
    {
      this.Active = false;
    }

    public enum AlarmMode
    {
      Persist,
      Oneshot,
      Looping,
    }
  }
}
