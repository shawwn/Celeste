// Decompiled with JetBrains decompiler
// Type: Celeste.OuiJournalPage
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public abstract class OuiJournalPage
  {
    public readonly Vector2 TextJustify = new Vector2(0.5f, 0.5f);
    public readonly Color TextColor = Color.op_Multiply(Color.get_Black(), 0.6f);
    public const int PageWidth = 1610;
    public const int PageHeight = 1000;
    public const float TextScale = 0.5f;
    public int PageIndex;
    public string PageTexture;
    public OuiJournal Journal;

    public OuiJournalPage(OuiJournal journal)
    {
      this.Journal = journal;
    }

    public virtual void Redraw(VirtualRenderTarget buffer)
    {
      Engine.Graphics.get_GraphicsDevice().SetRenderTarget((RenderTarget2D) buffer);
      Engine.Graphics.get_GraphicsDevice().Clear(Color.get_Transparent());
    }

    public virtual void Update()
    {
    }

    internal void RenderStamps()
    {
      if (SaveData.Instance.AssistMode)
        GFX.Gui["fileselect/assist"].DrawCentered(new Vector2(1250f, 810f), Color.op_Multiply(Color.get_White(), 0.5f), 1f, 0.2f);
      if (!SaveData.Instance.CheatMode)
        return;
      GFX.Gui["fileselect/cheatmode"].DrawCentered(new Vector2(1400f, 860f), Color.op_Multiply(Color.get_White(), 0.5f), 1f, 0.0f);
    }

    public class Table
    {
      private List<OuiJournalPage.Row> rows = new List<OuiJournalPage.Row>();
      private const float headerHeight = 80f;
      private const float headerBottomMargin = 20f;
      private const float rowHeight = 60f;

      public int Rows
      {
        get
        {
          return this.rows.Count;
        }
      }

      public OuiJournalPage.Row Header
      {
        get
        {
          if (this.rows.Count <= 0)
            return (OuiJournalPage.Row) null;
          return this.rows[0];
        }
      }

      public OuiJournalPage.Table AddColumn(OuiJournalPage.Cell label)
      {
        if (this.rows.Count == 0)
          this.AddRow();
        this.rows[0].Add(label);
        return this;
      }

      public OuiJournalPage.Row AddRow()
      {
        OuiJournalPage.Row row = new OuiJournalPage.Row();
        this.rows.Add(row);
        return row;
      }

      public float Height()
      {
        return (float) (100.0 + 60.0 * (double) (this.rows.Count - 1));
      }

      public void Render(Vector2 position)
      {
        if (this.Header == null)
          return;
        float num1 = 0.0f;
        float num2 = 0.0f;
        for (int index = 0; index < this.Header.Count; ++index)
          num2 += this.Header[index].Width() + 20f;
        for (int index1 = 0; index1 < this.Header.Count; ++index1)
        {
          float columnWidth = this.Header[index1].Width();
          this.Header[index1].Render(Vector2.op_Addition(position, new Vector2(num1 + columnWidth * 0.5f, 40f)), columnWidth);
          for (int index2 = 1; index2 < this.rows.Count; ++index2)
          {
            Vector2 center = Vector2.op_Addition(position, new Vector2(num1 + columnWidth * 0.5f, (float) (100.0 + ((double) index2 - 0.5) * 60.0)));
            if (index2 % 2 == 0)
              Draw.Rect((float) (center.X - (double) columnWidth * 0.5), (float) (center.Y - 27.0), columnWidth + 20f, 54f, Color.op_Multiply(Color.get_Black(), 0.08f));
            if (index1 < this.rows[index2].Count && this.rows[index2][index1] != null)
            {
              OuiJournalPage.Cell cell = this.rows[index2][index1];
              if (cell.SpreadOverColumns > 1)
              {
                for (int index3 = index1 + 1; index3 < index1 + cell.SpreadOverColumns; ++index3)
                {
                  ref __Null local = ref center.X;
                  // ISSUE: cast to a reference type
                  // ISSUE: explicit reference operation
                  // ISSUE: cast to a reference type
                  // ISSUE: explicit reference operation
                  ^(float&) ref local = ^(float&) ref local + this.Header[index3].Width() * 0.5f;
                }
                ref __Null local1 = ref center.X;
                // ISSUE: cast to a reference type
                // ISSUE: explicit reference operation
                // ISSUE: cast to a reference type
                // ISSUE: explicit reference operation
                ^(float&) ref local1 = ^(float&) ref local1 + (float) ((double) (cell.SpreadOverColumns - 1) * 20.0 * 0.5);
              }
              this.rows[index2][index1].Render(center, columnWidth);
            }
          }
          num1 += columnWidth + 20f;
        }
      }
    }

    public class Row
    {
      public List<OuiJournalPage.Cell> Entries = new List<OuiJournalPage.Cell>();

      public OuiJournalPage.Row Add(OuiJournalPage.Cell entry)
      {
        this.Entries.Add(entry);
        return this;
      }

      public int Count
      {
        get
        {
          return this.Entries.Count;
        }
      }

      public OuiJournalPage.Cell this[int index]
      {
        get
        {
          return this.Entries[index];
        }
      }
    }

    public abstract class Cell
    {
      public int SpreadOverColumns = 1;

      public virtual float Width()
      {
        return 0.0f;
      }

      public virtual void Render(Vector2 center, float columnWidth)
      {
      }
    }

    public class EmptyCell : OuiJournalPage.Cell
    {
      private float width;

      public EmptyCell(float width)
      {
        this.width = width;
      }

      public override float Width()
      {
        return this.width;
      }
    }

    public class TextCell : OuiJournalPage.Cell
    {
      private string text;
      private Vector2 justify;
      private float scale;
      private Color color;
      private float width;
      private bool forceWidth;

      public TextCell(
        string text,
        Vector2 justify,
        float scale,
        Color color,
        float width = 0.0f,
        bool forceWidth = false)
      {
        this.text = text;
        this.justify = justify;
        this.scale = scale;
        this.color = color;
        this.width = width;
        this.forceWidth = forceWidth;
      }

      public override float Width()
      {
        if (this.forceWidth)
          return this.width;
        return Math.Max(this.width, (float) ActiveFont.Measure(this.text).X * this.scale);
      }

      public override void Render(Vector2 center, float columnWidth)
      {
        float num1 = (float) ActiveFont.Measure(this.text).X * this.scale;
        float num2 = 1f;
        if (!this.forceWidth && (double) num1 > (double) columnWidth)
          num2 = columnWidth / num1;
        ActiveFont.Draw(this.text, Vector2.op_Addition(center, new Vector2((float) (-(double) columnWidth / 2.0 + (double) columnWidth * this.justify.X), 0.0f)), this.justify, Vector2.op_Multiply(Vector2.op_Multiply(Vector2.get_One(), this.scale), num2), this.color);
      }
    }

    public class IconCell : OuiJournalPage.Cell
    {
      private string icon;
      private float width;

      public IconCell(string icon, float width = 0.0f)
      {
        this.icon = icon;
        this.width = width;
      }

      public override float Width()
      {
        return Math.Max((float) GFX.Journal[this.icon].Width, this.width);
      }

      public override void Render(Vector2 center, float columnWidth)
      {
        GFX.Journal[this.icon].DrawCentered(center);
      }
    }

    public class IconsCell : OuiJournalPage.Cell
    {
      private float iconSpacing = 4f;
      private string[] icons;

      public IconsCell(float iconSpacing, params string[] icons)
      {
        this.iconSpacing = iconSpacing;
        this.icons = icons;
      }

      public IconsCell(params string[] icons)
      {
        this.icons = icons;
      }

      public override float Width()
      {
        float num = 0.0f;
        for (int index = 0; index < this.icons.Length; ++index)
          num += (float) GFX.Journal[this.icons[index]].Width;
        return num + (float) (this.icons.Length - 1) * this.iconSpacing;
      }

      public override void Render(Vector2 center, float columnWidth)
      {
        float num = this.Width();
        Vector2 position = Vector2.op_Addition(center, new Vector2((float) (-(double) num * 0.5), 0.0f));
        for (int index = 0; index < this.icons.Length; ++index)
        {
          MTexture mtexture = GFX.Journal[this.icons[index]];
          mtexture.DrawJustified(position, new Vector2(0.0f, 0.5f));
          ref __Null local = ref position.X;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local + ((float) mtexture.Width + this.iconSpacing);
        }
      }
    }
  }
}
