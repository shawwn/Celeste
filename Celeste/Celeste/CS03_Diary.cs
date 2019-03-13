// Decompiled with JetBrains decompiler
// Type: Celeste.CS03_Diary
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;
using System.Collections;

namespace Celeste
{
  public class CS03_Diary : CutsceneEntity
  {
    private Player player;

    public CS03_Diary(Player player)
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
      CS03_Diary cs03Diary = this;
      cs03Diary.player.StateMachine.State = Player.StDummy;
      cs03Diary.player.StateMachine.Locked = true;
      yield return (object) Textbox.Say("CH3_DIARY");
      yield return (object) 0.1f;
      cs03Diary.EndCutscene(cs03Diary.Level, true);
    }

    public override void OnEnd(Level level)
    {
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = Player.StNormal;
    }
  }
}
