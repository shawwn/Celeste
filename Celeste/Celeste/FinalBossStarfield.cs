// Decompiled with JetBrains decompiler
// Type: Celeste.FinalBossStarfield
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
  public class FinalBossStarfield : Backdrop
  {
    private static readonly Color[] colors = new Color[4]
    {
      Calc.HexToColor("030c1b"),
      Calc.HexToColor("0b031b"),
      Calc.HexToColor("1b0319"),
      Calc.HexToColor("0f0301")
    };
    public float Alpha = 1f;
    private FinalBossStarfield.Particle[] particles = new FinalBossStarfield.Particle[200];
    private VertexPositionColor[] verts = new VertexPositionColor[1206];
    private const int particleCount = 200;

    public FinalBossStarfield()
    {
      this.UseSpritebatch = false;
      for (int index = 0; index < 200; ++index)
      {
        this.particles[index].Speed = Calc.Random.Range(500f, 1200f);
        this.particles[index].Direction = new Vector2(-1f, 0.0f);
        this.particles[index].DirectionApproach = Calc.Random.Range(0.25f, 4f);
        this.particles[index].Position.X = (__Null) (double) Calc.Random.Range(0, 384);
        this.particles[index].Position.Y = (__Null) (double) Calc.Random.Range(0, 244);
        this.particles[index].Color = Calc.Random.Choose<Color>(FinalBossStarfield.colors);
      }
    }

    public override void Update(Scene scene)
    {
      base.Update(scene);
      if (!this.Visible || (double) this.Alpha <= 0.0)
        return;
      Vector2 vector;
      ((Vector2) ref vector).\u002Ector(-1f, 0.0f);
      Level level = scene as Level;
      if (level.Bounds.Height > level.Bounds.Width)
        ((Vector2) ref vector).\u002Ector(0.0f, -1f);
      float target = vector.Angle();
      for (int index = 0; index < 200; ++index)
      {
        ref Vector2 local = ref this.particles[index].Position;
        local = Vector2.op_Addition(local, Vector2.op_Multiply(Vector2.op_Multiply(this.particles[index].Direction, this.particles[index].Speed), Engine.DeltaTime));
        float angleRadians = Calc.AngleApproach(this.particles[index].Direction.Angle(), target, this.particles[index].DirectionApproach * Engine.DeltaTime);
        this.particles[index].Direction = Calc.AngleToVector(angleRadians, 1f);
      }
    }

    public override void Render(Scene scene)
    {
      Vector2 position1 = (scene as Level).Camera.Position;
      Color color1 = Color.op_Multiply(Color.get_Black(), this.Alpha);
      this.verts[0].Color = (__Null) color1;
      this.verts[0].Position = (__Null) new Vector3(-10f, -10f, 0.0f);
      this.verts[1].Color = (__Null) color1;
      this.verts[1].Position = (__Null) new Vector3(330f, -10f, 0.0f);
      this.verts[2].Color = (__Null) color1;
      this.verts[2].Position = (__Null) new Vector3(330f, 190f, 0.0f);
      this.verts[3].Color = (__Null) color1;
      this.verts[3].Position = (__Null) new Vector3(-10f, -10f, 0.0f);
      this.verts[4].Color = (__Null) color1;
      this.verts[4].Position = (__Null) new Vector3(330f, 190f, 0.0f);
      this.verts[5].Color = (__Null) color1;
      this.verts[5].Position = (__Null) new Vector3(-10f, 190f, 0.0f);
      for (int index1 = 0; index1 < 200; ++index1)
      {
        int index2 = (index1 + 1) * 6;
        float num1 = Calc.ClampedMap(this.particles[index1].Speed, 0.0f, 1200f, 1f, 64f);
        float num2 = Calc.ClampedMap(this.particles[index1].Speed, 0.0f, 1200f, 3f, 0.6f);
        Vector2 direction = this.particles[index1].Direction;
        Vector2 vector2_1 = direction.Perpendicular();
        Vector2 position2 = this.particles[index1].Position;
        position2.X = (__Null) ((double) this.Mod((float) (position2.X - position1.X * 0.899999976158142), 384f) - 32.0);
        position2.Y = (__Null) ((double) this.Mod((float) (position2.Y - position1.Y * 0.899999976158142), 244f) - 32.0);
        Vector2 vector2_2 = Vector2.op_Subtraction(Vector2.op_Subtraction(position2, Vector2.op_Multiply(Vector2.op_Multiply(direction, num1), 0.5f)), Vector2.op_Multiply(vector2_1, num2));
        Vector2 vector2_3 = Vector2.op_Subtraction(Vector2.op_Addition(position2, Vector2.op_Multiply(Vector2.op_Multiply(direction, num1), 1f)), Vector2.op_Multiply(vector2_1, num2));
        Vector2 vector2_4 = Vector2.op_Addition(Vector2.op_Addition(position2, Vector2.op_Multiply(Vector2.op_Multiply(direction, num1), 0.5f)), Vector2.op_Multiply(vector2_1, num2));
        Vector2 vector2_5 = Vector2.op_Addition(Vector2.op_Subtraction(position2, Vector2.op_Multiply(Vector2.op_Multiply(direction, num1), 1f)), Vector2.op_Multiply(vector2_1, num2));
        Color color2 = Color.op_Multiply(this.particles[index1].Color, this.Alpha);
        this.verts[index2].Color = (__Null) color2;
        this.verts[index2].Position = (__Null) new Vector3(vector2_2, 0.0f);
        this.verts[index2 + 1].Color = (__Null) color2;
        this.verts[index2 + 1].Position = (__Null) new Vector3(vector2_3, 0.0f);
        this.verts[index2 + 2].Color = (__Null) color2;
        this.verts[index2 + 2].Position = (__Null) new Vector3(vector2_4, 0.0f);
        this.verts[index2 + 3].Color = (__Null) color2;
        this.verts[index2 + 3].Position = (__Null) new Vector3(vector2_2, 0.0f);
        this.verts[index2 + 4].Color = (__Null) color2;
        this.verts[index2 + 4].Position = (__Null) new Vector3(vector2_4, 0.0f);
        this.verts[index2 + 5].Color = (__Null) color2;
        this.verts[index2 + 5].Position = (__Null) new Vector3(vector2_5, 0.0f);
      }
      GFX.DrawVertices<VertexPositionColor>(Matrix.get_Identity(), this.verts, this.verts.Length, (Effect) null, (BlendState) null);
    }

    private float Mod(float x, float m)
    {
      return (x % m + m) % m;
    }

    private struct Particle
    {
      public Vector2 Position;
      public Vector2 Direction;
      public float Speed;
      public Color Color;
      public float DirectionApproach;
    }
  }
}
