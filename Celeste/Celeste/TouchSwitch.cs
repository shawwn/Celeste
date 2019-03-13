// Decompiled with JetBrains decompiler
// Type: Celeste.TouchSwitch
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class TouchSwitch : Entity
  {
    private MTexture border = GFX.Game["objects/touchswitch/container"];
    private Sprite icon = new Sprite(GFX.Game, "objects/touchswitch/icon");
    private Color inactiveColor = Calc.HexToColor("5fcde4");
    private Color activeColor = Color.White;
    private Color finishColor = Calc.HexToColor("f141df");
    private Vector2 pulse = Vector2.One;
    private float timer = 0.0f;
    public static ParticleType P_Fire;
    public static ParticleType P_FireWhite;
    public Switch Switch;
    private SoundSource touchSfx;
    private float ease;
    private Wiggler wiggler;
    private BloomPoint bloom;

    private Level level
    {
      get
      {
        return (Level) this.Scene;
      }
    }

    public TouchSwitch(Vector2 position)
      : base(position)
    {
      this.Depth = 2000;
      this.Add((Component) (this.Switch = new Switch(false)));
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) new Hitbox(30f, 30f, -15f, -15f)));
      this.Add((Component) this.icon);
      this.Add((Component) (this.bloom = new BloomPoint(0.0f, 16f)));
      this.bloom.Alpha = 0.0f;
      this.icon.Add("idle", "", 0.0f, new int[1]);
      this.icon.Add("spin", "", 0.1f, new Chooser<string>("spin", 1f), 0, 1, 2, 3, 4, 5);
      this.icon.Play("spin", false, false);
      this.icon.Color = this.inactiveColor;
      this.icon.CenterOrigin();
      this.Collider = (Collider) new Hitbox(16f, 16f, -8f, -8f);
      this.Add((Component) new HoldableCollider(new Action<Holdable>(this.OnHoldable), (Collider) new Hitbox(20f, 20f, -10f, -10f)));
      this.Add((Component) new SeekerCollider(new Action<Seeker>(this.OnSeeker), (Collider) new Hitbox(24f, 24f, -12f, -12f)));
      this.Switch.OnActivate = (Action) (() =>
      {
        this.wiggler.Start();
        for (int index = 0; index < 32; ++index)
        {
          float num = Calc.Random.NextFloat(6.283185f);
          this.level.Particles.Emit(TouchSwitch.P_FireWhite, this.Position + Calc.AngleToVector(num, 6f), num);
        }
        this.icon.Rate = 4f;
      });
      this.Switch.OnFinish = (Action) (() => this.ease = 0.0f);
      this.Switch.OnStartFinished = (Action) (() =>
      {
        this.icon.Rate = 0.1f;
        this.icon.Play("idle", false, false);
        this.icon.Color = this.finishColor;
        this.ease = 1f;
      });
      this.Add((Component) (this.wiggler = Wiggler.Create(0.5f, 4f, (Action<float>) (v => this.pulse = Vector2.One * (float) (1.0 + (double) v * 0.25)), false, false)));
      this.Add((Component) new VertexLight(Color.White, 0.8f, 16, 32));
      this.Add((Component) (this.touchSfx = new SoundSource()));
    }

    public TouchSwitch(EntityData data, Vector2 offset)
      : this(data.Position + offset)
    {
    }

    public void TurnOn()
    {
      if (this.Switch.Activated)
        return;
      this.touchSfx.Play("event:/game/general/touchswitch_any", (string) null, 0.0f);
      if (this.Switch.Activate())
      {
        SoundEmitter.Play("event:/game/general/touchswitch_last_oneshot");
        this.Add((Component) new SoundSource("event:/game/general/touchswitch_last_cutoff"));
      }
    }

    private void OnPlayer(Player player)
    {
      this.TurnOn();
    }

    private void OnHoldable(Holdable h)
    {
      this.TurnOn();
    }

    private void OnSeeker(Seeker seeker)
    {
      if (!this.SceneAs<Level>().InsideCamera(this.Position, 10f))
        return;
      this.TurnOn();
    }

    public override void Update()
    {
      this.timer += Engine.DeltaTime * 8f;
      this.ease = Calc.Approach(this.ease, this.Switch.Finished || this.Switch.Activated ? 1f : 0.0f, Engine.DeltaTime * 2f);
      this.icon.Color = Color.Lerp(this.inactiveColor, this.Switch.Finished ? this.finishColor : this.activeColor, this.ease);
      Sprite icon = this.icon;
      icon.Color = icon.Color * (float) (0.5 + (Math.Sin((double) this.timer) + 1.0) / 2.0 * (1.0 - (double) this.ease) * 0.5 + 0.5 * (double) this.ease);
      this.bloom.Alpha = this.ease;
      if (this.Switch.Finished)
      {
        if ((double) this.icon.Rate > 0.100000001490116)
        {
          this.icon.Rate -= 2f * Engine.DeltaTime;
          if ((double) this.icon.Rate <= 0.100000001490116)
          {
            this.icon.Rate = 0.1f;
            this.wiggler.Start();
            this.icon.Play("idle", false, false);
            this.level.Displacement.AddBurst(this.Position, 0.6f, 4f, 28f, 0.2f, (Ease.Easer) null, (Ease.Easer) null);
          }
        }
        else if (this.Scene.OnInterval(0.03f))
        {
          Vector2 position = this.Position + new Vector2(0.0f, 1f) + Calc.AngleToVector(Calc.Random.NextAngle(), 5f);
          this.level.ParticlesBG.Emit(TouchSwitch.P_Fire, position);
        }
      }
      base.Update();
    }

    public override void Render()
    {
      this.border.DrawCentered(this.Position + new Vector2(0.0f, -1f), Color.Black);
      this.border.DrawCentered(this.Position, this.icon.Color, this.pulse);
      base.Render();
    }
  }
}

