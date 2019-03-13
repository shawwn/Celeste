// Decompiled with JetBrains decompiler
// Type: Celeste.AutoSplitterInfo
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;

namespace Celeste
{
  public class AutoSplitterInfo
  {
    public int Chapter;
    public int Mode;
    public string Level;
    public bool TimerActive;
    public bool ChapterStarted;
    public bool ChapterComplete;
    public long ChapterTime;
    public int ChapterStrawberries;
    public bool ChapterCassette;
    public bool ChapterHeart;
    public long FileTime;
    public int FileStrawberries;
    public int FileCassettes;
    public int FileHearts;

    public void Update()
    {
      Level scene = Engine.Scene as Level;
      this.ChapterStarted = scene != null;
      this.ChapterComplete = this.ChapterStarted && scene.Completed;
      this.TimerActive = this.ChapterStarted && !scene.Completed;
      this.Chapter = this.ChapterStarted ? scene.Session.Area.ID : -1;
      this.Mode = this.ChapterStarted ? (int) scene.Session.Area.Mode : -1;
      this.Level = this.ChapterStarted ? scene.Session.Level : "";
      this.ChapterTime = this.ChapterStarted ? scene.Session.Time : 0L;
      this.FileTime = SaveData.Instance != null ? SaveData.Instance.Time : 0L;
      this.ChapterStrawberries = this.ChapterStarted ? scene.Session.Strawberries.Count : 0;
      this.FileStrawberries = SaveData.Instance != null ? SaveData.Instance.TotalStrawberries : 0;
      this.ChapterHeart = this.ChapterStarted && scene.Session.HeartGem;
      this.FileHearts = SaveData.Instance != null ? SaveData.Instance.TotalHeartGems : 0;
      this.ChapterCassette = this.ChapterStarted && scene.Session.Cassette;
      this.FileCassettes = SaveData.Instance != null ? SaveData.Instance.TotalCassettes : 0;
    }
  }
}

