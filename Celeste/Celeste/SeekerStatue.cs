// Decompiled with JetBrains decompiler
// Type: Celeste.SeekerStatue
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class SeekerStatue : Entity
  {
    private SeekerStatue.Hatch hatch;
    private Sprite sprite;

    public SeekerStatue(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      SeekerStatue seekerStatue = this;
      this.Depth = 8999;
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("seeker")));
      this.sprite.Play("statue", false, false);
      this.sprite.OnLastFrame = (Action<string>) (f =>
      {
        if (!(f == nameof (hatch)))
          return;
        seekerStatue.Scene.Add((Entity) new Seeker(data, offset)
        {
          Light = {
            Alpha = 0.0f
          }
        });
        seekerStatue.RemoveSelf();
      });
      this.hatch = data.Enum<SeekerStatue.Hatch>(nameof (hatch), SeekerStatue.Hatch.Distance);
    }

    public override void Update()
    {
      base.Update();
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity == null || !(this.sprite.CurrentAnimationID == "statue"))
        return;
      bool flag = false;
      if (this.hatch == SeekerStatue.Hatch.Distance && (double) (entity.Position - this.Position).Length() < 220.0)
        flag = true;
      else if (this.hatch == SeekerStatue.Hatch.PlayerRightOfX && (double) entity.X > (double) this.X + 32.0)
        flag = true;
      if (flag)
      {
        this.BreakOutParticles();
        this.sprite.Play("hatch", false, false);
        Audio.Play("event:/game/05_mirror_temple/seeker_statue_break", this.Position);
        Alarm.Set((Entity) this, 0.8f, new Action(this.BreakOutParticles), Alarm.AlarmMode.Oneshot);
      }
    }

    private void BreakOutParticles()
    {
      Level level = this.SceneAs<Level>();
      for (float direction = 0.0f; (double) direction < 6.28318548202515; direction += 0.1745329f)
      {
        Vector2 position = this.Center + Calc.AngleToVector(direction + Calc.Random.Range(-1f * (float) Math.PI / 90f, (float) Math.PI / 90f), (float) Calc.Random.Range(12, 20));
        level.Particles.Emit(Seeker.P_BreakOut, position, direction);
      }
    }

    private enum Hatch
    {
      Distance,
      PlayerRightOfX,
    }
  }
}

