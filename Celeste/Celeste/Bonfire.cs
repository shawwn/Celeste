// Decompiled with JetBrains decompiler
// Type: Celeste.Bonfire
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class Bonfire : Entity
  {
    private float brightness = 0.0f;
    private float multiplier = 0.0f;
    private Bonfire.Mode mode;
    private Sprite sprite;
    private VertexLight light;
    private BloomPoint bloom;
    private Wiggler wiggle;
    public bool Activated;
    private SoundSource loopSfx;

    public Bonfire(Vector2 position, Bonfire.Mode mode)
    {
      this.Tag = (int) Tags.TransitionUpdate;
      this.Depth = -5;
      this.Add((Component) (this.loopSfx = new SoundSource()));
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("campfire")));
      this.Add((Component) (this.light = new VertexLight(new Vector2(0.0f, -6f), Color.PaleVioletRed, 1f, 32, 64)));
      this.Add((Component) (this.bloom = new BloomPoint(new Vector2(0.0f, -6f), 1f, 32f)));
      this.Add((Component) (this.wiggle = Wiggler.Create(0.2f, 4f, (Action<float>) (f => this.light.Alpha = this.bloom.Alpha = Math.Min(1f, this.brightness + f * 0.25f) * this.multiplier), false, false)));
      this.Position = position;
      this.mode = mode;
    }

    public Bonfire(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Enum<Bonfire.Mode>(nameof (mode), Bonfire.Mode.Unlit))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.SetMode(this.mode);
    }

    public void SetMode(Bonfire.Mode mode)
    {
      this.mode = mode;
      switch (mode)
      {
        case Bonfire.Mode.Lit:
          if (this.Activated)
          {
            Audio.Play("event:/env/local/campfire_start", this.Position);
            this.loopSfx.Play("event:/env/local/campfire_loop", (string) null, 0.0f);
            this.sprite.Play(this.SceneAs<Level>().Session.Dreaming ? "startDream" : "start", false, false);
            break;
          }
          this.loopSfx.Play("event:/env/local/campfire_loop", (string) null, 0.0f);
          this.sprite.Play(this.SceneAs<Level>().Session.Dreaming ? "burnDream" : "burn", false, false);
          break;
        case Bonfire.Mode.Smoking:
          this.sprite.Play("smoking", false, false);
          break;
        default:
          this.sprite.Play("idle", false, false);
          this.bloom.Alpha = this.light.Alpha = this.brightness = 0.0f;
          break;
      }
      this.Activated = true;
    }

    public override void Update()
    {
      if (this.mode == Bonfire.Mode.Lit)
      {
        this.multiplier = Calc.Approach(this.multiplier, 1f, Engine.DeltaTime * 2f);
        if (this.Scene.OnInterval(0.25f))
        {
          this.brightness = 0.5f + Calc.Random.NextFloat(0.5f);
          this.wiggle.Start();
        }
      }
      base.Update();
    }

    public enum Mode
    {
      Unlit,
      Lit,
      Smoking,
    }
  }
}

