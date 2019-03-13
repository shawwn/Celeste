// Decompiled with JetBrains decompiler
// Type: Celeste.ForsakenCitySatellite
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Celeste
{
  public class ForsakenCitySatellite : Entity
  {
    public static readonly Dictionary<string, Color> Colors = new Dictionary<string, Color>()
    {
      {
        "U",
        Calc.HexToColor("f0f0f0")
      },
      {
        "L",
        Calc.HexToColor("9171f2")
      },
      {
        "DR",
        Calc.HexToColor("0a44e0")
      },
      {
        "UR",
        Calc.HexToColor("b32d00")
      },
      {
        "UL",
        Calc.HexToColor("ffcd37")
      }
    };
    public static readonly Dictionary<string, string> Sounds = new Dictionary<string, string>()
    {
      {
        "U",
        "event:/game/01_forsaken_city/console_white"
      },
      {
        "L",
        "event:/game/01_forsaken_city/console_purple"
      },
      {
        "DR",
        "event:/game/01_forsaken_city/console_blue"
      },
      {
        "UR",
        "event:/game/01_forsaken_city/console_red"
      },
      {
        "UL",
        "event:/game/01_forsaken_city/console_yellow"
      }
    };
    public static readonly Dictionary<string, ParticleType> Particles = new Dictionary<string, ParticleType>();
    private static readonly string[] Code = new string[6]
    {
      "U",
      "L",
      "DR",
      "UR",
      "L",
      "UL"
    };
    private static List<string> uniqueCodes = new List<string>();
    private List<string> currentInputs = new List<string>();
    private List<ForsakenCitySatellite.CodeBird> birds = new List<ForsakenCitySatellite.CodeBird>();
    private const string UnlockedFlag = "unlocked_satellite";
    private bool enabled;
    private Vector2 gemSpawnPosition;
    private Vector2 birdFlyPosition;
    private Monocle.Image sprite;
    private Monocle.Image pulse;
    private Monocle.Image computer;
    private Monocle.Image computerScreen;
    private Sprite computerScreenNoise;
    private Monocle.Image computerScreenShine;
    private BloomPoint pulseBloom;
    private BloomPoint screenBloom;
    private Level level;
    private DashListener dashListener;
    private SoundSource birdFlyingSfx;
    private SoundSource birdThrustSfx;
    private SoundSource birdFinishSfx;
    private SoundSource staticLoopSfx;

    public ForsakenCitySatellite(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Add((Component) (this.sprite = new Monocle.Image(GFX.Game["objects/citysatellite/dish"])));
      this.Add((Component) (this.pulse = new Monocle.Image(GFX.Game["objects/citysatellite/light"])));
      this.Add((Component) (this.computer = new Monocle.Image(GFX.Game["objects/citysatellite/computer"])));
      this.Add((Component) (this.computerScreen = new Monocle.Image(GFX.Game["objects/citysatellite/computerscreen"])));
      this.Add((Component) (this.computerScreenNoise = new Sprite(GFX.Game, "objects/citysatellite/computerScreenNoise")));
      this.Add((Component) (this.computerScreenShine = new Monocle.Image(GFX.Game["objects/citysatellite/computerscreenShine"])));
      this.sprite.JustifyOrigin(0.5f, 1f);
      this.pulse.JustifyOrigin(0.5f, 1f);
      this.Add((Component) new Coroutine(this.PulseRoutine(), true));
      this.Add((Component) (this.pulseBloom = new BloomPoint(new Vector2(-12f, -44f), 1f, 8f)));
      this.Add((Component) (this.screenBloom = new BloomPoint(new Vector2(32f, 20f), 1f, 8f)));
      this.computerScreenNoise.AddLoop("static", "", 0.05f);
      this.computerScreenNoise.Play("static", false, false);
      this.computer.Position = this.computerScreen.Position = this.computerScreenShine.Position = this.computerScreenNoise.Position = new Vector2(8f, 8f);
      this.birdFlyPosition = offset + data.Nodes[0];
      this.gemSpawnPosition = offset + data.Nodes[1];
      this.Add((Component) (this.dashListener = new DashListener()));
      this.dashListener.OnDash = (Action<Vector2>) (dir =>
      {
        string str = "";
        if ((double) dir.Y < 0.0)
          str = "U";
        else if ((double) dir.Y > 0.0)
          str = "D";
        if ((double) dir.X < 0.0)
          str += "L";
        else if ((double) dir.X > 0.0)
          str += "R";
        this.currentInputs.Add(str);
        if (this.currentInputs.Count > ForsakenCitySatellite.Code.Length)
          this.currentInputs.RemoveAt(0);
        if (this.currentInputs.Count != ForsakenCitySatellite.Code.Length)
          return;
        bool flag = true;
        for (int index = 0; index < ForsakenCitySatellite.Code.Length; ++index)
        {
          if (!this.currentInputs[index].Equals(ForsakenCitySatellite.Code[index]))
            flag = false;
        }
        if (flag && (double) this.level.Camera.Left + 32.0 < (double) this.gemSpawnPosition.X && this.enabled)
          this.Add((Component) new Coroutine(this.UnlockGem(), true));
      });
      foreach (string str in ForsakenCitySatellite.Code)
      {
        if (!ForsakenCitySatellite.uniqueCodes.Contains(str))
          ForsakenCitySatellite.uniqueCodes.Add(str);
      }
      this.Depth = 8999;
      this.Add((Component) (this.staticLoopSfx = new SoundSource()));
      this.staticLoopSfx.Position = this.computer.Position;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.level = scene as Level;
      this.enabled = !this.level.Session.HeartGem && !this.level.Session.GetFlag("unlocked_satellite");
      if (this.enabled)
      {
        foreach (string uniqueCode in ForsakenCitySatellite.uniqueCodes)
        {
          ForsakenCitySatellite.CodeBird codeBird = new ForsakenCitySatellite.CodeBird(this.birdFlyPosition, uniqueCode);
          this.birds.Add(codeBird);
          this.level.Add((Entity) codeBird);
        }
        this.Add((Component) (this.birdFlyingSfx = new SoundSource()));
        this.Add((Component) (this.birdFinishSfx = new SoundSource()));
        this.Add((Component) (this.birdThrustSfx = new SoundSource()));
        this.birdFlyingSfx.Position = this.birdFlyPosition - this.Position;
        this.birdFlyingSfx.Play("event:/game/01_forsaken_city/birdbros_fly_loop", (string) null, 0.0f);
      }
      else
        this.staticLoopSfx.Play("event:/game/01_forsaken_city/console_static_loop", (string) null, 0.0f);
      if (this.level.Session.HeartGem || !this.level.Session.GetFlag("unlocked_satellite"))
        return;
      this.level.Add((Entity) new HeartGem(this.gemSpawnPosition));
    }

    public override void Update()
    {
      base.Update();
      this.computerScreenNoise.Visible = !this.pulse.Visible;
      this.computerScreen.Visible = this.pulse.Visible;
      this.screenBloom.Visible = this.pulseBloom.Visible;
    }

    private IEnumerator PulseRoutine()
    {
      this.pulseBloom.Visible = this.pulse.Visible = false;
      while (this.enabled)
      {
        yield return (object) 2f;
        for (int i = 0; i < ForsakenCitySatellite.Code.Length && this.enabled; ++i)
        {
          this.pulse.Color = this.computerScreen.Color = ForsakenCitySatellite.Colors[ForsakenCitySatellite.Code[i]];
          this.pulseBloom.Visible = this.pulse.Visible = true;
          Audio.Play(ForsakenCitySatellite.Sounds[ForsakenCitySatellite.Code[i]], this.Position + this.computer.Position);
          yield return (object) 0.5f;
          this.pulseBloom.Visible = this.pulse.Visible = false;
          string sfx = i < ForsakenCitySatellite.Code.Length - 1 ? "event:/game/01_forsaken_city/console_static_short" : "event:/game/01_forsaken_city/console_static_long";
          Audio.Play(sfx, this.Position + this.computer.Position);
          yield return (object) 0.2f;
          sfx = (string) null;
        }
        this.Add((Component) Alarm.Create(Alarm.AlarmMode.Oneshot, (Action) (() =>
        {
          if (!this.enabled)
            return;
          this.birdThrustSfx.Position = this.birdFlyPosition - this.Position;
          this.birdThrustSfx.Play("event:/game/01_forsaken_city/birdbros_thrust", (string) null, 0.0f);
        }), 1.1f, true));
        this.birds.Shuffle<ForsakenCitySatellite.CodeBird>();
        foreach (ForsakenCitySatellite.CodeBird bird1 in this.birds)
        {
          ForsakenCitySatellite.CodeBird bird = bird1;
          if (this.enabled)
          {
            bird.Dash();
            yield return (object) 0.02f;
          }
          bird = (ForsakenCitySatellite.CodeBird) null;
        }
      }
      this.pulseBloom.Visible = this.pulse.Visible = false;
    }

    private IEnumerator UnlockGem()
    {
      this.level.Session.SetFlag("unlocked_satellite", true);
      this.birdFinishSfx.Position = this.birdFlyPosition - this.Position;
      this.birdFinishSfx.Play("event:/game/01_forsaken_city/birdbros_finish", (string) null, 0.0f);
      this.staticLoopSfx.Play("event:/game/01_forsaken_city/console_static_loop", (string) null, 0.0f);
      this.enabled = false;
      yield return (object) 0.25f;
      this.level.Displacement.Clear();
      yield return (object) null;
      this.birdFlyingSfx.Stop(true);
      this.level.Frozen = true;
      this.Tag = (int) Tags.FrozenUpdate;
      BloomPoint bloom = new BloomPoint(this.birdFlyPosition - this.Position, 0.0f, 32f);
      this.Add((Component) bloom);
      foreach (ForsakenCitySatellite.CodeBird bird1 in this.birds)
      {
        ForsakenCitySatellite.CodeBird bird = bird1;
        bird.Transform(3f);
        bird = (ForsakenCitySatellite.CodeBird) null;
      }
      while ((double) bloom.Alpha < 1.0)
      {
        bloom.Alpha += Engine.DeltaTime / 3f;
        yield return (object) null;
      }
      yield return (object) 0.25f;
      foreach (ForsakenCitySatellite.CodeBird bird1 in this.birds)
      {
        ForsakenCitySatellite.CodeBird bird = bird1;
        bird.RemoveSelf();
        bird = (ForsakenCitySatellite.CodeBird) null;
      }
      ParticleSystem particles = new ParticleSystem(-10000, 100);
      particles.Tag = (int) Tags.FrozenUpdate;
      particles.Emit(BirdNPC.P_Feather, 24, this.birdFlyPosition, new Vector2(4f, 4f));
      this.level.Add((Entity) particles);
      HeartGem gem = new HeartGem(this.birdFlyPosition);
      gem.Tag = (int) Tags.FrozenUpdate;
      this.level.Add((Entity) gem);
      yield return (object) null;
      gem.ScaleWiggler.Start();
      yield return (object) 0.85f;
      SimpleCurve curve = new SimpleCurve(gem.Position, this.gemSpawnPosition, (gem.Position + this.gemSpawnPosition) / 2f + new Vector2(0.0f, -64f));
      for (float t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime)
      {
        yield return (object) null;
        gem.Position = curve.GetPoint(Ease.CubeInOut(t));
      }
      yield return (object) 0.5f;
      particles.RemoveSelf();
      this.Remove((Component) bloom);
      this.level.Frozen = false;
    }

    private class CodeBird : Entity
    {
      private float timer = Calc.Random.NextFloat();
      private Sprite sprite;
      private Coroutine routine;
      private Vector2 speed;
      private Monocle.Image heartImage;
      private readonly string code;
      private readonly Vector2 origin;
      private readonly Vector2 dash;

      public CodeBird(Vector2 origin, string code)
        : base(origin)
      {
        this.code = code;
        this.origin = origin;
        this.Add((Component) (this.sprite = new Sprite(GFX.Game, "scenery/flutterbird/")));
        this.sprite.AddLoop("fly", "flap", 0.08f);
        this.sprite.Play("fly", false, false);
        this.sprite.CenterOrigin();
        this.sprite.Color = ForsakenCitySatellite.Colors[code];
        Vector2 zero = Vector2.Zero;
        zero.X = code.Contains<char>('L') ? -1f : (code.Contains<char>('R') ? 1f : 0.0f);
        zero.Y = code.Contains<char>('U') ? -1f : (code.Contains<char>('D') ? 1f : 0.0f);
        this.dash = zero.SafeNormalize();
        this.Add((Component) (this.routine = new Coroutine(this.AimlessFlightRoutine(), true)));
      }

      public override void Update()
      {
        this.timer += Engine.DeltaTime;
        this.sprite.Y = (float) Math.Sin((double) this.timer * 2.0);
        base.Update();
      }

      public void Dash()
      {
        this.routine.Replace(this.DashRoutine());
      }

      public void Transform(float duration)
      {
        this.Tag = (int) Tags.FrozenUpdate;
        this.routine.Replace(this.TransformRoutine(duration));
      }

      private IEnumerator AimlessFlightRoutine()
      {
        this.speed = Vector2.Zero;
        while (true)
        {
          Vector2 target = this.origin + Calc.AngleToVector(Calc.Random.NextFloat(6.283185f), 16f + Calc.Random.NextFloat(40f));
          float reset = 0.0f;
          while ((double) reset < 1.0 && (double) (target - this.Position).Length() > 8.0)
          {
            Vector2 dir = (target - this.Position).SafeNormalize();
            this.speed += dir * 420f * Engine.DeltaTime;
            if ((double) this.speed.Length() > 90.0)
              this.speed = this.speed.SafeNormalize(90f);
            this.Position = this.Position + this.speed * Engine.DeltaTime;
            reset += Engine.DeltaTime;
            if ((uint) Math.Sign(dir.X) > 0U)
              this.sprite.Scale.X = (float) Math.Sign(dir.X);
            yield return (object) null;
            dir = new Vector2();
          }
          target = new Vector2();
        }
      }

      private IEnumerator DashRoutine()
      {
        for (float t = 0.25f; (double) t > 0.0; t -= Engine.DeltaTime)
        {
          this.speed = Calc.Approach(this.speed, Vector2.Zero, 200f * Engine.DeltaTime);
          this.Position = this.Position + this.speed * Engine.DeltaTime;
          yield return (object) null;
        }
        Vector2 from = this.Position;
        Vector2 to = this.origin + this.dash * 8f;
        if ((uint) Math.Sign(to.X - from.X) > 0U)
          this.sprite.Scale.X = (float) Math.Sign(to.X - from.X);
        for (float t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime * 1.5f)
        {
          this.Position = from + (to - from) * Ease.CubeInOut(t);
          yield return (object) null;
        }
        this.Position = to;
        yield return (object) 0.2f;
        from = new Vector2();
        to = new Vector2();
        if ((double) this.dash.X != 0.0)
          this.sprite.Scale.X = (float) Math.Sign(this.dash.X);
        (this.Scene as Level).Displacement.AddBurst(this.Position, 0.25f, 4f, 24f, 0.4f, (Ease.Easer) null, (Ease.Easer) null);
        this.speed = this.dash * 300f;
        for (float t = 0.4f; (double) t > 0.0; t -= Engine.DeltaTime)
        {
          if ((double) t > 0.100000001490116 && this.Scene.OnInterval(0.02f))
            this.SceneAs<Level>().ParticlesBG.Emit(ForsakenCitySatellite.Particles[this.code], 1, this.Position, Vector2.One * 2f, this.dash.Angle());
          this.speed = Calc.Approach(this.speed, Vector2.Zero, 800f * Engine.DeltaTime);
          this.Position = this.Position + this.speed * Engine.DeltaTime;
          yield return (object) null;
        }
        yield return (object) 0.4f;
        this.routine.Replace(this.AimlessFlightRoutine());
      }

      private IEnumerator TransformRoutine(float duration)
      {
        Color colorFrom = this.sprite.Color;
        Color colorTo = Color.White;
        Vector2 target = this.origin;
        this.Add((Component) (this.heartImage = new Monocle.Image(GFX.Game["collectables/heartGem/shape"])));
        this.heartImage.CenterOrigin();
        this.heartImage.Scale = Vector2.Zero;
        for (float t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime / duration)
        {
          Vector2 dir = (target - this.Position).SafeNormalize();
          this.speed += 400f * dir * Engine.DeltaTime;
          float maxspeed = Math.Max(20f, (float) ((1.0 - (double) t) * 200.0));
          if ((double) this.speed.Length() > (double) maxspeed)
            this.speed = this.speed.SafeNormalize(maxspeed);
          this.Position = this.Position + this.speed * Engine.DeltaTime;
          this.sprite.Color = Color.Lerp(colorFrom, colorTo, t);
          this.heartImage.Scale = Vector2.One * Math.Max(0.0f, (float) (((double) t - 0.75) * 4.0));
          if ((double) dir.X != 0.0)
            this.sprite.Scale.X = Math.Abs(this.sprite.Scale.X) * (float) Math.Sign(dir.X);
          this.sprite.Scale.X = (float) Math.Sign(this.sprite.Scale.X) * (1f - this.heartImage.Scale.X);
          this.sprite.Scale.Y = 1f - this.heartImage.Scale.X;
          yield return (object) null;
          dir = new Vector2();
        }
      }
    }
  }
}

