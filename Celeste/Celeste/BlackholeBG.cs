// Decompiled with JetBrains decompiler
// Type: Celeste.BlackholeBG
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class BlackholeBG : Backdrop
  {
    private const string STRENGTH_FLAG = "blackhole_strength";
    private const int BG_STEPS = 20;
    private const int STREAM_MIN_COUNT = 30;
    private const int STREAM_MAX_COUNT = 50;
    private const int PARTICLE_MIN_COUNT = 150;
    private const int PARTICLE_MAX_COUNT = 220;
    private const int SPIRAL_MIN_COUNT = 0;
    private const int SPIRAL_MAX_COUNT = 10;
    private const int SPIRAL_SEGMENTS = 12;
    private Color[] colorsMild = new Color[3]
    {
      Calc.HexToColor("6e3199") * 0.8f,
      Calc.HexToColor("851f91") * 0.8f,
      Calc.HexToColor("3026b0") * 0.8f
    };
    private Color[] colorsWild = new Color[3]
    {
      Calc.HexToColor("ca4ca7"),
      Calc.HexToColor("b14cca"),
      Calc.HexToColor("ca4ca7")
    };
    private Color[] colorsLerp;
    private Color[,] colorsLerpBlack;
    private Color[,] colorsLerpTransparent;
    private const int colorSteps = 20;
    public float Alpha = 1f;
    public float Scale = 1f;
    public float Direction = 1f;
    public float StrengthMultiplier = 1f;
    public Vector2 CenterOffset;
    public Vector2 OffsetOffset;
    private BlackholeBG.Strengths strength;
    private readonly Color bgColorInner = Calc.HexToColor("000000");
    private readonly Color bgColorOuterMild = Calc.HexToColor("512a8b");
    private readonly Color bgColorOuterWild = Calc.HexToColor("bd2192");
    private readonly MTexture bgTexture;
    private BlackholeBG.StreamParticle[] streams = new BlackholeBG.StreamParticle[50];
    private VertexPositionColorTexture[] streamVerts = new VertexPositionColorTexture[300];
    private BlackholeBG.Particle[] particles = new BlackholeBG.Particle[220];
    private BlackholeBG.SpiralDebris[] spirals = new BlackholeBG.SpiralDebris[10];
    private VertexPositionColorTexture[] spiralVerts = new VertexPositionColorTexture[720];
    private VirtualRenderTarget buffer;
    private Vector2 center;
    private Vector2 offset;
    private Vector2 shake;
    private float spinTime;
    private bool checkedFlag;

    public BlackholeBG()
    {
      this.bgTexture = GFX.Game["objects/temple/portal/portal"];
      List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("bgs/10/blackhole/particle");
      int index1 = 0;
      for (int index2 = 0; index2 < 50; ++index2)
      {
        MTexture mtexture = this.streams[index2].Texture = Calc.Random.Choose<MTexture>(atlasSubtextures);
        this.streams[index2].Percent = Calc.Random.NextFloat();
        this.streams[index2].Speed = Calc.Random.Range(0.2f, 0.4f);
        this.streams[index2].Normal = Calc.AngleToVector(Calc.Random.NextFloat() * 6.2831855f, 1f);
        this.streams[index2].Color = Calc.Random.Next(this.colorsMild.Length);
        this.streamVerts[index1].TextureCoordinate = new Vector2(mtexture.LeftUV, mtexture.TopUV);
        this.streamVerts[index1 + 1].TextureCoordinate = new Vector2(mtexture.RightUV, mtexture.TopUV);
        this.streamVerts[index1 + 2].TextureCoordinate = new Vector2(mtexture.RightUV, mtexture.BottomUV);
        this.streamVerts[index1 + 3].TextureCoordinate = new Vector2(mtexture.LeftUV, mtexture.TopUV);
        this.streamVerts[index1 + 4].TextureCoordinate = new Vector2(mtexture.RightUV, mtexture.BottomUV);
        this.streamVerts[index1 + 5].TextureCoordinate = new Vector2(mtexture.LeftUV, mtexture.BottomUV);
        index1 += 6;
      }
      int index3 = 0;
      for (int index4 = 0; index4 < 10; ++index4)
      {
        MTexture mtexture = this.streams[index4].Texture = Calc.Random.Choose<MTexture>(atlasSubtextures);
        this.spirals[index4].Percent = Calc.Random.NextFloat();
        this.spirals[index4].Offset = Calc.Random.NextFloat(6.2831855f);
        this.spirals[index4].Color = Calc.Random.Next(this.colorsMild.Length);
        for (int index5 = 0; index5 < 12; ++index5)
        {
          float x1 = MathHelper.Lerp(mtexture.LeftUV, mtexture.RightUV, (float) index5 / 12f);
          float x2 = MathHelper.Lerp(mtexture.LeftUV, mtexture.RightUV, (float) (index5 + 1) / 12f);
          this.spiralVerts[index3].TextureCoordinate = new Vector2(x1, mtexture.TopUV);
          this.spiralVerts[index3 + 1].TextureCoordinate = new Vector2(x2, mtexture.TopUV);
          this.spiralVerts[index3 + 2].TextureCoordinate = new Vector2(x2, mtexture.BottomUV);
          this.spiralVerts[index3 + 3].TextureCoordinate = new Vector2(x1, mtexture.TopUV);
          this.spiralVerts[index3 + 4].TextureCoordinate = new Vector2(x2, mtexture.BottomUV);
          this.spiralVerts[index3 + 5].TextureCoordinate = new Vector2(x1, mtexture.BottomUV);
          index3 += 6;
        }
      }
      for (int index6 = 0; index6 < 220; ++index6)
      {
        this.particles[index6].Percent = Calc.Random.NextFloat();
        this.particles[index6].Normal = Calc.AngleToVector(Calc.Random.NextFloat() * 6.2831855f, 1f);
        this.particles[index6].Color = Calc.Random.Next(this.colorsMild.Length);
      }
      this.center = new Vector2(320f, 180f) / 2f;
      this.offset = Vector2.Zero;
      this.colorsLerp = new Color[this.colorsMild.Length];
      this.colorsLerpBlack = new Color[this.colorsMild.Length, 20];
      this.colorsLerpTransparent = new Color[this.colorsMild.Length, 20];
    }

    public void SnapStrength(Level level, BlackholeBG.Strengths strength)
    {
      this.strength = strength;
      this.StrengthMultiplier = 1f + (float) strength;
      level.Session.SetCounter("blackhole_strength", (int) strength);
    }

    public void NextStrength(Level level, BlackholeBG.Strengths strength)
    {
      this.strength = strength;
      level.Session.SetCounter("blackhole_strength", (int) strength);
    }

    public int StreamCount => (int) MathHelper.Lerp(30f, 50f, (float) (((double) this.StrengthMultiplier - 1.0) / 3.0));

    public int ParticleCount => (int) MathHelper.Lerp(150f, 220f, (float) (((double) this.StrengthMultiplier - 1.0) / 3.0));

    public int SpiralCount => (int) MathHelper.Lerp(0.0f, 10f, (float) (((double) this.StrengthMultiplier - 1.0) / 3.0));

    public override void Update(Scene scene)
    {
      base.Update(scene);
      if (!this.checkedFlag)
      {
        int counter = (scene as Level).Session.GetCounter("blackhole_strength");
        if (counter >= 0)
          this.SnapStrength(scene as Level, (BlackholeBG.Strengths) counter);
        this.checkedFlag = true;
      }
      if (!this.Visible)
        return;
      this.StrengthMultiplier = Calc.Approach(this.StrengthMultiplier, 1f + (float) this.strength, Engine.DeltaTime * 0.1f);
      if (scene.OnInterval(0.05f))
      {
        for (int index1 = 0; index1 < this.colorsMild.Length; ++index1)
        {
          this.colorsLerp[index1] = Color.Lerp(this.colorsMild[index1], this.colorsWild[index1], (float) (((double) this.StrengthMultiplier - 1.0) / 3.0));
          for (int index2 = 0; index2 < 20; ++index2)
          {
            this.colorsLerpBlack[index1, index2] = Color.Lerp(this.colorsLerp[index1], Color.Black, (float) index2 / 19f) * this.FadeAlphaMultiplier;
            this.colorsLerpTransparent[index1, index2] = Color.Lerp(this.colorsLerp[index1], Color.Transparent, (float) index2 / 19f) * this.FadeAlphaMultiplier;
          }
        }
      }
      float num1 = (float) (1.0 + ((double) this.StrengthMultiplier - 1.0) * 0.699999988079071);
      int streamCount = this.StreamCount;
      int v1 = 0;
      for (int index = 0; index < streamCount; ++index)
      {
        this.streams[index].Percent += this.streams[index].Speed * Engine.DeltaTime * num1 * this.Direction;
        if ((double) this.streams[index].Percent >= 1.0 && (double) this.Direction > 0.0)
        {
          this.streams[index].Normal = Calc.AngleToVector(Calc.Random.NextFloat() * 6.2831855f, 1f);
          --this.streams[index].Percent;
        }
        else if ((double) this.streams[index].Percent < 0.0 && (double) this.Direction < 0.0)
        {
          this.streams[index].Normal = Calc.AngleToVector(Calc.Random.NextFloat() * 6.2831855f, 1f);
          ++this.streams[index].Percent;
        }
        float percent = this.streams[index].Percent;
        float num2 = Ease.CubeIn(Calc.ClampedMap(percent, 0.0f, 0.8f));
        float num3 = Ease.CubeIn(Calc.ClampedMap(percent, 0.2f, 1f));
        Vector2 normal = this.streams[index].Normal;
        Vector2 vector2_1 = normal.Perpendicular();
        Vector2 vector2_2 = normal * 16f + normal * (1f - num2) * 200f;
        float num4 = (float) ((1.0 - (double) num2) * 8.0);
        Color color1 = this.colorsLerpBlack[this.streams[index].Color, (int) ((double) num2 * 0.6000000238418579 * 19.0)];
        Vector2 vector2_3 = normal * 16f + normal * (1f - num3) * 280f;
        float num5 = (float) ((1.0 - (double) num3) * 8.0);
        Color color2 = this.colorsLerpBlack[this.streams[index].Color, (int) ((double) num3 * 0.6000000238418579 * 19.0)];
        Vector2 a = vector2_2 - vector2_1 * num4;
        Vector2 b = vector2_2 + vector2_1 * num4;
        Vector2 c = vector2_3 + vector2_1 * num5;
        Vector2 d = vector2_3 - vector2_1 * num5;
        this.AssignVertColors(this.streamVerts, v1, ref color1, ref color1, ref color2, ref color2);
        this.AssignVertPosition(this.streamVerts, v1, ref a, ref b, ref c, ref d);
        v1 += 6;
      }
      float num6 = this.StrengthMultiplier * 0.25f;
      int particleCount = this.ParticleCount;
      for (int index = 0; index < particleCount; ++index)
      {
        this.particles[index].Percent += Engine.DeltaTime * num6 * this.Direction;
        if ((double) this.particles[index].Percent >= 1.0 && (double) this.Direction > 0.0)
        {
          this.particles[index].Normal = Calc.AngleToVector(Calc.Random.NextFloat() * 6.2831855f, 1f);
          --this.particles[index].Percent;
        }
        else if ((double) this.particles[index].Percent < 0.0 && (double) this.Direction < 0.0)
        {
          this.particles[index].Normal = Calc.AngleToVector(Calc.Random.NextFloat() * 6.2831855f, 1f);
          ++this.particles[index].Percent;
        }
      }
      float num7 = (float) (0.20000000298023224 + ((double) this.StrengthMultiplier - 1.0) * 0.10000000149011612);
      int spiralCount = this.SpiralCount;
      Color color3 = Color.Lerp(Color.Lerp(this.bgColorOuterMild, this.bgColorOuterWild, (float) (((double) this.StrengthMultiplier - 1.0) / 3.0)), Color.White, 0.1f) * 0.8f;
      int v2 = 0;
      for (int index3 = 0; index3 < spiralCount; ++index3)
      {
        this.spirals[index3].Percent += this.streams[index3].Speed * Engine.DeltaTime * num7 * this.Direction;
        if ((double) this.spirals[index3].Percent >= 1.0 && (double) this.Direction > 0.0)
        {
          this.spirals[index3].Offset = Calc.Random.NextFloat(6.2831855f);
          --this.spirals[index3].Percent;
        }
        else if ((double) this.spirals[index3].Percent < 0.0 && (double) this.Direction < 0.0)
        {
          this.spirals[index3].Offset = Calc.Random.NextFloat(6.2831855f);
          ++this.spirals[index3].Percent;
        }
        double percent = (double) this.spirals[index3].Percent;
        float offset = this.spirals[index3].Offset;
        float num8 = Calc.ClampedMap((float) percent, 0.0f, 0.8f);
        float num9 = Calc.ClampedMap((float) percent, 0.0f, 1f);
        for (int index4 = 0; index4 < 12; ++index4)
        {
          float num10 = 1f - MathHelper.Lerp(num8, num9, (float) index4 / 12f);
          float num11 = 1f - MathHelper.Lerp(num8, num9, (float) (index4 + 1) / 12f);
          Vector2 vector1 = Calc.AngleToVector(num10 * (float) (20.0 + (double) index4 * 0.20000000298023224) + offset, 1f);
          Vector2 vector2_4 = vector1 * num10 * 200f;
          float num12 = num10 * (float) (4.0 + (double) this.StrengthMultiplier * 4.0);
          Vector2 vector2 = Calc.AngleToVector(num11 * (float) (20.0 + (double) (index4 + 1) * 0.20000000298023224) + offset, 1f);
          Vector2 vector2_5 = vector2 * num11 * 200f;
          float num13 = num11 * (float) (4.0 + (double) this.StrengthMultiplier * 4.0);
          Color color4 = Color.Lerp(color3, Color.Black, (float) ((1.0 - (double) num10) * 0.5));
          Color color5 = Color.Lerp(color3, Color.Black, (float) ((1.0 - (double) num11) * 0.5));
          Vector2 a = vector2_4 + vector1 * num12;
          Vector2 b = vector2_5 + vector2 * num13;
          Vector2 c = vector2_5 - vector2 * num13;
          Vector2 d = vector2_4 - vector1 * num12;
          this.AssignVertColors(this.spiralVerts, v2, ref color4, ref color5, ref color5, ref color4);
          this.AssignVertPosition(this.spiralVerts, v2, ref a, ref b, ref c, ref d);
          v2 += 6;
        }
      }
      Vector2 wind = (scene as Level).Wind;
      this.center += (new Vector2(320f, 180f) / 2f + wind * 0.15f + this.CenterOffset - this.center) * (1f - (float) Math.Pow(0.009999999776482582, (double) Engine.DeltaTime));
      this.offset += (-wind * 0.25f + this.OffsetOffset - this.offset) * (1f - (float) Math.Pow(0.009999999776482582, (double) Engine.DeltaTime));
      if (scene.OnInterval(0.025f))
        this.shake = Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), (float) (2.0 * ((double) this.StrengthMultiplier - 1.0)));
      this.spinTime += (2f + this.StrengthMultiplier) * Engine.DeltaTime;
    }

    private void AssignVertColors(
      VertexPositionColorTexture[] verts,
      int v,
      ref Color a,
      ref Color b,
      ref Color c,
      ref Color d)
    {
      verts[v].Color = a;
      verts[v + 1].Color = b;
      verts[v + 2].Color = c;
      verts[v + 3].Color = a;
      verts[v + 4].Color = c;
      verts[v + 5].Color = d;
    }

    private void AssignVertPosition(
      VertexPositionColorTexture[] verts,
      int v,
      ref Vector2 a,
      ref Vector2 b,
      ref Vector2 c,
      ref Vector2 d)
    {
      verts[v].Position = new Vector3(a, 0.0f);
      verts[v + 1].Position = new Vector3(b, 0.0f);
      verts[v + 2].Position = new Vector3(c, 0.0f);
      verts[v + 3].Position = new Vector3(a, 0.0f);
      verts[v + 4].Position = new Vector3(c, 0.0f);
      verts[v + 5].Position = new Vector3(d, 0.0f);
    }

    public override void BeforeRender(Scene scene)
    {
      if (this.buffer == null || this.buffer.IsDisposed)
        this.buffer = VirtualContent.CreateRenderTarget("Black Hole", 320, 180);
      Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) this.buffer);
      Engine.Graphics.GraphicsDevice.Clear(this.bgColorInner);
      Draw.SpriteBatch.Begin();
      Color color1 = Color.Lerp(this.bgColorOuterMild, this.bgColorOuterWild, (float) (((double) this.StrengthMultiplier - 1.0) / 3.0));
      for (int index = 0; index < 20; ++index)
      {
        float num = (float) ((1.0 - (double) this.spinTime % 1.0) * 0.05000000074505806 + (double) index / 20.0);
        Color color2 = Color.Lerp(this.bgColorInner, color1, Ease.SineOut(num));
        float scale = Calc.ClampedMap(num, 0.0f, 1f, 0.1f, 4f);
        float rotation = 6.2831855f * num;
        this.bgTexture.DrawCentered(this.center + this.offset * num + this.shake * (1f - num), color2, scale, rotation);
      }
      Draw.SpriteBatch.End();
      if (this.SpiralCount > 0)
      {
        Engine.Instance.GraphicsDevice.Textures[0] = (Texture) GFX.Game.Sources[0].Texture;
        GFX.DrawVertices<VertexPositionColorTexture>(Matrix.CreateTranslation(this.center.X, this.center.Y, 0.0f), this.spiralVerts, this.SpiralCount * 12 * 6, GFX.FxTexture);
      }
      if (this.StreamCount > 0)
      {
        Engine.Instance.GraphicsDevice.Textures[0] = (Texture) GFX.Game.Sources[0].Texture;
        GFX.DrawVertices<VertexPositionColorTexture>(Matrix.CreateTranslation(this.center.X, this.center.Y, 0.0f), this.streamVerts, this.StreamCount * 6, GFX.FxTexture);
      }
      Draw.SpriteBatch.Begin();
      int particleCount = this.ParticleCount;
      for (int index = 0; index < particleCount; ++index)
      {
        float val = Ease.CubeIn(Calc.Clamp(this.particles[index].Percent, 0.0f, 1f));
        Vector2 vector2_1 = this.center + this.particles[index].Normal * Calc.ClampedMap(val, 1f, 0.0f, 8f, 220f);
        Color color3 = this.colorsLerpTransparent[this.particles[index].Color, (int) ((double) val * 19.0)];
        float num = (float) (1.0 + (1.0 - (double) val) * 1.5);
        Vector2 vector2_2 = new Vector2(num, num) / 2f;
        Draw.Rect(vector2_1 - vector2_2, num, num, color3);
      }
      Draw.SpriteBatch.End();
    }

    public override void Ended(Scene scene)
    {
      if (this.buffer == null)
        return;
      this.buffer.Dispose();
      this.buffer = (VirtualRenderTarget) null;
    }

    public override void Render(Scene scene)
    {
      if (this.buffer == null || this.buffer.IsDisposed)
        return;
      Vector2 vector2 = new Vector2((float) this.buffer.Width, (float) this.buffer.Height) / 2f;
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.buffer, vector2, new Rectangle?(this.buffer.Bounds), Color.White * this.FadeAlphaMultiplier * this.Alpha, 0.0f, vector2, this.Scale, SpriteEffects.None, 0.0f);
    }

    public enum Strengths
    {
      Mild,
      Medium,
      High,
      Wild,
    }

    private struct StreamParticle
    {
      public int Color;
      public MTexture Texture;
      public float Percent;
      public float Speed;
      public Vector2 Normal;
    }

    private struct Particle
    {
      public int Color;
      public Vector2 Normal;
      public float Percent;
    }

    private struct SpiralDebris
    {
      public int Color;
      public float Percent;
      public float Offset;
    }
  }
}
