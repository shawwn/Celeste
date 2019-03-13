// Decompiled with JetBrains decompiler
// Type: Celeste.Dialog
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Celeste
{
  public static class Dialog
  {
    public static Language Language = (Language) null;
    private static readonly Regex command = new Regex("\\{(.*?)\\}", RegexOptions.RightToLeft);
    private static readonly Regex insert = new Regex("\\{\\+\\s*(.*?)\\}");
    private static readonly Regex variable = new Regex("^\\w+\\=.*");
    private static readonly Regex portrait = new Regex("\\[(?<content>[^\\[\\\\]*(?:\\\\.[^\\]\\\\]*)*)\\]", RegexOptions.IgnoreCase);
    private static string[] LanguageDataVariables = new string[7]
    {
      "language",
      "icon",
      "order",
      "split_regex",
      "commas",
      "periods",
      "font"
    };
    public static Dictionary<string, Language> Languages;
    public static List<Language> OrderedLanguages;
    private const string path = "Dialog";

    public static void Load()
    {
      Dialog.Language = (Language) null;
      Dialog.Languages = new Dictionary<string, Language>();
      foreach (string file in Directory.GetFiles(Path.Combine(Engine.ContentDirectory, nameof (Dialog)), "*.txt", SearchOption.AllDirectories))
        Dialog.LoadLanguage(file);
      Dialog.InitLanguages();
      Dialog.OrderedLanguages = new List<Language>();
      foreach (KeyValuePair<string, Language> language in Dialog.Languages)
        Dialog.OrderedLanguages.Add(language.Value);
      Dialog.OrderedLanguages.Sort((Comparison<Language>) ((a, b) =>
      {
        if (a.Order != b.Order)
          return a.Order - b.Order;
        return a.Id.CompareTo(b.Id);
      }));
    }

    public static Language LoadLanguage(string filename)
    {
      Language language = (Language) null;
      string key = "";
      StringBuilder stringBuilder = new StringBuilder();
      string input1 = "";
      foreach (string readLine in File.ReadLines(filename, Encoding.UTF8))
      {
        string input2 = readLine.Trim();
        if (input2.Length > 0 && input2[0] != '#')
        {
          if (input2.IndexOf('[') >= 0)
            input2 = Dialog.portrait.Replace(input2, "{portrait ${content}}");
          string input3 = input2.Replace("\\#", "#");
          if (input3.Length > 0)
          {
            if (Dialog.variable.IsMatch(input3))
            {
              if (!string.IsNullOrEmpty(key) && !language.Dialog.ContainsKey(key))
                language.Dialog.Add(key, stringBuilder.ToString());
              string[] strArray1 = input3.Split('=');
              string str1 = strArray1[0].Trim();
              string str2 = strArray1.Length > 1 ? strArray1[1].Trim() : "";
              if (str1.Equals("language", StringComparison.OrdinalIgnoreCase))
              {
                string[] strArray2 = str2.Split(',');
                if (!Dialog.Languages.TryGetValue(strArray2[0], out language))
                {
                  language = new Language();
                  language.FontFace = (string) null;
                  language.Id = strArray2[0];
                  Dialog.Languages.Add(language.Id, language);
                }
                if (strArray2.Length > 1)
                  language.Label = strArray2[1];
              }
              else if (str1.Equals("icon", StringComparison.OrdinalIgnoreCase))
              {
                VirtualTexture texture = VirtualContent.CreateTexture(Path.Combine(nameof (Dialog), str2));
                language.Icon = new MTexture(texture);
              }
              else if (str1.Equals("order", StringComparison.OrdinalIgnoreCase))
                language.Order = int.Parse(str2);
              else if (str1.Equals("font", StringComparison.OrdinalIgnoreCase))
              {
                string[] strArray2 = str2.Split(',');
                language.FontFace = strArray2[0];
                language.FontFaceSize = float.Parse(strArray2[1], (IFormatProvider) CultureInfo.InvariantCulture);
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
                key = str1;
                stringBuilder.Clear();
                stringBuilder.Append(str2);
              }
            }
            else
            {
              if (stringBuilder.Length > 0)
              {
                string str = stringBuilder.ToString();
                if (!str.EndsWith("{break}") && !str.EndsWith("{n}") && Dialog.command.Replace(input1, "").Length > 0)
                  stringBuilder.Append("{break}");
              }
              stringBuilder.Append(input3);
            }
            input1 = input3;
          }
        }
      }
      if (!string.IsNullOrEmpty(key) && !language.Dialog.ContainsKey(key))
        language.Dialog.Add(key, stringBuilder.ToString());
      return language;
    }

    public static void InitLanguages()
    {
      foreach (KeyValuePair<string, Language> language1 in Dialog.Languages)
      {
        Language language2 = language1.Value;
        if (!language2.Initialized)
        {
          language2.Initialized = true;
          List<string> stringList = new List<string>();
          foreach (KeyValuePair<string, string> keyValuePair in language2.Dialog)
            stringList.Add(keyValuePair.Key);
          foreach (string index1 in stringList)
          {
            string input = language2.Dialog[index1];
            MatchCollection matchCollection = (MatchCollection) null;
            while (matchCollection == null || matchCollection.Count > 0)
            {
              matchCollection = Dialog.insert.Matches(input);
              for (int index2 = 0; index2 < matchCollection.Count; ++index2)
              {
                Match match = matchCollection[index2];
                string key = match.Groups[1].Value;
                string newValue;
                input = !language2.Dialog.TryGetValue(key, out newValue) ? input.Replace(match.Value, "[XXX]") : input.Replace(match.Value, newValue);
              }
            }
            language2.Dialog[index1] = input;
          }
          language2.Lines = 0;
          language2.Words = 0;
          foreach (string key in stringList)
          {
            string str = language2.Dialog[key];
            if (str.IndexOf('{') >= 0)
            {
              string input = str.Replace("{n}", "\n").Replace("{break}", "\n");
              str = Dialog.command.Replace(input, "");
            }
            language2.Cleaned.Add(key, str);
          }
        }
      }
      if (Settings.Instance != null && Settings.Instance.Language != null && Dialog.Languages.ContainsKey(Settings.Instance.Language))
        Dialog.Language = Dialog.Languages[Settings.Instance.Language];
      else if (Dialog.Languages.ContainsKey("english"))
      {
        Dialog.Language = Dialog.Languages["english"];
      }
      else
      {
        if (Dialog.Languages.Count <= 0)
          throw new Exception("Missing Language Files");
        Dialog.Language = Dialog.Languages.ElementAt<KeyValuePair<string, Language>>(0).Value;
      }
      Settings.Instance.Language = Dialog.Language.Id;
    }

    public static void Unload()
    {
      foreach (KeyValuePair<string, Language> language in Dialog.Languages)
        language.Value.Dispose();
      Dialog.Languages.Clear();
      Dialog.Language = (Language) null;
      Dialog.OrderedLanguages.Clear();
      Dialog.OrderedLanguages = (List<Language>) null;
    }

    public static bool Has(string name, Language language = null)
    {
      if (language == null)
        language = Dialog.Language;
      return language.Dialog.ContainsKey(name);
    }

    public static string Get(string name, Language language = null)
    {
      if (language == null)
        language = Dialog.Language;
      string str = "";
      if (language.Dialog.TryGetValue(name, out str))
        return str;
      return "XXX";
    }

    public static string Clean(string name, Language language = null)
    {
      if (language == null)
        language = Dialog.Language;
      string str = "";
      if (language.Cleaned.TryGetValue(name, out str))
        return str;
      return "XXX";
    }

    public static string Time(long ticks)
    {
      TimeSpan timeSpan = TimeSpan.FromTicks(ticks);
      if ((int) timeSpan.TotalHours > 0)
        return ((int) timeSpan.TotalHours).ToString() + timeSpan.ToString("\\:mm\\:ss\\.fff");
      return timeSpan.Minutes.ToString() + timeSpan.ToString("\\:ss\\.fff");
    }

    public static string FileTime(long ticks)
    {
      TimeSpan timeSpan = TimeSpan.FromTicks(ticks);
      if (timeSpan.TotalHours >= 1.0)
        return ((int) timeSpan.TotalHours).ToString() + timeSpan.ToString("\\:mm\\:ss\\.fff");
      return timeSpan.ToString("mm\\:ss\\.fff");
    }

    public static string Deaths(int deaths)
    {
      if (deaths > 999999)
        return ((float) deaths / 1000000f).ToString("0.00") + "m";
      if (deaths > 9999)
        return ((float) deaths / 1000f).ToString("0.0") + "k";
      return deaths.ToString();
    }

    public static void CheckCharacters()
    {
      foreach (string file in Directory.GetFiles(Path.Combine(Engine.ContentDirectory, nameof (Dialog)), "*.txt", SearchOption.AllDirectories))
      {
        HashSet<int> intSet = new HashSet<int>();
        foreach (string readLine in File.ReadLines(file, Encoding.UTF8))
        {
          for (int index = 0; index < readLine.Length; ++index)
          {
            if (!intSet.Contains((int) readLine[index]))
              intSet.Add((int) readLine[index]);
          }
        }
        List<int> intList = new List<int>();
        foreach (int num in intSet)
          intList.Add(num);
        intList.Sort();
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("chars=");
        int num1 = 0;
        Console.WriteLine("Characters of : " + file);
        int num2;
        for (int index1 = 0; index1 < intList.Count; index1 = num2 + 1)
        {
          bool flag = false;
          int index2;
          for (index2 = index1 + 1; index2 < intList.Count && intList[index2] == intList[index2 - 1] + 1; ++index2)
            flag = true;
          if (flag)
            stringBuilder.Append(intList[index1].ToString() + "-" + (object) intList[index2 - 1] + ",");
          else
            stringBuilder.Append(intList[index1].ToString() + ",");
          num2 = index2 - 1;
          ++num1;
          if (num1 >= 10)
          {
            num1 = 0;
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            Console.WriteLine(stringBuilder.ToString());
            stringBuilder.Clear();
            stringBuilder.Append("chars=");
          }
        }
        stringBuilder.Remove(stringBuilder.Length - 1, 1);
        Console.WriteLine(stringBuilder.ToString());
        Console.WriteLine();
      }
    }

    public static bool CheckLanguageFontCharacters(string a)
    {
      Language language = Dialog.Languages[a];
      bool flag = true;
      HashSet<int> intSet = new HashSet<int>();
      foreach (KeyValuePair<string, string> keyValuePair in language.Dialog)
      {
        for (int index = 0; index < keyValuePair.Value.Length; ++index)
        {
          int key = (int) keyValuePair.Value[index];
          if (!intSet.Contains(key) && !language.FontSize.Characters.ContainsKey(key))
          {
            intSet.Add(key);
            flag = false;
          }
        }
      }
      Console.WriteLine("FONT: " + a);
      if (intSet.Count > 0)
        Console.WriteLine(" - Missing Characters: " + string.Join<int>(",", (IEnumerable<int>) intSet));
      Console.WriteLine(" - OK: " + flag.ToString());
      Console.WriteLine();
      if (intSet.Count > 0)
      {
        string contents = "";
        foreach (int num in intSet)
          contents += ((char) num).ToString();
        File.WriteAllText(a + "-missing-debug.txt", contents);
      }
      return flag;
    }

    public static bool CompareLanguages(string a, string b)
    {
      Console.WriteLine("COMPARE: " + a + " -> " + b);
      Language language1 = Dialog.Languages[a];
      Language language2 = Dialog.Languages[b];
      bool flag = true;
      List<string> stringList1 = new List<string>();
      List<string> stringList2 = new List<string>();
      foreach (KeyValuePair<string, string> keyValuePair in language1.Dialog)
      {
        if (!language2.Dialog.ContainsKey(keyValuePair.Key))
        {
          stringList2.Add(keyValuePair.Key);
          flag = false;
        }
      }
      foreach (KeyValuePair<string, string> keyValuePair in language2.Dialog)
      {
        if (!language1.Dialog.ContainsKey(keyValuePair.Key))
        {
          stringList2.Add(keyValuePair.Key);
          flag = false;
        }
      }
      if (stringList1.Count > 0)
        Console.WriteLine(" - Missing from " + a + ": " + string.Join(", ", (IEnumerable<string>) stringList1));
      if (stringList2.Count > 0)
        Console.WriteLine(" - Missing from " + b + ": " + string.Join(", ", (IEnumerable<string>) stringList2));
      Func<string, List<List<string>>> func = (Func<string, List<List<string>>>) (text =>
      {
        List<List<string>> stringListList = new List<List<string>>();
        foreach (Capture match in Regex.Matches(text, "\\{([^}]*)\\}"))
        {
          string[] strArray = Regex.Split(match.Value, "(\\{|\\}|\\s)");
          List<string> stringList3 = new List<string>();
          foreach (string str in strArray)
          {
            if (!string.IsNullOrWhiteSpace(str) && str.Length > 0 && (str != "{" && str != "}"))
              stringList3.Add(str);
          }
          stringListList.Add(stringList3);
        }
        return stringListList;
      });
      foreach (KeyValuePair<string, string> keyValuePair in language1.Dialog)
      {
        if (language2.Dialog.ContainsKey(keyValuePair.Key))
        {
          List<List<string>> stringListList1 = func(keyValuePair.Value);
          List<List<string>> stringListList2 = func(language2.Dialog[keyValuePair.Key]);
          int index1 = 0;
          int index2 = 0;
          for (; index1 < stringListList1.Count; ++index1)
          {
            string str = stringListList1[index1][0];
            if (str == "portrait" || str == "trigger")
            {
              while (index2 < stringListList2.Count && stringListList2[index2][0] != str)
                ++index2;
              if (index2 >= stringListList2.Count)
              {
                Console.WriteLine(" - Command number mismatch in " + keyValuePair.Key + " in " + b);
                flag = false;
                index1 = stringListList1.Count;
              }
              else
              {
                if (str == "portrait")
                {
                  for (int index3 = 0; index3 < stringListList1[index1].Count; ++index3)
                  {
                    if (stringListList1[index1][index3] != stringListList2[index2][index3])
                    {
                      Console.WriteLine(" - Portrait in " + keyValuePair.Key + " is incorrect in " + b + " ({" + string.Join(" ", (IEnumerable<string>) stringListList1[index1]) + "} vs {" + string.Join(" ", (IEnumerable<string>) stringListList2[index2]) + "})");
                      flag = false;
                    }
                  }
                }
                else if (str == "trigger" && stringListList1[index1][1] != stringListList2[index2][1])
                {
                  Console.WriteLine(" - Trigger in " + keyValuePair.Key + " is incorrect in " + b + " (" + stringListList1[index1][1] + " vs " + stringListList2[index2][1] + ")");
                  flag = false;
                }
                ++index2;
              }
            }
          }
        }
      }
      Console.WriteLine(" - OK: " + flag.ToString());
      Console.WriteLine();
      return flag;
    }
  }
}
