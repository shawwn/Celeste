// Decompiled with JetBrains decompiler
// Type: Celeste.LightningRenderer
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(false)]
  public class LightningRenderer : Entity
  {
    private List<Lightning> list = new List<Lightning>();
    private List<LightningRenderer.Edge> edges = new List<LightningRenderer.Edge>();
    private List<LightningRenderer.Bolt> bolts = new List<LightningRenderer.Bolt>();
    private VertexPositionColor[] edgeVerts;
    private VirtualMap<bool> tiles;
    private Rectangle levelTileBounds;
    private uint edgeSeed;
    private uint leapSeed;
    private bool dirty;
    private Color[] electricityColors = new Color[2]
    {
      Calc.HexToColor("fcf579"),
      Calc.HexToColor("8cf7e2")
    };
    private Color[] electricityColorsLerped;
    public float Fade;
    public bool UpdateSeeds = true;
    public const int BoltBufferSize = 160;
    public bool DrawEdges = true;
    public SoundSource AmbientSfx;

    public LightningRenderer()
    {
      this.Tag = (int) Tags.Global | (int) Tags.TransitionUpdate;
      this.Depth = -1000100;
      this.electricityColorsLerped = new Color[this.electricityColors.Length];
      this.Add((Component) new CustomBloom(new Action(this.OnRenderBloom)));
      this.Add((Component) new BeforeRenderHook(new Action(this.OnBeforeRender)));
      this.Add((Component) (this.AmbientSfx = new SoundSource()));
      this.AmbientSfx.DisposeOnTransition = false;
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      for (int index = 0; index < 4; ++index)
      {
        this.bolts.Add(new LightningRenderer.Bolt(this.electricityColors[0], 1f, 160, 160));
        this.bolts.Add(new LightningRenderer.Bolt(this.electricityColors[1], 0.35f, 160, 160));
      }
    }

    public void StartAmbience()
    {
      if (this.AmbientSfx.Playing)
        return;
      this.AmbientSfx.Play("event:/new_content/env/10_electricity");
    }

    public void StopAmbience() => this.AmbientSfx.Stop();

    public void Reset()
    {
      this.UpdateSeeds = true;
      this.Fade = 0.0f;
    }

    public void Track(Lightning block)
    {
      this.list.Add(block);
      if (this.tiles == null)
      {
        this.levelTileBounds = (this.Scene as Level).TileBounds;
        this.tiles = new VirtualMap<bool>(this.levelTileBounds.Width, this.levelTileBounds.Height);
      }
      for (int index1 = (int) block.X / 8; index1 < ((int) block.X + block.VisualWidth) / 8; ++index1)
      {
        for (int index2 = (int) block.Y / 8; index2 < ((int) block.Y + block.VisualHeight) / 8; ++index2)
          this.tiles[index1 - this.levelTileBounds.X, index2 - this.levelTileBounds.Y] = true;
      }
      this.dirty = true;
    }

    public void Untrack(Lightning block)
    {
      this.list.Remove(block);
      if (this.list.Count <= 0)
      {
        this.tiles = (VirtualMap<bool>) null;
      }
      else
      {
        for (int index1 = (int) block.X / 8; (double) index1 < (double) block.Right / 8.0; ++index1)
        {
          for (int index2 = (int) block.Y / 8; (double) index2 < (double) block.Bottom / 8.0; ++index2)
            this.tiles[index1 - this.levelTileBounds.X, index2 - this.levelTileBounds.Y] = false;
        }
      }
      this.dirty = true;
    }

    public override void Update()
    {
      if (this.dirty)
        this.RebuildEdges();
      this.ToggleEdges();
      if (this.list.Count <= 0)
        return;
      foreach (LightningRenderer.Bolt bolt in this.bolts)
        bolt.Update(this.Scene);
      if (!this.UpdateSeeds)
        return;
      if (this.Scene.OnInterval(0.1f))
        this.edgeSeed = (uint) Calc.Random.Next();
      if (!this.Scene.OnInterval(0.7f))
        return;
      this.leapSeed = (uint) Calc.Random.Next();
    }

    public void ToggleEdges(bool immediate = false)
    {
      Camera camera = (this.Scene as Level).Camera;
      Rectangle view = new Rectangle((int) camera.Left - 4, (int) camera.Top - 4, (int) ((double) camera.Right - (double) camera.Left) + 8, (int) ((double) camera.Bottom - (double) camera.Top) + 8);
      for (int index = 0; index < this.edges.Count; ++index)
      {
        if (immediate)
          this.edges[index].Visible = this.edges[index].InView(ref view);
        else if (!this.edges[index].Visible && this.Scene.OnInterval(0.05f, (float) index * 0.01f) && this.edges[index].InView(ref view))
          this.edges[index].Visible = true;
        else if (this.edges[index].Visible && this.Scene.OnInterval(0.25f, (float) index * 0.01f) && !this.edges[index].InView(ref view))
          this.edges[index].Visible = false;
      }
    }

    private void RebuildEdges()
    {
      this.dirty = false;
      this.edges.Clear();
      if (this.list.Count <= 0)
        return;
      Level scene = this.Scene as Level;
      int left = scene.TileBounds.Left;
      Rectangle tileBounds = scene.TileBounds;
      int top = tileBounds.Top;
      tileBounds = scene.TileBounds;
      int right = tileBounds.Right;
      tileBounds = scene.TileBounds;
      int bottom = tileBounds.Bottom;
      Point[] pointArray = new Point[4]
      {
        new Point(0, -1),
        new Point(0, 1),
        new Point(-1, 0),
        new Point(1, 0)
      };
      foreach (Lightning parent in this.list)
      {
        for (int x = (int) parent.X / 8; (double) x < (double) parent.Right / 8.0; ++x)
        {
          for (int y = (int) parent.Y / 8; (double) y < (double) parent.Bottom / 8.0; ++y)
          {
            foreach (Point point1 in pointArray)
            {
              Point point2 = new Point(-point1.Y, point1.X);
              if (!this.Inside(x + point1.X, y + point1.Y) && (!this.Inside(x - point2.X, y - point2.Y) || this.Inside(x + point1.X - point2.X, y + point1.Y - point2.Y)))
              {
                Point point3 = new Point(x, y);
                Point point4 = new Point(x + point2.X, y + point2.Y);
                Vector2 vector2 = new Vector2(4f) + new Vector2((float) (point1.X - point2.X), (float) (point1.Y - point2.Y)) * 4f;
                int num = 1;
                while (this.Inside(point4.X, point4.Y) && !this.Inside(point4.X + point1.X, point4.Y + point1.Y))
                {
                  point4.X += point2.X;
                  point4.Y += point2.Y;
                  ++num;
                  if (num > 8)
                  {
                    Vector2 a = new Vector2((float) point3.X, (float) point3.Y) * 8f + vector2 - parent.Position;
                    Vector2 b = new Vector2((float) point4.X, (float) point4.Y) * 8f + vector2 - parent.Position;
                    this.edges.Add(new LightningRenderer.Edge(parent, a, b));
                    num = 0;
                    point3 = point4;
                  }
                }
                if (num > 0)
                {
                  Vector2 a = new Vector2((float) point3.X, (float) point3.Y) * 8f + vector2 - parent.Position;
                  Vector2 b = new Vector2((float) point4.X, (float) point4.Y) * 8f + vector2 - parent.Position;
                  this.edges.Add(new LightningRenderer.Edge(parent, a, b));
                }
              }
            }
          }
        }
      }
      if (this.edgeVerts != null)
        return;
      this.edgeVerts = new VertexPositionColor[1024];
    }

    private bool Inside(int tx, int ty) => this.tiles[tx - this.levelTileBounds.X, ty - this.levelTileBounds.Y];

    private void OnRenderBloom()
    {
      Camera camera = (this.Scene as Level).Camera;
      Rectangle rectangle = new Rectangle((int) camera.Left, (int) camera.Top, (int) ((double) camera.Right - (double) camera.Left), (int) ((double) camera.Bottom - (double) camera.Top));
      Color color = Color.White * (float) (0.25 + (double) this.Fade * 0.75);
      foreach (LightningRenderer.Edge edge in this.edges)
      {
        if (edge.Visible)
          Draw.Line(edge.Parent.Position + edge.A, edge.Parent.Position + edge.B, color, 4f);
      }
      foreach (Lightning lightning in this.list)
      {
        if (lightning.Visible)
          Draw.Rect(lightning.X, lightning.Y, (float) lightning.VisualWidth, (float) lightning.VisualHeight, color);
      }
      if ((double) this.Fade <= 0.0)
        return;
      Level scene = this.Scene as Level;
      Draw.Rect(scene.Camera.X, scene.Camera.Y, 320f, 180f, Color.White * this.Fade);
    }

    private void OnBeforeRender()
    {
      if (this.list.Count <= 0)
        return;
      Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) GameplayBuffers.Lightning);
      Engine.Graphics.GraphicsDevice.Clear(Color.Lerp(Calc.HexToColor("f7b262") * 0.1f, Color.White, this.Fade));
      Draw.SpriteBatch.Begin();
      foreach (LightningRenderer.Bolt bolt in this.bolts)
        bolt.Render();
      Draw.SpriteBatch.End();
    }

    public override void Render()
    {
      if (this.list.Count <= 0)
        return;
      Camera camera = (this.Scene as Level).Camera;
      Rectangle rectangle = new Rectangle((int) camera.Left, (int) camera.Top, (int) ((double) camera.Right - (double) camera.Left), (int) ((double) camera.Bottom - (double) camera.Top));
      foreach (Lightning lightning in this.list)
      {
        if (lightning.Visible)
          Draw.SpriteBatch.Draw((Texture2D) (RenderTarget2D) GameplayBuffers.Lightning, lightning.Position, new Rectangle?(new Rectangle((int) lightning.X, (int) lightning.Y, lightning.VisualWidth, lightning.VisualHeight)), Color.White);
      }
      if (this.edges.Count <= 0 || !this.DrawEdges)
        return;
      for (int index = 0; index < this.electricityColorsLerped.Length; ++index)
        this.electricityColorsLerped[index] = Color.Lerp(this.electricityColors[index], Color.White, this.Fade);
      int index1 = 0;
      uint leapSeed = this.leapSeed;
      foreach (LightningRenderer.Edge edge in this.edges)
      {
        if (edge.Visible)
        {
          LightningRenderer.DrawSimpleLightning(ref index1, ref this.edgeVerts, this.edgeSeed, edge.Parent.Position, edge.A, edge.B, this.electricityColorsLerped[0], (float) (1.0 + (double) this.Fade * 3.0));
          LightningRenderer.DrawSimpleLightning(ref index1, ref this.edgeVerts, this.edgeSeed + 1U, edge.Parent.Position, edge.A, edge.B, this.electricityColorsLerped[1], (float) (1.0 + (double) this.Fade * 3.0));
          if (LightningRenderer.PseudoRand(ref leapSeed) % 30U == 0U)
            LightningRenderer.DrawBezierLightning(ref index1, ref this.edgeVerts, this.edgeSeed, edge.Parent.Position, edge.A, edge.B, 24f, 10, this.electricityColorsLerped[1]);
        }
      }
      if (index1 <= 0)
        return;
      GameplayRenderer.End();
      GFX.DrawVertices<VertexPositionColor>(camera.Matrix, this.edgeVerts, index1);
      GameplayRenderer.Begin();
    }

    private static void DrawSimpleLightning(
      ref int index,
      ref VertexPositionColor[] verts,
      uint seed,
      Vector2 pos,
      Vector2 a,
      Vector2 b,
      Color color,
      float thickness = 1f)
    {
      seed += (uint) (a.GetHashCode() + b.GetHashCode());
      a += pos;
      b += pos;
      float num1 = (b - a).Length();
      Vector2 vec = (b - a) / num1;
      Vector2 vector2_1 = vec.TurnRight();
      a += vector2_1;
      b += vector2_1;
      Vector2 vector2_2 = a;
      int num2 = LightningRenderer.PseudoRand(ref seed) % 2U == 0U ? -1 : 1;
      float num3 = LightningRenderer.PseudoRandRange(ref seed, 0.0f, 6.2831855f);
      float num4 = 0.0f;
      float num5 = (float) index + (float) (((double) (b - a).Length() / 4.0 + 1.0) * 6.0);
      while ((double) num5 >= (double) verts.Length)
        Array.Resize<VertexPositionColor>(ref verts, verts.Length * 2);
      for (int index1 = index; (double) index1 < (double) num5; ++index1)
        verts[index1].Color = color;
      do
      {
        float num6 = LightningRenderer.PseudoRandRange(ref seed, 0.0f, 4f);
        num3 += 0.1f;
        num4 += 4f + num6;
        Vector2 vector2_3 = a + vec * num4;
        Vector2 vector2_4 = (double) num4 >= (double) num1 ? b : vector2_3 + ((float) num2 * vector2_1 * num6 - vector2_1);
        verts[index++].Position = new Vector3(vector2_2 - vector2_1 * thickness, 0.0f);
        verts[index++].Position = new Vector3(vector2_4 - vector2_1 * thickness, 0.0f);
        verts[index++].Position = new Vector3(vector2_4 + vector2_1 * thickness, 0.0f);
        verts[index++].Position = new Vector3(vector2_2 - vector2_1 * thickness, 0.0f);
        verts[index++].Position = new Vector3(vector2_4 + vector2_1 * thickness, 0.0f);
        verts[index++].Position = new Vector3(vector2_2, 0.0f);
        vector2_2 = vector2_4;
        num2 = -num2;
      }
      while ((double) num4 < (double) num1);
    }

    private static void DrawBezierLightning(
      ref int index,
      ref VertexPositionColor[] verts,
      uint seed,
      Vector2 pos,
      Vector2 a,
      Vector2 b,
      float anchor,
      int steps,
      Color color)
    {
      seed += (uint) (a.GetHashCode() + b.GetHashCode());
      a += pos;
      b += pos;
      Vector2 vector2_1 = (b - a).SafeNormalize().TurnRight();
      SimpleCurve simpleCurve = new SimpleCurve(a, b, (b + a) / 2f + vector2_1 * anchor);
      int num = index + (steps + 2) * 6;
      while (num >= verts.Length)
        Array.Resize<VertexPositionColor>(ref verts, verts.Length * 2);
      Vector2 vector2_2 = simpleCurve.GetPoint(0.0f);
      for (int index1 = 0; index1 <= steps; ++index1)
      {
        Vector2 point = simpleCurve.GetPoint((float) index1 / (float) steps);
        if (index1 != steps)
          point += new Vector2(LightningRenderer.PseudoRandRange(ref seed, -2f, 2f), LightningRenderer.PseudoRandRange(ref seed, -2f, 2f));
        verts[index].Position = new Vector3(vector2_2 - vector2_1, 0.0f);
        verts[index++].Color = color;
        verts[index].Position = new Vector3(point - vector2_1, 0.0f);
        verts[index++].Color = color;
        verts[index].Position = new Vector3(point, 0.0f);
        verts[index++].Color = color;
        verts[index].Position = new Vector3(vector2_2 - vector2_1, 0.0f);
        verts[index++].Color = color;
        verts[index].Position = new Vector3(point, 0.0f);
        verts[index++].Color = color;
        verts[index].Position = new Vector3(vector2_2, 0.0f);
        verts[index++].Color = color;
        vector2_2 = point;
      }
    }

    private static void DrawFatLightning(
      uint seed,
      Vector2 a,
      Vector2 b,
      float size,
      float gap,
      Color color)
    {
      seed += (uint) (a.GetHashCode() + b.GetHashCode());
      float num1 = (b - a).Length();
      Vector2 vec = (b - a) / num1;
      Vector2 vector2_1 = vec.TurnRight();
      Vector2 start = a;
      int num2 = 1;
      double num3 = (double) LightningRenderer.PseudoRandRange(ref seed, 0.0f, 6.2831855f);
      float num4 = 0.0f;
      do
      {
        num4 += LightningRenderer.PseudoRandRange(ref seed, 10f, 14f);
        Vector2 vector2_2 = a + vec * num4;
        Vector2 vector2_3 = (double) num4 >= (double) num1 ? b : vector2_2 + (float) num2 * vector2_1 * LightningRenderer.PseudoRandRange(ref seed, 0.0f, 6f);
        Vector2 vector2_4 = vector2_3;
        if ((double) gap > 0.0)
        {
          vector2_4 = start + (vector2_3 - start) * (1f - gap);
          Draw.Line(start, vector2_3 + vec, color, size * 0.5f);
        }
        Draw.Line(start, vector2_4 + vec, color, size);
        start = vector2_3;
        num2 = -num2;
      }
      while ((double) num4 < (double) num1);
    }

    private static uint PseudoRand(ref uint seed)
    {
      seed ^= seed << 13;
      seed ^= seed >> 17;
      return seed;
    }

    public static float PseudoRandRange(ref uint seed, float min, float max) => min + (float) ((double) (LightningRenderer.PseudoRand(ref seed) & 1023U) / 1024.0 * ((double) max - (double) min));

    private class Bolt
    {
      private List<Vector2> nodes = new List<Vector2>();
      private Coroutine routine;
      private bool visible;
      private float size;
      private float gap;
      private float alpha;
      private uint seed;
      private float flash;
      private readonly Color color;
      private readonly float scale;
      private readonly int width;
      private readonly int height;

      public Bolt(Color color, float scale, int width, int height)
      {
        this.color = color;
        this.width = width;
        this.height = height;
        this.scale = scale;
        this.routine = new Coroutine(this.Run());
      }

      public void Update(Scene scene)
      {
        this.routine.Update();
        this.flash = Calc.Approach(this.flash, 0.0f, Engine.DeltaTime * 2f);
      }

      private IEnumerator Run()
      {
        yield return (object) Calc.Random.Range(0.0f, 4f);
        while (true)
        {
          List<Vector2> vector2List = new List<Vector2>();
          for (int index = 0; index < 3; ++index)
          {
            Vector2 vector2_1 = Calc.Random.Choose<Vector2>(new Vector2(0.0f, (float) Calc.Random.Range(8, this.height - 16)), new Vector2((float) Calc.Random.Range(8, this.width - 16), 0.0f), new Vector2((float) this.width, (float) Calc.Random.Range(8, this.height - 16)), new Vector2((float) Calc.Random.Range(8, this.width - 16), (float) this.height));
            Vector2 vector2_2 = (double) vector2_1.X <= 0.0 || (double) vector2_1.X >= (double) this.width ? new Vector2((float) this.width - vector2_1.X, vector2_1.Y) : new Vector2(vector2_1.X, (float) this.height - vector2_1.Y);
            vector2List.Add(vector2_1);
            vector2List.Add(vector2_2);
          }
          List<Vector2> list = new List<Vector2>();
          for (int index = 0; index < 3; ++index)
            list.Add(new Vector2(Calc.Random.Range(0.25f, 0.75f) * (float) this.width, Calc.Random.Range(0.25f, 0.75f) * (float) this.height));
          this.nodes.Clear();
          foreach (Vector2 to in vector2List)
          {
            this.nodes.Add(to);
            this.nodes.Add(list.ClosestTo(to));
          }
          Vector2 vector2_3 = list[list.Count - 1];
          foreach (Vector2 vector2_4 in list)
          {
            this.nodes.Add(vector2_3);
            this.nodes.Add(vector2_4);
            vector2_3 = vector2_4;
          }
          this.flash = 1f;
          this.visible = true;
          this.size = 5f;
          this.gap = 0.0f;
          this.alpha = 1f;
          int i;
          for (i = 0; i < 4; ++i)
          {
            this.seed = (uint) Calc.Random.Next();
            yield return (object) 0.1f;
          }
          for (i = 0; i < 5; ++i)
          {
            if (!Settings.Instance.DisableFlashes)
              this.visible = false;
            yield return (object) (float) (0.05000000074505806 + (double) i * 0.019999999552965164);
            float num = (float) i / 5f;
            this.visible = true;
            this.size = (float) ((1.0 - (double) num) * 5.0);
            this.gap = num;
            this.alpha = 1f - num;
            this.visible = true;
            this.seed = (uint) Calc.Random.Next();
            yield return (object) 0.025f;
          }
          this.visible = false;
          yield return (object) Calc.Random.Range(4f, 8f);
        }
      }

      public void Render()
      {
        if ((double) this.flash > 0.0 && !Settings.Instance.DisableFlashes)
          Draw.Rect(0.0f, 0.0f, (float) this.width, (float) this.height, Color.White * this.flash * 0.15f * this.scale);
        if (!this.visible)
          return;
        for (int index = 0; index < this.nodes.Count; index += 2)
          LightningRenderer.DrawFatLightning(this.seed, this.nodes[index], this.nodes[index + 1], this.size * this.scale, this.gap, this.color * this.alpha);
      }
    }

    private class Edge
    {
      public Lightning Parent;
      public bool Visible;
      public Vector2 A;
      public Vector2 B;
      public Vector2 Min;
      public Vector2 Max;

      public Edge(Lightning parent, Vector2 a, Vector2 b)
      {
        this.Parent = parent;
        this.Visible = true;
        this.A = a;
        this.B = b;
        this.Min = new Vector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        this.Max = new Vector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
      }

      public bool InView(ref Rectangle view) => (double) view.Left < (double) this.Parent.X + (double) this.Max.X && (double) view.Right > (double) this.Parent.X + (double) this.Min.X && (double) view.Top < (double) this.Parent.Y + (double) this.Max.Y && (double) view.Bottom > (double) this.Parent.Y + (double) this.Min.Y;
    }
  }
}
