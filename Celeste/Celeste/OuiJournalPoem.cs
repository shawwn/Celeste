// Decompiled with JetBrains decompiler
// Type: Celeste.OuiJournalPoem
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class OuiJournalPoem : OuiJournalPage
  {
    private List<OuiJournalPoem.PoemLine> lines = new List<OuiJournalPoem.PoemLine>();
    private Coroutine swapRoutine = new Coroutine(true);
    private Wiggler wiggler = Wiggler.Create(0.4f, 4f, (Action<float>) null, false, false);
    private const float textScale = 0.5f;
    private const float holdingScaleAdd = 0.1f;
    private const float poemHeight = 44f;
    private const float poemSpacing = 4f;
    private const float poemStanzaSpacing = 16f;
    private int index;
    private float slider;
    private bool dragging;
    private bool swapping;
    private Tween tween;

    public OuiJournalPoem(OuiJournal journal)
      : base(journal)
    {
      this.PageTexture = "page";
      this.swapRoutine.RemoveOnComplete = false;
      float num = 0.0f;
      foreach (string id in SaveData.Instance.Poem)
      {
        this.lines.Add(new OuiJournalPoem.PoemLine()
        {
          Text = Dialog.Clean("poem_" + id, (Language) null),
          Index = num,
          Remix = AreaData.IsPoemRemix(id)
        });
        ++num;
      }
    }

    public static float GetY(float index)
    {
      return (float) (120.0 + 44.0 * ((double) index + 0.5) + 4.0 * (double) index + (double) ((int) index / 4) * 16.0);
    }

    public override void Redraw(VirtualRenderTarget buffer)
    {
      base.Redraw(buffer);
      Draw.SpriteBatch.Begin();
      ActiveFont.Draw(Dialog.Clean("journal_poem", (Language) null), new Vector2(60f, 60f), new Vector2(0.0f, 0.5f), Vector2.get_One(), Color.op_Multiply(Color.get_Black(), 0.6f));
      foreach (OuiJournalPoem.PoemLine line in this.lines)
        line.Render();
      if (this.lines.Count > 0)
      {
        MTexture mtexture = GFX.Journal[this.dragging ? "poemSlider" : "poemArrow"];
        Vector2 vector2;
        ((Vector2) ref vector2).\u002Ector(50f, OuiJournalPoem.GetY(this.slider));
        Vector2 position = vector2;
        Color white = Color.get_White();
        double num = 1.0 + 0.25 * (double) this.wiggler.Value;
        mtexture.DrawCentered(position, white, (float) num);
      }
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
        Draw.Rect(x, y - 22f, 810f, 44f, Color.op_Multiply(Color.get_White(), 0.25f));
        Vector2 scale1 = Vector2.op_Multiply(Vector2.get_One(), (float) (0.600000023841858 + (double) this.HoldingEase * 0.400000005960464));
        GFX.Journal[this.Remix ? "heartgem1" : "heartgem0"].DrawCentered(new Vector2(x + 20f, y), Color.get_White(), scale1);
        Color color = Color.op_Multiply(Color.get_Black(), (float) (0.699999988079071 + (double) this.HoveringEase * 0.300000011920929));
        Vector2 scale2 = Vector2.op_Multiply(Vector2.get_One(), (float) (0.5 + (double) this.HoldingEase * 0.100000001490116));
        ActiveFont.Draw(this.Text, new Vector2(x + 60f, y), new Vector2(0.0f, 0.5f), scale2, color);
      }
    }
  }
}
