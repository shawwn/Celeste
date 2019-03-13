// Decompiled with JetBrains decompiler
// Type: Celeste.SurfaceIndex
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  public class SurfaceIndex
  {
    public static Dictionary<char, int> TileToIndex = new Dictionary<char, int>()
    {
      {
        '1',
        3
      },
      {
        '3',
        4
      },
      {
        '4',
        7
      },
      {
        '5',
        8
      },
      {
        '6',
        8
      },
      {
        '7',
        8
      },
      {
        '8',
        8
      },
      {
        '9',
        13
      },
      {
        'a',
        8
      },
      {
        'b',
        23
      },
      {
        'c',
        8
      },
      {
        'd',
        8
      },
      {
        'e',
        8
      },
      {
        'f',
        8
      },
      {
        'g',
        8
      },
      {
        'h',
        33
      },
      {
        'i',
        4
      },
      {
        'j',
        8
      },
      {
        'k',
        3
      },
      {
        'l',
        33
      },
      {
        'm',
        3
      }
    };
    public const string Param = "surface_index";
    public const int Asphalt = 1;
    public const int Car = 2;
    public const int Dirt = 3;
    public const int Snow = 4;
    public const int Wood = 5;
    public const int Girder = 7;
    public const int Brick = 8;
    public const int ZipMover = 9;
    public const int ResortWood = 13;
    public const int DreamBlockInactive = 11;
    public const int DreamBlockActive = 12;
    public const int ResortRoof = 14;
    public const int ResortSinkingPlatforms = 15;
    public const int ResortLinens = 17;
    public const int ResortBoxes = 18;
    public const int ResortBooks = 19;
    public const int ClutterDoor = 20;
    public const int ClutterSwitch = 21;
    public const int ResortElevator = 22;
    public const int CliffsideSnow = 23;
    public const int CliffsideWhiteBlock = 27;
    public const int Gondola = 28;
    public const int AuroraGlass = 32;
    public const int Grass = 33;
    public const int CoreIce = 36;
    public const int CoreMoltenRock = 37;
    public const int StoneBridge = 6;
    public const int ResortBasementTile = 16;
    public const int ResortMagicButton = 21;

    public static Platform GetPlatformByPriority(List<Entity> platforms)
    {
      Platform platform1 = (Platform) null;
      foreach (Entity platform2 in platforms)
      {
        if (platform2 is Platform && (platform1 == null || (platform2 as Platform).SurfaceSoundPriority > platform1.SurfaceSoundPriority))
          platform1 = platform2 as Platform;
      }
      return platform1;
    }
  }
}
