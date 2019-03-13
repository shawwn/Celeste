// Decompiled with JetBrains decompiler
// Type: Celeste.CS03_Guestbook
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;
using System.Collections;

namespace Celeste
{
  public class CS03_Guestbook : CutsceneEntity
  {
    private Player player;

    public CS03_Guestbook(Player player)
      : base(true, false)
    {
      this.player = player;
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Routine(), true));
    }

    private IEnumerator Routine()
    {
      CS03_Guestbook cs03Guestbook = this;
      cs03Guestbook.player.StateMachine.State = Player.StDummy;
      cs03Guestbook.player.StateMachine.Locked = true;
      yield return (object) Textbox.Say("ch3_guestbook");
      yield return (object) 0.1f;
      cs03Guestbook.EndCutscene(cs03Guestbook.Level, true);
    }

    public override void OnEnd(Level level)
    {
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = Player.StNormal;
    }
  }
}
