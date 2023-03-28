// Decompiled with JetBrains decompiler
// Type: Celeste.CS10_FreeBird
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Monocle;
using System.Collections;

namespace Celeste
{
    public class CS10_FreeBird : CutsceneEntity
    {
        public CS10_FreeBird()
            : base()
        {
        }

        public override void OnBegin(Level level) => this.Add((Component) new Coroutine(this.Cutscene(level)));

        private IEnumerator Cutscene(Level level)
        {
            CS10_FreeBird cs10FreeBird = this;
            yield return (object) Textbox.Say("CH9_FREE_BIRD");
            FadeWipe fadeWipe = new FadeWipe((Scene) level, false);
            fadeWipe.Duration = 3f;
            yield return (object) fadeWipe.Duration;
            cs10FreeBird.EndCutscene(level);
        }

        public override void OnEnd(Level level) => level.CompleteArea(false, true);
    }
}