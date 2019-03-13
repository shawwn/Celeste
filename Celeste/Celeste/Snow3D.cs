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
    private BoundingFrustum Frustum = new BoundingFrustum(Matrix.get_Identity());
    private BoundingFrustum LastFrustum = new BoundingFrustum(Matrix.get_Identity());
    private float Range = 30f;
    private MountainModel Model;

    public Snow3D(MountainModel model)
    {
      this.Model = model;
      for (int index = 0; index < Snow3D.alphas.Length; ++index)
        Snow3D.alphas[index] = Color.op_Multiply(Color.get_White(), (float) index / (float) Snow3D.alphas.Length);
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
      Matrix matrix = Matrix.op_Multiply(Matrix.op_Multiply(Matrix.CreateTranslation(Vector3.op_UnaryNegation(this.Model.Camera.Position)), Matrix.CreateFromQuaternion(this.Model.Camera.Rotation)), Matrix.CreatePerspectiveFieldOfView(0.9817477f, (float) Engine.Width / (float) Engine.Height, 0.1f, this.Range));
      if (this.Scene.OnInterval(0.05f))
        this.LastFrustum.set_Matrix(matrix);
      this.Frustum.set_Matrix(matrix);
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
        : base(GFX.Overworld["snow"], Vector3.get_Zero(), new Vector2?(), new Color?(), new Vector2?())
      {
        this.Manager = manager;
        this.size = size;
        this.Size = Vector2.op_Multiply(Vector2.get_One(), size);
        this.Reset(Calc.Random.NextFloat());
        this.ResetPosition();
      }

      public void ResetPosition()
      {
        float range = this.Manager.Range;
        this.Position = Vector3.op_Addition(Vector3.op_Addition(this.Manager.Model.Camera.Position, Vector3.op_Multiply(Vector3.op_Multiply(this.Manager.Model.Forward, range), 0.5f)), new Vector3(Calc.Random.Range(-range, range), Calc.Random.Range(-range, range), Calc.Random.Range(-range, range)));
      }

      public void Reset(float percent = 0.0f)
      {
        float num = this.Manager.Range / 30f;
        this.Speed = Calc.Random.Range(1f, 6f) * num;
        this.Percent = percent;
        this.Duration = Calc.Random.Range(1f, 5f);
        this.Float = Vector2.op_Multiply(new Vector2((float) Calc.Random.Range(-1, 1), (float) Calc.Random.Range(-1, 1)).SafeNormalize(), 0.25f);
        this.Scale = Vector2.op_Multiply(Vector2.op_Multiply(Vector2.get_One(), 0.05f), num);
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
            this.Color = Color.get_Transparent();
            return;
          }
        }
        this.Percent += Engine.DeltaTime / this.Duration;
        this.Color = Color.op_Multiply(Color.get_White(), Calc.YoYo(this.Percent));
        ref __Null local1 = ref this.Position.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local1 = ^(float&) ref local1 - this.Speed * Engine.DeltaTime;
        ref __Null local2 = ref this.Position.X;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local2 = ^(float&) ref local2 + (float) this.Float.X * Engine.DeltaTime;
        ref __Null local3 = ref this.Position.Z;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local3 = ^(float&) ref local3 + (float) this.Float.Y * Engine.DeltaTime;
      }

      private bool InView()
      {
        if (this.Manager.Frustum.Contains(this.Position) == 1)
          return this.Position.Y > 0.0;
        return false;
      }

      private bool InLastView()
      {
        if (BoundingFrustum.op_Inequality(this.Manager.LastFrustum, (BoundingFrustum) null))
          return this.Manager.LastFrustum.Contains(this.Position) == 1;
        return false;
      }
    }
  }
}
