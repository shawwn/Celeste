// Decompiled with JetBrains decompiler
// Type: Celeste.OuiJournalSpeedrun
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

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
      Vector2 justify = new Vector2(0.5f, 0.5f);
      float scale = 0.5f;
      Color color = Color.Black * 0.6f;
      this.table = new OuiJournalPage.Table().AddColumn((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_speedruns"), new Vector2(1f, 0.5f), 0.7f, Color.Black * 0.7f)).AddColumn((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_mode_normal"), justify, scale + 0.1f, color, 240f)).AddColumn((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_mode_normal_fullclear"), justify, scale + 0.1f, color, 240f));
      if (SaveData.Instance.UnlockedModes >= 2)
        this.table.AddColumn((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_mode_bside"), justify, scale + 0.1f, color, 240f));
      if (SaveData.Instance.UnlockedModes >= 3)
        this.table.AddColumn((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_mode_cside"), justify, scale + 0.1f, color, 240f));
      foreach (AreaStats area in SaveData.Instance.Areas)
      {
        AreaData areaData = AreaData.Get(area.ID);
        if (!areaData.Interlude && !areaData.IsFinal)
        {
          if (areaData.ID <= SaveData.Instance.UnlockedAreas)
          {
            OuiJournalPage.Row row = this.table.AddRow().Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean(areaData.Name), new Vector2(1f, 0.5f), scale + 0.1f, color));
            if (area.Modes[0].BestTime > 0L)
              row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(area.Modes[0].BestTime), justify, scale, color));
            else
              row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot"));
            if (areaData.CanFullClear)
            {
              if (area.Modes[0].BestFullClearTime > 0L)
                row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(area.Modes[0].BestFullClearTime), justify, scale, color));
              else
                row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot"));
            }
            else
              row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell("-", this.TextJustify, 0.5f, this.TextColor));
            if (SaveData.Instance.UnlockedModes >= 2)
            {
              if (areaData.HasMode(AreaMode.BSide))
              {
                if (area.Modes[1].BestTime > 0L)
                  row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(area.Modes[1].BestTime), justify, scale, color));
                else
                  row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot"));
              }
              else
                row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell("-", this.TextJustify, 0.5f, this.TextColor));
            }
            if (SaveData.Instance.UnlockedModes >= 3)
            {
              if (areaData.HasMode(AreaMode.CSide))
              {
                if (area.Modes[2].BestTime > 0L)
                  row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(area.Modes[2].BestTime), justify, scale, color));
                else
                  row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot"));
              }
              else
                row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell("-", this.TextJustify, 0.5f, this.TextColor));
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
        AreaData areaData = AreaData.Get(area.ID);
        if (!areaData.Interlude && !areaData.IsFinal)
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
          if (areaData.CanFullClear && area.Modes[0].BestFullClearTime <= 0L)
            flag2 = false;
          if (areaData.HasMode(AreaMode.BSide) && area.Modes[1].BestTime <= 0L)
            flag3 = false;
          if (areaData.HasMode(AreaMode.CSide) && area.Modes[2].BestTime <= 0L)
            flag4 = false;
        }
      }
      if (flag1 | flag2 | flag3 | flag4)
      {
        this.table.AddRow();
        OuiJournalPage.Row row = this.table.AddRow().Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_totals"), new Vector2(1f, 0.5f), scale + 0.2f, color));
        if (flag1)
          row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(ticks1), justify, scale + 0.1f, color));
        else
          row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot"));
        if (flag2)
          row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(ticks2), justify, scale + 0.1f, color));
        else
          row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot"));
        if (SaveData.Instance.UnlockedModes >= 2)
        {
          if (flag3)
            row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(ticks3), justify, scale + 0.1f, color));
          else
            row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot"));
        }
        if (SaveData.Instance.UnlockedModes >= 3)
        {
          if (flag4)
            row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(ticks4), justify, scale + 0.1f, color));
          else
            row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot"));
        }
        this.table.AddRow();
      }
      long num1 = 0;
      foreach (AreaStats area in SaveData.Instance.Areas)
      {
        AreaData areaData = AreaData.Get(area.ID);
        if (areaData.IsFinal)
        {
          if (areaData.ID <= SaveData.Instance.UnlockedAreas)
          {
            num1 += area.Modes[0].BestTime;
            OuiJournalPage.Row row = this.table.AddRow().Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean(areaData.Name), new Vector2(1f, 0.5f), scale + 0.1f, color));
            row.Add((OuiJournalPage.Cell) null);
            OuiJournalPage.Cell cell;
            if (area.Modes[0].BestTime > 0L)
              row.Add(cell = (OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(area.Modes[0].BestTime), justify, scale, color));
            else
              row.Add(cell = (OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot"));
            this.table.AddRow();
          }
          else
            break;
        }
      }
      if (!(flag1 & flag2 & flag3 & flag4))
        return;
      OuiJournalPage.TextCell entry = new OuiJournalPage.TextCell(Dialog.Time(ticks1 + ticks2 + ticks3 + ticks4 + num1), justify, scale + 0.2f, color);
      entry.SpreadOverColumns = 1 + SaveData.Instance.UnlockedModes;
      this.table.AddRow().Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_grandtotal"), new Vector2(1f, 0.5f), scale + 0.3f, color)).Add((OuiJournalPage.Cell) entry);
    }

    public override void Redraw(VirtualRenderTarget buffer)
    {
      base.Redraw(buffer);
      Draw.SpriteBatch.Begin();
      this.table.Render(new Vector2(60f, 20f));
      Draw.SpriteBatch.End();
    }
  }
}
