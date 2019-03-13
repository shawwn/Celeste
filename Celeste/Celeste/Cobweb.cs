// Decompiled with JetBrains decompiler
// Type: Celeste.Cobweb
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class Cobweb : Entity
  {
    private Color color;
    private Color edge;
    private Vector2 anchorA;
    private Vector2 anchorB;
    private List<Vector2> offshoots;
    private List<float> offshootEndings;
    private float waveTimer;

    public Cobweb(EntityData data, Vector2 offset)
    {
      this.Depth = -1;
      this.anchorA = this.Position = Vector2.op_Addition(data.Position, offset);
      this.anchorB = Vector2.op_Addition(data.Nodes[0], offset);
      foreach (Vector2 node in data.Nodes)
      {
        if (this.offshoots == null)
        {
          this.offshoots = new List<Vector2>();
          this.offshootEndings = new List<float>();
        }
        else
        {
          this.offshoots.Add(Vector2.op_Addition(node, offset));
          this.offshootEndings.Add(0.3f + Calc.Random.NextFloat(0.4f));
        }
      }
      this.waveTimer = Calc.Random.NextFloat();
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.color = AreaData.Get(scene).CobwebColor;
      this.edge = Color.Lerp(this.color, Calc.HexToColor("0f0e17"), 0.2f);
      if (!this.Scene.CollideCheck<Solid>(new Rectangle((int) this.anchorA.X - 2, (int) this.anchorA.Y - 2, 4, 4)) || !this.Scene.CollideCheck<Solid>(new Rectangle((int) this.anchorB.X - 2, (int) this.anchorB.Y - 2, 4, 4)))
        this.RemoveSelf();
      for (int index = 0; index < this.offshoots.Count; ++index)
      {
        Vector2 offshoot = this.offshoots[index];
        if (!this.Scene.CollideCheck<Solid>(new Rectangle((int) offshoot.X - 2, (int) offshoot.Y - 2, 4, 4)))
        {
          this.offshoots.RemoveAt(index);
          this.offshootEndings.RemoveAt(index);
          --index;
        }
      }
    }

    public override void Update()
    {
      this.waveTimer += Engine.DeltaTime;
      base.Update();
    }

    public override void Render()
    {
      this.DrawCobweb(this.anchorA, this.anchorB, 12, true);
    }

    private void DrawCobweb(Vector2 a, Vector2 b, int steps, bool drawOffshoots)
    {
      SimpleCurve simpleCurve = new SimpleCurve(a, b, Vector2.op_Addition(Vector2.op_Division(Vector2.op_Addition(a, b), 2f), Vector2.op_Multiply(Vector2.get_UnitY(), (float) (8.0 + Math.Sin((double) this.waveTimer) * 4.0))));
      if (drawOffshoots && this.offshoots != null)
      {
        for (int index = 0; index < this.offshoots.Count; ++index)
          this.DrawCobweb(this.offshoots[index], simpleCurve.GetPoint(this.offshootEndings[index]), 4, false);
      }
      Vector2 start = simpleCurve.Begin;
      for (int index = 1; index <= steps; ++index)
      {
        float percent = (float) index / (float) steps;
        Vector2 point = simpleCurve.GetPoint(percent);
        Draw.Line(start, point, index <= 2 || index >= steps - 1 ? this.edge : this.color);
        start = Vector2.op_Addition(point, Vector2.op_Subtraction(start, point).SafeNormalize());
      }
    }
  }
}
