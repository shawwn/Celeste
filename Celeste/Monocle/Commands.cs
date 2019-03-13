// Decompiled with JetBrains decompiler
// Type: Monocle.Commands
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Monocle
{
  public class Commands
  {
    public bool Enabled = true;
    private string currentText = "";
    private int seekIndex = -1;
    private int tabIndex = -1;
    private float repeatCounter = 0.0f;
    private Keys? repeatKey = new Keys?();
    private const float UNDERSCORE_TIME = 0.5f;
    private const float REPEAT_DELAY = 0.5f;
    private const float REPEAT_EVERY = 0.03333334f;
    private const float OPACITY = 0.8f;
    public bool Open;
    private Dictionary<string, Commands.CommandInfo> commands;
    private List<string> sorted;
    private KeyboardState oldState;
    private KeyboardState currentState;
    private List<Commands.Line> drawCommands;
    private bool underscore;
    private float underscoreCounter;
    private List<string> commandHistory;
    private string tabSearch;
    private bool canOpen;

    public Action[] FunctionKeyActions { get; private set; }

    public Commands()
    {
      this.commandHistory = new List<string>();
      this.drawCommands = new List<Commands.Line>();
      this.commands = new Dictionary<string, Commands.CommandInfo>();
      this.sorted = new List<string>();
      this.FunctionKeyActions = new Action[12];
      this.BuildCommandsList();
    }

    public void Log(object obj, Color color)
    {
      string text = obj.ToString();
      if (text.Contains("\n"))
      {
        string str = text;
        char[] chArray = new char[1]{ '\n' };
        foreach (object obj1 in str.Split(chArray))
          this.Log(obj1, color);
      }
      else
      {
        int length1;
        for (int index = Engine.Instance.Window.ClientBounds.Width - 40; (double) Draw.DefaultFont.MeasureString(text).X > (double) index; text = text.Substring(length1 + 1))
        {
          length1 = -1;
          for (int length2 = 0; length2 < text.Length; ++length2)
          {
            if (text[length2] == ' ')
            {
              if ((double) Draw.DefaultFont.MeasureString(text.Substring(0, length2)).X <= (double) index)
                length1 = length2;
              else
                break;
            }
          }
          if (length1 != -1)
            this.drawCommands.Insert(0, new Commands.Line(text.Substring(0, length1), color));
          else
            break;
        }
        this.drawCommands.Insert(0, new Commands.Line(text, color));
        int num = (Engine.Instance.Window.ClientBounds.Height - 100) / 30;
        while (this.drawCommands.Count > num)
          this.drawCommands.RemoveAt(this.drawCommands.Count - 1);
      }
    }

    public void Log(object obj)
    {
      this.Log(obj, Color.White);
    }

    internal void UpdateClosed()
    {
      if (!this.canOpen)
        this.canOpen = true;
      else if (MInput.Keyboard.Pressed(Keys.OemTilde, Keys.Oem8))
      {
        this.Open = true;
        this.currentState = Keyboard.GetState();
      }
      for (int num = 0; num < this.FunctionKeyActions.Length; ++num)
      {
        if (MInput.Keyboard.Pressed((Keys) (112 + num)))
          this.ExecuteFunctionKeyAction(num);
      }
    }

    internal void UpdateOpen()
    {
      this.oldState = this.currentState;
      this.currentState = Keyboard.GetState();
      this.underscoreCounter += Engine.DeltaTime;
      while ((double) this.underscoreCounter >= 0.5)
      {
        this.underscoreCounter -= 0.5f;
        this.underscore = !this.underscore;
      }
      if (this.repeatKey.HasValue)
      {
        if (this.currentState[this.repeatKey.Value] == KeyState.Down)
        {
          for (this.repeatCounter += Engine.DeltaTime; (double) this.repeatCounter >= 0.5; this.repeatCounter -= 0.03333334f)
            this.HandleKey(this.repeatKey.Value);
        }
        else
          this.repeatKey = new Keys?();
      }
      foreach (Keys pressedKey in this.currentState.GetPressedKeys())
      {
        if (this.oldState[pressedKey] == KeyState.Up)
        {
          this.HandleKey(pressedKey);
          break;
        }
      }
    }

    private void HandleKey(Keys key)
    {
      if (key != Keys.Tab && key != Keys.LeftShift && (key != Keys.RightShift && key != Keys.RightAlt) && (key != Keys.LeftAlt && key != Keys.RightControl) && key != Keys.LeftControl)
        this.tabIndex = -1;
      int num;
      if (key != Keys.OemTilde && key != Keys.Oem8 && key != Keys.Enter)
      {
        Keys? repeatKey = this.repeatKey;
        Keys keys = key;
        num = !(repeatKey.GetValueOrDefault() == keys & repeatKey.HasValue) ? 1 : 0;
      }
      else
        num = 0;
      if (num != 0)
      {
        this.repeatKey = new Keys?(key);
        this.repeatCounter = 0.0f;
      }
      Keys keys1 = key;
      if (keys1 <= Keys.Enter)
      {
        if (keys1 != Keys.Back)
        {
          if (keys1 != Keys.Tab)
          {
            if (keys1 == Keys.Enter)
            {
              if (this.currentText.Length <= 0)
                return;
              this.EnterCommand();
              return;
            }
          }
          else
          {
            if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
            {
              if (this.tabIndex == -1)
              {
                this.tabSearch = this.currentText;
                this.FindLastTab();
              }
              else
              {
                --this.tabIndex;
                if (this.tabIndex < 0 || this.tabSearch != "" && (uint) this.sorted[this.tabIndex].IndexOf(this.tabSearch) > 0U)
                  this.FindLastTab();
              }
            }
            else if (this.tabIndex == -1)
            {
              this.tabSearch = this.currentText;
              this.FindFirstTab();
            }
            else
            {
              ++this.tabIndex;
              if (this.tabIndex >= this.sorted.Count || this.tabSearch != "" && (uint) this.sorted[this.tabIndex].IndexOf(this.tabSearch) > 0U)
                this.FindFirstTab();
            }
            if (this.tabIndex == -1)
              return;
            this.currentText = this.sorted[this.tabIndex];
            return;
          }
        }
        else
        {
          if (this.currentText.Length <= 0)
            return;
          this.currentText = this.currentText.Substring(0, this.currentText.Length - 1);
          return;
        }
      }
      else if (keys1 <= Keys.F12)
      {
        switch (keys1 - 32)
        {
          case Keys.None:
            this.currentText += " ";
            return;
          case (Keys) 1:
          case (Keys) 2:
          case (Keys) 3:
          case (Keys) 4:
          case (Keys) 5:
          case (Keys) 7:
          case Keys.Tab:
          case (Keys) 10:
          case (Keys) 11:
          case (Keys) 12:
          case Keys.Enter:
          case (Keys) 15:
            break;
          case (Keys) 6:
            if (this.seekIndex >= this.commandHistory.Count - 1)
              return;
            ++this.seekIndex;
            this.currentText = string.Join(" ", this.commandHistory[this.seekIndex]);
            return;
          case Keys.Back:
            if (this.seekIndex <= -1)
              return;
            --this.seekIndex;
            if (this.seekIndex == -1)
              this.currentText = "";
            else
              this.currentText = string.Join(" ", this.commandHistory[this.seekIndex]);
            return;
          case (Keys) 14:
            this.currentText = "";
            return;
          case (Keys) 16:
            if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
            {
              this.currentText += ")";
              return;
            }
            this.currentText += "0";
            return;
          case (Keys) 17:
            if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
            {
              this.currentText += "!";
              return;
            }
            this.currentText += "1";
            return;
          case (Keys) 18:
            if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
            {
              this.currentText += "@";
              return;
            }
            this.currentText += "2";
            return;
          case Keys.Pause:
            if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
            {
              this.currentText += "#";
              return;
            }
            this.currentText += "3";
            return;
          case Keys.CapsLock:
            if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
            {
              this.currentText += "$";
              return;
            }
            this.currentText += "4";
            return;
          case Keys.Kana:
            if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
            {
              this.currentText += "%";
              return;
            }
            this.currentText += "5";
            return;
          case (Keys) 22:
            if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
            {
              this.currentText += "^";
              return;
            }
            this.currentText += "6";
            return;
          case (Keys) 23:
            if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
            {
              this.currentText += "&";
              return;
            }
            this.currentText += "7";
            return;
          case (Keys) 24:
            if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
            {
              this.currentText += "*";
              return;
            }
            this.currentText += "8";
            return;
          case Keys.Kanji:
            if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
            {
              this.currentText += "(";
              return;
            }
            this.currentText += "9";
            return;
          default:
            if ((uint) (keys1 - 112) <= 11U)
            {
              this.ExecuteFunctionKeyAction((int) (key - 112));
              return;
            }
            break;
        }
      }
      else
      {
        switch (keys1 - 186)
        {
          case Keys.None:
            if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
            {
              this.currentText += ":";
              return;
            }
            this.currentText += ";";
            return;
          case (Keys) 1:
            if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
            {
              this.currentText += "+";
              return;
            }
            this.currentText += "=";
            return;
          case (Keys) 2:
            if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
            {
              this.currentText += "<";
              return;
            }
            this.currentText += ",";
            return;
          case (Keys) 3:
            if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
            {
              this.currentText += "_";
              return;
            }
            this.currentText += "-";
            return;
          case (Keys) 4:
            if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
            {
              this.currentText += ">";
              return;
            }
            this.currentText += ".";
            return;
          case (Keys) 5:
            if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
            {
              this.currentText += "?";
              return;
            }
            this.currentText += "/";
            return;
          case (Keys) 6:
label_107:
            this.Open = this.canOpen = false;
            return;
          default:
            switch (keys1 - 219)
            {
              case Keys.None:
                if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
                {
                  this.currentText += "{";
                  return;
                }
                this.currentText += "[";
                return;
              case (Keys) 2:
                if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
                {
                  this.currentText += "}";
                  return;
                }
                this.currentText += "]";
                return;
              case (Keys) 3:
                if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
                {
                  this.currentText += "\"";
                  return;
                }
                this.currentText += "'";
                return;
              case (Keys) 4:
                goto label_107;
              case (Keys) 7:
                if (this.currentState[Keys.LeftShift] == KeyState.Down || this.currentState[Keys.RightShift] == KeyState.Down)
                {
                  this.currentText += "|";
                  return;
                }
                this.currentText += "\\";
                return;
            }
            break;
        }
      }
      if (key.ToString().Length != 1)
        return;
      this.currentText = this.currentState[Keys.LeftShift] != KeyState.Down && this.currentState[Keys.RightShift] != KeyState.Down ? this.currentText + key.ToString().ToLower() : this.currentText + key.ToString();
    }

    private void EnterCommand()
    {
      string[] strArray = this.currentText.Split(new char[2]
      {
        ' ',
        ','
      }, StringSplitOptions.RemoveEmptyEntries);
      if (this.commandHistory.Count == 0 || this.commandHistory[0] != this.currentText)
        this.commandHistory.Insert(0, this.currentText);
      this.drawCommands.Insert(0, new Commands.Line(this.currentText, Color.Aqua));
      this.currentText = "";
      this.seekIndex = -1;
      string[] args = new string[strArray.Length - 1];
      for (int index = 1; index < strArray.Length; ++index)
        args[index - 1] = strArray[index];
      this.ExecuteCommand(strArray[0].ToLower(), args);
    }

    private void FindFirstTab()
    {
      for (int index = 0; index < this.sorted.Count; ++index)
      {
        if (this.tabSearch == "" || this.sorted[index].IndexOf(this.tabSearch) == 0)
        {
          this.tabIndex = index;
          break;
        }
      }
    }

    private void FindLastTab()
    {
      for (int index = 0; index < this.sorted.Count; ++index)
      {
        if (this.tabSearch == "" || this.sorted[index].IndexOf(this.tabSearch) == 0)
          this.tabIndex = index;
      }
    }

    internal void Render()
    {
      int viewWidth = Engine.ViewWidth;
      int viewHeight = Engine.ViewHeight;
      Draw.SpriteBatch.Begin();
      Draw.Rect(10f, (float) (viewHeight - 50), (float) (viewWidth - 20), 40f, Color.Black * 0.8f);
      if (this.underscore)
        Draw.SpriteBatch.DrawString(Draw.DefaultFont, ">" + this.currentText + "_", new Vector2(20f, (float) (viewHeight - 42)), Color.White);
      else
        Draw.SpriteBatch.DrawString(Draw.DefaultFont, ">" + this.currentText, new Vector2(20f, (float) (viewHeight - 42)), Color.White);
      if (this.drawCommands.Count > 0)
      {
        int num = 10 + 30 * this.drawCommands.Count;
        Draw.Rect(10f, (float) (viewHeight - num - 60), (float) (viewWidth - 20), (float) num, Color.Black * 0.8f);
        for (int index = 0; index < this.drawCommands.Count; ++index)
          Draw.SpriteBatch.DrawString(Draw.DefaultFont, this.drawCommands[index].Text, new Vector2(20f, (float) (viewHeight - 92 - 30 * index)), this.drawCommands[index].Color);
      }
      Draw.SpriteBatch.End();
    }

    public void ExecuteCommand(string command, string[] args)
    {
      if (this.commands.ContainsKey(command))
        this.commands[command].Action(args);
      else
        this.Log((object) ("Command '" + command + "' not found! Type 'help' for list of commands"), Color.Yellow);
    }

    public void ExecuteFunctionKeyAction(int num)
    {
      if (this.FunctionKeyActions[num] == null)
        return;
      this.FunctionKeyActions[num]();
    }

    private void BuildCommandsList()
    {
      foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
      {
        foreach (MethodInfo method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
          this.ProcessMethod(method);
      }
      foreach (Type type in Assembly.GetEntryAssembly().GetTypes())
      {
        foreach (MethodInfo method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
          this.ProcessMethod(method);
      }
      foreach (KeyValuePair<string, Commands.CommandInfo> command in this.commands)
        this.sorted.Add(command.Key);
      this.sorted.Sort();
    }

    private void ProcessMethod(MethodInfo method)
    {
      Command command = (Command) null;
      object[] customAttributes = method.GetCustomAttributes(typeof (Command), false);
      if ((uint) customAttributes.Length > 0U)
        command = customAttributes[0] as Command;
      if (command == null)
        return;
      if (!method.IsStatic)
        throw new Exception(method.DeclaringType.Name + "." + method.Name + " is marked as a command, but is not static");
      Commands.CommandInfo commandInfo = new Commands.CommandInfo();
      commandInfo.Help = command.Help;
      ParameterInfo[] parameters = method.GetParameters();
      object[] defaults = new object[parameters.Length];
      string[] strArray = new string[parameters.Length];
      for (int index = 0; index < parameters.Length; ++index)
      {
        ParameterInfo parameterInfo = parameters[index];
        strArray[index] = parameterInfo.Name + ":";
        if (parameterInfo.ParameterType == typeof (string))
        {
          strArray[index] += "string";
        }
        else if (parameterInfo.ParameterType == typeof (int))
        {
          strArray[index] += "int";
        }
        else if (parameterInfo.ParameterType == typeof (float))
        {
          strArray[index] += "float";
        }
        else
        {
          if (!(parameterInfo.ParameterType == typeof (bool)))
            throw new Exception(method.DeclaringType.Name + "." + method.Name + " is marked as a command, but has an invalid parameter type. Allowed types are: string, int, float, and bool");
          strArray[index] += "bool";
        }
        if (parameterInfo.DefaultValue == DBNull.Value)
          defaults[index] = (object) null;
        else if (parameterInfo.DefaultValue != null)
        {
          defaults[index] = parameterInfo.DefaultValue;
          if (parameterInfo.ParameterType == typeof (string))
          {
            ref string local = ref strArray[index];
            local = local + "=\"" + parameterInfo.DefaultValue + "\"";
          }
          else
          {
            ref string local = ref strArray[index];
            local = local + "=" + parameterInfo.DefaultValue;
          }
        }
        else
          defaults[index] = (object) null;
      }
      commandInfo.Usage = strArray.Length != 0 ? "[" + string.Join(" ", strArray) + "]" : "";
      commandInfo.Action = (Action<string[]>) (args =>
      {
        if (parameters.Length == 0)
        {
          this.InvokeMethod(method, (object[]) null);
        }
        else
        {
          object[] objArray = (object[]) defaults.Clone();
          for (int index = 0; index < objArray.Length && index < args.Length; ++index)
          {
            if (parameters[index].ParameterType == typeof (string))
              objArray[index] = (object) Commands.ArgString(args[index]);
            else if (parameters[index].ParameterType == typeof (int))
              objArray[index] = (object) Commands.ArgInt(args[index]);
            else if (parameters[index].ParameterType == typeof (float))
              objArray[index] = (object) Commands.ArgFloat(args[index]);
            else if (parameters[index].ParameterType == typeof (bool))
              objArray[index] = (object) Commands.ArgBool(args[index]);
          }
          this.InvokeMethod(method, objArray);
        }
      });
      this.commands[command.Name] = commandInfo;
    }

    private void InvokeMethod(MethodInfo method, object[] param = null)
    {
      try
      {
        method.Invoke((object) null, param);
      }
      catch (Exception ex)
      {
        Engine.Commands.Log((object) ex.InnerException.Message, Color.Yellow);
        this.LogStackTrace(ex.InnerException.StackTrace);
      }
    }

    private void LogStackTrace(string stackTrace)
    {
      string str1 = stackTrace;
      char[] chArray = new char[1]{ '\n' };
      foreach (string str2_ in str1.Split(chArray))
      {
        string str2 = str2_;
        int length1 = str2.LastIndexOf(" in ") + 4;
        int startIndex1 = str2.LastIndexOf('\\') + 1;
        if (length1 != -1 && startIndex1 != -1)
          str2 = str2.Substring(0, length1) + str2.Substring(startIndex1);
        int length2 = str2.IndexOf('(') + 1;
        int startIndex2 = str2.IndexOf(')');
        if (length2 != -1 && startIndex2 != -1)
          str2 = str2.Substring(0, length2) + str2.Substring(startIndex2);
        int startIndex3 = str2.LastIndexOf(':');
        if (startIndex3 != -1)
          str2 = str2.Insert(startIndex3 + 1, " ").Insert(startIndex3, " ");
        Engine.Commands.Log((object) ("-> " + str2.TrimStart()), Color.White);
      }
    }

    private static string ArgString(string arg)
    {
      if (arg == null)
        return "";
      return arg;
    }

    private static bool ArgBool(string arg)
    {
      if (arg != null)
        return !(arg == "0") && !(arg.ToLower() == "false") && !(arg.ToLower() == "f");
      return false;
    }

    private static int ArgInt(string arg)
    {
      try
      {
        return Convert.ToInt32(arg);
      }
      catch
      {
        return 0;
      }
    }

    private static float ArgFloat(string arg)
    {
      try
      {
        return Convert.ToSingle(arg, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      catch
      {
        return 0.0f;
      }
    }

    [Command("clear", "Clears the terminal")]
    public static void Clear()
    {
      Engine.Commands.drawCommands.Clear();
    }

    [Command("exit", "Exits the game")]
    private static void Exit()
    {
      Engine.Instance.Exit();
    }

    [Command("vsync", "Enables or disables vertical sync")]
    private static void Vsync(bool enabled = true)
    {
      Engine.Graphics.SynchronizeWithVerticalRetrace = enabled;
      Engine.Graphics.ApplyChanges();
      Engine.Commands.Log((object) ("Vertical Sync " + (enabled ? "Enabled" : "Disabled")));
    }

    [Command("count", "Logs amount of Entities in the Scene. Pass a tagIndex to count only Entities with that tag")]
    private static void Count(int tagIndex = -1)
    {
      if (Engine.Scene == null)
        Engine.Commands.Log((object) "Current Scene is null!");
      else if (tagIndex < 0)
        Engine.Commands.Log((object) Engine.Scene.Entities.Count.ToString());
      else
        Engine.Commands.Log((object) Engine.Scene.TagLists[tagIndex].Count.ToString());
    }

    [Command("tracker", "Logs all tracked objects in the scene. Set mode to 'e' for just entities, or 'c' for just components")]
    private static void Tracker(string mode)
    {
      if (Engine.Scene == null)
      {
        Engine.Commands.Log((object) "Current Scene is null!");
      }
      else
      {
        string str = mode;
        if (!(str == "e"))
        {
          if (!(str == "c"))
          {
            Engine.Commands.Log((object) "-- Entities --");
            Engine.Scene.Tracker.LogEntities();
            Engine.Commands.Log((object) "-- Components --");
            Engine.Scene.Tracker.LogComponents();
          }
          else
            Engine.Scene.Tracker.LogComponents();
        }
        else
          Engine.Scene.Tracker.LogEntities();
      }
    }

    [Command("pooler", "Logs the pooled Entity counts")]
    private static void Pooler()
    {
      Engine.Pooler.Log();
    }

    [Command("fullscreen", "Switches to fullscreen mode")]
    private static void Fullscreen()
    {
      Engine.SetFullscreen();
    }

    [Command("window", "Switches to window mode")]
    private static void Window(int scale = 1)
    {
      Engine.SetWindowed(320 * scale, 180 * scale);
    }

    [Command("help", "Shows usage help for a given command")]
    private static void Help(string command)
    {
      if (Engine.Commands.sorted.Contains(command))
      {
        Commands.CommandInfo command1 = Engine.Commands.commands[command];
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(":: ");
        stringBuilder.Append(command);
        if (!string.IsNullOrEmpty(command1.Usage))
        {
          stringBuilder.Append(" ");
          stringBuilder.Append(command1.Usage);
        }
        Engine.Commands.Log((object) stringBuilder.ToString());
        if (string.IsNullOrEmpty(command1.Help))
          Engine.Commands.Log((object) "No help info set");
        else
          Engine.Commands.Log((object) command1.Help);
      }
      else
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("Commands list: ");
        stringBuilder.Append(string.Join(", ", (IEnumerable<string>) Engine.Commands.sorted));
        Engine.Commands.Log((object) stringBuilder.ToString());
        Engine.Commands.Log((object) "Type 'help command' for more info on that command!");
      }
    }

    private struct CommandInfo
    {
      public Action<string[]> Action;
      public string Help;
      public string Usage;
    }

    private struct Line
    {
      public string Text;
      public Color Color;

      public Line(string text)
      {
        this.Text = text;
        this.Color = Color.White;
      }

      public Line(string text, Color color)
      {
        this.Text = text;
        this.Color = color;
      }
    }
  }
}

