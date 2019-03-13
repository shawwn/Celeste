// Decompiled with JetBrains decompiler
// Type: Monocle.ErrorLog
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Monocle
{
  public static class ErrorLog
  {
    public const string Filename = "error_log.txt";
    public const string Marker = "==========================================";

    public static void Write(Exception e)
    {
      ErrorLog.Write(e.ToString());
    }

    public static void Write(string str)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str1 = "";
      if (File.Exists("error_log.txt"))
      {
        StreamReader streamReader = new StreamReader("error_log.txt");
        str1 = streamReader.ReadToEnd();
        streamReader.Close();
        if (!str1.Contains("=========================================="))
          str1 = "";
      }
      if (Engine.Instance != null)
        stringBuilder.Append(Engine.Instance.Title);
      else
        stringBuilder.Append("Monocle Engine");
      stringBuilder.AppendLine(" Error Log");
      stringBuilder.AppendLine("==========================================");
      stringBuilder.AppendLine();
      if (Engine.Instance != null && Engine.Instance.Version != (Version) null)
      {
        stringBuilder.Append("Ver ");
        stringBuilder.AppendLine(Engine.Instance.Version.ToString());
      }
      stringBuilder.AppendLine(DateTime.Now.ToString());
      stringBuilder.AppendLine(str);
      if (str1 != "")
      {
        int startIndex = str1.IndexOf("==========================================") + "==========================================".Length;
        string str2 = str1.Substring(startIndex);
        stringBuilder.AppendLine(str2);
      }
      StreamWriter streamWriter = new StreamWriter("error_log.txt", false);
      streamWriter.Write(stringBuilder.ToString());
      streamWriter.Close();
    }

    public static void Open()
    {
      if (!File.Exists("error_log.txt"))
        return;
      Process.Start("error_log.txt");
    }
  }
}
