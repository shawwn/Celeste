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
    private float optionEase = 0.0f;
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
      level.Camera.Position = new Vector2((float) level.Bounds.Left, this.bonfire.Y - 144f);
      level.ZoomSnap(new Vector2(80f, 120f), 2f);
      this.cameraStart = level.Camera.Position;
      this.theo.X = level.Camera.X - 48f;
      this.theoCampfirePosition = new Vector2(this.bonfire.X - 16f, this.bonfire.Y);
      this.player.Light.Alpha = 0.0f;
      this.player.X = (float) (level.Bounds.Left - 40);
      this.player.StateMachine.State = Player.StDummy;
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
      yield return (object) 0.1f;
      this.Add((Component) new Coroutine(this.PlayerLightApproach(), true));
      Coroutine camTo;
      this.Add((Component) (camTo = new Coroutine(CutsceneEntity.CameraTo(new Vector2(level.Camera.X + 90f, level.Camera.Y), 6f, Ease.CubeIn, 0.0f), true)));
      this.player.DummyAutoAnimate = false;
      this.player.Sprite.Play("carryTheoWalk", false, false);
      for (float p = 0.0f; (double) p < 3.5; p += Engine.DeltaTime)
      {
        SpotlightWipe.FocusPoint = new Vector2(40f, 120f);
        this.player.NaiveMove(new Vector2(32f * Engine.DeltaTime, 0.0f));
        yield return (object) null;
      }
      this.player.Sprite.Play("carryTheoCollapse", false, false);
      Audio.Play("event:/char/madeline/theo_collapse", this.player.Position);
      yield return (object) 0.3f;
      Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
      Vector2 at = this.player.Position + new Vector2(16f, 1f);
      this.Level.ParticlesFG.Emit(Payphone.P_Snow, 2, at, Vector2.UnitX * 4f);
      this.Level.ParticlesFG.Emit(Payphone.P_SnowB, 12, at, Vector2.UnitX * 10f);
      yield return (object) 0.7f;
      FadeWipe fade = new FadeWipe((Scene) level, false, (Action) null);
      fade.Duration = 1.5f;
      fade.EndTimer = 2.5f;
      yield return (object) fade.Wait();
      this.bonfire.SetMode(Bonfire.Mode.Lit);
      yield return (object) 2.45f;
      camTo.Cancel();
      this.theo.Position = this.theoCampfirePosition;
      this.theo.Sprite.Play("sleep", false, false);
      this.theo.Sprite.SetAnimationFrame(this.theo.Sprite.CurrentAnimationTotalFrames - 1);
      this.player.Position = this.playerCampfirePosition;
      this.player.Facing = Facings.Left;
      this.player.Sprite.Play("asleep", false, false);
      level.Session.SetFlag("starsbg", true);
      level.Session.SetFlag("duskbg", false);
      fade.EndTimer = 0.0f;
      FadeWipe fadeWipe = new FadeWipe((Scene) level, true, (Action) null);
      yield return (object) null;
      level.ResetZoom();
      level.Camera.Position = new Vector2(this.bonfire.X - 160f, this.bonfire.Y - 140f);
      yield return (object) 3f;
      Audio.SetMusic("event:/music/lvl6/madeline_and_theo", true, true);
      yield return (object) 1.5f;
      this.Add((Component) Wiggler.Create(0.6f, 3f, (Action<float>) (v => this.theo.Sprite.Scale = Vector2.One * (float) (1.0 + 0.100000001490116 * (double) v)), true, true));
      this.Level.Particles.Emit(NPC01_Theo.P_YOLO, 4, this.theo.Position + new Vector2(-4f, -14f), Vector2.One * 3f);
      yield return (object) 0.5f;
      this.theo.Sprite.Play("wakeup", false, false);
      yield return (object) 1f;
      this.player.Sprite.Play("halfWakeUp", false, false);
      yield return (object) 0.25f;
      yield return (object) Textbox.Say("ch6_theo_intro");
      string node = "start";
      while (!string.IsNullOrEmpty(node) && this.nodes.ContainsKey(node))
      {
        this.currentOptionIndex = 0;
        this.currentOptions = new List<CS06_Campfire.Option>();
        CS06_Campfire.Option[] optionArray = this.nodes[node];
        for (int index = 0; index < optionArray.Length; ++index)
        {
          CS06_Campfire.Option option = optionArray[index];
          if (option.CanAsk(this.asked))
            this.currentOptions.Add(option);
          option = (CS06_Campfire.Option) null;
        }
        optionArray = (CS06_Campfire.Option[]) null;
        if (this.currentOptions.Count > 0)
        {
          Audio.Play("event:/ui/game/chatoptions_appear");
          while ((double) (this.optionEase += Engine.DeltaTime * 4f) < 1.0)
            yield return (object) null;
          this.optionEase = 1f;
          yield return (object) 0.25f;
          while (!Input.MenuConfirm.Pressed)
          {
            if (Input.MenuUp.Pressed && this.currentOptionIndex > 0)
            {
              Audio.Play("event:/ui/game/chatoptions_roll_up");
              --this.currentOptionIndex;
            }
            else if (Input.MenuDown.Pressed && this.currentOptionIndex < this.currentOptions.Count - 1)
            {
              Audio.Play("event:/ui/game/chatoptions_roll_down");
              ++this.currentOptionIndex;
            }
            yield return (object) null;
          }
          Audio.Play("event:/ui/game/chatoptions_select");
          while ((double) (this.optionEase -= Engine.DeltaTime * 4f) > 0.0)
            yield return (object) null;
          CS06_Campfire.Option selected = this.currentOptions[this.currentOptionIndex];
          this.asked.Add(selected.Question);
          this.currentOptions = (List<CS06_Campfire.Option>) null;
          yield return (object) Textbox.Say(selected.Question.Answer, new Func<IEnumerator>(this.WaitABit), new Func<IEnumerator>(this.SelfieSequence), new Func<IEnumerator>(this.BeerSequence));
          node = selected.Goto;
          if (!string.IsNullOrEmpty(node))
            selected = (CS06_Campfire.Option) null;
          else
            break;
        }
        else
          break;
      }
      FadeWipe wipe = new FadeWipe((Scene) level, false, (Action) null);
      wipe.Duration = 3f;
      yield return (object) wipe.Wait();
      this.EndCutscene(level, true);
    }

    private IEnumerator WaitABit()
    {
      yield return (object) 0.8f;
    }

    private IEnumerator SelfieSequence()
    {
      this.Add((Component) new Coroutine(this.Level.ZoomTo(new Vector2(160f, 105f), 2f, 0.5f), true));
      yield return (object) 0.1f;
      this.theo.Sprite.Play("idle", false, false);
      this.Add((Component) Alarm.Create(Alarm.AlarmMode.Oneshot, (Action) (() => this.theo.Sprite.Scale.X = -1f), 0.25f, true));
      this.player.DummyAutoAnimate = true;
      yield return (object) this.player.DummyWalkToExact((int) ((double) this.theo.X + 5.0), false, 0.7f);
      yield return (object) 0.2f;
      Audio.Play("event:/game/02_old_site/theoselfie_foley", this.theo.Position);
      this.theo.Sprite.Play("takeSelfie", false, false);
      yield return (object) 1f;
      this.selfie = new Selfie(this.SceneAs<Level>());
      this.Scene.Add((Entity) this.selfie);
      yield return (object) this.selfie.PictureRoutine("selfieCampfire");
      this.selfie = (Selfie) null;
      yield return (object) 0.5f;
      yield return (object) this.Level.ZoomBack(0.5f);
      yield return (object) 0.2f;
      this.theo.Sprite.Scale.X = 1f;
      yield return (object) this.player.DummyWalkToExact((int) this.playerCampfirePosition.X, false, 0.7f);
      this.theo.Sprite.Play("wakeup", false, false);
      yield return (object) 0.1;
      this.player.DummyAutoAnimate = false;
      this.player.Facing = Facings.Left;
      this.player.Sprite.Play("sleep", false, false);
      yield return (object) 2f;
      this.player.Sprite.Play("halfWakeUp", false, false);
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
      level.Add((Entity) new FlyFeather(level.LevelOffset + new Vector2(272f, 2616f), false, false));
      this.SetBloom(1f);
      this.bonfire.Activated = false;
      this.bonfire.SetMode(Bonfire.Mode.Lit);
      this.theo.Sprite.Play("sleep", false, false);
      this.theo.Sprite.SetAnimationFrame(this.theo.Sprite.CurrentAnimationTotalFrames - 1);
      this.theo.Sprite.Scale.X = 1f;
      this.theo.Position = this.theoCampfirePosition;
      this.player.Sprite.Play("asleep", false, false);
      this.player.Position = this.playerCampfirePosition;
      this.player.StateMachine.Locked = false;
      this.player.StateMachine.State = Player.StIntroWakeUp;
      this.player.Speed = Vector2.Zero;
      this.player.Facing = Facings.Left;
      level.Camera.Position = this.player.CameraTarget;
      if (this.WasSkipped)
        this.player.StateMachine.State = Player.StNormal;
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
        currentOption.Render(new Vector2(260f, (float) (120.0 + 160.0 * (double) num)), this.optionEase);
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
        float amount = Ease.CubeInOut(this.Highlight);
        position.Y += (float) (-32.0 * (1.0 - (double) num1));
        position.X += amount * 32f;
        Color color1 = Color.Lerp(Color.Gray, Color.White, amount) * num1;
        float alpha = MathHelper.Lerp(0.6f, 1f, amount) * num1;
        Color color2 = Color.White * (float) (0.5 + (double) amount * 0.5);
        GFX.Portraits[this.Question.Textbox].Draw(position, Vector2.Zero, color1);
        Facings facings = this.Question.PortraitSide;
        if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
          facings = ToFacing.Convert(-(int) facings);
        float num2 = 100f;
        this.Question.Portrait.Scale = Vector2.One * (num2 / this.Question.PortraitSize);
        if (facings == Facings.Right)
        {
          this.Question.Portrait.Position = position + new Vector2((float) (1380.0 - (double) num2 * 0.5), 70f);
          this.Question.Portrait.Scale.X *= -1f;
        }
        else
          this.Question.Portrait.Position = position + new Vector2((float) (20.0 + (double) num2 * 0.5), 70f);
        this.Question.Portrait.Color = color2 * num1;
        this.Question.Portrait.Render();
        float num3 = (float) ((140.0 - (double) ActiveFont.LineHeight * 0.699999988079071) / 2.0);
        Vector2 position1 = new Vector2(0.0f, position.Y + 70f);
        Vector2 justify = new Vector2(0.0f, 0.5f);
        if (facings == Facings.Right)
        {
          justify.X = 1f;
          position1.X = (float) ((double) position.X + 1400.0 - 20.0) - num3 - num2;
        }
        else
          position1.X = position.X + 20f + num3 + num2;
        this.Question.AskText.Draw(position1, justify, Vector2.One * 0.7f, alpha, 0, int.MaxValue);
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

