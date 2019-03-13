// Decompiled with JetBrains decompiler
// Type: Celeste.NPC09_Granny_Inside
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class NPC09_Granny_Inside : NPC
  {
    public const string DoorConversationAvailable = "granny_door";
    private const string DoorConversationDone = "granny_door_done";
    private const string CounterFlag = "granny";
    private int conversation;
    private const int MaxConversation = 4;
    public Hahaha Hahaha;
    public GrannyLaughSfx LaughSfx;
    private Player player;
    private TalkComponent talker;
    private bool talking;
    private Coroutine talkRoutine;

    private bool HasDoorConversation
    {
      get
      {
        if (this.Level.Session.GetFlag("granny_door"))
          return !this.Level.Session.GetFlag("granny_door_done");
        return false;
      }
    }

    private bool talkerEnabled
    {
      get
      {
        if (this.conversation <= 0 || this.conversation >= 4)
          return this.HasDoorConversation;
        return true;
      }
    }

    public NPC09_Granny_Inside(EntityData data, Vector2 offset)
      : base(Vector2.op_Addition(data.Position, offset))
    {
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("granny")));
      this.Sprite.Play("idle", false, false);
      this.Add((Component) (this.LaughSfx = new GrannyLaughSfx(this.Sprite)));
      this.MoveAnim = "walk";
      this.Maxspeed = 40f;
      this.Add((Component) (this.talker = new TalkComponent(new Rectangle(-20, -8, 40, 8), new Vector2(0.0f, -24f), new Action<Player>(this.OnTalk), (TalkComponent.HoverDisplay) null)));
      this.talker.Enabled = false;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.conversation = this.Level.Session.GetCounter("granny");
      scene.Add((Entity) (this.Hahaha = new Hahaha(Vector2.op_Addition(this.Position, new Vector2(8f, -4f)), "", false, new Vector2?())));
      this.Hahaha.Enabled = false;
    }

    public override void Update()
    {
      if (!this.talking && this.conversation == 0)
      {
        this.player = this.Level.Tracker.GetEntity<Player>();
        if (this.player != null && (double) Math.Abs(this.player.X - this.X) < 48.0)
          this.OnTalk(this.player);
      }
      this.talker.Enabled = this.talkerEnabled;
      this.Hahaha.Enabled = this.Sprite.CurrentAnimationID == "laugh";
      base.Update();
    }

    private void OnTalk(Player player)
    {
      this.player = player;
      (this.Scene as Level).StartCutscene(new Action<Level>(this.EndTalking), true, false);
      this.Add((Component) (this.talkRoutine = new Coroutine(this.TalkRoutine(player), true)));
      this.talking = true;
    }

    private IEnumerator TalkRoutine(Player player)
    {
      NPC09_Granny_Inside npC09GrannyInside = this;
      player.StateMachine.State = 11;
      player.Dashes = 1;
      player.ForceCameraUpdate = true;
      while (!player.OnGround(1))
        yield return (object) null;
      yield return (object) player.DummyWalkToExact((int) npC09GrannyInside.X - 16, false, 1f);
      player.Facing = Facings.Right;
      player.ForceCameraUpdate = false;
      Vector2 zoomPoint = new Vector2(npC09GrannyInside.X - 8f - npC09GrannyInside.Level.Camera.X, 110f);
      if (npC09GrannyInside.HasDoorConversation)
      {
        npC09GrannyInside.Sprite.Scale.X = (__Null) -1.0;
        yield return (object) npC09GrannyInside.Level.ZoomTo(zoomPoint, 2f, 0.5f);
        yield return (object) Textbox.Say("APP_OLDLADY_LOCKED");
      }
      else if (npC09GrannyInside.conversation == 0)
      {
        yield return (object) 0.5f;
        npC09GrannyInside.Sprite.Scale.X = (__Null) -1.0;
        yield return (object) 0.25f;
        yield return (object) npC09GrannyInside.Level.ZoomTo(zoomPoint, 2f, 0.5f);
        yield return (object) Textbox.Say("APP_OLDLADY_B", new Func<IEnumerator>(npC09GrannyInside.StartLaughing), new Func<IEnumerator>(npC09GrannyInside.StopLaughing));
      }
      else if (npC09GrannyInside.conversation == 1)
      {
        npC09GrannyInside.Sprite.Scale.X = (__Null) -1.0;
        yield return (object) npC09GrannyInside.Level.ZoomTo(zoomPoint, 2f, 0.5f);
        yield return (object) Textbox.Say("APP_OLDLADY_C", new Func<IEnumerator>(npC09GrannyInside.StartLaughing), new Func<IEnumerator>(npC09GrannyInside.StopLaughing));
      }
      else if (npC09GrannyInside.conversation == 2)
      {
        npC09GrannyInside.Sprite.Scale.X = (__Null) -1.0;
        yield return (object) npC09GrannyInside.Level.ZoomTo(zoomPoint, 2f, 0.5f);
        yield return (object) Textbox.Say("APP_OLDLADY_D", new Func<IEnumerator>(npC09GrannyInside.StartLaughing), new Func<IEnumerator>(npC09GrannyInside.StopLaughing));
      }
      else if (npC09GrannyInside.conversation == 3)
      {
        npC09GrannyInside.Sprite.Scale.X = (__Null) -1.0;
        yield return (object) npC09GrannyInside.Level.ZoomTo(zoomPoint, 2f, 0.5f);
        yield return (object) Textbox.Say("APP_OLDLADY_E", new Func<IEnumerator>(npC09GrannyInside.StartLaughing), new Func<IEnumerator>(npC09GrannyInside.StopLaughing));
      }
      npC09GrannyInside.talker.Enabled = npC09GrannyInside.talkerEnabled;
      yield return (object) npC09GrannyInside.Level.ZoomBack(0.5f);
      npC09GrannyInside.Level.EndCutscene();
      npC09GrannyInside.EndTalking(npC09GrannyInside.Level);
    }

    private IEnumerator StartLaughing()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      NPC09_Granny_Inside npC09GrannyInside = this;
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
      npC09GrannyInside.Sprite.Play("laugh", false, false);
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) null;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }

    private IEnumerator StopLaughing()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      NPC09_Granny_Inside npC09GrannyInside = this;
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
      npC09GrannyInside.Sprite.Play("idle", false, false);
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) null;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }

    private void EndTalking(Level level)
    {
      if (this.player != null)
      {
        this.player.StateMachine.State = 0;
        this.player.ForceCameraUpdate = false;
      }
      if (this.HasDoorConversation)
      {
        this.Level.Session.SetFlag("granny_door_done", true);
      }
      else
      {
        this.Level.Session.IncrementCounter("granny");
        ++this.conversation;
      }
      if (this.talkRoutine != null)
      {
        this.talkRoutine.RemoveSelf();
        this.talkRoutine = (Coroutine) null;
      }
      this.Sprite.Play("idle", false, false);
      this.talking = false;
    }
  }
}
