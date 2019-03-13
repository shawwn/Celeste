// Decompiled with JetBrains decompiler
// Type: Celeste.CoverupWall
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  [Tracked(false)]
  public class CoverupWall : Entity
  {
    private char fillTile;
    private TileGrid tiles;
    private EffectCutout cutout;

    public CoverupWall(Vector2 position, char tile, float width, float height)
      : base(position)
    {
      this.fillTile = tile;
      this.Depth = -13000;
      this.Collider = (Collider) new Hitbox(width, height, 0.0f, 0.0f);
      this.Add((Component) (this.cutout = new EffectCutout()));
    }

    public CoverupWall(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Char("tiletype", '3'), (float) data.Width, (float) data.Height)
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      int tilesX = (int) this.Width / 8;
      int tilesY = (int) this.Height / 8;
      Level level = this.SceneAs<Level>();
      Rectangle tileBounds = level.Session.MapData.TileBounds;
      VirtualMap<char> solidsData = level.SolidsData;
      int x = (int) this.X / 8 - tileBounds.Left;
      int y = (int) this.Y / 8 - tileBounds.Top;
      this.Add((Component) (this.tiles = GFX.FGAutotiler.GenerateOverlay(this.fillTile, x, y, tilesX, tilesY, solidsData).TileGrid));
      this.Add((Component) new TileInterceptor(this.tiles, false));
    }
  }
}

