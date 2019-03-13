// Decompiled with JetBrains decompiler
// Type: Celeste.CS03_OshiroRooftop
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS03_OshiroRooftop : CutsceneEntity
  {
    private Sprite bossSprite = GFX.SpriteBank.Create("oshiro_boss");
    public const string Flag = "oshiro_resort_roof";
    private const float playerEndPosition = 170f;
    private Player player;
    private NPC oshiro;
    private BadelineDummy evil;
    private Vector2 bossSpawnPosition;
    private float anxiety;
    private float anxietyFlicker;
    private float bossSpriteOffset;
    private bool oshiroRumble;

    public CS03_OshiroRooftop(NPC oshiro)
      : base(true, false)
    {
      this.oshiro = oshiro;
    }

    public override void OnBegin(Level level)
    {
      double x = (double) this.oshiro.X;
      Rectangle bounds = level.Bounds;
      double num = (double) (((Rectangle) ref bounds).get_Bottom() - 40);
      this.bossSpawnPosition = new Vector2((float) x, (float) num);
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      CS03_OshiroRooftop cs03OshiroRooftop1 = this;
      while (cs03OshiroRooftop1.player == null)
      {
        cs03OshiroRooftop1.player = cs03OshiroRooftop1.Scene.Tracker.GetEntity<Player>();
        if (cs03OshiroRooftop1.player == null)
          yield return (object) null;
        else
          break;
      }
      cs03OshiroRooftop1.player.StateMachine.State = 11;
      cs03OshiroRooftop1.player.StateMachine.Locked = true;
      while (!cs03OshiroRooftop1.player.OnGround(1) || cs03OshiroRooftop1.player.Speed.Y < 0.0)
        yield return (object) null;
      yield return (object) 0.6f;
      CS03_OshiroRooftop cs03OshiroRooftop2 = cs03OshiroRooftop1;
      double num1 = (double) cs03OshiroRooftop1.oshiro.X - 40.0;
      Rectangle bounds = level.Bounds;
      double num2 = (double) (((Rectangle) ref bounds).get_Bottom() - 60);
      BadelineDummy badelineDummy = new BadelineDummy(new Vector2((float) num1, (float) num2));
      cs03OshiroRooftop2.evil = badelineDummy;
      cs03OshiroRooftop1.evil.Sprite.Scale.X = (__Null) 1.0;
      cs03OshiroRooftop1.evil.Appear(level, false);
      level.Add((Entity) cs03OshiroRooftop1.evil);
      yield return (object) 0.1f;
      cs03OshiroRooftop1.player.Facing = Facings.Left;
      yield return (object) Textbox.Say("CH3_OSHIRO_START_CHASE", new Func<IEnumerator>(cs03OshiroRooftop1.MaddyWalkAway), new Func<IEnumerator>(cs03OshiroRooftop1.MaddyTurnAround), new Func<IEnumerator>(cs03OshiroRooftop1.EnterOshiro), new Func<IEnumerator>(cs03OshiroRooftop1.OshiroGetsAngry));
      yield return (object) cs03OshiroRooftop1.OshiroTransform();
      cs03OshiroRooftop1.Add((Component) new Coroutine(cs03OshiroRooftop1.AnxietyAndCameraOut(), true));
      yield return (object) level.ZoomBack(0.5f);
      yield return (object) 0.25f;
      cs03OshiroRooftop1.EndCutscene(level, true);
    }

    private IEnumerator MaddyWalkAway()
    {
      CS03_OshiroRooftop cs03OshiroRooftop1 = this;
      Level scene = cs03OshiroRooftop1.Scene as Level;
      CS03_OshiroRooftop cs03OshiroRooftop2 = cs03OshiroRooftop1;
      Player player = cs03OshiroRooftop1.player;
      Rectangle bounds = scene.Bounds;
      double num = (double) ((Rectangle) ref bounds).get_Left() + 170.0;
      Coroutine coroutine = new Coroutine(player.DummyWalkTo((float) num, false, 1f, false), true);
      cs03OshiroRooftop2.Add((Component) coroutine);
      yield return (object) 0.2f;
      Audio.Play("event:/game/03_resort/suite_bad_moveroof", cs03OshiroRooftop1.evil.Position);
      cs03OshiroRooftop1.Add((Component) new Coroutine(cs03OshiroRooftop1.evil.FloatTo(Vector2.op_Addition(cs03OshiroRooftop1.evil.Position, new Vector2(80f, 30f)), new int?(), true, false), true));
      yield return (object) null;
    }

    private IEnumerator MaddyTurnAround()
    {
      CS03_OshiroRooftop cs03OshiroRooftop = this;
      yield return (object) 0.25f;
      cs03OshiroRooftop.player.Facing = Facings.Left;
      yield return (object) 0.1f;
      Level level = cs03OshiroRooftop.SceneAs<Level>();
      yield return (object) level.ZoomTo(new Vector2(150f, (float) (cs03OshiroRooftop.bossSpawnPosition.Y - (double) (float) level.Bounds.Y - 8.0)), 2f, 0.5f);
    }

    private IEnumerator EnterOshiro()
    {
      CS03_OshiroRooftop cs03OshiroRooftop = this;
      yield return (object) 0.3f;
      cs03OshiroRooftop.bossSpriteOffset = (float) (cs03OshiroRooftop.bossSprite.Justify.Value.Y - cs03OshiroRooftop.oshiro.Sprite.Justify.Value.Y) * cs03OshiroRooftop.bossSprite.Height;
      cs03OshiroRooftop.oshiro.Visible = true;
      cs03OshiroRooftop.oshiro.Sprite.Scale.X = (__Null) 1.0;
      cs03OshiroRooftop.Add((Component) new Coroutine(cs03OshiroRooftop.oshiro.MoveTo(Vector2.op_Subtraction(cs03OshiroRooftop.bossSpawnPosition, new Vector2(0.0f, cs03OshiroRooftop.bossSpriteOffset)), false, new int?(), false), true));
      cs03OshiroRooftop.oshiro.Add((Component) new SoundSource("event:/char/oshiro/move_07_roof00_enter"));
      float from = (float) cs03OshiroRooftop.Level.ZoomFocusPoint.X;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.7f)
      {
        cs03OshiroRooftop.Level.ZoomFocusPoint.X = (__Null) ((double) from + (126.0 - (double) from) * (double) Ease.CubeInOut(p));
        yield return (object) null;
      }
      yield return (object) 0.3f;
      cs03OshiroRooftop.player.Facing = Facings.Left;
      yield return (object) 0.1f;
      cs03OshiroRooftop.evil.Sprite.Scale.X = (__Null) -1.0;
    }

    private IEnumerator OshiroGetsAngry()
    {
      yield return (object) 0.1f;
      this.evil.Vanish();
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      this.evil = (BadelineDummy) null;
      yield return (object) 0.8f;
      Audio.Play("event:/char/oshiro/boss_transform_begin", this.oshiro.Position);
      this.oshiro.Remove((Component) this.oshiro.Sprite);
      this.oshiro.Sprite = this.bossSprite;
      this.oshiro.Sprite.Play("transformStart", false, false);
      this.oshiro.Y += this.bossSpriteOffset;
      this.oshiro.Add((Component) this.oshiro.Sprite);
      this.oshiro.Depth = -12500;
      this.oshiroRumble = true;
      yield return (object) 1f;
    }

    private IEnumerator OshiroTransform()
    {
      CS03_OshiroRooftop cs03OshiroRooftop = this;
      yield return (object) 0.2f;
      Audio.Play("event:/char/oshiro/boss_transform_burst", cs03OshiroRooftop.oshiro.Position);
      cs03OshiroRooftop.oshiro.Sprite.Play("transformFinish", false, false);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      cs03OshiroRooftop.SceneAs<Level>().Shake(0.5f);
      cs03OshiroRooftop.SetChaseMusic();
      while ((double) cs03OshiroRooftop.anxiety < 0.5)
      {
        cs03OshiroRooftop.anxiety = Calc.Approach(cs03OshiroRooftop.anxiety, 0.5f, Engine.DeltaTime * 0.5f);
        yield return (object) null;
      }
      yield return (object) 0.25f;
    }

    private IEnumerator AnxietyAndCameraOut()
    {
      CS03_OshiroRooftop cs03OshiroRooftop = this;
      Level level = cs03OshiroRooftop.Scene as Level;
      Vector2 from = level.Camera.Position;
      Vector2 to = cs03OshiroRooftop.player.CameraTarget;
      for (float t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime * 2f)
      {
        cs03OshiroRooftop.anxiety = Calc.Approach(cs03OshiroRooftop.anxiety, 0.0f, Engine.DeltaTime * 4f);
        level.Camera.Position = Vector2.op_Addition(from, Vector2.op_Multiply(Vector2.op_Subtraction(to, from), Ease.CubeInOut(t)));
        yield return (object) null;
      }
    }

    private void SetChaseMusic()
    {
      Level scene = this.Scene as Level;
      scene.Session.Audio.Music.Event = "event:/music/lvl3/oshiro_chase";
      scene.Session.Audio.Apply();
    }

    public override void OnEnd(Level level)
    {
      Distort.Anxiety = this.anxiety = this.anxietyFlicker = 0.0f;
      if (this.evil != null)
        level.Remove((Entity) this.evil);
      this.player = this.Scene.Tracker.GetEntity<Player>();
      if (this.player != null)
      {
        this.player.StateMachine.Locked = false;
        this.player.StateMachine.State = 0;
        Player player = this.player;
        Rectangle bounds = level.Bounds;
        double num = (double) ((Rectangle) ref bounds).get_Left() + 170.0;
        player.X = (float) num;
        this.player.Speed.Y = (__Null) 0.0;
        while (this.player.CollideCheck<Solid>())
          --this.player.Y;
        level.Camera.Position = this.player.CameraTarget;
      }
      if (this.WasSkipped)
        this.SetChaseMusic();
      this.oshiro.RemoveSelf();
      this.Scene.Add((Entity) new AngryOshiro(this.bossSpawnPosition, true));
      Session session = level.Session;
      Rectangle bounds1 = level.Bounds;
      double num1 = (double) ((Rectangle) ref bounds1).get_Left() + 170.0;
      Rectangle bounds2 = level.Bounds;
      double num2 = (double) (((Rectangle) ref bounds2).get_Top() + 160);
      Vector2? nullable = new Vector2?(new Vector2((float) num1, (float) num2));
      session.RespawnPoint = nullable;
      level.Session.SetFlag("oshiro_resort_roof", true);
    }

    public override void Update()
    {
      Distort.Anxiety = this.anxiety + this.anxiety * this.anxietyFlicker;
      if (this.Scene.OnInterval(0.05f))
        this.anxietyFlicker = Calc.Random.NextFloat(0.4f) - 0.2f;
      base.Update();
      if (!this.oshiroRumble)
        return;
      Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
    }
  }
}
