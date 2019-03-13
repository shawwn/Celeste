﻿// Decompiled with JetBrains decompiler
// Type: Celeste.Maddy3D
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class Maddy3D : Entity
  {
    public Vector2 Scale = Vector2.One;
    public bool Show = true;
    private float alpha = 1f;
    public MountainRenderer Renderer;
    public Billboard Image;
    public Wiggler Wiggler;
    new public Vector3 Position;
    public bool Disabled;
    private List<MTexture> frames;
    private float frame;
    private float frameSpeed;
    private int hideDown;
    private bool running;

    public Maddy3D(MountainRenderer renderer)
    {
      this.Renderer = renderer;
      this.Add((Component) (this.Image = new Billboard((MTexture) null, Vector3.Zero, new Vector2?(), new Color?(), new Vector2?())));
      this.Image.BeforeRender = (Action) (() =>
      {
        if (this.Disabled)
        {
          this.Image.Color = Color.Transparent;
        }
        else
        {
          this.Image.Position = this.Position + (float) this.hideDown * Vector3.Up * (1f - Ease.CubeOut(this.alpha)) * 0.25f;
          this.Image.Scale = this.Scale + Vector2.One * this.Wiggler.Value * this.Scale * 0.2f;
          this.Image.Scale *= (this.Renderer.Model.Camera.Position - this.Position).Length() / 20f;
          this.Image.Color = Color.White * this.alpha;
        }
      });
      this.Add((Component) (this.Wiggler = Wiggler.Create(0.5f, 3f, (Action<float>) null, false, false)));
      this.Running(renderer.Area < 7);
    }

    public void Running(bool backpack = true)
    {
      this.running = true;
      this.Show = true;
      this.hideDown = -1;
      this.SetRunAnim();
      this.frameSpeed = 8f;
      this.frame = 0.0f;
      this.Image.Size = new Vector2((float) this.frames[0].ClipRect.Width, (float) this.frames[0].ClipRect.Height) / (float) this.frames[0].ClipRect.Width;
    }

    public void Falling()
    {
      this.running = false;
      this.Show = true;
      this.hideDown = -1;
      this.frames = GFX.Mountain.GetAtlasSubtextures("marker/Fall");
      this.frameSpeed = 2f;
      this.frame = 0.0f;
      this.Image.Size = new Vector2((float) this.frames[0].ClipRect.Width, (float) this.frames[0].ClipRect.Height) / (float) this.frames[0].ClipRect.Width;
    }

    public void Hide(bool down = true)
    {
      this.running = false;
      this.Show = false;
      this.hideDown = down ? -1 : 1;
    }

    private void SetRunAnim()
    {
      if (this.Renderer.Area < 7)
        this.frames = GFX.Mountain.GetAtlasSubtextures("marker/runBackpack");
      else
        this.frames = GFX.Mountain.GetAtlasSubtextures("marker/runNoBackpack");
    }

    public override void Update()
    {
      base.Update();
      if (this.running)
        this.SetRunAnim();
      if (this.frames != null && this.frames.Count > 0)
      {
        this.frame += Engine.DeltaTime * this.frameSpeed;
        if ((double) this.frame >= (double) this.frames.Count)
          this.frame -= (float) this.frames.Count;
        this.Image.Texture = this.frames[(int) this.frame];
      }
      this.alpha = Calc.Approach(this.alpha, this.Show ? 1f : 0.0f, Engine.DeltaTime * 4f);
    }
  }
}

