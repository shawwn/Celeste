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
    private Vector2 bounce = Vector2.get_Zero();
    private float scale = 1f;
    public Action DeathAction;
    public float ActionDelay;
    private Color initialHairColor;
    private Player player;
    private PlayerHair hair;
    private PlayerSprite sprite;
    private VertexLight light;
    private DeathEffect deathEffect;
    private Facings facing;
    private bool finished;

    public PlayerDeadBody(Player player, Vector2 direction)
    {
      this.Depth = -1000000;
      this.player = player;
      this.facing = player.Facing;
      this.Position = player.Position;
      this.Add((Component) (this.hair = player.Hair));
      this.Add((Component) (this.sprite = player.Sprite));
      this.Add((Component) (this.light = player.Light));
      this.sprite.Color = Color.get_White();
      this.initialHairColor = this.hair.Color;
      this.bounce = direction;
      this.Add((Component) new Coroutine(this.DeathRoutine(), true));
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (!Vector2.op_Inequality(this.bounce, Vector2.get_Zero()))
        return;
      if ((double) Math.Abs((float) this.bounce.X) > (double) Math.Abs((float) this.bounce.Y))
      {
        this.sprite.Play("deadside", false, false);
        this.facing = (Facings) -Math.Sign((float) this.bounce.X);
      }
      else
      {
        this.bounce = Calc.AngleToVector(Calc.AngleApproach(this.bounce.Angle(), new Vector2((float) -(int) this.player.Facing, 0.0f).Angle(), 0.5f), 1f);
        if (this.bounce.Y < 0.0)
          this.sprite.Play("deadup", false, false);
        else
          this.sprite.Play("deaddown", false, false);
      }
    }

    private IEnumerator DeathRoutine()
    {
      PlayerDeadBody playerDeadBody1 = this;
      Level level = playerDeadBody1.SceneAs<Level>();
      if (Vector2.op_Inequality(playerDeadBody1.bounce, Vector2.get_Zero()))
      {
        PlayerDeadBody playerDeadBody = playerDeadBody1;
        Audio.Play("event:/char/madeline/predeath", playerDeadBody1.Position);
        playerDeadBody1.scale = 1.5f;
        Celeste.Celeste.Freeze(0.05f);
        yield return (object) null;
        Vector2 from = playerDeadBody1.Position;
        Vector2 to = Vector2.op_Addition(from, Vector2.op_Multiply(playerDeadBody1.bounce, 24f));
        Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.5f, true);
        playerDeadBody1.Add((Component) tween);
        tween.OnUpdate = (Action<Tween>) (t =>
        {
          playerDeadBody.Position = Vector2.op_Addition(from, Vector2.op_Multiply(Vector2.op_Subtraction(to, from), t.Eased));
          playerDeadBody.scale = (float) (1.5 - (double) t.Eased * 0.5);
          playerDeadBody.sprite.Rotation = (float) (Math.Floor((double) t.Eased * 4.0) * 6.28318548202515);
        });
        yield return (object) (float) ((double) tween.Duration * 0.75);
        tween.Stop();
        tween = (Tween) null;
      }
      playerDeadBody1.Position = Vector2.op_Addition(playerDeadBody1.Position, Vector2.op_Multiply(Vector2.get_UnitY(), -5f));
      level.Displacement.AddBurst(playerDeadBody1.Position, 0.3f, 0.0f, 80f, 1f, (Ease.Easer) null, (Ease.Easer) null);
      level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      Audio.Play("event:/char/madeline/death", playerDeadBody1.Position);
      playerDeadBody1.deathEffect = new DeathEffect(playerDeadBody1.initialHairColor, new Vector2?(Vector2.op_Subtraction(playerDeadBody1.Center, playerDeadBody1.Position)));
      // ISSUE: reference to a compiler-generated method
      playerDeadBody1.deathEffect.OnUpdate = new Action<float>(playerDeadBody1.\u003CDeathRoutine\u003Eb__14_0);
      playerDeadBody1.Add((Component) playerDeadBody1.deathEffect);
      yield return (object) (float) ((double) playerDeadBody1.deathEffect.Duration * 0.649999976158142);
      if ((double) playerDeadBody1.ActionDelay > 0.0)
        yield return (object) playerDeadBody1.ActionDelay;
      playerDeadBody1.End();
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
      this.hair.Color = this.sprite.CurrentAnimationFrame == 0 ? Color.get_White() : this.initialHairColor;
    }

    public override void Render()
    {
      if (this.deathEffect == null)
      {
        this.sprite.Scale.X = (__Null) ((double) this.facing * (double) this.scale);
        this.sprite.Scale.Y = (__Null) (double) this.scale;
        this.hair.Facing = this.facing;
        base.Render();
      }
      else
        this.deathEffect.Render();
    }
  }
}
