// Decompiled with JetBrains decompiler
// Type: Celeste.Godrays
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
  public class Godrays : Backdrop
  {
    private VertexPositionColor[] vertices = new VertexPositionColor[36];
    private int vertexCount = 0;
    private Color rayColor = Calc.HexToColor("f52b63") * 0.5f;
    private Godrays.Ray[] rays = new Godrays.Ray[6];
    private float fade = 0.0f;
    private const int RayCount = 6;

    public Godrays()
    {
      this.UseSpritebatch = false;
      for (int index = 0; index < this.rays.Length; ++index)
      {
        this.rays[index].Reset();
        this.rays[index].Percent = Calc.Random.NextFloat();
      }
    }

    public override void Update(Scene scene)
    {
      Level level = scene as Level;
      this.fade = Calc.Approach(this.fade, this.IsVisible(level) ? 1f : 0.0f, Engine.DeltaTime);
      this.Visible = (double) this.fade > 0.0;
      if (!this.Visible)
        return;
      Player entity = level.Tracker.GetEntity<Player>();
      Vector2 vector = Calc.AngleToVector(-1.670796f, 1f);
      Vector2 vector2_1 = new Vector2(-vector.Y, vector.X);
      int num1 = 0;
      for (int index1 = 0; index1 < this.rays.Length; ++index1)
      {
        if ((double) this.rays[index1].Percent >= 1.0)
          this.rays[index1].Reset();
        this.rays[index1].Percent += Engine.DeltaTime / this.rays[index1].Duration;
        this.rays[index1].Y += 8f * Engine.DeltaTime;
        float percent = this.rays[index1].Percent;
        float num2 = this.Mod(this.rays[index1].X - level.Camera.X * 0.9f, 384f) - 32f;
        float num3 = this.Mod(this.rays[index1].Y - level.Camera.Y * 0.9f, 244f) - 32f;
        float width = this.rays[index1].Width;
        float length = this.rays[index1].Length;
        Vector2 vector2_2 = new Vector2((float) (int) num2, (float) (int) num3);
        Color color = this.rayColor * Ease.CubeInOut(Calc.Clamp((float) (((double) percent < 0.5 ? (double) percent : 1.0 - (double) percent) * 2.0), 0.0f, 1f)) * this.fade;
        if (entity != null)
        {
          float num4 = (vector2_2 + level.Camera.Position - entity.Position).Length();
          if ((double) num4 < 64.0)
            color *= (float) (0.25 + 0.75 * ((double) num4 / 64.0));
        }
        VertexPositionColor vertexPositionColor1 = new VertexPositionColor(new Vector3(vector2_2 + vector2_1 * width + vector * length, 0.0f), color);
        VertexPositionColor vertexPositionColor2 = new VertexPositionColor(new Vector3(vector2_2 - vector2_1 * width, 0.0f), color);
        VertexPositionColor vertexPositionColor3 = new VertexPositionColor(new Vector3(vector2_2 + vector2_1 * width, 0.0f), color);
        VertexPositionColor vertexPositionColor4 = new VertexPositionColor(new Vector3(vector2_2 - vector2_1 * width - vector * length, 0.0f), color);
        VertexPositionColor[] vertices1 = this.vertices;
        int index2 = num1;
        int num5 = index2 + 1;
        VertexPositionColor vertexPositionColor5 = vertexPositionColor1;
        vertices1[index2] = vertexPositionColor5;
        VertexPositionColor[] vertices2 = this.vertices;
        int index3 = num5;
        int num6 = index3 + 1;
        VertexPositionColor vertexPositionColor6 = vertexPositionColor2;
        vertices2[index3] = vertexPositionColor6;
        VertexPositionColor[] vertices3 = this.vertices;
        int index4 = num6;
        int num7 = index4 + 1;
        VertexPositionColor vertexPositionColor7 = vertexPositionColor3;
        vertices3[index4] = vertexPositionColor7;
        VertexPositionColor[] vertices4 = this.vertices;
        int index5 = num7;
        int num8 = index5 + 1;
        VertexPositionColor vertexPositionColor8 = vertexPositionColor2;
        vertices4[index5] = vertexPositionColor8;
        VertexPositionColor[] vertices5 = this.vertices;
        int index6 = num8;
        int num9 = index6 + 1;
        VertexPositionColor vertexPositionColor9 = vertexPositionColor3;
        vertices5[index6] = vertexPositionColor9;
        VertexPositionColor[] vertices6 = this.vertices;
        int index7 = num9;
        num1 = index7 + 1;
        VertexPositionColor vertexPositionColor10 = vertexPositionColor4;
        vertices6[index7] = vertexPositionColor10;
      }
      this.vertexCount = num1;
    }

    private float Mod(float x, float m)
    {
      return (x % m + m) % m;
    }

    public override void Render(Scene scene)
    {
      if (this.vertexCount <= 0 || (double) this.fade <= 0.0)
        return;
      GFX.DrawVertices<VertexPositionColor>(Matrix.Identity, this.vertices, this.vertexCount, (Effect) null, (BlendState) null);
    }

    private struct Ray
    {
      public float X;
      public float Y;
      public float Percent;
      public float Duration;
      public float Width;
      public float Length;

      public void Reset()
      {
        this.Percent = 0.0f;
        this.X = Calc.Random.NextFloat(384f);
        this.Y = Calc.Random.NextFloat(244f);
        this.Duration = (float) (4.0 + (double) Calc.Random.NextFloat() * 8.0);
        this.Width = (float) Calc.Random.Next(8, 16);
        this.Length = (float) Calc.Random.Next(20, 40);
      }
    }
  }
}

