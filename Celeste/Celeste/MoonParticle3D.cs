// Decompiled with JetBrains decompiler
// Type: Celeste.MoonParticle3D
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class MoonParticle3D : Entity
  {
    private MountainModel model;
    private List<MoonParticle3D.Particle> particles = new List<MoonParticle3D.Particle>();

    public MoonParticle3D(MountainModel model, Vector3 center)
    {
      this.model = model;
      this.Visible = false;
      Matrix rotationZ = Matrix.CreateRotationZ(0.4f);
      Color[] colorArray1 = new Color[2]
      {
        Calc.HexToColor("53f3dd"),
        Calc.HexToColor("53c9f3")
      };
      for (int index = 0; index < 20; ++index)
        this.Add((Component) new MoonParticle3D.Particle(OVR.Atlas["star"], Calc.Random.Choose<Color>(colorArray1), center, 1f, rotationZ));
      for (int index = 0; index < 30; ++index)
        this.Add((Component) new MoonParticle3D.Particle(OVR.Atlas["snow"], Calc.Random.Choose<Color>(colorArray1), center, 0.3f, rotationZ));
      Matrix matrix = Matrix.CreateRotationZ(0.8f) * Matrix.CreateRotationX(0.4f);
      Color[] colorArray2 = new Color[2]
      {
        Calc.HexToColor("ab6ffa"),
        Calc.HexToColor("fa70ea")
      };
      for (int index = 0; index < 20; ++index)
        this.Add((Component) new MoonParticle3D.Particle(OVR.Atlas["star"], Calc.Random.Choose<Color>(colorArray2), center, 1f, matrix));
      for (int index = 0; index < 30; ++index)
        this.Add((Component) new MoonParticle3D.Particle(OVR.Atlas["snow"], Calc.Random.Choose<Color>(colorArray2), center, 0.3f, matrix));
    }

    public override void Update()
    {
      base.Update();
      this.Visible = (double) this.model.StarEase > 0.0;
    }

    public class Particle : Billboard
    {
      public Vector3 Center;
      public Matrix Matrix;
      public float Rotation;
      public float Distance;
      public float YOff;
      public float Spd;

      public Particle(MTexture texture, Color color, Vector3 center, float size, Matrix matrix)
        : base(texture, Vector3.Zero, color: new Color?(color))
      {
        this.Center = center;
        this.Matrix = matrix;
        this.Size = Vector2.One * Calc.Random.Range(0.05f, 0.15f) * size;
        this.Distance = Calc.Random.Range(1.8f, 1.9f);
        this.Rotation = Calc.Random.NextFloat(6.2831855f);
        this.YOff = Calc.Random.Range(-0.1f, 0.1f);
        this.Spd = Calc.Random.Range(0.8f, 1.2f);
      }

      public override void Update()
      {
        this.Rotation += Engine.DeltaTime * 0.4f * this.Spd;
        this.Position = this.Center + Vector3.Transform(new Vector3((float) Math.Cos((double) this.Rotation) * this.Distance, (float) Math.Sin((double) this.Rotation * 3.0) * 0.25f + this.YOff, (float) Math.Sin((double) this.Rotation) * this.Distance), this.Matrix);
      }
    }
  }
}
