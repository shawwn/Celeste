// Decompiled with JetBrains decompiler
// Type: Celeste.PlayerDeadBody
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class PlayerDeadBody : Entity
  {
    private Vector2 bounce = Vector2.Zero;
    private float scale = 1f;
    private bool finished = false;
    public Action DeathAction;
    public float ActionDelay;
    private Color initialHairColor;
    private Player player;
    private PlayerHair hair;
    private PlayerSprite sprite;
    private VertexLight light;
    private DeathEffect deathEffect;
    private Facings facing;

    public PlayerDeadBody(Player player, Vector2 direction)
    {
      this.Depth = -1000000;
      this.player = player;
      this.facing = player.Facing;
      this.Position = player.Position;
      this.Add((Component) (this.hair = player.Hair));
      this.Add((Component) (this.sprite = player.Sprite));
      this.Add((Component) (this.light = player.Light));
      this.sprite.Color = Color.White;
      this.initialHairColor = this.hair.Color;
      this.bounce = direction;
      this.Add((Component) new Coroutine(this.DeathRoutine(), true));
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (!(this.bounce != Vector2.Zero))
        return;
      if ((double) Math.Abs(this.bounce.X) > (double) Math.Abs(this.bounce.Y))
      {
        this.sprite.Play("deadside", false, false);
        this.facing = ToFacing.Convert(-Math.Sign(this.bounce.X));
      }
      else
      {
        this.bounce = Calc.AngleToVector(Calc.AngleApproach(this.bounce.Angle(), new Vector2((float) -(int) this.player.Facing, 0.0f).Angle(), 0.5f), 1f);
        if ((double) this.bounce.Y < 0.0)
          this.sprite.Play("deadup", false, false);
        else
          this.sprite.Play("deaddown", false, false);
      }
    }

    private IEnumerator DeathRoutine()
    {
      Level level = this.SceneAs<Level>();
      if (this.bounce != Vector2.Zero)
      {
        Audio.Play("event:/char/madeline/predeath", this.Position);
        this.scale = 1.5f;
        Celeste.Freeze(0.05f);
        yield return (object) null;
        Vector2 from = this.Position;
        Vector2 to = from + this.bounce * 24f;
        Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.5f, true);
        this.Add((Component) tween);
        tween.OnUpdate = (Action<Tween>) (t =>
        {
          this.Position = from + (to - from) * t.Eased;
          this.scale = (float) (1.5 - (double) t.Eased * 0.5);
          this.sprite.Rotation = (float) (Math.Floor((double) t.Eased * 4.0) * 6.28318548202515);
        });
        yield return (object) (float) ((double) tween.Duration * 0.75);
        tween.Stop();
        tween = (Tween) null;
      }
      this.Position = this.Position + Vector2.UnitY * -5f;
      level.Displacement.AddBurst(this.Position, 0.3f, 0.0f, 80f, 1f, (Ease.Easer) null, (Ease.Easer) null);
      level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      Audio.Play("event:/char/madeline/death", this.Position);
      this.deathEffect = new DeathEffect(this.initialHairColor, new Vector2?(this.Center - this.Position));
      this.deathEffect.OnUpdate = (Action<float>) (f => this.light.Alpha = 1f - f);
      this.Add((Component) this.deathEffect);
      yield return (object) (float) ((double) this.deathEffect.Duration * 0.649999976158142);
      if ((double) this.ActionDelay > 0.0)
        yield return (object) this.ActionDelay;
      this.End();
    }

    private void End()
    {
      if (this.finished)
        return;
      this.finished = true;
      Level level = this.SceneAs<Level>();
      if (this.DeathAction == null)
        this.DeathAction = new Action(level.Reload);
      level.DoScreenWipe(false, this.DeathAction, false);
    }

    public override void Update()
    {
      base.Update();
      if (Input.MenuConfirm.Pressed && !this.finished)
        this.End();
      this.hair.Color = this.sprite.CurrentAnimationFrame == 0 ? Color.White : this.initialHairColor;
    }

    public override void Render()
    {
      if (this.deathEffect == null)
      {
        this.sprite.Scale.X = (float) this.facing * this.scale;
        this.sprite.Scale.Y = this.scale;
        this.hair.Facing = this.facing;
        base.Render();
      }
      else
        this.deathEffect.Render();
    }
  }
}

