// Decompiled with JetBrains decompiler
// Type: Celeste.CS03_OshiroLobby
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS03_OshiroLobby : CutsceneEntity
  {
    private SoundSource sfx = new SoundSource();
    public const string Flag = "oshiro_resort_talked_1";
    private Player player;
    private NPC oshiro;
    private float startLightAlpha;
    private bool createSparks;

    public CS03_OshiroLobby(Player player, NPC oshiro)
      : base(true, false)
    {
      this.player = player;
      this.oshiro = oshiro;
      this.Add((Component) this.sfx);
    }

    public override void Update()
    {
      base.Update();
      if (!this.createSparks || !this.Level.OnInterval(0.025f))
        return;
      Vector2 position = Vector2.op_Addition(Vector2.op_Addition(this.oshiro.Position, new Vector2(0.0f, -12f)), new Vector2((float) (Calc.Random.Range(4, 12) * Calc.Random.Choose<int>(1, -1)), (float) (Calc.Random.Range(4, 12) * Calc.Random.Choose<int>(1, -1))));
      this.Level.Particles.Emit(NPC03_Oshiro_Lobby.P_AppearSpark, position, Vector2.op_Subtraction(position, this.oshiro.Position).Angle());
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      CS03_OshiroLobby cs03OshiroLobby = this;
      cs03OshiroLobby.startLightAlpha = level.Lighting.Alpha;
      float endLightAlpha = 1f;
      float from = cs03OshiroLobby.oshiro.Y;
      cs03OshiroLobby.player.StateMachine.State = 11;
      cs03OshiroLobby.player.StateMachine.Locked = true;
      yield return (object) 0.5f;
      yield return (object) cs03OshiroLobby.player.DummyWalkTo(cs03OshiroLobby.oshiro.X - 16f, false, 1f, false);
      cs03OshiroLobby.player.Facing = Facings.Right;
      cs03OshiroLobby.sfx.Play("event:/game/03_resort/sequence_oshiro_intro", (string) null, 0.0f);
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      yield return (object) 1.4f;
      level.Shake(0.3f);
      level.Lighting.Alpha += 0.5f;
      while ((double) level.Lighting.Alpha > (double) cs03OshiroLobby.startLightAlpha)
      {
        level.Lighting.Alpha -= Engine.DeltaTime * 4f;
        yield return (object) null;
      }
      VertexLight light = new VertexLight(new Vector2(0.0f, -8f), Color.get_White(), 1f, 32, 64);
      BloomPoint bloom = new BloomPoint(new Vector2(0.0f, -8f), 1f, 16f);
      level.Lighting.SetSpotlight(light);
      cs03OshiroLobby.oshiro.Add((Component) light);
      cs03OshiroLobby.oshiro.Add((Component) bloom);
      cs03OshiroLobby.oshiro.Y -= 16f;
      Vector2 target = light.Position;
      Tween tween1 = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.5f, true);
      tween1.OnUpdate = (Action<Tween>) (t =>
      {
        light.Alpha = bloom.Alpha = t.Percent;
        light.Position = Vector2.Lerp(Vector2.op_Subtraction(target, Vector2.op_Multiply(Vector2.get_UnitY(), 48f)), target, t.Percent);
        level.Lighting.Alpha = MathHelper.Lerp(this.startLightAlpha, endLightAlpha, t.Eased);
      });
      cs03OshiroLobby.Add((Component) tween1);
      yield return (object) tween1.Wait();
      yield return (object) 0.2f;
      yield return (object) level.ZoomTo(new Vector2(170f, 126f), 2f, 0.5f);
      yield return (object) 0.6f;
      level.Shake(0.3f);
      cs03OshiroLobby.oshiro.Sprite.Visible = true;
      cs03OshiroLobby.oshiro.Sprite.Play("appear", false, false);
      yield return (object) cs03OshiroLobby.player.DummyWalkToExact((int) ((double) cs03OshiroLobby.player.X - 12.0), true, 1f);
      cs03OshiroLobby.player.DummyAutoAnimate = false;
      cs03OshiroLobby.player.Sprite.Play("shaking", false, false);
      Input.Rumble(RumbleStrength.Medium, RumbleLength.FullSecond);
      yield return (object) 0.6f;
      cs03OshiroLobby.createSparks = true;
      yield return (object) 0.4f;
      cs03OshiroLobby.createSparks = false;
      yield return (object) 0.2f;
      level.Shake(0.3f);
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      yield return (object) 1.4f;
      level.Lighting.UnsetSpotlight();
      Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, 0.5f, true);
      tween.OnUpdate = (Action<Tween>) (t =>
      {
        level.Lighting.Alpha = MathHelper.Lerp(endLightAlpha, this.startLightAlpha, t.Percent);
        bloom.Alpha = 1f - t.Percent;
      });
      cs03OshiroLobby.Add((Component) tween);
      while ((double) cs03OshiroLobby.oshiro.Y != (double) from)
      {
        cs03OshiroLobby.oshiro.Y = Calc.Approach(cs03OshiroLobby.oshiro.Y, from, Engine.DeltaTime * 40f);
        yield return (object) null;
      }
      yield return (object) tween.Wait();
      tween = (Tween) null;
      Audio.SetMusic("event:/music/lvl3/oshiro_theme", true, true);
      cs03OshiroLobby.player.DummyAutoAnimate = true;
      yield return (object) Textbox.Say("CH3_OSHIRO_FRONT_DESK", new Func<IEnumerator>(cs03OshiroLobby.ZoomOut));
      foreach (MrOshiroDoor mrOshiroDoor in cs03OshiroLobby.Scene.Entities.FindAll<MrOshiroDoor>())
        mrOshiroDoor.Open();
      NPC oshiro = cs03OshiroLobby.oshiro;
      Rectangle bounds = level.Bounds;
      Vector2 target1 = new Vector2((float) (((Rectangle) ref bounds).get_Right() + 64), cs03OshiroLobby.oshiro.Y);
      oshiro.MoveToAndRemove(target1);
      cs03OshiroLobby.oshiro.Add((Component) new SoundSource("event:/char/oshiro/move_01_0xa_exit"));
      yield return (object) 1.5f;
      cs03OshiroLobby.EndCutscene(level, true);
    }

    private IEnumerator ZoomOut()
    {
      CS03_OshiroLobby cs03OshiroLobby = this;
      yield return (object) 0.2f;
      yield return (object) cs03OshiroLobby.Level.ZoomBack(0.5f);
      yield return (object) 0.2f;
    }

    public override void OnEnd(Level level)
    {
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = 0;
      if (this.WasSkipped)
      {
        foreach (MrOshiroDoor mrOshiroDoor in this.Scene.Entities.FindAll<MrOshiroDoor>())
          mrOshiroDoor.InstantOpen();
      }
      level.Lighting.Alpha = this.startLightAlpha;
      level.Lighting.UnsetSpotlight();
      level.Session.SetFlag("oshiro_resort_talked_1", true);
      level.Session.Audio.Music.Event = "event:/music/lvl3/explore";
      level.Session.Audio.Music.Progress = 1;
      level.Session.Audio.Apply();
      if (!this.WasSkipped)
        return;
      level.Remove((Entity) this.oshiro);
    }
  }
}
