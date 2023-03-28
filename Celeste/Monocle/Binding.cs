// Decompiled with JetBrains decompiler
// Type: Monocle.Binding
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Monocle
{
  [Serializable]
  public class Binding
  {
    public List<Keys> Keyboard = new List<Keys>();
    public List<Buttons> Controller = new List<Buttons>();
    [XmlIgnore]
    public List<Binding> ExclusiveFrom = new List<Binding>();

    public bool HasInput => this.Keyboard.Count > 0 || this.Controller.Count > 0;

    public bool Add(params Keys[] keys)
    {
      bool flag = false;
      Keys[] keysArray = keys;
label_9:
      for (int index = 0; index < keysArray.Length; ++index)
      {
        Keys key = keysArray[index];
        if (!this.Keyboard.Contains(key))
        {
          foreach (Binding binding in this.ExclusiveFrom)
          {
            if (binding.Needs(key))
              goto label_9;
          }
          this.Keyboard.Add(key);
          flag = true;
        }
      }
      return flag;
    }

    public bool Add(params Buttons[] buttons)
    {
      bool flag = false;
      Buttons[] buttonsArray = buttons;
label_9:
      for (int index = 0; index < buttonsArray.Length; ++index)
      {
        Buttons button = buttonsArray[index];
        if (!this.Controller.Contains(button))
        {
          foreach (Binding binding in this.ExclusiveFrom)
          {
            if (binding.Needs(button))
              goto label_9;
          }
          this.Controller.Add(button);
          flag = true;
        }
      }
      return flag;
    }

    public bool Needs(Buttons button)
    {
      if (!this.Controller.Contains(button))
        return false;
      if (this.Controller.Count <= 1)
        return true;
      if (!this.IsExclusive(button))
        return false;
      foreach (Buttons button1 in this.Controller)
      {
        if (button1 != button && this.IsExclusive(button1))
          return false;
      }
      return true;
    }

    public bool Needs(Keys key)
    {
      if (!this.Keyboard.Contains(key))
        return false;
      if (this.Keyboard.Count <= 1)
        return true;
      if (!this.IsExclusive(key))
        return false;
      foreach (Keys key1 in this.Keyboard)
      {
        if (key1 != key && this.IsExclusive(key1))
          return false;
      }
      return true;
    }

    public bool IsExclusive(Buttons button)
    {
      foreach (Binding binding in this.ExclusiveFrom)
      {
        if (binding.Controller.Contains(button))
          return false;
      }
      return true;
    }

    public bool IsExclusive(Keys key)
    {
      foreach (Binding binding in this.ExclusiveFrom)
      {
        if (binding.Keyboard.Contains(key))
          return false;
      }
      return true;
    }

    public bool ClearKeyboard()
    {
      if (this.ExclusiveFrom.Count > 0)
      {
        if (this.Keyboard.Count <= 1)
          return false;
        int index1 = 0;
        for (int index2 = 1; index2 < this.Keyboard.Count; ++index2)
        {
          if (this.IsExclusive(this.Keyboard[index2]))
            index1 = index2;
        }
        Keys keys = this.Keyboard[index1];
        this.Keyboard.Clear();
        this.Keyboard.Add(keys);
      }
      else
        this.Keyboard.Clear();
      return true;
    }

    public bool ClearGamepad()
    {
      if (this.ExclusiveFrom.Count > 0)
      {
        if (this.Controller.Count <= 1)
          return false;
        int index1 = 0;
        for (int index2 = 1; index2 < this.Controller.Count; ++index2)
        {
          if (this.IsExclusive(this.Controller[index2]))
            index1 = index2;
        }
        Buttons buttons = this.Controller[index1];
        this.Controller.Clear();
        this.Controller.Add(buttons);
      }
      else
        this.Controller.Clear();
      return true;
    }

    public float Axis(int gamepadIndex, float threshold)
    {
      foreach (Keys key in this.Keyboard)
      {
        if (MInput.Keyboard.Check(key))
          return 1f;
      }
      foreach (Buttons button in this.Controller)
      {
        float num = MInput.GamePads[gamepadIndex].Axis(button, threshold);
        if ((double) num != 0.0)
          return num;
      }
      return 0.0f;
    }

    public bool Check(int gamepadIndex, float threshold)
    {
      for (int index = 0; index < this.Keyboard.Count; ++index)
      {
        if (MInput.Keyboard.Check(this.Keyboard[index]))
          return true;
      }
      for (int index = 0; index < this.Controller.Count; ++index)
      {
        if (MInput.GamePads[gamepadIndex].Check(this.Controller[index], threshold))
          return true;
      }
      return false;
    }

    public bool Pressed(int gamepadIndex, float threshold)
    {
      for (int index = 0; index < this.Keyboard.Count; ++index)
      {
        if (MInput.Keyboard.Pressed(this.Keyboard[index]))
          return true;
      }
      for (int index = 0; index < this.Controller.Count; ++index)
      {
        if (MInput.GamePads[gamepadIndex].Pressed(this.Controller[index], threshold))
          return true;
      }
      return false;
    }

    public bool Released(int gamepadIndex, float threshold)
    {
      for (int index = 0; index < this.Keyboard.Count; ++index)
      {
        if (MInput.Keyboard.Released(this.Keyboard[index]))
          return true;
      }
      for (int index = 0; index < this.Controller.Count; ++index)
      {
        if (MInput.GamePads[gamepadIndex].Released(this.Controller[index], threshold))
          return true;
      }
      return false;
    }

    public static void SetExclusive(params Binding[] list)
    {
      foreach (Binding binding in list)
        binding.ExclusiveFrom.Clear();
      foreach (Binding binding1 in list)
      {
        foreach (Binding binding2 in list)
        {
          if (binding1 != binding2)
          {
            binding1.ExclusiveFrom.Add(binding2);
            binding2.ExclusiveFrom.Add(binding1);
          }
        }
      }
    }
  }
}
