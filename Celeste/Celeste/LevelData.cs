// Decompiled with JetBrains decompiler
// Type: Celeste.LevelData
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Celeste
{
  public class LevelData
  {
    public string Solids = "";
    public string Bg = "";
    public string FgTiles = "";
    public string BgTiles = "";
    public string ObjTiles = "";
    public string Music = "";
    public string AltMusic = "";
    public string Ambience = "";
    public float[] MusicLayers = new float[4];
    public int MusicProgress = -1;
    public int AmbienceProgress = -1;
    public string Name;
    public bool Dummy;
    public int Strawberries;
    public bool HasGem;
    public bool HasHeartGem;
    public bool HasCheckpoint;
    public bool DisableDownTransition;
    public Rectangle Bounds;
    public List<EntityData> Entities;
    public List<EntityData> Triggers;
    public List<Vector2> Spawns;
    public List<DecalData> FgDecals;
    public List<DecalData> BgDecals;
    public WindController.Patterns WindPattern;
    public Vector2 CameraOffset;
    public bool Dark;
    public bool Underwater;
    public bool Space;
    public bool MusicWhispers;
    public bool DelayAltMusic;
    public int EditorColorIndex;

    public LevelData(BinaryPacker.Element data)
    {
      this.Bounds = (Rectangle) null;
      foreach (KeyValuePair<string, object> attribute in data.Attributes)
      {
        switch (attribute.Key)
        {
          case "alt_music":
            this.AltMusic = (string) attribute.Value;
            continue;
          case "ambience":
            this.Ambience = (string) attribute.Value;
            continue;
          case "ambienceProgress":
            string s1 = attribute.Value.ToString();
            if (string.IsNullOrEmpty(s1) || !int.TryParse(s1, out this.AmbienceProgress))
            {
              this.AmbienceProgress = -1;
              continue;
            }
            continue;
          case "c":
            this.EditorColorIndex = (int) attribute.Value;
            continue;
          case "cameraOffsetX":
            this.CameraOffset.X = (__Null) (double) Convert.ToSingle(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture);
            continue;
          case "cameraOffsetY":
            this.CameraOffset.Y = (__Null) (double) Convert.ToSingle(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture);
            continue;
          case "dark":
            this.Dark = (bool) attribute.Value;
            continue;
          case "delayAltMusicFade":
            this.DelayAltMusic = (bool) attribute.Value;
            continue;
          case "disableDownTransition":
            this.DisableDownTransition = (bool) attribute.Value;
            continue;
          case "height":
            this.Bounds.Height = (__Null) (int) attribute.Value;
            if (this.Bounds.Height == 184)
            {
              this.Bounds.Height = (__Null) 180;
              continue;
            }
            continue;
          case "music":
            this.Music = (string) attribute.Value;
            continue;
          case "musicLayer1":
            this.MusicLayers[0] = (bool) attribute.Value ? 1f : 0.0f;
            continue;
          case "musicLayer2":
            this.MusicLayers[1] = (bool) attribute.Value ? 1f : 0.0f;
            continue;
          case "musicLayer3":
            this.MusicLayers[2] = (bool) attribute.Value ? 1f : 0.0f;
            continue;
          case "musicLayer4":
            this.MusicLayers[3] = (bool) attribute.Value ? 1f : 0.0f;
            continue;
          case "musicProgress":
            string s2 = attribute.Value.ToString();
            if (string.IsNullOrEmpty(s2) || !int.TryParse(s2, out this.MusicProgress))
            {
              this.MusicProgress = -1;
              continue;
            }
            continue;
          case "name":
            this.Name = attribute.Value.ToString().Substring(4);
            continue;
          case "space":
            this.Space = (bool) attribute.Value;
            continue;
          case "underwater":
            this.Underwater = (bool) attribute.Value;
            continue;
          case "whisper":
            this.MusicWhispers = (bool) attribute.Value;
            continue;
          case "width":
            this.Bounds.Width = (__Null) (int) attribute.Value;
            continue;
          case "windPattern":
            this.WindPattern = (WindController.Patterns) Enum.Parse(typeof (WindController.Patterns), (string) attribute.Value);
            continue;
          case "x":
            this.Bounds.X = (__Null) (int) attribute.Value;
            continue;
          case "y":
            this.Bounds.Y = (__Null) (int) attribute.Value;
            continue;
          default:
            continue;
        }
      }
      this.Spawns = new List<Vector2>();
      this.Entities = new List<EntityData>();
      this.Triggers = new List<EntityData>();
      this.BgDecals = new List<DecalData>();
      this.FgDecals = new List<DecalData>();
      foreach (BinaryPacker.Element child1 in data.Children)
      {
        if (child1.Name == "entities")
        {
          if (child1.Children != null)
          {
            foreach (BinaryPacker.Element child2 in child1.Children)
            {
              if (child2.Name == "player")
                this.Spawns.Add(new Vector2((float) this.Bounds.X + Convert.ToSingle(child2.Attributes["x"], (IFormatProvider) CultureInfo.InvariantCulture), (float) this.Bounds.Y + Convert.ToSingle(child2.Attributes["y"], (IFormatProvider) CultureInfo.InvariantCulture)));
              else if (child2.Name == "strawberry" || child2.Name == "snowberry")
                ++this.Strawberries;
              else if (child2.Name == "shard")
                this.HasGem = true;
              else if (child2.Name == "blackGem")
                this.HasHeartGem = true;
              else if (child2.Name == "checkpoint")
                this.HasCheckpoint = true;
              if (!child2.Name.Equals("player"))
                this.Entities.Add(this.CreateEntityData(child2));
            }
          }
        }
        else if (child1.Name == "triggers")
        {
          if (child1.Children != null)
          {
            foreach (BinaryPacker.Element child2 in child1.Children)
              this.Triggers.Add(this.CreateEntityData(child2));
          }
        }
        else if (child1.Name == "bgdecals")
        {
          if (child1.Children != null)
          {
            foreach (BinaryPacker.Element child2 in child1.Children)
              this.BgDecals.Add(new DecalData()
              {
                Position = new Vector2(Convert.ToSingle(child2.Attributes["x"], (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToSingle(child2.Attributes["y"], (IFormatProvider) CultureInfo.InvariantCulture)),
                Scale = new Vector2(Convert.ToSingle(child2.Attributes["scaleX"], (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToSingle(child2.Attributes["scaleY"], (IFormatProvider) CultureInfo.InvariantCulture)),
                Texture = (string) child2.Attributes["texture"]
              });
          }
        }
        else if (child1.Name == "fgdecals")
        {
          if (child1.Children != null)
          {
            foreach (BinaryPacker.Element child2 in child1.Children)
              this.FgDecals.Add(new DecalData()
              {
                Position = new Vector2(Convert.ToSingle(child2.Attributes["x"], (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToSingle(child2.Attributes["y"], (IFormatProvider) CultureInfo.InvariantCulture)),
                Scale = new Vector2(Convert.ToSingle(child2.Attributes["scaleX"], (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToSingle(child2.Attributes["scaleY"], (IFormatProvider) CultureInfo.InvariantCulture)),
                Texture = (string) child2.Attributes["texture"]
              });
          }
        }
        else if (child1.Name == "solids")
          this.Solids = child1.Attr("innerText", "");
        else if (child1.Name == "bg")
          this.Bg = child1.Attr("innerText", "");
        else if (child1.Name == "fgtiles")
          this.FgTiles = child1.Attr("innerText", "");
        else if (child1.Name == "bgtiles")
          this.BgTiles = child1.Attr("innerText", "");
        else if (child1.Name == "objtiles")
          this.ObjTiles = child1.Attr("innerText", "");
      }
      this.Dummy = this.Spawns.Count <= 0;
    }

    private EntityData CreateEntityData(BinaryPacker.Element entity)
    {
      EntityData entityData = new EntityData();
      entityData.Name = entity.Name;
      entityData.Level = this;
      if (entity.Attributes != null)
      {
        foreach (KeyValuePair<string, object> attribute in entity.Attributes)
        {
          if (attribute.Key == "id")
            entityData.ID = (int) attribute.Value;
          else if (attribute.Key == "x")
            entityData.Position.X = (__Null) (double) Convert.ToSingle(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture);
          else if (attribute.Key == "y")
            entityData.Position.Y = (__Null) (double) Convert.ToSingle(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture);
          else if (attribute.Key == "width")
            entityData.Width = (int) attribute.Value;
          else if (attribute.Key == "height")
            entityData.Height = (int) attribute.Value;
          else if (attribute.Key == "originX")
            entityData.Origin.X = (__Null) (double) Convert.ToSingle(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture);
          else if (attribute.Key == "originY")
          {
            entityData.Origin.Y = (__Null) (double) Convert.ToSingle(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture);
          }
          else
          {
            if (entityData.Values == null)
              entityData.Values = new Dictionary<string, object>();
            entityData.Values.Add(attribute.Key, attribute.Value);
          }
        }
      }
      entityData.Nodes = new Vector2[entity.Children == null ? 0 : entity.Children.Count];
      for (int index = 0; index < entityData.Nodes.Length; ++index)
      {
        foreach (KeyValuePair<string, object> attribute in entity.Children[index].Attributes)
        {
          if (attribute.Key == "x")
            entityData.Nodes[index].X = (__Null) (double) Convert.ToSingle(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture);
          else if (attribute.Key == "y")
            entityData.Nodes[index].Y = (__Null) (double) Convert.ToSingle(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture);
        }
      }
      return entityData;
    }

    public bool Check(Vector2 at)
    {
      if (at.X >= (double) ((Rectangle) ref this.Bounds).get_Left() && at.Y >= (double) ((Rectangle) ref this.Bounds).get_Top() && at.X < (double) ((Rectangle) ref this.Bounds).get_Right())
        return at.Y < (double) ((Rectangle) ref this.Bounds).get_Bottom();
      return false;
    }

    public Rectangle TileBounds
    {
      get
      {
        return new Rectangle(this.Bounds.X / 8, this.Bounds.Y / 8, (int) Math.Ceiling((double) (float) this.Bounds.Width / 8.0), (int) Math.Ceiling((double) (float) this.Bounds.Height / 8.0));
      }
    }

    public Vector2 Position
    {
      get
      {
        return new Vector2((float) this.Bounds.X, (float) this.Bounds.Y);
      }
      set
      {
        for (int index1 = 0; index1 < this.Spawns.Count; ++index1)
        {
          List<Vector2> spawns = this.Spawns;
          int index2 = index1;
          spawns[index2] = Vector2.op_Subtraction(spawns[index2], this.Position);
        }
        this.Bounds.X = (__Null) (int) value.X;
        this.Bounds.Y = (__Null) (int) value.Y;
        for (int index1 = 0; index1 < this.Spawns.Count; ++index1)
        {
          List<Vector2> spawns = this.Spawns;
          int index2 = index1;
          spawns[index2] = Vector2.op_Addition(spawns[index2], this.Position);
        }
      }
    }

    public int LoadSeed
    {
      get
      {
        int num = 0;
        foreach (char ch in this.Name)
          num += (int) ch;
        return num;
      }
    }
  }
}
