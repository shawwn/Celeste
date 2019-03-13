// Decompiled with JetBrains decompiler
// Type: Celeste.CutsceneEntity
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public abstract class CutsceneEntity : Entity
  {
    public bool RemoveOnSkipped = true;
    public bool WasSkipped;
    public bool EndingChapterAfter;
    public Level Level;

    public bool Running { get; private set; }

    public bool FadeInOnSkip { get; private set; }

    public CutsceneEntity(bool fadeInOnSkip = true, bool endingChapterAfter = false)
    {
      this.FadeInOnSkip = fadeInOnSkip;
      this.EndingChapterAfter = endingChapterAfter;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.Level = scene as Level;
      this.Start();
    }

    public void Start()
    {
      this.Running = true;
      this.Level.StartCutscene(new Action<Level>(this.SkipCutscene), this.FadeInOnSkip, this.EndingChapterAfter);
      this.OnBegin(this.Level);
    }

    public override void Update()
    {
      if (this.Level.RetryPlayerCorpse != null)
        this.Active = false;
      else
        base.Update();
    }

    private void SkipCutscene(Level level)
    {
      this.WasSkipped = true;
      this.EndCutscene(level, this.RemoveOnSkipped);
    }

    public void EndCutscene(Level level, bool removeSelf = true)
    {
      this.Running = false;
      this.OnEnd(level);
      level.EndCutscene();
      if (!removeSelf)
        return;
      this.RemoveSelf();
    }

    public abstract void OnBegin(Level level);

    public abstract void OnEnd(Level level);

    public static IEnumerator CameraTo(
      Vector2 target,
      float duration,
      Ease.Easer ease = null,
      float delay = 0.0f)
    {
      if (ease == null)
        ease = Ease.CubeInOut;
      if ((double) delay > 0.0)
        yield return (object) delay;
      Level level = Engine.Scene as Level;
      Vector2 from = level.Camera.Position;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / duration)
      {
        level.Camera.Position = from + (target - from) * ease(p);
        yield return (object) null;
      }
    }
  }
}

