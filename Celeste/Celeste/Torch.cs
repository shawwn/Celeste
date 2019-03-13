// Decompiled with JetBrains decompiler
// Type: Celeste.Torch
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class Torch : Entity
  {
    public static readonly Color Color = Color.Lerp(Color.White, Color.Cyan, 0.5f);
    public static readonly Color StartLitColor = Color.Lerp(Color.White, Color.Orange, 0.5f);
    public static ParticleType P_OnLight;
    public const float BloomAlpha = 0.5f;
    public const int StartRadius = 48;
    public const int EndRadius = 64;
    private EntityID id;
    private bool lit;
    private VertexLight light;
    private BloomPoint bloom;
    private bool startLit;
    private Sprite sprite;

    public Torch(EntityID id, Vector2 position, bool startLit)
      : base(position)
    {
      this.id = id;
      this.startLit = startLit;
      this.Collider = (Collider) new Hitbox(32f, 32f, -16f, -16f);
      this.Add((Component) new PlayerCollider(new Action<Player>(this.OnPlayer), (Collider) null, (Collider) null));
      this.Add((Component) (this.light = new VertexLight(Torch.Color, 1f, 48, 64)));
      this.Add((Component) (this.bloom = new BloomPoint(0.5f, 8f)));
      this.bloom.Visible = false;
      this.light.Visible = false;
      this.Depth = 2000;
      if (startLit)
      {
        this.light.Color = Torch.StartLitColor;
        this.Add((Component) (this.sprite = GFX.SpriteBank.Create("litTorch")));
      }
      else
        this.Add((Component) (this.sprite = GFX.SpriteBank.Create("torch")));
    }

    public Torch(EntityData data, Vector2 offset, EntityID id)
      : this(id, data.Position + offset, data.Bool(nameof (startLit), false))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if (!this.startLit && !this.SceneAs<Level>().Session.GetFlag(this.FlagName))
        return;
      this.bloom.Visible = this.light.Visible = true;
      this.lit = true;
      this.Collidable = false;
      this.sprite.Play("on", false, false);
    }

    private void OnPlayer(Player player)
    {
      if (this.lit)
        return;
      Audio.Play("event:/game/05_mirror_temple/torch_activate", this.Position);
      this.lit = true;
      this.bloom.Visible = true;
      this.light.Visible = true;
      this.Collidable = false;
      this.sprite.Play("turnOn", false, false);
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.BackOut, 1f, true);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        this.light.Color = Color.Lerp(Color.White, Torch.Color, t.Eased);
        this.light.StartRadius = (float) (48.0 + (1.0 - (double) t.Eased) * 32.0);
        this.light.EndRadius = (float) (64.0 + (1.0 - (double) t.Eased) * 32.0);
        this.bloom.Alpha = (float) (0.5 + 0.5 * (1.0 - (double) t.Eased));
      });
      this.Add((Component) tween);
      this.SceneAs<Level>().Session.SetFlag(this.FlagName, true);
      this.SceneAs<Level>().ParticlesFG.Emit(Torch.P_OnLight, 12, this.Position, new Vector2(3f, 3f));
    }

    private string FlagName
    {
      get
      {
        return "torch_" + this.id.Key;
      }
    }
  }
}

