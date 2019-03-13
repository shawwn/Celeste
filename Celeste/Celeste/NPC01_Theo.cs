// Decompiled with JetBrains decompiler
// Type: Celeste.NPC01_Theo
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class NPC01_Theo : NPC
  {
    private int currentConversation = 0;
    public static ParticleType P_YOLO;
    private const string DoneTalking = "theoDoneTalking";
    private Coroutine talkRoutine;

    public NPC01_Theo(Vector2 position)
      : base(position)
    {
      this.Add((Component) (this.Sprite = GFX.SpriteBank.Create("theo")));
      this.Sprite.Play("idle", false, false);
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      this.currentConversation = this.Session.GetCounter("theo");
      if (this.Session.GetFlag("theoDoneTalking"))
        return;
      this.Add((Component) (this.Talker = new TalkComponent(new Rectangle(-8, -8, 88, 8), new Vector2(0.0f, -24f), new Action<Player>(this.OnTalk), (TalkComponent.HoverDisplay) null)));
    }

    private void OnTalk(Player player)
    {
      this.Level.StartCutscene(new Action<Level>(this.OnTalkEnd), true, false);
      this.Add((Component) (this.talkRoutine = new Coroutine(this.Talk(player), true)));
    }

    private IEnumerator Talk(Player player)
    {
      if (this.currentConversation == 0)
      {
        yield return (object) this.PlayerApproachRightSide(player, true, new float?());
        yield return (object) Textbox.Say("CH1_THEO_A", new Func<IEnumerator>(((NPC) this).PlayerApproach48px));
      }
      else if (this.currentConversation == 1)
      {
        yield return (object) this.PlayerApproachRightSide(player, true, new float?());
        yield return (object) 0.2f;
        yield return (object) this.PlayerApproach(player, true, new float?(48f), new int?());
        yield return (object) Textbox.Say("CH1_THEO_B");
      }
      else if (this.currentConversation == 2)
      {
        yield return (object) this.PlayerApproachRightSide(player, true, new float?(48f));
        yield return (object) Textbox.Say("CH1_THEO_C");
      }
      else if (this.currentConversation == 3)
      {
        yield return (object) this.PlayerApproachRightSide(player, true, new float?(48f));
        yield return (object) Textbox.Say("CH1_THEO_D");
      }
      else if (this.currentConversation == 4)
      {
        yield return (object) this.PlayerApproachRightSide(player, true, new float?(48f));
        yield return (object) Textbox.Say("CH1_THEO_E");
      }
      else if (this.currentConversation == 5)
      {
        yield return (object) this.PlayerApproachRightSide(player, true, new float?(48f));
        yield return (object) Textbox.Say("CH1_THEO_F", new Func<IEnumerator>(this.Yolo));
        this.Sprite.Play("yoloEnd", false, false);
        this.Remove((Component) this.Talker);
        yield return (object) this.Level.ZoomBack(0.5f);
      }
      this.Level.EndCutscene();
      this.OnTalkEnd(this.Level);
    }

    private void OnTalkEnd(Level level)
    {
      if (this.currentConversation == 0)
        SaveData.Instance.SetFlag("MetTheo");
      else if (this.currentConversation == 1)
        SaveData.Instance.SetFlag("TheoKnowsName");
      else if (this.currentConversation == 5)
      {
        this.Session.SetFlag("theoDoneTalking", true);
        this.Remove((Component) this.Talker);
      }
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity != null)
      {
        entity.StateMachine.Locked = false;
        entity.StateMachine.State = Player.StNormal;
      }
      this.Session.IncrementCounter("theo");
      ++this.currentConversation;
      this.talkRoutine.Cancel();
      this.talkRoutine.RemoveSelf();
      this.Sprite.Play("idle", false, false);
    }

    private IEnumerator Yolo()
    {
      yield return (object) this.Level.ZoomTo(new Vector2(128f, 128f), 2f, 0.5f);
      yield return (object) 0.2f;
      Audio.Play("event:/char/theo/yolo_fist", this.Position);
      this.Sprite.Play("yolo", false, false);
      yield return (object) 0.1f;
      this.Level.DirectionalShake(-Vector2.UnitY, 0.3f);
      this.Level.ParticlesFG.Emit(NPC01_Theo.P_YOLO, 6, this.Position + new Vector2(-3f, -24f), Vector2.One * 4f);
      yield return (object) 0.5f;
    }
  }
}

