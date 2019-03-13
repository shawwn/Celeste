// Decompiled with JetBrains decompiler
// Type: Celeste.TheoCrystalPedestal
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class TheoCrystalPedestal : Solid
  {
    public Monocle.Image sprite;
    public bool DroppedTheo;

    public TheoCrystalPedestal(EntityData data, Vector2 offset)
      : base(data.Position + offset, 32f, 32f, false)
    {
      this.Add((Component) (this.sprite = new Monocle.Image(GFX.Game["characters/theoCrystal/pedestal"])));
      this.EnableAssistModeChecks = false;
      this.sprite.JustifyOrigin(0.5f, 1f);
      this.Depth = 8998;
      this.Collider.Position = new Vector2(-16f, -64f);
      this.Collidable = false;
      this.OnDashCollide = (DashCollision) ((player, direction) =>
      {
        TheoCrystal entity = this.Scene.Tracker.GetEntity<TheoCrystal>();
        entity.OnPedestal = false;
        entity.Speed = new Vector2(0.0f, -300f);
        this.DroppedTheo = true;
        this.Collidable = false;
        (this.Scene as Level).Flash(Color.White, false);
        Celeste.Freeze(0.1f);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
        Audio.Play("event:/game/05_mirror_temple/crystaltheo_break_free", entity.Position);
        return DashCollisionResults.Rebound;
      });
      this.Tag = (int) Tags.TransitionUpdate;
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if ((scene as Level).Session.GetFlag("foundTheoInCrystal"))
      {
        this.DroppedTheo = true;
      }
      else
      {
        TheoCrystal first = this.Scene.Entities.FindFirst<TheoCrystal>();
        if (first != null)
          first.Depth = this.Depth + 1;
      }
    }

    public override void Update()
    {
      TheoCrystal entity = this.Scene.Tracker.GetEntity<TheoCrystal>();
      if (entity != null && !this.DroppedTheo)
      {
        entity.Position = this.Position + new Vector2(0.0f, -32f);
        entity.OnPedestal = true;
      }
      base.Update();
    }
  }
}

