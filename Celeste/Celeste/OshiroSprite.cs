// Decompiled with JetBrains decompiler
// Type: Celeste.OshiroSprite
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class OshiroSprite : Sprite
  {
    public bool AllowSpriteChanges = true;
    public bool AllowTurnInvisible = true;
    private Wiggler wiggler;

    public OshiroSprite(int facing)
    {
      this.Scale.X = (float) facing;
      GFX.SpriteBank.CreateOn((Sprite) this, "oshiro");
    }

    public override void Added(Entity entity)
    {
      base.Added(entity);
      entity.Add((Component) (this.wiggler = Wiggler.Create(0.3f, 2f, (Action<float>) (f =>
      {
        this.Scale.X = (float) Math.Sign(this.Scale.X) * (float) (1.0 + (double) f * 0.200000002980232);
        this.Scale.Y = (float) (1.0 - (double) f * 0.200000002980232);
      }), false, false)));
    }

    public override void Update()
    {
      base.Update();
      if (this.AllowSpriteChanges)
      {
        Textbox entity = this.Scene.Tracker.GetEntity<Textbox>();
        if (entity != null)
        {
          if (entity.PortraitName.Equals("oshiro", StringComparison.OrdinalIgnoreCase) && entity.PortraitAnimation.StartsWith("side", StringComparison.OrdinalIgnoreCase))
          {
            if (this.CurrentAnimationID.Equals("idle"))
              this.Pop("side", true);
          }
          else if (this.CurrentAnimationID.Equals("side"))
            this.Pop("idle", true);
        }
      }
      if (!this.AllowTurnInvisible || !this.Visible)
        return;
      Level scene = this.Scene as Level;
      int num1;
      if ((double) this.RenderPosition.X > (double) (scene.Bounds.Left - 8))
      {
        double y1 = (double) this.RenderPosition.Y;
        Rectangle bounds = scene.Bounds;
        double num2 = (double) (bounds.Top - 8);
        if (y1 > num2)
        {
          double x = (double) this.RenderPosition.X;
          bounds = scene.Bounds;
          double num3 = (double) (bounds.Right + 8);
          if (x < num3)
          {
            double y2 = (double) this.RenderPosition.Y;
            bounds = scene.Bounds;
            double num4 = (double) (bounds.Bottom + 16);
            num1 = y2 < num4 ? 1 : 0;
            goto label_16;
          }
        }
      }
      num1 = 0;
label_16:
      this.Visible = num1 != 0;
    }

    public void Wiggle()
    {
      this.wiggler.Start();
    }

    public void Pop(string name, bool flip)
    {
      if (this.CurrentAnimationID.Equals(name))
        return;
      this.Play(name, false, false);
      if (flip)
      {
        this.Scale.X = -this.Scale.X;
        if ((double) this.Scale.X < 0.0)
          Audio.Play("event:/char/oshiro/chat_turn_left", this.Entity.Position);
        else
          Audio.Play("event:/char/oshiro/chat_turn_right", this.Entity.Position);
      }
      this.wiggler.Start();
    }
  }
}

