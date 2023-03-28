// Decompiled with JetBrains decompiler
// Type: Celeste.WallBooster
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

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
    private bool notCoreMode;
    private List<Sprite> tiles;

    public WallBooster(Vector2 position, float height, bool left, bool notCoreMode)
      : base(position)
    {
      this.Tag = (int) Tags.TransitionUpdate;
      this.Depth = 1999;
      this.notCoreMode = notCoreMode;
      if (left)
      {
        this.Facing = Facings.Left;
        this.Collider = (Collider) new Hitbox(2f, height);
      }
      else
      {
        this.Facing = Facings.Right;
        this.Collider = (Collider) new Hitbox(2f, height, 6f);
      }
      this.Add((Component) new CoreModeListener(new Action<Session.CoreModes>(this.OnChangeMode)));
      this.Add((Component) (this.staticMover = new StaticMover()));
      this.Add((Component) (this.climbBlocker = new ClimbBlocker(false)));
      this.Add((Component) (this.idleSfx = new SoundSource()));
      this.tiles = this.BuildSprite(left);
    }

    public WallBooster(EntityData data, Vector2 offset)
      : this(data.Position + offset, (float) data.Height, data.Bool("left"), data.Bool(nameof (notCoreMode)))
    {
    }

    private List<Sprite> BuildSprite(bool left)
    {
      List<Sprite> spriteList = new List<Sprite>();
      for (int y = 0; (double) y < (double) this.Height; y += 8)
      {
        string id = y != 0 ? ((double) (y + 16) <= (double) this.Height ? "WallBoosterMid" : "WallBoosterBottom") : "WallBoosterTop";
        Sprite sprite = GFX.SpriteBank.Create(id);
        if (!left)
        {
          sprite.FlipX = true;
          sprite.Position = new Vector2(4f, (float) y);
        }
        else
          sprite.Position = new Vector2(0.0f, (float) y);
        spriteList.Add(sprite);
        this.Add((Component) sprite);
      }
      return spriteList;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      Session.CoreModes mode = Session.CoreModes.None;
      if (this.SceneAs<Level>().CoreMode == Session.CoreModes.Cold || this.notCoreMode)
        mode = Session.CoreModes.Cold;
      this.OnChangeMode(mode);
    }

    private void OnChangeMode(Session.CoreModes mode)
    {
      this.IceMode = mode == Session.CoreModes.Cold;
      this.climbBlocker.Blocking = this.IceMode;
      this.tiles.ForEach((Action<Sprite>) (t => t.Play(this.IceMode ? "ice" : "hot")));
      if (this.IceMode)
      {
        this.idleSfx.Stop();
      }
      else
      {
        if (this.idleSfx.Playing)
          return;
        this.idleSfx.Play("event:/env/local/09_core/conveyor_idle");
      }
    }

    public override void Update()
    {
      this.PositionIdleSfx();
      if ((this.Scene as Level).Transitioning)
        return;
      base.Update();
    }

    private void PositionIdleSfx()
    {
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity == null)
        return;
      this.idleSfx.Position = Calc.ClosestPointOnLine(this.Position, this.Position + new Vector2(0.0f, this.Height), entity.Center) - this.Position;
      this.idleSfx.UpdateSfxPosition();
    }
  }
}
