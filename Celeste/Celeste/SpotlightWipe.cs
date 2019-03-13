// Decompiled with JetBrains decompiler
// Type: Celeste.SpotlightWipe
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste
{
  public class SpotlightWipe : ScreenWipe
  {
    public static float Modifier = 0.0f;
    private static VertexPositionColor[] vertexBuffer = new VertexPositionColor[768];
    public static Vector2 FocusPoint;
    public bool Linear;
    private const float SmallCircleRadius = 288f;
    private const float EaseDuration = 1.8f;
    private const float EaseOpenPercent = 0.2f;
    private const float EaseClosePercent = 0.2f;
    private EventInstance sfx;

    public SpotlightWipe(Scene scene, bool wipeIn, Action onComplete = null)
      : base(scene, wipeIn, onComplete)
    {
      this.Duration = 1.8f;
      SpotlightWipe.Modifier = 0.0f;
      if (wipeIn)
        this.sfx = Audio.Play("event:/game/general/spotlight_intro");
      else
        this.sfx = Audio.Play("event:/game/general/spotlight_outro");
    }

    public override void Cancel()
    {
      if ((HandleBase) this.sfx != (HandleBase) null)
      {
        int num = (int) this.sfx.stop(STOP_MODE.IMMEDIATE);
        this.sfx = (EventInstance) null;
      }
      base.Cancel();
    }

    public override void Render(Scene scene)
    {
      float t = this.WipeIn ? this.Percent : 1f - this.Percent;
      Vector2 focusPoint = SpotlightWipe.FocusPoint;
      if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
        focusPoint.X = (__Null) (320.0 - focusPoint.X);
      ref __Null local1 = ref focusPoint.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 * 6f;
      ref __Null local2 = ref focusPoint.Y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 * 6f;
      float num = 288f + SpotlightWipe.Modifier;
      float radius = this.Linear ? Ease.CubeInOut(t) * 1920f : ((double) t >= 0.200000002980232 ? ((double) t >= 0.800000011920929 ? num + (float) (((double) t - 0.800000011920929) / 0.200000002980232 * (1920.0 - (double) num)) : num) : Ease.CubeInOut(t / 0.2f) * num);
      SpotlightWipe.DrawSpotlight(focusPoint, radius, ScreenWipe.WipeColor);
    }

    public static void DrawSpotlight(Vector2 position, float radius, Color color)
    {
      Vector2 vector2;
      ((Vector2) ref vector2).\u002Ector(1f, 0.0f);
      for (int index = 0; index < SpotlightWipe.vertexBuffer.Length; index += 12)
      {
        Vector2 vector = Calc.AngleToVector((float) (((double) index + 12.0) / (double) SpotlightWipe.vertexBuffer.Length * 6.28318548202515), 1f);
        SpotlightWipe.vertexBuffer[index].Position = (__Null) new Vector3(Vector2.op_Addition(position, Vector2.op_Multiply(vector2, 5000f)), 0.0f);
        SpotlightWipe.vertexBuffer[index].Color = (__Null) color;
        SpotlightWipe.vertexBuffer[index + 1].Position = (__Null) new Vector3(Vector2.op_Addition(position, Vector2.op_Multiply(vector2, radius)), 0.0f);
        SpotlightWipe.vertexBuffer[index + 1].Color = (__Null) color;
        SpotlightWipe.vertexBuffer[index + 2].Position = (__Null) new Vector3(Vector2.op_Addition(position, Vector2.op_Multiply(vector, radius)), 0.0f);
        SpotlightWipe.vertexBuffer[index + 2].Color = (__Null) color;
        SpotlightWipe.vertexBuffer[index + 3].Position = (__Null) new Vector3(Vector2.op_Addition(position, Vector2.op_Multiply(vector2, 5000f)), 0.0f);
        SpotlightWipe.vertexBuffer[index + 3].Color = (__Null) color;
        SpotlightWipe.vertexBuffer[index + 4].Position = (__Null) new Vector3(Vector2.op_Addition(position, Vector2.op_Multiply(vector, 5000f)), 0.0f);
        SpotlightWipe.vertexBuffer[index + 4].Color = (__Null) color;
        SpotlightWipe.vertexBuffer[index + 5].Position = (__Null) new Vector3(Vector2.op_Addition(position, Vector2.op_Multiply(vector, radius)), 0.0f);
        SpotlightWipe.vertexBuffer[index + 5].Color = (__Null) color;
        SpotlightWipe.vertexBuffer[index + 6].Position = (__Null) new Vector3(Vector2.op_Addition(position, Vector2.op_Multiply(vector2, radius)), 0.0f);
        SpotlightWipe.vertexBuffer[index + 6].Color = (__Null) color;
        SpotlightWipe.vertexBuffer[index + 7].Position = (__Null) new Vector3(Vector2.op_Addition(position, Vector2.op_Multiply(vector2, radius - 2f)), 0.0f);
        SpotlightWipe.vertexBuffer[index + 7].Color = (__Null) Color.get_Transparent();
        SpotlightWipe.vertexBuffer[index + 8].Position = (__Null) new Vector3(Vector2.op_Addition(position, Vector2.op_Multiply(vector, radius - 2f)), 0.0f);
        SpotlightWipe.vertexBuffer[index + 8].Color = (__Null) Color.get_Transparent();
        SpotlightWipe.vertexBuffer[index + 9].Position = (__Null) new Vector3(Vector2.op_Addition(position, Vector2.op_Multiply(vector2, radius)), 0.0f);
        SpotlightWipe.vertexBuffer[index + 9].Color = (__Null) color;
        SpotlightWipe.vertexBuffer[index + 10].Position = (__Null) new Vector3(Vector2.op_Addition(position, Vector2.op_Multiply(vector, radius)), 0.0f);
        SpotlightWipe.vertexBuffer[index + 10].Color = (__Null) color;
        SpotlightWipe.vertexBuffer[index + 11].Position = (__Null) new Vector3(Vector2.op_Addition(position, Vector2.op_Multiply(vector, radius - 2f)), 0.0f);
        SpotlightWipe.vertexBuffer[index + 11].Color = (__Null) Color.get_Transparent();
        vector2 = vector;
      }
      ScreenWipe.DrawPrimitives(SpotlightWipe.vertexBuffer);
    }
  }
}
