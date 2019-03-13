// Decompiled with JetBrains decompiler
// Type: Celeste.Switch
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class Switch : Component
  {
    public bool GroundReset;
    public Action OnActivate;
    public Action OnDeactivate;
    public Action OnFinish;
    public Action OnStartFinished;

    public Switch(bool groundReset)
      : base(true, false)
    {
      this.GroundReset = groundReset;
    }

    public bool Activated { get; private set; }

    public bool Finished { get; private set; }

    public override void EntityAdded(Scene scene)
    {
      base.EntityAdded(scene);
      if (!Switch.CheckLevelFlag(this.SceneAs<Level>()))
        return;
      this.StartFinished();
    }

    public override void Update()
    {
      base.Update();
      if (!this.GroundReset || !this.Activated || this.Finished)
        return;
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity == null || !entity.OnGround(1))
        return;
      this.Deactivate();
    }

    public bool Activate()
    {
      if (this.Finished || this.Activated)
        return false;
      this.Activated = true;
      if (this.OnActivate != null)
        this.OnActivate();
      return Switch.FinishedCheck(this.SceneAs<Level>());
    }

    public void Deactivate()
    {
      if (this.Finished || !this.Activated)
        return;
      this.Activated = false;
      if (this.OnDeactivate == null)
        return;
      this.OnDeactivate();
    }

    public void Finish()
    {
      this.Finished = true;
      if (this.OnFinish == null)
        return;
      this.OnFinish();
    }

    public void StartFinished()
    {
      if (this.Finished)
        return;
      this.Finished = this.Activated = true;
      if (this.OnStartFinished == null)
        return;
      this.OnStartFinished();
    }

    public static bool Check(Scene scene)
    {
      Switch component = scene.Tracker.GetComponent<Switch>();
      if (component == null)
        return false;
      return component.Finished;
    }

    private static bool FinishedCheck(Level level)
    {
      foreach (Switch component in level.Tracker.GetComponents<Switch>())
      {
        if (!component.Activated)
          return false;
      }
      foreach (Switch component in level.Tracker.GetComponents<Switch>())
        component.Finish();
      return true;
    }

    public static bool CheckLevelFlag(Level level)
    {
      return level.Session.GetFlag("switches_" + level.Session.Level);
    }

    public static void SetLevelFlag(Level level)
    {
      level.Session.SetFlag("switches_" + level.Session.Level, true);
    }
  }
}
