// Decompiled with JetBrains decompiler
// Type: Celeste.CS06_BossIntro
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS06_BossIntro : CutsceneEntity
  {
    public const string Flag = "boss_intro";
    private Player player;
    private FinalBoss boss;
    private Vector2 bossEndPosition;
    private BadelineAutoAnimator animator;
    private float playerTargetX;

    public CS06_BossIntro(float playerTargetX, Player player, FinalBoss boss)
      : base(true, false)
    {
      this.player = player;
      this.boss = boss;
      this.playerTargetX = playerTargetX;
      this.bossEndPosition = Vector2.op_Addition(boss.Position, new Vector2(0.0f, -16f));
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      CS06_BossIntro cs06BossIntro1 = this;
      cs06BossIntro1.player.StateMachine.State = 11;
      cs06BossIntro1.player.StateMachine.Locked = true;
      while (!cs06BossIntro1.player.Dead && !cs06BossIntro1.player.OnGround(1))
        yield return (object) null;
      while (cs06BossIntro1.player.Dead)
        yield return (object) null;
      cs06BossIntro1.player.Facing = Facings.Right;
      CS06_BossIntro cs06BossIntro2 = cs06BossIntro1;
      double num1 = ((double) cs06BossIntro1.player.X + (double) cs06BossIntro1.boss.X) / 2.0 - 160.0;
      Rectangle bounds = level.Bounds;
      double num2 = (double) (((Rectangle) ref bounds).get_Bottom() - 180);
      Coroutine coroutine = new Coroutine(CutsceneEntity.CameraTo(new Vector2((float) num1, (float) num2), 1f, (Ease.Easer) null, 0.0f), true);
      cs06BossIntro2.Add((Component) coroutine);
      yield return (object) 0.5f;
      if (!cs06BossIntro1.player.Dead)
        yield return (object) cs06BossIntro1.player.DummyWalkToExact((int) ((double) cs06BossIntro1.playerTargetX - 8.0), false, 1f);
      cs06BossIntro1.player.Facing = Facings.Right;
      yield return (object) Textbox.Say("ch6_boss_start", new Func<IEnumerator>(cs06BossIntro1.BadelineFloat), new Func<IEnumerator>(cs06BossIntro1.PlayerStepForward));
      yield return (object) level.ZoomBack(0.5f);
      cs06BossIntro1.EndCutscene(level, true);
    }

    private IEnumerator BadelineFloat()
    {
      CS06_BossIntro cs06BossIntro = this;
      cs06BossIntro.Add((Component) new Coroutine(cs06BossIntro.Level.ZoomTo(new Vector2(170f, 110f), 2f, 1f), true));
      Audio.Play("event:/char/badeline/boss_prefight_getup", cs06BossIntro.boss.Position);
      cs06BossIntro.boss.Sitting = false;
      cs06BossIntro.boss.NormalSprite.Play("fallSlow", false, false);
      cs06BossIntro.boss.NormalSprite.Scale.X = (__Null) -1.0;
      cs06BossIntro.boss.Add((Component) (cs06BossIntro.animator = new BadelineAutoAnimator()));
      float fromY = cs06BossIntro.boss.Y;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
      {
        cs06BossIntro.boss.Position.Y = (__Null) (double) MathHelper.Lerp(fromY, (float) cs06BossIntro.bossEndPosition.Y, Ease.CubeInOut(p));
        yield return (object) null;
      }
    }

    private IEnumerator PlayerStepForward()
    {
      yield return (object) this.player.DummyWalkToExact((int) this.player.X + 8, false, 1f);
    }

    public override void OnEnd(Level level)
    {
      if (this.WasSkipped && this.player != null)
      {
        this.player.X = this.playerTargetX;
        while (!this.player.OnGround(1))
        {
          double y = (double) this.player.Y;
          Rectangle bounds = level.Bounds;
          double bottom = (double) ((Rectangle) ref bounds).get_Bottom();
          if (y < bottom)
            ++this.player.Y;
          else
            break;
        }
      }
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = 0;
      this.boss.Position = this.bossEndPosition;
      if (this.boss.NormalSprite != null)
      {
        this.boss.NormalSprite.Scale.X = (__Null) -1.0;
        this.boss.NormalSprite.Play("laugh", false, false);
      }
      this.boss.Sitting = false;
      if (this.animator != null)
        this.boss.Remove((Component) this.animator);
      level.Session.SetFlag("boss_intro", true);
    }
  }
}
