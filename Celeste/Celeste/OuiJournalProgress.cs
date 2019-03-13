// Decompiled with JetBrains decompiler
// Type: Celeste.OuiJournalProgress
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  public class OuiJournalProgress : OuiJournalPage
  {
    private OuiJournalPage.Table table;

    public OuiJournalProgress(OuiJournal journal)
      : base(journal)
    {
      this.PageTexture = "page";
      this.table = new OuiJournalPage.Table().AddColumn((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_progress", (Language) null), new Vector2(0.0f, 0.5f), 1f, Color.Black * 0.7f, 0.0f, false)).AddColumn((OuiJournalPage.Cell) new OuiJournalPage.EmptyCell(20f)).AddColumn((OuiJournalPage.Cell) new OuiJournalPage.EmptyCell(64f)).AddColumn((OuiJournalPage.Cell) new OuiJournalPage.EmptyCell(64f)).AddColumn((OuiJournalPage.Cell) new OuiJournalPage.EmptyCell(100f)).AddColumn((OuiJournalPage.Cell) new OuiJournalPage.IconCell("strawberry", 150f)).AddColumn((OuiJournalPage.Cell) new OuiJournalPage.IconCell("skullblue", 100f));
      if (SaveData.Instance.UnlockedModes >= 2)
        this.table.AddColumn((OuiJournalPage.Cell) new OuiJournalPage.IconCell("skullred", 100f));
      if (SaveData.Instance.UnlockedModes >= 3)
        this.table.AddColumn((OuiJournalPage.Cell) new OuiJournalPage.IconCell("skullgold", 100f));
      this.table.AddColumn((OuiJournalPage.Cell) new OuiJournalPage.IconCell("time", 220f));
      foreach (AreaStats area in SaveData.Instance.Areas)
      {
        AreaData areaData = AreaData.Get(area.ID);
        if (!areaData.Interlude)
        {
          if (areaData.ID <= SaveData.Instance.UnlockedAreas)
          {
            string text;
            if (areaData.Mode[0].TotalStrawberries > 0 || area.TotalStrawberries > 0)
            {
              text = area.TotalStrawberries.ToString();
              if (area.Modes[0].Completed)
                text = text + "/" + (object) areaData.Mode[0].TotalStrawberries;
            }
            else
              text = "-";
            List<string> stringList = new List<string>();
            for (int index = 0; index < area.Modes.Length; ++index)
            {
              if (area.Modes[index].HeartGem)
                stringList.Add("heartgem" + (object) index);
            }
            if (stringList.Count <= 0)
              stringList.Add("dot");
            OuiJournalPage.Row row = this.table.AddRow().Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean(areaData.Name, (Language) null), new Vector2(1f, 0.5f), 0.6f, this.TextColor, 0.0f, false)).Add((OuiJournalPage.Cell) null).Add((OuiJournalPage.Cell) new OuiJournalPage.IconsCell(new string[1]
            {
              this.CompletionIcon(area)
            })).Add((OuiJournalPage.Cell) new OuiJournalPage.IconsCell(new string[1]
            {
              area.Cassette ? "cassette" : "dot"
            })).Add((OuiJournalPage.Cell) new OuiJournalPage.IconsCell(-32f, stringList.ToArray())).Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(text, this.TextJustify, 0.5f, this.TextColor, 0.0f, false));
            for (int index = 0; index < SaveData.Instance.UnlockedModes; ++index)
              row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Deaths(area.Modes[index].Deaths), this.TextJustify, 0.5f, this.TextColor, 0.0f, false));
            if (area.TotalTimePlayed > 0L)
              row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(area.TotalTimePlayed), this.TextJustify, 0.5f, this.TextColor, 0.0f, false));
            else
              row.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot", 0.0f));
          }
          else
            break;
        }
      }
      if (this.table.Rows <= 1)
        return;
      this.table.AddRow();
      OuiJournalPage.Row row1 = this.table.AddRow().Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_totals", (Language) null), new Vector2(1f, 0.5f), 0.7f, this.TextColor, 0.0f, false)).Add((OuiJournalPage.Cell) null).Add((OuiJournalPage.Cell) null).Add((OuiJournalPage.Cell) null).Add((OuiJournalPage.Cell) null).Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(SaveData.Instance.TotalStrawberries.ToString(), this.TextJustify, 0.6f, this.TextColor, 0.0f, false));
      OuiJournalPage.TextCell textCell = new OuiJournalPage.TextCell(Dialog.Deaths(SaveData.Instance.TotalDeaths), this.TextJustify, 0.6f, this.TextColor, 0.0f, false);
      textCell.SpreadOverColumns = SaveData.Instance.UnlockedModes;
      row1.Add((OuiJournalPage.Cell) textCell);
      for (int index = 1; index < SaveData.Instance.UnlockedModes; ++index)
        row1.Add((OuiJournalPage.Cell) null);
      row1.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(SaveData.Instance.Time), this.TextJustify, 0.6f, this.TextColor, 0.0f, false));
    }

    private string CompletionIcon(AreaStats data)
    {
      if (data.Modes[0].FullClear)
        return "fullclear";
      return data.Modes[0].Completed ? "clear" : "dot";
    }

    public override void Redraw(VirtualRenderTarget buffer)
    {
      base.Redraw(buffer);
      Draw.SpriteBatch.Begin();
      this.table.Render(new Vector2(60f, 20f));
      this.RenderStamps();
      Draw.SpriteBatch.End();
    }

    private void DrawIcon(Vector2 pos, bool obtained, string icon)
    {
      if (obtained)
        GFX.Journal[icon].DrawCentered(pos);
      else
        GFX.Journal["dot"].DrawCentered(pos);
    }
  }
}

