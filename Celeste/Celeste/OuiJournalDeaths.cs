// Decompiled with JetBrains decompiler
// Type: Celeste.OuiJournalDeaths
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

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
      this.table = new OuiJournalPage.Table().AddColumn((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_deaths", (Language) null), new Vector2(1f, 0.5f), 0.7f, this.TextColor, 300f, false));
      for (int index = 0; index < SaveData.Instance.UnlockedModes; ++index)
        this.table.AddColumn((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_mode_" + (object) (AreaMode) index, (Language) null), this.TextJustify, 0.6f, this.TextColor, 240f, false));
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
        if (!areaData.Interlude)
        {
          if (areaData.ID > SaveData.Instance.UnlockedAreas)
          {
            flagArray[0] = flagArray[1] = flagArray[2] = false;
            break;
          }
          OuiJournalPage.Row row = this.table.AddRow();
          row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean(areaData.Name, (Language) null), new Vector2(1f, 0.5f), 0.6f, this.TextColor, 0.0f, false));
          for (int index = 0; index < SaveData.Instance.UnlockedModes; ++index)
          {
            if (area.Modes[index].SingleRunCompleted)
            {
              int deaths = area.Modes[index].BestDeaths;
              if (deaths > 0)
              {
                foreach (EntityData goldenberry in AreaData.Areas[area.ID].Mode[index].MapData.Goldenberries)
                {
                  EntityID entityId = new EntityID(goldenberry.Level.Name, goldenberry.ID);
                  if (area.Modes[index].Strawberries.Contains(entityId))
                    deaths = 0;
                }
              }
              row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Deaths(deaths), this.TextJustify, 0.5f, this.TextColor, 0.0f, false));
              numArray[index] += deaths;
            }
            else
            {
              row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot", 0.0f));
              flagArray[index] = false;
            }
          }
        }
      }
      if (!flagArray[0] && !flagArray[1] && !flagArray[2])
        return;
      this.table.AddRow();
      OuiJournalPage.Row row1 = this.table.AddRow();
      row1.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_totals", (Language) null), new Vector2(1f, 0.5f), 0.7f, this.TextColor, 0.0f, false));
      for (int index = 0; index < SaveData.Instance.UnlockedModes; ++index)
        row1.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Deaths(numArray[index]), this.TextJustify, 0.6f, this.TextColor, 0.0f, false));
      if (!flagArray[0] || !flagArray[1] || !flagArray[2])
        return;
      OuiJournalPage.TextCell textCell = new OuiJournalPage.TextCell(Dialog.Deaths(numArray[0] + numArray[1] + numArray[2]), this.TextJustify, 0.6f, this.TextColor, 0.0f, false);
      textCell.SpreadOverColumns = 3;
      this.table.AddRow().Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_grandtotal", (Language) null), new Vector2(1f, 0.5f), 0.7f, this.TextColor, 0.0f, false)).Add((OuiJournalPage.Cell) textCell);
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
