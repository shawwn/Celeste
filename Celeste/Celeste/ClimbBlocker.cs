// Decompiled with JetBrains decompiler
// Type: Celeste.ClimbBlocker
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
    [Tracked(false)]
    public class ClimbBlocker : Component
    {
        public bool Blocking = true;
        public bool Edge;

        public ClimbBlocker(bool edge)
            : base(false, false)
        {
            this.Edge = edge;
        }

        public static bool Check(Scene scene, Entity entity, Vector2 at)
        {
            Vector2 position = entity.Position;
            entity.Position = at;
            int num = ClimbBlocker.Check(scene, entity) ? 1 : 0;
            entity.Position = position;
            return num != 0;
        }

        public static bool Check(Scene scene, Entity entity)
        {
            foreach (ClimbBlocker component in scene.Tracker.GetComponents<ClimbBlocker>())
            {
                if (component.Blocking && entity.CollideCheck(component.Entity))
                    return true;
            }
            return false;
        }

        public static bool EdgeCheck(Scene scene, Entity entity, int dir)
        {
            foreach (ClimbBlocker component in scene.Tracker.GetComponents<ClimbBlocker>())
            {
                if (component.Blocking && component.Edge && entity.CollideCheck(component.Entity, entity.Position + Vector2.UnitX * (float) dir))
                    return true;
            }
            return false;
        }
    }
}