// Decompiled with JetBrains decompiler
// Type: Celeste.RainFG
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class RainFG : Backdrop
  {
    public float Alpha = 1f;
    private float visibleFade = 1f;
    private float linearFade = 1f;
    private RainFG.Particle[] particles = new RainFG.Particle[240];

    public RainFG()
    {
      for (int index = 0; index < this.particles.Length; ++index)
        this.particles[index].Init();
    }

    public override void Update(Scene scene)
    {
      base.Update(scene);
      bool flag = this.IsVisible(scene as Level);
      (scene as Level).Raining = flag;
      this.visibleFade = Calc.Approach(this.visibleFade, flag ? 1f : 0.0f, Engine.DeltaTime * (flag ? 10f : 0.25f));
      if (this.FadeX != null)
        this.linearFade = this.FadeX.Value((scene as Level).Camera.X + 160f);
      for (int index = 0; index < this.particles.Length; ++index)
        this.particles[index].Position += this.particles[index].Speed * Engine.DeltaTime;
    }

    public override void Render(Scene scene)
    {
      if ((double) this.Alpha <= 0.0 || (double) this.visibleFade <= 0.0 || (double) this.linearFade <= 0.0)
        return;
      Color color = Calc.HexToColor("161933") * 0.5f * this.Alpha * this.linearFade * this.visibleFade;
      Camera camera = (scene as Level).Camera;
      for (int index = 0; index < this.particles.Length; ++index)
      {
        Vector2 position = new Vector2(this.mod((float) ((double) this.particles[index].Position.X - (double) camera.X - 32.0), 384f), this.mod((float) ((double) this.particles[index].Position.Y - (double) camera.Y - 32.0), 244f));
        Draw.Pixel.DrawCentered(position, color, this.particles[index].Scale, this.particles[index].Rotation);
      }
    }

    private float mod(float x, float m) => (x % m + m) % m;

    private struct Particle
    {
      public Vector2 Position;
      public Vector2 Speed;
      public float Rotation;
      public Vector2 Scale;

      public void Init()
      {
        this.Position = new Vector2(Calc.Random.NextFloat(384f) - 32f, Calc.Random.NextFloat(244f) - 32f);
        this.Rotation = 1.5707964f + Calc.Random.Range(-0.05f, 0.05f);
        this.Speed = Calc.AngleToVector(this.Rotation, Calc.Random.Range(200f, 600f));
        this.Scale = new Vector2((float) (4.0 + ((double) this.Speed.Length() - 200.0) / 400.0 * 12.0), 1f);
      }
    }
  }
}
