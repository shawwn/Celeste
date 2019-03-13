// Decompiled with JetBrains decompiler
// Type: Celeste.BadelineAutoAnimator
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class BadelineAutoAnimator : Component
  {
    public bool Enabled = true;
    private string lastAnimation;
    private bool wasSyncingSprite;
    private Wiggler pop;

    public BadelineAutoAnimator()
      : base(true, false)
    {
    }

    public override void Added(Entity entity)
    {
      base.Added(entity);
      entity.Add((Component) (this.pop = Wiggler.Create(0.5f, 4f, (Action<float>) (f =>
      {
        Sprite sprite = this.Entity.Get<Sprite>();
        if (sprite == null)
          return;
        sprite.Scale = new Vector2((float) Math.Sign(sprite.Scale.X), 1f) * (float) (1.0 + 0.25 * (double) f);
      }), false, false)));
    }

    public override void Removed(Entity entity)
    {
      entity.Remove((Component) this.pop);
      base.Removed(entity);
    }

    public void SetReturnToAnimation(string anim)
    {
      this.lastAnimation = anim;
    }

    public override void Update()
    {
      Sprite sprite = this.Entity.Get<Sprite>();
      if (this.Scene == null || sprite == null)
        return;
      bool flag = false;
      Textbox entity = this.Scene.Tracker.GetEntity<Textbox>();
      int num;
      if (this.Enabled && entity != null)
        num = entity.PortraitName.IsIgnoreCase("badeline") ? 1 : 0;
      else
        num = 0;
      if (num != 0)
      {
        if (entity.PortraitAnimation.IsIgnoreCase("scoff"))
        {
          if (!this.wasSyncingSprite)
            this.lastAnimation = sprite.CurrentAnimationID;
          sprite.Play("laugh", false, false);
          this.wasSyncingSprite = flag = true;
        }
        else if (entity.PortraitAnimation.IsIgnoreCase("yell", "freakA", "freakB", "freakC"))
        {
          if (!this.wasSyncingSprite)
          {
            this.pop.Start();
            this.lastAnimation = sprite.CurrentAnimationID;
          }
          sprite.Play("angry", false, false);
          this.wasSyncingSprite = flag = true;
        }
      }
      if (this.wasSyncingSprite && !flag)
      {
        this.wasSyncingSprite = false;
        if (string.IsNullOrEmpty(this.lastAnimation) || this.lastAnimation == "spin")
          this.lastAnimation = "fallSlow";
        if (sprite.CurrentAnimationID == "angry")
          this.pop.Start();
        sprite.Play(this.lastAnimation, false, false);
      }
    }
  }
}

