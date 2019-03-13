// Decompiled with JetBrains decompiler
// Type: Celeste.Pathfinder
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class Pathfinder
  {
    private static readonly Point[] directions = new Point[4]
    {
      new Point(1, 0),
      new Point(0, 1),
      new Point(-1, 0),
      new Point(0, -1)
    };
    private List<Point> active = new List<Point>();
    private const int MapSize = 200;
    private Level level;
    private Pathfinder.Tile[,] map;
    private Pathfinder.PointMapComparer comparer;
    public bool DebugRenderEnabled;
    private List<Vector2> lastPath;
    private Point debugLastStart;
    private Point debugLastEnd;

    public Pathfinder(Level level)
    {
      this.level = level;
      this.map = new Pathfinder.Tile[200, 200];
      this.comparer = new Pathfinder.PointMapComparer(this.map);
    }

    public bool Find(
      ref List<Vector2> path,
      Vector2 from,
      Vector2 to,
      bool fewerTurns = true,
      bool logging = false)
    {
      this.lastPath = (List<Vector2>) null;
      Rectangle bounds1 = this.level.Bounds;
      int num1 = ((Rectangle) ref bounds1).get_Left() / 8;
      Rectangle bounds2 = this.level.Bounds;
      int num2 = ((Rectangle) ref bounds2).get_Top() / 8;
      int num3 = this.level.Bounds.Width / 8;
      int num4 = this.level.Bounds.Height / 8;
      Point levelSolidOffset = this.level.LevelSolidOffset;
      for (int index1 = 0; index1 < num3; ++index1)
      {
        for (int index2 = 0; index2 < num4; ++index2)
        {
          this.map[index1, index2].Solid = this.level.SolidsData[index1 + levelSolidOffset.X, index2 + levelSolidOffset.Y] != '0';
          this.map[index1, index2].Cost = int.MaxValue;
          this.map[index1, index2].Parent = new Point?();
        }
      }
      foreach (Entity entity in this.level.Tracker.GetEntities<Solid>())
      {
        if (entity.Collidable && entity.Collider is Hitbox)
        {
          int num5 = (int) Math.Floor((double) entity.Left / 8.0);
          for (int index1 = (int) Math.Ceiling((double) entity.Right / 8.0); num5 < index1; ++num5)
          {
            int num6 = (int) Math.Floor((double) entity.Top / 8.0);
            for (int index2 = (int) Math.Ceiling((double) entity.Bottom / 8.0); num6 < index2; ++num6)
            {
              int index3 = num5 - num1;
              int index4 = num6 - num2;
              if (index3 >= 0 && index4 >= 0 && (index3 < num3 && index4 < num4))
                this.map[index3, index4].Solid = true;
            }
          }
        }
      }
      Point point1 = this.debugLastStart = new Point((int) Math.Floor(from.X / 8.0) - num1, (int) Math.Floor(from.Y / 8.0) - num2);
      Point point2 = this.debugLastEnd = new Point((int) Math.Floor(to.X / 8.0) - num1, (int) Math.Floor(to.Y / 8.0) - num2);
      if (point1.X < 0 || point1.Y < 0 || (point1.X >= num3 || point1.Y >= num4) || (point2.X < 0 || point2.Y < 0 || (point2.X >= num3 || point2.Y >= num4)))
      {
        if (logging)
          Calc.Log((object) "PF: FAILED - Start or End outside the level bounds");
        return false;
      }
      if (this.map[(int) point1.X, (int) point1.Y].Solid)
      {
        if (logging)
          Calc.Log((object) "PF: FAILED - Start inside a solid");
        return false;
      }
      if (this.map[(int) point2.X, (int) point2.Y].Solid)
      {
        if (logging)
          Calc.Log((object) "PF: FAILED - End inside a solid");
        return false;
      }
      this.active.Clear();
      this.active.Add(point1);
      this.map[(int) point1.X, (int) point1.Y].Cost = 0;
      bool flag = false;
      while (this.active.Count > 0 && !flag)
      {
        Point point3 = this.active[this.active.Count - 1];
        this.active.RemoveAt(this.active.Count - 1);
        for (int index1 = 0; index1 < 4; ++index1)
        {
          Point point4;
          ((Point) ref point4).\u002Ector((int) Pathfinder.directions[index1].X, (int) Pathfinder.directions[index1].Y);
          Point point5;
          ((Point) ref point5).\u002Ector((int) (point3.X + point4.X), (int) (point3.Y + point4.Y));
          int num5 = 1;
          if (point5.X >= 0 && point5.Y >= 0 && (point5.X < num3 && point5.Y < num4) && !this.map[(int) point5.X, (int) point5.Y].Solid)
          {
            for (int index2 = 0; index2 < 4; ++index2)
            {
              Point point6;
              ((Point) ref point6).\u002Ector((int) (point5.X + Pathfinder.directions[index2].X), (int) (point5.Y + Pathfinder.directions[index2].Y));
              if (point6.X >= 0 && point6.Y >= 0 && (point6.X < num3 && point6.Y < num4) && this.map[(int) point6.X, (int) point6.Y].Solid)
              {
                num5 = 7;
                break;
              }
            }
            if (fewerTurns && this.map[(int) point3.X, (int) point3.Y].Parent.HasValue && (point5.X != this.map[(int) point3.X, (int) point3.Y].Parent.Value.X && point5.Y != this.map[(int) point3.X, (int) point3.Y].Parent.Value.Y))
              num5 += 4;
            int cost = this.map[(int) point3.X, (int) point3.Y].Cost;
            if (point4.Y != null)
              num5 += (int) ((double) cost * 0.5);
            int num6 = cost + num5;
            if (this.map[(int) point5.X, (int) point5.Y].Cost > num6)
            {
              this.map[(int) point5.X, (int) point5.Y].Cost = num6;
              this.map[(int) point5.X, (int) point5.Y].Parent = new Point?(point3);
              int index2 = this.active.BinarySearch(point5, (IComparer<Point>) this.comparer);
              if (index2 < 0)
                index2 = ~index2;
              this.active.Insert(index2, point5);
              if (Point.op_Equality(point5, point2))
              {
                flag = true;
                break;
              }
            }
          }
        }
      }
      if (!flag)
      {
        if (logging)
          Calc.Log((object) "PF: FAILED - ran out of active nodes, can't find ending");
        return false;
      }
      path.Clear();
      Point point7 = point2;
      int num7;
      for (num7 = 0; Point.op_Inequality(point7, point1) && num7++ < 1000; point7 = this.map[(int) point7.X, (int) point7.Y].Parent.Value)
        path.Add(Vector2.op_Addition(Vector2.op_Multiply(new Vector2((float) point7.X + 0.5f, (float) point7.Y + 0.5f), 8f), this.level.LevelOffset));
      if (num7 >= 1000)
      {
        Console.WriteLine("WARNING: Pathfinder 'succeeded' but then was unable to work out its path?");
        return false;
      }
      for (int index = 1; index < path.Count - 1 && path.Count > 2; ++index)
      {
        if (path[index].X == path[index - 1].X && path[index].X == path[index + 1].X || path[index].Y == path[index - 1].Y && path[index].Y == path[index + 1].Y)
        {
          path.RemoveAt(index);
          --index;
        }
      }
      path.Reverse();
      this.lastPath = path;
      if (logging)
        Calc.Log((object) "PF: SUCCESS");
      return true;
    }

    public void Render()
    {
      Rectangle bounds;
      for (int index1 = 0; index1 < 200; ++index1)
      {
        for (int index2 = 0; index2 < 200; ++index2)
        {
          if (this.map[index1, index2].Solid)
          {
            bounds = this.level.Bounds;
            double num1 = (double) (((Rectangle) ref bounds).get_Left() + index1 * 8);
            bounds = this.level.Bounds;
            double num2 = (double) (((Rectangle) ref bounds).get_Top() + index2 * 8);
            Color color = Color.op_Multiply(Color.get_Red(), 0.25f);
            Draw.Rect((float) num1, (float) num2, 8f, 8f, color);
          }
        }
      }
      if (this.lastPath != null)
      {
        Vector2 start = this.lastPath[0];
        for (int index = 1; index < this.lastPath.Count; ++index)
        {
          Vector2 end = this.lastPath[index];
          Draw.Line(start, end, Color.get_Red());
          Draw.Rect((float) (start.X - 2.0), (float) (start.Y - 2.0), 4f, 4f, Color.get_Red());
          start = end;
        }
        Draw.Rect((float) (start.X - 2.0), (float) (start.Y - 2.0), 4f, 4f, Color.get_Red());
      }
      bounds = this.level.Bounds;
      double num3 = (double) (((Rectangle) ref bounds).get_Left() + this.debugLastStart.X * 8 + 2);
      bounds = this.level.Bounds;
      double num4 = (double) (((Rectangle) ref bounds).get_Top() + this.debugLastStart.Y * 8 + 2);
      Color green1 = Color.get_Green();
      Draw.Rect((float) num3, (float) num4, 4f, 4f, green1);
      bounds = this.level.Bounds;
      double num5 = (double) (((Rectangle) ref bounds).get_Left() + this.debugLastEnd.X * 8 + 2);
      bounds = this.level.Bounds;
      double num6 = (double) (((Rectangle) ref bounds).get_Top() + this.debugLastEnd.Y * 8 + 2);
      Color green2 = Color.get_Green();
      Draw.Rect((float) num5, (float) num6, 4f, 4f, green2);
    }

    private struct Tile
    {
      public bool Solid;
      public int Cost;
      public Point? Parent;
    }

    private class PointMapComparer : IComparer<Point>
    {
      private Pathfinder.Tile[,] map;

      public PointMapComparer(Pathfinder.Tile[,] map)
      {
        this.map = map;
      }

      public int Compare(Point a, Point b)
      {
        return this.map[(int) b.X, (int) b.Y].Cost - this.map[(int) a.X, (int) a.Y].Cost;
      }
    }
  }
}
