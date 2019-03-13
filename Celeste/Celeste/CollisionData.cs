// Decompiled with JetBrains decompiler
// Type: Celeste.CollisionData
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

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
