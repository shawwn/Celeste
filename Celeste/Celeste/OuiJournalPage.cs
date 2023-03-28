// Decompiled with JetBrains decompiler
// Type: Celeste.OuiJournalPage
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public abstract class OuiJournalPage
  {
    public const int PageWidth = 1610;
    public const int PageHeight = 1000;
    public readonly Vector2 TextJustify = new Vector2(0.5f, 0.5f);
    public const float TextScale = 0.5f;
    public readonly Color TextColor = Color.Black * 0.6f;
    public int PageIndex;
    public string PageTexture;
    public OuiJournal Journal;

    public OuiJournalPage(OuiJournal journal) => this.Journal = journal;

    public virtual void Redraw(VirtualRenderTarget buffer)
    {
      Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) buffer);
      Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
    }

    public virtual void Update()
    {
    }

    public class Table
    {
      private const float headerHeight = 80f;
      private const float headerBottomMargin = 20f;
      private const float rowHeight = 60f;
      private List<OuiJournalPage.Row> rows = new List<OuiJournalPage.Row>();

      public int Rows => this.rows.Count;

      public OuiJournalPage.Row Header => this.rows.Count <= 0 ? (OuiJournalPage.Row) null : this.rows[0];

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

      public float Height() => (float) (100.0 + 60.0 * (double) (this.rows.Count - 1));

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
          this.Header[index1].Render(position + new Vector2(num1 + columnWidth * 0.5f, 40f), columnWidth);
          int num3 = 1;
          float y = 130f;
          for (int index2 = 1; index2 < this.rows.Count; ++index2)
          {
            Vector2 center = position + new Vector2(num1 + columnWidth * 0.5f, y);
            if (this.rows[index2].Count > 0)
            {
              if (num3 % 2 == 0)
                Draw.Rect(center.X - columnWidth * 0.5f, center.Y - 27f, columnWidth + 20f, 54f, Color.Black * 0.08f);
              if (index1 < this.rows[index2].Count && this.rows[index2][index1] != null)
              {
                OuiJournalPage.Cell cell = this.rows[index2][index1];
                if (cell.SpreadOverColumns > 1)
                {
                  for (int index3 = index1 + 1; index3 < index1 + cell.SpreadOverColumns; ++index3)
                    center.X += this.Header[index3].Width() * 0.5f;
                  center.X += (float) ((double) (cell.SpreadOverColumns - 1) * 20.0 * 0.5);
                }
                this.rows[index2][index1].Render(center, columnWidth);
              }
              ++num3;
              y += 60f;
            }
            else
            {
              Draw.Rect(center.X - columnWidth * 0.5f, center.Y - 25.5f, columnWidth + 20f, 6f, Color.Black * 0.3f);
              y += 15f;
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

      public int Count => this.Entries.Count;

      public OuiJournalPage.Cell this[int index] => this.Entries[index];
    }

    public abstract class Cell
    {
      public int SpreadOverColumns = 1;

      public virtual float Width() => 0.0f;

      public virtual void Render(Vector2 center, float columnWidth)
      {
      }
    }

    public class EmptyCell : OuiJournalPage.Cell
    {
      private float width;

      public EmptyCell(float width) => this.width = width;

      public override float Width() => this.width;
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

      public override float Width() => this.forceWidth ? this.width : Math.Max(this.width, ActiveFont.Measure(this.text).X * this.scale);

      public override void Render(Vector2 center, float columnWidth)
      {
        float num1 = ActiveFont.Measure(this.text).X * this.scale;
        float num2 = 1f;
        if (!this.forceWidth && (double) num1 > (double) columnWidth)
          num2 = columnWidth / num1;
        ActiveFont.Draw(this.text, center + new Vector2((float) (-(double) columnWidth / 2.0 + (double) columnWidth * (double) this.justify.X), 0.0f), this.justify, Vector2.One * this.scale * num2, this.color);
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

      public override float Width() => Math.Max((float) MTN.Journal[this.icon].Width, this.width);

      public override void Render(Vector2 center, float columnWidth) => MTN.Journal[this.icon].DrawCentered(center);
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

      public IconsCell(params string[] icons) => this.icons = icons;

      public override float Width()
      {
        float num = 0.0f;
        for (int index = 0; index < this.icons.Length; ++index)
          num += (float) MTN.Journal[this.icons[index]].Width;
        return num + (float) (this.icons.Length - 1) * this.iconSpacing;
      }

      public override void Render(Vector2 center, float columnWidth)
      {
        float num = this.Width();
        Vector2 position = center + new Vector2((float) (-(double) num * 0.5), 0.0f);
        for (int index = 0; index < this.icons.Length; ++index)
        {
          MTexture mtexture = MTN.Journal[this.icons[index]];
          mtexture.DrawJustified(position, new Vector2(0.0f, 0.5f));
          position.X += (float) mtexture.Width + this.iconSpacing;
        }
      }
    }
  }
}
