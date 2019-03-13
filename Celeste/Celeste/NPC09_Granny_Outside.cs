// Decompiled with JetBrains decompiler
// Type: Celeste.NPC09_Granny_Outside
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class NPC09_Granny_Outside : NPC
  {
    public const string Flag = "granny_outside";
    public Hahaha Hahaha;
    public GrannyLaughSfx LaughSfx;
    private bool talking;
    private Player player;
    private bool leaving;

    public NPC09_Granny_Outside(EntityData data, Vector2 offset)
      : base(Vector2.op_Addition(data.Position, offset))
    {
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("granny")));
      this.Sprite.Play("idle", false, false);
      this.Add((Component) (this.LaughSfx = new GrannyLaughSfx(this.Sprite)));
      this.MoveAnim = "walk";
      this.IdleAnim = "idle";
      this.Maxspeed = 40f;
      this.SetupGrannySpriteSounds();
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if ((scene as Level).Session.GetFlag("granny_outside"))
        this.RemoveSelf();
      scene.Add((Entity) (this.Hahaha = new Hahaha(Vector2.op_Addition(this.Position, new Vector2(8f, -4f)), "", false, new Vector2?())));
      this.Hahaha.Enabled = false;
    }

    public override void Update()
    {
      if (!this.talking)
      {
        this.player = this.Level.Tracker.GetEntity<Player>();
        if (this.player != null && (double) this.player.X > (double) this.X - 48.0)
        {
          (this.Scene as Level).StartCutscene(new Action<Level>(this.EndTalking), true, false);
          this.Add((Component) new Coroutine(this.TalkRoutine(this.player), true));
          this.talking = true;
        }
      }
      this.Hahaha.Enabled = this.Sprite.CurrentAnimationID == "laugh";
      base.Update();
    }

    private IEnumerator TalkRoutine(Player player)
    {
      NPC09_Granny_Outside c09GrannyOutside = this;
      player.StateMachine.State = 11;
      while (!player.OnGround(1))
        yield return (object) null;
      c09GrannyOutside.Sprite.Scale.X = (__Null) -1.0;
      yield return (object) player.DummyWalkToExact((int) c09GrannyOutside.X - 16, false, 1f);
      yield return (object) 0.5f;
      yield return (object) c09GrannyOutside.Level.ZoomTo(new Vector2(200f, 110f), 2f, 0.5f);
      yield return (object) Textbox.Say("APP_OLDLADY_A", new Func<IEnumerator>(c09GrannyOutside.MoveRight), new Func<IEnumerator>(c09GrannyOutside.ExitRight));
      yield return (object) c09GrannyOutside.Level.ZoomBack(0.5f);
      c09GrannyOutside.Sprite.Scale.X = (__Null) 1.0;
      if (!c09GrannyOutside.leaving)
        yield return (object) c09GrannyOutside.ExitRight();
      while (true)
      {
        double x = (double) c09GrannyOutside.X;
        Rectangle bounds = c09GrannyOutside.Level.Bounds;
        double num = (double) (((Rectangle) ref bounds).get_Right() + 8);
        if (x < num)
          yield return (object) null;
        else
          break;
      }
      c09GrannyOutside.Level.EndCutscene();
      c09GrannyOutside.EndTalking(c09GrannyOutside.Level);
    }

    private IEnumerator MoveRight()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      NPC09_Granny_Outside c09GrannyOutside = this;
      if (num != 0)
      {
        if (num != 1)
          return false;
        // ISSUE: reference to a compiler-generated field
        this.\u003C\u003E1__state = -1;
        return false;
      }
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) c09GrannyOutside.MoveTo(new Vector2(c09GrannyOutside.X + 8f, c09GrannyOutside.Y), false, new int?(), false);
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }

    private IEnumerator ExitRight()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      NPC09_Granny_Outside c09GrannyOutside1 = this;
      if (num != 0)
      {
        if (num != 1)
          return false;
        // ISSUE: reference to a compiler-generated field
        this.\u003C\u003E1__state = -1;
        return false;
      }
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      c09GrannyOutside1.leaving = true;
      NPC09_Granny_Outside c09GrannyOutside2 = c09GrannyOutside1;
      NPC09_Granny_Outside c09GrannyOutside3 = c09GrannyOutside1;
      Rectangle bounds = c09GrannyOutside1.Level.Bounds;
      Vector2 target = new Vector2((float) (((Rectangle) ref bounds).get_Right() + 16), c09GrannyOutside1.Y);
      int? turnAtEndTo = new int?();
      Coroutine coroutine = new Coroutine(c09GrannyOutside3.MoveTo(target, false, turnAtEndTo, false), true);
      c09GrannyOutside2.Add((Component) coroutine);
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) null;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }

    private void EndTalking(Level level)
    {
      if (this.player != null)
        this.player.StateMachine.State = 0;
      this.Level.Session.SetFlag("granny_outside", true);
      this.RemoveSelf();
    }
  }
}
