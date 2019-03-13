// Decompiled with JetBrains decompiler
// Type: Monocle.VirtualContent
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Monocle
{
  public static class VirtualContent
  {
    private static List<VirtualAsset> assets = new List<VirtualAsset>();
    private static bool reloading;

    public static int Count
    {
      get
      {
        return VirtualContent.assets.Count;
      }
    }

    public static VirtualTexture CreateTexture(string path)
    {
      VirtualTexture virtualTexture = new VirtualTexture(path);
      VirtualContent.assets.Add((VirtualAsset) virtualTexture);
      return virtualTexture;
    }

    public static VirtualTexture CreateTexture(
      string name,
      int width,
      int height,
      Color color)
    {
      VirtualTexture virtualTexture = new VirtualTexture(name, width, height, color);
      VirtualContent.assets.Add((VirtualAsset) virtualTexture);
      return virtualTexture;
    }

    public static VirtualRenderTarget CreateRenderTarget(
      string name,
      int width,
      int height,
      bool depth = false,
      bool preserve = true,
      int multiSampleCount = 0)
    {
      VirtualRenderTarget virtualRenderTarget = new VirtualRenderTarget(name, width, height, multiSampleCount, depth, preserve);
      VirtualContent.assets.Add((VirtualAsset) virtualRenderTarget);
      return virtualRenderTarget;
    }

    public static void BySize()
    {
      Dictionary<int, Dictionary<int, int>> dictionary = new Dictionary<int, Dictionary<int, int>>();
      foreach (VirtualAsset asset in VirtualContent.assets)
      {
        if (!dictionary.ContainsKey(asset.Width))
          dictionary.Add(asset.Width, new Dictionary<int, int>());
        if (!dictionary[asset.Width].ContainsKey(asset.Height))
          dictionary[asset.Width].Add(asset.Height, 0);
        dictionary[asset.Width][asset.Height]++;
      }
      foreach (KeyValuePair<int, Dictionary<int, int>> keyValuePair1 in dictionary)
      {
        foreach (KeyValuePair<int, int> keyValuePair2 in keyValuePair1.Value)
          Console.WriteLine(keyValuePair1.Key.ToString() + "x" + (object) keyValuePair2.Key + ": " + (object) keyValuePair2.Value);
      }
    }

    public static void ByName()
    {
      foreach (VirtualAsset asset in VirtualContent.assets)
        Console.WriteLine(asset.Name + "[" + (object) asset.Width + "x" + (object) asset.Height + "]");
    }

    internal static void Remove(VirtualAsset asset)
    {
      VirtualContent.assets.Remove(asset);
    }

    internal static void Reload()
    {
      if (VirtualContent.reloading)
      {
        foreach (VirtualAsset asset in VirtualContent.assets)
          asset.Reload();
      }
      VirtualContent.reloading = false;
    }

    internal static void Unload()
    {
      foreach (VirtualAsset asset in VirtualContent.assets)
        asset.Unload();
      VirtualContent.reloading = true;
    }
  }
}
