// Decompiled with JetBrains decompiler
// Type: Celeste.WallBooster
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(false)]
  public class WallBooster : Entity
  {
    public Facings Facing;
    private StaticMover staticMover;
    private ClimbBlocker climbBlocker;
    private SoundSource idleSfx;
    public bool IceMode;
    private List<Sprite> tiles;

    public WallBooster(Vector2 position, float height, bool left)
      : base(position)
    {
      this.Depth = 1999;
      if (left)
      {
        this.Facing = Facings.Left;
        this.Collider = (Collider) new Hitbox(2f, height, 0.0f, 0.0f);
      }
      else
      {
        this.Facing = Facings.Right;
        this.Collider = (Collider) new Hitbox(2f, height, 6f, 0.0f);
      }
      this.Add((Component) new CoreModeListener(new Action<Session.CoreModes>(this.OnChangeMode)));
      this.Add((Component) (this.staticMover = new StaticMover()));
      this.Add((Component) (this.climbBlocker = new ClimbBlocker()));
      this.Add((Component) (this.idleSfx = new SoundSource()));
      this.tiles = this.BuildSprite(left);
    }

    public WallBooster(EntityData data, Vector2 offset)
      : this(data.Position + offset, (float) data.Height, data.Bool("left", false))
    {
    }

    private List<Sprite> BuildSprite(bool left)
    {
      List<Sprite> spriteList = new List<Sprite>();
      for (int index = 0; (double) index < (double) this.Height; index += 8)
      {
        string id = index != 0 ? ((double) (index + 16) <= (double) this.Height ? "WallBoosterMid" : "WallBoosterBottom") : "WallBoosterTop";
        Sprite sprite = GFX.SpriteBank.Create(id);
        if (!left)
        {
          sprite.FlipX = true;
          sprite.Position = new Vector2(4f, (float) index);
        }
        else
          sprite.Position = new Vector2(0.0f, (float) index);
        spriteList.Add(sprite);
        this.Add((Component) sprite);
      }
      return spriteList;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.IceMode = this.SceneAs<Level>().CoreMode == Session.CoreModes.Cold;
      this.climbBlocker.Blocking = this.IceMode;
      this.tiles.ForEach((Action<Sprite>) (t => t.Play(this.IceMode ? "ice" : "hot", false, false)));
    }

    private void OnChangeMode(Session.CoreModes mode)
    {
      this.IceMode = mode == Session.CoreModes.Cold;
      this.climbBlocker.Blocking = this.IceMode;
      this.tiles.ForEach((Action<Sprite>) (t => t.Play(this.IceMode ? "ice" : "hot", false, false)));
      if (this.IceMode)
        this.idleSfx.Stop(true);
      else
        this.idleSfx.Play("event:/env/local/09_core/conveyor_idle", (string) null, 0.0f);
    }

    public override void Update()
    {
      base.Update();
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity == null)
        return;
      this.idleSfx.Position = Calc.ClosestPointOnLine(this.Position, this.Position + new Vector2(0.0f, this.Height), entity.Center) - this.Position;
    }
  }
}

