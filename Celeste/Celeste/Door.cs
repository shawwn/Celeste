// Decompiled with JetBrains decompiler
// Type: Celeste.Door
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class Door : Entity
  {
    private Sprite sprite;
    private string openSfx;
    private string closeSfx;
    private LightOcclude occlude;
    private bool disabled;

    public Door(EntityData data, Vector2 offset)
    {
      this.Position = data.Position + offset;
      this.Depth = 8999;
      string str = data.Attr("type", "wood");
      if (str == "wood")
      {
        this.Add((Component) (this.sprite = GFX.SpriteBank.Create("door")));
        this.openSfx = "event:/game/03_resort/door_wood_open";
        this.closeSfx = "event:/game/03_resort/door_wood_close";
      }
      else
      {
        this.Add((Component) (this.sprite = GFX.SpriteBank.Create(str + "door")));
        this.openSfx = "event:/game/03_resort/door_metal_open";
        this.closeSfx = "event:/game/03_resort/door_metal_close";
      }
      this.sprite.Play("idle", false, false);
      this.Collider = (Collider) new Hitbox(12f, 24f, -6f, -24f);
      this.Add((Component) (this.occlude = new LightOcclude(new Rectangle(-1, -24, 2, 24), 1f)));
      this.Add((Component) new PlayerCollider(new Action<Player>(this.HitPlayer), (Collider) null, (Collider) null));
    }

    private void HitPlayer(Player player)
    {
      if (this.disabled)
        return;
      this.Open(player.X);
    }

    public void Open(float x)
    {
      if (this.sprite.CurrentAnimationID == "idle")
      {
        Audio.Play(this.openSfx, this.Position);
        this.sprite.Play("open", false, false);
        if ((double) this.X == (double) x)
          return;
        this.sprite.Scale.X = (float) Math.Sign(x - this.X);
      }
      else
      {
        if (!(this.sprite.CurrentAnimationID == "close"))
          return;
        this.sprite.Play("close", true, false);
      }
    }

    public override void Update()
    {
      string currentAnimationId = this.sprite.CurrentAnimationID;
      base.Update();
      this.occlude.Visible = this.sprite.CurrentAnimationID == "idle";
      if (!this.disabled && this.CollideCheck<Solid>())
        this.disabled = true;
      if (!(currentAnimationId == "close") || !(this.sprite.CurrentAnimationID == "idle"))
        return;
      Audio.Play(this.closeSfx, this.Position);
    }
  }
}

