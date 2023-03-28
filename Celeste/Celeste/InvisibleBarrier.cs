// Decompiled with JetBrains decompiler
// Type: Celeste.InvisibleBarrier
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
    [Tracked(false)]
    public class InvisibleBarrier : Solid
    {
        public InvisibleBarrier(Vector2 position, float width, float height)
            : base(position, width, height, true)
        {
            this.Tag = (int) Tags.TransitionUpdate;
            this.Collidable = false;
            this.Visible = false;
            this.Add((Component) new ClimbBlocker(true));
            this.SurfaceSoundIndex = 33;
        }

        public InvisibleBarrier(EntityData data, Vector2 offset)
            : this(data.Position + offset, (float) data.Width, (float) data.Height)
        {
        }

        public override void Update()
        {
            this.Collidable = true;
            if (this.CollideCheck<Player>())
                this.Collidable = false;
            if (this.Collidable)
                return;
            this.Active = false;
        }
    }
}