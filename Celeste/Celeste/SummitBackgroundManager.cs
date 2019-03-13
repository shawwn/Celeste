﻿// Decompiled with JetBrains decompiler
// Type: Celeste.SummitBackgroundManager
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class SummitBackgroundManager : Entity
  {
    private const string BgSwapFlag = "bgswap_";
    private bool introLaunch;
    private int index;
    private string cutscene;
    private Level level;
    private float fade;
    private float scroll;
    private bool outTheTop;

    public SummitBackgroundManager(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Tag = (int) Tags.TransitionUpdate;
      this.Depth = 8900;
      this.index = data.Int(nameof (index), 0);
      this.cutscene = data.Attr(nameof (cutscene), "");
      this.introLaunch = data.Bool("intro_launch", false);
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.level = this.Scene as Level;
      this.Add((Component) new Coroutine(this.Routine(), true));
    }

    private IEnumerator Routine()
    {
      Player player = this.Scene.Tracker.GetEntity<Player>();
      while (player == null || (double) player.Y > (double) this.Y)
      {
        player = this.Scene.Tracker.GetEntity<Player>();
        yield return (object) null;
      }
      SummitBackgroundManager.WhiteStreaks streaks = new SummitBackgroundManager.WhiteStreaks(this);
      SummitBackgroundManager.Clouds clouds = new SummitBackgroundManager.Clouds(this);
      this.Scene.Add((Entity) streaks);
      this.Scene.Add((Entity) clouds);
      player.Sprite.Play("launch", false, false);
      player.Speed = Vector2.Zero;
      player.StateMachine.State = Player.StDummy;
      player.DummyGravity = false;
      player.DummyAutoAnimate = false;
      if (this.introLaunch)
      {
        this.FadeSnapTo(1f);
        this.level.Camera.Position = player.Center + new Vector2(-160f, -90f);
        yield return (object) 2.3f;
      }
      else
      {
        yield return (object) this.FadeTo(1f);
        if (!string.IsNullOrEmpty(this.cutscene))
        {
          yield return (object) 0.25f;
          CS07_Ascend cs = new CS07_Ascend(this.index, this.cutscene);
          this.level.Add((Entity) cs);
          yield return (object) null;
          while (cs.Running)
            yield return (object) null;
          cs = (CS07_Ascend) null;
        }
        else
          yield return (object) 0.5f;
      }
      this.level.CanRetry = false;
      player.Sprite.Play("launch", false, false);
      Audio.Play("event:/char/madeline/summit_flytonext", player.Position);
      yield return (object) 0.25f;
      Vector2 from = player.Position;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 1f)
      {
        player.Position = Vector2.Lerp(from, from + new Vector2(0.0f, 60f), Ease.CubeInOut(p)) + Calc.Random.ShakeVector();
        Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
        yield return (object) null;
      }
      from = player.Position;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.5f)
      {
        float was = player.Y;
        player.Position = Vector2.Lerp(from, from + new Vector2(0.0f, -160f), Ease.SineIn(p));
        if ((double) p == 0.0 || Calc.OnInterval(player.Y, was, 16f))
          this.level.Add((Entity) Engine.Pooler.Create<SpeedRing>().Init(player.Center, new Vector2(0.0f, -1f).Angle(), Color.White));
        clouds.Fade = (double) p < 0.5 ? 0.0f : (float) (((double) p - 0.5) * 2.0);
        yield return (object) null;
      }
      from = new Vector2();
      this.level.CanRetry = true;
      this.outTheTop = true;
      player.Y = (float) this.level.Bounds.Top;
      player.SummitLaunch(player.X);
      player.DummyGravity = true;
      player.DummyAutoAnimate = true;
      this.level.Session.SetFlag("bgswap_" + (object) this.index, true);
      this.level.NextTransitionDuration = 0.05f;
      if (this.introLaunch)
        this.level.Add((Entity) new HeightDisplay(-1));
    }

    public override void Update()
    {
      this.scroll += Engine.DeltaTime * 240f;
      base.Update();
    }

    public override void Render()
    {
      Draw.Rect(this.level.Camera.X - 10f, this.level.Camera.Y - 10f, 340f, 200f, Calc.HexToColor("75a0ab") * this.fade);
    }

    public override void Removed(Scene scene)
    {
      this.FadeSnapTo(0.0f);
      this.level.Session.SetFlag("bgswap_" + (object) this.index, false);
      if (this.outTheTop)
      {
        ScreenWipe.WipeColor = Color.White;
        if (this.introLaunch)
        {
          MountainWipe mountainWipe = new MountainWipe(this.Scene, true, (Action) null);
        }
        else if (this.index == 0)
          AreaData.Get(1).DoScreenWipe(this.Scene, true, (Action) null);
        else if (this.index == 1)
          AreaData.Get(2).DoScreenWipe(this.Scene, true, (Action) null);
        else if (this.index == 2)
          AreaData.Get(3).DoScreenWipe(this.Scene, true, (Action) null);
        else if (this.index == 3)
          AreaData.Get(4).DoScreenWipe(this.Scene, true, (Action) null);
        else if (this.index == 4)
          AreaData.Get(5).DoScreenWipe(this.Scene, true, (Action) null);
        else if (this.index >= 5)
          AreaData.Get(7).DoScreenWipe(this.Scene, true, (Action) null);
        ScreenWipe.WipeColor = Color.Black;
      }
      base.Removed(scene);
    }

    private IEnumerator FadeTo(float target)
    {
      while ((double) (this.fade = Calc.Approach(this.fade, target, Engine.DeltaTime / 0.8f)) != (double) target)
      {
        this.FadeSnapTo(this.fade);
        yield return (object) null;
      }
      this.FadeSnapTo(target);
    }

    private void FadeSnapTo(float target)
    {
      this.fade = target;
      this.SetSnowAlpha(1f - this.fade);
      this.SetBloom(this.fade * 0.1f);
    }

    private void SetBloom(float add)
    {
      this.level.Bloom.Base = AreaData.Get((Scene) this.level).BloomBase + add;
    }

    private void SetSnowAlpha(float value)
    {
      Snow snow = this.level.Foreground.Get<Snow>();
      if (snow != null)
        snow.Alpha = value;
      WindSnowFG windSnowFg = this.level.Foreground.Get<WindSnowFG>();
      if (windSnowFg == null)
        return;
      windSnowFg.Alpha = value;
    }

    private static float Mod(float x, float m)
    {
      return (x % m + m) % m;
    }

    private class WhiteStreaks : Entity
    {
      private SummitBackgroundManager.WhiteStreaks.Particle[] particles = new SummitBackgroundManager.WhiteStreaks.Particle[80];
      private Color[] colors = new Color[2]
      {
        Color.White,
        Calc.HexToColor("e69ecb")
      };
      private const float MinSpeed = 600f;
      private const float MaxSpeed = 2000f;
      private List<MTexture> textures;
      private Color[] alphaColors;
      private SummitBackgroundManager manager;

      public WhiteStreaks(SummitBackgroundManager manager)
      {
        this.manager = manager;
        this.Depth = 20;
        this.textures = GFX.Game.GetAtlasSubtextures("scenery/launch/slice");
        this.alphaColors = new Color[this.colors.Length];
        for (int index = 0; index < this.particles.Length; ++index)
        {
          float x = (float) (160.0 + (double) Calc.Random.Range(24f, 144f) * (double) Calc.Random.Choose<int>(-1, 1));
          float y = Calc.Random.NextFloat(436f);
          float num = Calc.ClampedMap(Math.Abs(x - 160f), 0.0f, 160f, 0.25f, 1f) * Calc.Random.Range(600f, 2000f);
          this.particles[index] = new SummitBackgroundManager.WhiteStreaks.Particle()
          {
            Position = new Vector2(x, y),
            Speed = num,
            Index = Calc.Random.Next(this.textures.Count),
            Color = Calc.Random.Next(this.colors.Length)
          };
        }
      }

      public override void Update()
      {
        base.Update();
        for (int index = 0; index < this.particles.Length; ++index)
          this.particles[index].Position.Y += this.particles[index].Speed * Engine.DeltaTime;
      }

      public override void Render()
      {
        float num = Ease.SineInOut(this.manager.fade);
        Vector2 position1 = (this.Scene as Level).Camera.Position;
        for (int index = 0; index < this.colors.Length; ++index)
          this.alphaColors[index] = this.colors[index] * num;
        for (int index = 0; index < this.particles.Length; ++index)
        {
          Vector2 position2 = this.particles[index].Position;
          position2.X = SummitBackgroundManager.Mod(position2.X, 320f);
          position2.Y = SummitBackgroundManager.Mod(position2.Y, 436f) - 128f;
          Vector2 position3 = position2 + position1;
          Vector2 scale = new Vector2()
          {
            X = Calc.ClampedMap(this.particles[index].Speed, 600f, 2000f, 1f, 0.25f),
            Y = Calc.ClampedMap(this.particles[index].Speed, 600f, 2000f, 1f, 2f)
          } * Calc.ClampedMap(this.particles[index].Speed, 600f, 2000f, 1f, 4f);
          MTexture texture = this.textures[this.particles[index].Index];
          Color alphaColor = this.alphaColors[this.particles[index].Color];
          texture.DrawCentered(position3, alphaColor, scale);
        }
        Draw.Rect(position1.X - 10f, position1.Y - 10f, 26f, 200f, this.alphaColors[0]);
        Draw.Rect((float) ((double) position1.X + 320.0 - 16.0), position1.Y - 10f, 26f, 200f, this.alphaColors[0]);
      }

      private class Particle
      {
        public Vector2 Position;
        public float Speed;
        public int Index;
        public int Color;
      }
    }

    private class Clouds : Entity
    {
      private SummitBackgroundManager.Clouds.Particle[] particles = new SummitBackgroundManager.Clouds.Particle[10];
      public float Fade = 0.0f;
      private SummitBackgroundManager manager;
      private List<MTexture> textures;

      public Clouds(SummitBackgroundManager manager)
      {
        this.Depth = -1000000;
        this.manager = manager;
        this.textures = GFX.Game.GetAtlasSubtextures("scenery/launch/cloud");
        for (int index = 0; index < this.particles.Length; ++index)
          this.particles[index] = new SummitBackgroundManager.Clouds.Particle()
          {
            Position = new Vector2(Calc.Random.NextFloat(320f), Calc.Random.NextFloat(900f)),
            Speed = (float) Calc.Random.Range(400, 800),
            Index = Calc.Random.Next(this.textures.Count)
          };
      }

      public override void Update()
      {
        base.Update();
        for (int index = 0; index < this.particles.Length; ++index)
          this.particles[index].Position.Y += this.particles[index].Speed * Engine.DeltaTime;
      }

      public override void Render()
      {
        Color color = Calc.HexToColor("b64a86") * this.manager.fade;
        Vector2 position1 = (this.Scene as Level).Camera.Position;
        for (int index = 0; index < this.particles.Length; ++index)
        {
          Vector2 position2 = this.particles[index].Position;
          position2.Y = SummitBackgroundManager.Mod(position2.Y, 900f) - 360f;
          Vector2 position3 = position2 + position1;
          this.textures[this.particles[index].Index].DrawCentered(position3, color);
        }
        if ((double) this.Fade <= 0.0)
          return;
        Draw.Rect(position1.X - 10f, position1.Y - 10f, 340f, 200f, Color.White * this.Fade);
      }

      private class Particle
      {
        public Vector2 Position;
        public float Speed;
        public int Index;
      }
    }
  }
}

