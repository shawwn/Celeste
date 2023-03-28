// Decompiled with JetBrains decompiler
// Type: Celeste.PlayerDeadBody
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class PlayerDeadBody : Entity
  {
    public Action DeathAction;
    public float ActionDelay;
    public bool HasGolden;
    private Color initialHairColor;
    private Vector2 bounce = Vector2.Zero;
    private Player player;
    private PlayerHair hair;
    private PlayerSprite sprite;
    private VertexLight light;
    private DeathEffect deathEffect;
    private Facings facing;
    private float scale = 1f;
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
      this.sprite.Color = Color.White;
      this.initialHairColor = this.hair.Color;
      this.bounce = direction;
      this.Add((Component) new Coroutine(this.DeathRoutine()));
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (!(this.bounce != Vector2.Zero))
        return;
      if ((double) Math.Abs(this.bounce.X) > (double) Math.Abs(this.bounce.Y))
      {
        this.sprite.Play("deadside");
        this.facing = ToFacing.Convert(Math.Sign(this.bounce.X));
      }
      else
      {
        this.bounce = Calc.AngleToVector(Calc.AngleApproach(this.bounce.Angle(), new Vector2((float) -(int) this.player.Facing, 0.0f).Angle(), 0.5f), 1f);
        if ((double) this.bounce.Y < 0.0)
          this.sprite.Play("deadup");
        else
          this.sprite.Play("deaddown");
      }
    }

    private IEnumerator DeathRoutine()
    {
      PlayerDeadBody playerDeadBody1 = this;
      Level level = playerDeadBody1.SceneAs<Level>();
      if (playerDeadBody1.bounce != Vector2.Zero)
      {
        PlayerDeadBody playerDeadBody = playerDeadBody1;
        Audio.Play("event:/char/madeline/predeath", playerDeadBody1.Position);
        playerDeadBody1.scale = 1.5f;
        Celeste.Freeze(0.05f);
        yield return (object) null;
        Vector2 from = playerDeadBody1.Position;
        Vector2 to = from + playerDeadBody1.bounce * 24f;
        Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.5f, true);
        playerDeadBody1.Add((Component) tween);
        tween.OnUpdate = (Action<Tween>) (t =>
        {
          playerDeadBody.Position = from + (to - from) * t.Eased;
          playerDeadBody.scale = (float) (1.5 - (double) t.Eased * 0.5);
          playerDeadBody.sprite.Rotation = (float) (Math.Floor((double) t.Eased * 4.0) * 6.2831854820251465);
        });
        yield return (object) (float) ((double) tween.Duration * 0.75);
        tween.Stop();
        tween = (Tween) null;
      }
      playerDeadBody1.Position = playerDeadBody1.Position + Vector2.UnitY * -5f;
      level.Displacement.AddBurst(playerDeadBody1.Position, 0.3f, 0.0f, 80f);
      level.Shake();
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      Audio.Play(playerDeadBody1.HasGolden ? "event:/new_content/char/madeline/death_golden" : "event:/char/madeline/death", playerDeadBody1.Position);
      playerDeadBody1.deathEffect = new DeathEffect(playerDeadBody1.initialHairColor, new Vector2?(playerDeadBody1.Center - playerDeadBody1.Position));
      // ISSUE: reference to a compiler-generated method
      playerDeadBody1.deathEffect.OnUpdate = new Action<float>(f => this.light.Alpha = 1f - f);
      playerDeadBody1.Add((Component) playerDeadBody1.deathEffect);
      yield return (object) (float) ((double) playerDeadBody1.deathEffect.Duration * 0.6499999761581421);
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
      level.DoScreenWipe(false, this.DeathAction);
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
