// Decompiled with JetBrains decompiler
// Type: Celeste.Bridge
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  public class Bridge : Entity
  {
    private List<Rectangle> tileSizes = new List<Rectangle>();
    private bool ending = false;
    private List<BridgeTile> tiles;
    private Level level;
    private bool canCollapse;
    private bool canEndCollapseA;
    private bool canEndCollapseB;
    private float collapseTimer;
    private int width;
    private float gapStartX;
    private float gapEndX;
    private SoundSource collapseSfx;

    public Bridge(Vector2 position, int width, float gapStartX, float gapEndX)
      : base(position)
    {
      this.width = width;
      this.gapStartX = gapStartX;
      this.gapEndX = gapEndX;
      this.tileSizes.Add(new Rectangle(0, 0, 16, 52));
      this.tileSizes.Add(new Rectangle(16, 0, 8, 52));
      this.tileSizes.Add(new Rectangle(24, 0, 8, 52));
      this.tileSizes.Add(new Rectangle(32, 0, 8, 52));
      this.tileSizes.Add(new Rectangle(40, 0, 8, 52));
      this.tileSizes.Add(new Rectangle(48, 0, 8, 52));
      this.tileSizes.Add(new Rectangle(56, 0, 8, 52));
      this.tileSizes.Add(new Rectangle(64, 0, 8, 52));
      this.tileSizes.Add(new Rectangle(72, 0, 8, 52));
      this.tileSizes.Add(new Rectangle(80, 0, 16, 52));
      this.tileSizes.Add(new Rectangle(96, 0, 8, 52));
      this.Add((Component) (this.collapseSfx = new SoundSource()));
    }

    public Bridge(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Width, data.Nodes[0].X, data.Nodes[1].X)
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.level = scene as Level;
      this.tiles = new List<BridgeTile>();
      this.gapStartX += (float) this.level.Bounds.Left;
      this.gapEndX += (float) this.level.Bounds.Left;
      Calc.PushRandom(1);
      Vector2 position = this.Position;
      int index = 0;
      while ((double) position.X < (double) this.X + (double) this.width)
      {
        Rectangle tileSize = index < 2 || index > 7 ? this.tileSizes[index] : this.tileSizes[2 + Calc.Random.Next(6)];
        if ((double) position.X < (double) this.gapStartX || (double) position.X >= (double) this.gapEndX)
        {
          BridgeTile bridgeTile = new BridgeTile(position, tileSize);
          this.tiles.Add(bridgeTile);
          this.level.Add((Entity) bridgeTile);
        }
        position.X += (float) tileSize.Width;
        index = (index + 1) % this.tileSizes.Count;
      }
      Calc.PopRandom();
    }

    public override void Update()
    {
      base.Update();
      Player entity = this.level.Tracker.GetEntity<Player>();
      if (entity == null || entity.Dead)
        this.collapseSfx.Stop(true);
      if (!this.canCollapse)
      {
        if (entity == null || (double) entity.X < (double) this.X + 112.0)
          return;
        Audio.SetMusic("event:/music/lvl0/bridge", true, true);
        this.collapseSfx.Play("event:/game/00_prologue/bridge_rumble_loop", (string) null, 0.0f);
        this.canCollapse = true;
        this.canEndCollapseA = true;
        this.canEndCollapseB = true;
        for (int index = 0; index < 11; ++index)
        {
          this.tiles[0].Fall(Calc.Random.Range(0.1f, 0.5f));
          this.tiles.RemoveAt(0);
        }
      }
      else if (this.tiles.Count > 0)
      {
        if (entity == null)
          return;
        if (this.canEndCollapseA && (double) entity.X > (double) this.X + (double) this.width - 216.0)
        {
          this.canEndCollapseA = false;
          for (int index = 0; index < 5; ++index)
          {
            this.tiles[this.tiles.Count - 8].Fall(Calc.Random.Range(0.1f, 0.5f));
            this.tiles.RemoveAt(this.tiles.Count - 8);
          }
        }
        else if (this.canEndCollapseB && (double) entity.X > (double) this.X + (double) this.width - 104.0)
        {
          this.canEndCollapseB = false;
          for (int index = 0; index < 7 && this.tiles.Count > 0; ++index)
          {
            this.tiles[this.tiles.Count - 1].Fall(Calc.Random.Range(0.1f, 0.3f));
            this.tiles.RemoveAt(this.tiles.Count - 1);
          }
        }
        else if ((double) this.collapseTimer > 0.0)
        {
          this.collapseTimer -= Engine.DeltaTime;
          if (this.tiles.Count >= 5 && (double) entity.X >= (double) this.tiles[4].X)
          {
            int index = 0;
            this.tiles[index].Fall(0.2f);
            this.tiles.RemoveAt(index);
          }
        }
        else
        {
          this.tiles[0].Fall(0.2f);
          this.tiles.RemoveAt(0);
          this.collapseTimer = 0.2f;
        }
      }
      else
      {
        if (this.ending)
          return;
        this.ending = true;
      }
    }

    public void StopCollapseLoop()
    {
      this.collapseSfx.Stop(true);
    }
  }
}

