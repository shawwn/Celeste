// Decompiled with JetBrains decompiler
// Type: Celeste.ResortLantern
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class ResortLantern : Entity
  {
    private Monocle.Image holder;
    private Sprite lantern;
    private float collideTimer;
    private int mult;
    private Wiggler wiggler;
    private VertexLight light;
    private BloomPoint bloom;
    private float alphaTimer;
    private SoundSource sfx;

    public ResortLantern(Vector2 position)
      : base(position)
    {
      this.Collider = (Collider) new Hitbox(8f, 8f, -4f, -4f);
      this.Depth = 2000;
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.holder = new Monocle.Image(GFX.Game["objects/resortLantern/holder"]);
      this.holder.CenterOrigin();
      this.Add((Component) this.holder);
      this.lantern = new Sprite(GFX.Game, "objects/resortLantern/");
      this.lantern.AddLoop(nameof (light), nameof (lantern), 0.3f, 0, 0, 1, 2, 1);
      this.lantern.Play(nameof (light), false, false);
      this.lantern.Origin = new Vector2(7f, 7f);
      this.lantern.Position = new Vector2(-1f, -5f);
      this.Add((Component) this.lantern);
      this.wiggler = Wiggler.Create(2.5f, 1.2f, (Action<float>) (v => this.lantern.Rotation = (float) ((double) v * (double) this.mult * (Math.PI / 180.0) * 30.0)), false, false);
      this.wiggler.StartZero = true;
      this.Add((Component) this.wiggler);
      this.Add((Component) (this.light = new VertexLight(Color.White, 0.95f, 32, 64)));
      this.Add((Component) (this.bloom = new BloomPoint(0.8f, 8f)));
      this.Add((Component) (this.sfx = new SoundSource()));
    }

    public ResortLantern(EntityData data, Vector2 offset)
      : this(data.Position + offset)
    {
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (!this.CollideCheck<Solid>(this.Position + Vector2.UnitX * 8f))
        return;
      this.holder.Scale.X = -1f;
      this.lantern.Scale.X = -1f;
      this.lantern.X += 2f;
    }

    public override void Update()
    {
      base.Update();
      if ((double) this.collideTimer > 0.0)
        this.collideTimer -= Engine.DeltaTime;
      this.alphaTimer += Engine.DeltaTime;
      this.bloom.Alpha = this.light.Alpha = (float) (0.949999988079071 + Math.Sin((double) this.alphaTimer * 1.0) * 0.0500000007450581);
    }

    private void OnPlayer(Player player)
    {
      if ((double) this.collideTimer <= 0.0)
      {
        if (!(player.Speed != Vector2.Zero))
          return;
        this.sfx.Play("event:/game/03_resort/lantern_bump", (string) null, 0.0f);
        this.collideTimer = 0.5f;
        this.mult = Calc.Random.Choose<int>(1, -1);
        this.wiggler.Start();
      }
      else
        this.collideTimer = 0.5f;
    }
  }
}

