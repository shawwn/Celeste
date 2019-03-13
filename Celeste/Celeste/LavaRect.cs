// Decompiled with JetBrains decompiler
// Type: Celeste.LavaRect
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class LavaRect : Component
  {
    public float Fade = 16f;
    public float SmallWaveAmplitude = 1f;
    public float BigWaveAmplitude = 4f;
    public float CurveAmplitude = 12f;
    public float UpdateMultiplier = 1f;
    public Color SurfaceColor = Color.get_White();
    public Color EdgeColor = Color.get_LightGray();
    public Color CenterColor = Color.get_DarkGray();
    private float timer = Calc.Random.NextFloat(100f);
    public Vector2 Position;
    public float Spikey;
    public LavaRect.OnlyModes OnlyMode;
    private VertexPositionColor[] verts;
    private bool dirty;
    private int vertCount;
    private LavaRect.Bubble[] bubbles;
    private LavaRect.SurfaceBubble[] surfaceBubbles;
    private int surfaceBubbleIndex;
    private List<List<MTexture>> surfaceBubbleAnimations;

    public int SurfaceStep { get; private set; }

    public float Width { get; private set; }

    public float Height { get; private set; }

    public LavaRect(float width, float height, int step)
      : base(true, true)
    {
      this.Resize(width, height, step);
    }

    public void Resize(float width, float height, int step)
    {
      this.Width = width;
      this.Height = height;
      this.SurfaceStep = step;
      this.dirty = true;
      this.verts = new VertexPositionColor[(int) ((double) width / (double) this.SurfaceStep * 2.0 + (double) height / (double) this.SurfaceStep * 2.0 + 4.0) * 3 * 6 + 6];
      this.bubbles = new LavaRect.Bubble[(int) ((double) width * (double) height * 0.00499999988824129)];
      this.surfaceBubbles = new LavaRect.SurfaceBubble[(int) Math.Max(4f, (float) this.bubbles.Length * 0.25f)];
      for (int index = 0; index < this.bubbles.Length; ++index)
      {
        this.bubbles[index].Position = new Vector2(1f + Calc.Random.NextFloat(this.Width - 2f), Calc.Random.NextFloat(this.Height));
        this.bubbles[index].Speed = (float) Calc.Random.Range(4, 12);
        this.bubbles[index].Alpha = Calc.Random.Range(0.4f, 0.8f);
      }
      for (int index = 0; index < this.surfaceBubbles.Length; ++index)
        this.surfaceBubbles[index].X = -1f;
      this.surfaceBubbleAnimations = new List<List<MTexture>>();
      this.surfaceBubbleAnimations.Add(GFX.Game.GetAtlasSubtextures("danger/lava/bubble_a"));
    }

    public override void Update()
    {
      this.timer += this.UpdateMultiplier * Engine.DeltaTime;
      if ((double) this.UpdateMultiplier != 0.0)
        this.dirty = true;
      for (int index = 0; index < this.bubbles.Length; ++index)
      {
        ref __Null local = ref this.bubbles[index].Position.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local = ^(float&) ref local - this.UpdateMultiplier * this.bubbles[index].Speed * Engine.DeltaTime;
        if (this.bubbles[index].Position.Y < 2.0 - (double) this.Wave((int) (this.bubbles[index].Position.X / (double) this.SurfaceStep), this.Width))
        {
          this.bubbles[index].Position.Y = (__Null) ((double) this.Height - 1.0);
          if (Calc.Random.Chance(0.75f))
          {
            this.surfaceBubbles[this.surfaceBubbleIndex].X = (float) this.bubbles[index].Position.X;
            this.surfaceBubbles[this.surfaceBubbleIndex].Frame = 0.0f;
            this.surfaceBubbles[this.surfaceBubbleIndex].Animation = (byte) Calc.Random.Next(this.surfaceBubbleAnimations.Count);
            this.surfaceBubbleIndex = (this.surfaceBubbleIndex + 1) % this.surfaceBubbles.Length;
          }
        }
      }
      for (int index = 0; index < this.surfaceBubbles.Length; ++index)
      {
        if ((double) this.surfaceBubbles[index].X >= 0.0)
        {
          this.surfaceBubbles[index].Frame += Engine.DeltaTime * 6f;
          if ((double) this.surfaceBubbles[index].Frame >= (double) this.surfaceBubbleAnimations[(int) this.surfaceBubbles[index].Animation].Count)
            this.surfaceBubbles[index].X = -1f;
        }
      }
      base.Update();
    }

    private float Sin(float value)
    {
      return (float) ((1.0 + Math.Sin((double) value)) / 2.0);
    }

    private float Wave(int step, float length)
    {
      int num1 = step * this.SurfaceStep;
      float num2 = this.OnlyMode != LavaRect.OnlyModes.None ? 1f : Calc.ClampedMap((float) num1, 0.0f, length * 0.1f, 0.0f, 1f) * Calc.ClampedMap((float) num1, length * 0.9f, length, 1f, 0.0f);
      float num3 = this.Sin((float) ((double) num1 * 0.25 + (double) this.timer * 4.0)) * this.SmallWaveAmplitude + this.Sin((float) ((double) num1 * 0.0500000007450581 + (double) this.timer * 0.5)) * this.BigWaveAmplitude;
      if (step % 2 == 0)
        num3 += this.Spikey;
      if (this.OnlyMode != LavaRect.OnlyModes.None)
        num3 += (1f - Calc.YoYo((float) num1 / length)) * this.CurveAmplitude;
      return num3 * num2;
    }

    private void Quad(ref int vert, Vector2 va, Vector2 vb, Vector2 vc, Vector2 vd, Color color)
    {
      this.Quad(ref vert, va, color, vb, color, vc, color, vd, color);
    }

    private void Quad(
      ref int vert,
      Vector2 va,
      Color ca,
      Vector2 vb,
      Color cb,
      Vector2 vc,
      Color cc,
      Vector2 vd,
      Color cd)
    {
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.verts[vert].Position).X = va.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.verts[vert].Position).Y = va.Y;
      this.verts[vert++].Color = (__Null) ca;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.verts[vert].Position).X = vb.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.verts[vert].Position).Y = vb.Y;
      this.verts[vert++].Color = (__Null) cb;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.verts[vert].Position).X = vc.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.verts[vert].Position).Y = vc.Y;
      this.verts[vert++].Color = (__Null) cc;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.verts[vert].Position).X = va.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.verts[vert].Position).Y = va.Y;
      this.verts[vert++].Color = (__Null) ca;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.verts[vert].Position).X = vc.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.verts[vert].Position).Y = vc.Y;
      this.verts[vert++].Color = (__Null) cc;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.verts[vert].Position).X = vd.X;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      (^(Vector3&) ref this.verts[vert].Position).Y = vd.Y;
      this.verts[vert++].Color = (__Null) cd;
    }

    private void Edge(ref int vert, Vector2 a, Vector2 b, float fade, float insetFade)
    {
      Vector2 vector2_1 = Vector2.op_Subtraction(a, b);
      float length = ((Vector2) ref vector2_1).Length();
      float newMin = this.OnlyMode == LavaRect.OnlyModes.None ? insetFade / length : 0.0f;
      float num1 = length / (float) this.SurfaceStep;
      Vector2 vector2_2 = Vector2.op_Subtraction(b, a).SafeNormalize().Perpendicular();
      for (int step = 1; (double) step <= (double) num1; ++step)
      {
        Vector2 vector2_3 = Vector2.Lerp(a, b, (float) (step - 1) / num1);
        float num2 = this.Wave(step - 1, length);
        Vector2 vector2_4 = Vector2.op_Multiply(vector2_2, num2);
        Vector2 va = Vector2.op_Subtraction(vector2_3, vector2_4);
        Vector2 vector2_5 = Vector2.Lerp(a, b, (float) step / num1);
        float num3 = this.Wave(step, length);
        Vector2 vector2_6 = Vector2.op_Multiply(vector2_2, num3);
        Vector2 vb = Vector2.op_Subtraction(vector2_5, vector2_6);
        Vector2 vector2_7 = Vector2.Lerp(a, b, Calc.ClampedMap((float) (step - 1) / num1, 0.0f, 1f, newMin, 1f - newMin));
        Vector2 vector2_8 = Vector2.Lerp(a, b, Calc.ClampedMap((float) step / num1, 0.0f, 1f, newMin, 1f - newMin));
        this.Quad(ref vert, Vector2.op_Addition(va, vector2_2), this.EdgeColor, Vector2.op_Addition(vb, vector2_2), this.EdgeColor, Vector2.op_Addition(vector2_8, Vector2.op_Multiply(vector2_2, fade - num3)), this.CenterColor, Vector2.op_Addition(vector2_7, Vector2.op_Multiply(vector2_2, fade - num2)), this.CenterColor);
        this.Quad(ref vert, Vector2.op_Addition(vector2_7, Vector2.op_Multiply(vector2_2, fade - num2)), Vector2.op_Addition(vector2_8, Vector2.op_Multiply(vector2_2, fade - num3)), Vector2.op_Addition(vector2_8, Vector2.op_Multiply(vector2_2, fade)), Vector2.op_Addition(vector2_7, Vector2.op_Multiply(vector2_2, fade)), this.CenterColor);
        this.Quad(ref vert, va, vb, Vector2.op_Addition(vb, Vector2.op_Multiply(vector2_2, 1f)), Vector2.op_Addition(va, Vector2.op_Multiply(vector2_2, 1f)), this.SurfaceColor);
      }
    }

    public override void Render()
    {
      GameplayRenderer.End();
      if (this.dirty)
      {
        Vector2 zero = Vector2.get_Zero();
        Vector2 vector2_1 = zero;
        Vector2 vector2_2;
        ((Vector2) ref vector2_2).\u002Ector((float) zero.X + this.Width, (float) zero.Y);
        Vector2 vector2_3;
        ((Vector2) ref vector2_3).\u002Ector((float) zero.X, (float) zero.Y + this.Height);
        Vector2 vector2_4 = Vector2.op_Addition(zero, new Vector2(this.Width, this.Height));
        Vector2 vector2_5;
        ((Vector2) ref vector2_5).\u002Ector(Math.Min(this.Fade, this.Width / 2f), Math.Min(this.Fade, this.Height / 2f));
        this.vertCount = 0;
        if (this.OnlyMode == LavaRect.OnlyModes.None)
        {
          this.Edge(ref this.vertCount, vector2_1, vector2_2, (float) vector2_5.Y, (float) vector2_5.X);
          this.Edge(ref this.vertCount, vector2_2, vector2_4, (float) vector2_5.X, (float) vector2_5.Y);
          this.Edge(ref this.vertCount, vector2_4, vector2_3, (float) vector2_5.Y, (float) vector2_5.X);
          this.Edge(ref this.vertCount, vector2_3, vector2_1, (float) vector2_5.X, (float) vector2_5.Y);
          this.Quad(ref this.vertCount, Vector2.op_Addition(vector2_1, vector2_5), Vector2.op_Addition(vector2_2, new Vector2((float) -vector2_5.X, (float) vector2_5.Y)), Vector2.op_Subtraction(vector2_4, vector2_5), Vector2.op_Addition(vector2_3, new Vector2((float) vector2_5.X, (float) -vector2_5.Y)), this.CenterColor);
        }
        else if (this.OnlyMode == LavaRect.OnlyModes.OnlyTop)
        {
          this.Edge(ref this.vertCount, vector2_1, vector2_2, (float) vector2_5.Y, 0.0f);
          this.Quad(ref this.vertCount, Vector2.op_Addition(vector2_1, new Vector2(0.0f, (float) vector2_5.Y)), Vector2.op_Addition(vector2_2, new Vector2(0.0f, (float) vector2_5.Y)), vector2_4, vector2_3, this.CenterColor);
        }
        else if (this.OnlyMode == LavaRect.OnlyModes.OnlyBottom)
        {
          this.Edge(ref this.vertCount, vector2_4, vector2_3, (float) vector2_5.Y, 0.0f);
          this.Quad(ref this.vertCount, vector2_1, vector2_2, Vector2.op_Addition(vector2_4, new Vector2(0.0f, (float) -vector2_5.Y)), Vector2.op_Addition(vector2_3, new Vector2(0.0f, (float) -vector2_5.Y)), this.CenterColor);
        }
        this.dirty = false;
      }
      GFX.DrawVertices<VertexPositionColor>(Matrix.op_Multiply(Matrix.CreateTranslation(new Vector3(Vector2.op_Addition(this.Entity.Position, this.Position), 0.0f)), (this.Scene as Level).Camera.Matrix), this.verts, this.vertCount, (Effect) null, (BlendState) null);
      GameplayRenderer.Begin();
      Vector2 vector2 = Vector2.op_Addition(this.Entity.Position, this.Position);
      MTexture mtexture1 = GFX.Game["particles/bubble"];
      for (int index = 0; index < this.bubbles.Length; ++index)
        mtexture1.DrawCentered(Vector2.op_Addition(vector2, this.bubbles[index].Position), Color.op_Multiply(this.SurfaceColor, this.bubbles[index].Alpha));
      for (int index = 0; index < this.surfaceBubbles.Length; ++index)
      {
        if ((double) this.surfaceBubbles[index].X >= 0.0)
        {
          MTexture mtexture2 = this.surfaceBubbleAnimations[(int) this.surfaceBubbles[index].Animation][(int) this.surfaceBubbles[index].Frame];
          int step = (int) ((double) this.surfaceBubbles[index].X / (double) this.SurfaceStep);
          float num = 1f - this.Wave(step, this.Width);
          Vector2 position = Vector2.op_Addition(vector2, new Vector2((float) (step * this.SurfaceStep), num));
          Vector2 justify = new Vector2(0.5f, 1f);
          Color surfaceColor = this.SurfaceColor;
          mtexture2.DrawJustified(position, justify, surfaceColor);
        }
      }
    }

    public enum OnlyModes
    {
      None,
      OnlyTop,
      OnlyBottom,
    }

    private struct Bubble
    {
      public Vector2 Position;
      public float Speed;
      public float Alpha;
    }

    private struct SurfaceBubble
    {
      public float X;
      public float Frame;
      public byte Animation;
    }
  }
}
