// Decompiled with JetBrains decompiler
// Type: Monocle.StateMachine
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Collections;

namespace Monocle
{
  public class StateMachine : Component
  {
    private int state;
    private Action[] begins;
    private Func<int>[] updates;
    private Action[] ends;
    private Func<IEnumerator>[] coroutines;
    private Coroutine currentCoroutine;
    public bool ChangedStates;
    public bool Log;
    public bool Locked;

    public int PreviousState { get; private set; }

    public StateMachine(int maxStates = 10)
      : base(true, false)
    {
      this.PreviousState = this.state = -1;
      this.begins = new Action[maxStates];
      this.updates = new Func<int>[maxStates];
      this.ends = new Action[maxStates];
      this.coroutines = new Func<IEnumerator>[maxStates];
      this.currentCoroutine = new Coroutine(true);
      this.currentCoroutine.RemoveOnComplete = false;
    }

    public override void Added(Entity entity)
    {
      base.Added(entity);
      if (this.Entity.Scene == null || this.state != -1)
        return;
      this.State = 0;
    }

    public override void EntityAdded(Scene scene)
    {
      base.EntityAdded(scene);
      if (this.state != -1)
        return;
      this.State = 0;
    }

    public int State
    {
      get
      {
        return this.state;
      }
      set
      {
        if (this.Locked || this.state == value)
          return;
        if (this.Log)
          Calc.Log((object) ("Enter State " + (object) value + " (leaving " + (object) this.state + ")"));
        this.ChangedStates = true;
        this.PreviousState = this.state;
        this.state = value;
        if (this.PreviousState != -1 && this.ends[this.PreviousState] != null)
        {
          if (this.Log)
            Calc.Log((object) ("Calling End " + (object) this.PreviousState));
          this.ends[this.PreviousState]();
        }
        if (this.begins[this.state] != null)
        {
          if (this.Log)
            Calc.Log((object) ("Calling Begin " + (object) this.state));
          this.begins[this.state]();
        }
        if (this.coroutines[this.state] != null)
        {
          if (this.Log)
            Calc.Log((object) ("Starting Coroutine " + (object) this.state));
          this.currentCoroutine.Replace(this.coroutines[this.state]());
        }
        else
          this.currentCoroutine.Cancel();
      }
    }

    public void ForceState(int toState)
    {
      if (this.state != toState)
      {
        this.State = toState;
      }
      else
      {
        if (this.Log)
          Calc.Log((object) ("Enter State " + (object) toState + " (leaving " + (object) this.state + ")"));
        this.ChangedStates = true;
        this.PreviousState = this.state;
        this.state = toState;
        if (this.PreviousState != -1 && this.ends[this.PreviousState] != null)
        {
          if (this.Log)
            Calc.Log((object) ("Calling End " + (object) this.state));
          this.ends[this.PreviousState]();
        }
        if (this.begins[this.state] != null)
        {
          if (this.Log)
            Calc.Log((object) ("Calling Begin " + (object) this.state));
          this.begins[this.state]();
        }
        if (this.coroutines[this.state] != null)
        {
          if (this.Log)
            Calc.Log((object) ("Starting Coroutine " + (object) this.state));
          this.currentCoroutine.Replace(this.coroutines[this.state]());
        }
        else
          this.currentCoroutine.Cancel();
      }
    }

    public void SetCallbacks(
      int state,
      Func<int> onUpdate,
      Func<IEnumerator> coroutine = null,
      Action begin = null,
      Action end = null)
    {
      this.updates[state] = onUpdate;
      this.begins[state] = begin;
      this.ends[state] = end;
      this.coroutines[state] = coroutine;
    }

    public void ReflectState(Entity from, int index, string name)
    {
      this.updates[index] = (Func<int>) Calc.GetMethod<Func<int>>((object) from, name + "Update");
      this.begins[index] = (Action) Calc.GetMethod<Action>((object) from, name + "Begin");
      this.ends[index] = (Action) Calc.GetMethod<Action>((object) from, name + "End");
      this.coroutines[index] = (Func<IEnumerator>) Calc.GetMethod<Func<IEnumerator>>((object) from, name + "Coroutine");
    }

    public override void Update()
    {
      this.ChangedStates = false;
      if (this.updates[this.state] != null)
        this.State = this.updates[this.state]();
      if (!this.currentCoroutine.Active)
        return;
      this.currentCoroutine.Update();
      if (this.ChangedStates || !this.Log || !this.currentCoroutine.Finished)
        return;
      Calc.Log((object) ("Finished Coroutine " + (object) this.state));
    }

    public static implicit operator int(StateMachine s)
    {
      return s.state;
    }

    public void LogAllStates()
    {
      for (int index = 0; index < this.updates.Length; ++index)
        this.LogState(index);
    }

    public void LogState(int index)
    {
      Calc.Log((object) ("State " + (object) index + ": " + (this.updates[index] != null ? (object) "U" : (object) "") + (this.begins[index] != null ? (object) "B" : (object) "") + (this.ends[index] != null ? (object) "E" : (object) "") + (this.coroutines[index] != null ? (object) "C" : (object) "")));
    }
  }
}
