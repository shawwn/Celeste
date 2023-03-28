// Decompiled with JetBrains decompiler
// Type: Celeste.OuiJournalPoem
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class OuiJournalPoem : OuiJournalPage
  {
    private const float textScale = 0.5f;
    private const float holdingScaleAdd = 0.1f;
    private const float poemHeight = 44f;
    private const float poemSpacing = 4f;
    private const float poemStanzaSpacing = 16f;
    private List<OuiJournalPoem.PoemLine> lines = new List<OuiJournalPoem.PoemLine>();
    private int index;
    private float slider;
    private bool dragging;
    private bool swapping;
    private Coroutine swapRoutine = new Coroutine();
    private Wiggler wiggler = Wiggler.Create(0.4f, 4f);
    private Tween tween;

    public OuiJournalPoem(OuiJournal journal)
      : base(journal)
    {
      this.PageTexture = "page";
      this.swapRoutine.RemoveOnComplete = false;
      float num = 0.0f;
      foreach (string id in SaveData.Instance.Poem)
      {
        string str = Dialog.Clean("poem_" + id).Replace("\n", " - ");
        this.lines.Add(new OuiJournalPoem.PoemLine()
        {
          Text = str,
          Index = num,
          Remix = AreaData.IsPoemRemix(id)
        });
        ++num;
      }
    }

    public static float GetY(float index) => (float) (120.0 + 44.0 * ((double) index + 0.5) + 4.0 * (double) index + (double) ((int) index / 4) * 16.0);

    public override void Redraw(VirtualRenderTarget buffer)
    {
      base.Redraw(buffer);
      Draw.SpriteBatch.Begin();
      ActiveFont.Draw(Dialog.Clean("journal_poem"), new Vector2(60f, 60f), new Vector2(0.0f, 0.5f), Vector2.One, Color.Black * 0.6f);
      foreach (OuiJournalPoem.PoemLine line in this.lines)
        line.Render();
      if (this.lines.Count > 0)
        MTN.Journal[this.dragging ? "poemSlider" : "poemArrow"].DrawCentered(new Vector2(50f, OuiJournalPoem.GetY(this.slider)), Color.White, (float) (1.0 + 0.25 * (double) this.wiggler.Value));
      Draw.SpriteBatch.End();
    }

    private IEnumerator Swap(int a, int b)
    {
      string str = SaveData.Instance.Poem[a];
      SaveData.Instance.Poem[a] = SaveData.Instance.Poem[b];
      SaveData.Instance.Poem[b] = str;
      OuiJournalPoem.PoemLine poemA = this.lines[a];
      OuiJournalPoem.PoemLine poemB = this.lines[b];
      OuiJournalPoem.PoemLine line = this.lines[a];
      this.lines[a] = this.lines[b];
      this.lines[b] = line;
      this.tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, 0.125f, true);
      this.tween.OnUpdate = (Action<Tween>) (t =>
      {
        poemA.Index = MathHelper.Lerp((float) a, (float) b, t.Eased);
        poemB.Index = MathHelper.Lerp((float) b, (float) a, t.Eased);
      });
      this.tween.OnComplete = (Action<Tween>) (t => this.tween = (Tween) null);
      yield return (object) this.tween.Wait();
      this.swapping = false;
    }

    public override void Update()
    {
      if (this.lines.Count <= 0)
        return;
      if (this.tween != null && this.tween.Active)
        this.tween.Update();
      if (Input.MenuDown.Pressed && this.index + 1 < this.lines.Count && !this.swapping)
      {
        if (this.dragging)
        {
          Audio.Play("event:/ui/world_map/journal/heart_shift_down");
          this.swapRoutine.Replace(this.Swap(this.index, this.index + 1));
          this.swapping = true;
        }
        else
          Audio.Play("event:/ui/world_map/journal/heart_roll");
        ++this.index;
      }
      else if (Input.MenuUp.Pressed && this.index > 0 && !this.swapping)
      {
        if (this.dragging)
        {
          Audio.Play("event:/ui/world_map/journal/heart_shift_up");
          this.swapRoutine.Replace(this.Swap(this.index, this.index - 1));
          this.swapping = true;
        }
        else
          Audio.Play("event:/ui/world_map/journal/heart_roll");
        --this.index;
      }
      else if (Input.MenuConfirm.Pressed)
      {
        this.Journal.PageTurningLocked = true;
        Audio.Play("event:/ui/world_map/journal/heart_grab");
        this.dragging = true;
        this.wiggler.Start();
      }
      else if (!Input.MenuConfirm.Check && this.dragging)
      {
        this.Journal.PageTurningLocked = false;
        Audio.Play("event:/ui/world_map/journal/heart_release");
        this.dragging = false;
        this.wiggler.Start();
      }
      for (int index = 0; index < this.lines.Count; ++index)
      {
        OuiJournalPoem.PoemLine line = this.lines[index];
        line.HoveringEase = Calc.Approach(line.HoveringEase, this.index == index ? 1f : 0.0f, 8f * Engine.DeltaTime);
        line.HoldingEase = Calc.Approach(line.HoldingEase, this.index != index || !this.dragging ? 0.0f : 1f, 8f * Engine.DeltaTime);
      }
      this.slider = Calc.Approach(this.slider, (float) this.index, 16f * Engine.DeltaTime);
      if (this.swapRoutine != null && this.swapRoutine.Active)
        this.swapRoutine.Update();
      this.wiggler.Update();
      this.Redraw(this.Journal.CurrentPageBuffer);
    }

    private class PoemLine
    {
      public float Index;
      public string Text;
      public float HoveringEase;
      public float HoldingEase;
      public bool Remix;

      public void Render()
      {
        float x = (float) (100.0 + (double) Ease.CubeInOut(this.HoveringEase) * 20.0);
        float y = OuiJournalPoem.GetY(this.Index);
        Draw.Rect(x, y - 22f, 810f, 44f, Color.White * 0.25f);
        Vector2 scale1 = Vector2.One * (float) (0.6000000238418579 + (double) this.HoldingEase * 0.4000000059604645);
        MTN.Journal[this.Remix ? "heartgem1" : "heartgem0"].DrawCentered(new Vector2(x + 20f, y), Color.White, scale1);
        Color color = Color.Black * (float) (0.699999988079071 + (double) this.HoveringEase * 0.30000001192092896);
        Vector2 scale2 = Vector2.One * (float) (0.5 + (double) this.HoldingEase * 0.10000000149011612);
        ActiveFont.Draw(this.Text, new Vector2(x + 60f, y), new Vector2(0.0f, 0.5f), scale2, color);
      }
    }
  }
}
