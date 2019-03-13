// Decompiled with JetBrains decompiler
// Type: Celeste.CS06_Campfire
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Celeste
{
  public class CS06_Campfire : CutsceneEntity
  {
    private Dictionary<string, CS06_Campfire.Option[]> nodes = new Dictionary<string, CS06_Campfire.Option[]>();
    private HashSet<CS06_Campfire.Question> asked = new HashSet<CS06_Campfire.Question>();
    private List<CS06_Campfire.Option> currentOptions = new List<CS06_Campfire.Option>();
    public const string Flag = "campfire_chat";
    public const string DuskBackgroundFlag = "duskbg";
    public const string StarsBackgrundFlag = "starsbg";
    private NPC theo;
    private Player player;
    private Bonfire bonfire;
    private Plateau plateau;
    private Vector2 cameraStart;
    private Vector2 playerCampfirePosition;
    private Vector2 theoCampfirePosition;
    private Selfie selfie;
    private float optionEase;
    private int currentOptionIndex;

    public CS06_Campfire(NPC theo, Player player)
      : base(true, false)
    {
      this.Tag = (int) Tags.HUD;
      this.theo = theo;
      this.player = player;
      CS06_Campfire.Question question1 = new CS06_Campfire.Question("outfor");
      CS06_Campfire.Question question2 = new CS06_Campfire.Question("temple");
      CS06_Campfire.Question question3 = new CS06_Campfire.Question("explain");
      CS06_Campfire.Question question4 = new CS06_Campfire.Question("thankyou");
      CS06_Campfire.Question question5 = new CS06_Campfire.Question("why");
      CS06_Campfire.Question question6 = new CS06_Campfire.Question("depression");
      CS06_Campfire.Question question7 = new CS06_Campfire.Question("defense");
      CS06_Campfire.Question question8 = new CS06_Campfire.Question("vacation");
      CS06_Campfire.Question question9 = new CS06_Campfire.Question("trust");
      CS06_Campfire.Question question10 = new CS06_Campfire.Question("family");
      CS06_Campfire.Question question11 = new CS06_Campfire.Question("grandpa");
      CS06_Campfire.Question question12 = new CS06_Campfire.Question("tips");
      CS06_Campfire.Question question13 = new CS06_Campfire.Question(nameof (selfie));
      CS06_Campfire.Question question14 = new CS06_Campfire.Question("sleep");
      CS06_Campfire.Question question15 = new CS06_Campfire.Question("sleep_confirm");
      CS06_Campfire.Question question16 = new CS06_Campfire.Question("sleep_cancel");
      this.nodes.Add("start", new CS06_Campfire.Option[14]
      {
        new CS06_Campfire.Option(question1, "start").ExcludedBy(question5),
        new CS06_Campfire.Option(question2, "start").Require(question9),
        new CS06_Campfire.Option(question9, "start").Require(question3),
        new CS06_Campfire.Option(question10, "start").Require(question9, question5),
        new CS06_Campfire.Option(question11, "start").Require(question10, question7),
        new CS06_Campfire.Option(question12, "start").Require(question11),
        new CS06_Campfire.Option(question3, "start"),
        new CS06_Campfire.Option(question4, "start").Require(question3),
        new CS06_Campfire.Option(question5, "start").Require(question3, question9),
        new CS06_Campfire.Option(question6, "start").Require(question5),
        new CS06_Campfire.Option(question7, "start").Require(question6),
        new CS06_Campfire.Option(question8, "start").Require(question6),
        new CS06_Campfire.Option(question13, "").Require(question7, question11),
        new CS06_Campfire.Option(question14, "sleep").Require(question5).ExcludedBy(question7, question11).Repeatable()
      });
      this.nodes.Add("sleep", new CS06_Campfire.Option[2]
      {
        new CS06_Campfire.Option(question16, "start").Repeatable(),
        new CS06_Campfire.Option(question15, "")
      });
    }

    public override void OnBegin(Level level)
    {
      Audio.SetMusic((string) null, false, false);
      level.SnapColorGrade((string) null);
      level.Bloom.Base = 0.0f;
      level.Session.SetFlag("duskbg", true);
      this.plateau = this.Scene.Entities.FindFirst<Plateau>();
      this.bonfire = this.Scene.Tracker.GetEntity<Bonfire>();
      Camera camera = level.Camera;
      Rectangle bounds1 = level.Bounds;
      Vector2 vector2 = new Vector2((float) ((Rectangle) ref bounds1).get_Left(), this.bonfire.Y - 144f);
      camera.Position = vector2;
      level.ZoomSnap(new Vector2(80f, 120f), 2f);
      this.cameraStart = level.Camera.Position;
      this.theo.X = level.Camera.X - 48f;
      this.theoCampfirePosition = new Vector2(this.bonfire.X - 16f, this.bonfire.Y);
      this.player.Light.Alpha = 0.0f;
      Player player = this.player;
      Rectangle bounds2 = level.Bounds;
      double num = (double) (((Rectangle) ref bounds2).get_Left() - 40);
      player.X = (float) num;
      this.player.StateMachine.State = 11;
      this.player.StateMachine.Locked = true;
      this.playerCampfirePosition = new Vector2(this.bonfire.X + 20f, this.bonfire.Y);
      if (level.Session.GetFlag("campfire_chat"))
      {
        this.WasSkipped = true;
        level.ResetZoom();
        level.EndCutscene();
        this.EndCutscene(level, true);
      }
      else
        this.Add((Component) new Coroutine(this.Cutscene(level), true));
    }

    private IEnumerator PlayerLightApproach()
    {
      while ((double) this.player.Light.Alpha < 1.0)
      {
        this.player.Light.Alpha = Calc.Approach(this.player.Light.Alpha, 1f, Engine.DeltaTime * 2f);
        yield return (object) null;
      }
    }

    private IEnumerator Cutscene(Level level)
    {
      CS06_Campfire cs06Campfire = this;
      yield return (object) 0.1f;
      cs06Campfire.Add((Component) new Coroutine(cs06Campfire.PlayerLightApproach(), true));
      Coroutine camTo;
      cs06Campfire.Add((Component) (camTo = new Coroutine(CutsceneEntity.CameraTo(new Vector2(level.Camera.X + 90f, level.Camera.Y), 6f, Ease.CubeIn, 0.0f), true)));
      cs06Campfire.player.DummyAutoAnimate = false;
      cs06Campfire.player.Sprite.Play("carryTheoWalk", false, false);
      for (float p = 0.0f; (double) p < 3.5; p += Engine.DeltaTime)
      {
        SpotlightWipe.FocusPoint = new Vector2(40f, 120f);
        cs06Campfire.player.NaiveMove(new Vector2(32f * Engine.DeltaTime, 0.0f));
        yield return (object) null;
      }
      cs06Campfire.player.Sprite.Play("carryTheoCollapse", false, false);
      Audio.Play("event:/char/madeline/theo_collapse", cs06Campfire.player.Position);
      yield return (object) 0.3f;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      Vector2 position = Vector2.op_Addition(cs06Campfire.player.Position, new Vector2(16f, 1f));
      cs06Campfire.Level.ParticlesFG.Emit(Payphone.P_Snow, 2, position, Vector2.op_Multiply(Vector2.get_UnitX(), 4f));
      cs06Campfire.Level.ParticlesFG.Emit(Payphone.P_SnowB, 12, position, Vector2.op_Multiply(Vector2.get_UnitX(), 10f));
      yield return (object) 0.7f;
      FadeWipe fade = new FadeWipe((Scene) level, false, (Action) null);
      fade.Duration = 1.5f;
      fade.EndTimer = 2.5f;
      yield return (object) fade.Wait();
      cs06Campfire.bonfire.SetMode(Bonfire.Mode.Lit);
      yield return (object) 2.45f;
      camTo.Cancel();
      cs06Campfire.theo.Position = cs06Campfire.theoCampfirePosition;
      cs06Campfire.theo.Sprite.Play("sleep", false, false);
      cs06Campfire.theo.Sprite.SetAnimationFrame(cs06Campfire.theo.Sprite.CurrentAnimationTotalFrames - 1);
      cs06Campfire.player.Position = cs06Campfire.playerCampfirePosition;
      cs06Campfire.player.Facing = Facings.Left;
      cs06Campfire.player.Sprite.Play("asleep", false, false);
      level.Session.SetFlag("starsbg", true);
      level.Session.SetFlag("duskbg", false);
      fade.EndTimer = 0.0f;
      FadeWipe fadeWipe1 = new FadeWipe((Scene) level, true, (Action) null);
      yield return (object) null;
      level.ResetZoom();
      level.Camera.Position = new Vector2(cs06Campfire.bonfire.X - 160f, cs06Campfire.bonfire.Y - 140f);
      yield return (object) 3f;
      Audio.SetMusic("event:/music/lvl6/madeline_and_theo", true, true);
      yield return (object) 1.5f;
      // ISSUE: reference to a compiler-generated method
      cs06Campfire.Add((Component) Wiggler.Create(0.6f, 3f, new Action<float>(cs06Campfire.\u003CCutscene\u003Eb__21_0), true, true));
      cs06Campfire.Level.Particles.Emit(NPC01_Theo.P_YOLO, 4, Vector2.op_Addition(cs06Campfire.theo.Position, new Vector2(-4f, -14f)), Vector2.op_Multiply(Vector2.get_One(), 3f));
      yield return (object) 0.5f;
      cs06Campfire.theo.Sprite.Play("wakeup", false, false);
      yield return (object) 1f;
      cs06Campfire.player.Sprite.Play("halfWakeUp", false, false);
      yield return (object) 0.25f;
      yield return (object) Textbox.Say("ch6_theo_intro");
      string key = "start";
      while (!string.IsNullOrEmpty(key) && cs06Campfire.nodes.ContainsKey(key))
      {
        cs06Campfire.currentOptionIndex = 0;
        cs06Campfire.currentOptions = new List<CS06_Campfire.Option>();
        foreach (CS06_Campfire.Option option in cs06Campfire.nodes[key])
        {
          if (option.CanAsk(cs06Campfire.asked))
            cs06Campfire.currentOptions.Add(option);
        }
        if (cs06Campfire.currentOptions.Count > 0)
        {
          Audio.Play("event:/ui/game/chatoptions_appear");
          while ((double) (cs06Campfire.optionEase += Engine.DeltaTime * 4f) < 1.0)
            yield return (object) null;
          cs06Campfire.optionEase = 1f;
          yield return (object) 0.25f;
          while (!Input.MenuConfirm.Pressed)
          {
            if (Input.MenuUp.Pressed && cs06Campfire.currentOptionIndex > 0)
            {
              Audio.Play("event:/ui/game/chatoptions_roll_up");
              --cs06Campfire.currentOptionIndex;
            }
            else if (Input.MenuDown.Pressed && cs06Campfire.currentOptionIndex < cs06Campfire.currentOptions.Count - 1)
            {
              Audio.Play("event:/ui/game/chatoptions_roll_down");
              ++cs06Campfire.currentOptionIndex;
            }
            yield return (object) null;
          }
          Audio.Play("event:/ui/game/chatoptions_select");
          while ((double) (cs06Campfire.optionEase -= Engine.DeltaTime * 4f) > 0.0)
            yield return (object) null;
          CS06_Campfire.Option selected = cs06Campfire.currentOptions[cs06Campfire.currentOptionIndex];
          cs06Campfire.asked.Add(selected.Question);
          cs06Campfire.currentOptions = (List<CS06_Campfire.Option>) null;
          yield return (object) Textbox.Say(selected.Question.Answer, new Func<IEnumerator>(cs06Campfire.WaitABit), new Func<IEnumerator>(cs06Campfire.SelfieSequence), new Func<IEnumerator>(cs06Campfire.BeerSequence));
          key = selected.Goto;
          if (!string.IsNullOrEmpty(key))
            selected = (CS06_Campfire.Option) null;
          else
            break;
        }
        else
          break;
      }
      FadeWipe fadeWipe2 = new FadeWipe((Scene) level, false, (Action) null);
      fadeWipe2.Duration = 3f;
      yield return (object) fadeWipe2.Wait();
      cs06Campfire.EndCutscene(level, true);
    }

    private IEnumerator WaitABit()
    {
      yield return (object) 0.8f;
    }

    private IEnumerator SelfieSequence()
    {
      CS06_Campfire cs06Campfire = this;
      cs06Campfire.Add((Component) new Coroutine(cs06Campfire.Level.ZoomTo(new Vector2(160f, 105f), 2f, 0.5f), true));
      yield return (object) 0.1f;
      cs06Campfire.theo.Sprite.Play("idle", false, false);
      // ISSUE: reference to a compiler-generated method
      cs06Campfire.Add((Component) Alarm.Create(Alarm.AlarmMode.Oneshot, new Action(cs06Campfire.\u003CSelfieSequence\u003Eb__23_0), 0.25f, true));
      cs06Campfire.player.DummyAutoAnimate = true;
      yield return (object) cs06Campfire.player.DummyWalkToExact((int) ((double) cs06Campfire.theo.X + 5.0), false, 0.7f);
      yield return (object) 0.2f;
      Audio.Play("event:/game/02_old_site/theoselfie_foley", cs06Campfire.theo.Position);
      cs06Campfire.theo.Sprite.Play("takeSelfie", false, false);
      yield return (object) 1f;
      cs06Campfire.selfie = new Selfie(cs06Campfire.SceneAs<Level>());
      cs06Campfire.Scene.Add((Entity) cs06Campfire.selfie);
      yield return (object) cs06Campfire.selfie.PictureRoutine("selfieCampfire");
      cs06Campfire.selfie = (Selfie) null;
      yield return (object) 0.5f;
      yield return (object) cs06Campfire.Level.ZoomBack(0.5f);
      yield return (object) 0.2f;
      cs06Campfire.theo.Sprite.Scale.X = (__Null) 1.0;
      yield return (object) cs06Campfire.player.DummyWalkToExact((int) cs06Campfire.playerCampfirePosition.X, false, 0.7f);
      cs06Campfire.theo.Sprite.Play("wakeup", false, false);
      yield return (object) 0.1;
      cs06Campfire.player.DummyAutoAnimate = false;
      cs06Campfire.player.Facing = Facings.Left;
      cs06Campfire.player.Sprite.Play("sleep", false, false);
      yield return (object) 2f;
      cs06Campfire.player.Sprite.Play("halfWakeUp", false, false);
    }

    private IEnumerator BeerSequence()
    {
      yield return (object) 0.5f;
    }

    public override void OnEnd(Level level)
    {
      if (!this.WasSkipped)
      {
        level.ZoomSnap(new Vector2(160f, 120f), 2f);
        FadeWipe fadeWipe = new FadeWipe((Scene) level, true, (Action) null);
        fadeWipe.Duration = 3f;
        Coroutine zoom = new Coroutine(level.ZoomBack(fadeWipe.Duration), true);
        fadeWipe.OnUpdate = (Action<float>) (f => zoom.Update());
      }
      if (this.selfie != null)
        this.selfie.RemoveSelf();
      level.Session.SetFlag("campfire_chat", true);
      level.Session.SetFlag("starsbg", false);
      level.Session.SetFlag("duskbg", false);
      level.Session.Dreaming = true;
      level.Add((Entity) new StarJumpController());
      level.Add((Entity) new CS06_StarJumpEnd(this.theo, this.player, this.playerCampfirePosition, this.cameraStart));
      level.Add((Entity) new FlyFeather(Vector2.op_Addition(level.LevelOffset, new Vector2(272f, 2616f)), false, false));
      this.SetBloom(1f);
      this.bonfire.Activated = false;
      this.bonfire.SetMode(Bonfire.Mode.Lit);
      this.theo.Sprite.Play("sleep", false, false);
      this.theo.Sprite.SetAnimationFrame(this.theo.Sprite.CurrentAnimationTotalFrames - 1);
      this.theo.Sprite.Scale.X = (__Null) 1.0;
      this.theo.Position = this.theoCampfirePosition;
      this.player.Sprite.Play("asleep", false, false);
      this.player.Position = this.playerCampfirePosition;
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = 15;
      this.player.Speed = Vector2.get_Zero();
      this.player.Facing = Facings.Left;
      level.Camera.Position = this.player.CameraTarget;
      if (this.WasSkipped)
        this.player.StateMachine.State = 0;
      this.RemoveSelf();
    }

    private void SetBloom(float add)
    {
      this.Level.Session.BloomBaseAdd = add;
      this.Level.Bloom.Base = AreaData.Get((Scene) this.Level).BloomBase + add;
    }

    public override void Update()
    {
      if (this.currentOptions != null)
      {
        for (int index = 0; index < this.currentOptions.Count; ++index)
        {
          this.currentOptions[index].Update();
          this.currentOptions[index].Highlight = Calc.Approach(this.currentOptions[index].Highlight, this.currentOptionIndex == index ? 1f : 0.0f, Engine.DeltaTime * 4f);
        }
      }
      base.Update();
    }

    public override void Render()
    {
      if (this.Level.Paused || this.currentOptions == null)
        return;
      int num = 0;
      foreach (CS06_Campfire.Option currentOption in this.currentOptions)
      {
        Vector2 vector2;
        ((Vector2) ref vector2).\u002Ector(260f, (float) (120.0 + 160.0 * (double) num));
        Vector2 position = vector2;
        double optionEase = (double) this.optionEase;
        currentOption.Render(position, (float) optionEase);
        ++num;
      }
    }

    private class Option
    {
      public CS06_Campfire.Question Question;
      public string Goto;
      public List<CS06_Campfire.Question> OnlyAppearIfAsked;
      public List<CS06_Campfire.Question> DoNotAppearIfAsked;
      public bool CanRepeat;
      public float Highlight;
      public const float Width = 1400f;
      public const float Height = 140f;
      public const float Padding = 20f;
      public const float TextScale = 0.7f;

      public Option(CS06_Campfire.Question question, string go)
      {
        this.Question = question;
        this.Goto = go;
      }

      public CS06_Campfire.Option Require(
        params CS06_Campfire.Question[] onlyAppearIfAsked)
      {
        this.OnlyAppearIfAsked = new List<CS06_Campfire.Question>((IEnumerable<CS06_Campfire.Question>) onlyAppearIfAsked);
        return this;
      }

      public CS06_Campfire.Option ExcludedBy(
        params CS06_Campfire.Question[] doNotAppearIfAsked)
      {
        this.DoNotAppearIfAsked = new List<CS06_Campfire.Question>((IEnumerable<CS06_Campfire.Question>) doNotAppearIfAsked);
        return this;
      }

      public CS06_Campfire.Option Repeatable()
      {
        this.CanRepeat = true;
        return this;
      }

      public bool CanAsk(HashSet<CS06_Campfire.Question> asked)
      {
        if (!this.CanRepeat && asked.Contains(this.Question))
          return false;
        if (this.OnlyAppearIfAsked != null)
        {
          foreach (CS06_Campfire.Question question in this.OnlyAppearIfAsked)
          {
            if (!asked.Contains(question))
              return false;
          }
        }
        if (this.DoNotAppearIfAsked != null)
        {
          bool flag = true;
          foreach (CS06_Campfire.Question question in this.DoNotAppearIfAsked)
          {
            if (!asked.Contains(question))
            {
              flag = false;
              break;
            }
          }
          if (flag)
            return false;
        }
        return true;
      }

      public void Update()
      {
        this.Question.Portrait.Update();
      }

      public void Render(Vector2 position, float ease)
      {
        float num1 = Ease.CubeOut(ease);
        float num2 = Ease.CubeInOut(this.Highlight);
        ref __Null local1 = ref position.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local1 = ^(float&) ref local1 + (float) (-32.0 * (1.0 - (double) num1));
        ref __Null local2 = ref position.X;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local2 = ^(float&) ref local2 + num2 * 32f;
        Color color1 = Color.op_Multiply(Color.Lerp(Color.get_Gray(), Color.get_White(), num2), num1);
        float alpha = MathHelper.Lerp(0.6f, 1f, num2) * num1;
        Color color2 = Color.op_Multiply(Color.get_White(), (float) (0.5 + (double) num2 * 0.5));
        GFX.Portraits[this.Question.Textbox].Draw(position, Vector2.get_Zero(), color1);
        Facings facings = this.Question.PortraitSide;
        if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
          facings = (Facings) -(int) facings;
        float num3 = 100f;
        this.Question.Portrait.Scale = Vector2.op_Multiply(Vector2.get_One(), num3 / this.Question.PortraitSize);
        if (facings == Facings.Right)
        {
          this.Question.Portrait.Position = Vector2.op_Addition(position, new Vector2((float) (1380.0 - (double) num3 * 0.5), 70f));
          ref __Null local3 = ref this.Question.Portrait.Scale.X;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local3 = ^(float&) ref local3 * -1f;
        }
        else
          this.Question.Portrait.Position = Vector2.op_Addition(position, new Vector2((float) (20.0 + (double) num3 * 0.5), 70f));
        this.Question.Portrait.Color = Color.op_Multiply(color2, num1);
        this.Question.Portrait.Render();
        float num4 = (float) ((140.0 - (double) ActiveFont.LineHeight * 0.699999988079071) / 2.0);
        Vector2 position1;
        ((Vector2) ref position1).\u002Ector(0.0f, (float) (position.Y + 70.0));
        Vector2 justify;
        ((Vector2) ref justify).\u002Ector(0.0f, 0.5f);
        if (facings == Facings.Right)
        {
          justify.X = (__Null) 1.0;
          position1.X = (__Null) (position.X + 1400.0 - 20.0 - (double) num4 - (double) num3);
        }
        else
          position1.X = (__Null) (position.X + 20.0 + (double) num4 + (double) num3);
        this.Question.AskText.Draw(position1, justify, Vector2.op_Multiply(Vector2.get_One(), 0.7f), alpha, 0, int.MaxValue);
      }
    }

    private class Question
    {
      public string Ask;
      public string Answer;
      public string Textbox;
      public FancyText.Text AskText;
      public Sprite Portrait;
      public Facings PortraitSide;
      public float PortraitSize;

      public Question(string id)
      {
        int maxLineWidth = 1828;
        this.Ask = "ch6_theo_ask_" + id;
        this.Answer = "ch6_theo_say_" + id;
        this.AskText = FancyText.Parse(Dialog.Get(this.Ask, (Language) null), maxLineWidth, -1, 1f, new Color?(), (Language) null);
        foreach (FancyText.Node node in this.AskText.Nodes)
        {
          if (node is FancyText.Portrait)
          {
            FancyText.Portrait portrait = node as FancyText.Portrait;
            this.Portrait = GFX.PortraitsSpriteBank.Create(portrait.SpriteId);
            this.Portrait.Play(portrait.IdleAnimation, false, false);
            this.PortraitSide = (Facings) portrait.Side;
            this.Textbox = "textbox/" + portrait.Sprite + "_ask";
            XmlElement xml = GFX.PortraitsSpriteBank.SpriteData[portrait.SpriteId].Sources[0].XML;
            if (xml == null)
              break;
            this.PortraitSize = (float) xml.AttrInt("size", 160);
            break;
          }
        }
      }
    }
  }
}
