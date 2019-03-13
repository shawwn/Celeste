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
    private int currentConversation;
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
      NPC02_Theo npC02Theo = this;
      if (!SaveData.Instance.HasFlag("MetTheo"))
      {
        npC02Theo.Session.SetFlag("hadntMetTheoAtStart", true);
        SaveData.Instance.SetFlag("MetTheo");
        yield return (object) npC02Theo.PlayerApproachRightSide(player, true, new float?(48f));
        yield return (object) Textbox.Say("CH2_THEO_INTRO_NEVER_MET");
      }
      else if (!SaveData.Instance.HasFlag("TheoKnowsName"))
      {
        npC02Theo.Session.SetFlag("hadntMetTheoAtStart", true);
        SaveData.Instance.SetFlag("TheoKnowsName");
        yield return (object) npC02Theo.PlayerApproachRightSide(player, true, new float?(48f));
        yield return (object) Textbox.Say("CH2_THEO_INTRO_NEVER_INTRODUCED");
      }
      else if (npC02Theo.currentConversation <= 0)
      {
        yield return (object) npC02Theo.PlayerApproachRightSide(player, true, new float?());
        yield return (object) 0.2f;
        if (npC02Theo.Session.GetFlag("hadntMetTheoAtStart"))
        {
          yield return (object) npC02Theo.PlayerApproach48px();
          yield return (object) Textbox.Say("CH2_THEO_A", new Func<IEnumerator>(npC02Theo.ShowPhotos), new Func<IEnumerator>(npC02Theo.HidePhotos), new Func<IEnumerator>(npC02Theo.Selfie));
        }
        else
          yield return (object) Textbox.Say("CH2_THEO_A_EXT", new Func<IEnumerator>(npC02Theo.ShowPhotos), new Func<IEnumerator>(npC02Theo.HidePhotos), new Func<IEnumerator>(npC02Theo.Selfie), new Func<IEnumerator>(((NPC) npC02Theo).PlayerApproach48px));
      }
      else if (npC02Theo.currentConversation == 1)
      {
        yield return (object) npC02Theo.PlayerApproachRightSide(player, true, new float?(48f));
        yield return (object) Textbox.Say("CH2_THEO_B", new Func<IEnumerator>(npC02Theo.SelfieFiltered));
      }
      else if (npC02Theo.currentConversation == 2)
      {
        yield return (object) npC02Theo.PlayerApproachRightSide(player, true, new float?(48f));
        yield return (object) Textbox.Say("CH2_THEO_C");
      }
      else if (npC02Theo.currentConversation == 3)
      {
        yield return (object) npC02Theo.PlayerApproachRightSide(player, true, new float?(48f));
        yield return (object) Textbox.Say("CH2_THEO_D");
      }
      else if (npC02Theo.currentConversation == 4)
      {
        yield return (object) npC02Theo.PlayerApproachRightSide(player, true, new float?(48f));
        yield return (object) Textbox.Say("CH2_THEO_E");
      }
      npC02Theo.Level.EndCutscene();
      npC02Theo.OnTalkEnd(npC02Theo.Level);
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
        entity.StateMachine.State = 0;
        if (level.SkippingCutscene)
        {
          entity.X = (float) (int) ((double) this.X + 48.0);
          entity.Facing = Facings.Left;
        }
      }
      this.Sprite.Scale.X = (__Null) 1.0;
      if (this.selfie != null)
        this.selfie.RemoveSelf();
      this.Session.IncrementCounter("theo");
      ++this.currentConversation;
      this.talkRoutine.Cancel();
      this.talkRoutine.RemoveSelf();
    }

    private IEnumerator ShowPhotos()
    {
      NPC02_Theo npC02Theo = this;
      Player entity = npC02Theo.Scene.Tracker.GetEntity<Player>();
      yield return (object) npC02Theo.PlayerApproach(entity, true, new float?(10f), new int?());
      npC02Theo.Sprite.Play("getPhone", false, false);
      yield return (object) 2f;
    }

    private IEnumerator HidePhotos()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      NPC02_Theo npC02Theo = this;
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
      npC02Theo.Sprite.Play("idle", false, false);
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) 0.5f;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }

    private IEnumerator Selfie()
    {
      NPC02_Theo npC02Theo = this;
      yield return (object) 0.5f;
      Audio.Play("event:/game/02_old_site/theoselfie_foley", npC02Theo.Position);
      npC02Theo.Sprite.Scale.X = -npC02Theo.Sprite.Scale.X;
      npC02Theo.Sprite.Play("takeSelfie", false, false);
      yield return (object) 1f;
      npC02Theo.Scene.Add((Entity) (npC02Theo.selfie = new Selfie(npC02Theo.SceneAs<Level>())));
      yield return (object) npC02Theo.selfie.PictureRoutine("selfie");
      npC02Theo.selfie = (Selfie) null;
      npC02Theo.Sprite.Scale.X = -npC02Theo.Sprite.Scale.X;
    }

    private IEnumerator SelfieFiltered()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      NPC02_Theo npC02Theo = this;
      if (num != 0)
      {
        if (num != 1)
          return false;
        // ISSUE: reference to a compiler-generated field
        this.\u003C\u003E1__state = -1;
        npC02Theo.selfie = (Selfie) null;
        return false;
      }
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      npC02Theo.Scene.Add((Entity) (npC02Theo.selfie = new Selfie(npC02Theo.SceneAs<Level>())));
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) npC02Theo.selfie.FilterRoutine();
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }
  }
}
