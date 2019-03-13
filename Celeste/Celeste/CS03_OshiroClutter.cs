// Decompiled with JetBrains decompiler
// Type: Celeste.CS03_OshiroClutter
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class CS03_OshiroClutter : CutsceneEntity
  {
    private int index;
    private Player player;
    private NPC03_Oshiro_Cluttter oshiro;
    private List<ClutterDoor> doors;

    public CS03_OshiroClutter(Player player, NPC03_Oshiro_Cluttter oshiro, int index)
      : base(true, false)
    {
      this.player = player;
      this.oshiro = oshiro;
      this.index = index;
    }

    public override void OnBegin(Level level)
    {
      this.doors = this.Scene.Entities.FindAll<ClutterDoor>();
      this.doors.Sort((Comparison<ClutterDoor>) ((a, b) => (int) ((double) a.Y - (double) b.Y)));
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      this.player.StateMachine.State = Player.StDummy;
      this.player.StateMachine.Locked = true;
      int side = this.index != 1 && this.index != 2 ? 1 : -1;
      if (side == -1)
      {
        yield return (object) this.player.DummyWalkToExact((int) this.oshiro.X - 24, false, 1f);
        this.player.Facing = Facings.Right;
        this.oshiro.Sprite.Scale.X = -1f;
      }
      else
      {
        this.Add((Component) new Coroutine(this.oshiro.PaceRight(), true));
        yield return (object) this.player.DummyWalkToExact((int) this.oshiro.HomePosition.X + 24, false, 1f);
        this.player.Facing = Facings.Left;
        this.oshiro.Sprite.Scale.X = 1f;
      }
      if (this.index < 3)
      {
        yield return (object) this.Level.ZoomTo(this.oshiro.ZoomPoint, 2f, 0.5f);
        yield return (object) Textbox.Say("CH3_OSHIRO_CLUTTER" + (object) this.index, new Func<IEnumerator>(this.Collapse), new Func<IEnumerator>(this.oshiro.PaceLeft), new Func<IEnumerator>(this.oshiro.PaceRight));
        yield return (object) this.Level.ZoomBack(0.5f);
        level.Session.SetFlag("oshiro_clutter_door_open", true);
        if (this.index == 0)
          this.SetMusic();
        foreach (ClutterDoor door1 in this.doors)
        {
          ClutterDoor door = door1;
          if (!door.IsLocked(level.Session))
            yield return (object) door.UnlockRoutine();
          door = (ClutterDoor) null;
        }
      }
      else
      {
        yield return (object) CutsceneEntity.CameraTo(new Vector2((float) this.Level.Bounds.X, (float) this.Level.Bounds.Y), 0.5f, (Ease.Easer) null, 0.0f);
        yield return (object) this.Level.ZoomTo(new Vector2(90f, 60f), 2f, 0.5f);
        yield return (object) Textbox.Say("CH3_OSHIRO_CLUTTER_ENDING");
        yield return (object) this.oshiro.MoveTo(new Vector2(this.oshiro.X, (float) (level.Bounds.Top - 32)), false, new int?(), false);
        this.oshiro.Add((Component) new SoundSource("event:/char/oshiro/move_05_09b_exit"));
        yield return (object) this.Level.ZoomBack(0.5f);
      }
      this.EndCutscene(level, true);
    }

    private IEnumerator Collapse()
    {
      Audio.Play("event:/char/oshiro/chat_collapse", this.oshiro.Position);
      this.oshiro.Sprite.Play("fall", false, false);
      yield return (object) 0.5f;
    }

    private void SetMusic()
    {
      Level scene = this.Scene as Level;
      scene.Session.Audio.Music.Event = "event:/music/lvl3/clean";
      scene.Session.Audio.Music.Progress = 1;
      scene.Session.Audio.Apply();
    }

    public override void OnEnd(Level level)
    {
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = Player.StNormal;
      if (this.oshiro.Sprite.CurrentAnimationID == "side")
        (this.oshiro.Sprite as OshiroSprite).Pop("idle", true);
      if (this.index < 3)
      {
        level.Session.SetFlag("oshiro_clutter_door_open", true);
        level.Session.SetFlag("oshiro_clutter_" + (object) this.index, true);
        if (this.index == 0 && this.WasSkipped)
          this.SetMusic();
        foreach (ClutterDoor door in this.doors)
        {
          if (!door.IsLocked(level.Session))
            door.InstantUnlock();
        }
        if (!this.WasSkipped || this.index != 0)
          return;
        this.oshiro.Sprite.Play("idle_ground", false, false);
      }
      else
      {
        level.Session.SetFlag("oshiro_clutter_finished", true);
        this.Scene.Remove((Entity) this.oshiro);
      }
    }
  }
}

