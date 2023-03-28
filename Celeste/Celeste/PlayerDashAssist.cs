// Decompiled with JetBrains decompiler
// Type: Celeste.PlayerDashAssist
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  [Tracked(false)]
  public class PlayerDashAssist : Entity
  {
    public float Direction;
    public float Scale;
    public Vector2 Offset;
    private List<MTexture> images;
    private EventInstance snapshot;
    private float timer;
    private bool paused;
    private int lastIndex;

    public PlayerDashAssist()
    {
      this.Tag = (int) Tags.Global;
      this.Depth = -1000000;
      this.Visible = false;
      this.images = GFX.Game.GetAtlasSubtextures("util/dasharrow/dasharrow");
    }

    public override void Update()
    {
      if (!Engine.DashAssistFreeze)
      {
        if (!this.paused)
          return;
        if (!this.Scene.Paused)
          Audio.PauseGameplaySfx = false;
        this.DisableSnapshot();
        this.timer = 0.0f;
        this.paused = false;
      }
      else
      {
        this.paused = true;
        Audio.PauseGameplaySfx = true;
        this.timer += Engine.RawDeltaTime;
        if ((double) this.timer > 0.20000000298023224 && (HandleBase) this.snapshot == (HandleBase) null)
          this.EnableSnapshot();
        Player entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity == null)
          return;
        float num1 = Input.GetAimVector(entity.Facing).Angle();
        if ((double) Calc.AbsAngleDiff(num1, this.Direction) >= 1.5807963609695435)
        {
          this.Direction = num1;
          this.Scale = 0.0f;
        }
        else
          this.Direction = Calc.AngleApproach(this.Direction, num1, 18.849556f * Engine.RawDeltaTime);
        this.Scale = Calc.Approach(this.Scale, 1f, Engine.DeltaTime * 4f);
        int num2 = 1 + (8 + (int) Math.Round((double) num1 / 0.7853981852531433)) % 8;
        if (this.lastIndex != 0 && this.lastIndex != num2)
          Audio.Play("event:/game/general/assist_dash_aim", entity.Center, "dash_direction", (float) num2);
        this.lastIndex = num2;
      }
    }

    public override void Render()
    {
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity == null || !Engine.DashAssistFreeze)
        return;
      MTexture mtexture = (MTexture) null;
      float rotation = float.MaxValue;
      for (int index = 0; index < 8; ++index)
      {
        float num = Calc.AngleDiff((float) (6.2831854820251465 * ((double) index / 8.0)), this.Direction);
        if ((double) Math.Abs(num) < (double) Math.Abs(rotation))
        {
          rotation = num;
          mtexture = this.images[index];
        }
      }
      if (mtexture == null)
        return;
      if ((double) Math.Abs(rotation) < 0.05000000074505806)
        rotation = 0.0f;
      mtexture.DrawOutlineCentered((entity.Center + this.Offset + Calc.AngleToVector(this.Direction, 20f)).Round(), Color.White, Ease.BounceOut(this.Scale), rotation);
    }

    private void EnableSnapshot()
    {
    }

    private void DisableSnapshot()
    {
      if (!((HandleBase) this.snapshot != (HandleBase) null))
        return;
      Audio.ReleaseSnapshot(this.snapshot);
      this.snapshot = (EventInstance) null;
    }

    public override void Removed(Scene scene)
    {
      this.DisableSnapshot();
      base.Removed(scene);
    }

    public override void SceneEnd(Scene scene)
    {
      this.DisableSnapshot();
      base.SceneEnd(scene);
    }
  }
}
