// Decompiled with JetBrains decompiler
// Type: Celeste.Snow3D
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  public class Snow3D : Entity
  {
    private static Color[] alphas = new Color[32];
    private List<Snow3D.Particle> particles = new List<Snow3D.Particle>();
    private BoundingFrustum Frustum = new BoundingFrustum(Matrix.Identity);
    private BoundingFrustum LastFrustum = new BoundingFrustum(Matrix.Identity);
    private float Range = 30f;
    private MountainModel Model;

    public Snow3D(MountainModel model)
    {
      this.Model = model;
      for (int index = 0; index < Snow3D.alphas.Length; ++index)
        Snow3D.alphas[index] = Color.White * ((float) index / (float) Snow3D.alphas.Length);
      for (int index = 0; index < 400; ++index)
      {
        Snow3D.Particle particle = new Snow3D.Particle(this, 1f);
        this.particles.Add(particle);
        this.Add((Component) particle);
      }
    }

    public override void Update()
    {
      Overworld scene = this.Scene as Overworld;
      this.Range = 20f;
      if (SaveData.Instance != null && scene != null && (scene.IsCurrent<OuiChapterPanel>() || scene.IsCurrent<OuiChapterSelect>()))
      {
        int id = SaveData.Instance.LastArea.ID;
        int num;
        switch (id)
        {
          case 0:
          case 2:
            num = 1;
            break;
          default:
            num = id == 8 ? 1 : 0;
            break;
        }
        if (num != 0)
          this.Range = 3f;
        else if (id == 1)
          this.Range = 12f;
      }
      Matrix matrix = Matrix.CreateTranslation(-this.Model.Camera.Position) * Matrix.CreateFromQuaternion(this.Model.Camera.Rotation) * Matrix.CreatePerspectiveFieldOfView(0.9817477f, (float) Engine.Width / (float) Engine.Height, 0.1f, this.Range);
      if (this.Scene.OnInterval(0.05f))
        this.LastFrustum.Matrix = matrix;
      this.Frustum.Matrix = matrix;
      base.Update();
    }

    [Tracked(false)]
    public class Particle : Billboard
    {
      public Snow3D Manager;
      public Vector2 Float;
      public float Percent;
      public float Duration;
      public float Speed;
      private float size;

      public Particle(Snow3D manager, float size)
        : base(GFX.Overworld["snow"], Vector3.Zero, new Vector2?(), new Color?(), new Vector2?())
      {
        this.Manager = manager;
        this.size = size;
        this.Size = Vector2.One * size;
        this.Reset(Calc.Random.NextFloat());
        this.ResetPosition();
      }

      public void ResetPosition()
      {
        float range = this.Manager.Range;
        this.Position = this.Manager.Model.Camera.Position + this.Manager.Model.Forward * range * 0.5f + new Vector3(Calc.Random.Range(-range, range), Calc.Random.Range(-range, range), Calc.Random.Range(-range, range));
      }

      public void Reset(float percent = 0.0f)
      {
        float num = this.Manager.Range / 30f;
        this.Speed = Calc.Random.Range(1f, 6f) * num;
        this.Percent = percent;
        this.Duration = Calc.Random.Range(1f, 5f);
        this.Float = new Vector2((float) Calc.Random.Range(-1, 1), (float) Calc.Random.Range(-1, 1)).SafeNormalize() * 0.25f;
        this.Scale = Vector2.One * 0.05f * num;
      }

      public override void Update()
      {
        if ((double) this.Percent > 1.0 || !this.InView())
        {
          this.ResetPosition();
          int num = 0;
          while (!this.InView() && num++ < 10)
            this.ResetPosition();
          if (num <= 10)
          {
            this.Reset(!this.InLastView() ? Calc.Random.NextFloat() : 0.0f);
          }
          else
          {
            this.Color = Color.Transparent;
            return;
          }
        }
        this.Percent += Engine.DeltaTime / this.Duration;
        this.Color = Color.White * Calc.YoYo(this.Percent);
        this.Position.Y -= this.Speed * Engine.DeltaTime;
        this.Position.X += this.Float.X * Engine.DeltaTime;
        this.Position.Z += this.Float.Y * Engine.DeltaTime;
      }

      private bool InView()
      {
        return this.Manager.Frustum.Contains(this.Position) == ContainmentType.Contains && (double) this.Position.Y > 0.0;
      }

      private bool InLastView()
      {
        return this.Manager.LastFrustum != (BoundingFrustum) null && this.Manager.LastFrustum.Contains(this.Position) == ContainmentType.Contains;
      }
    }
  }
}

