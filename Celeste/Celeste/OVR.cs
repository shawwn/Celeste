// Decompiled with JetBrains decompiler
// Type: Celeste.OVR
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Monocle;
using System.IO;

namespace Celeste
{
    public static class OVR
    {
        public static Atlas Atlas;

        public static bool Loaded { get; private set; }

        public static void Load()
        {
            OVR.Atlas = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Overworld"), Atlas.AtlasDataFormat.PackerNoAtlas);
            OVR.Loaded = true;
        }

        public static void Unload()
        {
            OVR.Atlas.Dispose();
            OVR.Atlas = (Atlas) null;
            OVR.Loaded = false;
        }
    }
}