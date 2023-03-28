// Decompiled with JetBrains decompiler
// Type: Celeste.Cobweb
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

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
      this.anchorA = this.Position = data.Position + offset;
      this.anchorB = data.Nodes[0] + offset;
      foreach (Vector2 node in data.Nodes)
      {
        if (this.offshoots == null)
        {
          this.offshoots = new List<Vector2>();
          this.offshootEndings = new List<float>();
        }
        else
        {
          this.offshoots.Add(node + offset);
          this.offshootEndings.Add(0.3f + Calc.Random.NextFloat(0.4f));
        }
      }
      this.waveTimer = Calc.Random.NextFloat();
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.color = Calc.Random.Choose<Color>(AreaData.Get(scene).CobwebColor);
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

    public override void Render() => this.DrawCobweb(this.anchorA, this.anchorB, 12, true);

    private void DrawCobweb(Vector2 a, Vector2 b, int steps, bool drawOffshoots)
    {
      SimpleCurve simpleCurve = new SimpleCurve(a, b, (a + b) / 2f + Vector2.UnitY * (float) (8.0 + Math.Sin((double) this.waveTimer) * 4.0));
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
        start = point + (start - point).SafeNormalize();
      }
    }
  }
}
