// Decompiled with JetBrains decompiler
// Type: Celeste.ClutterBlockGenerator
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public static class ClutterBlockGenerator
  {
    private static List<Point> active = new List<Point>();
    private static bool[] enabled = new bool[3];
    private static Level level;
    private static ClutterBlockGenerator.Tile[,] tiles;
    private static List<List<ClutterBlockGenerator.TextureSet>> textures;
    private static int columns;
    private static int rows;
    private static bool initialized;

    public static void Init(Level lvl)
    {
      if (ClutterBlockGenerator.initialized)
        return;
      ClutterBlockGenerator.initialized = true;
      ClutterBlockGenerator.level = lvl;
      ClutterBlockGenerator.columns = ClutterBlockGenerator.level.Bounds.Width / 8;
      ClutterBlockGenerator.rows = ClutterBlockGenerator.level.Bounds.Height / 8 + 1;
      if (ClutterBlockGenerator.tiles == null)
        ClutterBlockGenerator.tiles = new ClutterBlockGenerator.Tile[200, 200];
      for (int index1 = 0; index1 < ClutterBlockGenerator.columns; ++index1)
      {
        for (int index2 = 0; index2 < ClutterBlockGenerator.rows; ++index2)
        {
          ClutterBlockGenerator.tiles[index1, index2].Color = -1;
          ClutterBlockGenerator.tiles[index1, index2].Block = (ClutterBlock) null;
        }
      }
      for (int index = 0; index < ClutterBlockGenerator.enabled.Length; ++index)
        ClutterBlockGenerator.enabled[index] = !ClutterBlockGenerator.level.Session.GetFlag("oshiro_clutter_cleared_" + (object) index);
      if (ClutterBlockGenerator.textures == null)
      {
        ClutterBlockGenerator.textures = new List<List<ClutterBlockGenerator.TextureSet>>();
        for (int index = 0; index < 3; ++index)
        {
          List<ClutterBlockGenerator.TextureSet> textureSetList1 = new List<ClutterBlockGenerator.TextureSet>();
          foreach (MTexture atlasSubtexture in GFX.Game.GetAtlasSubtextures("objects/resortclutter/" + ((ClutterBlock.Colors) index).ToString() + "_"))
          {
            int num1 = atlasSubtexture.Width / 8;
            int num2 = atlasSubtexture.Height / 8;
            ClutterBlockGenerator.TextureSet textureSet1 = (ClutterBlockGenerator.TextureSet) null;
            foreach (ClutterBlockGenerator.TextureSet textureSet2 in textureSetList1)
            {
              if (textureSet2.Columns == num1 && textureSet2.Rows == num2)
              {
                textureSet1 = textureSet2;
                break;
              }
            }
            if (textureSet1 == null)
            {
              List<ClutterBlockGenerator.TextureSet> textureSetList2 = textureSetList1;
              ClutterBlockGenerator.TextureSet textureSet2 = new ClutterBlockGenerator.TextureSet();
              textureSet2.Columns = num1;
              textureSet2.Rows = num2;
              textureSet1 = textureSet2;
              textureSetList2.Add(textureSet2);
            }
            textureSet1.textures.Add(atlasSubtexture);
          }
          textureSetList1.Sort((Comparison<ClutterBlockGenerator.TextureSet>) ((a, b) => -Math.Sign(a.Columns * a.Rows - b.Columns * b.Rows)));
          ClutterBlockGenerator.textures.Add(textureSetList1);
        }
      }
      Point levelSolidOffset = ClutterBlockGenerator.level.LevelSolidOffset;
      for (int index1 = 0; index1 < ClutterBlockGenerator.columns; ++index1)
      {
        for (int index2 = 0; index2 < ClutterBlockGenerator.rows; ++index2)
          ClutterBlockGenerator.tiles[index1, index2].Wall = ClutterBlockGenerator.level.SolidsData[levelSolidOffset.X + index1, levelSolidOffset.Y + index2] != '0';
      }
    }

    public static void Dispose()
    {
      ClutterBlockGenerator.textures = (List<List<ClutterBlockGenerator.TextureSet>>) null;
      ClutterBlockGenerator.tiles = (ClutterBlockGenerator.Tile[,]) null;
      ClutterBlockGenerator.initialized = false;
    }

    public static void Add(int x, int y, int w, int h, ClutterBlock.Colors color)
    {
      ClutterBlockGenerator.level.Add((Entity) new ClutterBlockBase(new Vector2((float) ClutterBlockGenerator.level.Bounds.X, (float) ClutterBlockGenerator.level.Bounds.Y) + new Vector2((float) x, (float) y) * 8f, w * 8, h * 8, ClutterBlockGenerator.enabled[(int) color], color));
      if (!ClutterBlockGenerator.enabled[(int) color])
        return;
      int x1 = Math.Max(0, x);
      for (int index1 = Math.Min(ClutterBlockGenerator.columns, x + w); x1 < index1; ++x1)
      {
        int y1 = Math.Max(0, y);
        for (int index2 = Math.Min(ClutterBlockGenerator.rows, y + h); y1 < index2; ++y1)
        {
          Point point = new Point(x1, y1);
          ClutterBlockGenerator.tiles[point.X, point.Y].Color = (int) color;
          ClutterBlockGenerator.active.Add(point);
        }
      }
    }

    public static void Generate()
    {
      if (!ClutterBlockGenerator.initialized)
        return;
      ClutterBlockGenerator.active.Shuffle<Point>();
      List<ClutterBlock> clutterBlockList = new List<ClutterBlock>();
      Rectangle bounds = ClutterBlockGenerator.level.Bounds;
      foreach (Point point in ClutterBlockGenerator.active)
      {
        if (ClutterBlockGenerator.tiles[point.X, point.Y].Block == null)
        {
          int index1 = 0;
          int color;
          ClutterBlockGenerator.TextureSet textureSet;
          while (true)
          {
            color = ClutterBlockGenerator.tiles[point.X, point.Y].Color;
            textureSet = ClutterBlockGenerator.textures[color][index1];
            bool flag = true;
            if (point.X + textureSet.Columns <= ClutterBlockGenerator.columns && point.Y + textureSet.Rows <= ClutterBlockGenerator.rows)
            {
              int x = point.X;
              for (int index2 = point.X + textureSet.Columns; flag && x < index2; ++x)
              {
                int y = point.Y;
                for (int index3 = point.Y + textureSet.Rows; flag && y < index3; ++y)
                {
                  ClutterBlockGenerator.Tile tile = ClutterBlockGenerator.tiles[x, y];
                  if (tile.Block != null || tile.Color != color)
                    flag = false;
                }
              }
              if (flag)
                break;
            }
            ++index1;
          }
          ClutterBlock clutterBlock = new ClutterBlock(new Vector2((float) bounds.X, (float) bounds.Y) + new Vector2((float) point.X, (float) point.Y) * 8f, Calc.Random.Choose<MTexture>(textureSet.textures), (ClutterBlock.Colors) color);
          for (int x = point.X; x < point.X + textureSet.Columns; ++x)
          {
            for (int y = point.Y; y < point.Y + textureSet.Rows; ++y)
              ClutterBlockGenerator.tiles[x, y].Block = clutterBlock;
          }
          clutterBlockList.Add(clutterBlock);
          ClutterBlockGenerator.level.Add((Entity) clutterBlock);
        }
      }
      for (int index1 = 0; index1 < ClutterBlockGenerator.columns; ++index1)
      {
        for (int index2 = 0; index2 < ClutterBlockGenerator.rows; ++index2)
        {
          ClutterBlockGenerator.Tile tile1 = ClutterBlockGenerator.tiles[index1, index2];
          if (tile1.Block != null)
          {
            ClutterBlock block = tile1.Block;
            if (!block.TopSideOpen && (index2 == 0 || ClutterBlockGenerator.tiles[index1, index2 - 1].Empty))
              block.TopSideOpen = true;
            if (!block.LeftSideOpen && (index1 == 0 || ClutterBlockGenerator.tiles[index1 - 1, index2].Empty))
              block.LeftSideOpen = true;
            if (!block.RightSideOpen && (index1 == ClutterBlockGenerator.columns - 1 || ClutterBlockGenerator.tiles[index1 + 1, index2].Empty))
              block.RightSideOpen = true;
            if (!block.OnTheGround && index2 < ClutterBlockGenerator.rows - 1)
            {
              ClutterBlockGenerator.Tile tile2 = ClutterBlockGenerator.tiles[index1, index2 + 1];
              if (tile2.Wall)
                block.OnTheGround = true;
              else if (tile2.Block != null && tile2.Block != block && !block.HasBelow.Contains(tile2.Block))
              {
                block.HasBelow.Add(tile2.Block);
                block.Below.Add(tile2.Block);
                tile2.Block.Above.Add(block);
              }
            }
          }
        }
      }
      foreach (ClutterBlock block in clutterBlockList)
      {
        if (block.OnTheGround)
          ClutterBlockGenerator.SetAboveToOnGround(block);
      }
      ClutterBlockGenerator.initialized = false;
      ClutterBlockGenerator.level = (Level) null;
      ClutterBlockGenerator.active.Clear();
    }

    private static void SetAboveToOnGround(ClutterBlock block)
    {
      foreach (ClutterBlock block1 in block.Above)
      {
        if (!block1.OnTheGround)
        {
          block1.OnTheGround = true;
          ClutterBlockGenerator.SetAboveToOnGround(block1);
        }
      }
    }

    private struct Tile
    {
      public int Color;
      public bool Wall;
      public ClutterBlock Block;

      public bool Empty
      {
        get
        {
          return !this.Wall && this.Color == -1;
        }
      }
    }

    private class TextureSet
    {
      public List<MTexture> textures = new List<MTexture>();
      public int Columns;
      public int Rows;
    }
  }
}

