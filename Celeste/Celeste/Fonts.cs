// Decompiled with JetBrains decompiler
// Type: Celeste.Fonts
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Monocle;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Celeste
{
  public static class Fonts
  {
    private static Dictionary<string, List<string>> paths = new Dictionary<string, List<string>>();
    private static Dictionary<string, PixelFont> loadedFonts = new Dictionary<string, PixelFont>();

    public static PixelFont Load(string face)
    {
      PixelFont pixelFont;
      List<string> stringList;
      if (!Fonts.loadedFonts.TryGetValue(face, out pixelFont) && Fonts.paths.TryGetValue(face, out stringList))
      {
        Fonts.loadedFonts.Add(face, pixelFont = new PixelFont(face));
        foreach (string path in stringList)
          pixelFont.AddFontSize(path, GFX.Gui);
      }
      return pixelFont;
    }

    public static PixelFont Get(string face)
    {
      PixelFont pixelFont;
      return Fonts.loadedFonts.TryGetValue(face, out pixelFont) ? pixelFont : (PixelFont) null;
    }

    public static void Unload(string face)
    {
      PixelFont pixelFont;
      if (!Fonts.loadedFonts.TryGetValue(face, out pixelFont))
        return;
      pixelFont.Dispose();
      Fonts.loadedFonts.Remove(face);
    }

    public static void Reload()
    {
      List<string> stringList = new List<string>();
      foreach (string key in Fonts.loadedFonts.Keys)
        stringList.Add(key);
      foreach (string str in stringList)
      {
        Fonts.loadedFonts[str].Dispose();
        Fonts.Load(str);
      }
    }

    public static void Prepare()
    {
      XmlReaderSettings settings = new XmlReaderSettings();
      settings.CloseInput = true;
      foreach (string file in Directory.GetFiles(Path.Combine(Engine.ContentDirectory, "Dialog"), "*.fnt", SearchOption.AllDirectories))
      {
        string key = (string) null;
        using (XmlReader xmlReader = XmlReader.Create((Stream) File.OpenRead(file), settings))
        {
          while (xmlReader.Read())
          {
            if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "info")
              key = xmlReader.GetAttribute("face");
          }
        }
        if (key != null)
        {
          List<string> stringList;
          if (!Fonts.paths.TryGetValue(key, out stringList))
            Fonts.paths.Add(key, stringList = new List<string>());
          stringList.Add(file);
        }
      }
    }

    public static void Log()
    {
      Engine.Commands.Log((object) "EXISTING FONTS:");
      foreach (KeyValuePair<string, List<string>> path in Fonts.paths)
      {
        Engine.Commands.Log((object) (" - " + path.Key));
        foreach (string str in path.Value)
          Engine.Commands.Log((object) (" - > " + str));
      }
      Engine.Commands.Log((object) "LOADED:");
      foreach (KeyValuePair<string, PixelFont> loadedFont in Fonts.loadedFonts)
        Engine.Commands.Log((object) (" - " + loadedFont.Key));
    }
  }
}
