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
      this.Position = Vector2.get_Zero();
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
      img.Position = Vector2.op_Subtraction(position, this.Position);
      img.CenterOrigin();
      this.Add((Component) img);
      this.Add((Component) new Coroutine(this.FlyClutterRoutine(img, shake, delay), true)
      {
        RemoveOnComplete = true
      });
    }

    private IEnumerator FlyClutterRoutine(Monocle.Image img, bool shake, float delay)
    {
      ClutterAbsorbEffect clutterAbsorbEffect = this;
      yield return (object) delay;
      ClutterCabinet cabinet = Calc.Random.Choose<ClutterCabinet>(clutterAbsorbEffect.cabinets);
      Vector2 vector2_1 = Vector2.op_Addition(cabinet.Position, new Vector2(8f));
      Vector2 from = img.Position;
      Vector2 vector2_2 = new Vector2((float) (Calc.Random.Next(16) - 8), (float) (Calc.Random.Next(4) - 2));
      Vector2 end = Vector2.op_Addition(vector2_1, vector2_2);
      Vector2 vector2_3 = Vector2.op_Subtraction(end, from).SafeNormalize();
      Vector2 vector2_4 = Vector2.op_Subtraction(end, from);
      float num = ((Vector2) ref vector2_4).Length();
      Vector2 vector2_5 = Vector2.op_Multiply(Vector2.op_Multiply(new Vector2((float) -vector2_3.Y, (float) vector2_3.X), num / 4f + Calc.Random.NextFloat(40f)), Calc.Random.Chance(0.5f) ? -1f : 1f);
      SimpleCurve curve = new SimpleCurve(from, end, Vector2.op_Addition(Vector2.op_Division(Vector2.op_Addition(end, from), 2f), vector2_5));
      float time;
      if (shake)
      {
        for (time = 0.25f; (double) time > 0.0; time -= Engine.DeltaTime)
        {
          img.X = (float) (from.X + (double) Calc.Random.Next(3) - 1.0);
          img.Y = (float) (from.Y + (double) Calc.Random.Next(3) - 1.0);
          yield return (object) null;
        }
      }
      for (time = 0.0f; (double) time < 1.0; time += Engine.DeltaTime)
      {
        img.Position = curve.GetPoint(Ease.CubeInOut(time));
        img.Scale = Vector2.op_Multiply(Vector2.get_One(), Ease.CubeInOut((float) (1.0 - (double) time * 0.5)));
        if ((double) time > 0.5 && !cabinet.Opened)
          cabinet.Open();
        if (clutterAbsorbEffect.level.OnInterval(0.25f))
          clutterAbsorbEffect.level.ParticlesFG.Emit(ClutterSwitch.P_ClutterFly, img.Position);
        yield return (object) null;
      }
      clutterAbsorbEffect.Remove((Component) img);
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
      foreach (ClutterCabinet cabinet in this.cabinets)
      {
        cabinet.Close();
        if (i++ % 3 == 0)
          yield return (object) 0.1f;
      }
    }
  }
}
