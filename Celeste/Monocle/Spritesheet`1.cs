// Decompiled with JetBrains decompiler
// Type: Monocle.Spritesheet`1
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Collections.Generic;

namespace Monocle
{
  public class Spritesheet<T> : Image
  {
    public float Rate = 1f;
    public int CurrentFrame;
    public bool UseRawDeltaTime;
    public Action<T> OnFinish;
    public Action<T> OnLoop;
    public Action<T> OnAnimate;
    private Dictionary<T, Spritesheet<T>.Animation> animations;
    private Spritesheet<T>.Animation currentAnimation;
    private float animationTimer;
    private bool played;

    public Spritesheet(MTexture texture, int frameWidth, int frameHeight, int frameSep = 0)
      : base(texture, true)
    {
      this.SetFrames(texture, frameWidth, frameHeight, frameSep);
      this.animations = new Dictionary<T, Spritesheet<T>.Animation>();
    }

    public void SetFrames(MTexture texture, int frameWidth, int frameHeight, int frameSep = 0)
    {
      List<MTexture> mtextureList = new List<MTexture>();
      int x = 0;
      int y = 0;
      while (y <= texture.Height - frameHeight)
      {
        for (; x <= texture.Width - frameWidth; x += frameWidth + frameSep)
          mtextureList.Add(texture.GetSubtexture(x, y, frameWidth, frameHeight, (MTexture) null));
        y += frameHeight + frameSep;
        x = 0;
      }
      this.Frames = mtextureList.ToArray();
    }

    public override void Update()
    {
      if (!this.Animating || (double) this.currentAnimation.Delay <= 0.0)
        return;
      if (this.UseRawDeltaTime)
        this.animationTimer += Engine.RawDeltaTime * this.Rate;
      else
        this.animationTimer += Engine.DeltaTime * this.Rate;
      if ((double) Math.Abs(this.animationTimer) < (double) this.currentAnimation.Delay)
        return;
      this.CurrentAnimationFrame += Math.Sign(this.animationTimer);
      this.animationTimer -= (float) Math.Sign(this.animationTimer) * this.currentAnimation.Delay;
      if (this.CurrentAnimationFrame < 0 || this.CurrentAnimationFrame >= this.currentAnimation.Frames.Length)
      {
        if (this.currentAnimation.Loop)
        {
          this.CurrentAnimationFrame -= Math.Sign(this.CurrentAnimationFrame) * this.currentAnimation.Frames.Length;
          this.CurrentFrame = this.currentAnimation.Frames[this.CurrentAnimationFrame];
          if (this.OnAnimate != null)
            this.OnAnimate(this.CurrentAnimationID);
          if (this.OnLoop == null)
            return;
          this.OnLoop(this.CurrentAnimationID);
        }
        else
        {
          this.CurrentAnimationFrame = this.CurrentAnimationFrame >= 0 ? this.currentAnimation.Frames.Length - 1 : 0;
          this.Animating = false;
          this.animationTimer = 0.0f;
          if (this.OnFinish == null)
            return;
          this.OnFinish(this.CurrentAnimationID);
        }
      }
      else
      {
        this.CurrentFrame = this.currentAnimation.Frames[this.CurrentAnimationFrame];
        if (this.OnAnimate == null)
          return;
        this.OnAnimate(this.CurrentAnimationID);
      }
    }

    public override void Render()
    {
      this.Texture = this.Frames[this.CurrentFrame];
      base.Render();
    }

    public void Add(T id, bool loop, float delay, params int[] frames)
    {
      this.animations[id] = new Spritesheet<T>.Animation()
      {
        Delay = delay,
        Frames = frames,
        Loop = loop
      };
    }

    public void Add(T id, float delay, params int[] frames)
    {
      this.Add(id, true, delay, frames);
    }

    public void Add(T id, int frame)
    {
      this.Add(id, false, 0.0f, frame);
    }

    public void ClearAnimations()
    {
      this.animations.Clear();
    }

    public bool IsPlaying(T id)
    {
      if (!this.played)
        return false;
      if ((object) this.CurrentAnimationID == null)
        return (object) id == null;
      return this.CurrentAnimationID.Equals((object) id);
    }

    public void Play(T id, bool restart = false)
    {
      if (!(!this.IsPlaying(id) | restart))
        return;
      this.CurrentAnimationID = id;
      this.currentAnimation = this.animations[id];
      this.animationTimer = 0.0f;
      this.CurrentAnimationFrame = 0;
      this.played = true;
      this.Animating = this.currentAnimation.Frames.Length > 1;
      this.CurrentFrame = this.currentAnimation.Frames[0];
    }

    public void Reverse(T id, bool restart = false)
    {
      this.Play(id, restart);
      if ((double) this.Rate <= 0.0)
        return;
      this.Rate *= -1f;
    }

    public void Stop()
    {
      this.Animating = false;
      this.played = false;
    }

    public MTexture[] Frames { get; private set; }

    public bool Animating { get; private set; }

    public T CurrentAnimationID { get; private set; }

    public int CurrentAnimationFrame { get; private set; }

    public override float Width
    {
      get
      {
        if (this.Frames.Length != 0)
          return (float) this.Frames[0].Width;
        return 0.0f;
      }
    }

    public override float Height
    {
      get
      {
        if (this.Frames.Length != 0)
          return (float) this.Frames[0].Height;
        return 0.0f;
      }
    }

    private struct Animation
    {
      public float Delay;
      public int[] Frames;
      public bool Loop;
    }
  }
}
