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
    private int conversation = 0;
    public const string DoorConversationAvailable = "granny_door";
    private const string DoorConversationDone = "granny_door_done";
    private const string CounterFlag = "granny";
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
        return this.Level.Session.GetFlag("granny_door") && !this.Level.Session.GetFlag("granny_door_done");
      }
    }

    private bool talkerEnabled
    {
      get
      {
        return this.conversation > 0 && this.conversation < 4 || this.HasDoorConversation;
      }
    }

    public NPC09_Granny_Inside(EntityData data, Vector2 offset)
      : base(data.Position + offset)
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
      scene.Add((Entity) (this.Hahaha = new Hahaha(this.Position + new Vector2(8f, -4f), "", false, new Vector2?())));
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
      player.StateMachine.State = Player.StDummy;
      player.Dashes = 1;
      player.ForceCameraUpdate = true;
      while (!player.OnGround(1))
        yield return (object) null;
      yield return (object) player.DummyWalkToExact((int) this.X - 16, false, 1f);
      player.Facing = Facings.Right;
      player.ForceCameraUpdate = false;
      Vector2 zoomPoint = new Vector2(this.X - 8f - this.Level.Camera.X, 110f);
      if (this.HasDoorConversation)
      {
        this.Sprite.Scale.X = -1f;
        yield return (object) this.Level.ZoomTo(zoomPoint, 2f, 0.5f);
        yield return (object) Textbox.Say("APP_OLDLADY_LOCKED");
      }
      else if (this.conversation == 0)
      {
        yield return (object) 0.5f;
        this.Sprite.Scale.X = -1f;
        yield return (object) 0.25f;
        yield return (object) this.Level.ZoomTo(zoomPoint, 2f, 0.5f);
        yield return (object) Textbox.Say("APP_OLDLADY_B", new Func<IEnumerator>(this.StartLaughing), new Func<IEnumerator>(this.StopLaughing));
      }
      else if (this.conversation == 1)
      {
        this.Sprite.Scale.X = -1f;
        yield return (object) this.Level.ZoomTo(zoomPoint, 2f, 0.5f);
        yield return (object) Textbox.Say("APP_OLDLADY_C", new Func<IEnumerator>(this.StartLaughing), new Func<IEnumerator>(this.StopLaughing));
      }
      else if (this.conversation == 2)
      {
        this.Sprite.Scale.X = -1f;
        yield return (object) this.Level.ZoomTo(zoomPoint, 2f, 0.5f);
        yield return (object) Textbox.Say("APP_OLDLADY_D", new Func<IEnumerator>(this.StartLaughing), new Func<IEnumerator>(this.StopLaughing));
      }
      else if (this.conversation == 3)
      {
        this.Sprite.Scale.X = -1f;
        yield return (object) this.Level.ZoomTo(zoomPoint, 2f, 0.5f);
        yield return (object) Textbox.Say("APP_OLDLADY_E", new Func<IEnumerator>(this.StartLaughing), new Func<IEnumerator>(this.StopLaughing));
      }
      this.talker.Enabled = this.talkerEnabled;
      yield return (object) this.Level.ZoomBack(0.5f);
      this.Level.EndCutscene();
      this.EndTalking(this.Level);
    }

    private IEnumerator StartLaughing()
    {
      this.Sprite.Play("laugh", false, false);
      yield return (object) null;
    }

    private IEnumerator StopLaughing()
    {
      this.Sprite.Play("idle", false, false);
      yield return (object) null;
    }

    private void EndTalking(Level level)
    {
      if (this.player != null)
      {
        this.player.StateMachine.State = Player.StNormal;
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

