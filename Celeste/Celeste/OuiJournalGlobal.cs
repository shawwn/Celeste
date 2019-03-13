// Decompiled with JetBrains decompiler
// Type: Celeste.OuiJournalGlobal
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class OuiJournalGlobal : OuiJournalPage
  {
    private OuiJournalPage.Table table;

    public OuiJournalGlobal(OuiJournal journal)
      : base(journal)
    {
      this.PageTexture = "page";
      this.table = new OuiJournalPage.Table().AddColumn((OuiJournalPage.Cell) new OuiJournalPage.TextCell("", new Vector2(1f, 0.5f), 1f, this.TextColor, 700f, false)).AddColumn((OuiJournalPage.Cell) new OuiJournalPage.TextCell("Global Steam Stats", new Vector2(0.5f, 0.5f), 1f, this.TextColor, 48f, true)).AddColumn((OuiJournalPage.Cell) new OuiJournalPage.TextCell("", new Vector2(1f, 0.5f), 0.7f, this.TextColor, 700f, false));
      foreach (Stat stat in Enum.GetValues(typeof (Stat)))
      {
        if (SaveData.Instance.CheatMode || SaveData.Instance.DebugMode || (stat != Stat.GOLDBERRIES || SaveData.Instance.TotalHeartGems >= 16) && (stat != Stat.PICO_BERRIES && stat != Stat.PICO_COMPLETES && stat != Stat.PICO_DEATHS || Settings.Instance.Pico8OnMainMenu))
        {
          string str = Stats.Global(stat).ToString();
          string text1 = Stats.Name(stat);
          string text2 = "";
          int index = str.Length - 1;
          int num = 0;
          while (index >= 0)
          {
            text2 = str[index].ToString() + (num <= 0 || num % 3 != 0 ? "" : ",") + text2;
            --index;
            ++num;
          }
          OuiJournalPage.Row row = this.table.AddRow();
          row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(text1, new Vector2(1f, 0.5f), 0.7f, this.TextColor, 0.0f, false));
          row.Add((OuiJournalPage.Cell) null);
          row.Add((OuiJournalPage.Cell) new OuiJournalPage.TextCell(text2, new Vector2(0.0f, 0.5f), 0.8f, this.TextColor, 0.0f, false));
        }
      }
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
