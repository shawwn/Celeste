// Decompiled with JetBrains decompiler
// Type: Celeste.GlassBlockBg
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
  public class GlassBlockBg : Entity
  {
    private static readonly Color[] starColors = new Color[3]
    {
      Calc.HexToColor("7f9fba"),
      Calc.HexToColor("9bd1cd"),
      Calc.HexToColor("bacae3")
    };
    private GlassBlockBg.Star[] stars = new GlassBlockBg.Star[100];
    private GlassBlockBg.Ray[] rays = new GlassBlockBg.Ray[50];
    private VertexPositionColor[] verts = new VertexPositionColor[2700];
    private Vector2 rayNormal = new Vector2(-5f, -8f).SafeNormalize();
    private Color bgColor = Calc.HexToColor("7887de");
    private const int StarCount = 100;
    private const int RayCount = 50;
    private VirtualRenderTarget beamsTarget;
    private VirtualRenderTarget starsTarget;
    private bool hasBlocks;

    public GlassBlockBg()
    {
      this.Tag = (int) Tags.Global;
      this.Add((Component) new BeforeRenderHook(new Action(this.BeforeRender)));
      this.Depth = -9990;
      List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("particles/stars/");
      for (int index = 0; index < this.stars.Length; ++index)
      {
        this.stars[index].Position.X = (float) Calc.Random.Next(320);
        this.stars[index].Position.Y = (float) Calc.Random.Next(180);
        this.stars[index].Texture = Calc.Random.Choose<MTexture>(atlasSubtextures);
        this.stars[index].Color = Calc.Random.Choose<Color>(GlassBlockBg.starColors);
        this.stars[index].Scroll = Vector2.One * Calc.Random.NextFloat(0.05f);
      }
      for (int index = 0; index < this.rays.Length; ++index)
      {
        this.rays[index].Position.X = (float) Calc.Random.Next(320);
        this.rays[index].Position.Y = (float) Calc.Random.Next(180);
        this.rays[index].Width = Calc.Random.Range(4f, 16f);
        this.rays[index].Length = (float) Calc.Random.Choose<int>(48, 96, 128);
        this.rays[index].Color = Color.White * Calc.Random.Range(0.2f, 0.4f);
      }
    }

    private void BeforeRender()
    {
      if (!(this.hasBlocks = this.Scene.Tracker.GetEntities<GlassBlock>().Count > 0))
        return;
      Camera camera = (this.Scene as Level).Camera;
      int num1 = 320;
      int num2 = 180;
      if (this.starsTarget == null)
        this.starsTarget = VirtualContent.CreateRenderTarget("glass-block-surfaces", 320, 180, false, true, 0);
      Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) this.starsTarget);
      Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, (Effect) null, Matrix.Identity);
      Vector2 origin = new Vector2(8f, 8f);
      for (int index = 0; index < this.stars.Length; ++index)
      {
        MTexture texture = this.stars[index].Texture;
        Color color = this.stars[index].Color;
        Vector2 scroll = this.stars[index].Scroll;
        Vector2 position = new Vector2();
        position.X = this.Mod(this.stars[index].Position.X - camera.X * (1f - scroll.X), (float) num1);
        position.Y = this.Mod(this.stars[index].Position.Y - camera.Y * (1f - scroll.Y), (float) num2);
        texture.Draw(position, origin, color);
        if ((double) position.X < (double) origin.X)
          texture.Draw(position + new Vector2((float) num1, 0.0f), origin, color);
        else if ((double) position.X > (double) num1 - (double) origin.X)
          texture.Draw(position - new Vector2((float) num1, 0.0f), origin, color);
        if ((double) position.Y < (double) origin.Y)
          texture.Draw(position + new Vector2(0.0f, (float) num2), origin, color);
        else if ((double) position.Y > (double) num2 - (double) origin.Y)
          texture.Draw(position - new Vector2(0.0f, (float) num2), origin, color);
      }
      Draw.SpriteBatch.End();
      int vertex = 0;
      for (int index = 0; index < this.rays.Length; ++index)
      {
        Vector2 position = new Vector2();
        position.X = this.Mod(this.rays[index].Position.X - camera.X * 0.9f, (float) num1);
        position.Y = this.Mod(this.rays[index].Position.Y - camera.Y * 0.9f, (float) num2);
        this.DrawRay(position, ref vertex, ref this.rays[index]);
        if ((double) position.X < 64.0)
          this.DrawRay(position + new Vector2((float) num1, 0.0f), ref vertex, ref this.rays[index]);
        else if ((double) position.X > (double) (num1 - 64))
          this.DrawRay(position - new Vector2((float) num1, 0.0f), ref vertex, ref this.rays[index]);
        if ((double) position.Y < 64.0)
          this.DrawRay(position + new Vector2(0.0f, (float) num2), ref vertex, ref this.rays[index]);
        else if ((double) position.Y > (double) (num2 - 64))
          this.DrawRay(position - new Vector2(0.0f, (float) num2), ref vertex, ref this.rays[index]);
      }
      if (this.beamsTarget == null)
        this.beamsTarget = VirtualContent.CreateRenderTarget("glass-block-beams", 320, 180, false, true, 0);
      Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) this.beamsTarget);
      Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
      GFX.DrawVertices<VertexPositionColor>(Matrix.Identity, this.verts, vertex, (Effect) null, (BlendState) null);
    }

    private void DrawRay(Vector2 position, ref int vertex, ref GlassBlockBg.Ray ray)
    {
      Vector2 vector2_1 = new Vector2(-this.rayNormal.Y, this.rayNormal.X);
      Vector2 vector2_2 = this.rayNormal * ray.Width * 0.5f;
      Vector2 vector2_3 = vector2_1 * ray.Length * 0.25f * 0.5f;
      Vector2 vector2_4 = vector2_1 * ray.Length * 0.5f * 0.5f;
      Vector2 v0 = position + vector2_2 - vector2_3 - vector2_4;
      Vector2 v3 = position - vector2_2 - vector2_3 - vector2_4;
      Vector2 vector2_5 = position + vector2_2 - vector2_3;
      Vector2 vector2_6 = position - vector2_2 - vector2_3;
      Vector2 vector2_7 = position + vector2_2 + vector2_3;
      Vector2 vector2_8 = position - vector2_2 + vector2_3;
      Vector2 v1 = position + vector2_2 + vector2_3 + vector2_4;
      Vector2 v2 = position - vector2_2 + vector2_3 + vector2_4;
      Color transparent = Color.Transparent;
      Color color = ray.Color;
      this.Quad(ref vertex, v0, vector2_5, vector2_6, v3, transparent, color, color, transparent);
      this.Quad(ref vertex, vector2_5, vector2_7, vector2_8, vector2_6, color, color, color, color);
      this.Quad(ref vertex, vector2_7, v1, v2, vector2_8, color, transparent, transparent, color);
    }

    private void Quad(
      ref int vertex,
      Vector2 v0,
      Vector2 v1,
      Vector2 v2,
      Vector2 v3,
      Color c0,
      Color c1,
      Color c2,
      Color c3)
    {
      this.verts[vertex].Position.X = v0.X;
      this.verts[vertex].Position.Y = v0.Y;
      this.verts[vertex++].Color = c0;
      this.verts[vertex].Position.X = v1.X;
      this.verts[vertex].Position.Y = v1.Y;
      this.verts[vertex++].Color = c1;
      this.verts[vertex].Position.X = v2.X;
      this.verts[vertex].Position.Y = v2.Y;
      this.verts[vertex++].Color = c2;
      this.verts[vertex].Position.X = v0.X;
      this.verts[vertex].Position.Y = v0.Y;
      this.verts[vertex++].Color = c0;
      this.verts[vertex].Position.X = v2.X;
      this.verts[vertex].Position.Y = v2.Y;
      this.verts[vertex++].Color = c2;
      this.verts[vertex].Position.X = v3.X;
      this.verts[vertex].Position.Y = v3.Y;
      this.verts[vertex++].Color = c3;
    }

    public override void Render()
    {
      if (!this.hasBlocks)
        return;
      Vector2 position = (this.Scene as Level).Camera.Position;
      List<Entity> entities = this.Scene.Tracker.GetEntities<GlassBlock>();
      foreach (Entity entity in entities)
        Draw.Rect(entity.X, entity.Y, entity.Width, entity.Height, this.bgColor);
      if (this.starsTarget != null && !this.starsTarget.IsDisposed)
      {
        foreach (Entity entity in entities)
        {
          Rectangle rectangle = new Rectangle((int) ((double) entity.X - (double) position.X), (int) ((double) entity.Y - (double) position.Y), (int) entity.Width, (int) entity.Height);
          Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.starsTarget, entity.Position, new Rectangle?(rectangle), Color.White);
        }
      }
      if (this.beamsTarget == null || this.beamsTarget.IsDisposed)
        return;
      foreach (Entity entity in entities)
      {
        Rectangle rectangle = new Rectangle((int) ((double) entity.X - (double) position.X), (int) ((double) entity.Y - (double) position.Y), (int) entity.Width, (int) entity.Height);
        Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) this.beamsTarget, entity.Position, new Rectangle?(rectangle), Color.White);
      }
    }

    public override void Removed(Scene scene)
    {
      base.Removed(scene);
      this.Dispose();
    }

    public override void SceneEnd(Scene scene)
    {
      base.SceneEnd(scene);
      this.Dispose();
    }

    public void Dispose()
    {
      if (this.starsTarget != null && !this.starsTarget.IsDisposed)
        this.starsTarget.Dispose();
      if (this.beamsTarget != null && !this.beamsTarget.IsDisposed)
        this.beamsTarget.Dispose();
      this.starsTarget = (VirtualRenderTarget) null;
      this.beamsTarget = (VirtualRenderTarget) null;
    }

    private float Mod(float x, float m)
    {
      return (x % m + m) % m;
    }

    private struct Star
    {
      public Vector2 Position;
      public MTexture Texture;
      public Color Color;
      public Vector2 Scroll;
    }

    private struct Ray
    {
      public Vector2 Position;
      public float Width;
      public float Length;
      public Color Color;
    }
  }
}

