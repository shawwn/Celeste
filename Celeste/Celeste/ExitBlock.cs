// Decompiled with JetBrains decompiler
// Type: Celeste.ExitBlock
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class ExitBlock : Solid
  {
    private TileGrid tiles;
    private TransitionListener tl;
    private EffectCutout cutout;
    private float startAlpha;
    private char tileType;

    public ExitBlock(Vector2 position, float width, float height, char tileType)
      : base(position, width, height, true)
    {
      this.Depth = -13000;
      this.tileType = tileType;
      this.tl = new TransitionListener();
      this.tl.OnOutBegin = new Action(this.OnTransitionOutBegin);
      this.tl.OnInBegin = new Action(this.OnTransitionInBegin);
      this.Add((Component) this.tl);
      this.Add((Component) (this.cutout = new EffectCutout()));
      this.SurfaceSoundIndex = SurfaceIndex.TileToIndex[tileType];
      this.EnableAssistModeChecks = false;
    }

    public ExitBlock(EntityData data, Vector2 offset)
      : this(data.Position + offset, (float) data.Width, (float) data.Height, data.Char(nameof (tileType), '3'))
    {
    }

    private void OnTransitionOutBegin()
    {
      if (!Collide.CheckRect((Entity) this, this.SceneAs<Level>().Bounds))
        return;
      this.tl.OnOut = new Action<float>(this.OnTransitionOut);
      this.startAlpha = this.tiles.Alpha;
    }

    private void OnTransitionOut(float percent)
    {
      this.cutout.Alpha = this.tiles.Alpha = MathHelper.Lerp(this.startAlpha, 0.0f, percent);
      this.cutout.Update();
    }

    private void OnTransitionInBegin()
    {
      if (!Collide.CheckRect((Entity) this, this.SceneAs<Level>().PreviousBounds.Value) || this.CollideCheck<Player>())
        return;
      this.cutout.Alpha = 0.0f;
      this.tiles.Alpha = 0.0f;
      this.tl.OnIn = new Action<float>(this.OnTransitionIn);
    }

    private void OnTransitionIn(float percent)
    {
      this.cutout.Alpha = this.tiles.Alpha = percent;
      this.cutout.Update();
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      Level level = this.SceneAs<Level>();
      Rectangle tileBounds = level.Session.MapData.TileBounds;
      VirtualMap<char> solidsData = level.SolidsData;
      int x = (int) ((double) this.X / 8.0) - tileBounds.Left;
      int y = (int) ((double) this.Y / 8.0) - tileBounds.Top;
      int tilesX = (int) this.Width / 8;
      int tilesY = (int) this.Height / 8;
      this.tiles = GFX.FGAutotiler.GenerateOverlay(this.tileType, x, y, tilesX, tilesY, solidsData).TileGrid;
      this.Add((Component) this.tiles);
      this.Add((Component) new TileInterceptor(this.tiles, false));
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (!this.CollideCheck<Player>())
        return;
      this.cutout.Alpha = this.tiles.Alpha = 0.0f;
      this.Collidable = false;
    }

    public override void Update()
    {
      base.Update();
      if (this.Collidable)
      {
        this.cutout.Alpha = this.tiles.Alpha = Calc.Approach(this.tiles.Alpha, 1f, Engine.DeltaTime);
      }
      else
      {
        if (this.CollideCheck<Player>())
          return;
        this.Collidable = true;
        Audio.Play("event:/game/general/passage_closed_behind", this.Center);
      }
    }

    public override void Render()
    {
      if ((double) this.tiles.Alpha >= 1.0)
      {
        Level scene = this.Scene as Level;
        if ((double) scene.ShakeVector.X < 0.0 && (double) scene.Camera.X <= (double) scene.Bounds.Left && (double) this.X <= (double) scene.Bounds.Left)
          this.tiles.RenderAt(this.Position + new Vector2(-3f, 0.0f));
        Rectangle bounds;
        int num1;
        if ((double) scene.ShakeVector.X > 0.0)
        {
          double num2 = (double) scene.Camera.X + 320.0;
          bounds = scene.Bounds;
          double right1 = (double) bounds.Right;
          if (num2 >= right1)
          {
            double num3 = (double) this.X + (double) this.Width;
            bounds = scene.Bounds;
            double right2 = (double) bounds.Right;
            num1 = num3 >= right2 ? 1 : 0;
            goto label_7;
          }
        }
        num1 = 0;
label_7:
        if (num1 != 0)
          this.tiles.RenderAt(this.Position + new Vector2(3f, 0.0f));
        int num4;
        if ((double) scene.ShakeVector.Y < 0.0)
        {
          double y1 = (double) scene.Camera.Y;
          bounds = scene.Bounds;
          double top1 = (double) bounds.Top;
          if (y1 <= top1)
          {
            double y2 = (double) this.Y;
            bounds = scene.Bounds;
            double top2 = (double) bounds.Top;
            num4 = y2 <= top2 ? 1 : 0;
            goto label_13;
          }
        }
        num4 = 0;
label_13:
        if (num4 != 0)
          this.tiles.RenderAt(this.Position + new Vector2(0.0f, -3f));
        int num5;
        if ((double) scene.ShakeVector.Y > 0.0)
        {
          double num2 = (double) scene.Camera.Y + 180.0;
          bounds = scene.Bounds;
          double bottom1 = (double) bounds.Bottom;
          if (num2 >= bottom1)
          {
            double num3 = (double) this.Y + (double) this.Height;
            bounds = scene.Bounds;
            double bottom2 = (double) bounds.Bottom;
            num5 = num3 >= bottom2 ? 1 : 0;
            goto label_19;
          }
        }
        num5 = 0;
label_19:
        if (num5 != 0)
          this.tiles.RenderAt(this.Position + new Vector2(0.0f, 3f));
      }
      base.Render();
    }
  }
}

