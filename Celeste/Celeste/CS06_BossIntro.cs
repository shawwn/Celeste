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
      this.bossEndPosition = boss.Position + new Vector2(0.0f, -16f);
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      this.player.StateMachine.State = Player.StDummy;
      this.player.StateMachine.Locked = true;
      while (!this.player.Dead && !this.player.OnGround(1))
        yield return (object) null;
      while (this.player.Dead)
        yield return (object) null;
      this.player.Facing = Facings.Right;
      this.Add((Component) new Coroutine(CutsceneEntity.CameraTo(new Vector2((float) (((double) this.player.X + (double) this.boss.X) / 2.0 - 160.0), (float) (level.Bounds.Bottom - 180)), 1f, (Ease.Easer) null, 0.0f), true));
      yield return (object) 0.5f;
      if (!this.player.Dead)
        yield return (object) this.player.DummyWalkToExact((int) ((double) this.playerTargetX - 8.0), false, 1f);
      this.player.Facing = Facings.Right;
      yield return (object) Textbox.Say("ch6_boss_start", new Func<IEnumerator>(this.BadelineFloat), new Func<IEnumerator>(this.PlayerStepForward));
      yield return (object) level.ZoomBack(0.5f);
      this.EndCutscene(level, true);
    }

    private IEnumerator BadelineFloat()
    {
      this.Add((Component) new Coroutine(this.Level.ZoomTo(new Vector2(170f, 110f), 2f, 1f), true));
      Audio.Play("event:/char/badeline/boss_prefight_getup", this.boss.Position);
      this.boss.Sitting = false;
      this.boss.NormalSprite.Play("fallSlow", false, false);
      this.boss.NormalSprite.Scale.X = -1f;
      this.boss.Add((Component) (this.animator = new BadelineAutoAnimator()));
      float fromY = this.boss.Y;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime * 4f)
      {
        this.boss.Position.Y = MathHelper.Lerp(fromY, this.bossEndPosition.Y, Ease.CubeInOut(p));
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
        Player player;
        for (; !this.player.OnGround(1) && (double) this.player.Y < (double) level.Bounds.Bottom; ++player.Y)
          player = this.player;
      }
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = Player.StNormal;
      this.boss.Position = this.bossEndPosition;
      if (this.boss.NormalSprite != null)
      {
        this.boss.NormalSprite.Scale.X = -1f;
        this.boss.NormalSprite.Play("laugh", false, false);
      }
      this.boss.Sitting = false;
      if (this.animator != null)
        this.boss.Remove((Component) this.animator);
      level.Session.SetFlag("boss_intro", true);
    }
  }
}

