// Decompiled with JetBrains decompiler
// Type: Celeste.CS02_DreamingPhonecall
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS02_DreamingPhonecall : CutsceneEntity
  {
    private BadelineDummy evil;
    private Player player;
    private Payphone payphone;
    private SoundSource ringtone;

    public CS02_DreamingPhonecall(Player player)
      : base(false, false)
    {
      this.player = player;
    }

    public override void OnBegin(Level level)
    {
      this.payphone = this.Scene.Tracker.GetEntity<Payphone>();
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
      this.Add((Component) (this.ringtone = new SoundSource()));
      this.ringtone.Position = this.payphone.Position;
    }

    private IEnumerator Cutscene(Level level)
    {
      this.player.StateMachine.State = Player.StDummy;
      this.player.Dashes = 1;
      yield return (object) 0.3f;
      this.ringtone.Play("event:/game/02_old_site/sequence_phone_ring_loop", (string) null, 0.0f);
      while ((double) this.player.Light.Alpha > 0.0)
      {
        this.player.Light.Alpha -= Engine.DeltaTime * 2f;
        yield return (object) null;
      }
      yield return (object) 3.2f;
      yield return (object) this.player.DummyWalkTo(this.payphone.X - 24f, false, 1f, false);
      yield return (object) 1.5f;
      this.player.Facing = Facings.Left;
      yield return (object) 1.5f;
      this.player.Facing = Facings.Right;
      yield return (object) 0.25f;
      yield return (object) this.player.DummyWalkTo(this.payphone.X - 4f, false, 1f, false);
      yield return (object) 1.5f;
      this.Add((Component) Alarm.Create(Alarm.AlarmMode.Oneshot, (Action) (() => this.ringtone.Param("end", 1f)), 0.43f, true));
      this.player.Visible = false;
      Audio.Play("event:/game/02_old_site/sequence_phone_pickup", this.player.Position);
      yield return (object) this.payphone.Sprite.PlayRoutine("pickUp", false);
      yield return (object) 1f;
      if (level.Session.Area.Mode == AreaMode.Normal)
        Audio.SetMusic("event:/music/lvl2/phone_loop", true, true);
      this.payphone.Sprite.Play("talkPhone", false, false);
      yield return (object) Textbox.Say("CH2_DREAM_PHONECALL", new Func<IEnumerator>(this.ShowShadowMadeline));
      if (this.evil != null)
      {
        if (level.Session.Area.Mode == AreaMode.Normal)
          Audio.SetMusic("event:/music/lvl2/phone_end", true, true);
        this.evil.Vanish();
        this.evil = (BadelineDummy) null;
        yield return (object) 1f;
      }
      this.Add((Component) new Coroutine(this.WireFalls(), true));
      this.payphone.Broken = true;
      level.Shake(0.2f);
      VertexLight light = new VertexLight(new Vector2(16f, -28f), Color.White, 0.0f, 32, 48);
      this.payphone.Add((Component) light);
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, (Ease.Easer) null, 2f, true);
      tween.OnUpdate = (Action<Tween>) (t => light.Alpha = t.Eased);
      this.Add((Component) tween);
      tween = (Tween) null;
      Audio.Play("event:/game/02_old_site/sequence_phone_transform", this.payphone.Position);
      yield return (object) this.payphone.Sprite.PlayRoutine("transform", false);
      yield return (object) 0.4f;
      yield return (object) this.payphone.Sprite.PlayRoutine("eat", false);
      this.payphone.Sprite.Play("monsterIdle", false, false);
      yield return (object) 1.2f;
      level.EndCutscene();
      FadeWipe fadeWipe = new FadeWipe((Scene) level, false, (Action) (() => this.EndCutscene(level, true)));
    }

    private IEnumerator ShowShadowMadeline()
    {
      Payphone payphone = this.Scene.Tracker.GetEntity<Payphone>();
      Level level = this.Scene as Level;
      yield return (object) level.ZoomTo(new Vector2(240f, 116f), 2f, 0.5f);
      this.evil = new BadelineDummy(payphone.Position + new Vector2(32f, -24f));
      this.evil.Appear(level, false);
      this.Scene.Add((Entity) this.evil);
      yield return (object) 0.2f;
      ++payphone.Blink.X;
      yield return (object) payphone.Sprite.PlayRoutine("jumpBack", false);
      yield return (object) payphone.Sprite.PlayRoutine("scare", false);
      yield return (object) 1.2f;
    }

    private IEnumerator WireFalls()
    {
      yield return (object) 0.5f;
      Wire wire = this.Scene.Entities.FindFirst<Wire>();
      Vector2 speed = Vector2.Zero;
      Level level = this.SceneAs<Level>();
      while (wire != null && (double) wire.Curve.Begin.X < (double) level.Bounds.Right)
      {
        speed += new Vector2(0.7f, 1f) * 200f * Engine.DeltaTime;
        wire.Curve.Begin += speed * Engine.DeltaTime;
        yield return (object) null;
      }
    }

    public override void OnEnd(Level level)
    {
      level.ResetZoom();
      level.Bloom.Base = 0.0f;
      level.Remove((Entity) this.player);
      level.UnloadLevel();
      level.Session.Dreaming = false;
      level.Session.Level = "end_0";
      Session session = level.Session;
      Level level1 = level;
      Rectangle bounds = level.Bounds;
      double left = (double) bounds.Left;
      bounds = level.Bounds;
      double bottom = (double) bounds.Bottom;
      Vector2 from = new Vector2((float) left, (float) bottom);
      Vector2? nullable = new Vector2?(level1.GetSpawnPoint(from));
      session.RespawnPoint = nullable;
      level.Session.Audio.Music.Event = "event:/music/lvl2/awake";
      level.Session.Audio.Ambience.Event = "event:/env/amb/02_awake";
      level.LoadLevel(Player.IntroTypes.WakeUp, false);
      level.EndCutscene();
    }
  }
}

