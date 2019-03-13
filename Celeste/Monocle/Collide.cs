﻿// Decompiled with JetBrains decompiler
// Type: Monocle.Collide
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Monocle
{
  public static class Collide
  {
    public static bool Check(Entity a, Entity b)
    {
      if (a.Collider == null || b.Collider == null || (a == b || !b.Collidable))
        return false;
      return a.Collider.Collide(b);
    }

    public static bool Check(Entity a, Entity b, Vector2 at)
    {
      Vector2 position = a.Position;
      a.Position = at;
      int num = Collide.Check(a, b) ? 1 : 0;
      a.Position = position;
      return num != 0;
    }

    public static bool Check(Entity a, IEnumerable<Entity> b)
    {
      foreach (Entity b1 in b)
      {
        if (Collide.Check(a, b1))
          return true;
      }
      return false;
    }

    public static bool Check(Entity a, IEnumerable<Entity> b, Vector2 at)
    {
      Vector2 position = a.Position;
      a.Position = at;
      int num = Collide.Check(a, b) ? 1 : 0;
      a.Position = position;
      return num != 0;
    }

    public static Entity First(Entity a, IEnumerable<Entity> b)
    {
      foreach (Entity b1 in b)
      {
        if (Collide.Check(a, b1))
          return b1;
      }
      return (Entity) null;
    }

    public static Entity First(Entity a, IEnumerable<Entity> b, Vector2 at)
    {
      Vector2 position = a.Position;
      a.Position = at;
      Entity entity = Collide.First(a, b);
      a.Position = position;
      return entity;
    }

    public static List<Entity> All(Entity a, IEnumerable<Entity> b, List<Entity> into)
    {
      foreach (Entity b1 in b)
      {
        if (Collide.Check(a, b1))
          into.Add(b1);
      }
      return into;
    }

    public static List<Entity> All(
      Entity a,
      IEnumerable<Entity> b,
      List<Entity> into,
      Vector2 at)
    {
      Vector2 position = a.Position;
      a.Position = at;
      List<Entity> entityList = Collide.All(a, b, into);
      a.Position = position;
      return entityList;
    }

    public static List<Entity> All(Entity a, IEnumerable<Entity> b)
    {
      return Collide.All(a, b, new List<Entity>());
    }

    public static List<Entity> All(Entity a, IEnumerable<Entity> b, Vector2 at)
    {
      return Collide.All(a, b, new List<Entity>(), at);
    }

    public static bool CheckPoint(Entity a, Vector2 point)
    {
      if (a.Collider == null)
        return false;
      return a.Collider.Collide(point);
    }

    public static bool CheckPoint(Entity a, Vector2 point, Vector2 at)
    {
      Vector2 position = a.Position;
      a.Position = at;
      int num = Collide.CheckPoint(a, point) ? 1 : 0;
      a.Position = position;
      return num != 0;
    }

    public static bool CheckLine(Entity a, Vector2 from, Vector2 to)
    {
      if (a.Collider == null)
        return false;
      return a.Collider.Collide(from, to);
    }

    public static bool CheckLine(Entity a, Vector2 from, Vector2 to, Vector2 at)
    {
      Vector2 position = a.Position;
      a.Position = at;
      int num = Collide.CheckLine(a, from, to) ? 1 : 0;
      a.Position = position;
      return num != 0;
    }

    public static bool CheckRect(Entity a, Rectangle rect)
    {
      if (a.Collider == null)
        return false;
      return a.Collider.Collide(rect);
    }

    public static bool CheckRect(Entity a, Rectangle rect, Vector2 at)
    {
      Vector2 position = a.Position;
      a.Position = at;
      int num = Collide.CheckRect(a, rect) ? 1 : 0;
      a.Position = position;
      return num != 0;
    }

    public static bool LineCheck(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
      Vector2 vector2_1 = Vector2.op_Subtraction(a2, a1);
      Vector2 vector2_2 = Vector2.op_Subtraction(b2, b1);
      float num1 = (float) (vector2_1.X * vector2_2.Y - vector2_1.Y * vector2_2.X);
      if ((double) num1 == 0.0)
        return false;
      Vector2 vector2_3 = Vector2.op_Subtraction(b1, a1);
      float num2 = (float) (vector2_3.X * vector2_2.Y - vector2_3.Y * vector2_2.X) / num1;
      if ((double) num2 < 0.0 || (double) num2 > 1.0)
        return false;
      float num3 = (float) (vector2_3.X * vector2_1.Y - vector2_3.Y * vector2_1.X) / num1;
      return (double) num3 >= 0.0 && (double) num3 <= 1.0;
    }

    public static bool LineCheck(
      Vector2 a1,
      Vector2 a2,
      Vector2 b1,
      Vector2 b2,
      out Vector2 intersection)
    {
      intersection = Vector2.get_Zero();
      Vector2 vector2_1 = Vector2.op_Subtraction(a2, a1);
      Vector2 vector2_2 = Vector2.op_Subtraction(b2, b1);
      float num1 = (float) (vector2_1.X * vector2_2.Y - vector2_1.Y * vector2_2.X);
      if ((double) num1 == 0.0)
        return false;
      Vector2 vector2_3 = Vector2.op_Subtraction(b1, a1);
      float num2 = (float) (vector2_3.X * vector2_2.Y - vector2_3.Y * vector2_2.X) / num1;
      if ((double) num2 < 0.0 || (double) num2 > 1.0)
        return false;
      float num3 = (float) (vector2_3.X * vector2_1.Y - vector2_3.Y * vector2_1.X) / num1;
      if ((double) num3 < 0.0 || (double) num3 > 1.0)
        return false;
      intersection = Vector2.op_Addition(a1, Vector2.op_Multiply(num2, vector2_1));
      return true;
    }

    public static bool CircleToLine(
      Vector2 cPosiition,
      float cRadius,
      Vector2 lineFrom,
      Vector2 lineTo)
    {
      return (double) Vector2.DistanceSquared(cPosiition, Calc.ClosestPointOnLine(lineFrom, lineTo, cPosiition)) < (double) cRadius * (double) cRadius;
    }

    public static bool CircleToPoint(Vector2 cPosition, float cRadius, Vector2 point)
    {
      return (double) Vector2.DistanceSquared(cPosition, point) < (double) cRadius * (double) cRadius;
    }

    public static bool CircleToRect(
      Vector2 cPosition,
      float cRadius,
      float rX,
      float rY,
      float rW,
      float rH)
    {
      return Collide.RectToCircle(rX, rY, rW, rH, cPosition, cRadius);
    }

    public static bool CircleToRect(Vector2 cPosition, float cRadius, Rectangle rect)
    {
      return Collide.RectToCircle(rect, cPosition, cRadius);
    }

    public static bool RectToCircle(
      float rX,
      float rY,
      float rW,
      float rH,
      Vector2 cPosition,
      float cRadius)
    {
      if (Collide.RectToPoint(rX, rY, rW, rH, cPosition))
        return true;
      PointSectors sector = Collide.GetSector(rX, rY, rW, rH, cPosition);
      Vector2 lineFrom;
      Vector2 lineTo;
      if ((sector & PointSectors.Top) != PointSectors.Center)
      {
        ((Vector2) ref lineFrom).\u002Ector(rX, rY);
        ((Vector2) ref lineTo).\u002Ector(rX + rW, rY);
        if (Collide.CircleToLine(cPosition, cRadius, lineFrom, lineTo))
          return true;
      }
      if ((sector & PointSectors.Bottom) != PointSectors.Center)
      {
        ((Vector2) ref lineFrom).\u002Ector(rX, rY + rH);
        ((Vector2) ref lineTo).\u002Ector(rX + rW, rY + rH);
        if (Collide.CircleToLine(cPosition, cRadius, lineFrom, lineTo))
          return true;
      }
      if ((sector & PointSectors.Left) != PointSectors.Center)
      {
        ((Vector2) ref lineFrom).\u002Ector(rX, rY);
        ((Vector2) ref lineTo).\u002Ector(rX, rY + rH);
        if (Collide.CircleToLine(cPosition, cRadius, lineFrom, lineTo))
          return true;
      }
      if ((sector & PointSectors.Right) != PointSectors.Center)
      {
        ((Vector2) ref lineFrom).\u002Ector(rX + rW, rY);
        ((Vector2) ref lineTo).\u002Ector(rX + rW, rY + rH);
        if (Collide.CircleToLine(cPosition, cRadius, lineFrom, lineTo))
          return true;
      }
      return false;
    }

    public static bool RectToCircle(Rectangle rect, Vector2 cPosition, float cRadius)
    {
      return Collide.RectToCircle((float) rect.X, (float) rect.Y, (float) rect.Width, (float) rect.Height, cPosition, cRadius);
    }

    public static bool RectToLine(
      float rX,
      float rY,
      float rW,
      float rH,
      Vector2 lineFrom,
      Vector2 lineTo)
    {
      PointSectors sector1 = Collide.GetSector(rX, rY, rW, rH, lineFrom);
      PointSectors sector2 = Collide.GetSector(rX, rY, rW, rH, lineTo);
      if (sector1 == PointSectors.Center || sector2 == PointSectors.Center)
        return true;
      if ((sector1 & sector2) != PointSectors.Center)
        return false;
      PointSectors pointSectors = sector1 | sector2;
      Vector2 vector2;
      if ((pointSectors & PointSectors.Top) != PointSectors.Center)
      {
        Vector2 a1 = new Vector2(rX, rY);
        ((Vector2) ref vector2).\u002Ector(rX + rW, rY);
        Vector2 a2 = vector2;
        Vector2 b1 = lineFrom;
        Vector2 b2 = lineTo;
        if (Collide.LineCheck(a1, a2, b1, b2))
          return true;
      }
      if ((pointSectors & PointSectors.Bottom) != PointSectors.Center)
      {
        Vector2 a1 = new Vector2(rX, rY + rH);
        ((Vector2) ref vector2).\u002Ector(rX + rW, rY + rH);
        Vector2 a2 = vector2;
        Vector2 b1 = lineFrom;
        Vector2 b2 = lineTo;
        if (Collide.LineCheck(a1, a2, b1, b2))
          return true;
      }
      if ((pointSectors & PointSectors.Left) != PointSectors.Center)
      {
        Vector2 a1 = new Vector2(rX, rY);
        ((Vector2) ref vector2).\u002Ector(rX, rY + rH);
        Vector2 a2 = vector2;
        Vector2 b1 = lineFrom;
        Vector2 b2 = lineTo;
        if (Collide.LineCheck(a1, a2, b1, b2))
          return true;
      }
      if ((pointSectors & PointSectors.Right) != PointSectors.Center)
      {
        Vector2 a1 = new Vector2(rX + rW, rY);
        ((Vector2) ref vector2).\u002Ector(rX + rW, rY + rH);
        Vector2 a2 = vector2;
        Vector2 b1 = lineFrom;
        Vector2 b2 = lineTo;
        if (Collide.LineCheck(a1, a2, b1, b2))
          return true;
      }
      return false;
    }

    public static bool RectToLine(Rectangle rect, Vector2 lineFrom, Vector2 lineTo)
    {
      return Collide.RectToLine((float) rect.X, (float) rect.Y, (float) rect.Width, (float) rect.Height, lineFrom, lineTo);
    }

    public static bool RectToPoint(float rX, float rY, float rW, float rH, Vector2 point)
    {
      if (point.X >= (double) rX && point.Y >= (double) rY && point.X < (double) rX + (double) rW)
        return point.Y < (double) rY + (double) rH;
      return false;
    }

    public static bool RectToPoint(Rectangle rect, Vector2 point)
    {
      return Collide.RectToPoint((float) rect.X, (float) rect.Y, (float) rect.Width, (float) rect.Height, point);
    }

    public static PointSectors GetSector(Rectangle rect, Vector2 point)
    {
      PointSectors pointSectors = PointSectors.Center;
      if (point.X < (double) ((Rectangle) ref rect).get_Left())
        pointSectors |= PointSectors.Left;
      else if (point.X >= (double) ((Rectangle) ref rect).get_Right())
        pointSectors |= PointSectors.Right;
      if (point.Y < (double) ((Rectangle) ref rect).get_Top())
        pointSectors |= PointSectors.Top;
      else if (point.Y >= (double) ((Rectangle) ref rect).get_Bottom())
        pointSectors |= PointSectors.Bottom;
      return pointSectors;
    }

    public static PointSectors GetSector(
      float rX,
      float rY,
      float rW,
      float rH,
      Vector2 point)
    {
      PointSectors pointSectors = PointSectors.Center;
      if (point.X < (double) rX)
        pointSectors |= PointSectors.Left;
      else if (point.X >= (double) rX + (double) rW)
        pointSectors |= PointSectors.Right;
      if (point.Y < (double) rY)
        pointSectors |= PointSectors.Top;
      else if (point.Y >= (double) rY + (double) rH)
        pointSectors |= PointSectors.Bottom;
      return pointSectors;
    }
  }
}
