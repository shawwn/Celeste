// Decompiled with JetBrains decompiler
// Type: Celeste.UnlockedRemixDisplay
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class UnlockedRemixDisplay : Entity
  {
    private float drawLerp = 0.0f;
    private const float DisplayDuration = 4f;
    private const float LerpInSpeed = 1.2f;
    private const float LerpOutSpeed = 2f;
    private const float IconSize = 128f;
    private const float Spacing = 20f;
    private string text;
    private MTexture bg;
    private MTexture icon;
    private float rotation;
    private bool unlockedRemix;
    private TotalStrawberriesDisplay strawberries;
    private Wiggler wiggler;
    private bool hasCassetteAlready;

    public UnlockedRemixDisplay()
    {
      this.Tag = (int) Tags.HUD | (int) Tags.Global | (int) Tags.PauseUpdate | (int) Tags.TransitionUpdate;
      this.bg = GFX.Gui["strawberryCountBG"];
      this.icon = GFX.Gui["collectables/cassette"];
      this.text = Dialog.Clean("ui_remix_unlocked", (Language) null);
      this.Visible = false;
      this.Y = 96f;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.hasCassetteAlready = SaveData.Instance.Areas[AreaData.Get(this.Scene).ID].Cassette;
      this.unlockedRemix = (scene as Level).Session.Cassette;
    }

    public override void Update()
    {
      base.Update();
      if (!this.unlockedRemix && (this.Scene as Level).Session.Cassette)
      {
        this.unlockedRemix = true;
        this.Add((Component) new Coroutine(this.DisplayRoutine(), true));
      }
      if (!this.Visible)
        return;
      float target = 96f;
      if (Settings.Instance.SpeedrunClock == SpeedrunType.Chapter)
        target += 58f;
      else if (Settings.Instance.SpeedrunClock == SpeedrunType.File)
        target += 78f;
      if (this.strawberries.Visible)
        target += 96f;
      this.Y = Calc.Approach(this.Y, target, Engine.DeltaTime * 800f);
    }

    private IEnumerator DisplayRoutine()
    {
      this.strawberries = this.Scene.Entities.FindFirst<TotalStrawberriesDisplay>();
      this.Visible = true;
      while ((double) (this.drawLerp += Engine.DeltaTime * 1.2f) < 1.0)
        yield return (object) null;
      this.Add((Component) (this.wiggler = Wiggler.Create(0.8f, 4f, (Action<float>) (f => this.rotation = f * 0.1f), true, false)));
      this.drawLerp = 1f;
      yield return (object) 4f;
      while ((double) (this.drawLerp -= Engine.DeltaTime * 2f) > 0.0)
        yield return (object) null;
      this.Visible = false;
      this.RemoveSelf();
    }

    public override void Render()
    {
      float x = !this.hasCassetteAlready ? (float) ((double) ActiveFont.Measure(this.text).X + 128.0 + 80.0) : 188f;
      Vector2 vector2 = Vector2.Lerp(new Vector2(-x, this.Y), new Vector2(0.0f, this.Y), Ease.CubeOut(this.drawLerp));
      this.bg.DrawJustified(vector2 + new Vector2(x, 0.0f), new Vector2(1f, 0.5f));
      Draw.Rect(vector2.X, vector2.Y - (float) (this.bg.Height / 2), (float) ((double) x - (double) this.bg.Width + 1.0), (float) this.bg.Height, Color.Black);
      float scale = 128f / (float) this.icon.Width;
      this.icon.DrawJustified(vector2 + new Vector2((float) (20.0 + (double) this.icon.Width * (double) scale * 0.5), 0.0f), new Vector2(0.5f, 0.5f), Color.White, scale, this.rotation);
      if (this.hasCassetteAlready)
        return;
      ActiveFont.DrawOutline(this.text, vector2 + new Vector2(168f, 0.0f), new Vector2(0.0f, 0.6f), Vector2.One, Color.White, 2f, Color.Black);
    }
  }
}

