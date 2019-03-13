﻿// Decompiled with JetBrains decompiler
// Type: Monocle.Pnt
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

namespace Monocle
{
  public struct Pnt
  {
    public static readonly Pnt Zero = new Pnt(0, 0);
    public static readonly Pnt UnitX = new Pnt(1, 0);
    public static readonly Pnt UnitY = new Pnt(0, 1);
    public static readonly Pnt One = new Pnt(1, 1);
    public int X;
    public int Y;

    public Pnt(int x, int y)
    {
      this.X = x;
      this.Y = y;
    }

    public static bool operator ==(Pnt a, Pnt b)
    {
      if (a.X == b.X)
        return a.Y == b.Y;
      return false;
    }

    public static bool operator !=(Pnt a, Pnt b)
    {
      if (a.X == b.X)
        return a.Y != b.Y;
      return true;
    }

    public static Pnt operator +(Pnt a, Pnt b)
    {
      return new Pnt(a.X + b.X, a.Y + b.Y);
    }

    public static Pnt operator -(Pnt a, Pnt b)
    {
      return new Pnt(a.X - b.X, a.Y - b.Y);
    }

    public static Pnt operator *(Pnt a, Pnt b)
    {
      return new Pnt(a.X * b.X, a.Y * b.Y);
    }

    public static Pnt operator /(Pnt a, Pnt b)
    {
      return new Pnt(a.X / b.X, a.Y / b.Y);
    }

    public static Pnt operator %(Pnt a, Pnt b)
    {
      return new Pnt(a.X % b.X, a.Y % b.Y);
    }

    public static bool operator ==(Pnt a, int b)
    {
      if (a.X == b)
        return a.Y == b;
      return false;
    }

    public static bool operator !=(Pnt a, int b)
    {
      if (a.X == b)
        return a.Y != b;
      return true;
    }

    public static Pnt operator +(Pnt a, int b)
    {
      return new Pnt(a.X + b, a.Y + b);
    }

    public static Pnt operator -(Pnt a, int b)
    {
      return new Pnt(a.X - b, a.Y - b);
    }

    public static Pnt operator *(Pnt a, int b)
    {
      return new Pnt(a.X * b, a.Y * b);
    }

    public static Pnt operator /(Pnt a, int b)
    {
      return new Pnt(a.X / b, a.Y / b);
    }

    public static Pnt operator %(Pnt a, int b)
    {
      return new Pnt(a.X % b, a.Y % b);
    }

    public override bool Equals(object obj)
    {
      return false;
    }

    public override int GetHashCode()
    {
      return this.X * 10000 + this.Y;
    }

    public override string ToString()
    {
      return "{ X: " + (object) this.X + ", Y: " + (object) this.Y + " }";
    }
  }
}
