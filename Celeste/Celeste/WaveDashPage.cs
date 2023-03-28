// Decompiled with JetBrains decompiler
// Type: Celeste.WaveDashPage
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using System.Collections;

namespace Celeste
{
    public abstract class WaveDashPage
    {
        public WaveDashPresentation Presentation;
        public Color ClearColor;
        public WaveDashPage.Transitions Transition;
        public bool AutoProgress;
        public bool WaitingForInput;

        public int Width => this.Presentation.ScreenWidth;

        public int Height => this.Presentation.ScreenHeight;

        public abstract IEnumerator Routine();

        public virtual void Added(WaveDashPresentation presentation) => this.Presentation = presentation;

        public virtual void Update()
        {
        }

        public virtual void Render()
        {
        }

        protected IEnumerator PressButton()
        {
            this.WaitingForInput = true;
            while (!Input.MenuConfirm.Pressed)
                yield return (object) null;
            this.WaitingForInput = false;
            Audio.Play("event:/new_content/game/10_farewell/ppt_mouseclick");
        }

        public enum Transitions
        {
            ScaleIn,
            FadeIn,
            Rotate3D,
            Blocky,
            Spiral,
        }
    }
}