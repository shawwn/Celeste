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
    public static Dictionary<int, DustStyles.DustStyle> Styles;

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

    static DustStyles()
    {
      Dictionary<int, DustStyles.DustStyle> dictionary1 = new Dictionary<int, DustStyles.DustStyle>();
      Dictionary<int, DustStyles.DustStyle> dictionary2 = dictionary1;
      DustStyles.DustStyle dustStyle1 = new DustStyles.DustStyle();
      ref DustStyles.DustStyle local1 = ref dustStyle1;
      Vector3[] vector3Array1 = new Vector3[3];
      Color color1 = Calc.HexToColor("f25a10");
      vector3Array1[0] = ((Color) ref color1).ToVector3();
      Color color2 = Calc.HexToColor("ff0000");
      vector3Array1[1] = ((Color) ref color2).ToVector3();
      Color color3 = Calc.HexToColor("f21067");
      vector3Array1[2] = ((Color) ref color3).ToVector3();
      local1.EdgeColors = vector3Array1;
      dustStyle1.EyeColor = Color.get_Red();
      dustStyle1.EyeTextures = "danger/dustcreature/eyes";
      DustStyles.DustStyle dustStyle2 = dustStyle1;
      dictionary2.Add(3, dustStyle2);
      Dictionary<int, DustStyles.DustStyle> dictionary3 = dictionary1;
      DustStyles.DustStyle dustStyle3 = new DustStyles.DustStyle();
      ref DustStyles.DustStyle local2 = ref dustStyle3;
      Vector3[] vector3Array2 = new Vector3[3];
      Color color4 = Calc.HexToColor("245ebb");
      vector3Array2[0] = ((Color) ref color4).ToVector3();
      Color color5 = Calc.HexToColor("17a0ff");
      vector3Array2[1] = ((Color) ref color5).ToVector3();
      Color color6 = Calc.HexToColor("17a0ff");
      vector3Array2[2] = ((Color) ref color6).ToVector3();
      local2.EdgeColors = vector3Array2;
      dustStyle3.EyeColor = Calc.HexToColor("245ebb");
      dustStyle3.EyeTextures = "danger/dustcreature/templeeyes";
      DustStyles.DustStyle dustStyle4 = dustStyle3;
      dictionary3.Add(5, dustStyle4);
      DustStyles.Styles = dictionary1;
    }

    public struct DustStyle
    {
      public Vector3[] EdgeColors;
      public Color EyeColor;
      public string EyeTextures;
    }
  }
}
