// Decompiled with JetBrains decompiler
// Type: Celeste.OuiJournalProgress
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

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
      this.table = new OuiJournalPage.Table().AddColumn((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_progress"), new Vector2(0.0f, 0.5f), 1f, Color.Black * 0.7f)).AddColumn((OuiJournalPage.Cell) new OuiJournalPage.EmptyCell(20f)).AddColumn((OuiJournalPage.Cell) new OuiJournalPage.EmptyCell(64f)).AddColumn((OuiJournalPage.Cell) new OuiJournalPage.EmptyCell(64f)).AddColumn((OuiJournalPage.Cell) new OuiJournalPage.EmptyCell(100f)).AddColumn((OuiJournalPage.Cell) new OuiJournalPage.IconCell("strawberry", 150f)).AddColumn((OuiJournalPage.Cell) new OuiJournalPage.IconCell("skullblue", 100f));
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
            OuiJournalPage.Row row1 = this.table.AddRow().Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean(areaData.Name), new Vector2(1f, 0.5f), 0.6f, this.TextColor)).Add((OuiJournalPage.Cell) null);
            string[] strArray = new string[1]
            {
              this.CompletionIcon(area)
            };
            OuiJournalPage.IconsCell entry1;
            OuiJournalPage.IconsCell iconsCell = entry1 = new OuiJournalPage.IconsCell(strArray);
            OuiJournalPage.Row row2 = row1.Add((OuiJournalPage.Cell) entry1);
            if (areaData.CanFullClear)
            {
              row2.Add((OuiJournalPage.Cell) new OuiJournalPage.IconsCell(new string[1]
              {
                area.Cassette ? "cassette" : "dot"
              }));
              row2.Add((OuiJournalPage.Cell) new OuiJournalPage.IconsCell(-32f, stringList.ToArray()));
            }
            else
            {
              iconsCell.SpreadOverColumns = 3;
              row2.Add((OuiJournalPage.Cell) null).Add((OuiJournalPage.Cell) null);
            }
            row2.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(text, this.TextJustify, 0.5f, this.TextColor));
            if (areaData.IsFinal)
            {
              OuiJournalPage.TextCell entry2 = new OuiJournalPage.TextCell(Dialog.Deaths(area.Modes[0].Deaths), this.TextJustify, 0.5f, this.TextColor);
              entry2.SpreadOverColumns = SaveData.Instance.UnlockedModes;
              row2.Add((OuiJournalPage.Cell) entry2);
              for (int index = 0; index < SaveData.Instance.UnlockedModes - 1; ++index)
                row2.Add((OuiJournalPage.Cell) null);
            }
            else
            {
              for (int mode = 0; mode < SaveData.Instance.UnlockedModes; ++mode)
              {
                if (areaData.HasMode((AreaMode) mode))
                  row2.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Deaths(area.Modes[mode].Deaths), this.TextJustify, 0.5f, this.TextColor));
                else
                  row2.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell("-", this.TextJustify, 0.5f, this.TextColor));
              }
            }
            if (area.TotalTimePlayed > 0L)
              row2.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(area.TotalTimePlayed), this.TextJustify, 0.5f, this.TextColor));
            else
              row2.Add((OuiJournalPage.Cell) new OuiJournalPage.IconCell("dot"));
          }
          else
            break;
        }
      }
      if (this.table.Rows <= 1)
        return;
      this.table.AddRow();
      OuiJournalPage.Row row = this.table.AddRow().Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Clean("journal_totals"), new Vector2(1f, 0.5f), 0.7f, this.TextColor)).Add((OuiJournalPage.Cell) null).Add((OuiJournalPage.Cell) null).Add((OuiJournalPage.Cell) null).Add((OuiJournalPage.Cell) null).Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(SaveData.Instance.TotalStrawberries.ToString(), this.TextJustify, 0.6f, this.TextColor));
      OuiJournalPage.TextCell entry = new OuiJournalPage.TextCell(Dialog.Deaths(SaveData.Instance.TotalDeaths), this.TextJustify, 0.6f, this.TextColor);
      entry.SpreadOverColumns = SaveData.Instance.UnlockedModes;
      row.Add((OuiJournalPage.Cell) entry);
      for (int index = 1; index < SaveData.Instance.UnlockedModes; ++index)
        row.Add((OuiJournalPage.Cell) null);
      row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(Dialog.Time(SaveData.Instance.Time), this.TextJustify, 0.6f, this.TextColor));
      this.table.AddRow();
    }

    private string CompletionIcon(AreaStats data)
    {
      if (!AreaData.Get(data.ID).CanFullClear && data.Modes[0].Completed)
        return "beat";
      if (data.Modes[0].FullClear)
        return "fullclear";
      return data.Modes[0].Completed ? "clear" : "dot";
    }

    public override void Redraw(VirtualRenderTarget buffer)
    {
      base.Redraw(buffer);
      Draw.SpriteBatch.Begin();
      this.table.Render(new Vector2(60f, 20f));
      Draw.SpriteBatch.End();
    }

    private void DrawIcon(Vector2 pos, bool obtained, string icon)
    {
      if (obtained)
        MTN.Journal[icon].DrawCentered(pos);
      else
        MTN.Journal["dot"].DrawCentered(pos);
    }
  }
}
