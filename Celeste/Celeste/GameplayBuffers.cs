// Decompiled with JetBrains decompiler
// Type: Celeste.GameplayBuffers
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  public static class GameplayBuffers
  {
    private static List<VirtualRenderTarget> all = new List<VirtualRenderTarget>();
    public static VirtualRenderTarget Gameplay;
    public static VirtualRenderTarget Level;
    public static VirtualRenderTarget ResortDust;
    public static VirtualRenderTarget LightBuffer;
    public static VirtualRenderTarget Light;
    public static VirtualRenderTarget Displacement;
    public static VirtualRenderTarget MirrorSources;
    public static VirtualRenderTarget MirrorMasks;
    public static VirtualRenderTarget SpeedRings;
    public static VirtualRenderTarget TempA;
    public static VirtualRenderTarget TempB;

    public static void Create()
    {
      GameplayBuffers.Unload();
      GameplayBuffers.Gameplay = GameplayBuffers.Create(320, 180);
      GameplayBuffers.Level = GameplayBuffers.Create(320, 180);
      GameplayBuffers.ResortDust = GameplayBuffers.Create(320, 180);
      GameplayBuffers.Light = GameplayBuffers.Create(320, 180);
      GameplayBuffers.Displacement = GameplayBuffers.Create(320, 180);
      GameplayBuffers.LightBuffer = GameplayBuffers.Create(1024, 1024);
      GameplayBuffers.MirrorSources = GameplayBuffers.Create(384, 244);
      GameplayBuffers.MirrorMasks = GameplayBuffers.Create(384, 244);
      GameplayBuffers.SpeedRings = GameplayBuffers.Create(512, 512);
      GameplayBuffers.TempA = GameplayBuffers.Create(320, 180);
      GameplayBuffers.TempB = GameplayBuffers.Create(320, 180);
    }

    private static VirtualRenderTarget Create(int width, int height)
    {
      VirtualRenderTarget renderTarget = VirtualContent.CreateRenderTarget("gameplay-buffer-" + (object) GameplayBuffers.all.Count, width, height, false, true, 0);
      GameplayBuffers.all.Add(renderTarget);
      return renderTarget;
    }

    public static void Unload()
    {
      foreach (VirtualAsset virtualAsset in GameplayBuffers.all)
        virtualAsset.Dispose();
      GameplayBuffers.all.Clear();
    }
  }
}
