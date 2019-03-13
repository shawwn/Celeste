// Decompiled with JetBrains decompiler
// Type: Celeste.NPC02_Theo
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class NPC02_Theo : NPC
  {
    private int currentConversation = 0;
    private const string DoneTalking = "theoDoneTalking";
    private const string HadntMetAtStart = "hadntMetTheoAtStart";
    private Coroutine talkRoutine;
    private Selfie selfie;

    public NPC02_Theo(Vector2 position)
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
      this.Add((Component) (this.Talker = new TalkComponent(new Rectangle(-20, -8, 100, 8), new Vector2(0.0f, -24f), new Action<Player>(this.OnTalk), (TalkComponent.HoverDisplay) null)));
    }

    private void OnTalk(Player player)
    {
      if (!SaveData.Instance.HasFlag("MetTheo") || !SaveData.Instance.HasFlag("TheoKnowsName"))
        this.currentConversation = -1;
      this.Level.StartCutscene(new Action<Level>(this.OnTalkEnd), true, false);
      this.Add((Component) (this.talkRoutine = new Coroutine(this.Talk(player), true)));
    }

    private IEnumerator Talk(Player player)
    {
      if (!SaveData.Instance.HasFlag("MetTheo"))
      {
        this.Session.SetFlag("hadntMetTheoAtStart", true);
        SaveData.Instance.SetFlag("MetTheo");
        yield return (object) this.PlayerApproachRightSide(player, true, new float?(48f));
        yield return (object) Textbox.Say("CH2_THEO_INTRO_NEVER_MET");
      }
      else if (!SaveData.Instance.HasFlag("TheoKnowsName"))
      {
        this.Session.SetFlag("hadntMetTheoAtStart", true);
        SaveData.Instance.SetFlag("TheoKnowsName");
        yield return (object) this.PlayerApproachRightSide(player, true, new float?(48f));
        yield return (object) Textbox.Say("CH2_THEO_INTRO_NEVER_INTRODUCED");
      }
      else if (this.currentConversation <= 0)
      {
        yield return (object) this.PlayerApproachRightSide(player, true, new float?());
        yield return (object) 0.2f;
        if (this.Session.GetFlag("hadntMetTheoAtStart"))
        {
          yield return (object) this.PlayerApproach48px();
          yield return (object) Textbox.Say("CH2_THEO_A", new Func<IEnumerator>(this.ShowPhotos), new Func<IEnumerator>(this.HidePhotos), new Func<IEnumerator>(this.Selfie));
        }
        else
          yield return (object) Textbox.Say("CH2_THEO_A_EXT", new Func<IEnumerator>(this.ShowPhotos), new Func<IEnumerator>(this.HidePhotos), new Func<IEnumerator>(this.Selfie), new Func<IEnumerator>(((NPC) this).PlayerApproach48px));
      }
      else if (this.currentConversation == 1)
      {
        yield return (object) this.PlayerApproachRightSide(player, true, new float?(48f));
        yield return (object) Textbox.Say("CH2_THEO_B", new Func<IEnumerator>(this.SelfieFiltered));
      }
      else if (this.currentConversation == 2)
      {
        yield return (object) this.PlayerApproachRightSide(player, true, new float?(48f));
        yield return (object) Textbox.Say("CH2_THEO_C");
      }
      else if (this.currentConversation == 3)
      {
        yield return (object) this.PlayerApproachRightSide(player, true, new float?(48f));
        yield return (object) Textbox.Say("CH2_THEO_D");
      }
      else if (this.currentConversation == 4)
      {
        yield return (object) this.PlayerApproachRightSide(player, true, new float?(48f));
        yield return (object) Textbox.Say("CH2_THEO_E");
      }
      this.Level.EndCutscene();
      this.OnTalkEnd(this.Level);
    }

    private void OnTalkEnd(Level level)
    {
      if (this.currentConversation == 4)
      {
        this.Session.SetFlag("theoDoneTalking", true);
        this.Remove((Component) this.Talker);
      }
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity != null)
      {
        entity.StateMachine.Locked = false;
        entity.StateMachine.State = Player.StNormal;
        if (level.SkippingCutscene)
        {
          entity.X = (float) (int) ((double) this.X + 48.0);
          entity.Facing = Facings.Left;
        }
      }
      this.Sprite.Scale.X = 1f;
      if (this.selfie != null)
        this.selfie.RemoveSelf();
      this.Session.IncrementCounter("theo");
      ++this.currentConversation;
      this.talkRoutine.Cancel();
      this.talkRoutine.RemoveSelf();
    }

    private IEnumerator ShowPhotos()
    {
      Player player = this.Scene.Tracker.GetEntity<Player>();
      yield return (object) this.PlayerApproach(player, true, new float?(10f), new int?());
      this.Sprite.Play("getPhone", false, false);
      yield return (object) 2f;
    }

    private IEnumerator HidePhotos()
    {
      this.Sprite.Play("idle", false, false);
      yield return (object) 0.5f;
    }

    private IEnumerator Selfie()
    {
      yield return (object) 0.5f;
      Audio.Play("event:/game/02_old_site/theoselfie_foley", this.Position);
      this.Sprite.Scale.X = -this.Sprite.Scale.X;
      this.Sprite.Play("takeSelfie", false, false);
      yield return (object) 1f;
      this.Scene.Add((Entity) (this.selfie = new Selfie(this.SceneAs<Level>())));
      yield return (object) this.selfie.PictureRoutine("selfie");
      this.selfie = (Selfie) null;
      this.Sprite.Scale.X = -this.Sprite.Scale.X;
    }

    private IEnumerator SelfieFiltered()
    {
      this.Scene.Add((Entity) (this.selfie = new Selfie(this.SceneAs<Level>())));
      yield return (object) this.selfie.FilterRoutine();
      this.selfie = (Selfie) null;
    }
  }
}

