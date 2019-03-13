// Decompiled with JetBrains decompiler
// Type: Monocle.TileGrid
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using System;

namespace Monocle
{
  public class TileGrid : Component
  {
    public Color Color = Color.get_White();
    public float Alpha = 1f;
    public Vector2 Position;
    public int VisualExtend;
    public VirtualMap<MTexture> Tiles;
    public Camera ClipCamera;

    public TileGrid(int tileWidth, int tileHeight, int tilesX, int tilesY)
      : base(false, true)
    {
      this.TileWidth = tileWidth;
      this.TileHeight = tileHeight;
      this.Tiles = new VirtualMap<MTexture>(tilesX, tilesY, (MTexture) null);
    }

    public int TileWidth { get; private set; }

    public int TileHeight { get; private set; }

    public int TilesX
    {
      get
      {
        return this.Tiles.Columns;
      }
    }

    public int TilesY
    {
      get
      {
        return this.Tiles.Rows;
      }
    }

    public void Populate(Tileset tileset, int[,] tiles, int offsetX = 0, int offsetY = 0)
    {
      for (int index1 = 0; index1 < tiles.GetLength(0) && index1 + offsetX < this.TilesX; ++index1)
      {
        for (int index2 = 0; index2 < tiles.GetLength(1) && index2 + offsetY < this.TilesY; ++index2)
          this.Tiles[index1 + offsetX, index2 + offsetY] = tileset[tiles[index1, index2]];
      }
    }

    public void Overlay(Tileset tileset, int[,] tiles, int offsetX = 0, int offsetY = 0)
    {
      for (int index1 = 0; index1 < tiles.GetLength(0) && index1 + offsetX < this.TilesX; ++index1)
      {
        for (int index2 = 0; index2 < tiles.GetLength(1) && index2 + offsetY < this.TilesY; ++index2)
        {
          if (tiles[index1, index2] >= 0)
            this.Tiles[index1 + offsetX, index2 + offsetY] = tileset[tiles[index1, index2]];
        }
      }
    }

    public void Extend(int left, int right, int up, int down)
    {
      this.Position = Vector2.op_Subtraction(this.Position, new Vector2((float) (left * this.TileWidth), (float) (up * this.TileHeight)));
      int columns = this.TilesX + left + right;
      int rows = this.TilesY + up + down;
      if (columns <= 0 || rows <= 0)
      {
        this.Tiles = new VirtualMap<MTexture>(0, 0, (MTexture) null);
      }
      else
      {
        VirtualMap<MTexture> virtualMap = new VirtualMap<MTexture>(columns, rows, (MTexture) null);
        for (int index1 = 0; index1 < this.TilesX; ++index1)
        {
          for (int index2 = 0; index2 < this.TilesY; ++index2)
          {
            int index3 = index1 + left;
            int index4 = index2 + up;
            if (index3 >= 0 && index3 < columns && (index4 >= 0 && index4 < rows))
              virtualMap[index3, index4] = this.Tiles[index1, index2];
          }
        }
        for (int index1 = 0; index1 < left; ++index1)
        {
          for (int index2 = 0; index2 < rows; ++index2)
            virtualMap[index1, index2] = this.Tiles[0, Calc.Clamp(index2 - up, 0, this.TilesY - 1)];
        }
        for (int index1 = columns - right; index1 < columns; ++index1)
        {
          for (int index2 = 0; index2 < rows; ++index2)
            virtualMap[index1, index2] = this.Tiles[this.TilesX - 1, Calc.Clamp(index2 - up, 0, this.TilesY - 1)];
        }
        for (int index1 = 0; index1 < up; ++index1)
        {
          for (int index2 = 0; index2 < columns; ++index2)
            virtualMap[index2, index1] = this.Tiles[Calc.Clamp(index2 - left, 0, this.TilesX - 1), 0];
        }
        for (int index1 = rows - down; index1 < rows; ++index1)
        {
          for (int index2 = 0; index2 < columns; ++index2)
            virtualMap[index2, index1] = this.Tiles[Calc.Clamp(index2 - left, 0, this.TilesX - 1), this.TilesY - 1];
        }
        this.Tiles = virtualMap;
      }
    }

    public void FillRect(int x, int y, int columns, int rows, MTexture tile)
    {
      int num1 = Math.Max(0, x);
      int num2 = Math.Max(0, y);
      int num3 = Math.Min(this.TilesX, x + columns);
      int num4 = Math.Min(this.TilesY, y + rows);
      for (int index1 = num1; index1 < num3; ++index1)
      {
        for (int index2 = num2; index2 < num4; ++index2)
          this.Tiles[index1, index2] = tile;
      }
    }

    public void Clear()
    {
      for (int index1 = 0; index1 < this.TilesX; ++index1)
      {
        for (int index2 = 0; index2 < this.TilesY; ++index2)
          this.Tiles[index1, index2] = (MTexture) null;
      }
    }

    public Rectangle GetClippedRenderTiles()
    {
      Vector2 vector2 = Vector2.op_Addition(this.Entity.Position, this.Position);
      int val1_1;
      int val1_2;
      int val1_3;
      int val1_4;
      if (this.ClipCamera == null)
      {
        val1_1 = -this.VisualExtend;
        val1_2 = -this.VisualExtend;
        val1_3 = this.TilesX + this.VisualExtend;
        val1_4 = this.TilesY + this.VisualExtend;
      }
      else
      {
        Camera clipCamera = this.ClipCamera;
        val1_1 = (int) Math.Max(0.0, Math.Floor(((double) clipCamera.Left - vector2.X) / (double) this.TileWidth) - (double) this.VisualExtend);
        val1_2 = (int) Math.Max(0.0, Math.Floor(((double) clipCamera.Top - vector2.Y) / (double) this.TileHeight) - (double) this.VisualExtend);
        val1_3 = (int) Math.Min((double) this.TilesX, Math.Ceiling(((double) clipCamera.Right - vector2.X) / (double) this.TileWidth) + (double) this.VisualExtend);
        val1_4 = (int) Math.Min((double) this.TilesY, Math.Ceiling(((double) clipCamera.Bottom - vector2.Y) / (double) this.TileHeight) + (double) this.VisualExtend);
      }
      int num1 = Math.Max(val1_1, 0);
      int num2 = Math.Max(val1_2, 0);
      int num3 = Math.Min(val1_3, this.TilesX);
      int num4 = Math.Min(val1_4, this.TilesY);
      return new Rectangle(num1, num2, num3 - num1, num4 - num2);
    }

    public override void Render()
    {
      this.RenderAt(Vector2.op_Addition(this.Entity.Position, this.Position));
    }

    public void RenderAt(Vector2 position)
    {
      if ((double) this.Alpha <= 0.0)
        return;
      Rectangle clippedRenderTiles = this.GetClippedRenderTiles();
      Color color = Color.op_Multiply(this.Color, this.Alpha);
      for (int left = ((Rectangle) ref clippedRenderTiles).get_Left(); left < ((Rectangle) ref clippedRenderTiles).get_Right(); ++left)
      {
        for (int top = ((Rectangle) ref clippedRenderTiles).get_Top(); top < ((Rectangle) ref clippedRenderTiles).get_Bottom(); ++top)
          this.Tiles[left, top]?.Draw(Vector2.op_Addition(position, new Vector2((float) (left * this.TileWidth), (float) (top * this.TileHeight))), Vector2.get_Zero(), color);
      }
    }
  }
}
