// Decompiled with JetBrains decompiler
// Type: Celeste.NPC07X_Granny_Ending
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class NPC07X_Granny_Ending : NPC
  {
    private int conversation = 0;
    public Hahaha Hahaha;
    public GrannyLaughSfx LaughSfx;
    private Player player;
    private TalkComponent talker;
    private Coroutine talkRoutine;

    public NPC07X_Granny_Ending(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("granny")));
      this.Sprite.Play("idle", false, false);
      this.Sprite.Scale.X = -1f;
      this.Add((Component) (this.LaughSfx = new GrannyLaughSfx(this.Sprite)));
      this.Add((Component) (this.talker = new TalkComponent(new Rectangle(-20, -8, 40, 8), new Vector2(0.0f, -24f), new Action<Player>(this.OnTalk), (TalkComponent.HoverDisplay) null)));
      this.MoveAnim = "walk";
      this.Maxspeed = 40f;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      scene.Add((Entity) (this.Hahaha = new Hahaha(this.Position + new Vector2(8f, -4f), "", false, new Vector2?())));
      this.Hahaha.Enabled = false;
    }

    public override void Update()
    {
      this.Hahaha.Enabled = this.Sprite.CurrentAnimationID == "laugh";
      base.Update();
    }

    private void OnTalk(Player player)
    {
      this.player = player;
      (this.Scene as Level).StartCutscene(new Action<Level>(this.EndTalking), true, false);
      this.Add((Component) (this.talkRoutine = new Coroutine(this.TalkRoutine(player), true)));
    }

    private IEnumerator TalkRoutine(Player player)
    {
      player.StateMachine.State = 11;
      player.ForceCameraUpdate = true;
      while (!player.OnGround(1))
        yield return (object) null;
      yield return (object) player.DummyWalkToExact((int) this.X - 16, false, 1f);
      player.Facing = Facings.Right;
      if (this.conversation == 0)
      {
        yield return (object) 0.5f;
        yield return (object) this.Level.ZoomTo(this.Position - this.Level.Camera.Position + new Vector2(0.0f, -32f), 2f, 0.5f);
        yield return (object) Textbox.Say("CH7_CSIDE_OLDLADY", new Func<IEnumerator>(this.StartLaughing), new Func<IEnumerator>(this.StopLaughing));
      }
      else if (this.conversation == 1)
      {
        yield return (object) 0.5f;
        yield return (object) this.Level.ZoomTo(this.Position - this.Level.Camera.Position + new Vector2(0.0f, -32f), 2f, 0.5f);
        yield return (object) Textbox.Say("CH7_CSIDE_OLDLADY_B", new Func<IEnumerator>(this.StartLaughing), new Func<IEnumerator>(this.StopLaughing));
        this.talker.Enabled = false;
      }
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
        this.player.StateMachine.State = 0;
        this.player.ForceCameraUpdate = false;
      }
      ++this.conversation;
      if (this.talkRoutine != null)
      {
        this.talkRoutine.RemoveSelf();
        this.talkRoutine = (Coroutine) null;
      }
      this.Sprite.Play("idle", false, false);
    }
  }
}

