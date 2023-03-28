// Decompiled with JetBrains decompiler
// Type: Celeste.GrabbyIcon
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
    public class GrabbyIcon : Entity
    {
        private bool enabled;
        private Wiggler wiggler;

        public GrabbyIcon()
        {
            this.Depth = -1000001;
            this.Tag = (int) Tags.Global | (int) Tags.PauseUpdate | (int) Tags.TransitionUpdate;
            this.Add((Component) (this.wiggler = Wiggler.Create(0.1f, 0.3f)));
        }

        public override void Update()
        {
            base.Update();
            bool flag = false;
            if (!this.SceneAs<Level>().InCutscene)
            {
                Player entity = this.Scene.Tracker.GetEntity<Player>();
                if (entity != null && !entity.Dead && Settings.Instance.GrabMode == GrabModes.Toggle && Input.GrabCheck)
                    flag = true;
            }
            if (flag == this.enabled)
                return;
            this.enabled = flag;
            this.wiggler.Start();
        }

        public override void Render()
        {
            if (!this.enabled)
                return;
            Vector2 scale = Vector2.One * (float) (1.0 + (double) this.wiggler.Value * 0.20000000298023224);
            Player entity = this.Scene.Tracker.GetEntity<Player>();
            if (entity == null)
                return;
            GFX.Game["util/glove"].DrawJustified(new Vector2(entity.X, entity.Y - 16f), new Vector2(0.5f, 1f), Color.White, scale);
        }
    }
}