// Decompiled with JetBrains decompiler
// Type: Celeste.WaterFall
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class WaterFall : Entity
  {
    private float height;
    private Water water;
    private Solid solid;
    private SoundSource loopingSfx;
    private SoundSource enteringSfx;

    public WaterFall(Vector2 position)
      : base(position)
    {
      this.Depth = -9999;
      this.Tag = (int) Tags.TransitionUpdate;
    }

    public WaterFall(EntityData data, Vector2 offset)
      : this(Vector2.op_Addition(data.Position, offset))
    {
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      Level scene1 = this.Scene as Level;
      bool flag = false;
      this.height = 8f;
      while (true)
      {
        double num = (double) this.Y + (double) this.height;
        Rectangle bounds = scene1.Bounds;
        double bottom = (double) ((Rectangle) ref bounds).get_Bottom();
        if (num < bottom && (this.water = this.Scene.CollideFirst<Water>(new Rectangle((int) this.X, (int) ((double) this.Y + (double) this.height), 8, 8))) == null && ((this.solid = this.Scene.CollideFirst<Solid>(new Rectangle((int) this.X, (int) ((double) this.Y + (double) this.height), 8, 8))) == null || !this.solid.BlockWaterfalls))
        {
          this.height += 8f;
          this.solid = (Solid) null;
        }
        else
          break;
      }
      if (this.water != null && !this.Scene.CollideCheck<Solid>(new Rectangle((int) this.X, (int) ((double) this.Y + (double) this.height), 8, 16)))
        flag = true;
      this.Add((Component) (this.loopingSfx = new SoundSource()));
      this.loopingSfx.Play("event:/env/local/waterfall_small_main", (string) null, 0.0f);
      this.Add((Component) (this.enteringSfx = new SoundSource()));
      this.enteringSfx.Play(flag ? "event:/env/local/waterfall_small_in_deep" : "event:/env/local/waterfall_small_in_shallow", (string) null, 0.0f);
      this.enteringSfx.Position.Y = (__Null) (double) this.height;
      this.Add((Component) new DisplacementRenderHook(new Action(this.RenderDisplacement)));
    }

    public override void Update()
    {
      this.loopingSfx.Position.Y = (__Null) (double) Calc.Clamp((float) ((this.Scene as Level).Camera.Position.Y + 90.0), this.Y, this.height);
      if (this.water != null && this.Scene.OnInterval(0.3f))
        this.water.TopSurface.DoRipple(new Vector2(this.X + 4f, this.water.Y), 0.75f);
      if (this.water != null || this.solid != null)
      {
        Vector2 position;
        ((Vector2) ref position).\u002Ector(this.X + 4f, (float) ((double) this.Y + (double) this.height + 2.0));
        (this.Scene as Level).ParticlesFG.Emit(Water.P_Splash, 1, position, new Vector2(8f, 2f), new Vector2(0.0f, -1f).Angle());
      }
      base.Update();
    }

    public void RenderDisplacement()
    {
      Draw.Rect(this.X, this.Y, 8f, this.height, new Color(0.5f, 0.5f, 0.8f, 1f));
    }

    public override void Render()
    {
      if (this.water == null || this.water.TopSurface == null)
      {
        Draw.Rect(this.X + 1f, this.Y, 6f, this.height, Water.FillColor);
        Draw.Rect(this.X - 1f, this.Y, 2f, this.height, Water.SurfaceColor);
        Draw.Rect(this.X + 7f, this.Y, 2f, this.height, Water.SurfaceColor);
      }
      else
      {
        Water.Surface topSurface = this.water.TopSurface;
        float num = this.height + (float) this.water.TopSurface.Position.Y - this.water.Y;
        for (int index = 0; index < 6; ++index)
          Draw.Rect((float) ((double) this.X + (double) index + 1.0), this.Y, 1f, num - topSurface.GetSurfaceHeight(new Vector2(this.X + 1f + (float) index, this.water.Y)), Water.FillColor);
        Draw.Rect(this.X - 1f, this.Y, 2f, num - topSurface.GetSurfaceHeight(new Vector2(this.X, this.water.Y)), Water.SurfaceColor);
        Draw.Rect(this.X + 7f, this.Y, 2f, num - topSurface.GetSurfaceHeight(new Vector2(this.X + 8f, this.water.Y)), Water.SurfaceColor);
      }
    }
  }
}
