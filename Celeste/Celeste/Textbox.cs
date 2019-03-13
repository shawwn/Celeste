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
    private MTexture textboxOverlay = (MTexture) null;
    private float ease = 0.0f;
    private FancyText.Anchors anchor = FancyText.Anchors.Top;
    private int index = 0;
    private int shakeSeed = 0;
    private float timer = 0.0f;
    private float gradientFade = 0.0f;
    private bool canSkip = true;
    private Sprite portraitSprite = new Sprite((Atlas) null, (string) null);
    private bool portraitExists = false;
    private bool portraitIdling = false;
    private float portraitScale = 1.5f;
    private Dictionary<string, SoundSource> talkers = new Dictionary<string, SoundSource>();
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
    private FancyText.Text text;
    private Func<IEnumerator>[] events;
    private Coroutine runRoutine;
    private Coroutine skipRoutine;
    private PixelFont font;
    private float lineHeight;
    private FancyText.Portrait portrait;
    private bool waitingForInput;
    private bool disableInput;
    private bool isInTrigger;
    private bool easingClose;
    private bool easingOpen;
    public Vector2 RenderOffset;
    private bool autoPressContinue;
    private char lastChar;
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
        return this.portrait == null || this.portrait.Sprite == null ? "" : this.portrait.Sprite;
      }
    }

    public string PortraitAnimation
    {
      get
      {
        return this.portrait == null || this.portrait.Sprite == null ? "" : this.portrait.Animation;
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
      FancyText.Node last = (FancyText.Node) null;
      float delayBuildup = 0.0f;
      while (this.index < this.Nodes.Count)
      {
        FancyText.Node current = this.Nodes[this.index];
        float delay = 0.0f;
        if (current is FancyText.Anchor)
        {
          if (this.RenderOffset == Vector2.Zero)
          {
            FancyText.Anchors next = (current as FancyText.Anchor).Position;
            if ((double) this.ease >= 1.0 && next != this.anchor)
              yield return (object) this.EaseClose(false);
            this.anchor = next;
          }
        }
        else if (current is FancyText.Portrait)
        {
          FancyText.Portrait next = current as FancyText.Portrait;
          this.phonestatic.Stop(true);
          if ((double) this.ease >= 1.0 && (this.portrait == null || next.Sprite != this.portrait.Sprite || next.Side != this.portrait.Side))
            yield return (object) this.EaseClose(false);
          this.textbox = GFX.Portraits["textbox/default"];
          this.textboxOverlay = (MTexture) null;
          this.portraitExists = false;
          this.activeTalker = (SoundSource) null;
          XmlElement xml = (XmlElement) null;
          if (!string.IsNullOrEmpty(next.Sprite))
          {
            if (GFX.PortraitsSpriteBank.Has(next.SpriteId))
              xml = GFX.PortraitsSpriteBank.SpriteData[next.SpriteId].Sources[0].XML;
            this.portraitExists = xml != null;
          }
          if (this.portraitExists)
          {
            if (this.portrait == null || next.Sprite != this.portrait.Sprite)
            {
              GFX.PortraitsSpriteBank.CreateOn(this.portraitSprite, next.SpriteId);
              this.portraitScale = 240f / (float) xml.AttrInt("size", 160);
              if (!this.talkers.ContainsKey(next.SfxEvent))
              {
                SoundSource sfx = new SoundSource().Play(next.SfxEvent, (string) null, 0.0f);
                this.talkers.Add(next.SfxEvent, sfx);
                this.Add((Component) sfx);
                sfx = (SoundSource) null;
              }
            }
            if (this.talkers.ContainsKey(next.SfxEvent))
              this.activeTalker = this.talkers[next.SfxEvent];
            string tex = "textbox/" + xml.Attr("textbox", "default");
            this.textbox = GFX.Portraits[tex];
            if (GFX.Portraits.Has(tex + "_overlay"))
              this.textboxOverlay = GFX.Portraits[tex + "_overlay"];
            tex = (string) null;
            string stat = xml.Attr("phonestatic", "");
            if (!string.IsNullOrEmpty(stat))
            {
              if (stat == "ex")
                this.phonestatic.Play("event:/char/dialogue/sfx_support/phone_static_ex", (string) null, 0.0f);
              else if (stat == "mom")
                this.phonestatic.Play("event:/char/dialogue/sfx_support/phone_static_mom", (string) null, 0.0f);
            }
            stat = (string) null;
            this.canSkip = false;
            FancyText.Portrait was = this.portrait;
            this.portrait = next;
            if (next.Pop)
              this.portraitWiggle.Start();
            if (was == null || was.Sprite != next.Sprite || was.Animation != next.Animation)
            {
              if (this.portraitSprite.Has(next.BeginAnimation))
              {
                this.portraitSprite.Play(next.BeginAnimation, true, false);
                yield return (object) this.EaseOpen();
                while (this.portraitSprite.CurrentAnimationID == next.BeginAnimation && this.portraitSprite.Animating)
                  yield return (object) null;
              }
              if (this.portraitSprite.Has(next.IdleAnimation))
              {
                this.portraitIdling = true;
                this.portraitSprite.Play(next.IdleAnimation, true, false);
              }
            }
            yield return (object) this.EaseOpen();
            was = (FancyText.Portrait) null;
            this.canSkip = true;
          }
          else
          {
            this.portrait = (FancyText.Portrait) null;
            yield return (object) this.EaseOpen();
          }
          next = (FancyText.Portrait) null;
          xml = (XmlElement) null;
        }
        else if (current is FancyText.NewPage)
        {
          this.PlayIdleAnimation();
          if ((double) this.ease >= 1.0)
          {
            this.waitingForInput = true;
            yield return (object) 0.1f;
            while (!this.ContinuePressed())
              yield return (object) null;
            this.waitingForInput = false;
          }
          this.Start = this.index + 1;
          ++this.Page;
        }
        else if (current is FancyText.Wait)
        {
          this.PlayIdleAnimation();
          delay = (current as FancyText.Wait).Duration;
        }
        else if (current is FancyText.Trigger)
        {
          this.isInTrigger = true;
          this.PlayIdleAnimation();
          FancyText.Trigger trigger = current as FancyText.Trigger;
          if (!trigger.Silent)
            yield return (object) this.EaseClose(false);
          int triggerIndex = trigger.Index;
          if (this.events != null && triggerIndex >= 0 && triggerIndex < this.events.Length)
            yield return (object) this.events[triggerIndex]();
          this.isInTrigger = false;
          trigger = (FancyText.Trigger) null;
        }
        else if (current is FancyText.Char)
        {
          FancyText.Char ch = current as FancyText.Char;
          this.lastChar = (char) ch.Character;
          if ((double) this.ease < 1.0)
            yield return (object) this.EaseOpen();
          bool idling = false;
          if (this.index - 5 > this.Start)
          {
            for (int i = this.index; i < Math.Min(this.index + 4, this.Nodes.Count); ++i)
            {
              if (this.Nodes[i] is FancyText.NewPage)
              {
                idling = true;
                this.PlayIdleAnimation();
              }
            }
          }
          if (!idling && !ch.IsPunctuation)
            this.PlayTalkAnimation();
          if (last != null && last is FancyText.NewPage)
          {
            --this.index;
            yield return (object) 0.2f;
            ++this.index;
          }
          delay = ch.Delay + delayBuildup;
          ch = (FancyText.Char) null;
        }
        last = current;
        ++this.index;
        if ((double) delay < 0.0160000007599592)
        {
          delayBuildup += delay;
        }
        else
        {
          delayBuildup = 0.0f;
          if ((double) delay > 0.5)
            this.PlayIdleAnimation();
          yield return (object) delay;
        }
        current = (FancyText.Node) null;
      }
      this.PlayIdleAnimation();
      if ((double) this.ease > 0.0)
      {
        this.waitingForInput = true;
        while (!this.ContinuePressed())
          yield return (object) null;
        this.waitingForInput = false;
        this.Start = this.Nodes.Count;
        yield return (object) this.EaseClose(true);
      }
      this.Close();
    }

    private void PlayIdleAnimation()
    {
      this.StopTalker();
      if (this.portraitIdling || this.portraitSprite == null || this.portrait == null || !this.portraitSprite.Has(this.portrait.IdleAnimation))
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
      if (!this.portraitIdling || this.portraitSprite == null || this.portrait == null || !this.portraitSprite.Has(this.portrait.TalkAnimation))
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
      if ((double) this.ease < 1.0)
      {
        this.easingOpen = true;
        if (this.portrait != null && this.portrait.Sprite.IndexOf("madeline", StringComparison.InvariantCultureIgnoreCase) >= 0)
          Audio.Play("event:/ui/game/textbox_madeline_in");
        else
          Audio.Play("event:/ui/game/textbox_other_in");
        while ((double) (this.ease += (float) ((this.runRoutine.UseRawDeltaTime ? (double) Engine.RawDeltaTime : (double) Engine.DeltaTime) / 0.400000005960464)) < 1.0)
        {
          this.gradientFade = Math.Max(this.gradientFade, this.ease);
          yield return (object) null;
        }
        this.ease = this.gradientFade = 1f;
        this.easingOpen = false;
      }
    }

    private IEnumerator EaseClose(bool final)
    {
      this.easingClose = true;
      if (this.portrait != null && this.portrait.Sprite.IndexOf("madeline", StringComparison.InvariantCultureIgnoreCase) >= 0)
        Audio.Play("event:/ui/game/textbox_madeline_out");
      else
        Audio.Play("event:/ui/game/textbox_other_out");
      while ((double) (this.ease -= (float) ((this.runRoutine.UseRawDeltaTime ? (double) Engine.RawDeltaTime : (double) Engine.DeltaTime) / 0.400000005960464)) > 0.0)
      {
        if (final)
          this.gradientFade = this.ease;
        yield return (object) null;
      }
      this.ease = 0.0f;
      this.easingClose = false;
    }

    private IEnumerator SkipDialog()
    {
      while (true)
      {
        if (!this.waitingForInput && (this.canSkip && !this.easingOpen) && !this.easingClose && this.ContinuePressed())
        {
          this.StopTalker();
          this.disableInput = true;
          while (!this.waitingForInput && this.canSkip && (!this.easingOpen && !this.easingClose) && !this.isInTrigger && !this.runRoutine.Finished)
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
      return this.Page == page && this.Opened;
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
      return this.autoPressContinue || (Input.MenuConfirm.Pressed || Input.MenuCancel.Pressed) && !this.disableInput;
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
      float num1 = Ease.CubeInOut(this.ease);
      if ((double) num1 < 0.0500000007450581)
        return;
      float x = 116f;
      Vector2 vector2_1 = new Vector2(x, x / 2f) + this.RenderOffset;
      if (this.RenderOffset == Vector2.Zero)
      {
        if (this.anchor == FancyText.Anchors.Bottom)
          vector2_1 = new Vector2(x, (float) (1080.0 - (double) x / 2.0 - 272.0));
        else if (this.anchor == FancyText.Anchors.Middle)
          vector2_1 = new Vector2(x, 404f);
        vector2_1.Y += (float) (int) (136.0 * (1.0 - (double) num1));
      }
      this.textbox.DrawCentered(vector2_1 + new Vector2(1688f, 272f * num1) / 2f, Color.White, new Vector2(1f, num1));
      if (this.waitingForInput)
      {
        float num2 = this.portrait == null || this.PortraitSide(this.portrait) < 0 ? 1688f : 1432f;
        Vector2 position = new Vector2(vector2_1.X + num2, vector2_1.Y + 272f) + new Vector2(-48f, (float) (((double) this.timer % 1.0 < 0.25 ? 6 : 0) - 40));
        GFX.Gui["textboxbutton"].DrawCentered(position);
      }
      if (this.portraitExists)
      {
        if (this.PortraitSide(this.portrait) > 0)
        {
          this.portraitSprite.Position = new Vector2((float) ((double) vector2_1.X + 1688.0 - 240.0 - 16.0), vector2_1.Y);
          this.portraitSprite.Scale.X = -this.portraitScale;
        }
        else
        {
          this.portraitSprite.Position = new Vector2(vector2_1.X + 16f, vector2_1.Y);
          this.portraitSprite.Scale.X = this.portraitScale;
        }
        this.portraitSprite.Scale.X *= this.portrait.Flipped ? -1f : 1f;
        this.portraitSprite.Scale.Y = (float) ((double) this.portraitScale * ((272.0 * (double) num1 - 32.0) / 240.0) * (this.portrait.UpsideDown ? -1.0 : 1.0));
        Sprite portraitSprite1 = this.portraitSprite;
        portraitSprite1.Scale = portraitSprite1.Scale * (float) (0.899999976158142 + (double) this.portraitWiggle.Value * 0.100000001490116);
        Sprite portraitSprite2 = this.portraitSprite;
        portraitSprite2.Position = portraitSprite2.Position + new Vector2(120f, (float) (272.0 * (double) num1 * 0.5));
        this.portraitSprite.Color = Color.White * num1;
        if ((double) Math.Abs(this.portraitSprite.Scale.Y) > 0.0500000007450581)
          this.portraitSprite.Render();
      }
      if (this.textboxOverlay != null)
      {
        int num2 = 1;
        if (this.portrait != null && this.PortraitSide(this.portrait) > 0)
          num2 = -1;
        this.textboxOverlay.DrawCentered(vector2_1 + new Vector2(1688f, 272f * num1) / 2f, Color.White, new Vector2((float) num2, num1));
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
      Vector2 vector2_2 = new Vector2(this.innerTextPadding + (this.portrait == null || this.PortraitSide(this.portrait) >= 0 ? 0.0f : 256f), this.innerTextPadding);
      Vector2 vector2_3 = new Vector2(this.portrait == null ? this.maxLineWidthNoPortrait : this.maxLineWidth, (float) this.linesPerPage * this.lineHeight * num1) / 2f;
      float num4 = num3 >= 4 ? 0.75f : 1f;
      this.text.Draw(vector2_1 + vector2_2 + vector2_3, new Vector2(0.5f, 0.5f), new Vector2(1f, num1) * num4, num1, this.Start, int.MaxValue);
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

