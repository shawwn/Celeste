// Decompiled with JetBrains decompiler
// Type: Celeste.SeekerBarrierRenderer
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(false)]
  public class SeekerBarrierRenderer : Entity
  {
    private List<SeekerBarrier> list = new List<SeekerBarrier>();
    private List<SeekerBarrierRenderer.Edge> edges = new List<SeekerBarrierRenderer.Edge>();
    private VirtualMap<bool> tiles;
    private Rectangle levelTileBounds;
    private bool dirty;

    public SeekerBarrierRenderer()
    {
      this.Tag = (int) Tags.Global | (int) Tags.TransitionUpdate;
      this.Depth = 0;
      this.Add((Component) new CustomBloom(new Action(this.OnRenderBloom)));
    }

    public void Track(SeekerBarrier block)
    {
      this.list.Add(block);
      if (this.tiles == null)
      {
        this.levelTileBounds = (this.Scene as Level).TileBounds;
        this.tiles = new VirtualMap<bool>(this.levelTileBounds.Width, this.levelTileBounds.Height);
      }
      for (int index1 = (int) block.X / 8; (double) index1 < (double) block.Right / 8.0; ++index1)
      {
        for (int index2 = (int) block.Y / 8; (double) index2 < (double) block.Bottom / 8.0; ++index2)
          this.tiles[index1 - this.levelTileBounds.X, index2 - this.levelTileBounds.Y] = true;
      }
      this.dirty = true;
    }

    public void Untrack(SeekerBarrier block)
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
      this.UpdateEdges();
    }

    public void UpdateEdges()
    {
      Camera camera = (this.Scene as Level).Camera;
      Rectangle view = new Rectangle((int) camera.Left - 4, (int) camera.Top - 4, (int) ((double) camera.Right - (double) camera.Left) + 8, (int) ((double) camera.Bottom - (double) camera.Top) + 8);
      for (int index = 0; index < this.edges.Count; ++index)
      {
        if (this.edges[index].Visible)
        {
          if (this.Scene.OnInterval(0.25f, (float) index * 0.01f) && !this.edges[index].InView(ref view))
            this.edges[index].Visible = false;
        }
        else if (this.Scene.OnInterval(0.05f, (float) index * 0.01f) && this.edges[index].InView(ref view))
          this.edges[index].Visible = true;
        if (this.edges[index].Visible && (this.Scene.OnInterval(0.05f, (float) index * 0.01f) || this.edges[index].Wave == null))
          this.edges[index].UpdateWave(this.Scene.TimeActive * 3f);
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
      foreach (SeekerBarrier parent in this.list)
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
                for (; this.Inside(point4.X, point4.Y) && !this.Inside(point4.X + point1.X, point4.Y + point1.Y); point4.Y += point2.Y)
                  point4.X += point2.X;
                Vector2 a = new Vector2((float) point3.X, (float) point3.Y) * 8f + vector2 - parent.Position;
                Vector2 b = new Vector2((float) point4.X, (float) point4.Y) * 8f + vector2 - parent.Position;
                this.edges.Add(new SeekerBarrierRenderer.Edge(parent, a, b));
              }
            }
          }
        }
      }
    }

    private bool Inside(int tx, int ty) => this.tiles[tx - this.levelTileBounds.X, ty - this.levelTileBounds.Y];

    private void OnRenderBloom()
    {
      Camera camera = (this.Scene as Level).Camera;
      Rectangle rectangle = new Rectangle((int) camera.Left, (int) camera.Top, (int) ((double) camera.Right - (double) camera.Left), (int) ((double) camera.Bottom - (double) camera.Top));
      foreach (SeekerBarrier seekerBarrier in this.list)
      {
        if (seekerBarrier.Visible)
          Draw.Rect(seekerBarrier.X, seekerBarrier.Y, seekerBarrier.Width, seekerBarrier.Height, Color.White);
      }
      foreach (SeekerBarrierRenderer.Edge edge in this.edges)
      {
        if (edge.Visible)
        {
          Vector2 vector2_1 = edge.Parent.Position + edge.A;
          Vector2 vector2_2 = edge.Parent.Position + edge.B;
          for (int index = 0; (double) index <= (double) edge.Length; ++index)
          {
            Vector2 start = vector2_1 + edge.Normal * (float) index;
            Draw.Line(start, start + edge.Perpendicular * edge.Wave[index], Color.White);
          }
        }
      }
    }

    public override void Render()
    {
      if (this.list.Count <= 0)
        return;
      Color color1 = Color.White * 0.15f;
      Color color2 = Color.White * 0.25f;
      foreach (SeekerBarrier seekerBarrier in this.list)
      {
        if (seekerBarrier.Visible)
          Draw.Rect(seekerBarrier.Collider, color1);
      }
      if (this.edges.Count <= 0)
        return;
      foreach (SeekerBarrierRenderer.Edge edge in this.edges)
      {
        if (edge.Visible)
        {
          Vector2 vector2_1 = edge.Parent.Position + edge.A;
          Vector2 vector2_2 = edge.Parent.Position + edge.B;
          Color.Lerp(color2, Color.White, edge.Parent.Flash);
          for (int index = 0; (double) index <= (double) edge.Length; ++index)
          {
            Vector2 start = vector2_1 + edge.Normal * (float) index;
            Draw.Line(start, start + edge.Perpendicular * edge.Wave[index], color1);
          }
        }
      }
    }

    private class Edge
    {
      public SeekerBarrier Parent;
      public bool Visible;
      public Vector2 A;
      public Vector2 B;
      public Vector2 Min;
      public Vector2 Max;
      public Vector2 Normal;
      public Vector2 Perpendicular;
      public float[] Wave;
      public float Length;

      public Edge(SeekerBarrier parent, Vector2 a, Vector2 b)
      {
        this.Parent = parent;
        this.Visible = true;
        this.A = a;
        this.B = b;
        this.Min = new Vector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        this.Max = new Vector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
        this.Normal = (b - a).SafeNormalize();
        this.Perpendicular = -this.Normal.Perpendicular();
        this.Length = (a - b).Length();
      }

      public void UpdateWave(float time)
      {
        if (this.Wave == null || (double) this.Wave.Length <= (double) this.Length)
          this.Wave = new float[(int) this.Length + 2];
        for (int along = 0; (double) along <= (double) this.Length; ++along)
          this.Wave[along] = this.GetWaveAt(time, (float) along, this.Length);
      }

      private float GetWaveAt(float offset, float along, float length)
      {
        if ((double) along <= 1.0 || (double) along >= (double) length - 1.0 || (double) this.Parent.Solidify >= 1.0)
          return 0.0f;
        float a = offset + along * 0.25f;
        return (float) ((1.0 + (Math.Sin((double) a) * 2.0 + Math.Sin((double) a * 0.25)) * (double) Ease.SineInOut(Calc.YoYo(along / length))) * (1.0 - (double) this.Parent.Solidify));
      }

      public bool InView(ref Rectangle view) => (double) view.Left < (double) this.Parent.X + (double) this.Max.X && (double) view.Right > (double) this.Parent.X + (double) this.Min.X && (double) view.Top < (double) this.Parent.Y + (double) this.Max.Y && (double) view.Bottom > (double) this.Parent.Y + (double) this.Min.Y;
    }
  }
}
