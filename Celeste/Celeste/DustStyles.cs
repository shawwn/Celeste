// Decompiled with JetBrains decompiler
// Type: Celeste.DustStyles
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  public static class DustStyles
  {
    public static Dictionary<int, DustStyles.DustStyle> Styles = new Dictionary<int, DustStyles.DustStyle>()
    {
      {
        3,
        new DustStyles.DustStyle()
        {
          EdgeColors = new Vector3[3]
          {
            Calc.HexToColor("f25a10").ToVector3(),
            Calc.HexToColor("ff0000").ToVector3(),
            Calc.HexToColor("f21067").ToVector3()
          },
          EyeColor = Color.Red,
          EyeTextures = "danger/dustcreature/eyes"
        }
      },
      {
        5,
        new DustStyles.DustStyle()
        {
          EdgeColors = new Vector3[3]
          {
            Calc.HexToColor("245ebb").ToVector3(),
            Calc.HexToColor("17a0ff").ToVector3(),
            Calc.HexToColor("17a0ff").ToVector3()
          },
          EyeColor = Calc.HexToColor("245ebb"),
          EyeTextures = "danger/dustcreature/templeeyes"
        }
      }
    };

    public static DustStyles.DustStyle Get(Session session)
    {
      if (!DustStyles.Styles.ContainsKey(session.Area.ID))
        return DustStyles.Styles[3];
      return DustStyles.Styles[session.Area.ID];
    }

    public static DustStyles.DustStyle Get(Scene scene)
    {
      return DustStyles.Get((scene as Level).Session);
    }

    public struct DustStyle
    {
      public Vector3[] EdgeColors;
      public Color EyeColor;
      public string EyeTextures;
    }
  }
}

