// Decompiled with JetBrains decompiler
// Type: Celeste.IntroCrusher
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class IntroCrusher : Solid
  {
    private Vector2 shake;
    private Vector2 start;
    private Vector2 end;
    private TileGrid tilegrid;
    private SoundSource shakingSfx;

    public IntroCrusher(Vector2 position, int width, int height, Vector2 node)
      : base(position, (float) width, (float) height, true)
    {
      this.start = position;
      this.end = node;
      this.Depth = -10501;
      this.SurfaceSoundIndex = 4;
      this.Add((Component) (this.tilegrid = GFX.FGAutotiler.GenerateBox('3', width / 8, height / 8).TileGrid));
      this.Add((Component) (this.shakingSfx = new SoundSource()));
    }

    public IntroCrusher(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Width, data.Height, data.Nodes[0] + offset)
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if (this.SceneAs<Level>().Session.GetLevelFlag("1") || this.SceneAs<Level>().Session.GetLevelFlag("0b"))
        this.Position = this.end;
      else
        this.Add((Component) new Coroutine(this.Sequence(), true));
    }

    public override void Update()
    {
      this.tilegrid.Position = this.shake;
      base.Update();
    }

    private IEnumerator Sequence()
    {
      while (true)
      {
        yield return (object) null;
        Player p = this.Scene.Tracker.GetEntity<Player>();
        if (p == null || (double) p.X < (double) this.X + 30.0 || (double) p.X > (double) this.Right + 8.0)
          p = (Player) null;
        else
          break;
      }
      this.shakingSfx.Play("event:/game/00_prologue/fallblock_first_shake", (string) null, 0.0f);
      float time = 1.2f;
      Shaker shaker = new Shaker(time, true, (Action<Vector2>) (v => this.shake = v));
      this.Add((Component) shaker);
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      while ((double) time > 0.0)
      {
        Player p = this.Scene.Tracker.GetEntity<Player>();
        if (p != null && ((double) p.X >= (double) this.X + (double) this.Width - 8.0 || (double) p.X < (double) this.X + 28.0))
        {
          shaker.RemoveSelf();
          break;
        }
        yield return (object) null;
        time -= Engine.DeltaTime;
        p = (Player) null;
      }
      shaker = (Shaker) null;
      for (int i = 2; (double) i < (double) this.Width; i += 4)
      {
        this.SceneAs<Level>().Particles.Emit(FallingBlock.P_FallDustA, 2, new Vector2(this.X + (float) i, this.Y), Vector2.One * 4f, 1.570796f);
        this.SceneAs<Level>().Particles.Emit(FallingBlock.P_FallDustB, 2, new Vector2(this.X + (float) i, this.Y), Vector2.One * 4f);
      }
      this.shakingSfx.Param("release", 1f);
      float percent = 0.0f;
      do
      {
        yield return (object) null;
        percent = Calc.Approach(percent, 1f, 2f * Engine.DeltaTime);
        this.MoveTo(Vector2.Lerp(this.start, this.end, Ease.CubeIn(percent)));
      }
      while ((double) percent < 1.0);
      for (int i = 0; (double) i <= (double) this.Width; i += 4)
      {
        this.SceneAs<Level>().ParticlesFG.Emit(FallingBlock.P_FallDustA, 1, new Vector2(this.X + (float) i, this.Bottom), Vector2.One * 4f, -1.570796f);
        float dir = (double) i >= (double) this.Width / 2.0 ? 0.0f : 3.141593f;
        this.SceneAs<Level>().ParticlesFG.Emit(FallingBlock.P_LandDust, 1, new Vector2(this.X + (float) i, this.Bottom), Vector2.One * 4f, dir);
      }
      this.shakingSfx.Stop(true);
      Audio.Play("event:/game/00_prologue/fallblock_first_impact", this.Position);
      this.SceneAs<Level>().Shake(0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      this.Add((Component) new Shaker(0.25f, true, (Action<Vector2>) (v => this.shake = v)));
    }
  }
}

