// Decompiled with JetBrains decompiler
// Type: Celeste.MountainState
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
  public class MountainState
  {
    public Skybox Skybox;
    public VirtualTexture TerrainTexture;
    public VirtualTexture BuildingsTexture;
    public Color FogColor;

    public MountainState(
      VirtualTexture terrainTexture,
      VirtualTexture buildingsTexture,
      VirtualTexture skyboxTexture,
      Color fogColor)
    {
      this.TerrainTexture = terrainTexture;
      this.BuildingsTexture = buildingsTexture;
      this.Skybox = new Skybox(skyboxTexture, 25f, false);
      this.FogColor = fogColor;
    }
  }
}
