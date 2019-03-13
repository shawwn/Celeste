// Decompiled with JetBrains decompiler
// Type: Celeste.WindController
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class WindController : Entity
  {
    private const float Weak = 400f;
    private const float Strong = 800f;
    private const float Crazy = 1200f;
    private const float Accel = 1000f;
    private const float Down = 300f;
    private const float Up = -400f;
    private const float Space = -600f;
    private Level level;
    private WindController.Patterns pattern;
    private Vector2 targetSpeed;
    private Coroutine coroutine;
    private WindController.Patterns startPattern;
    private bool everSetPattern;

    public WindController(WindController.Patterns pattern)
    {
      this.Tag = (int) Tags.TransitionUpdate;
      this.startPattern = pattern;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.level = this.SceneAs<Level>();
    }

    public void SetStartPattern()
    {
      if (this.everSetPattern)
        return;
      this.SetPattern(this.startPattern);
    }

    public void SetPattern(WindController.Patterns pattern)
    {
      if (this.pattern == pattern && this.everSetPattern)
        return;
      this.everSetPattern = true;
      this.pattern = pattern;
      if (this.coroutine != null)
      {
        this.Remove((Component) this.coroutine);
        this.coroutine = (Coroutine) null;
      }
      switch (pattern)
      {
        case WindController.Patterns.None:
          this.targetSpeed = Vector2.Zero;
          this.SetAmbienceStrength(false);
          break;
        case WindController.Patterns.Left:
          this.targetSpeed.X = -400f;
          this.SetAmbienceStrength(false);
          break;
        case WindController.Patterns.Right:
          this.targetSpeed.X = 400f;
          this.SetAmbienceStrength(false);
          break;
        case WindController.Patterns.LeftStrong:
          this.targetSpeed.X = -800f;
          this.SetAmbienceStrength(true);
          break;
        case WindController.Patterns.RightStrong:
          this.targetSpeed.X = 800f;
          this.SetAmbienceStrength(true);
          break;
        case WindController.Patterns.LeftOnOff:
          this.Add((Component) (this.coroutine = new Coroutine(this.LeftOnOffSequence(), true)));
          break;
        case WindController.Patterns.RightOnOff:
          this.Add((Component) (this.coroutine = new Coroutine(this.RightOnOffSequence(), true)));
          break;
        case WindController.Patterns.LeftOnOffFast:
          this.Add((Component) (this.coroutine = new Coroutine(this.LeftOnOffFastSequence(), true)));
          break;
        case WindController.Patterns.RightOnOffFast:
          this.Add((Component) (this.coroutine = new Coroutine(this.RightOnOffFastSequence(), true)));
          break;
        case WindController.Patterns.Alternating:
          this.Add((Component) (this.coroutine = new Coroutine(this.AlternatingSequence(), true)));
          break;
        case WindController.Patterns.RightCrazy:
          this.targetSpeed.X = 1200f;
          this.SetAmbienceStrength(true);
          break;
        case WindController.Patterns.Down:
          this.targetSpeed.Y = 300f;
          break;
        case WindController.Patterns.Up:
          this.targetSpeed.Y = -400f;
          break;
        case WindController.Patterns.Space:
          this.targetSpeed.Y = -600f;
          break;
      }
    }

    private void SetAmbienceStrength(bool strong)
    {
      Audio.SetParameter(Audio.CurrentAmbienceEventInstance, "wind_direction", (float) Math.Sign(this.targetSpeed.X));
      Audio.SetParameter(Audio.CurrentAmbienceEventInstance, "strong_wind", strong ? 1f : 0.0f);
    }

    public void SnapWind()
    {
      if (this.coroutine != null && this.coroutine.Active)
        this.coroutine.Update();
      this.level.Wind = this.targetSpeed;
    }

    public override void Update()
    {
      base.Update();
      if (this.pattern == WindController.Patterns.LeftGemsOnly)
      {
        bool flag = false;
        foreach (StrawberrySeed entity in this.Scene.Tracker.GetEntities<StrawberrySeed>())
        {
          if (entity.Collected)
          {
            flag = true;
            break;
          }
        }
        if (flag)
        {
          this.targetSpeed.X = -400f;
          this.SetAmbienceStrength(false);
        }
        else
        {
          this.targetSpeed.X = 0.0f;
          this.SetAmbienceStrength(false);
        }
      }
      this.level.Wind = Calc.Approach(this.level.Wind, this.targetSpeed, 1000f * Engine.DeltaTime);
      if (!(this.level.Wind != Vector2.Zero) || this.level.Transitioning)
        return;
      foreach (WindMover component in this.Scene.Tracker.GetComponents<WindMover>())
        component.Move(this.level.Wind * 0.1f * Engine.DeltaTime);
    }

    private IEnumerator AlternatingSequence()
    {
      while (true)
      {
        this.targetSpeed.X = -400f;
        this.SetAmbienceStrength(false);
        yield return (object) 3f;
        this.targetSpeed.X = 0.0f;
        this.SetAmbienceStrength(false);
        yield return (object) 2f;
        this.targetSpeed.X = 400f;
        this.SetAmbienceStrength(false);
        yield return (object) 3f;
        this.targetSpeed.X = 0.0f;
        this.SetAmbienceStrength(false);
        yield return (object) 2f;
      }
    }

    private IEnumerator RightOnOffSequence()
    {
      while (true)
      {
        this.targetSpeed.X = 800f;
        this.SetAmbienceStrength(true);
        yield return (object) 3f;
        this.targetSpeed.X = 0.0f;
        this.SetAmbienceStrength(false);
        yield return (object) 3f;
      }
    }

    private IEnumerator LeftOnOffSequence()
    {
      while (true)
      {
        this.targetSpeed.X = -800f;
        this.SetAmbienceStrength(true);
        yield return (object) 3f;
        this.targetSpeed.X = 0.0f;
        this.SetAmbienceStrength(false);
        yield return (object) 3f;
      }
    }

    private IEnumerator RightOnOffFastSequence()
    {
      while (true)
      {
        this.targetSpeed.X = 800f;
        this.SetAmbienceStrength(true);
        yield return (object) 2f;
        this.targetSpeed.X = 0.0f;
        this.SetAmbienceStrength(false);
        yield return (object) 2f;
      }
    }

    private IEnumerator LeftOnOffFastSequence()
    {
      while (true)
      {
        this.targetSpeed.X = -800f;
        this.SetAmbienceStrength(true);
        yield return (object) 2f;
        this.targetSpeed.X = 0.0f;
        this.SetAmbienceStrength(false);
        yield return (object) 2f;
      }
    }

    public enum Patterns
    {
      None,
      Left,
      Right,
      LeftStrong,
      RightStrong,
      LeftOnOff,
      RightOnOff,
      LeftOnOffFast,
      RightOnOffFast,
      Alternating,
      LeftGemsOnly,
      RightCrazy,
      Down,
      Up,
      Space,
    }
  }
}

