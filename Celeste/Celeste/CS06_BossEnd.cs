// Decompiled with JetBrains decompiler
// Type: Celeste.CS06_BossEnd
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS06_BossEnd : CutsceneEntity
  {
    public const string Flag = "badeline_connection";
    private Player player;
    private NPC06_Badeline_Crying badeline;
    private float fade;
    private float pictureFade;
    private float pictureGlow;
    private MTexture picture;
    private bool waitForKeyPress;
    private float timer;
    private EventInstance sfx;

    public CS06_BossEnd(Player player, NPC06_Badeline_Crying badeline)
      : base(true, false)
    {
      this.Tag = (int) Tags.HUD;
      this.player = player;
      this.badeline = badeline;
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      CS06_BossEnd cs06BossEnd = this;
      cs06BossEnd.player.StateMachine.State = 11;
      cs06BossEnd.player.StateMachine.Locked = true;
      while (!cs06BossEnd.player.OnGround(1))
        yield return (object) null;
      cs06BossEnd.player.Facing = Facings.Right;
      yield return (object) 1f;
      Level level1 = cs06BossEnd.SceneAs<Level>();
      level1.Session.Audio.Music.Event = "event:/music/lvl6/badeline_acoustic";
      level1.Session.Audio.Apply();
      yield return (object) Textbox.Say("ch6_boss_ending", new Func<IEnumerator>(cs06BossEnd.StartMusic), new Func<IEnumerator>(cs06BossEnd.PlayerHug), new Func<IEnumerator>(cs06BossEnd.BadelineCalmDown));
      yield return (object) 0.5f;
      while ((double) (cs06BossEnd.fade += Engine.DeltaTime) < 1.0)
        yield return (object) null;
      cs06BossEnd.picture = GFX.Portraits["hug1"];
      cs06BossEnd.sfx = Audio.Play("event:/game/06_reflection/hug_image_1");
      yield return (object) cs06BossEnd.PictureFade(1f, 1f);
      yield return (object) cs06BossEnd.WaitForPress();
      cs06BossEnd.sfx = Audio.Play("event:/game/06_reflection/hug_image_2");
      yield return (object) cs06BossEnd.PictureFade(0.0f, 0.5f);
      cs06BossEnd.picture = GFX.Portraits["hug2"];
      yield return (object) cs06BossEnd.PictureFade(1f, 1f);
      yield return (object) cs06BossEnd.WaitForPress();
      cs06BossEnd.sfx = Audio.Play("event:/game/06_reflection/hug_image_3");
      while ((double) (cs06BossEnd.pictureGlow += Engine.DeltaTime / 2f) < 1.0)
        yield return (object) null;
      yield return (object) 0.2f;
      yield return (object) cs06BossEnd.PictureFade(0.0f, 0.5f);
      while ((double) (cs06BossEnd.fade -= Engine.DeltaTime * 12f) > 0.0)
        yield return (object) null;
      level.Session.Audio.Music.Param("levelup", 1f);
      level.Session.Audio.Apply();
      cs06BossEnd.Add((Component) new Coroutine(cs06BossEnd.badeline.TurnWhite(1f), true));
      yield return (object) 0.5f;
      cs06BossEnd.player.Sprite.Play("idle", false, false);
      yield return (object) 0.25f;
      yield return (object) cs06BossEnd.player.DummyWalkToExact((int) cs06BossEnd.player.X - 8, true, 1f);
      cs06BossEnd.Add((Component) new Coroutine(cs06BossEnd.CenterCameraOnPlayer(), true));
      yield return (object) cs06BossEnd.badeline.Disperse();
      (cs06BossEnd.Scene as Level).Session.SetFlag("badeline_connection", true);
      level.Flash(Color.get_White(), false);
      level.Session.Inventory.Dashes = 2;
      cs06BossEnd.badeline.RemoveSelf();
      yield return (object) 0.1f;
      level.Add((Entity) new LevelUpEffect(cs06BossEnd.player.Position));
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      yield return (object) 2f;
      yield return (object) level.ZoomBack(0.5f);
      cs06BossEnd.EndCutscene(level, true);
    }

    private IEnumerator StartMusic()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      CS06_BossEnd cs06BossEnd = this;
      if (num != 0)
      {
        if (num != 1)
          return false;
        // ISSUE: reference to a compiler-generated field
        this.\u003C\u003E1__state = -1;
        return false;
      }
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      Level level = cs06BossEnd.SceneAs<Level>();
      level.Session.Audio.Music.Event = "event:/music/lvl6/badeline_acoustic";
      level.Session.Audio.Apply();
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) 0.5f;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }

    private IEnumerator PlayerHug()
    {
      CS06_BossEnd cs06BossEnd = this;
      cs06BossEnd.Add((Component) new Coroutine(cs06BossEnd.Level.ZoomTo(Vector2.op_Subtraction(Vector2.op_Addition(cs06BossEnd.badeline.Center, new Vector2(0.0f, -24f)), cs06BossEnd.Level.Camera.Position), 2f, 0.5f), true));
      yield return (object) 0.6f;
      yield return (object) cs06BossEnd.player.DummyWalkToExact((int) cs06BossEnd.badeline.X - 10, false, 1f);
      cs06BossEnd.player.Facing = Facings.Right;
      yield return (object) 0.25f;
      cs06BossEnd.player.DummyAutoAnimate = false;
      cs06BossEnd.player.Sprite.Play("hug", false, false);
      yield return (object) 0.5f;
    }

    private IEnumerator BadelineCalmDown()
    {
      CS06_BossEnd cs06BossEnd = this;
      Audio.SetParameter(Audio.CurrentAmbienceEventInstance, "postboss", 0.0f);
      cs06BossEnd.badeline.LoopingSfx.Param("end", 1f);
      yield return (object) 0.5f;
      cs06BossEnd.badeline.Sprite.Play("scaredTransition", false, false);
      Input.Rumble(RumbleStrength.Light, RumbleLength.Long);
      FinalBossStarfield bossBg = cs06BossEnd.Level.Background.Get<FinalBossStarfield>();
      if (bossBg != null)
      {
        while ((double) bossBg.Alpha > 0.0)
        {
          bossBg.Alpha -= Engine.DeltaTime;
          yield return (object) null;
        }
      }
      yield return (object) 1.5f;
    }

    private IEnumerator CenterCameraOnPlayer()
    {
      CS06_BossEnd cs06BossEnd = this;
      yield return (object) 0.5f;
      Vector2 from = cs06BossEnd.Level.ZoomFocusPoint;
      Rectangle bounds1 = cs06BossEnd.Level.Bounds;
      double num1 = (double) (((Rectangle) ref bounds1).get_Left() + 580);
      Rectangle bounds2 = cs06BossEnd.Level.Bounds;
      double num2 = (double) (((Rectangle) ref bounds2).get_Top() + 124);
      Vector2 to = Vector2.op_Subtraction(new Vector2((float) num1, (float) num2), cs06BossEnd.Level.Camera.Position);
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        cs06BossEnd.Level.ZoomFocusPoint = Vector2.op_Addition(from, Vector2.op_Multiply(Vector2.op_Subtraction(to, from), Ease.SineInOut(p)));
        yield return (object) null;
      }
    }

    private IEnumerator PictureFade(float to, float duration = 1f)
    {
      while ((double) (this.pictureFade = Calc.Approach(this.pictureFade, to, Engine.DeltaTime / duration)) != (double) to)
        yield return (object) null;
    }

    private IEnumerator WaitForPress()
    {
      this.waitForKeyPress = true;
      while (!Input.MenuConfirm.Pressed)
        yield return (object) null;
      this.waitForKeyPress = false;
    }

    public override void OnEnd(Level level)
    {
      if (this.WasSkipped && (HandleBase) this.sfx != (HandleBase) null)
        Audio.Stop(this.sfx, true);
      Audio.SetParameter(Audio.CurrentAmbienceEventInstance, "postboss", 0.0f);
      level.ResetZoom();
      level.Session.Inventory.Dashes = 2;
      level.Session.Audio.Music.Event = "event:/music/lvl6/badeline_acoustic";
      if (this.WasSkipped)
        level.Session.Audio.Music.Param("levelup", 2f);
      level.Session.Audio.Apply();
      if (this.WasSkipped)
        level.Add((Entity) new LevelUpEffect(this.player.Position));
      this.player.DummyAutoAnimate = true;
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = 0;
      FinalBossStarfield finalBossStarfield = this.Level.Background.Get<FinalBossStarfield>();
      if (finalBossStarfield != null)
        finalBossStarfield.Alpha = 0.0f;
      this.badeline.RemoveSelf();
      level.Session.SetFlag("badeline_connection", true);
    }

    public override void Update()
    {
      this.timer += Engine.DeltaTime;
      base.Update();
    }

    public override void Render()
    {
      if ((double) this.fade <= 0.0)
        return;
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.op_Multiply(Color.op_Multiply(Color.get_Black(), Ease.CubeOut(this.fade)), 0.8f));
      if (this.picture == null || (double) this.pictureFade <= 0.0)
        return;
      float num = Ease.CubeOut(this.pictureFade);
      Vector2 position;
      ((Vector2) ref position).\u002Ector(960f, 540f);
      float scale = (float) (1.0 + (1.0 - (double) num) * 0.025000000372529);
      this.picture.DrawCentered(position, Color.op_Multiply(Color.get_White(), Ease.CubeOut(this.pictureFade)), scale, 0.0f);
      if ((double) this.pictureGlow > 0.0)
      {
        GFX.Portraits["hug-light2a"].DrawCentered(position, Color.op_Multiply(Color.get_White(), Ease.CubeOut(this.pictureFade * this.pictureGlow)), scale);
        GFX.Portraits["hug-light2b"].DrawCentered(position, Color.op_Multiply(Color.get_White(), Ease.CubeOut(this.pictureFade * this.pictureGlow)), scale);
        GFX.Portraits["hug-light2c"].DrawCentered(position, Color.op_Multiply(Color.get_White(), Ease.CubeOut(this.pictureFade * this.pictureGlow)), scale);
        HiresRenderer.EndRender();
        HiresRenderer.BeginRender((BlendState) BlendState.Additive, (SamplerState) null);
        GFX.Portraits["hug-light2a"].DrawCentered(position, Color.op_Multiply(Color.get_White(), Ease.CubeOut(this.pictureFade * this.pictureGlow)), scale);
        GFX.Portraits["hug-light2b"].DrawCentered(position, Color.op_Multiply(Color.get_White(), Ease.CubeOut(this.pictureFade * this.pictureGlow)), scale);
        GFX.Portraits["hug-light2c"].DrawCentered(position, Color.op_Multiply(Color.get_White(), Ease.CubeOut(this.pictureFade * this.pictureGlow)), scale);
        HiresRenderer.EndRender();
        HiresRenderer.BeginRender((BlendState) null, (SamplerState) null);
      }
      if (!this.waitForKeyPress)
        return;
      GFX.Gui["textboxbutton"].DrawCentered(new Vector2(1520f, (float) (880 + ((double) this.timer % 1.0 < 0.25 ? 6 : 0))));
    }
  }
}
