// Decompiled with JetBrains decompiler
// Type: Monocle.PointSectors
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;

namespace Monocle
{
  [Flags]
  public enum PointSectors
  {
    Center = 0,
    Top = 1,
    Bottom = 2,
    TopLeft = 9,
    TopRight = 5,
    Left = 8,
    Right = 4,
    BottomLeft = Left | Bottom, // 0x0000000A
    BottomRight = Right | Bottom, // 0x00000006
  }
}
