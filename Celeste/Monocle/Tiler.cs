// Decompiled with JetBrains decompiler
// Type: Monocle.Tiler
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace Monocle
{
  public static class Tiler
  {
    public static int[,] Tile(
      bool[,] bits,
      Func<int> tileDecider,
      Action<int> tileOutput,
      int tileWidth,
      int tileHeight,
      Tiler.EdgeBehavior edges)
    {
      int length1 = bits.GetLength(0);
      int length2 = bits.GetLength(1);
      int[,] numArray = new int[length1, length2];
      for (Tiler.TileX = 0; Tiler.TileX < length1; ++Tiler.TileX)
      {
        for (Tiler.TileY = 0; Tiler.TileY < length2; ++Tiler.TileY)
        {
          if (bits[Tiler.TileX, Tiler.TileY])
          {
            switch (edges)
            {
              case Tiler.EdgeBehavior.True:
                Tiler.Left = Tiler.TileX == 0 || bits[Tiler.TileX - 1, Tiler.TileY];
                Tiler.Right = Tiler.TileX == length1 - 1 || bits[Tiler.TileX + 1, Tiler.TileY];
                Tiler.Up = Tiler.TileY == 0 || bits[Tiler.TileX, Tiler.TileY - 1];
                Tiler.Down = Tiler.TileY == length2 - 1 || bits[Tiler.TileX, Tiler.TileY + 1];
                Tiler.UpLeft = Tiler.TileX == 0 || Tiler.TileY == 0 || bits[Tiler.TileX - 1, Tiler.TileY - 1];
                Tiler.UpRight = Tiler.TileX == length1 - 1 || Tiler.TileY == 0 || bits[Tiler.TileX + 1, Tiler.TileY - 1];
                Tiler.DownLeft = Tiler.TileX == 0 || Tiler.TileY == length2 - 1 || bits[Tiler.TileX - 1, Tiler.TileY + 1];
                Tiler.DownRight = Tiler.TileX == length1 - 1 || Tiler.TileY == length2 - 1 || bits[Tiler.TileX + 1, Tiler.TileY + 1];
                break;
              case Tiler.EdgeBehavior.False:
                Tiler.Left = Tiler.TileX != 0 && bits[Tiler.TileX - 1, Tiler.TileY];
                Tiler.Right = Tiler.TileX != length1 - 1 && bits[Tiler.TileX + 1, Tiler.TileY];
                Tiler.Up = Tiler.TileY != 0 && bits[Tiler.TileX, Tiler.TileY - 1];
                Tiler.Down = Tiler.TileY != length2 - 1 && bits[Tiler.TileX, Tiler.TileY + 1];
                Tiler.UpLeft = Tiler.TileX != 0 && Tiler.TileY != 0 && bits[Tiler.TileX - 1, Tiler.TileY - 1];
                Tiler.UpRight = Tiler.TileX != length1 - 1 && Tiler.TileY != 0 && bits[Tiler.TileX + 1, Tiler.TileY - 1];
                Tiler.DownLeft = Tiler.TileX != 0 && Tiler.TileY != length2 - 1 && bits[Tiler.TileX - 1, Tiler.TileY + 1];
                Tiler.DownRight = Tiler.TileX != length1 - 1 && Tiler.TileY != length2 - 1 && bits[Tiler.TileX + 1, Tiler.TileY + 1];
                break;
              case Tiler.EdgeBehavior.Wrap:
                Tiler.Left = bits[(Tiler.TileX + length1 - 1) % length1, Tiler.TileY];
                Tiler.Right = bits[(Tiler.TileX + 1) % length1, Tiler.TileY];
                Tiler.Up = bits[Tiler.TileX, (Tiler.TileY + length2 - 1) % length2];
                Tiler.Down = bits[Tiler.TileX, (Tiler.TileY + 1) % length2];
                Tiler.UpLeft = bits[(Tiler.TileX + length1 - 1) % length1, (Tiler.TileY + length2 - 1) % length2];
                Tiler.UpRight = bits[(Tiler.TileX + 1) % length1, (Tiler.TileY + length2 - 1) % length2];
                Tiler.DownLeft = bits[(Tiler.TileX + length1 - 1) % length1, (Tiler.TileY + 1) % length2];
                Tiler.DownRight = bits[(Tiler.TileX + 1) % length1, (Tiler.TileY + 1) % length2];
                break;
            }
            int num = tileDecider();
            tileOutput(num);
            numArray[Tiler.TileX, Tiler.TileY] = num;
          }
        }
      }
      return numArray;
    }

    public static int[,] Tile(
      bool[,] bits,
      bool[,] mask,
      Func<int> tileDecider,
      Action<int> tileOutput,
      int tileWidth,
      int tileHeight,
      Tiler.EdgeBehavior edges)
    {
      int length1 = bits.GetLength(0);
      int length2 = bits.GetLength(1);
      int[,] numArray = new int[length1, length2];
      for (Tiler.TileX = 0; Tiler.TileX < length1; ++Tiler.TileX)
      {
        for (Tiler.TileY = 0; Tiler.TileY < length2; ++Tiler.TileY)
        {
          if (bits[Tiler.TileX, Tiler.TileY])
          {
            switch (edges)
            {
              case Tiler.EdgeBehavior.True:
                Tiler.Left = Tiler.TileX == 0 || (bits[Tiler.TileX - 1, Tiler.TileY] || mask[Tiler.TileX - 1, Tiler.TileY]);
                Tiler.Right = Tiler.TileX == length1 - 1 || (bits[Tiler.TileX + 1, Tiler.TileY] || mask[Tiler.TileX + 1, Tiler.TileY]);
                Tiler.Up = Tiler.TileY == 0 || (bits[Tiler.TileX, Tiler.TileY - 1] || mask[Tiler.TileX, Tiler.TileY - 1]);
                Tiler.Down = Tiler.TileY == length2 - 1 || (bits[Tiler.TileX, Tiler.TileY + 1] || mask[Tiler.TileX, Tiler.TileY + 1]);
                Tiler.UpLeft = Tiler.TileX == 0 || Tiler.TileY == 0 || (bits[Tiler.TileX - 1, Tiler.TileY - 1] || mask[Tiler.TileX - 1, Tiler.TileY - 1]);
                Tiler.UpRight = Tiler.TileX == length1 - 1 || Tiler.TileY == 0 || (bits[Tiler.TileX + 1, Tiler.TileY - 1] || mask[Tiler.TileX + 1, Tiler.TileY - 1]);
                Tiler.DownLeft = Tiler.TileX == 0 || Tiler.TileY == length2 - 1 || (bits[Tiler.TileX - 1, Tiler.TileY + 1] || mask[Tiler.TileX - 1, Tiler.TileY + 1]);
                Tiler.DownRight = Tiler.TileX == length1 - 1 || Tiler.TileY == length2 - 1 || (bits[Tiler.TileX + 1, Tiler.TileY + 1] || mask[Tiler.TileX + 1, Tiler.TileY + 1]);
                break;
              case Tiler.EdgeBehavior.False:
                Tiler.Left = Tiler.TileX != 0 && (bits[Tiler.TileX - 1, Tiler.TileY] || mask[Tiler.TileX - 1, Tiler.TileY]);
                Tiler.Right = Tiler.TileX != length1 - 1 && (bits[Tiler.TileX + 1, Tiler.TileY] || mask[Tiler.TileX + 1, Tiler.TileY]);
                Tiler.Up = Tiler.TileY != 0 && (bits[Tiler.TileX, Tiler.TileY - 1] || mask[Tiler.TileX, Tiler.TileY - 1]);
                Tiler.Down = Tiler.TileY != length2 - 1 && (bits[Tiler.TileX, Tiler.TileY + 1] || mask[Tiler.TileX, Tiler.TileY + 1]);
                Tiler.UpLeft = Tiler.TileX != 0 && Tiler.TileY != 0 && (bits[Tiler.TileX - 1, Tiler.TileY - 1] || mask[Tiler.TileX - 1, Tiler.TileY - 1]);
                Tiler.UpRight = Tiler.TileX != length1 - 1 && Tiler.TileY != 0 && (bits[Tiler.TileX + 1, Tiler.TileY - 1] || mask[Tiler.TileX + 1, Tiler.TileY - 1]);
                Tiler.DownLeft = Tiler.TileX != 0 && Tiler.TileY != length2 - 1 && (bits[Tiler.TileX - 1, Tiler.TileY + 1] || mask[Tiler.TileX - 1, Tiler.TileY + 1]);
                Tiler.DownRight = Tiler.TileX != length1 - 1 && Tiler.TileY != length2 - 1 && (bits[Tiler.TileX + 1, Tiler.TileY + 1] || mask[Tiler.TileX + 1, Tiler.TileY + 1]);
                break;
              case Tiler.EdgeBehavior.Wrap:
                Tiler.Left = bits[(Tiler.TileX + length1 - 1) % length1, Tiler.TileY] || mask[(Tiler.TileX + length1 - 1) % length1, Tiler.TileY];
                Tiler.Right = bits[(Tiler.TileX + 1) % length1, Tiler.TileY] || mask[(Tiler.TileX + 1) % length1, Tiler.TileY];
                Tiler.Up = bits[Tiler.TileX, (Tiler.TileY + length2 - 1) % length2] || mask[Tiler.TileX, (Tiler.TileY + length2 - 1) % length2];
                Tiler.Down = bits[Tiler.TileX, (Tiler.TileY + 1) % length2] || mask[Tiler.TileX, (Tiler.TileY + 1) % length2];
                Tiler.UpLeft = bits[(Tiler.TileX + length1 - 1) % length1, (Tiler.TileY + length2 - 1) % length2] || mask[(Tiler.TileX + length1 - 1) % length1, (Tiler.TileY + length2 - 1) % length2];
                Tiler.UpRight = bits[(Tiler.TileX + 1) % length1, (Tiler.TileY + length2 - 1) % length2] || mask[(Tiler.TileX + 1) % length1, (Tiler.TileY + length2 - 1) % length2];
                Tiler.DownLeft = bits[(Tiler.TileX + length1 - 1) % length1, (Tiler.TileY + 1) % length2] || mask[(Tiler.TileX + length1 - 1) % length1, (Tiler.TileY + 1) % length2];
                Tiler.DownRight = bits[(Tiler.TileX + 1) % length1, (Tiler.TileY + 1) % length2] || mask[(Tiler.TileX + 1) % length1, (Tiler.TileY + 1) % length2];
                break;
            }
            int num = tileDecider();
            tileOutput(num);
            numArray[Tiler.TileX, Tiler.TileY] = num;
          }
        }
      }
      return numArray;
    }

    public static int[,] Tile(
      bool[,] bits,
      AutotileData autotileData,
      Action<int> tileOutput,
      int tileWidth,
      int tileHeight,
      Tiler.EdgeBehavior edges)
    {
      return Tiler.Tile(bits, new Func<int>(autotileData.TileHandler), tileOutput, tileWidth, tileHeight, edges);
    }

    public static int[,] Tile(
      bool[,] bits,
      bool[,] mask,
      AutotileData autotileData,
      Action<int> tileOutput,
      int tileWidth,
      int tileHeight,
      Tiler.EdgeBehavior edges)
    {
      return Tiler.Tile(bits, mask, new Func<int>(autotileData.TileHandler), tileOutput, tileWidth, tileHeight, edges);
    }

    public static int TileX { get; private set; }

    public static int TileY { get; private set; }

    public static bool Left { get; private set; }

    public static bool Right { get; private set; }

    public static bool Up { get; private set; }

    public static bool Down { get; private set; }

    public static bool UpLeft { get; private set; }

    public static bool UpRight { get; private set; }

    public static bool DownLeft { get; private set; }

    public static bool DownRight { get; private set; }

    public enum EdgeBehavior
    {
      True,
      False,
      Wrap,
    }
  }
}
