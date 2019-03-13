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
    public bool Linear = false;
    public static Vector2 FocusPoint;
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
        focusPoint.X = 320f - focusPoint.X;
      focusPoint.X *= 6f;
      focusPoint.Y *= 6f;
      float num = 288f + SpotlightWipe.Modifier;
      float radius = this.Linear ? Ease.CubeInOut(t) * 1920f : ((double) t >= 0.200000002980232 ? ((double) t >= 0.800000011920929 ? num + (float) (((double) t - 0.800000011920929) / 0.200000002980232 * (1920.0 - (double) num)) : num) : Ease.CubeInOut(t / 0.2f) * num);
      SpotlightWipe.DrawSpotlight(focusPoint, radius, ScreenWipe.WipeColor);
    }

    public static void DrawSpotlight(Vector2 position, float radius, Color color)
    {
      Vector2 vector2 = new Vector2(1f, 0.0f);
      for (int index = 0; index < SpotlightWipe.vertexBuffer.Length; index += 12)
      {
        Vector2 vector = Calc.AngleToVector((float) (((double) index + 12.0) / (double) SpotlightWipe.vertexBuffer.Length * 6.28318548202515), 1f);
        SpotlightWipe.vertexBuffer[index].Position = new Vector3(position + vector2 * 5000f, 0.0f);
        SpotlightWipe.vertexBuffer[index].Color = color;
        SpotlightWipe.vertexBuffer[index + 1].Position = new Vector3(position + vector2 * radius, 0.0f);
        SpotlightWipe.vertexBuffer[index + 1].Color = color;
        SpotlightWipe.vertexBuffer[index + 2].Position = new Vector3(position + vector * radius, 0.0f);
        SpotlightWipe.vertexBuffer[index + 2].Color = color;
        SpotlightWipe.vertexBuffer[index + 3].Position = new Vector3(position + vector2 * 5000f, 0.0f);
        SpotlightWipe.vertexBuffer[index + 3].Color = color;
        SpotlightWipe.vertexBuffer[index + 4].Position = new Vector3(position + vector * 5000f, 0.0f);
        SpotlightWipe.vertexBuffer[index + 4].Color = color;
        SpotlightWipe.vertexBuffer[index + 5].Position = new Vector3(position + vector * radius, 0.0f);
        SpotlightWipe.vertexBuffer[index + 5].Color = color;
        SpotlightWipe.vertexBuffer[index + 6].Position = new Vector3(position + vector2 * radius, 0.0f);
        SpotlightWipe.vertexBuffer[index + 6].Color = color;
        SpotlightWipe.vertexBuffer[index + 7].Position = new Vector3(position + vector2 * (radius - 2f), 0.0f);
        SpotlightWipe.vertexBuffer[index + 7].Color = Color.Transparent;
        SpotlightWipe.vertexBuffer[index + 8].Position = new Vector3(position + vector * (radius - 2f), 0.0f);
        SpotlightWipe.vertexBuffer[index + 8].Color = Color.Transparent;
        SpotlightWipe.vertexBuffer[index + 9].Position = new Vector3(position + vector2 * radius, 0.0f);
        SpotlightWipe.vertexBuffer[index + 9].Color = color;
        SpotlightWipe.vertexBuffer[index + 10].Position = new Vector3(position + vector * radius, 0.0f);
        SpotlightWipe.vertexBuffer[index + 10].Color = color;
        SpotlightWipe.vertexBuffer[index + 11].Position = new Vector3(position + vector * (radius - 2f), 0.0f);
        SpotlightWipe.vertexBuffer[index + 11].Color = Color.Transparent;
        vector2 = vector;
      }
      ScreenWipe.DrawPrimitives(SpotlightWipe.vertexBuffer);
    }
  }
}

