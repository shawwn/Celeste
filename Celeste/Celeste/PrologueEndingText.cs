// Decompiled with JetBrains decompiler
// Type: Celeste.PrologueEndingText
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste
{
  public class PrologueEndingText : Entity
  {
    private FancyText.Text text;

    public PrologueEndingText(bool instant)
    {
      this.Tag = (int) Tags.HUD;
      this.text = FancyText.Parse(Dialog.Clean("CH0_END", (Language) null), 960, 4, 0.0f, new Color?(), (Language) null);
      this.Add((Component) new Coroutine(this.Routine(instant), true));
    }

    private IEnumerator Routine(bool instant)
    {
      if (!instant)
        yield return (object) 4f;
      for (int i = 0; i < this.text.Count; ++i)
      {
        FancyText.Char c = this.text[i] as FancyText.Char;
        if (c != null)
        {
          while ((double) (c.Fade += Engine.DeltaTime * 20f) < 1.0)
            yield return (object) null;
          c.Fade = 1f;
          c = (FancyText.Char) null;
        }
      }
    }

    public override void Render()
    {
      this.text.Draw(this.Position, new Vector2(0.5f, 0.5f), Vector2.One, 1f, 0, int.MaxValue);
    }
  }
}

