// Decompiled with JetBrains decompiler
// Type: Celeste.TempleEndingMusicHandler
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Celeste
{
  public class TempleEndingMusicHandler : Entity
  {
    private HashSet<string> levels = new HashSet<string>();
    public const string StartLevel = "e-01";
    public const string EndLevel = "e-09";
    public const string ApplyIn = "e-*";
    private float startX;
    private float endX;

    public TempleEndingMusicHandler()
    {
      this.Tag = (int) Tags.TransitionUpdate | (int) Tags.Global;
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      Regex regex = new Regex(Regex.Escape("e-*").Replace("\\*", ".*") + "$");
      foreach (LevelData level in (scene as Level).Session.MapData.Levels)
      {
        if (level.Name.Equals("e-01"))
          this.startX = (float) level.Bounds.Left;
        else if (level.Name.Equals("e-09"))
          this.endX = (float) level.Bounds.Right;
        if (regex.IsMatch(level.Name))
          this.levels.Add(level.Name);
      }
    }

    public override void Update()
    {
      base.Update();
      Level scene = this.Scene as Level;
      Player entity = this.Scene.Tracker.GetEntity<Player>();
      if (entity == null || !this.levels.Contains(scene.Session.Level) || !(Audio.CurrentMusic == "event:/music/lvl5/mirror"))
        return;
      float num = Calc.Clamp((float) (((double) entity.X - (double) this.startX) / ((double) this.endX - (double) this.startX)), 0.0f, 1f);
      scene.Session.Audio.Music.Layer(1, 1f - num);
      scene.Session.Audio.Music.Layer(5, num);
      scene.Session.Audio.Apply();
    }
  }
}

