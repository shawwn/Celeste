// Decompiled with JetBrains decompiler
// Type: Celeste.OuiJournalSpeedrun
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class OuiJournalSpeedrun : OuiJournalPage
  {
    private OuiJournalPage.Table table;

    public OuiJournalSpeedrun(OuiJournal journal)
      : base(journal)
    {
      this.PageTexture = "page";
      Vector2 justify;
      ((Vector2) ref justify).\u002Ector(0.5f, 0.5f);
      float scale = 0.5f;
      Color color = Color.op_Multiply(Color.get_Black(), 0.6f);
      this.table = new OuiJournalPage.Table().AddColumn((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_speedruns", (Language) null), new Vector2(1f, 0.5f), 0.7f, Color.op_Multiply(Color.get_Black(), 0.7f), 0.0f, false)).AddColumn((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_mode_normal", (Language) null), justify, scale + 0.1f, color, 240f, false)).AddColumn((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_mode_normal_fullclear", (Language) null), justify, scale + 0.1f, color, 240f, false));
      if (SaveData.Instance.UnlockedModes >= 2)
        this.table.AddColumn((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_mode_bside", (Language) null), justify, scale + 0.1f, color, 240f, false));
      if (SaveData.Instance.UnlockedModes >= 3)
        this.table.AddColumn((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_mode_cside", (Language) null), justify, scale + 0.1f, color, 240f, false));
      foreach (AreaStats area in SaveData.Instance.Areas)
      {
        AreaData areaData = AreaData.Get(area.ID);
        if (!areaData.Interlude)
        {
          if (areaData.ID <= SaveData.Instance.UnlockedAreas)
          {
            OuiJournalPage.Row row = this.table.AddRow().Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean(areaData.Name, (Language) null), new Vector2(1f, 0.5f), scale + 0.1f, color, 0.0f, false));
            if (area.Modes[0].BestTime > 0L)
              row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(area.Modes[0].BestTime), justify, scale, color, 0.0f, false));
            else
              row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot", 0.0f));
            if (area.Modes[0].BestFullClearTime > 0L)
              row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(area.Modes[0].BestFullClearTime), justify, scale, color, 0.0f, false));
            else
              row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot", 0.0f));
            if (SaveData.Instance.UnlockedModes >= 2)
            {
              if (area.Modes[1].BestTime > 0L)
                row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(area.Modes[1].BestTime), justify, scale, color, 0.0f, false));
              else
                row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot", 0.0f));
            }
            if (SaveData.Instance.UnlockedModes >= 3)
            {
              if (area.Modes[2].BestTime > 0L)
                row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(area.Modes[2].BestTime), justify, scale, color, 0.0f, false));
              else
                row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot", 0.0f));
            }
          }
          else
            break;
        }
      }
      bool flag1 = true;
      bool flag2 = true;
      bool flag3 = true;
      bool flag4 = true;
      long ticks1 = 0;
      long ticks2 = 0;
      long ticks3 = 0;
      long ticks4 = 0;
      foreach (AreaStats area in SaveData.Instance.Areas)
      {
        if (!AreaData.Get(area.ID).Interlude)
        {
          if (area.ID > SaveData.Instance.UnlockedAreas)
          {
            int num;
            flag4 = (num = 0) != 0;
            flag3 = num != 0;
            flag2 = num != 0;
            flag1 = num != 0;
            break;
          }
          ticks1 += area.Modes[0].BestTime;
          ticks2 += area.Modes[0].BestFullClearTime;
          ticks3 += area.Modes[1].BestTime;
          ticks4 += area.Modes[2].BestTime;
          if (area.Modes[0].BestTime <= 0L)
            flag1 = false;
          if (area.Modes[0].BestFullClearTime <= 0L)
            flag2 = false;
          if (area.Modes[1].BestTime <= 0L)
            flag3 = false;
          if (area.Modes[2].BestTime <= 0L)
            flag4 = false;
        }
      }
      if (flag1 | flag2 | flag3 | flag4)
      {
        this.table.AddRow();
        OuiJournalPage.Row row = this.table.AddRow().Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_totals", (Language) null), new Vector2(1f, 0.5f), scale + 0.2f, color, 0.0f, false));
        if (flag1)
          row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(ticks1), justify, scale + 0.1f, color, 0.0f, false));
        else
          row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot", 0.0f));
        if (flag2)
          row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(ticks2), justify, scale + 0.1f, color, 0.0f, false));
        else
          row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot", 0.0f));
        if (SaveData.Instance.UnlockedModes >= 2)
        {
          if (flag3)
            row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(ticks3), justify, scale + 0.1f, color, 0.0f, false));
          else
            row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot", 0.0f));
        }
        if (SaveData.Instance.UnlockedModes >= 3)
        {
          if (flag4)
            row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(ticks4), justify, scale + 0.1f, color, 0.0f, false));
          else
            row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot", 0.0f));
        }
      }
      if (!(flag1 & flag2 & flag3 & flag4))
        return;
      OuiJournalPage.TextCell textCell = new OuiJournalPage.TextCell(Dialog.Time(ticks1 + ticks2 + ticks3 + ticks4), justify, scale + 0.2f, color, 0.0f, false);
      textCell.SpreadOverColumns = 4;
      this.table.AddRow().Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_grandtotal", (Language) null), new Vector2(1f, 0.5f), scale + 0.3f, color, 0.0f, false)).Add((OuiJournalPage.Cell) textCell);
    }

    public override void Redraw(VirtualRenderTarget buffer)
    {
      base.Redraw(buffer);
      Draw.SpriteBatch.Begin();
      this.table.Render(new Vector2(60f, 20f));
      this.RenderStamps();
      Draw.SpriteBatch.End();
    }
  }
}
