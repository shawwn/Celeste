// Decompiled with JetBrains decompiler
// Type: Celeste.ClutterAbsorbEffect
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class ClutterAbsorbEffect : Entity
  {
    private List<ClutterCabinet> cabinets = new List<ClutterCabinet>();
    private Level level;

    public ClutterAbsorbEffect()
    {
      this.Position = Vector2.Zero;
      this.Tag = (int) Tags.TransitionUpdate;
      this.Depth = -10001;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.level = this.SceneAs<Level>();
      foreach (Entity entity in this.level.Tracker.GetEntities<ClutterCabinet>())
        this.cabinets.Add(entity as ClutterCabinet);
    }

    public void FlyClutter(Vector2 position, MTexture texture, bool shake, float delay)
    {
      Monocle.Image img = new Monocle.Image(texture);
      img.Position = position - this.Position;
      img.CenterOrigin();
      this.Add((Component) img);
      this.Add((Component) new Coroutine(this.FlyClutterRoutine(img, shake, delay), true)
      {
        RemoveOnComplete = true
      });
    }

    private IEnumerator FlyClutterRoutine(Monocle.Image img, bool shake, float delay)
    {
      yield return (object) delay;
      ClutterCabinet cabinet = Calc.Random.Choose<ClutterCabinet>(this.cabinets);
      Vector2 target = cabinet.Position + new Vector2(8f);
      Vector2 from = img.Position;
      Vector2 to = target + new Vector2((float) (Calc.Random.Next(16) - 8), (float) (Calc.Random.Next(4) - 2));
      Vector2 normal = (to - from).SafeNormalize();
      float dist = (to - from).Length();
      Vector2 perp = new Vector2(-normal.Y, normal.X) * (dist / 4f + Calc.Random.NextFloat(40f)) * (Calc.Random.Chance(0.5f) ? -1f : 1f);
      SimpleCurve curve = new SimpleCurve(from, to, (to + from) / 2f + perp);
      if (shake)
      {
        for (float time = 0.25f; (double) time > 0.0; time -= Engine.DeltaTime)
        {
          img.X = (float) ((double) from.X + (double) Calc.Random.Next(3) - 1.0);
          img.Y = (float) ((double) from.Y + (double) Calc.Random.Next(3) - 1.0);
          yield return (object) null;
        }
      }
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        img.Position = curve.GetPoint(Ease.CubeInOut(p));
        img.Scale = Vector2.One * Ease.CubeInOut((float) (1.0 - (double) p * 0.5));
        if ((double) p > 0.5 && !cabinet.Opened)
          cabinet.Open();
        if (this.level.OnInterval(0.25f))
          this.level.ParticlesFG.Emit(ClutterSwitch.P_ClutterFly, img.Position);
        yield return (object) null;
      }
      this.Remove((Component) img);
    }

    public void CloseCabinets()
    {
      this.Add((Component) new Coroutine(this.CloseCabinetsRoutine(), true));
    }

    private IEnumerator CloseCabinetsRoutine()
    {
      this.cabinets.Sort((Comparison<ClutterCabinet>) ((a, b) =>
      {
        if ((double) Math.Abs(a.Y - b.Y) < 24.0)
          return Math.Sign(a.X - b.X);
        return Math.Sign(a.Y - b.Y);
      }));
      int i = 0;
      foreach (ClutterCabinet cabinet1 in this.cabinets)
      {
        ClutterCabinet cabinet = cabinet1;
        cabinet.Close();
        if (i++ % 3 == 0)
          yield return (object) 0.1f;
        cabinet = (ClutterCabinet) null;
      }
    }
  }
}

