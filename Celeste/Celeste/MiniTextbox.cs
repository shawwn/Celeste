// Decompiled with JetBrains decompiler
// Type: Celeste.MiniTextbox
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
  public class MiniTextbox : Entity
  {
    private int index = 0;
    private float ease = 0.0f;
    public const float TextScale = 0.75f;
    public const float BoxWidth = 1688f;
    public const float BoxHeight = 144f;
    public const float HudElementHeight = 180f;
    private FancyText.Text text;
    private MTexture box;
    private bool closing;
    private Coroutine routine;
    private Sprite portrait;
    private FancyText.Portrait portraitData;
    private float portraitSize;
    private float portraitScale;
    private SoundSource talkerSfx;

    public static bool Displayed
    {
      get
      {
        foreach (MiniTextbox entity in Engine.Scene.Tracker.GetEntities<MiniTextbox>())
        {
          if (!entity.closing && (double) entity.ease > 0.25)
            return true;
        }
        return false;
      }
    }

    public MiniTextbox(string dialogId)
    {
      this.Tag = (int) Tags.HUD | (int) Tags.TransitionUpdate;
      this.portraitSize = 112f;
      this.box = GFX.Portraits["textbox/default_mini"];
      this.text = FancyText.Parse(Dialog.Get(dialogId.Trim(), (Language) null), (int) (1688.0 - (double) this.portraitSize - 32.0), 2, 1f, new Color?(), (Language) null);
      foreach (FancyText.Node node in this.text.Nodes)
      {
        if (node is FancyText.Portrait)
        {
          FancyText.Portrait portrait = node as FancyText.Portrait;
          this.portraitData = portrait;
          this.portrait = GFX.PortraitsSpriteBank.Create("portrait_" + portrait.Sprite);
          XmlElement xml = GFX.PortraitsSpriteBank.SpriteData["portrait_" + portrait.Sprite].Sources[0].XML;
          this.portraitScale = this.portraitSize / xml.AttrFloat("size", 160f);
          string id = "textbox/" + xml.Attr("textbox", "default") + "_mini";
          if (GFX.Portraits.Has(id))
            this.box = GFX.Portraits[id];
          this.Add((Component) this.portrait);
        }
      }
      this.Add((Component) (this.routine = new Coroutine(this.Routine(), true)));
      this.routine.UseRawDeltaTime = true;
      this.Add((Component) new TransitionListener()
      {
        OnOutBegin = (Action) (() =>
        {
          if (this.closing)
            return;
          this.routine.Replace(this.Close());
        })
      });
      if ((HandleBase) Level.DialogSnapshot == (HandleBase) null)
        Level.DialogSnapshot = Audio.CreateSnapshot("snapshot:/dialogue_in_progress", false);
      Audio.ResumeSnapshot(Level.DialogSnapshot);
    }

    private IEnumerator Routine()
    {
      List<Entity> others = this.Scene.Tracker.GetEntities<MiniTextbox>();
      foreach (MiniTextbox miniTextbox in others)
      {
        MiniTextbox other = miniTextbox;
        if (other != this)
          other.Add((Component) new Coroutine(other.Close(), true));
        other = (MiniTextbox) null;
      }
      if (others.Count > 0)
        yield return (object) 0.3f;
      while ((double) (this.ease += Engine.DeltaTime * 4f) < 1.0)
        yield return (object) null;
      this.ease = 1f;
      if (this.portrait != null)
      {
        string beginAnim = "begin_" + this.portraitData.Animation;
        if (this.portrait.Has(beginAnim))
        {
          this.portrait.Play(beginAnim, false, false);
          while (this.portrait.CurrentAnimationID == beginAnim && this.portrait.Animating)
            yield return (object) null;
        }
        this.portrait.Play("talk_" + this.portraitData.Animation, false, false);
        this.talkerSfx = new SoundSource().Play(this.portraitData.SfxEvent, (string) null, 0.0f);
        this.talkerSfx.Param("dialogue_portrait", (float) this.portraitData.SfxExpression);
        this.talkerSfx.Param("dialogue_end", 0.0f);
        this.Add((Component) this.talkerSfx);
        beginAnim = (string) null;
      }
      float delay = 0.0f;
      while (this.index < this.text.Nodes.Count)
      {
        if (this.text.Nodes[this.index] is FancyText.Char)
          delay += (this.text.Nodes[this.index] as FancyText.Char).Delay;
        ++this.index;
        if ((double) delay > 0.0160000007599592)
        {
          yield return (object) delay;
          delay = 0.0f;
        }
      }
      if (this.portrait != null)
        this.portrait.Play("idle_" + this.portraitData.Animation, false, false);
      if (this.talkerSfx != null)
      {
        this.talkerSfx.Param("dialogue_portrait", 0.0f);
        this.talkerSfx.Param("dialogue_end", 1f);
      }
      Audio.EndSnapshot(Level.DialogSnapshot);
      yield return (object) 3f;
      yield return (object) this.Close();
    }

    private IEnumerator Close()
    {
      if (!this.closing)
      {
        this.closing = true;
        while ((double) (this.ease -= Engine.DeltaTime * 4f) > 0.0)
          yield return (object) null;
        this.ease = 0.0f;
        this.RemoveSelf();
      }
    }

    public override void Update()
    {
      if ((this.Scene as Level).RetryPlayerCorpse != null && !this.closing)
        this.routine.Replace(this.Close());
      base.Update();
    }

    public override void Render()
    {
      if ((double) this.ease <= 0.0)
        return;
      Level scene = this.Scene as Level;
      if (scene.FrozenOrPaused || scene.RetryPlayerCorpse != null || scene.SkippingCutscene)
        return;
      Vector2 position = new Vector2((float) (Engine.Width / 2), (float) (72.0 + ((double) Engine.Width - 1688.0) / 4.0));
      Vector2 vector2 = position + new Vector2(-828f, -56f);
      this.box.DrawCentered(position, Color.White, new Vector2(1f, this.ease));
      if (this.portrait != null)
      {
        this.portrait.Scale = new Vector2(1f, this.ease) * this.portraitScale;
        this.portrait.RenderPosition = vector2 + new Vector2(this.portraitSize / 2f, this.portraitSize / 2f);
        this.portrait.Render();
      }
      this.text.Draw(new Vector2((float) ((double) vector2.X + (double) this.portraitSize + 32.0), position.Y), new Vector2(0.0f, 0.5f), new Vector2(1f, this.ease) * 0.75f, 1f, 0, this.index);
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
  }
}

