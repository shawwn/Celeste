// Decompiled with JetBrains decompiler
// Type: Celeste.HeatWave
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class HeatWave : Backdrop
  {
    private static readonly Color[] hotColors = new Color[2]
    {
      Color.Red,
      Color.Orange
    };
    private static readonly Color[] coldColors = new Color[2]
    {
      Color.LightSkyBlue,
      Color.Teal
    };
    private HeatWave.Particle[] particles = new HeatWave.Particle[50];
    private Color[] currentColors;
    private float colorLerp;
    private float fade;
    private float heat;
    private Parallax mist1;
    private Parallax mist2;
    private bool show;
    private bool wasShow;

    public HeatWave()
    {
      for (int i = 0; i < this.particles.Length; ++i)
        this.Reset(i, Calc.Random.NextFloat());
      this.currentColors = new Color[HeatWave.hotColors.Length];
      this.colorLerp = 1f;
      this.mist1 = new Parallax(GFX.Misc["mist"]);
      this.mist2 = new Parallax(GFX.Misc["mist"]);
    }

    private void Reset(int i, float p)
    {
      this.particles[i].Percent = p;
      this.particles[i].Position = new Vector2((float) Calc.Random.Range(0, 320), (float) Calc.Random.Range(0, 180));
      this.particles[i].Speed = (float) Calc.Random.Range(4, 14);
      this.particles[i].Spin = Calc.Random.Range(0.25f, 18.84956f);
      this.particles[i].Duration = Calc.Random.Range(1f, 4f);
      this.particles[i].Direction = Calc.AngleToVector(Calc.Random.NextFloat(6.283185f), 1f);
      this.particles[i].Color = Calc.Random.Next(HeatWave.hotColors.Length);
    }

    public override void Update(Scene scene)
    {
      Level level = scene as Level;
      this.show = this.IsVisible(level) && (uint) level.CoreMode > 0U;
      if (this.show)
      {
        if (!this.wasShow)
        {
          this.colorLerp = level.CoreMode == Session.CoreModes.Hot ? 1f : 0.0f;
          level.NextColorGrade(level.CoreMode == Session.CoreModes.Hot ? "hot" : "cold");
        }
        else
          level.SnapColorGrade(level.CoreMode == Session.CoreModes.Hot ? "hot" : "cold");
        this.colorLerp = Calc.Approach(this.colorLerp, level.CoreMode == Session.CoreModes.Hot ? 1f : 0.0f, Engine.DeltaTime * 100f);
        for (int index = 0; index < this.currentColors.Length; ++index)
          this.currentColors[index] = Color.Lerp(HeatWave.coldColors[index], HeatWave.hotColors[index], this.colorLerp);
      }
      else
        level.NextColorGrade("none");
      for (int i = 0; i < this.particles.Length; ++i)
      {
        if ((double) this.particles[i].Percent >= 1.0)
          this.Reset(i, 0.0f);
        float num = 1f;
        if (level.CoreMode == Session.CoreModes.Cold)
          num = 0.25f;
        this.particles[i].Percent += Engine.DeltaTime / this.particles[i].Duration;
        this.particles[i].Position += this.particles[i].Direction * this.particles[i].Speed * num * Engine.DeltaTime;
        this.particles[i].Direction.Rotate(this.particles[i].Spin * Engine.DeltaTime);
        if (level.CoreMode == Session.CoreModes.Hot)
          this.particles[i].Position.Y -= 10f * Engine.DeltaTime;
      }
      this.fade = Calc.Approach(this.fade, this.show ? 1f : 0.0f, Engine.DeltaTime);
      this.heat = Calc.Approach(this.heat, !this.show || level.CoreMode != Session.CoreModes.Hot ? 0.0f : 1f, Engine.DeltaTime * 100f);
      this.mist1.Color = Color.Lerp(Calc.HexToColor("639bff"), Calc.HexToColor("f1b22b"), this.heat) * this.fade * 0.7f;
      this.mist2.Color = Color.Lerp(Calc.HexToColor("5fcde4"), Calc.HexToColor("f12b3a"), this.heat) * this.fade * 0.7f;
      this.mist1.Speed = new Vector2(4f, -20f) * this.heat;
      this.mist2.Speed = new Vector2(4f, -40f) * this.heat;
      this.mist1.Update(scene);
      this.mist2.Update(scene);
      if ((double) this.heat > 0.0)
      {
        Distort.WaterSineDirection = -1f;
        Distort.WaterAlpha = this.heat * 0.5f;
      }
      else
        Distort.WaterAlpha = 1f;
      this.wasShow = this.show;
    }

    public void RenderDisplacement(Level level)
    {
      if ((double) this.heat <= 0.0)
        return;
      Color color = new Color(0.5f, 0.5f, 0.1f, 1f);
      Draw.Rect(level.Camera.X - 5f, level.Camera.Y - 5f, 370f, 190f, color);
    }

    public override void Render(Scene scene)
    {
      if ((double) this.fade <= 0.0)
        return;
      Camera camera = (scene as Level).Camera;
      for (int index = 0; index < this.particles.Length; ++index)
      {
        Vector2 position = new Vector2()
        {
          X = this.Mod(this.particles[index].Position.X - camera.X, 320f),
          Y = this.Mod(this.particles[index].Position.Y - camera.Y, 180f)
        };
        float percent = this.particles[index].Percent;
        float num = (double) percent >= 0.699999988079071 ? Calc.ClampedMap(percent, 0.7f, 1f, 1f, 0.0f) : Calc.ClampedMap(percent, 0.0f, 0.3f, 0.0f, 1f);
        Draw.Rect(position, 1f, 1f, this.currentColors[this.particles[index].Color] * (this.fade * num));
      }
      this.mist1.Render(scene);
      this.mist2.Render(scene);
    }

    private float Mod(float x, float m)
    {
      return (x % m + m) % m;
    }

    private struct Particle
    {
      public Vector2 Position;
      public float Percent;
      public float Duration;
      public Vector2 Direction;
      public float Speed;
      public float Spin;
      public int Color;
    }
  }
}

