// Decompiled with JetBrains decompiler
// Type: Celeste.CS05_TheoInMirror
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste
{
  public class CS05_TheoInMirror : CutsceneEntity
  {
    public const string Flag = "theoInMirror";
    private NPC theo;
    private Player player;
    private int playerFinalX;

    public CS05_TheoInMirror(NPC theo, Player player)
      : base(true, false)
    {
      this.theo = theo;
      this.player = player;
      this.playerFinalX = (int) theo.Position.X + 24;
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator Cutscene(Level level)
    {
      CS05_TheoInMirror cs05TheoInMirror = this;
      cs05TheoInMirror.player.StateMachine.State = 11;
      cs05TheoInMirror.player.StateMachine.Locked = true;
      yield return (object) cs05TheoInMirror.player.DummyWalkTo(cs05TheoInMirror.theo.X - 16f, false, 1f, false);
      yield return (object) 0.5f;
      cs05TheoInMirror.theo.Sprite.Scale.X = (__Null) -1.0;
      yield return (object) 0.25f;
      yield return (object) Textbox.Say("ch5_theo_mirror");
      cs05TheoInMirror.Add((Component) new Coroutine(cs05TheoInMirror.theo.MoveTo(Vector2.op_Addition(cs05TheoInMirror.theo.Position, new Vector2(64f, 0.0f)), false, new int?(), false), true));
      yield return (object) 0.4f;
      yield return (object) cs05TheoInMirror.player.DummyWalkToExact(cs05TheoInMirror.playerFinalX, false, 1f);
      cs05TheoInMirror.EndCutscene(level, true);
    }

    public override void OnEnd(Level level)
    {
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = 0;
      this.player.X = (float) this.playerFinalX;
      this.player.MoveV(200f, (Collision) null, (Solid) null);
      this.player.Speed = Vector2.get_Zero();
      this.Scene.Remove((Entity) this.theo);
      level.Session.SetFlag("theoInMirror", true);
    }
  }
}
