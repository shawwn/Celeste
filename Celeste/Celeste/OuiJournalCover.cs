// Decompiled with JetBrains decompiler
// Type: Celeste.OuiJournalCover
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class OuiJournalCover : OuiJournalPage
  {
    public OuiJournalCover(OuiJournal journal)
      : base(journal)
    {
      this.PageTexture = "cover";
    }

    public override void Redraw(VirtualRenderTarget buffer)
    {
      base.Redraw(buffer);
      Draw.SpriteBatch.Begin();
      string str = Dialog.Clean("journal_of", (Language) null);
      if (str.Length > 0)
        str += "\n";
      ActiveFont.Draw(str + SaveData.Instance.Name, new Vector2(805f, 400f), new Vector2(0.5f, 0.5f), Vector2.One * 2f, Color.Black * 0.5f);
      Draw.SpriteBatch.End();
    }
  }
}

