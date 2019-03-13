// Decompiled with JetBrains decompiler
// Type: Celeste.SlashFx
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class SlashFx : Entity
  {
    public Sprite Sprite;
    public Vector2 Direction;

    public SlashFx()
    {
      this.Add((Component) (this.Sprite = new Sprite(GFX.Game, "effects/slash/")));
      this.Sprite.Add("play", "", 0.1f, 0, 1, 2, 3);
      this.Sprite.CenterOrigin();
      this.Sprite.OnFinish = (Action<string>) (anim =>
      {
        this.Visible = false;
        this.Active = false;
      });
      this.Depth = -100;
    }

    public override void Update()
    {
      this.Position = this.Position + this.Direction * 8f * Engine.DeltaTime;
      base.Update();
    }

    public static void Burst(Vector2 position, float direction)
    {
      Scene scene = Engine.Scene;
      SlashFx slashFx = scene.Tracker.GetEntity<SlashFx>();
      if (slashFx == null)
        scene.Add((Entity) (slashFx = new SlashFx()));
      slashFx.Position = position;
      slashFx.Sprite.Play("play", true, false);
      slashFx.Sprite.Rotation = direction;
      slashFx.Direction = Calc.AngleToVector(direction, 1f);
      slashFx.Visible = slashFx.Active = true;
    }
  }
}

