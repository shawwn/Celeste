// Decompiled with JetBrains decompiler
// Type: Celeste.WindSnowFG
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class WindSnowFG : Backdrop
  {
    public Vector2 CameraOffset = Vector2.Zero;
    public float Alpha = 1f;
    private Vector2 scale = Vector2.One;
    private float rotation = 0.0f;
    private float loopWidth = 640f;
    private float loopHeight = 360f;
    private float visibleFade = 1f;
    private Vector2[] positions;
    private SineWave[] sines;

    public WindSnowFG()
    {
      this.Color = Color.White;
      this.positions = new Vector2[240];
      for (int index = 0; index < this.positions.Length; ++index)
        this.positions[index] = Calc.Random.Range(new Vector2(0.0f, 0.0f), new Vector2(this.loopWidth, this.loopHeight));
      this.sines = new SineWave[16];
      for (int index = 0; index < this.sines.Length; ++index)
      {
        this.sines[index] = new SineWave(Calc.Random.Range(0.8f, 1.2f));
        this.sines[index].Randomize();
      }
    }

    public override void Update(Scene scene)
    {
      base.Update(scene);
      this.visibleFade = Calc.Approach(this.visibleFade, this.IsVisible(scene as Level) ? 1f : 0.0f, Engine.DeltaTime * 2f);
      Level level = scene as Level;
      foreach (Component sine in this.sines)
        sine.Update();
      bool flag = (double) level.Wind.Y == 0.0;
      if (flag)
      {
        this.scale.X = Math.Max(1f, Math.Abs(level.Wind.X) / 100f);
        this.rotation = Calc.Approach(this.rotation, 0.0f, Engine.DeltaTime * 8f);
      }
      else
      {
        this.scale.X = Math.Max(1f, Math.Abs(level.Wind.Y) / 40f);
        this.rotation = Calc.Approach(this.rotation, -1.570796f, Engine.DeltaTime * 8f);
      }
      this.scale.Y = 1f / Math.Max(1f, this.scale.X * 0.25f);
      for (int index = 0; index < this.positions.Length; ++index)
      {
        float num = this.sines[index % this.sines.Length].Value;
        Vector2 vector2 = Vector2.Zero;
        vector2 = !flag ? new Vector2(0.0f, (float) ((double) level.Wind.Y * 3.0 + (double) num * 10.0)) : new Vector2(level.Wind.X + num * 10f, 20f);
        this.positions[index] += vector2 * Engine.DeltaTime;
      }
    }

    public override void Render(Scene scene)
    {
      if ((double) this.Alpha <= 0.0)
        return;
      Color color = this.Color * this.visibleFade * this.Alpha;
      int num1 = (double) (scene as Level).Wind.Y == 0.0 ? (int) (double) this.positions.Length : (int) ((double) this.positions.Length * 0.600000023841858);
      int num2 = 0;
      foreach (Vector2 position_ in this.positions)
      {
        Vector2 position = position_;
        position.Y -= (scene as Level).Camera.Y + this.CameraOffset.Y;
        position.Y %= this.loopHeight;
        if ((double) position.Y < 0.0)
          position.Y += this.loopHeight;
        position.X -= (scene as Level).Camera.X + this.CameraOffset.X;
        position.X %= this.loopWidth;
        if ((double) position.X < 0.0)
          position.X += this.loopWidth;
        if (num2 < num1)
          GFX.Game["particles/snow"].DrawCentered(position, color, this.scale, this.rotation);
        ++num2;
      }
    }
  }
}

