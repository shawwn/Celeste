// Decompiled with JetBrains decompiler
// Type: Celeste.CrumbleWallOnRumble
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  [Tracked(false)]
  public class CrumbleWallOnRumble : Solid
  {
    private bool permanent;
    private EntityID id;
    private char tileType;
    private bool blendIn;

    public CrumbleWallOnRumble(
      Vector2 position,
      char tileType,
      float width,
      float height,
      bool blendIn,
      bool persistent,
      EntityID id)
      : base(position, width, height, true)
    {
      this.Depth = -12999;
      this.id = id;
      this.tileType = tileType;
      this.blendIn = blendIn;
      this.permanent = persistent;
      this.SurfaceSoundIndex = SurfaceIndex.TileToIndex[this.tileType];
    }

    public CrumbleWallOnRumble(EntityData data, Vector2 offset, EntityID id)
      : this(data.Position + offset, data.Char("tiletype", 'm'), (float) data.Width, (float) data.Height, data.Bool("blendin"), data.Bool("persistent"), id)
    {
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      TileGrid tileGrid;
      if (!this.blendIn)
      {
        tileGrid = GFX.FGAutotiler.GenerateBox(this.tileType, (int) this.Width / 8, (int) this.Height / 8).TileGrid;
      }
      else
      {
        Level level = this.SceneAs<Level>();
        Rectangle tileBounds = level.Session.MapData.TileBounds;
        VirtualMap<char> solidsData = level.SolidsData;
        int x = (int) ((double) this.X / 8.0) - tileBounds.Left;
        int y = (int) ((double) this.Y / 8.0) - tileBounds.Top;
        int tilesX = (int) this.Width / 8;
        int tilesY = (int) this.Height / 8;
        tileGrid = GFX.FGAutotiler.GenerateOverlay(this.tileType, x, y, tilesX, tilesY, solidsData).TileGrid;
        this.Depth = -10501;
      }
      this.Add((Component) tileGrid);
      this.Add((Component) new TileInterceptor(tileGrid, true));
      this.Add((Component) new LightOcclude());
      if (!this.CollideCheck<Player>())
        return;
      this.RemoveSelf();
    }

    public void Break()
    {
      if (!this.Collidable || this.Scene == null)
        return;
      Audio.Play("event:/new_content/game/10_farewell/quake_rockbreak", this.Position);
      this.Collidable = false;
      for (int index1 = 0; (double) index1 < (double) this.Width / 8.0; ++index1)
      {
        for (int index2 = 0; (double) index2 < (double) this.Height / 8.0; ++index2)
        {
          if (!this.Scene.CollideCheck<Solid>(new Rectangle((int) this.X + index1 * 8, (int) this.Y + index2 * 8, 8, 8)))
            this.Scene.Add((Entity) Engine.Pooler.Create<Debris>().Init(this.Position + new Vector2((float) (4 + index1 * 8), (float) (4 + index2 * 8)), this.tileType).BlastFrom(this.TopCenter));
        }
      }
      if (this.permanent)
        this.SceneAs<Level>().Session.DoNotLoad.Add(this.id);
      this.RemoveSelf();
    }
  }
}
