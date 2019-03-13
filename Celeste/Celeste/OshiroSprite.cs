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
      this.Scale.X = (__Null) (double) facing;
      GFX.SpriteBank.CreateOn((Sprite) this, "oshiro");
    }

    public override void Added(Entity entity)
    {
      base.Added(entity);
      entity.Add((Component) (this.wiggler = Wiggler.Create(0.3f, 2f, (Action<float>) (f =>
      {
        this.Scale.X = (__Null) ((double) Math.Sign((float) this.Scale.X) * (1.0 + (double) f * 0.200000002980232));
        this.Scale.Y = (__Null) (1.0 - (double) f * 0.200000002980232);
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
      // ISSUE: variable of the null type
      __Null x1 = this.RenderPosition.X;
      Rectangle bounds = scene.Bounds;
      double num1 = (double) (((Rectangle) ref bounds).get_Left() - 8);
      int num2;
      if (x1 > num1)
      {
        // ISSUE: variable of the null type
        __Null y1 = this.RenderPosition.Y;
        bounds = scene.Bounds;
        double num3 = (double) (((Rectangle) ref bounds).get_Top() - 8);
        if (y1 > num3)
        {
          // ISSUE: variable of the null type
          __Null x2 = this.RenderPosition.X;
          bounds = scene.Bounds;
          double num4 = (double) (((Rectangle) ref bounds).get_Right() + 8);
          if (x2 < num4)
          {
            // ISSUE: variable of the null type
            __Null y2 = this.RenderPosition.Y;
            bounds = scene.Bounds;
            double num5 = (double) (((Rectangle) ref bounds).get_Bottom() + 16);
            num2 = y2 < num5 ? 1 : 0;
            goto label_13;
          }
        }
      }
      num2 = 0;
label_13:
      this.Visible = num2 != 0;
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
        if (this.Scale.X < 0.0)
          Audio.Play("event:/char/oshiro/chat_turn_left", this.Entity.Position);
        else
          Audio.Play("event:/char/oshiro/chat_turn_right", this.Entity.Position);
      }
      this.wiggler.Start();
    }
  }
}
