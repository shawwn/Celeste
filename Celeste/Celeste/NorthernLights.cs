// Decompiled with JetBrains decompiler
// Type: Celeste.NorthernLights
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
  public class NorthernLights : Backdrop
  {
    private static readonly Color[] colors = new Color[4]
    {
      Calc.HexToColor("2de079"),
      Calc.HexToColor("62f4f6"),
      Calc.HexToColor("45bc2e"),
      Calc.HexToColor("3856f0")
    };
    private List<NorthernLights.Strand> strands = new List<NorthernLights.Strand>();
    private NorthernLights.Particle[] particles = new NorthernLights.Particle[50];
    private VertexPositionColorTexture[] verts = new VertexPositionColorTexture[1024];
    private VertexPositionColor[] gradient = new VertexPositionColor[6];
    public float OffsetY = 0.0f;
    public float NorthernLightsAlpha = 1f;
    private VirtualRenderTarget buffer;
    private float timer;

    public NorthernLights()
    {
      for (int index = 0; index < 3; ++index)
        this.strands.Add(new NorthernLights.Strand());
      for (int index = 0; index < this.particles.Length; ++index)
      {
        this.particles[index].Position = new Vector2((float) Calc.Random.Range(0, 320), (float) Calc.Random.Range(0, 180));
        this.particles[index].Speed = (float) Calc.Random.Range(4, 14);
        this.particles[index].Color = Calc.Random.Choose<Color>(NorthernLights.colors);
      }
      Color color1 = Calc.HexToColor("020825");
      Color color2 = Calc.HexToColor("170c2f");
      this.gradient[0] = new VertexPositionColor(new Vector3(0.0f, 0.0f, 0.0f), color1);
      this.gradient[1] = new VertexPositionColor(new Vector3(320f, 0.0f, 0.0f), color1);
      this.gradient[2] = new VertexPositionColor(new Vector3(320f, 180f, 0.0f), color2);
      this.gradient[3] = new VertexPositionColor(new Vector3(0.0f, 0.0f, 0.0f), color1);
      this.gradient[4] = new VertexPositionColor(new Vector3(320f, 180f, 0.0f), color2);
      this.gradient[5] = new VertexPositionColor(new Vector3(0.0f, 180f, 0.0f), color2);
    }

    public override void Update(Scene scene)
    {
      if (this.Visible)
      {
        this.timer += Engine.DeltaTime;
        foreach (NorthernLights.Strand strand in this.strands)
        {
          strand.Percent += Engine.DeltaTime / strand.Duration;
          strand.Alpha = Calc.Approach(strand.Alpha, (double) strand.Percent < 1.0 ? 1f : 0.0f, Engine.DeltaTime);
          if ((double) strand.Alpha <= 0.0 && (double) strand.Percent >= 1.0)
            strand.Reset(0.0f);
          foreach (NorthernLights.Node node in strand.Nodes)
            node.SineOffset += Engine.DeltaTime;
        }
        for (int index = 0; index < this.particles.Length; ++index)
          this.particles[index].Position.Y += this.particles[index].Speed * Engine.DeltaTime;
      }
      base.Update(scene);
    }

    public override void BeforeRender(Scene scene)
    {
      if (this.buffer == null)
        this.buffer = VirtualContent.CreateRenderTarget("northern-lights", 320, 180, false, true, 0);
      int vert = 0;
      foreach (NorthernLights.Strand strand in this.strands)
      {
        NorthernLights.Node node1 = strand.Nodes[0];
        for (int index = 1; index < strand.Nodes.Count; ++index)
        {
          NorthernLights.Node node2 = strand.Nodes[index];
          float num1 = Math.Min(1f, (float) index / 4f) * this.NorthernLightsAlpha;
          float num2 = Math.Min(1f, (float) (strand.Nodes.Count - index) / 4f) * this.NorthernLightsAlpha;
          float num3 = this.OffsetY + (float) Math.Sin((double) node1.SineOffset) * 3f;
          float num4 = this.OffsetY + (float) Math.Sin((double) node2.SineOffset) * 3f;
          this.Set(ref vert, node1.Position.X, node1.Position.Y + num3, node1.TextureOffset, 1f, node1.Color * (node1.BottomAlpha * strand.Alpha * num1));
          this.Set(ref vert, node1.Position.X, node1.Position.Y - node1.Height + num3, node1.TextureOffset, 0.05f, node1.Color * (node1.TopAlpha * strand.Alpha * num1));
          this.Set(ref vert, node2.Position.X, node2.Position.Y - node2.Height + num4, node2.TextureOffset, 0.05f, node2.Color * (node2.TopAlpha * strand.Alpha * num2));
          this.Set(ref vert, node1.Position.X, node1.Position.Y + num3, node1.TextureOffset, 1f, node1.Color * (node1.BottomAlpha * strand.Alpha * num1));
          this.Set(ref vert, node2.Position.X, node2.Position.Y - node2.Height + num4, node2.TextureOffset, 0.05f, node2.Color * (node2.TopAlpha * strand.Alpha * num2));
          this.Set(ref vert, node2.Position.X, node2.Position.Y + num4, node2.TextureOffset, 1f, node2.Color * (node2.BottomAlpha * strand.Alpha * num2));
          node1 = node2;
        }
      }
      Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) this.buffer);
      GFX.DrawVertices<VertexPositionColor>(Matrix.Identity, this.gradient, this.gradient.Length, (Effect) null, (BlendState) null);
      Engine.Graphics.GraphicsDevice.Textures[0] = (Texture) GFX.Misc["northernlights"].Texture.Texture;
      Engine.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
      GFX.DrawVertices<VertexPositionColorTexture>(Matrix.Identity, this.verts, vert, GFX.FxTexture, (BlendState) null);
      bool clear = false;
      GaussianBlur.Blur((Texture2D) (RenderTarget2D) this.buffer, GameplayBuffers.TempA, this.buffer, 0.0f, clear, GaussianBlur.Samples.Five, 0.25f, GaussianBlur.Direction.Vertical, 1f);
      Draw.SpriteBatch.Begin();
      Camera camera = (scene as Level).Camera;
      for (int index = 0; index < this.particles.Length; ++index)
        Draw.Rect(new Vector2()
        {
          X = this.mod(this.particles[index].Position.X - camera.X * 0.2f, 320f),
          Y = this.mod(this.particles[index].Position.Y - camera.Y * 0.2f, 180f)
        }, 1f, 1f, this.particles[index].Color);
      Draw.SpriteBatch.End();
    }

    public override void Ended(Scene scene)
    {
      if (this.buffer != null)
        this.buffer.Dispose();
      this.buffer = (VirtualRenderTarget) null;
      base.Ended(scene);
    }

    private void Set(ref int vert, float px, float py, float tx, float ty, Color color)
    {
      this.verts[vert].Color = color;
      this.verts[vert].Position.X = px;
      this.verts[vert].Position.Y = py;
      this.verts[vert].TextureCoordinate.X = tx;
      this.verts[vert].TextureCoordinate.Y = ty;
      ++vert;
    }

    public override void Render(Scene scene)
    {
      Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.buffer, Vector2.Zero, Color.White);
    }

    private float mod(float x, float m)
    {
      return (x % m + m) % m;
    }

    private class Strand
    {
      public List<NorthernLights.Node> Nodes = new List<NorthernLights.Node>();
      public float Duration;
      public float Percent;
      public float Alpha;

      public Strand()
      {
        this.Reset(Calc.Random.NextFloat());
      }

      public void Reset(float startPercent)
      {
        this.Percent = startPercent;
        this.Duration = Calc.Random.Range(12f, 32f);
        this.Alpha = 0.0f;
        this.Nodes.Clear();
        Vector2 vector2 = new Vector2((float) Calc.Random.Range(-40, 60), (float) Calc.Random.Range(40, 90));
        float num = Calc.Random.NextFloat();
        Color color = Calc.Random.Choose<Color>(NorthernLights.colors);
        for (int index = 0; index < 40; ++index)
        {
          NorthernLights.Node node = new NorthernLights.Node()
          {
            Position = vector2,
            TextureOffset = num,
            Height = (float) Calc.Random.Range(10, 80),
            TopAlpha = Calc.Random.Range(0.3f, 0.8f),
            BottomAlpha = Calc.Random.Range(0.5f, 1f),
            SineOffset = Calc.Random.NextFloat() * 6.283185f,
            Color = Color.Lerp(color, Calc.Random.Choose<Color>(NorthernLights.colors), Calc.Random.Range(0.0f, 0.3f))
          };
          num += Calc.Random.Range(0.02f, 0.2f);
          vector2 += new Vector2((float) Calc.Random.Range(4, 20), (float) Calc.Random.Range(-15, 15));
          this.Nodes.Add(node);
        }
      }
    }

    private class Node
    {
      public Vector2 Position;
      public float TextureOffset;
      public float Height;
      public float TopAlpha;
      public float BottomAlpha;
      public float SineOffset;
      public Color Color;
    }

    private struct Particle
    {
      public Vector2 Position;
      public float Speed;
      public Color Color;
    }
  }
}

