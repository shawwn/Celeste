// Decompiled with JetBrains decompiler
// Type: Celeste.FlutterBird
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
  [Tracked(false)]
  public class FlutterBird : Entity
  {
    private static readonly Color[] colors = new Color[4]
    {
      Calc.HexToColor("89fbff"),
      Calc.HexToColor("f0fc6c"),
      Calc.HexToColor("f493ff"),
      Calc.HexToColor("93baff")
    };
    private Sprite sprite;
    private Vector2 start;
    private Coroutine routine;
    private bool flyingAway;
    private SoundSource tweetingSfx;
    private SoundSource flyawaySfx;

    public FlutterBird(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Depth = -9999;
      this.start = this.Position;
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("flutterbird")));
      this.sprite.Color = Calc.Random.Choose<Color>(FlutterBird.colors);
      this.Add((Component) (this.routine = new Coroutine(this.IdleRoutine(), true)));
      this.Add((Component) (this.flyawaySfx = new SoundSource()));
      this.Add((Component) (this.tweetingSfx = new SoundSource()));
      this.tweetingSfx.Play("event:/game/general/birdbaby_tweet_loop", (string) null, 0.0f);
    }

    public override void Update()
    {
      this.sprite.Scale.X = Calc.Approach(this.sprite.Scale.X, (float) Math.Sign(this.sprite.Scale.X), 4f * Engine.DeltaTime);
      this.sprite.Scale.Y = Calc.Approach(this.sprite.Scale.Y, 1f, 4f * Engine.DeltaTime);
      base.Update();
    }

    private IEnumerator IdleRoutine()
    {
      while (true)
      {
        Player player = this.Scene.Tracker.GetEntity<Player>();
        float delay = 0.25f + Calc.Random.NextFloat(1f);
        for (float p = 0.0f; (double) p < (double) delay; p += Engine.DeltaTime)
        {
          if (player != null && (double) Math.Abs(player.X - this.X) < 48.0 && (double) player.Y > (double) this.Y - 40.0 && (double) player.Y < (double) this.Y + 8.0)
            this.FlyAway(Math.Sign(this.X - player.X), Calc.Random.NextFloat(0.2f));
          yield return (object) null;
        }
        Audio.Play("event:/game/general/birdbaby_hop", this.Position);
        Vector2 target = this.start + new Vector2(Calc.Random.NextFloat(8f) - 4f, 0.0f);
        this.sprite.Scale.X = (float) Math.Sign(target.X - this.Position.X);
        SimpleCurve bezier = new SimpleCurve(this.Position, target, (this.Position + target) / 2f - Vector2.UnitY * 14f);
        for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
        {
          this.Position = bezier.GetPoint(p);
          yield return (object) null;
        }
        this.sprite.Scale.X = (float) Math.Sign(this.sprite.Scale.X) * 1.4f;
        this.sprite.Scale.Y = 0.6f;
        this.Position = target;
        player = (Player) null;
        target = new Vector2();
        bezier = new SimpleCurve();
      }
    }

    private IEnumerator FlyAwayRoutine(int direction, float delay)
    {
      Level level = this.Scene as Level;
      yield return (object) delay;
      this.sprite.Play("fly", false, false);
      this.sprite.Scale.X = (float) -direction * 1.25f;
      this.sprite.Scale.Y = 1.25f;
      level.ParticlesFG.Emit(Calc.Random.Choose<ParticleType>(new ParticleType[1]
      {
        ParticleTypes.Dust
      }), this.Position, -1.570796f);
      Vector2 from = this.Position;
      Vector2 to = this.Position + new Vector2((float) (direction * 4), -8f);
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 3f)
      {
        this.Position = from + (to - from) * Ease.CubeOut(p);
        yield return (object) null;
      }
      from = new Vector2();
      to = new Vector2();
      this.Depth = -10001;
      this.sprite.Scale.X = -this.sprite.Scale.X;
      Vector2 speed = new Vector2((float) direction, -4f) * 8f;
      while ((double) this.Y + 8.0 > (double) level.Bounds.Top)
      {
        speed += new Vector2((float) (direction * 64), (float) sbyte.MinValue) * Engine.DeltaTime;
        this.Position = this.Position + speed * Engine.DeltaTime;
        if (this.Scene.OnInterval(0.1f) && (double) this.Y > (double) level.Camera.Top + 32.0)
        {
          List<Entity> birds = this.Scene.Tracker.GetEntities<FlutterBird>();
          foreach (Entity entity in birds)
          {
            Entity bird = entity;
            if ((double) Math.Abs(this.X - bird.X) < 48.0 && (double) Math.Abs(this.Y - bird.Y) < 48.0 && !(bird as FlutterBird).flyingAway)
              (bird as FlutterBird).FlyAway(direction, Calc.Random.NextFloat(0.25f));
            bird = (Entity) null;
          }
          birds = (List<Entity>) null;
        }
        yield return (object) null;
      }
      speed = new Vector2();
      this.Scene.Remove((Entity) this);
    }

    public void FlyAway(int direction, float delay)
    {
      if (this.flyingAway)
        return;
      this.tweetingSfx.Stop(true);
      this.flyingAway = true;
      this.flyawaySfx.Play("event:/game/general/birdbaby_flyaway", (string) null, 0.0f);
      this.Remove((Component) this.routine);
      this.Add((Component) (this.routine = new Coroutine(this.FlyAwayRoutine(direction, delay), true)));
    }
  }
}

