// Decompiled with JetBrains decompiler
// Type: Celeste.FakeWall
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class FakeWall : Entity
  {
    private FakeWall.Modes mode;
    private char fillTile;
    private TileGrid tiles;
    private bool fade;
    private EffectCutout cutout;
    private float transitionStartAlpha;
    private bool transitionFade;
    private EntityID eid;
    private bool playRevealWhenTransitionedInto;

    public FakeWall(
      EntityID eid,
      Vector2 position,
      char tile,
      float width,
      float height,
      FakeWall.Modes mode)
      : base(position)
    {
      this.mode = mode;
      this.eid = eid;
      this.fillTile = tile;
      this.Collider = (Collider) new Hitbox(width, height, 0.0f, 0.0f);
      this.Depth = -13000;
      this.Add((Component) (this.cutout = new EffectCutout()));
    }

    public FakeWall(EntityID eid, EntityData data, Vector2 offset, FakeWall.Modes mode)
      : this(eid, Vector2.op_Addition(data.Position, offset), data.Char("tiletype", '3'), (float) data.Width, (float) data.Height, mode)
    {
      this.playRevealWhenTransitionedInto = data.Bool("playTransitionReveal", false);
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      int tilesX = (int) this.Width / 8;
      int tilesY = (int) this.Height / 8;
      if (this.mode == FakeWall.Modes.Wall)
      {
        Level level = this.SceneAs<Level>();
        Rectangle tileBounds = level.Session.MapData.TileBounds;
        VirtualMap<char> solidsData = level.SolidsData;
        int x = (int) this.X / 8 - ((Rectangle) ref tileBounds).get_Left();
        int y = (int) this.Y / 8 - ((Rectangle) ref tileBounds).get_Top();
        this.tiles = GFX.FGAutotiler.GenerateOverlay(this.fillTile, x, y, tilesX, tilesY, solidsData).TileGrid;
      }
      else if (this.mode == FakeWall.Modes.Block)
        this.tiles = GFX.FGAutotiler.GenerateBox(this.fillTile, tilesX, tilesY).TileGrid;
      this.Add((Component) this.tiles);
      this.Add((Component) new TileInterceptor(this.tiles, false));
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (this.CollideCheck<Player>())
      {
        this.tiles.Alpha = 0.0f;
        this.fade = true;
        this.cutout.Visible = false;
        if (this.playRevealWhenTransitionedInto)
          Audio.Play("event:/game/general/secret_revealed", this.Center);
        this.SceneAs<Level>().Session.DoNotLoad.Add(this.eid);
      }
      else
        this.Add((Component) new TransitionListener()
        {
          OnOut = new Action<float>(this.OnTransitionOut),
          OnOutBegin = new Action(this.OnTransitionOutBegin),
          OnIn = new Action<float>(this.OnTransitionIn),
          OnInBegin = new Action(this.OnTransitionInBegin)
        });
    }

    private void OnTransitionOutBegin()
    {
      if (Collide.CheckRect((Entity) this, this.SceneAs<Level>().Bounds))
      {
        this.transitionFade = true;
        this.transitionStartAlpha = this.tiles.Alpha;
      }
      else
        this.transitionFade = false;
    }

    private void OnTransitionOut(float percent)
    {
      if (!this.transitionFade)
        return;
      this.tiles.Alpha = this.transitionStartAlpha * (1f - percent);
    }

    private void OnTransitionInBegin()
    {
      Level level = this.SceneAs<Level>();
      if (level.PreviousBounds.HasValue && Collide.CheckRect((Entity) this, level.PreviousBounds.Value))
      {
        this.transitionFade = true;
        this.tiles.Alpha = 0.0f;
      }
      else
        this.transitionFade = false;
    }

    private void OnTransitionIn(float percent)
    {
      if (!this.transitionFade)
        return;
      this.tiles.Alpha = percent;
    }

    public override void Update()
    {
      base.Update();
      if (this.fade)
      {
        this.tiles.Alpha = Calc.Approach(this.tiles.Alpha, 0.0f, 2f * Engine.DeltaTime);
        this.cutout.Alpha = this.tiles.Alpha;
        if ((double) this.tiles.Alpha > 0.0)
          return;
        this.RemoveSelf();
      }
      else
      {
        Player player = this.CollideFirst<Player>();
        if (player == null || player.StateMachine.State == 9)
          return;
        this.SceneAs<Level>().Session.DoNotLoad.Add(this.eid);
        this.fade = true;
        Audio.Play("event:/game/general/secret_revealed", this.Center);
      }
    }

    public override void Render()
    {
      if (this.mode == FakeWall.Modes.Wall)
      {
        Level scene = this.Scene as Level;
        Rectangle bounds;
        if (scene.ShakeVector.X < 0.0)
        {
          double x1 = (double) scene.Camera.X;
          bounds = scene.Bounds;
          double left1 = (double) ((Rectangle) ref bounds).get_Left();
          if (x1 <= left1)
          {
            double x2 = (double) this.X;
            bounds = scene.Bounds;
            double left2 = (double) ((Rectangle) ref bounds).get_Left();
            if (x2 <= left2)
              this.tiles.RenderAt(Vector2.op_Addition(this.Position, new Vector2(-3f, 0.0f)));
          }
        }
        if (scene.ShakeVector.X > 0.0)
        {
          double num1 = (double) scene.Camera.X + 320.0;
          bounds = scene.Bounds;
          double right1 = (double) ((Rectangle) ref bounds).get_Right();
          if (num1 >= right1)
          {
            double num2 = (double) this.X + (double) this.Width;
            bounds = scene.Bounds;
            double right2 = (double) ((Rectangle) ref bounds).get_Right();
            if (num2 >= right2)
              this.tiles.RenderAt(Vector2.op_Addition(this.Position, new Vector2(3f, 0.0f)));
          }
        }
        if (scene.ShakeVector.Y < 0.0)
        {
          double y1 = (double) scene.Camera.Y;
          bounds = scene.Bounds;
          double top1 = (double) ((Rectangle) ref bounds).get_Top();
          if (y1 <= top1)
          {
            double y2 = (double) this.Y;
            bounds = scene.Bounds;
            double top2 = (double) ((Rectangle) ref bounds).get_Top();
            if (y2 <= top2)
              this.tiles.RenderAt(Vector2.op_Addition(this.Position, new Vector2(0.0f, -3f)));
          }
        }
        if (scene.ShakeVector.Y > 0.0)
        {
          double num1 = (double) scene.Camera.Y + 180.0;
          bounds = scene.Bounds;
          double bottom1 = (double) ((Rectangle) ref bounds).get_Bottom();
          if (num1 >= bottom1)
          {
            double num2 = (double) this.Y + (double) this.Height;
            bounds = scene.Bounds;
            double bottom2 = (double) ((Rectangle) ref bounds).get_Bottom();
            if (num2 >= bottom2)
              this.tiles.RenderAt(Vector2.op_Addition(this.Position, new Vector2(0.0f, 3f)));
          }
        }
      }
      base.Render();
    }

    public enum Modes
    {
      Wall,
      Block,
    }
  }
}
