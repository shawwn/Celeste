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
    public Vector2 CameraOffset = Vector2.get_Zero();
    public float Alpha = 1f;
    private Vector2 scale = Vector2.get_One();
    private float loopWidth = 640f;
    private float loopHeight = 360f;
    private float visibleFade = 1f;
    private Vector2[] positions;
    private SineWave[] sines;
    private float rotation;

    public WindSnowFG()
    {
      this.Color = Color.get_White();
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
      bool flag = level.Wind.Y == 0.0;
      if (flag)
      {
        this.scale.X = (__Null) (double) Math.Max(1f, Math.Abs((float) level.Wind.X) / 100f);
        this.rotation = Calc.Approach(this.rotation, 0.0f, Engine.DeltaTime * 8f);
      }
      else
      {
        this.scale.X = (__Null) (double) Math.Max(1f, Math.Abs((float) level.Wind.Y) / 40f);
        this.rotation = Calc.Approach(this.rotation, -1.570796f, Engine.DeltaTime * 8f);
      }
      this.scale.Y = (__Null) (1.0 / (double) Math.Max(1f, (float) (this.scale.X * 0.25)));
      for (int index = 0; index < this.positions.Length; ++index)
      {
        float num = this.sines[index % this.sines.Length].Value;
        Vector2 zero = Vector2.get_Zero();
        if (flag)
          ((Vector2) ref zero).\u002Ector((float) (level.Wind.X + (double) num * 10.0), 20f);
        else
          ((Vector2) ref zero).\u002Ector(0.0f, (float) (level.Wind.Y * 3.0 + (double) num * 10.0));
        ref Vector2 local = ref this.positions[index];
        local = Vector2.op_Addition(local, Vector2.op_Multiply(zero, Engine.DeltaTime));
      }
    }

    public override void Render(Scene scene)
    {
      if ((double) this.Alpha <= 0.0)
        return;
      Color color = Color.op_Multiply(Color.op_Multiply(this.Color, this.visibleFade), this.Alpha);
      int num1 = (scene as Level).Wind.Y == 0.0 ? (int) (double) this.positions.Length : (int) ((double) this.positions.Length * 0.600000023841858);
      int num2 = 0;
      foreach (Vector2 position in this.positions)
      {
        ref __Null local1 = ref position.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local1 = ^(float&) ref local1 - ((scene as Level).Camera.Y + (float) this.CameraOffset.Y);
        ref __Null local2 = ref position.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local2 = ^(float&) ref local2 % this.loopHeight;
        if (position.Y < 0.0)
        {
          ref __Null local3 = ref position.Y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local3 = ^(float&) ref local3 + this.loopHeight;
        }
        ref __Null local4 = ref position.X;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local4 = ^(float&) ref local4 - ((scene as Level).Camera.X + (float) this.CameraOffset.X);
        ref __Null local5 = ref position.X;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local5 = ^(float&) ref local5 % this.loopWidth;
        if (position.X < 0.0)
        {
          ref __Null local3 = ref position.X;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local3 = ^(float&) ref local3 + this.loopWidth;
        }
        if (num2 < num1)
          GFX.Game["particles/snow"].DrawCentered(position, color, this.scale, this.rotation);
        ++num2;
      }
    }
  }
}
