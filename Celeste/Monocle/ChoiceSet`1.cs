// Decompiled with JetBrains decompiler
// Type: Monocle.ChoiceSet`1
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Collections.Generic;

namespace Monocle
{
  public class ChoiceSet<T>
  {
    private Dictionary<T, int> choices;

    public int TotalWeight { get; private set; }

    public ChoiceSet()
    {
      this.choices = new Dictionary<T, int>();
      this.TotalWeight = 0;
    }

    public void Set(T choice, int weight)
    {
      int num = 0;
      this.choices.TryGetValue(choice, out num);
      this.TotalWeight -= num;
      if (weight <= 0)
      {
        if (!this.choices.ContainsKey(choice))
          return;
        this.choices.Remove(choice);
      }
      else
      {
        this.TotalWeight += weight;
        this.choices[choice] = weight;
      }
    }

    public int this[T choice]
    {
      get
      {
        int num = 0;
        this.choices.TryGetValue(choice, out num);
        return num;
      }
      set
      {
        this.Set(choice, value);
      }
    }

    public void Set(T choice, float chance)
    {
      int num1 = 0;
      this.choices.TryGetValue(choice, out num1);
      this.TotalWeight -= num1;
      int num2 = (int) Math.Round((double) this.TotalWeight / (1.0 - (double) chance));
      if (num2 <= 0 && (double) chance > 0.0)
        num2 = 1;
      if (num2 <= 0)
      {
        if (!this.choices.ContainsKey(choice))
          return;
        this.choices.Remove(choice);
      }
      else
      {
        this.TotalWeight += num2;
        this.choices[choice] = num2;
      }
    }

    public void SetMany(float totalChance, params T[] choices)
    {
      if (choices.Length == 0)
        return;
      double num1 = (double) totalChance / (double) choices.Length;
      int num2 = 0;
      foreach (T choice in choices)
      {
        int num3 = 0;
        this.choices.TryGetValue(choice, out num3);
        num2 += num3;
      }
      this.TotalWeight -= num2;
      int num4 = (int) Math.Round((double) this.TotalWeight / (1.0 - (double) totalChance) / (double) choices.Length);
      if (num4 <= 0 && (double) totalChance > 0.0)
        num4 = 1;
      if (num4 <= 0)
      {
        foreach (T choice in choices)
        {
          if (this.choices.ContainsKey(choice))
            this.choices.Remove(choice);
        }
      }
      else
      {
        this.TotalWeight += num4 * choices.Length;
        foreach (T choice in choices)
          this.choices[choice] = num4;
      }
    }

    public T Get(Random random)
    {
      int num = random.Next(this.TotalWeight);
      foreach (KeyValuePair<T, int> choice in this.choices)
      {
        if (num < choice.Value)
          return choice.Key;
        num -= choice.Value;
      }
      throw new Exception("Random choice error!");
    }

    public T Get()
    {
      return this.Get(Calc.Random);
    }

    private struct Choice
    {
      public T Data;
      public int Weight;

      public Choice(T data, int weight)
      {
        this.Data = data;
        this.Weight = weight;
      }
    }
  }
}
