// Decompiled with JetBrains decompiler
// Type: Celeste.OuiJournalDeaths
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class OuiJournalDeaths : OuiJournalPage
  {
    private OuiJournalPage.Table table;

    public OuiJournalDeaths(OuiJournal journal)
      : base(journal)
    {
      this.PageTexture = "page";
      this.table = new OuiJournalPage.Table().AddColumn((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_deaths"), new Vector2(1f, 0.5f), 0.7f, this.TextColor, 300f));
      for (int index = 0; index < SaveData.Instance.UnlockedModes; ++index)
        this.table.AddColumn((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_mode_" + (object) (AreaMode) index), this.TextJustify, 0.6f, this.TextColor, 240f));
      bool[] flagArray = new bool[3]
      {
        true,
        SaveData.Instance.UnlockedModes >= 2,
        SaveData.Instance.UnlockedModes >= 3
      };
      int[] numArray = new int[3];
      foreach (AreaStats area in SaveData.Instance.Areas)
      {
        AreaData areaData = AreaData.Get(area.ID);
        if (!areaData.Interlude && !areaData.IsFinal)
        {
          if (areaData.ID > SaveData.Instance.UnlockedAreas)
          {
            flagArray[0] = flagArray[1] = flagArray[2] = false;
            break;
          }
          OuiJournalPage.Row row = this.table.AddRow();
          row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean(areaData.Name), new Vector2(1f, 0.5f), 0.6f, this.TextColor));
          for (int mode = 0; mode < SaveData.Instance.UnlockedModes; ++mode)
          {
            if (areaData.HasMode((AreaMode) mode))
            {
              if (area.Modes[mode].SingleRunCompleted)
              {
                int deaths = area.Modes[mode].BestDeaths;
                if (deaths > 0)
                {
                  foreach (EntityData goldenberry in AreaData.Areas[area.ID].Mode[mode].MapData.Goldenberries)
                  {
                    EntityID entityId = new EntityID(goldenberry.Level.Name, goldenberry.ID);
                    if (area.Modes[mode].Strawberries.Contains(entityId))
                      deaths = 0;
                  }
                }
                row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Deaths(deaths), this.TextJustify, 0.5f, this.TextColor));
                numArray[mode] += deaths;
              }
              else
              {
                row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot"));
                flagArray[mode] = false;
              }
            }
            else
              row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell("-", this.TextJustify, 0.5f, this.TextColor));
          }
        }
      }
      if (flagArray[0] || flagArray[1] || flagArray[2])
      {
        this.table.AddRow();
        OuiJournalPage.Row row = this.table.AddRow();
        row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_totals"), new Vector2(1f, 0.5f), 0.7f, this.TextColor));
        for (int index = 0; index < SaveData.Instance.UnlockedModes; ++index)
          row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Deaths(numArray[index]), this.TextJustify, 0.6f, this.TextColor));
        this.table.AddRow();
      }
      int num = 0;
      foreach (AreaStats area in SaveData.Instance.Areas)
      {
        AreaData areaData = AreaData.Get(area.ID);
        if (areaData.IsFinal)
        {
          if (areaData.ID <= SaveData.Instance.UnlockedAreas)
          {
            OuiJournalPage.Row row = this.table.AddRow();
            row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean(areaData.Name), new Vector2(1f, 0.5f), 0.6f, this.TextColor));
            if (area.Modes[0].SingleRunCompleted)
            {
              int deaths = area.Modes[0].BestDeaths;
              if (deaths > 0)
              {
                foreach (EntityData goldenberry in AreaData.Areas[area.ID].Mode[0].MapData.Goldenberries)
                {
                  EntityID entityId = new EntityID(goldenberry.Level.Name, goldenberry.ID);
                  if (area.Modes[0].Strawberries.Contains(entityId))
                    deaths = 0;
                }
              }
              OuiJournalPage.TextCell entry = new OuiJournalPage.TextCell(Dialog.Deaths(deaths), this.TextJustify, 0.5f, this.TextColor);
              row.Add((OuiJournalPage.Cell) entry);
              num += deaths;
            }
            else
              row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot"));
            this.table.AddRow();
          }
          else
            break;
        }
      }
      if (!flagArray[0] || !flagArray[1] || !flagArray[2])
        return;
      OuiJournalPage.TextCell entry1 = new OuiJournalPage.TextCell(Dialog.Deaths(numArray[0] + numArray[1] + numArray[2] + num), this.TextJustify, 0.6f, this.TextColor);
      entry1.SpreadOverColumns = 3;
      this.table.AddRow().Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_grandtotal"), new Vector2(1f, 0.5f), 0.7f, this.TextColor)).Add((OuiJournalPage.Cell) entry1);
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
