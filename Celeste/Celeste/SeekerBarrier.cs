// Decompiled with JetBrains decompiler
// Type: Celeste.SeekerBarrier
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(false)]
  public class SeekerBarrier : Solid
  {
    private float flash = 0.0f;
    private bool flashing = false;
    private List<Vector2> particles = new List<Vector2>();
    private List<SeekerBarrier> adjacent = new List<SeekerBarrier>();
    private float[] speeds = new float[3]{ 12f, 20f, 40f };
    private MTexture temp;
    private float offX;
    private float offY;

    public SeekerBarrier(Vector2 position, float width, float height)
      : base(position, width, height, false)
    {
      this.Collidable = false;
      this.temp = new MTexture();
      for (int index = 0; (double) index < (double) this.Width * (double) this.Height / 16.0; ++index)
        this.particles.Add(new Vector2(Calc.Random.NextFloat(this.Width - 1f), Calc.Random.NextFloat(this.Height - 1f)));
      this.offX = position.X;
      this.offY = position.Y;
      while ((double) this.offX < 0.0)
        this.offX += 128f;
      while ((double) this.offY < 0.0)
        this.offY += 128f;
      this.Add((Component) new DisplacementRenderHook(new Action(this.RenderDisplacement)));
    }

    public SeekerBarrier(EntityData data, Vector2 offset)
      : this(data.Position + offset, (float) data.Width, (float) data.Height)
    {
    }

    public override void Update()
    {
      this.offX += Engine.DeltaTime * 12f;
      this.offY += Engine.DeltaTime * 12f;
      if (this.flashing)
      {
        this.flash = Calc.Approach(this.flash, 0.0f, Engine.DeltaTime * 5f);
        if ((double) this.flash <= 0.0)
          this.flashing = false;
      }
      int length = this.speeds.Length;
      int index = 0;
      for (int count = this.particles.Count; index < count; ++index)
      {
        Vector2 vector2 = this.particles[index] + Vector2.UnitY * this.speeds[index % length] * Engine.DeltaTime;
        vector2.Y %= this.Height - 1f;
        this.particles[index] = vector2;
      }
      base.Update();
    }

    public void OnReflectSeeker()
    {
      this.flash = 1f;
      this.flashing = true;
      this.Scene.CollideInto<SeekerBarrier>(new Rectangle((int) this.X, (int) this.Y - 2, (int) this.Width, (int) this.Height + 4), this.adjacent);
      this.Scene.CollideInto<SeekerBarrier>(new Rectangle((int) this.X - 2, (int) this.Y, (int) this.Width + 4, (int) this.Height), this.adjacent);
      foreach (SeekerBarrier seekerBarrier in this.adjacent)
      {
        if (!seekerBarrier.flashing)
          seekerBarrier.OnReflectSeeker();
      }
      this.adjacent.Clear();
    }

    public void RenderDisplacement()
    {
      MTexture mtexture = GFX.Game["util/displacementBlock"];
      Color color = Color.White * 0.3f;
      for (int index1 = 0; (double) index1 < (double) this.Width; index1 += 128)
      {
        for (int index2 = 0; (double) index2 < (double) this.Height; index2 += 128)
        {
          mtexture.GetSubtexture((int) ((double) this.offX % 128.0), (int) ((double) this.offY % 128.0), (int) Math.Min(128f, this.Width - (float) index1), (int) Math.Min(128f, this.Height - (float) index2), this.temp);
          this.temp.Draw(this.Position + new Vector2((float) index1, (float) index2), Vector2.Zero, color);
        }
      }
    }

    public override void Render()
    {
      Draw.Rect(this.Collider, Color.White * 0.1f);
      if ((double) this.flash > 0.0)
        Draw.Rect(this.Collider, Color.White * this.flash);
      Color color = Color.White * 0.5f;
      foreach (Vector2 particle in this.particles)
        Draw.Pixel.Draw(this.Position + particle, Vector2.Zero, color);
    }
  }
}

