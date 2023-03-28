// Decompiled with JetBrains decompiler
// Type: Celeste.PlayerPlayback
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class PlayerPlayback : Entity
  {
    public Vector2 LastPosition;
    public List<Player.ChaserState> Timeline;
    public PlayerSprite Sprite;
    public PlayerHair Hair;
    private Vector2 start;
    private float time;
    private int index;
    private float loopDelay;
    private float startDelay;
    public float TrimStart;
    public float TrimEnd;
    public readonly float Duration;
    private float rangeMinX = float.MinValue;
    private float rangeMaxX = float.MaxValue;
    private bool ShowTrail;

    public Vector2 DashDirection { get; private set; }

    public float Time => this.time;

    public int FrameIndex => this.index;

    public int FrameCount => this.Timeline.Count;

    public PlayerPlayback(EntityData e, Vector2 offset)
      : this(e.Position + offset, PlayerSpriteMode.Playback, PlaybackData.Tutorials[e.Attr("tutorial")])
    {
      if (e.Nodes != null && e.Nodes.Length != 0)
      {
        this.rangeMinX = this.X;
        this.rangeMaxX = this.X;
        foreach (Vector2 vector2 in e.NodesOffset(offset))
        {
          this.rangeMinX = Math.Min(this.rangeMinX, vector2.X);
          this.rangeMaxX = Math.Max(this.rangeMaxX, vector2.X);
        }
      }
      this.startDelay = 1f;
    }

    public PlayerPlayback(
      Vector2 start,
      PlayerSpriteMode sprite,
      List<Player.ChaserState> timeline)
    {
      this.start = start;
      this.Collider = (Collider) new Hitbox(8f, 11f, -4f, -11f);
      this.Timeline = timeline;
      this.Position = start;
      this.time = 0.0f;
      this.index = 0;
      this.Duration = timeline[timeline.Count - 1].TimeStamp;
      this.TrimStart = 0.0f;
      this.TrimEnd = this.Duration;
      this.Sprite = new PlayerSprite(sprite);
      this.Add((Component) (this.Hair = new PlayerHair(this.Sprite)));
      this.Add((Component) this.Sprite);
      this.Collider = (Collider) new Hitbox(8f, 4f, -4f, -4f);
      if (sprite == PlayerSpriteMode.Playback)
        this.ShowTrail = true;
      this.Depth = 9008;
      this.SetFrame(0);
      for (int index = 0; index < 10; ++index)
        this.Hair.AfterUpdate();
      this.Visible = false;
      this.index = this.Timeline.Count;
    }

    private void Restart()
    {
      Audio.Play("event:/new_content/char/tutorial_ghost/appear", this.Position);
      this.Visible = true;
      this.time = this.TrimStart;
      this.index = 0;
      this.loopDelay = 0.25f;
      while ((double) this.time > (double) this.Timeline[this.index].TimeStamp)
        ++this.index;
      this.SetFrame(this.index);
    }

    public void SetFrame(int index)
    {
      Player.ChaserState chaserState = this.Timeline[index];
      string currentAnimationId1 = this.Sprite.CurrentAnimationID;
      bool flag = this.Scene != null && this.CollideCheck<Solid>(this.Position + new Vector2(0.0f, 1f));
      Vector2 dashDirection = this.DashDirection;
      this.Position = this.start + chaserState.Position;
      if (chaserState.Animation != this.Sprite.CurrentAnimationID && chaserState.Animation != null && this.Sprite.Has(chaserState.Animation))
        this.Sprite.Play(chaserState.Animation, true);
      this.Sprite.Scale = chaserState.Scale;
      if ((double) this.Sprite.Scale.X != 0.0)
        this.Hair.Facing = (Facings) Math.Sign(this.Sprite.Scale.X);
      this.Hair.Color = chaserState.HairColor;
      if (this.Sprite.Mode == PlayerSpriteMode.Playback)
        this.Sprite.Color = this.Hair.Color;
      this.DashDirection = chaserState.DashDirection;
      if (this.Scene == null)
        return;
      if (!flag && this.Scene != null && this.CollideCheck<Solid>(this.Position + new Vector2(0.0f, 1f)))
        Audio.Play("event:/new_content/char/tutorial_ghost/land", this.Position);
      if (!(currentAnimationId1 != this.Sprite.CurrentAnimationID))
        return;
      string currentAnimationId2 = this.Sprite.CurrentAnimationID;
      int currentAnimationFrame = this.Sprite.CurrentAnimationFrame;
      if (currentAnimationId2 == "jumpFast" || currentAnimationId2 == "jumpSlow")
      {
        Audio.Play("event:/new_content/char/tutorial_ghost/jump", this.Position);
      }
      else
      {
        switch (currentAnimationId2)
        {
          case "dreamDashIn":
            Audio.Play("event:/new_content/char/tutorial_ghost/dreamblock_sequence", this.Position);
            break;
          case "dash":
            if ((double) this.DashDirection.Y != 0.0)
            {
              Audio.Play("event:/new_content/char/tutorial_ghost/jump_super", this.Position);
              break;
            }
            if ((double) chaserState.Scale.X > 0.0)
            {
              Audio.Play("event:/new_content/char/tutorial_ghost/dash_red_right", this.Position);
              break;
            }
            Audio.Play("event:/new_content/char/tutorial_ghost/dash_red_left", this.Position);
            break;
          default:
            if (currentAnimationId2 == "climbUp" || currentAnimationId2 == "climbDown" || currentAnimationId2 == "wallslide")
            {
              Audio.Play("event:/new_content/char/tutorial_ghost/grab", this.Position);
              break;
            }
            if ((!currentAnimationId2.Equals("runSlow_carry") || currentAnimationFrame != 0 && currentAnimationFrame != 6) && (!currentAnimationId2.Equals("runFast") || currentAnimationFrame != 0 && currentAnimationFrame != 6) && (!currentAnimationId2.Equals("runSlow") || currentAnimationFrame != 0 && currentAnimationFrame != 6) && (!currentAnimationId2.Equals("walk") || currentAnimationFrame != 0 && currentAnimationFrame != 6) && (!currentAnimationId2.Equals("runStumble") || currentAnimationFrame != 6) && (!currentAnimationId2.Equals("flip") || currentAnimationFrame != 4) && (!currentAnimationId2.Equals("runWind") || currentAnimationFrame != 0 && currentAnimationFrame != 6) && (!currentAnimationId2.Equals("idleC") || this.Sprite.Mode != PlayerSpriteMode.MadelineNoBackpack || currentAnimationFrame != 3 && currentAnimationFrame != 6 && currentAnimationFrame != 8 && currentAnimationFrame != 11) && (!currentAnimationId2.Equals("carryTheoWalk") || currentAnimationFrame != 0 && currentAnimationFrame != 6) && (!currentAnimationId2.Equals("push") || currentAnimationFrame != 8 && currentAnimationFrame != 15))
              break;
            Audio.Play("event:/new_content/char/tutorial_ghost/footstep", this.Position);
            break;
        }
      }
    }

    public override void Update()
    {
      if ((double) this.startDelay > 0.0)
        this.startDelay -= Engine.DeltaTime;
      this.LastPosition = this.Position;
      base.Update();
      if (this.index >= this.Timeline.Count - 1 || (double) this.Time >= (double) this.TrimEnd)
      {
        if (this.Visible)
          Audio.Play("event:/new_content/char/tutorial_ghost/disappear", this.Position);
        this.Visible = false;
        this.Position = this.start;
        this.loopDelay -= Engine.DeltaTime;
        if ((double) this.loopDelay <= 0.0)
        {
          Player player = this.Scene == null ? (Player) null : this.Scene.Tracker.GetEntity<Player>();
          if (player == null || (double) player.X > (double) this.rangeMinX && (double) player.X < (double) this.rangeMaxX)
            this.Restart();
        }
      }
      else if ((double) this.startDelay <= 0.0)
      {
        this.SetFrame(this.index);
        this.time += Engine.DeltaTime;
        while (this.index < this.Timeline.Count - 1 && (double) this.time >= (double) this.Timeline[this.index + 1].TimeStamp)
          ++this.index;
      }
      if (!this.Visible || !this.ShowTrail || this.Scene == null || !this.Scene.OnInterval(0.1f))
        return;
      TrailManager.Add(this.Position, (Monocle.Image) this.Sprite, this.Hair, this.Sprite.Scale, this.Hair.Color, this.Depth + 1);
    }
  }
}
