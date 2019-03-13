// Decompiled with JetBrains decompiler
// Type: Monocle.CheatListener
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Collections.Generic;

namespace Monocle
{
  public class CheatListener : Entity
  {
    public string CurrentInput;
    public bool Logging;
    private List<Tuple<char, Func<bool>>> inputs;
    private List<Tuple<string, Action>> cheats;
    private int maxInput;

    public CheatListener()
    {
      this.Visible = false;
      this.CurrentInput = "";
      this.inputs = new List<Tuple<char, Func<bool>>>();
      this.cheats = new List<Tuple<string, Action>>();
    }

    public override void Update()
    {
      bool flag = false;
      foreach (Tuple<char, Func<bool>> input in this.inputs)
      {
        if (input.Item2())
        {
          this.CurrentInput += input.Item1.ToString();
          flag = true;
        }
      }
      if (!flag)
        return;
      if (this.CurrentInput.Length > this.maxInput)
        this.CurrentInput = this.CurrentInput.Substring(this.CurrentInput.Length - this.maxInput);
      if (this.Logging)
        Calc.Log((object) this.CurrentInput);
      foreach (Tuple<string, Action> cheat in this.cheats)
      {
        if (this.CurrentInput.Contains(cheat.Item1))
        {
          this.CurrentInput = "";
          if (cheat.Item2 != null)
            cheat.Item2();
          this.cheats.Remove(cheat);
          if (!this.Logging)
            break;
          Calc.Log((object) ("Cheat Activated: " + cheat.Item1));
          break;
        }
      }
    }

    public void AddCheat(string code, Action onEntered = null)
    {
      this.cheats.Add(new Tuple<string, Action>(code, onEntered));
      this.maxInput = Math.Max(code.Length, this.maxInput);
    }

    public void AddInput(char id, Func<bool> checker)
    {
      this.inputs.Add(new Tuple<char, Func<bool>>(id, checker));
    }
  }
}
