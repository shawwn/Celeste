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
      CS02_DreamingPhonecall dreamingPhonecall = this;
      dreamingPhonecall.player.StateMachine.State = 11;
      dreamingPhonecall.player.Dashes = 1;
      yield return (object) 0.3f;
      dreamingPhonecall.ringtone.Play("event:/game/02_old_site/sequence_phone_ring_loop", (string) null, 0.0f);
      while ((double) dreamingPhonecall.player.Light.Alpha > 0.0)
      {
        dreamingPhonecall.player.Light.Alpha -= Engine.DeltaTime * 2f;
        yield return (object) null;
      }
      yield return (object) 3.2f;
      yield return (object) dreamingPhonecall.player.DummyWalkTo(dreamingPhonecall.payphone.X - 24f, false, 1f, false);
      yield return (object) 1.5f;
      dreamingPhonecall.player.Facing = Facings.Left;
      yield return (object) 1.5f;
      dreamingPhonecall.player.Facing = Facings.Right;
      yield return (object) 0.25f;
      yield return (object) dreamingPhonecall.player.DummyWalkTo(dreamingPhonecall.payphone.X - 4f, false, 1f, false);
      yield return (object) 1.5f;
      dreamingPhonecall.Add((Component) Alarm.Create(Alarm.AlarmMode.Oneshot, (Action) (() => this.ringtone.Param("end", 1f)), 0.43f, true));
      dreamingPhonecall.player.Visible = false;
      Audio.Play("event:/game/02_old_site/sequence_phone_pickup", dreamingPhonecall.player.Position);
      yield return (object) dreamingPhonecall.payphone.Sprite.PlayRoutine("pickUp", false);
      yield return (object) 1f;
      if (level.Session.Area.Mode == AreaMode.Normal)
        Audio.SetMusic("event:/music/lvl2/phone_loop", true, true);
      dreamingPhonecall.payphone.Sprite.Play("talkPhone", false, false);
      yield return (object) Textbox.Say("CH2_DREAM_PHONECALL", new Func<IEnumerator>(dreamingPhonecall.ShowShadowMadeline));
      if (dreamingPhonecall.evil != null)
      {
        if (level.Session.Area.Mode == AreaMode.Normal)
          Audio.SetMusic("event:/music/lvl2/phone_end", true, true);
        dreamingPhonecall.evil.Vanish();
        dreamingPhonecall.evil = (BadelineDummy) null;
        yield return (object) 1f;
      }
      dreamingPhonecall.Add((Component) new Coroutine(dreamingPhonecall.WireFalls(), true));
      dreamingPhonecall.payphone.Broken = true;
      level.Shake(0.2f);
      VertexLight light = new VertexLight(new Vector2(16f, -28f), Color.get_White(), 0.0f, 32, 48);
      dreamingPhonecall.payphone.Add((Component) light);
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, (Ease.Easer) null, 2f, true);
      tween.OnUpdate = (Action<Tween>) (t => light.Alpha = t.Eased);
      dreamingPhonecall.Add((Component) tween);
      Audio.Play("event:/game/02_old_site/sequence_phone_transform", dreamingPhonecall.payphone.Position);
      yield return (object) dreamingPhonecall.payphone.Sprite.PlayRoutine("transform", false);
      yield return (object) 0.4f;
      yield return (object) dreamingPhonecall.payphone.Sprite.PlayRoutine("eat", false);
      dreamingPhonecall.payphone.Sprite.Play("monsterIdle", false, false);
      yield return (object) 1.2f;
      level.EndCutscene();
      FadeWipe fadeWipe = new FadeWipe((Scene) level, false, (Action) (() => this.EndCutscene(level, true)));
    }

    private IEnumerator ShowShadowMadeline()
    {
      CS02_DreamingPhonecall dreamingPhonecall = this;
      Payphone payphone = dreamingPhonecall.Scene.Tracker.GetEntity<Payphone>();
      Level level = dreamingPhonecall.Scene as Level;
      yield return (object) level.ZoomTo(new Vector2(240f, 116f), 2f, 0.5f);
      dreamingPhonecall.evil = new BadelineDummy(Vector2.op_Addition(payphone.Position, new Vector2(32f, -24f)));
      dreamingPhonecall.evil.Appear(level, false);
      dreamingPhonecall.Scene.Add((Entity) dreamingPhonecall.evil);
      yield return (object) 0.2f;
      ++payphone.Blink.X;
      yield return (object) payphone.Sprite.PlayRoutine("jumpBack", false);
      yield return (object) payphone.Sprite.PlayRoutine("scare", false);
      yield return (object) 1.2f;
    }

    private IEnumerator WireFalls()
    {
      CS02_DreamingPhonecall dreamingPhonecall = this;
      yield return (object) 0.5f;
      Wire wire = dreamingPhonecall.Scene.Entities.FindFirst<Wire>();
      Vector2 speed = Vector2.get_Zero();
      Level level = dreamingPhonecall.SceneAs<Level>();
      while (wire != null)
      {
        // ISSUE: variable of the null type
        __Null x = wire.Curve.Begin.X;
        Rectangle bounds = level.Bounds;
        double right = (double) ((Rectangle) ref bounds).get_Right();
        if (x < right)
        {
          speed = Vector2.op_Addition(speed, Vector2.op_Multiply(Vector2.op_Multiply(new Vector2(0.7f, 1f), 200f), Engine.DeltaTime));
          ref Vector2 local = ref wire.Curve.Begin;
          local = Vector2.op_Addition(local, Vector2.op_Multiply(speed, Engine.DeltaTime));
          yield return (object) null;
        }
        else
          break;
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
      double left = (double) ((Rectangle) ref bounds).get_Left();
      bounds = level.Bounds;
      double bottom = (double) ((Rectangle) ref bounds).get_Bottom();
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
