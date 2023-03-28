// Decompiled with JetBrains decompiler
// Type: Celeste.CS10_FinalRoom
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste
{
  public class CS10_FinalRoom : CutsceneEntity
  {
    private Player player;
    private BadelineDummy badeline;
    private bool first;

    public CS10_FinalRoom(Player player, bool first)
      : base()
    {
      this.Depth = -8500;
      this.player = player;
      this.first = first;
    }

    public override void OnBegin(Level level) => this.Add((Component) new Coroutine(this.Cutscene(level)));

    private IEnumerator Cutscene(Level level)
    {
      CS10_FinalRoom cs10FinalRoom = this;
      cs10FinalRoom.player.StateMachine.State = 11;
      if (cs10FinalRoom.first)
      {
        yield return (object) cs10FinalRoom.player.DummyWalkToExact((int) ((double) cs10FinalRoom.player.X + 16.0));
        yield return (object) 0.5f;
      }
      else
      {
        cs10FinalRoom.player.DummyAutoAnimate = false;
        cs10FinalRoom.player.Sprite.Play("sitDown");
        cs10FinalRoom.player.Sprite.SetAnimationFrame(cs10FinalRoom.player.Sprite.CurrentAnimationTotalFrames - 1);
        yield return (object) 1.25f;
      }
      yield return (object) cs10FinalRoom.BadelineAppears();
      if (cs10FinalRoom.first)
        yield return (object) Textbox.Say("CH9_LAST_ROOM");
      else
        yield return (object) Textbox.Say("CH9_LAST_ROOM_ALT");
      yield return (object) cs10FinalRoom.BadelineVanishes();
      cs10FinalRoom.EndCutscene(level);
    }

    // private IEnumerator BadelineAppears()
    // {
    //   // ISSUE: reference to a compiler-generated field
    //   int num = this.\u003C\u003E1__state;
    //   CS10_FinalRoom cs10FinalRoom = this;
    //   if (num != 0)
    //   {
    //     if (num != 1)
    //       return false;
    //     // ISSUE: reference to a compiler-generated field
    //     this.\u003C\u003E1__state = -1;
    //     return false;
    //   }
    //   // ISSUE: reference to a compiler-generated field
    //   this.\u003C\u003E1__state = -1;
    //   cs10FinalRoom.Level.Add((Entity) (cs10FinalRoom.badeline = new BadelineDummy(cs10FinalRoom.player.Position + new Vector2(18f, -8f))));
    //   cs10FinalRoom.Level.Displacement.AddBurst(cs10FinalRoom.badeline.Center, 0.5f, 8f, 32f, 0.5f);
    //   Audio.Play("event:/char/badeline/maddy_split", cs10FinalRoom.badeline.Position);
    //   cs10FinalRoom.badeline.Sprite.Scale.X = -1f;
    //   // ISSUE: reference to a compiler-generated field
    //   this.\u003C\u003E2__current = (object) null;
    //   // ISSUE: reference to a compiler-generated field
    //   this.\u003C\u003E1__state = 1;
    //   return true;
    // }
    private IEnumerator BadelineAppears()
    {
      CS10_FinalRoom cs10FinalRoom = this;
      cs10FinalRoom.Level.Add((Entity) (cs10FinalRoom.badeline = new BadelineDummy(cs10FinalRoom.player.Position + new Vector2(18f, -8f))));
      cs10FinalRoom.Level.Displacement.AddBurst(cs10FinalRoom.badeline.Center, 0.5f, 8f, 32f, 0.5f);
      Audio.Play("event:/char/badeline/maddy_split", cs10FinalRoom.badeline.Position);
      cs10FinalRoom.badeline.Sprite.Scale.X = -1f;
      yield return (object) null;
    }

    private IEnumerator BadelineVanishes()
    {
      yield return (object) 0.2f;
      this.badeline.Vanish();
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      this.badeline = (BadelineDummy) null;
      yield return (object) 0.5f;
      this.player.Facing = Facings.Right;
    }

    public override void OnEnd(Level level)
    {
      this.Level.Session.Inventory.Dashes = 1;
      this.player.StateMachine.State = 0;
      if (!this.first && !this.WasSkipped)
        Audio.Play("event:/char/madeline/stand", this.player.Position);
      if (this.badeline == null)
        return;
      this.badeline.RemoveSelf();
    }
  }
}
