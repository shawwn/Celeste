// Decompiled with JetBrains decompiler
// Type: Celeste.CollisionData
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;

namespace Celeste
{
    public struct CollisionData
    {
        public Vector2 Direction;
        public Vector2 Moved;
        public Vector2 TargetPosition;
        public Platform Hit;
        public Solid Pusher;
        public static readonly CollisionData Empty;
    }
}