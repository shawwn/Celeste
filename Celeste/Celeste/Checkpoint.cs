// Decompiled with JetBrains decompiler
// Type: Celeste.Checkpoint
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class Checkpoint : Entity
  {
    private const float LightAlpha = 0.8f;
    private const float BloomAlpha = 0.5f;
    private const float TargetFade = 0.5f;
    private Monocle.Image image;
    private Sprite sprite;
    private Sprite flash;
    private VertexLight light;
    private BloomPoint bloom;
    private bool triggered;
    private float sine = 1.5707964f;
    private float fade = 1f;
    private string bg;
    public Vector2 SpawnOffset;

    public Checkpoint(Vector2 position, string bg = "", Vector2? spawnTarget = null)
      : base(position)
    {
      this.Depth = 9990;
      this.SpawnOffset = spawnTarget.HasValue ? spawnTarget.Value - this.Position : Vector2.Zero;
      this.bg = bg;
    }

    public Checkpoint(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Attr(nameof (bg)), data.FirstNodeNullable(new Vector2?(offset)))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      Level scene1 = this.Scene as Level;
      int id1 = scene1.Session.Area.ID;
      string id2 = !string.IsNullOrWhiteSpace(this.bg) ? "objects/checkpoint/bg/" + this.bg : "objects/checkpoint/bg/" + (object) id1;
      if (GFX.Game.Has(id2))
      {
        this.Add((Component) (this.image = new Monocle.Image(GFX.Game[id2])));
        this.image.JustifyOrigin(0.5f, 1f);
      }
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("checkpoint_highlight")));
      this.sprite.Play("off");
      this.Add((Component) (this.flash = GFX.SpriteBank.Create("checkpoint_flash")));
      this.flash.Visible = false;
      this.flash.Color = Color.White * 0.6f;
      if (!SaveData.Instance.HasCheckpoint(scene1.Session.Area, scene1.Session.Level))
        return;
      this.TurnOn(false);
    }

    public override void Update()
    {
      if (!this.triggered)
      {
        Level scene = this.Scene as Level;
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity != null && !scene.Transitioning)
        {
          if (!entity.CollideCheck<CheckpointBlockerTrigger>() && SaveData.Instance.SetCheckpoint(scene.Session.Area, scene.Session.Level))
          {
            scene.AutoSave();
            this.TurnOn(true);
          }
          this.triggered = true;
        }
      }
      if (this.triggered && this.sprite.CurrentAnimationID == "on")
      {
        this.sine += Engine.DeltaTime * 2f;
        this.fade = Calc.Approach(this.fade, 0.5f, Engine.DeltaTime);
        this.sprite.Color = Color.White * (float) (0.5 + (double) ((float) (1.0 + Math.Sin((double) this.sine)) / 2f) * 0.5) * this.fade;
      }
      base.Update();
    }

    private void TurnOn(bool animate)
    {
      this.triggered = true;
      this.Add((Component) (this.light = new VertexLight(Color.White, 0.0f, 16, 32)));
      this.Add((Component) (this.bloom = new BloomPoint(0.0f, 16f)));
      this.light.Position = new Vector2(0.0f, -8f);
      this.bloom.Position = new Vector2(0.0f, -8f);
      this.flash.Visible = true;
      this.flash.Play("flash", true);
      if (animate)
      {
        this.sprite.Play("turnOn");
        this.Add((Component) new Coroutine(this.EaseLightsOn()));
        this.fade = 1f;
      }
      else
      {
        this.fade = 0.5f;
        this.sprite.Play("on");
        this.sprite.Color = Color.White * 0.5f;
        this.light.Alpha = 0.8f;
        this.bloom.Alpha = 0.5f;
      }
    }

    private IEnumerator EaseLightsOn()
    {
      float lightStartRadius = this.light.StartRadius;
      float lightEndRadius = this.light.EndRadius;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 0.5f)
      {
        float num = Ease.BigBackOut(p);
        this.light.Alpha = 0.8f * num;
        this.light.StartRadius = (float) (int) ((double) lightStartRadius + (double) Calc.YoYo(p) * 8.0);
        this.light.EndRadius = (float) (int) ((double) lightEndRadius + (double) Calc.YoYo(p) * 16.0);
        this.bloom.Alpha = 0.5f * num;
        yield return (object) null;
      }
    }
  }
}
