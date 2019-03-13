// Decompiled with JetBrains decompiler
// Type: Celeste.DashBlock
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  [Tracked(false)]
  public class DashBlock : Solid
  {
    private bool permanent;
    private EntityID id;
    private char tileType;
    private float width;
    private float height;
    private bool blendIn;
    private bool canDash;

    public DashBlock(
      Vector2 position,
      char tiletype,
      float width,
      float height,
      bool blendIn,
      bool permanent,
      bool canDash,
      EntityID id)
      : base(position, width, height, true)
    {
      this.Depth = -12999;
      this.id = id;
      this.permanent = permanent;
      this.width = width;
      this.height = height;
      this.blendIn = blendIn;
      this.canDash = canDash;
      this.tileType = tiletype;
      this.OnDashCollide = new DashCollision(this.OnDashed);
      this.SurfaceSoundIndex = SurfaceIndex.TileToIndex[this.tileType];
    }

    public DashBlock(EntityData data, Vector2 offset, EntityID id)
      : this(data.Position + offset, data.Char("tiletype", '3'), (float) data.Width, (float) data.Height, data.Bool("blendin", false), data.Bool(nameof (permanent), true), data.Bool(nameof (canDash), true), id)
    {
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      TileGrid tileGrid;
      if (!this.blendIn)
      {
        tileGrid = GFX.FGAutotiler.GenerateBox(this.tileType, (int) this.width / 8, (int) this.height / 8).TileGrid;
        this.Add((Component) new LightOcclude(1f));
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
        this.Add((Component) new EffectCutout());
        this.Depth = -10501;
      }
      this.Add((Component) tileGrid);
      this.Add((Component) new TileInterceptor(tileGrid, true));
      if (!this.CollideCheck<Player>())
        return;
      this.RemoveSelf();
    }

    public override void Removed(Scene scene)
    {
      base.Removed(scene);
      Celeste.Freeze(0.05f);
    }

    public void Break(Vector2 from, Vector2 direction, bool playSound = true)
    {
      if (playSound)
      {
        if (this.tileType == '1')
          Audio.Play("event:/game/general/wall_break_dirt", this.Position);
        else if (this.tileType == '3')
          Audio.Play("event:/game/general/wall_break_ice", this.Position);
        else if (this.tileType == '9')
          Audio.Play("event:/game/general/wall_break_wood", this.Position);
        else
          Audio.Play("event:/game/general/wall_break_stone", this.Position);
      }
      for (int index1 = 0; (double) index1 < (double) this.Width / 8.0; ++index1)
      {
        for (int index2 = 0; (double) index2 < (double) this.Height / 8.0; ++index2)
          this.Scene.Add((Entity) Engine.Pooler.Create<Debris>().Init(this.Position + new Vector2((float) (4 + index1 * 8), (float) (4 + index2 * 8)), this.tileType).BlastFrom(from));
      }
      this.Collidable = false;
      if (this.permanent)
        this.RemoveAndFlagAsGone();
      else
        this.RemoveSelf();
    }

    public void RemoveAndFlagAsGone()
    {
      this.RemoveSelf();
      this.SceneAs<Level>().Session.DoNotLoad.Add(this.id);
    }

    private DashCollisionResults OnDashed(Player player, Vector2 direction)
    {
      if (!this.canDash && player.StateMachine.State != 5 && player.StateMachine.State != 10)
        return DashCollisionResults.NormalCollision;
      this.Break(player.Center, direction, true);
      return DashCollisionResults.Rebound;
    }

    public enum Modes
    {
      Dash,
      FinalBoss,
      Crusher,
    }
  }
}

