// Decompiled with JetBrains decompiler
// Type: Celeste.WaveDashPlaybackTutorial
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class WaveDashPlaybackTutorial
  {
    public Action OnRender;
    private bool hasUpdated;
    private float dashTrailTimer;
    private int dashTrailCounter;
    private bool dashing;
    private bool firstDash = true;
    private bool launched;
    private float launchedDelay;
    private float launchedTimer;
    private int tag;
    private Vector2 dashDirection0;
    private Vector2 dashDirection1;

    public PlayerPlayback Playback { get; private set; }

    public WaveDashPlaybackTutorial(
      string name,
      Vector2 offset,
      Vector2 dashDirection0,
      Vector2 dashDirection1)
    {
      List<Player.ChaserState> tutorial = PlaybackData.Tutorials[name];
      this.Playback = new PlayerPlayback(offset, PlayerSpriteMode.MadelineNoBackpack, tutorial);
      this.tag = Calc.Random.Next();
      this.dashDirection0 = dashDirection0;
      this.dashDirection1 = dashDirection1;
    }

    public void Update()
    {
      this.Playback.Update();
      this.Playback.Hair.AfterUpdate();
      if (this.Playback.Sprite.CurrentAnimationID == "dash" && this.Playback.Sprite.CurrentAnimationFrame == 0)
      {
        if (!this.dashing)
        {
          this.dashing = true;
          Celeste.Freeze(0.05f);
          SlashFx.Burst(this.Playback.Center, (this.firstDash ? this.dashDirection0 : this.dashDirection1).Angle()).Tag = this.tag;
          this.dashTrailTimer = 0.1f;
          this.dashTrailCounter = 2;
          this.CreateTrail();
          if (this.firstDash)
            this.launchedDelay = 0.15f;
          this.firstDash = !this.firstDash;
        }
      }
      else
        this.dashing = false;
      if ((double) this.dashTrailTimer > 0.0)
      {
        this.dashTrailTimer -= Engine.DeltaTime;
        if ((double) this.dashTrailTimer <= 0.0)
        {
          this.CreateTrail();
          --this.dashTrailCounter;
          if (this.dashTrailCounter > 0)
            this.dashTrailTimer = 0.1f;
        }
      }
      if ((double) this.launchedDelay > 0.0)
      {
        this.launchedDelay -= Engine.DeltaTime;
        if ((double) this.launchedDelay <= 0.0)
        {
          this.launched = true;
          this.launchedTimer = 0.0f;
        }
      }
      if (this.launched)
      {
        float launchedTimer = this.launchedTimer;
        this.launchedTimer += Engine.DeltaTime;
        if ((double) this.launchedTimer >= 0.5)
        {
          this.launched = false;
          this.launchedTimer = 0.0f;
        }
        else if (Calc.OnInterval(this.launchedTimer, launchedTimer, 0.15f))
        {
          SpeedRing speedRing = Engine.Pooler.Create<SpeedRing>().Init(this.Playback.Center, (this.Playback.Position - this.Playback.LastPosition).Angle(), Color.White);
          speedRing.Tag = this.tag;
          Engine.Scene.Add((Entity) speedRing);
        }
      }
      this.hasUpdated = true;
    }

    public void Render(Vector2 position, float scale)
    {
      Matrix transformationMatrix = Matrix.CreateScale(4f) * Matrix.CreateTranslation(position.X, position.Y, 0.0f);
      Draw.SpriteBatch.End();
      Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, transformationMatrix);
      foreach (Entity entity in Engine.Scene.Tracker.GetEntities<TrailManager.Snapshot>())
      {
        if (entity.Tag == this.tag)
          entity.Render();
      }
      foreach (Entity entity in Engine.Scene.Tracker.GetEntities<SlashFx>())
      {
        if (entity.Tag == this.tag && entity.Visible)
          entity.Render();
      }
      foreach (Entity entity in Engine.Scene.Tracker.GetEntities<SpeedRing>())
      {
        if (entity.Tag == this.tag)
          entity.Render();
      }
      if (this.Playback.Visible && this.hasUpdated)
        this.Playback.Render();
      if (this.OnRender != null)
        this.OnRender();
      Draw.SpriteBatch.End();
      Draw.SpriteBatch.Begin();
    }

    private void CreateTrail() => TrailManager.Add(this.Playback.Position, (Monocle.Image) this.Playback.Sprite, this.Playback.Hair, this.Playback.Sprite.Scale, Player.UsedHairColor, 0).Tag = this.tag;
  }
}
