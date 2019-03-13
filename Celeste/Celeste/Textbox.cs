// Decompiled with JetBrains decompiler
// Type: Celeste.Textbox
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Celeste
{
  [Tracked(false)]
  public class Textbox : Entity
  {
    private MTexture textbox = GFX.Portraits["textbox/default"];
    private bool canSkip = true;
    private Sprite portraitSprite = new Sprite((Atlas) null, (string) null);
    private float portraitScale = 1.5f;
    private Dictionary<string, SoundSource> talkers = new Dictionary<string, SoundSource>();
    private MTexture textboxOverlay;
    private const int textboxInnerWidth = 1688;
    private const int textboxInnerHeight = 272;
    private const float portraitPadding = 16f;
    private const float tweenDuration = 0.4f;
    private const float switchToIdleAnimationDelay = 0.5f;
    private readonly float innerTextPadding;
    private readonly float maxLineWidthNoPortrait;
    private readonly float maxLineWidth;
    private readonly int linesPerPage;
    private const int stopVoiceCharactersEarly = 4;
    private float ease;
    private FancyText.Text text;
    private Func<IEnumerator>[] events;
    private Coroutine runRoutine;
    private Coroutine skipRoutine;
    private PixelFont font;
    private float lineHeight;
    private FancyText.Anchors anchor;
    private FancyText.Portrait portrait;
    private int index;
    private bool waitingForInput;
    private bool disableInput;
    private int shakeSeed;
    private float timer;
    private float gradientFade;
    private bool isInTrigger;
    private bool easingClose;
    private bool easingOpen;
    public Vector2 RenderOffset;
    private bool autoPressContinue;
    private char lastChar;
    private bool portraitExists;
    private bool portraitIdling;
    private Wiggler portraitWiggle;
    private SoundSource activeTalker;
    private SoundSource phonestatic;

    public bool Opened { get; private set; }

    public int Page { get; private set; }

    public List<FancyText.Node> Nodes
    {
      get
      {
        return this.text.Nodes;
      }
    }

    public bool UseRawDeltaTime
    {
      set
      {
        this.runRoutine.UseRawDeltaTime = value;
      }
    }

    public int Start { get; private set; }

    public string PortraitName
    {
      get
      {
        if (this.portrait == null || this.portrait.Sprite == null)
          return "";
        return this.portrait.Sprite;
      }
    }

    public string PortraitAnimation
    {
      get
      {
        if (this.portrait == null || this.portrait.Sprite == null)
          return "";
        return this.portrait.Animation;
      }
    }

    public Textbox(string dialog, params Func<IEnumerator>[] events)
      : this(dialog, (Language) null, events)
    {
    }

    public Textbox(string dialog, Language language, params Func<IEnumerator>[] events)
    {
      this.Tag = (int) Tags.PauseUpdate | (int) Tags.HUD;
      this.Opened = true;
      this.font = Dialog.Language.Font;
      this.lineHeight = (float) (Dialog.Language.FontSize.LineHeight - 1);
      this.portraitSprite.UseRawDeltaTime = true;
      this.Add((Component) (this.portraitWiggle = Wiggler.Create(0.4f, 4f, (Action<float>) null, false, false)));
      this.events = events;
      this.linesPerPage = (int) (240.0 / (double) this.lineHeight);
      this.innerTextPadding = (float) ((272.0 - (double) this.lineHeight * (double) this.linesPerPage) / 2.0);
      this.maxLineWidthNoPortrait = (float) (1688.0 - (double) this.innerTextPadding * 2.0);
      this.maxLineWidth = (float) ((double) this.maxLineWidthNoPortrait - 240.0 - 32.0);
      this.text = FancyText.Parse(Dialog.Get(dialog, language), (int) this.maxLineWidth, this.linesPerPage, 0.0f, new Color?(), language);
      this.index = 0;
      this.Start = 0;
      this.skipRoutine = new Coroutine(this.SkipDialog(), true);
      this.runRoutine = new Coroutine(this.RunRoutine(), true);
      this.runRoutine.UseRawDeltaTime = true;
      if ((HandleBase) Level.DialogSnapshot == (HandleBase) null)
        Level.DialogSnapshot = Audio.CreateSnapshot("snapshot:/dialogue_in_progress", false);
      Audio.ResumeSnapshot(Level.DialogSnapshot);
      this.Add((Component) (this.phonestatic = new SoundSource()));
    }

    public void SetStart(int value)
    {
      this.index = this.Start = value;
    }

    private IEnumerator RunRoutine()
    {
      Textbox textbox = this;
      FancyText.Node last = (FancyText.Node) null;
      float delayBuildup = 0.0f;
      while (textbox.index < textbox.Nodes.Count)
      {
        FancyText.Node current = textbox.Nodes[textbox.index];
        float delay = 0.0f;
        if (current is FancyText.Anchor)
        {
          if (Vector2.op_Equality(textbox.RenderOffset, Vector2.get_Zero()))
          {
            FancyText.Anchors next = (current as FancyText.Anchor).Position;
            if ((double) textbox.ease >= 1.0 && next != textbox.anchor)
              yield return (object) textbox.EaseClose(false);
            textbox.anchor = next;
          }
        }
        else if (current is FancyText.Portrait)
        {
          FancyText.Portrait next = current as FancyText.Portrait;
          textbox.phonestatic.Stop(true);
          if ((double) textbox.ease >= 1.0 && (textbox.portrait == null || next.Sprite != textbox.portrait.Sprite || next.Side != textbox.portrait.Side))
            yield return (object) textbox.EaseClose(false);
          textbox.textbox = GFX.Portraits["textbox/default"];
          textbox.textboxOverlay = (MTexture) null;
          textbox.portraitExists = false;
          textbox.activeTalker = (SoundSource) null;
          XmlElement xml = (XmlElement) null;
          if (!string.IsNullOrEmpty(next.Sprite))
          {
            if (GFX.PortraitsSpriteBank.Has(next.SpriteId))
              xml = GFX.PortraitsSpriteBank.SpriteData[next.SpriteId].Sources[0].XML;
            textbox.portraitExists = xml != null;
          }
          if (textbox.portraitExists)
          {
            if (textbox.portrait == null || next.Sprite != textbox.portrait.Sprite)
            {
              GFX.PortraitsSpriteBank.CreateOn(textbox.portraitSprite, next.SpriteId);
              textbox.portraitScale = 240f / (float) xml.AttrInt("size", 160);
              if (!textbox.talkers.ContainsKey(next.SfxEvent))
              {
                SoundSource soundSource = new SoundSource().Play(next.SfxEvent, (string) null, 0.0f);
                textbox.talkers.Add(next.SfxEvent, soundSource);
                textbox.Add((Component) soundSource);
              }
            }
            if (textbox.talkers.ContainsKey(next.SfxEvent))
              textbox.activeTalker = textbox.talkers[next.SfxEvent];
            string index = "textbox/" + xml.Attr("textbox", "default");
            textbox.textbox = GFX.Portraits[index];
            if (GFX.Portraits.Has(index + "_overlay"))
              textbox.textboxOverlay = GFX.Portraits[index + "_overlay"];
            string str = xml.Attr("phonestatic", "");
            if (!string.IsNullOrEmpty(str))
            {
              if (str == "ex")
                textbox.phonestatic.Play("event:/char/dialogue/sfx_support/phone_static_ex", (string) null, 0.0f);
              else if (str == "mom")
                textbox.phonestatic.Play("event:/char/dialogue/sfx_support/phone_static_mom", (string) null, 0.0f);
            }
            textbox.canSkip = false;
            FancyText.Portrait portrait = textbox.portrait;
            textbox.portrait = next;
            if (next.Pop)
              textbox.portraitWiggle.Start();
            if (portrait == null || portrait.Sprite != next.Sprite || portrait.Animation != next.Animation)
            {
              if (textbox.portraitSprite.Has(next.BeginAnimation))
              {
                textbox.portraitSprite.Play(next.BeginAnimation, true, false);
                yield return (object) textbox.EaseOpen();
                while (textbox.portraitSprite.CurrentAnimationID == next.BeginAnimation && textbox.portraitSprite.Animating)
                  yield return (object) null;
              }
              if (textbox.portraitSprite.Has(next.IdleAnimation))
              {
                textbox.portraitIdling = true;
                textbox.portraitSprite.Play(next.IdleAnimation, true, false);
              }
            }
            yield return (object) textbox.EaseOpen();
            textbox.canSkip = true;
          }
          else
          {
            textbox.portrait = (FancyText.Portrait) null;
            yield return (object) textbox.EaseOpen();
          }
          next = (FancyText.Portrait) null;
        }
        else if (current is FancyText.NewPage)
        {
          textbox.PlayIdleAnimation();
          if ((double) textbox.ease >= 1.0)
          {
            textbox.waitingForInput = true;
            yield return (object) 0.1f;
            while (!textbox.ContinuePressed())
              yield return (object) null;
            textbox.waitingForInput = false;
          }
          textbox.Start = textbox.index + 1;
          textbox.Page++;
        }
        else if (current is FancyText.Wait)
        {
          textbox.PlayIdleAnimation();
          delay = (current as FancyText.Wait).Duration;
        }
        else if (current is FancyText.Trigger)
        {
          textbox.isInTrigger = true;
          textbox.PlayIdleAnimation();
          FancyText.Trigger trigger = current as FancyText.Trigger;
          if (!trigger.Silent)
            yield return (object) textbox.EaseClose(false);
          int index = trigger.Index;
          if (textbox.events != null && index >= 0 && index < textbox.events.Length)
            yield return (object) textbox.events[index]();
          textbox.isInTrigger = false;
          trigger = (FancyText.Trigger) null;
        }
        else if (current is FancyText.Char)
        {
          FancyText.Char ch = current as FancyText.Char;
          textbox.lastChar = (char) ch.Character;
          if ((double) textbox.ease < 1.0)
            yield return (object) textbox.EaseOpen();
          bool flag = false;
          if (textbox.index - 5 > textbox.Start)
          {
            for (int index = textbox.index; index < Math.Min(textbox.index + 4, textbox.Nodes.Count); ++index)
            {
              if (textbox.Nodes[index] is FancyText.NewPage)
              {
                flag = true;
                textbox.PlayIdleAnimation();
              }
            }
          }
          if (!flag && !ch.IsPunctuation)
            textbox.PlayTalkAnimation();
          if (last != null && last is FancyText.NewPage)
          {
            --textbox.index;
            yield return (object) 0.2f;
            ++textbox.index;
          }
          delay = ch.Delay + delayBuildup;
          ch = (FancyText.Char) null;
        }
        last = current;
        ++textbox.index;
        if ((double) delay < 0.0160000007599592)
        {
          delayBuildup += delay;
        }
        else
        {
          delayBuildup = 0.0f;
          if ((double) delay > 0.5)
            textbox.PlayIdleAnimation();
          yield return (object) delay;
        }
        current = (FancyText.Node) null;
      }
      textbox.PlayIdleAnimation();
      if ((double) textbox.ease > 0.0)
      {
        textbox.waitingForInput = true;
        while (!textbox.ContinuePressed())
          yield return (object) null;
        textbox.waitingForInput = false;
        textbox.Start = textbox.Nodes.Count;
        yield return (object) textbox.EaseClose(true);
      }
      textbox.Close();
    }

    private void PlayIdleAnimation()
    {
      this.StopTalker();
      if (this.portraitIdling || this.portraitSprite == null || (this.portrait == null || !this.portraitSprite.Has(this.portrait.IdleAnimation)))
        return;
      this.portraitSprite.Play(this.portrait.IdleAnimation, false, false);
      this.portraitIdling = true;
    }

    private void StopTalker()
    {
      if (this.activeTalker == null)
        return;
      this.activeTalker.Param("dialogue_portrait", 0.0f);
      this.activeTalker.Param("dialogue_end", 1f);
    }

    private void PlayTalkAnimation()
    {
      this.StartTalker();
      if (!this.portraitIdling || this.portraitSprite == null || (this.portrait == null || !this.portraitSprite.Has(this.portrait.TalkAnimation)))
        return;
      this.portraitSprite.Play(this.portrait.TalkAnimation, false, false);
      this.portraitIdling = false;
    }

    private void StartTalker()
    {
      if (this.activeTalker == null)
        return;
      this.activeTalker.Param("dialogue_portrait", this.portrait != null ? (float) this.portrait.SfxExpression : 1f);
      this.activeTalker.Param("dialogue_end", 0.0f);
    }

    private IEnumerator EaseOpen()
    {
      Textbox textbox1 = this;
      if ((double) textbox1.ease < 1.0)
      {
        textbox1.easingOpen = true;
        if (textbox1.portrait != null && textbox1.portrait.Sprite.IndexOf("madeline", StringComparison.InvariantCultureIgnoreCase) >= 0)
          Audio.Play("event:/ui/game/textbox_madeline_in");
        else
          Audio.Play("event:/ui/game/textbox_other_in");
        while (true)
        {
          Textbox textbox2 = textbox1;
          double ease = (double) textbox1.ease;
          double num1 = (textbox1.runRoutine.UseRawDeltaTime ? (double) Engine.RawDeltaTime : (double) Engine.DeltaTime) / 0.400000005960464;
          double num2;
          float num3 = (float) (num2 = ease + num1);
          textbox2.ease = (float) num2;
          if ((double) num3 < 1.0)
          {
            textbox1.gradientFade = Math.Max(textbox1.gradientFade, textbox1.ease);
            yield return (object) null;
          }
          else
            break;
        }
        textbox1.ease = textbox1.gradientFade = 1f;
        textbox1.easingOpen = false;
      }
    }

    private IEnumerator EaseClose(bool final)
    {
      Textbox textbox1 = this;
      textbox1.easingClose = true;
      if (textbox1.portrait != null && textbox1.portrait.Sprite.IndexOf("madeline", StringComparison.InvariantCultureIgnoreCase) >= 0)
        Audio.Play("event:/ui/game/textbox_madeline_out");
      else
        Audio.Play("event:/ui/game/textbox_other_out");
      while (true)
      {
        Textbox textbox2 = textbox1;
        double ease = (double) textbox1.ease;
        double num1 = (textbox1.runRoutine.UseRawDeltaTime ? (double) Engine.RawDeltaTime : (double) Engine.DeltaTime) / 0.400000005960464;
        double num2;
        float num3 = (float) (num2 = ease - num1);
        textbox2.ease = (float) num2;
        if ((double) num3 > 0.0)
        {
          if (final)
            textbox1.gradientFade = textbox1.ease;
          yield return (object) null;
        }
        else
          break;
      }
      textbox1.ease = 0.0f;
      textbox1.easingClose = false;
    }

    private IEnumerator SkipDialog()
    {
      while (true)
      {
        if (!this.waitingForInput && this.canSkip && (!this.easingOpen && !this.easingClose) && this.ContinuePressed())
        {
          this.StopTalker();
          this.disableInput = true;
          while (!this.waitingForInput && this.canSkip && (!this.easingOpen && !this.easingClose) && (!this.isInTrigger && !this.runRoutine.Finished))
            this.runRoutine.Update();
        }
        yield return (object) null;
        this.disableInput = false;
      }
    }

    public bool SkipToPage(int page)
    {
      this.autoPressContinue = true;
      while (this.Page != page && !this.runRoutine.Finished)
        this.Update();
      this.autoPressContinue = false;
      this.Update();
      while (this.Opened && (double) this.ease < 1.0)
        this.Update();
      if (this.Page == page)
        return this.Opened;
      return false;
    }

    public void Close()
    {
      this.Opened = false;
      if (this.Scene == null)
        return;
      this.Scene.Remove((Entity) this);
    }

    private bool ContinuePressed()
    {
      if (this.autoPressContinue)
        return true;
      if (Input.MenuConfirm.Pressed || Input.MenuCancel.Pressed)
        return !this.disableInput;
      return false;
    }

    public override void Update()
    {
      Level scene = this.Scene as Level;
      if (scene != null && (scene.FrozenOrPaused || scene.RetryPlayerCorpse != null))
        return;
      if (!this.autoPressContinue)
        this.skipRoutine.Update();
      this.runRoutine.Update();
      if (this.Scene != null && this.Scene.OnInterval(0.05f))
        this.shakeSeed = Calc.Random.Next();
      if (this.portraitSprite != null && (double) this.ease >= 1.0)
        this.portraitSprite.Update();
      this.timer += Engine.DeltaTime;
      this.portraitWiggle.Update();
      int num = Math.Min(this.index, this.Nodes.Count);
      for (int start = this.Start; start < num; ++start)
      {
        if (this.Nodes[start] is FancyText.Char)
        {
          FancyText.Char node = this.Nodes[start] as FancyText.Char;
          if ((double) node.Fade < 1.0)
            node.Fade = Calc.Clamp(node.Fade + 8f * Engine.DeltaTime, 0.0f, 1f);
        }
      }
    }

    public override void Render()
    {
      Level scene = this.Scene as Level;
      if (scene != null && (scene.FrozenOrPaused || scene.RetryPlayerCorpse != null || scene.SkippingCutscene))
        return;
      float alpha = Ease.CubeInOut(this.ease);
      if ((double) alpha < 0.0500000007450581)
        return;
      float num1 = 116f;
      Vector2 vector2_1 = Vector2.op_Addition(new Vector2(num1, num1 / 2f), this.RenderOffset);
      if (Vector2.op_Equality(this.RenderOffset, Vector2.get_Zero()))
      {
        if (this.anchor == FancyText.Anchors.Bottom)
          ((Vector2) ref vector2_1).\u002Ector(num1, (float) (1080.0 - (double) num1 / 2.0 - 272.0));
        else if (this.anchor == FancyText.Anchors.Middle)
          ((Vector2) ref vector2_1).\u002Ector(num1, 404f);
        ref __Null local = ref vector2_1.Y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local = ^(float&) ref local + (float) (int) (136.0 * (1.0 - (double) alpha));
      }
      this.textbox.DrawCentered(Vector2.op_Addition(vector2_1, Vector2.op_Division(new Vector2(1688f, 272f * alpha), 2f)), Color.get_White(), new Vector2(1f, alpha));
      if (this.waitingForInput)
      {
        float num2 = this.portrait == null || this.PortraitSide(this.portrait) < 0 ? 1688f : 1432f;
        Vector2 position = Vector2.op_Addition(new Vector2((float) vector2_1.X + num2, (float) (vector2_1.Y + 272.0)), new Vector2(-48f, (float) (((double) this.timer % 1.0 < 0.25 ? 6 : 0) - 40)));
        GFX.Gui["textboxbutton"].DrawCentered(position);
      }
      if (this.portraitExists)
      {
        if (this.PortraitSide(this.portrait) > 0)
        {
          this.portraitSprite.Position = new Vector2((float) (vector2_1.X + 1688.0 - 240.0 - 16.0), (float) vector2_1.Y);
          this.portraitSprite.Scale.X = (__Null) -(double) this.portraitScale;
        }
        else
        {
          this.portraitSprite.Position = new Vector2((float) (vector2_1.X + 16.0), (float) vector2_1.Y);
          this.portraitSprite.Scale.X = (__Null) (double) this.portraitScale;
        }
        ref __Null local = ref this.portraitSprite.Scale.X;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local = ^(float&) ref local * (this.portrait.Flipped ? -1f : 1f);
        this.portraitSprite.Scale.Y = (__Null) ((double) this.portraitScale * ((272.0 * (double) alpha - 32.0) / 240.0) * (this.portrait.UpsideDown ? -1.0 : 1.0));
        Sprite portraitSprite1 = this.portraitSprite;
        portraitSprite1.Scale = Vector2.op_Multiply(portraitSprite1.Scale, (float) (0.899999976158142 + (double) this.portraitWiggle.Value * 0.100000001490116));
        Sprite portraitSprite2 = this.portraitSprite;
        portraitSprite2.Position = Vector2.op_Addition(portraitSprite2.Position, new Vector2(120f, (float) (272.0 * (double) alpha * 0.5)));
        this.portraitSprite.Color = Color.op_Multiply(Color.get_White(), alpha);
        if ((double) Math.Abs((float) this.portraitSprite.Scale.Y) > 0.0500000007450581)
          this.portraitSprite.Render();
      }
      if (this.textboxOverlay != null)
      {
        int num2 = 1;
        if (this.portrait != null && this.PortraitSide(this.portrait) > 0)
          num2 = -1;
        this.textboxOverlay.DrawCentered(Vector2.op_Addition(vector2_1, Vector2.op_Division(new Vector2(1688f, 272f * alpha), 2f)), Color.get_White(), new Vector2((float) num2, alpha));
      }
      Calc.PushRandom(this.shakeSeed);
      int num3 = 1;
      for (int start = this.Start; start < this.text.Nodes.Count; ++start)
      {
        if (this.text.Nodes[start] is FancyText.NewLine)
          ++num3;
        else if (this.text.Nodes[start] is FancyText.NewPage)
          break;
      }
      Vector2 vector2_2;
      ((Vector2) ref vector2_2).\u002Ector(this.innerTextPadding + (this.portrait == null || this.PortraitSide(this.portrait) >= 0 ? 0.0f : 256f), this.innerTextPadding);
      Vector2 vector2_3 = Vector2.op_Division(new Vector2(this.portrait == null ? this.maxLineWidthNoPortrait : this.maxLineWidth, (float) this.linesPerPage * this.lineHeight * alpha), 2f);
      float num4 = num3 >= 4 ? 0.75f : 1f;
      this.text.Draw(Vector2.op_Addition(Vector2.op_Addition(vector2_1, vector2_2), vector2_3), new Vector2(0.5f, 0.5f), Vector2.op_Multiply(new Vector2(1f, alpha), num4), alpha, this.Start, int.MaxValue);
      Calc.PopRandom();
    }

    public int PortraitSide(FancyText.Portrait portrait)
    {
      if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
        return -portrait.Side;
      return portrait.Side;
    }

    public override void Removed(Scene scene)
    {
      Audio.EndSnapshot(Level.DialogSnapshot);
      base.Removed(scene);
    }

    public override void SceneEnd(Scene scene)
    {
      Audio.EndSnapshot(Level.DialogSnapshot);
      base.SceneEnd(scene);
    }

    public static IEnumerator Say(string dialog, params Func<IEnumerator>[] events)
    {
      Textbox textbox = new Textbox(dialog, events);
      Engine.Scene.Add((Entity) textbox);
      while (textbox.Opened)
        yield return (object) null;
    }
  }
}
