// Decompiled with JetBrains decompiler
// Type: Celeste.SeekerBarrier
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(false)]
  public class SeekerBarrier : Solid
  {
    public float Flash;
    public float Solidify;
    public bool Flashing;
    private float solidifyDelay;
    private List<Vector2> particles = new List<Vector2>();
    private List<SeekerBarrier> adjacent = new List<SeekerBarrier>();
    private float[] speeds = new float[3]{ 12f, 20f, 40f };

    public SeekerBarrier(Vector2 position, float width, float height)
      : base(position, width, height, false)
    {
      this.Collidable = false;
      for (int index = 0; (double) index < (double) this.Width * (double) this.Height / 16.0; ++index)
        this.particles.Add(new Vector2(Calc.Random.NextFloat(this.Width - 1f), Calc.Random.NextFloat(this.Height - 1f)));
    }

    public SeekerBarrier(EntityData data, Vector2 offset)
      : this(data.Position + offset, (float) data.Width, (float) data.Height)
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      scene.Tracker.GetEntity<SeekerBarrierRenderer>().Track(this);
    }

    public override void Removed(Scene scene)
    {
      base.Removed(scene);
      scene.Tracker.GetEntity<SeekerBarrierRenderer>().Untrack(this);
    }

    public override void Update()
    {
      if (this.Flashing)
      {
        this.Flash = Calc.Approach(this.Flash, 0.0f, Engine.DeltaTime * 4f);
        if ((double) this.Flash <= 0.0)
          this.Flashing = false;
      }
      else if ((double) this.solidifyDelay > 0.0)
        this.solidifyDelay -= Engine.DeltaTime;
      else if ((double) this.Solidify > 0.0)
        this.Solidify = Calc.Approach(this.Solidify, 0.0f, Engine.DeltaTime);
      int length = this.speeds.Length;
      float height = this.Height;
      int index = 0;
      for (int count = this.particles.Count; index < count; ++index)
      {
        Vector2 vector2 = this.particles[index] + Vector2.UnitY * this.speeds[index % length] * Engine.DeltaTime;
        vector2.Y %= height - 1f;
        this.particles[index] = vector2;
      }
      base.Update();
    }

    public void OnReflectSeeker()
    {
      this.Flash = 1f;
      this.Solidify = 1f;
      this.solidifyDelay = 1f;
      this.Flashing = true;
      this.Scene.CollideInto<SeekerBarrier>(new Rectangle((int) this.X, (int) this.Y - 2, (int) this.Width, (int) this.Height + 4), this.adjacent);
      this.Scene.CollideInto<SeekerBarrier>(new Rectangle((int) this.X - 2, (int) this.Y, (int) this.Width + 4, (int) this.Height), this.adjacent);
      foreach (SeekerBarrier seekerBarrier in this.adjacent)
      {
        if (!seekerBarrier.Flashing)
          seekerBarrier.OnReflectSeeker();
      }
      this.adjacent.Clear();
    }

    public override void Render()
    {
      Color color = Color.White * 0.5f;
      foreach (Vector2 particle in this.particles)
        Draw.Pixel.Draw(this.Position + particle, Vector2.Zero, color);
      if (!this.Flashing)
        return;
      Draw.Rect(this.Collider, Color.White * this.Flash * 0.5f);
    }
  }
}
