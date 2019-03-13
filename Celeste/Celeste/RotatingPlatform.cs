// Decompiled with JetBrains decompiler
// Type: Celeste.RotatingPlatform
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class RotatingPlatform : JumpThru
  {
    private const float RotateSpeed = 1.047198f;
    private Vector2 center;
    private bool clockwise;
    private float length;
    private float currentAngle;

    public RotatingPlatform(Vector2 position, int width, Vector2 center, bool clockwise)
      : base(position, width, false)
    {
      this.Collider.Position.X = (float) (-width / 2);
      this.Collider.Position.Y = (float) (-(double) this.Height / 2.0);
      this.center = center;
      this.clockwise = clockwise;
      this.length = (position - center).Length();
      this.currentAngle = (position - center).Angle();
      this.SurfaceSoundIndex = 5;
      this.Add((Component) new LightOcclude(0.2f));
    }

    public override void Update()
    {
      base.Update();
      if (this.clockwise)
        this.currentAngle -= 1.047198f * Engine.DeltaTime;
      else
        this.currentAngle += 1.047198f * Engine.DeltaTime;
      this.currentAngle = Calc.WrapAngle(this.currentAngle);
      this.MoveTo(this.center + Calc.AngleToVector(this.currentAngle, this.length));
    }

    public override void Render()
    {
      base.Render();
      Draw.Rect(this.Collider, Color.White);
    }
  }
}

