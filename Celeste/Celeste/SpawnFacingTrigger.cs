// Decompiled with JetBrains decompiler
// Type: Celeste.SpawnFacingTrigger
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
    [Tracked(false)]
    public class SpawnFacingTrigger : Entity
    {
        public Facings Facing;

        public SpawnFacingTrigger(EntityData data, Vector2 offset)
            : base(data.Position + offset)
        {
            this.Collider = (Collider) new Hitbox((float) data.Width, (float) data.Height);
            this.Facing = data.Enum<Facings>("facing");
            this.Visible = this.Active = false;
        }
    }
}