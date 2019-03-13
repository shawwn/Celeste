// Decompiled with JetBrains decompiler
// Type: Celeste.UserIO
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;
using SDL2;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Celeste
{
  public static class UserIO
  {
    private static readonly string SavePath = UserIO.GetSavePath("Saves");
    private static readonly string BackupPath = UserIO.GetSavePath("Backups");
    public const string SaveDataTitle = "Celeste Save Data";
    private const string Extension = ".celeste";
    private static bool savingInternal;
    private static bool savingFile;
    private static bool savingSettings;
    private static byte[] savingFileData;
    private static byte[] savingSettingsData;

    private static string GetSavePath(string dir)
    {
      string platform = SDL.SDL_GetPlatform();
      if (platform.Equals("Linux") || platform.Equals("FreeBSD") || platform.Equals("OpenBSD") || platform.Equals("NetBSD"))
      {
        string environmentVariable1 = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
        if (!string.IsNullOrEmpty(environmentVariable1))
          return Path.Combine(environmentVariable1, "Celeste/" + dir);
        string environmentVariable2 = Environment.GetEnvironmentVariable("HOME");
        if (!string.IsNullOrEmpty(environmentVariable2))
          return Path.Combine(environmentVariable2, ".local/share/Celeste/" + dir);
      }
      else if (platform.Equals("Mac OS X"))
      {
        string environmentVariable = Environment.GetEnvironmentVariable("HOME");
        if (!string.IsNullOrEmpty(environmentVariable))
          return Path.Combine(environmentVariable, "Library/Application Support/Celeste/" + dir);
      }
      else if (!platform.Equals("Windows"))
        throw new NotSupportedException("Unhandled SDL2 platform!");
      return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir);
    }

    private static string GetHandle(string name)
    {
      return Path.Combine(UserIO.SavePath, name + ".celeste");
    }

    private static string GetBackupHandle(string name)
    {
      return Path.Combine(UserIO.BackupPath, name + ".celeste");
    }

    public static bool Open(UserIO.Mode mode)
    {
      return true;
    }

    public static bool Save<T>(string path, byte[] data) where T : class
    {
      string handle = UserIO.GetHandle(path);
      bool flag = false;
      try
      {
        string backupHandle = UserIO.GetBackupHandle(path);
        DirectoryInfo directory1 = new FileInfo(handle).Directory;
        if (!directory1.Exists)
          directory1.Create();
        DirectoryInfo directory2 = new FileInfo(backupHandle).Directory;
        if (!directory2.Exists)
          directory2.Create();
        using (FileStream fileStream = File.Open(backupHandle, FileMode.Create, FileAccess.Write))
          fileStream.Write(data, 0, data.Length);
        if ((object) UserIO.Load<T>(path, true) != null)
        {
          File.Copy(backupHandle, handle, true);
          flag = true;
        }
      }
      catch (Exception ex)
      {
        ErrorLog.Write(ex);
        Console.WriteLine("ERROR: " + ex.ToString());
      }
      if (!flag)
        Console.WriteLine("Save Failed");
      return flag;
    }

    public static T Load<T>(string path, bool backup = false) where T : class
    {
      string path1 = !backup ? UserIO.GetHandle(path) : UserIO.GetBackupHandle(path);
      T obj = default (T);
      try
      {
        if (File.Exists(path1))
        {
          using (FileStream fileStream = File.OpenRead(path1))
            obj = UserIO.Deserialize<T>((Stream) fileStream);
        }
      }
      catch (Exception ex)
      {
        ErrorLog.Write(ex);
        Console.WriteLine("ERROR: " + ex.ToString());
      }
      return obj;
    }

    private static T Deserialize<T>(Stream stream) where T : class
    {
      T obj = default (T);
      try
      {
        obj = (T) new XmlSerializer(typeof (T)).Deserialize(stream);
      }
      catch (Exception ex)
      {
      }
      if ((object) obj == null)
      {
        stream.Position = 0L;
        obj = (T) new BinaryFormatter().Deserialize(stream);
      }
      return obj;
    }

    public static bool Exists(string path)
    {
      return File.Exists(UserIO.GetHandle(path));
    }

    public static bool Delete(string path)
    {
      string handle = UserIO.GetHandle(path);
      if (!File.Exists(handle))
        return false;
      File.Delete(handle);
      return true;
    }

    public static void Close()
    {
    }

    public static byte[] Serialize<T>(T instance)
    {
      byte[] array;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        new XmlSerializer(typeof (T)).Serialize((Stream) memoryStream, (object) instance);
        array = memoryStream.ToArray();
      }
      return array;
    }

    public static bool Saving { get; private set; }

    public static bool SavingResult { get; private set; }

    public static void SaveHandler(bool file, bool settings)
    {
      if (UserIO.Saving)
        return;
      UserIO.Saving = true;
      Celeste.SaveRoutine = new Coroutine(UserIO.SaveRoutine(file, settings), true);
    }

    private static IEnumerator SaveRoutine(bool file, bool settings)
    {
      UserIO.savingFile = file;
      UserIO.savingSettings = settings;
      FileErrorOverlay menu;
      do
      {
        if (UserIO.savingFile)
        {
          SaveData.Instance.BeforeSave();
          UserIO.savingFileData = UserIO.Serialize<SaveData>(SaveData.Instance);
        }
        if (UserIO.savingSettings)
          UserIO.savingSettingsData = UserIO.Serialize<Settings>(Settings.Instance);
        UserIO.savingInternal = true;
        UserIO.SavingResult = false;
        RunThread.Start(new Action(UserIO.SaveThread), "USER_IO", false);
        SaveLoadIcon.Show(Engine.Scene);
        while (UserIO.savingInternal)
          yield return (object) null;
        SaveLoadIcon.Hide();
        if (!UserIO.SavingResult)
        {
          menu = new FileErrorOverlay(FileErrorOverlay.Error.Save);
          while (menu.Open)
            yield return (object) null;
        }
        else
          goto label_14;
      }
      while (menu.TryAgain);
      menu = (FileErrorOverlay) null;
label_14:
      UserIO.Saving = false;
      Celeste.SaveRoutine = (Coroutine) null;
    }

    private static void SaveThread()
    {
      UserIO.SavingResult = false;
      if (UserIO.Open(UserIO.Mode.Write))
      {
        UserIO.SavingResult = true;
        if (UserIO.savingFile)
          UserIO.SavingResult &= UserIO.Save<SaveData>(SaveData.GetFilename(), UserIO.savingFileData);
        if (UserIO.savingSettings)
          UserIO.SavingResult &= UserIO.Save<Settings>("settings", UserIO.savingSettingsData);
        UserIO.Close();
      }
      UserIO.savingInternal = false;
    }

    public enum Mode
    {
      Read,
      Write,
    }
  }
}

