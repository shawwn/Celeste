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
    private float fade = 0.0f;
    public const string Flag = "badeline_connection";
    private Player player;
    private NPC06_Badeline_Crying badeline;
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
      this.player.StateMachine.State = Player.StDummy;
      this.player.StateMachine.Locked = true;
      while (!this.player.OnGround(1))
        yield return (object) null;
      this.player.Facing = Facings.Right;
      yield return (object) 1f;
      Level lvl = this.SceneAs<Level>();
      lvl.Session.Audio.Music.Event = "event:/music/lvl6/badeline_acoustic";
      lvl.Session.Audio.Apply();
      lvl = (Level) null;
      yield return (object) Textbox.Say("ch6_boss_ending", new Func<IEnumerator>(this.StartMusic), new Func<IEnumerator>(this.PlayerHug), new Func<IEnumerator>(this.BadelineCalmDown));
      yield return (object) 0.5f;
      while ((double) (this.fade += Engine.DeltaTime) < 1.0)
        yield return (object) null;
      this.picture = GFX.Portraits["hug1"];
      this.sfx = Audio.Play("event:/game/06_reflection/hug_image_1");
      yield return (object) this.PictureFade(1f, 1f);
      yield return (object) this.WaitForPress();
      this.sfx = Audio.Play("event:/game/06_reflection/hug_image_2");
      yield return (object) this.PictureFade(0.0f, 0.5f);
      this.picture = GFX.Portraits["hug2"];
      yield return (object) this.PictureFade(1f, 1f);
      yield return (object) this.WaitForPress();
      this.sfx = Audio.Play("event:/game/06_reflection/hug_image_3");
      while ((double) (this.pictureGlow += Engine.DeltaTime / 2f) < 1.0)
        yield return (object) null;
      yield return (object) 0.2f;
      yield return (object) this.PictureFade(0.0f, 0.5f);
      while ((double) (this.fade -= Engine.DeltaTime * 12f) > 0.0)
        yield return (object) null;
      level.Session.Audio.Music.Param("levelup", 1f);
      level.Session.Audio.Apply();
      this.Add((Component) new Coroutine(this.badeline.TurnWhite(1f), true));
      yield return (object) 0.5f;
      this.player.Sprite.Play("idle", false, false);
      yield return (object) 0.25f;
      yield return (object) this.player.DummyWalkToExact((int) this.player.X - 8, true, 1f);
      this.Add((Component) new Coroutine(this.CenterCameraOnPlayer(), true));
      yield return (object) this.badeline.Disperse();
      (this.Scene as Level).Session.SetFlag("badeline_connection", true);
      level.Flash(Color.White, false);
      level.Session.Inventory.Dashes = 2;
      this.badeline.RemoveSelf();
      yield return (object) 0.1f;
      level.Add((Entity) new LevelUpEffect(this.player.Position));
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      yield return (object) 2f;
      yield return (object) level.ZoomBack(0.5f);
      this.EndCutscene(level, true);
    }

    private IEnumerator StartMusic()
    {
      Level level = this.SceneAs<Level>();
      level.Session.Audio.Music.Event = "event:/music/lvl6/badeline_acoustic";
      level.Session.Audio.Apply();
      yield return (object) 0.5f;
    }

    private IEnumerator PlayerHug()
    {
      this.Add((Component) new Coroutine(this.Level.ZoomTo(this.badeline.Center + new Vector2(0.0f, -24f) - this.Level.Camera.Position, 2f, 0.5f), true));
      yield return (object) 0.6f;
      yield return (object) this.player.DummyWalkToExact((int) this.badeline.X - 10, false, 1f);
      this.player.Facing = Facings.Right;
      yield return (object) 0.25f;
      this.player.DummyAutoAnimate = false;
      this.player.Sprite.Play("hug", false, false);
      yield return (object) 0.5f;
    }

    private IEnumerator BadelineCalmDown()
    {
      Audio.SetParameter(Audio.CurrentAmbienceEventInstance, "postboss", 0.0f);
      this.badeline.LoopingSfx.Param("end", 1f);
      yield return (object) 0.5f;
      this.badeline.Sprite.Play("scaredTransition", false, false);
      Input.Rumble(RumbleStrength.Light, RumbleLength.Long);
      FinalBossStarfield bossBg = this.Level.Background.Get<FinalBossStarfield>();
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
      yield return (object) 0.5f;
      Vector2 from = this.Level.ZoomFocusPoint;
      Vector2 to = new Vector2((float) (this.Level.Bounds.Left + 580), (float) (this.Level.Bounds.Top + 124)) - this.Level.Camera.Position;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime)
      {
        this.Level.ZoomFocusPoint = from + (to - from) * Ease.SineInOut(p);
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
      this.player.StateMachine.State = Player.StNormal;
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
      Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * Ease.CubeOut(this.fade) * 0.8f);
      if (this.picture != null && (double) this.pictureFade > 0.0)
      {
        float num = Ease.CubeOut(this.pictureFade);
        Vector2 position = new Vector2(960f, 540f);
        float scale = (float) (1.0 + (1.0 - (double) num) * 0.025000000372529);
        this.picture.DrawCentered(position, Color.White * Ease.CubeOut(this.pictureFade), scale, 0.0f);
        if ((double) this.pictureGlow > 0.0)
        {
          GFX.Portraits["hug-light2a"].DrawCentered(position, Color.White * Ease.CubeOut(this.pictureFade * this.pictureGlow), scale);
          GFX.Portraits["hug-light2b"].DrawCentered(position, Color.White * Ease.CubeOut(this.pictureFade * this.pictureGlow), scale);
          GFX.Portraits["hug-light2c"].DrawCentered(position, Color.White * Ease.CubeOut(this.pictureFade * this.pictureGlow), scale);
          HiresRenderer.EndRender();
          HiresRenderer.BeginRender(BlendState.Additive, (SamplerState) null);
          GFX.Portraits["hug-light2a"].DrawCentered(position, Color.White * Ease.CubeOut(this.pictureFade * this.pictureGlow), scale);
          GFX.Portraits["hug-light2b"].DrawCentered(position, Color.White * Ease.CubeOut(this.pictureFade * this.pictureGlow), scale);
          GFX.Portraits["hug-light2c"].DrawCentered(position, Color.White * Ease.CubeOut(this.pictureFade * this.pictureGlow), scale);
          HiresRenderer.EndRender();
          HiresRenderer.BeginRender((BlendState) null, (SamplerState) null);
        }
        if (this.waitForKeyPress)
          GFX.Gui["textboxbutton"].DrawCentered(new Vector2(1520f, (float) (880 + ((double) this.timer % 1.0 < 0.25 ? 6 : 0))));
      }
    }
  }
}

