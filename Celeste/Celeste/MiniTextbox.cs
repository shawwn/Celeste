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
    public const float TextScale = 0.75f;
    public const float BoxWidth = 1688f;
    public const float BoxHeight = 144f;
    public const float HudElementHeight = 180f;
    private int index;
    private FancyText.Text text;
    private MTexture box;
    private float ease;
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
      MiniTextbox miniTextbox1 = this;
      List<Entity> entities = miniTextbox1.Scene.Tracker.GetEntities<MiniTextbox>();
      foreach (MiniTextbox miniTextbox2 in entities)
      {
        if (miniTextbox2 != miniTextbox1)
          miniTextbox2.Add((Component) new Coroutine(miniTextbox2.Close(), true));
      }
      if (entities.Count > 0)
        yield return (object) 0.3f;
      while ((double) (miniTextbox1.ease += Engine.DeltaTime * 4f) < 1.0)
        yield return (object) null;
      miniTextbox1.ease = 1f;
      if (miniTextbox1.portrait != null)
      {
        string beginAnim = "begin_" + miniTextbox1.portraitData.Animation;
        if (miniTextbox1.portrait.Has(beginAnim))
        {
          miniTextbox1.portrait.Play(beginAnim, false, false);
          while (miniTextbox1.portrait.CurrentAnimationID == beginAnim && miniTextbox1.portrait.Animating)
            yield return (object) null;
        }
        miniTextbox1.portrait.Play("talk_" + miniTextbox1.portraitData.Animation, false, false);
        miniTextbox1.talkerSfx = new SoundSource().Play(miniTextbox1.portraitData.SfxEvent, (string) null, 0.0f);
        miniTextbox1.talkerSfx.Param("dialogue_portrait", (float) miniTextbox1.portraitData.SfxExpression);
        miniTextbox1.talkerSfx.Param("dialogue_end", 0.0f);
        miniTextbox1.Add((Component) miniTextbox1.talkerSfx);
        beginAnim = (string) null;
      }
      float num = 0.0f;
      while (miniTextbox1.index < miniTextbox1.text.Nodes.Count)
      {
        if (miniTextbox1.text.Nodes[miniTextbox1.index] is FancyText.Char)
          num += (miniTextbox1.text.Nodes[miniTextbox1.index] as FancyText.Char).Delay;
        ++miniTextbox1.index;
        if ((double) num > 0.0160000007599592)
        {
          yield return (object) num;
          num = 0.0f;
        }
      }
      if (miniTextbox1.portrait != null)
        miniTextbox1.portrait.Play("idle_" + miniTextbox1.portraitData.Animation, false, false);
      if (miniTextbox1.talkerSfx != null)
      {
        miniTextbox1.talkerSfx.Param("dialogue_portrait", 0.0f);
        miniTextbox1.talkerSfx.Param("dialogue_end", 1f);
      }
      Audio.EndSnapshot(Level.DialogSnapshot);
      yield return (object) 3f;
      yield return (object) miniTextbox1.Close();
    }

    private IEnumerator Close()
    {
      MiniTextbox miniTextbox = this;
      if (!miniTextbox.closing)
      {
        miniTextbox.closing = true;
        while ((double) (miniTextbox.ease -= Engine.DeltaTime * 4f) > 0.0)
          yield return (object) null;
        miniTextbox.ease = 0.0f;
        miniTextbox.RemoveSelf();
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
      Vector2 position;
      ((Vector2) ref position).\u002Ector((float) (Engine.Width / 2), (float) (72.0 + ((double) Engine.Width - 1688.0) / 4.0));
      Vector2 vector2 = Vector2.op_Addition(position, new Vector2(-828f, -56f));
      this.box.DrawCentered(position, Color.get_White(), new Vector2(1f, this.ease));
      if (this.portrait != null)
      {
        this.portrait.Scale = Vector2.op_Multiply(new Vector2(1f, this.ease), this.portraitScale);
        this.portrait.RenderPosition = Vector2.op_Addition(vector2, new Vector2(this.portraitSize / 2f, this.portraitSize / 2f));
        this.portrait.Render();
      }
      this.text.Draw(new Vector2((float) (vector2.X + (double) this.portraitSize + 32.0), (float) position.Y), new Vector2(0.0f, 0.5f), Vector2.op_Multiply(new Vector2(1f, this.ease), 0.75f), 1f, 0, this.index);
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
