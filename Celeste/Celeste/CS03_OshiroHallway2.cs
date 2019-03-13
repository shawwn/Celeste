// Decompiled with JetBrains decompiler
// Type: Celeste.CS03_OshiroHallway2
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste
{
  public class CS03_OshiroHallway2 : CutsceneEntity
  {
    public const string Flag = "oshiro_resort_talked_3";
    private Player player;
    private NPC oshiro;

    public CS03_OshiroHallway2(Player player, NPC oshiro)
      : base(true, false)
    {
      this.player = player;
      this.oshiro = oshiro;
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      level.Session.Audio.Music.Layer(1, false);
      level.Session.Audio.Music.Layer(2, true);
      level.Session.Audio.Apply();
      this.player.StateMachine.State = Player.StDummy;
      this.player.StateMachine.Locked = true;
      yield return (object) Textbox.Say("CH3_OSHIRO_HALLWAY_B");
      this.oshiro.MoveToAndRemove(new Vector2((float) (level.Bounds.Right + 64), this.oshiro.Y));
      this.oshiro.Add((Component) new SoundSource("event:/char/oshiro/move_03_08a_exit"));
      yield return (object) 1f;
      this.EndCutscene(level, true);
    }

    public override void OnEnd(Level level)
    {
      level.Session.Audio.Music.Layer(1, true);
      level.Session.Audio.Music.Layer(2, false);
      level.Session.Audio.Apply();
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = Player.StNormal;
      level.Session.SetFlag("oshiro_resort_talked_3", true);
      if (!this.WasSkipped)
        return;
      level.Remove((Entity) this.oshiro);
    }
  }
}

