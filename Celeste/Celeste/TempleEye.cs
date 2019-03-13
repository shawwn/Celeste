// Decompiled with JetBrains decompiler
// Type: Celeste.TempleEye
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class TempleEye : Entity
  {
    private bool bursting = false;
    private MTexture eyeTexture;
    private MTexture pupilTexture;
    private Sprite eyelid;
    private Vector2 pupilPosition;
    private Vector2 pupilTarget;
    private float blinkTimer;
    private bool isBG;

    public TempleEye(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.isBG = !scene.CollideCheck<Solid>(this.Position);
      if (this.isBG)
      {
        this.eyeTexture = GFX.Game["scenery/temple/eye/bg_eye"];
        this.pupilTexture = GFX.Game["scenery/temple/eye/bg_pupil"];
        this.Add((Component) (this.eyelid = new Sprite(GFX.Game, "scenery/temple/eye/bg_lid")));
        this.Depth = 8990;
      }
      else
      {
        this.eyeTexture = GFX.Game["scenery/temple/eye/fg_eye"];
        this.pupilTexture = GFX.Game["scenery/temple/eye/fg_pupil"];
        this.Add((Component) (this.eyelid = new Sprite(GFX.Game, "scenery/temple/eye/fg_lid")));
        this.Depth = -10001;
      }
      this.eyelid.AddLoop("open", "", 0.0f, new int[1]);
      this.eyelid.Add("blink", "", 0.08f, "open", 0, 1, 1, 2, 3, 0);
      this.eyelid.Play("open", false, false);
      this.eyelid.CenterOrigin();
      this.SetBlinkTimer();
    }

    private void SetBlinkTimer()
    {
      this.blinkTimer = Calc.Random.Range(1f, 15f);
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      TheoCrystal entity = this.Scene.Tracker.GetEntity<TheoCrystal>();
      if (entity == null)
        return;
      this.pupilTarget = (entity.Center - this.Position).SafeNormalize();
      this.pupilPosition = this.pupilTarget * 3f;
    }

    public override void Update()
    {
      if (!this.bursting)
      {
        this.pupilPosition = Calc.Approach(this.pupilPosition, this.pupilTarget * 3f, Engine.DeltaTime * 16f);
        TheoCrystal entity = this.Scene.Tracker.GetEntity<TheoCrystal>();
        if (entity != null)
        {
          this.pupilTarget = (entity.Center - this.Position).SafeNormalize();
          if (this.Scene.OnInterval(0.25f) && Calc.Random.Chance(0.01f))
            this.eyelid.Play("blink", false, false);
        }
        this.blinkTimer -= Engine.DeltaTime;
        if ((double) this.blinkTimer <= 0.0)
        {
          this.SetBlinkTimer();
          this.eyelid.Play("blink", false, false);
        }
      }
      base.Update();
    }

    public void Burst()
    {
      this.bursting = true;
      Sprite sprite = new Sprite(GFX.Game, this.isBG ? "scenery/temple/eye/bg_burst" : "scenery/temple/eye/fg_burst");
      sprite.Add("burst", "", 0.08f);
      sprite.Play("burst", false, false);
      sprite.OnLastFrame = (Action<string>) (f => this.RemoveSelf());
      sprite.CenterOrigin();
      this.Add((Component) sprite);
      this.Remove((Component) this.eyelid);
    }

    public override void Render()
    {
      if (!this.bursting)
      {
        this.eyeTexture.DrawCentered(this.Position);
        this.pupilTexture.DrawCentered(this.Position + this.pupilPosition);
      }
      base.Render();
    }
  }
}

