// Decompiled with JetBrains decompiler
// Type: Celeste.Fonts
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Celeste
{
  public static class Fonts
  {
    public static Dictionary<string, PixelFont> Faces = new Dictionary<string, PixelFont>();

    public static void Load()
    {
      Fonts.Unload();
      foreach (string file in Directory.GetFiles(Path.Combine(Engine.ContentDirectory, "Dialog"), "*.fnt", SearchOption.AllDirectories))
      {
        XmlElement data = Calc.LoadXML(file)["font"];
        string index = data["info"].Attr("face");
        if (!Fonts.Faces.ContainsKey(index))
          Fonts.Faces.Add(index, new PixelFont(index));
        Fonts.Faces[index].AddFontSize(file, data, GFX.Gui, false);
      }
    }

    public static void Unload()
    {
      foreach (PixelFont pixelFont in Fonts.Faces.Values)
        pixelFont.Dispose();
      Fonts.Faces.Clear();
    }
  }
}
