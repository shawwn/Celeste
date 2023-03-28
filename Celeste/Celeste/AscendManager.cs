// Decompiled with JetBrains decompiler
// Type: Celeste.AscendManager
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class AscendManager : Entity
  {
    private const string BeginSwapFlag = "beginswap_";
    private const string BgSwapFlag = "bgswap_";
    public readonly bool Dark;
    public readonly bool Ch9Ending;
    private bool introLaunch;
    private int index;
    private string cutscene;
    private Level level;
    private float fade;
    private float scroll;
    private bool outTheTop;
    private Color background;
    private string ambience;

    public AscendManager(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Tag = (int) Tags.TransitionUpdate;
      this.Depth = 8900;
      this.index = data.Int(nameof (index));
      this.cutscene = data.Attr(nameof (cutscene));
      this.introLaunch = data.Bool("intro_launch");
      this.Dark = data.Bool("dark");
      this.Ch9Ending = this.cutscene.Equals("CH9_FREE_BIRD", StringComparison.InvariantCultureIgnoreCase);
      this.ambience = data.Attr(nameof (ambience));
      this.background = this.Dark ? Color.Black : Calc.HexToColor("75a0ab");
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.level = this.Scene as Level;
      this.Add((Component) new Coroutine(this.Routine()));
    }

    private IEnumerator Routine()
    {
      AscendManager manager = this;
      Player player = manager.Scene.Tracker.GetEntity<Player>();
      while (player == null || (double) player.Y > (double) manager.Y)
      {
        player = manager.Scene.Tracker.GetEntity<Player>();
        yield return (object) null;
      }
      if (manager.index == 9)
        yield return (object) 1.6f;
      AscendManager.Streaks streaks = new AscendManager.Streaks(manager);
      manager.Scene.Add((Entity) streaks);
      if (!manager.Dark)
      {
        AscendManager.Clouds clouds = new AscendManager.Clouds(manager);
        manager.Scene.Add((Entity) clouds);
      }
      manager.level.Session.SetFlag("beginswap_" + (object) manager.index);
      player.Sprite.Play("launch");
      player.Speed = Vector2.Zero;
      player.StateMachine.State = 11;
      player.DummyGravity = false;
      player.DummyAutoAnimate = false;
      if (!string.IsNullOrWhiteSpace(manager.ambience))
      {
        if (manager.ambience.Equals("null", StringComparison.InvariantCultureIgnoreCase))
          Audio.SetAmbience((string) null);
        else
          Audio.SetAmbience(SFX.EventnameByHandle(manager.ambience));
      }
      if (manager.introLaunch)
      {
        manager.FadeSnapTo(1f);
        manager.level.Camera.Position = player.Center + new Vector2(-160f, -90f);
        yield return (object) 2.3f;
      }
      else
      {
        yield return (object) manager.FadeTo(1f, manager.Dark ? 2f : 0.8f);
        if (manager.Ch9Ending)
        {
          manager.level.Add((Entity) new CS10_FreeBird());
          while (true)
            yield return (object) null;
        }
        else if (!string.IsNullOrEmpty(manager.cutscene))
        {
          yield return (object) 0.25f;
          CS07_Ascend cs = new CS07_Ascend(manager.index, manager.cutscene, manager.Dark);
          manager.level.Add((Entity) cs);
          yield return (object) null;
          while (cs.Running)
            yield return (object) null;
          cs = (CS07_Ascend) null;
        }
        else
          yield return (object) 0.5f;
      }
      manager.level.CanRetry = false;
      player.Sprite.Play("launch");
      Audio.Play("event:/char/madeline/summit_flytonext", player.Position);
      yield return (object) 0.25f;
      Vector2 from = player.Position;
      float p;
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 1f)
      {
        player.Position = Vector2.Lerp(from, from + new Vector2(0.0f, 60f), Ease.CubeInOut(p)) + Calc.Random.ShakeVector();
        Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
        yield return (object) null;
      }
      AscendManager.Fader fader = new AscendManager.Fader(manager);
      manager.Scene.Add((Entity) fader);
      from = player.Position;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      for (p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.5f)
      {
        float y = player.Y;
        player.Position = Vector2.Lerp(from, from + new Vector2(0.0f, -160f), Ease.SineIn(p));
        if ((double) p == 0.0 || Calc.OnInterval(player.Y, y, 16f))
          manager.level.Add((Entity) Engine.Pooler.Create<SpeedRing>().Init(player.Center, new Vector2(0.0f, -1f).Angle(), Color.White));
        fader.Fade = (double) p < 0.5 ? 0.0f : (float) (((double) p - 0.5) * 2.0);
        yield return (object) null;
      }
      from = new Vector2();
      fader = (AscendManager.Fader) null;
      manager.level.CanRetry = true;
      manager.outTheTop = true;
      player.Y = (float) manager.level.Bounds.Top;
      player.SummitLaunch(player.X);
      player.DummyGravity = true;
      player.DummyAutoAnimate = true;
      manager.level.Session.SetFlag("bgswap_" + (object) manager.index);
      manager.level.NextTransitionDuration = 0.05f;
      if (manager.introLaunch)
        manager.level.Add((Entity) new HeightDisplay(-1));
    }

    public override void Update()
    {
      this.scroll += Engine.DeltaTime * 240f;
      base.Update();
    }

    public override void Render() => Draw.Rect(this.level.Camera.X - 10f, this.level.Camera.Y - 10f, 340f, 200f, this.background * this.fade);

    public override void Removed(Scene scene)
    {
      this.FadeSnapTo(0.0f);
      this.level.Session.SetFlag("bgswap_" + (object) this.index, false);
      this.level.Session.SetFlag("beginswap_" + (object) this.index, false);
      if (this.outTheTop)
      {
        ScreenWipe.WipeColor = this.Dark ? Color.Black : Color.White;
        if (this.introLaunch)
        {
          MountainWipe mountainWipe = new MountainWipe(this.Scene, true);
        }
        else if (this.index == 0)
          AreaData.Get(1).DoScreenWipe(this.Scene, true);
        else if (this.index == 1)
          AreaData.Get(2).DoScreenWipe(this.Scene, true);
        else if (this.index == 2)
          AreaData.Get(3).DoScreenWipe(this.Scene, true);
        else if (this.index == 3)
          AreaData.Get(4).DoScreenWipe(this.Scene, true);
        else if (this.index == 4)
          AreaData.Get(5).DoScreenWipe(this.Scene, true);
        else if (this.index == 5)
          AreaData.Get(7).DoScreenWipe(this.Scene, true);
        else if (this.index >= 9)
          AreaData.Get(10).DoScreenWipe(this.Scene, true);
        ScreenWipe.WipeColor = Color.Black;
      }
      base.Removed(scene);
    }

    private IEnumerator FadeTo(float target, float duration = 0.8f)
    {
      while ((double) (this.fade = Calc.Approach(this.fade, target, Engine.DeltaTime / duration)) != (double) target)
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
      if (!this.Dark)
        return;
      foreach (Parallax parallax in this.level.Background.GetEach<Parallax>())
        parallax.CameraOffset.Y -= 25f * target;
      foreach (Parallax parallax in this.level.Foreground.GetEach<Parallax>())
        parallax.Alpha = 1f - this.fade;
    }

    private void SetBloom(float add) => this.level.Bloom.Base = AreaData.Get((Scene) this.level).BloomBase + add;

    private void SetSnowAlpha(float value)
    {
      Snow snow = this.level.Foreground.Get<Snow>();
      if (snow != null)
        snow.Alpha = value;
      RainFG rainFg = this.level.Foreground.Get<RainFG>();
      if (rainFg != null)
        rainFg.Alpha = value;
      WindSnowFG windSnowFg = this.level.Foreground.Get<WindSnowFG>();
      if (windSnowFg == null)
        return;
      windSnowFg.Alpha = value;
    }

    private static float Mod(float x, float m) => (x % m + m) % m;

    public class Streaks : Entity
    {
      private const float MinSpeed = 600f;
      private const float MaxSpeed = 2000f;
      public float Alpha = 1f;
      private AscendManager.Streaks.Particle[] particles = new AscendManager.Streaks.Particle[80];
      private List<MTexture> textures;
      private Color[] colors;
      private Color[] alphaColors;
      private AscendManager manager;

      public Streaks(AscendManager manager)
      {
        this.manager = manager;
        if (manager == null || !manager.Dark)
          this.colors = new Color[2]
          {
            Color.White,
            Calc.HexToColor("e69ecb")
          };
        else
          this.colors = new Color[2]
          {
            Calc.HexToColor("041b44"),
            Calc.HexToColor("011230")
          };
        this.Depth = 20;
        this.textures = GFX.Game.GetAtlasSubtextures("scenery/launch/slice");
        this.alphaColors = new Color[this.colors.Length];
        for (int index = 0; index < this.particles.Length; ++index)
        {
          float x = (float) (160.0 + (double) Calc.Random.Range(24f, 144f) * (double) Calc.Random.Choose<int>(-1, 1));
          float y = Calc.Random.NextFloat(436f);
          float num = Calc.ClampedMap(Math.Abs(x - 160f), 0.0f, 160f, 0.25f) * Calc.Random.Range(600f, 2000f);
          this.particles[index] = new AscendManager.Streaks.Particle()
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
        float num = Ease.SineInOut((this.manager != null ? this.manager.fade : 1f) * this.Alpha);
        Vector2 position1 = (this.Scene as Level).Camera.Position;
        for (int index = 0; index < this.colors.Length; ++index)
          this.alphaColors[index] = this.colors[index] * num;
        for (int index = 0; index < this.particles.Length; ++index)
        {
          Vector2 position2 = this.particles[index].Position;
          position2.X = AscendManager.Mod(position2.X, 320f);
          position2.Y = AscendManager.Mod(position2.Y, 436f) - 128f;
          Vector2 vector2_1 = position2 + position1;
          Vector2 vector2_2 = new Vector2()
          {
            X = Calc.ClampedMap(this.particles[index].Speed, 600f, 2000f, 1f, 0.25f),
            Y = Calc.ClampedMap(this.particles[index].Speed, 600f, 2000f, 1f, 2f)
          } * Calc.ClampedMap(this.particles[index].Speed, 600f, 2000f, 1f, 4f);
          MTexture texture = this.textures[this.particles[index].Index];
          Color alphaColor = this.alphaColors[this.particles[index].Color];
          Vector2 position3 = vector2_1;
          Color color = alphaColor;
          Vector2 scale = vector2_2;
          texture.DrawCentered(position3, color, scale);
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

    public class Clouds : Entity
    {
      public float Alpha;
      private AscendManager manager;
      private List<MTexture> textures;
      private AscendManager.Clouds.Particle[] particles = new AscendManager.Clouds.Particle[10];
      private Color color;

      public Clouds(AscendManager manager)
      {
        this.manager = manager;
        this.color = manager == null || !manager.Dark ? Calc.HexToColor("b64a86") : Calc.HexToColor("082644");
        this.Depth = -1000000;
        this.textures = GFX.Game.GetAtlasSubtextures("scenery/launch/cloud");
        for (int index = 0; index < this.particles.Length; ++index)
          this.particles[index] = new AscendManager.Clouds.Particle()
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
        Color color = this.color * ((this.manager != null ? this.manager.fade : 1f) * this.Alpha);
        Vector2 position1 = (this.Scene as Level).Camera.Position;
        for (int index = 0; index < this.particles.Length; ++index)
        {
          Vector2 position2 = this.particles[index].Position;
          position2.Y = AscendManager.Mod(position2.Y, 900f) - 360f;
          Vector2 position3 = position2 + position1;
          this.textures[this.particles[index].Index].DrawCentered(position3, color);
        }
      }

      private class Particle
      {
        public Vector2 Position;
        public float Speed;
        public int Index;
      }
    }

    private class Fader : Entity
    {
      public float Fade;
      private AscendManager manager;

      public Fader(AscendManager manager)
      {
        this.manager = manager;
        this.Depth = -1000010;
      }

      public override void Render()
      {
        if ((double) this.Fade <= 0.0)
          return;
        Vector2 position = (this.Scene as Level).Camera.Position;
        Draw.Rect(position.X - 10f, position.Y - 10f, 340f, 200f, (this.manager.Dark ? Color.Black : Color.White) * this.Fade);
      }
    }
  }
}
