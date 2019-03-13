// Decompiled with JetBrains decompiler
// Type: Celeste.CliffFlags
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class CliffFlags : Entity
  {
    private static readonly Color[] colors = new Color[4]
    {
      Calc.HexToColor("d85f2f"),
      Calc.HexToColor("d82f63"),
      Calc.HexToColor("2fd8a2"),
      Calc.HexToColor("d8d62f")
    };
    private static readonly Color lineColor = Color.Lerp(Color.Gray, Color.DarkBlue, 0.25f);
    private static readonly Color pinColor = Color.Gray;

    public CliffFlags(Vector2 from, Vector2 to)
    {
      this.Depth = 8999;
      this.Position = from;
      Flagline flagline;
      this.Add((Component) (flagline = new Flagline(to, CliffFlags.lineColor, CliffFlags.pinColor, CliffFlags.colors, 10, 10, 10, 10, 2, 8)));
      flagline.ClothDroopAmount = 0.2f;
    }

    public CliffFlags(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Nodes[0] + offset)
    {
    }
  }
}

