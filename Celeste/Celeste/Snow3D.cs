// Decompiled with JetBrains decompiler
// Type: Celeste.Snow3D
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

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
    private MountainModel Model;
    private float Range = 30f;

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
        switch (SaveData.Instance.LastArea.ID)
        {
          case 0:
          case 2:
          case 8:
            this.Range = 3f;
            break;
          case 1:
            this.Range = 12f;
            break;
        }
      }
      Matrix perspectiveFieldOfView = Matrix.CreatePerspectiveFieldOfView(0.98174775f, (float) Engine.Width / (float) Engine.Height, 0.1f, this.Range);
      Matrix matrix = Matrix.CreateTranslation(-this.Model.Camera.Position) * Matrix.CreateFromQuaternion(this.Model.Camera.Rotation) * perspectiveFieldOfView;
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
        : base(OVR.Atlas["snow"], Vector3.Zero)
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
        float num1 = Calc.YoYo(this.Percent);
        if ((double) this.Manager.Model.SnowForceFloat > 0.0)
          num1 *= this.Manager.Model.SnowForceFloat;
        else if ((double) this.Manager.Model.StarEase > 0.0)
          num1 *= Calc.Map(this.Manager.Model.StarEase, 0.0f, 1f, 1f, 0.0f);
        this.Color = Color.White * num1;
        this.Size.Y = this.size + this.Manager.Model.SnowStretch * (1f - this.Manager.Model.SnowForceFloat);
        this.Position.Y -= (float) (((double) this.Speed + (double) this.Manager.Model.SnowSpeedAddition) * (1.0 - (double) this.Manager.Model.SnowForceFloat)) * Engine.DeltaTime;
        this.Position.X += this.Float.X * Engine.DeltaTime;
        this.Position.Z += this.Float.Y * Engine.DeltaTime;
      }

      private bool InView() => this.Manager.Frustum.Contains(this.Position) == ContainmentType.Contains && (double) this.Position.Y > 0.0;

      private bool InLastView() => this.Manager.LastFrustum != (BoundingFrustum) null && this.Manager.LastFrustum.Contains(this.Position) == ContainmentType.Contains;
    }
  }
}
