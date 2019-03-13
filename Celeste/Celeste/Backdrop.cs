// Decompiled with JetBrains decompiler
// Type: Celeste.Backdrop
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste
{
  public abstract class Backdrop
  {
    public bool UseSpritebatch = true;
    public Vector2 Scroll = Vector2.One;
    public Color Color = Color.White;
    public bool LoopX = true;
    public bool LoopY = true;
    public bool InstantIn = true;
    public string Name;
    public Vector2 Position;
    public Vector2 Speed;
    public bool FlipX;
    public bool FlipY;
    public Backdrop.Fader FadeX;
    public Backdrop.Fader FadeY;
    public HashSet<string> ExcludeFrom;
    public HashSet<string> OnlyIn;
    public string OnlyIfFlag;
    public string OnlyIfNotFlag;
    public string AlsoIfFlag;
    public bool? Dreaming;
    public bool Visible;
    public bool InstantOut;

    public Backdrop()
    {
      this.Visible = true;
    }

    public bool IsVisible(Level level)
    {
      return (string.IsNullOrEmpty(this.OnlyIfNotFlag) || !level.Session.GetFlag(this.OnlyIfNotFlag)) && (!string.IsNullOrEmpty(this.AlsoIfFlag) && level.Session.GetFlag(this.AlsoIfFlag) || (!this.Dreaming.HasValue || this.Dreaming.Value == level.Session.Dreaming) && (string.IsNullOrEmpty(this.OnlyIfFlag) || level.Session.GetFlag(this.OnlyIfFlag)) && ((this.ExcludeFrom == null || !this.ExcludeFrom.Contains(level.Session.Level)) && (this.OnlyIn == null || this.OnlyIn.Contains(level.Session.Level))));
    }

    public virtual void Update(Scene scene)
    {
      Level level = scene as Level;
      if (level.Transitioning)
      {
        if (this.InstantIn && this.IsVisible(level))
          this.Visible = true;
        if (!this.InstantOut || this.IsVisible(level))
          return;
        this.Visible = false;
      }
      else
        this.Visible = this.IsVisible(level);
    }

    public virtual void BeforeRender(Scene scene)
    {
    }

    public virtual void Render(Scene scene)
    {
    }

    public virtual void Ended(Scene scene)
    {
    }

    public class Fader
    {
      private List<Backdrop.Fader.Segment> Segments = new List<Backdrop.Fader.Segment>();

      public Backdrop.Fader Add(float posFrom, float posTo, float fadeFrom, float fadeTo)
      {
        this.Segments.Add(new Backdrop.Fader.Segment()
        {
          PositionFrom = posFrom,
          PositionTo = posTo,
          From = fadeFrom,
          To = fadeTo
        });
        return this;
      }

      public float Value(float position)
      {
        float num = 1f;
        foreach (Backdrop.Fader.Segment segment in this.Segments)
          num *= Calc.ClampedMap(position, segment.PositionFrom, segment.PositionTo, segment.From, segment.To);
        return num;
      }

      private struct Segment
      {
        public float PositionFrom;
        public float PositionTo;
        public float From;
        public float To;
      }
    }
  }
}

