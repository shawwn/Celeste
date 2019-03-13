// Decompiled with JetBrains decompiler
// Type: Celeste.SolidTiles
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class SolidTiles : Solid
  {
    public TileGrid Tiles;
    public AnimatedTiles AnimatedTiles;
    public Grid Grid;
    private VirtualMap<char> tileTypes;

    public SolidTiles(Vector2 position, VirtualMap<char> data)
      : base(position, 0.0f, 0.0f, true)
    {
      this.Tag = (int) Tags.Global;
      this.Depth = -10000;
      this.tileTypes = data;
      this.EnableAssistModeChecks = false;
      this.Collider = (Collider) (this.Grid = new Grid(data.Columns, data.Rows, 8f, 8f));
      for (int x = 0; x < data.Columns; x += 50)
      {
        for (int y = 0; y < data.Rows; y += 50)
        {
          if (data.AnyInSegmentAtTile(x, y))
          {
            int index1 = x;
            for (int index2 = Math.Min(index1 + 50, data.Columns); index1 < index2; ++index1)
            {
              int index3 = y;
              for (int index4 = Math.Min(index3 + 50, data.Rows); index3 < index4; ++index3)
              {
                if (data[index1, index3] != '0')
                  this.Grid[index1, index3] = true;
              }
            }
          }
        }
      }
      Autotiler.Generated map = GFX.FGAutotiler.GenerateMap(data, true);
      this.Tiles = map.TileGrid;
      this.Tiles.VisualExtend = 1;
      this.Add((Component) this.Tiles);
      this.Add((Component) (this.AnimatedTiles = map.SpriteOverlay));
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.Tiles.ClipCamera = this.SceneAs<Level>().Camera;
      this.AnimatedTiles.ClipCamera = this.Tiles.ClipCamera;
    }

    private int CoreTileSurfaceIndex()
    {
      Level scene = this.Scene as Level;
      if (scene.CoreMode == Session.CoreModes.Hot)
        return 37;
      return scene.CoreMode == Session.CoreModes.Cold ? 36 : 3;
    }

    private int SurfaceSoundIndexAt(Vector2 readPosition)
    {
      int index1 = (int) (((double) readPosition.X - (double) this.X) / 8.0);
      int index2 = (int) (((double) readPosition.Y - (double) this.Y) / 8.0);
      if (index1 >= 0 && index2 >= 0 && index1 < this.Grid.CellsX && index2 < this.Grid.CellsY)
      {
        char tileType = this.tileTypes[index1, index2];
        int num;
        switch (tileType)
        {
          case '0':
            num = 0;
            break;
          case 'k':
            return this.CoreTileSurfaceIndex();
          default:
            num = SurfaceIndex.TileToIndex.ContainsKey(tileType) ? 1 : 0;
            break;
        }
        if (num != 0)
          return SurfaceIndex.TileToIndex[tileType];
      }
      return -1;
    }

    public override int GetWallSoundIndex(Player player, int side)
    {
      int num = this.SurfaceSoundIndexAt(player.Center + Vector2.UnitX * (float) side * 8f);
      if (num < 0)
        num = this.SurfaceSoundIndexAt(player.Center + new Vector2((float) (side * 8), -6f));
      if (num < 0)
        num = this.SurfaceSoundIndexAt(player.Center + new Vector2((float) (side * 8), 6f));
      return num;
    }

    public override int GetStepSoundIndex(Entity entity)
    {
      int num = this.SurfaceSoundIndexAt(entity.BottomCenter + Vector2.UnitY * 4f);
      if (num == -1)
        num = this.SurfaceSoundIndexAt(entity.BottomLeft + Vector2.UnitY * 4f);
      if (num == -1)
        num = this.SurfaceSoundIndexAt(entity.BottomRight + Vector2.UnitY * 4f);
      return num;
    }

    public override int GetLandSoundIndex(Entity entity)
    {
      int num = this.SurfaceSoundIndexAt(entity.BottomCenter + Vector2.UnitY * 4f);
      if (num == -1)
        num = this.SurfaceSoundIndexAt(entity.BottomLeft + Vector2.UnitY * 4f);
      if (num == -1)
        num = this.SurfaceSoundIndexAt(entity.BottomRight + Vector2.UnitY * 4f);
      return num;
    }
  }
}

