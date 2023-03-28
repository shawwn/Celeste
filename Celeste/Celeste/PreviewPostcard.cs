// Decompiled with JetBrains decompiler
// Type: Celeste.PreviewPostcard
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Monocle;
using System.Collections;

namespace Celeste
{
    public class PreviewPostcard : Scene
    {
        private Postcard postcard;

        public PreviewPostcard(Postcard postcard)
        {
            Audio.SetMusic((string) null);
            Audio.SetAmbience((string) null);
            this.postcard = postcard;
            this.Add(new Entity()
            {
                (Component) new Coroutine(this.Routine(postcard))
            });
            this.Add((Monocle.Renderer) new HudRenderer());
        }

        private IEnumerator Routine(Postcard postcard)
        {
            PreviewPostcard previewPostcard = this;
            yield return (object) 0.25f;
            previewPostcard.Add((Entity) postcard);
            yield return (object) postcard.DisplayRoutine();
            Engine.Scene = (Scene) new OverworldLoader(Overworld.StartMode.MainMenu);
        }

        public override void BeforeRender()
        {
            base.BeforeRender();
            if (this.postcard == null)
                return;
            this.postcard.BeforeRender();
        }
    }
}