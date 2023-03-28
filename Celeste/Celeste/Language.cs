// Decompiled with JetBrains decompiler
// Type: Celeste.Language
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Monocle;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Celeste
{
  public class Language
  {
    public string FilePath;
    public string Id;
    public string Label;
    public string IconPath;
    public MTexture Icon;
    public int Order = 100;
    public string FontFace;
    public float FontFaceSize;
    public string SplitRegex = "(\\s|\\{|\\})";
    public string CommaCharacters = ",";
    public string PeriodCharacters = ".!?";
    public int Lines;
    public int Words;
    public Dictionary<string, string> Dialog = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> Cleaned = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static readonly Regex command = new Regex("\\{(.*?)\\}", RegexOptions.RightToLeft);
    private static readonly Regex insert = new Regex("\\{\\+\\s*(.*?)\\}");
    private static readonly Regex variable = new Regex("^\\w+\\=.*");
    private static readonly Regex portrait = new Regex("\\[(?<content>[^\\[\\\\]*(?:\\\\.[^\\]\\\\]*)*)\\]", RegexOptions.IgnoreCase);

    public PixelFont Font => Fonts.Get(this.FontFace);

    public PixelFontSize FontSize => this.Font.Get(this.FontFaceSize);

    public string this[string name] => this.Dialog[name];

    public bool CanDisplay(string text)
    {
      PixelFontSize fontSize = this.FontSize;
      for (int index = 0; index < text.Length; ++index)
      {
        if (text[index] != ' ' && fontSize.Get((int) text[index]) == null)
          return false;
      }
      return true;
    }

    public void Export(string path)
    {
      using (BinaryWriter binaryWriter = new BinaryWriter((Stream) File.OpenWrite(path)))
      {
        binaryWriter.Write(this.Id);
        binaryWriter.Write(this.Label);
        binaryWriter.Write(this.IconPath);
        binaryWriter.Write(this.Order);
        binaryWriter.Write(this.FontFace);
        binaryWriter.Write(this.FontFaceSize);
        binaryWriter.Write(this.SplitRegex);
        binaryWriter.Write(this.CommaCharacters);
        binaryWriter.Write(this.PeriodCharacters);
        binaryWriter.Write(this.Lines);
        binaryWriter.Write(this.Words);
        binaryWriter.Write(this.Dialog.Count);
        foreach (KeyValuePair<string, string> keyValuePair in this.Dialog)
        {
          binaryWriter.Write(keyValuePair.Key);
          binaryWriter.Write(keyValuePair.Value);
          binaryWriter.Write(this.Cleaned[keyValuePair.Key]);
        }
      }
    }

    public static Language FromExport(string path)
    {
      Language language = new Language();
      using (BinaryReader binaryReader = new BinaryReader((Stream) File.OpenRead(path)))
      {
        language.Id = binaryReader.ReadString();
        language.Label = binaryReader.ReadString();
        language.IconPath = binaryReader.ReadString();
        language.Icon = new MTexture(VirtualContent.CreateTexture(Path.Combine("Dialog", language.IconPath)));
        language.Order = binaryReader.ReadInt32();
        language.FontFace = binaryReader.ReadString();
        language.FontFaceSize = binaryReader.ReadSingle();
        language.SplitRegex = binaryReader.ReadString();
        language.CommaCharacters = binaryReader.ReadString();
        language.PeriodCharacters = binaryReader.ReadString();
        language.Lines = binaryReader.ReadInt32();
        language.Words = binaryReader.ReadInt32();
        int num = binaryReader.ReadInt32();
        for (int index = 0; index < num; ++index)
        {
          string key = binaryReader.ReadString();
          language.Dialog[key] = binaryReader.ReadString();
          language.Cleaned[key] = binaryReader.ReadString();
        }
      }
      return language;
    }

    public static Language FromTxt(string path)
    {
      Language language = (Language) null;
      string key1 = "";
      StringBuilder stringBuilder = new StringBuilder();
      string input1 = "";
      foreach (string readLine in File.ReadLines(path, Encoding.UTF8))
      {
        string input2 = readLine.Trim();
        if (input2.Length > 0 && input2[0] != '#')
        {
          if (input2.IndexOf('[') >= 0)
            input2 = Language.portrait.Replace(input2, "{portrait ${content}}");
          string input3 = input2.Replace("\\#", "#");
          if (input3.Length > 0)
          {
            if (Language.variable.IsMatch(input3))
            {
              if (!string.IsNullOrEmpty(key1))
                language.Dialog[key1] = stringBuilder.ToString();
              string[] strArray1 = input3.Split('=');
              string str1 = strArray1[0].Trim();
              string str2 = strArray1.Length > 1 ? strArray1[1].Trim() : "";
              if (str1.Equals("language", StringComparison.OrdinalIgnoreCase))
              {
                string[] strArray2 = str2.Split(',');
                language = new Language();
                language.FontFace = (string) null;
                language.Id = strArray2[0];
                language.FilePath = Path.GetFileName(path);
                if (strArray2.Length > 1)
                  language.Label = strArray2[1];
              }
              else if (str1.Equals("icon", StringComparison.OrdinalIgnoreCase))
              {
                VirtualTexture texture = VirtualContent.CreateTexture(Path.Combine("Dialog", str2));
                language.IconPath = str2;
                language.Icon = new MTexture(texture);
              }
              else if (str1.Equals("order", StringComparison.OrdinalIgnoreCase))
                language.Order = int.Parse(str2);
              else if (str1.Equals("font", StringComparison.OrdinalIgnoreCase))
              {
                string[] strArray3 = str2.Split(',');
                language.FontFace = strArray3[0];
                language.FontFaceSize = float.Parse(strArray3[1], (IFormatProvider) CultureInfo.InvariantCulture);
              }
              else if (str1.Equals("SPLIT_REGEX", StringComparison.OrdinalIgnoreCase))
                language.SplitRegex = str2;
              else if (str1.Equals("commas", StringComparison.OrdinalIgnoreCase))
                language.CommaCharacters = str2;
              else if (str1.Equals("periods", StringComparison.OrdinalIgnoreCase))
              {
                language.PeriodCharacters = str2;
              }
              else
              {
                key1 = str1;
                stringBuilder.Clear();
                stringBuilder.Append(str2);
              }
            }
            else
            {
              if (stringBuilder.Length > 0)
              {
                string str = stringBuilder.ToString();
                if (!str.EndsWith("{break}") && !str.EndsWith("{n}") && Language.command.Replace(input1, "").Length > 0)
                  stringBuilder.Append("{break}");
              }
              stringBuilder.Append(input3);
            }
            input1 = input3;
          }
        }
      }
      if (!string.IsNullOrEmpty(key1))
        language.Dialog[key1] = stringBuilder.ToString();
      List<string> stringList = new List<string>();
      foreach (KeyValuePair<string, string> keyValuePair in language.Dialog)
        stringList.Add(keyValuePair.Key);
      foreach (string key2 in stringList)
      {
        string input4 = language.Dialog[key2];
        MatchCollection matchCollection = (MatchCollection) null;
        while (matchCollection == null || matchCollection.Count > 0)
        {
          matchCollection = Language.insert.Matches(input4);
          for (int i = 0; i < matchCollection.Count; ++i)
          {
            Match match = matchCollection[i];
            string key3 = match.Groups[1].Value;
            string newValue;
            input4 = !language.Dialog.TryGetValue(key3, out newValue) ? input4.Replace(match.Value, "[XXX]") : input4.Replace(match.Value, newValue);
          }
        }
        language.Dialog[key2] = input4;
      }
      language.Lines = 0;
      language.Words = 0;
      foreach (string key4 in stringList)
      {
        string str = language.Dialog[key4];
        if (str.IndexOf('{') >= 0)
        {
          string input5 = str.Replace("{n}", "\n").Replace("{break}", "\n");
          str = Language.command.Replace(input5, "");
        }
        language.Cleaned.Add(key4, str);
      }
      return language;
    }

    public void Dispose()
    {
      if (this.Icon.Texture == null || this.Icon.Texture.IsDisposed)
        return;
      this.Icon.Texture.Dispose();
    }
  }
}
