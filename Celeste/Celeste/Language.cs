// Decompiled with JetBrains decompiler
// Type: Celeste.Language
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class Language
  {
    public int Order = 100;
    public string SplitRegex = "(\\s|\\{|\\})";
    public string CommaCharacters = ",";
    public string PeriodCharacters = ".!?";
    public Dictionary<string, string> Dialog = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> Cleaned = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    public string Id;
    public string Label;
    public MTexture Icon;
    public string FontFace;
    public float FontFaceSize;
    public bool Initialized;
    public int Lines;
    public int Words;

    public PixelFont Font
    {
      get
      {
        return Fonts.Faces[this.FontFace];
      }
    }

    public PixelFontSize FontSize
    {
      get
      {
        return this.Font.Get(this.FontFaceSize);
      }
    }

    public string this[string name]
    {
      get
      {
        return this.Dialog[name];
      }
    }

    public void Dispose()
    {
      if (this.Icon.Texture == null || this.Icon.Texture.IsDisposed)
        return;
      this.Icon.Texture.Dispose();
    }
  }
}
