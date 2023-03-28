// Decompiled with JetBrains decompiler
// Type: Celeste.CutsceneNode
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
    [Tracked(false)]
    public class CutsceneNode : Entity
    {
        public string Name;

        public CutsceneNode(EntityData data, Vector2 offset)
            : base(data.Position + offset)
        {
            this.Name = data.Attr("nodeName");
        }

        public static CutsceneNode Find(string name)
        {
            foreach (CutsceneNode entity in Engine.Scene.Tracker.GetEntities<CutsceneNode>())
            {
                if (entity.Name != null && entity.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return entity;
            }
            return (CutsceneNode) null;
        }
    }
}