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
    private float anxiety = 0.0f;
    private float anxietyFlicker = 0.0f;
    private Sprite bossSprite = GFX.SpriteBank.Create("oshiro_boss");
    public const string Flag = "oshiro_resort_roof";
    private const float playerEndPosition = 170f;
    private Player player;
    private NPC oshiro;
    private BadelineDummy evil;
    private Vector2 bossSpawnPosition;
    private float bossSpriteOffset;
    private bool oshiroRumble;

    public CS03_OshiroRooftop(NPC oshiro)
      : base(true, false)
    {
      this.oshiro = oshiro;
    }

    public override void OnBegin(Level level)
    {
      this.bossSpawnPosition = new Vector2(this.oshiro.X, (float) (level.Bounds.Bottom - 40));
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      while (this.player == null)
      {
        this.player = this.Scene.Tracker.GetEntity<Player>();
        if (this.player == null)
          yield return (object) null;
        else
          break;
      }
      this.player.StateMachine.State = Player.StDummy;
      this.player.StateMachine.Locked = true;
      while (!this.player.OnGround(1) || (double) this.player.Speed.Y < 0.0)
        yield return (object) null;
      yield return (object) 0.6f;
      this.evil = new BadelineDummy(new Vector2(this.oshiro.X - 40f, (float) (level.Bounds.Bottom - 60)));
      this.evil.Sprite.Scale.X = 1f;
      this.evil.Appear(level, false);
      level.Add((Entity) this.evil);
      yield return (object) 0.1f;
      this.player.Facing = Facings.Left;
      yield return (object) Textbox.Say("CH3_OSHIRO_START_CHASE", new Func<IEnumerator>(this.MaddyWalkAway), new Func<IEnumerator>(this.MaddyTurnAround), new Func<IEnumerator>(this.EnterOshiro), new Func<IEnumerator>(this.OshiroGetsAngry));
      yield return (object) this.OshiroTransform();
      this.Add((Component) new Coroutine(this.AnxietyAndCameraOut(), true));
      yield return (object) level.ZoomBack(0.5f);
      yield return (object) 0.25f;
      this.EndCutscene(level, true);
    }

    private IEnumerator MaddyWalkAway()
    {
      Level level = this.Scene as Level;
      this.Add((Component) new Coroutine(this.player.DummyWalkTo((float) level.Bounds.Left + 170f, false, 1f, false), true));
      yield return (object) 0.2f;
      Audio.Play("event:/game/03_resort/suite_bad_moveroof", this.evil.Position);
      this.Add((Component) new Coroutine(this.evil.FloatTo(this.evil.Position + new Vector2(80f, 30f), new int?(), true, false), true));
      yield return (object) null;
    }

    private IEnumerator MaddyTurnAround()
    {
      yield return (object) 0.25f;
      this.player.Facing = Facings.Left;
      yield return (object) 0.1f;
      Level level = this.SceneAs<Level>();
      yield return (object) level.ZoomTo(new Vector2(150f, (float) ((double) this.bossSpawnPosition.Y - (double) level.Bounds.Y - 8.0)), 2f, 0.5f);
    }

    private IEnumerator EnterOshiro()
    {
      yield return (object) 0.3f;
      this.bossSpriteOffset = (this.bossSprite.Justify.Value.Y - this.oshiro.Sprite.Justify.Value.Y) * this.bossSprite.Height;
      this.oshiro.Visible = true;
      this.oshiro.Sprite.Scale.X = 1f;
      this.Add((Component) new Coroutine(this.oshiro.MoveTo(this.bossSpawnPosition - new Vector2(0.0f, this.bossSpriteOffset), false, new int?(), false), true));
      this.oshiro.Add((Component) new SoundSource("event:/char/oshiro/move_07_roof00_enter"));
      float from = this.Level.ZoomFocusPoint.X;
      for (float p = 0.0f; (double) p < 1.0; p += Engine.DeltaTime / 0.7f)
      {
        this.Level.ZoomFocusPoint.X = from + (126f - from) * Ease.CubeInOut(p);
        yield return (object) null;
      }
      yield return (object) 0.3f;
      this.player.Facing = Facings.Left;
      yield return (object) 0.1f;
      this.evil.Sprite.Scale.X = -1f;
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
      yield return (object) 0.2f;
      Audio.Play("event:/char/oshiro/boss_transform_burst", this.oshiro.Position);
      this.oshiro.Sprite.Play("transformFinish", false, false);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
      this.SceneAs<Level>().Shake(0.5f);
      this.SetChaseMusic();
      while ((double) this.anxiety < 0.5)
      {
        this.anxiety = Calc.Approach(this.anxiety, 0.5f, Engine.DeltaTime * 0.5f);
        yield return (object) null;
      }
      yield return (object) 0.25f;
    }

    private IEnumerator AnxietyAndCameraOut()
    {
      Level level = this.Scene as Level;
      Vector2 from = level.Camera.Position;
      Vector2 to = this.player.CameraTarget;
      for (float t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime * 2f)
      {
        this.anxiety = Calc.Approach(this.anxiety, 0.0f, Engine.DeltaTime * 4f);
        level.Camera.Position = from + (to - from) * Ease.CubeInOut(t);
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
        this.player.StateMachine.State = Player.StNormal;
        this.player.X = (float) level.Bounds.Left + 170f;
        this.player.Speed.Y = 0.0f;
        while (this.player.CollideCheck<Solid>())
          --this.player.Y;
        level.Camera.Position = this.player.CameraTarget;
      }
      if (this.WasSkipped)
        this.SetChaseMusic();
      this.oshiro.RemoveSelf();
      this.Scene.Add((Entity) new AngryOshiro(this.bossSpawnPosition, true));
      level.Session.RespawnPoint = new Vector2?(new Vector2((float) level.Bounds.Left + 170f, (float) (level.Bounds.Top + 160)));
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

