// Decompiled with JetBrains decompiler
// Type: Monocle.Chooser`1
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Monocle
{
  public class Chooser<T>
  {
    private List<Chooser<T>.Choice> choices;

    public Chooser()
    {
      this.choices = new List<Chooser<T>.Choice>();
    }

    public Chooser(T firstChoice, float weight)
      : this()
    {
      this.Add(firstChoice, weight);
    }

    public Chooser(params T[] choices)
      : this()
    {
      foreach (T choice in choices)
        this.Add(choice, 1f);
    }

    public int Count
    {
      get
      {
        return this.choices.Count;
      }
    }

    public T this[int index]
    {
      get
      {
        if (index < 0 || index >= this.Count)
          throw new IndexOutOfRangeException();
        return this.choices[index].Value;
      }
      set
      {
        if (index < 0 || index >= this.Count)
          throw new IndexOutOfRangeException();
        this.choices[index].Value = value;
      }
    }

    public Chooser<T> Add(T choice, float weight)
    {
      weight = Math.Max(weight, 0.0f);
      this.choices.Add(new Chooser<T>.Choice(choice, weight));
      this.TotalWeight += weight;
      return this;
    }

    public T Choose()
    {
      if ((double) this.TotalWeight <= 0.0)
        return default (T);
      if (this.choices.Count == 1)
        return this.choices[0].Value;
      double num1 = Calc.Random.NextDouble() * (double) this.TotalWeight;
      float num2 = 0.0f;
      for (int index = 0; index < this.choices.Count - 1; ++index)
      {
        num2 += this.choices[index].Weight;
        if (num1 < (double) num2)
          return this.choices[index].Value;
      }
      return this.choices[this.choices.Count - 1].Value;
    }

    public float TotalWeight { get; private set; }

    public bool CanChoose
    {
      get
      {
        return (double) this.TotalWeight > 0.0;
      }
    }

    public static Chooser<TT> FromString<TT>(string data) where TT : IConvertible
    {
      Chooser<TT> chooser = new Chooser<TT>();
      string[] strArray1 = data.Split(',');
      if (strArray1.Length == 1 && strArray1[0].IndexOf(':') == -1)
      {
        chooser.Add((TT) Convert.ChangeType((object) strArray1[0], typeof (TT)), 1f);
        return chooser;
      }
      foreach (string str1 in strArray1)
      {
        if (str1.IndexOf(':') == -1)
        {
          chooser.Add((TT) Convert.ChangeType((object) str1, typeof (TT)), 1f);
        }
        else
        {
          string[] strArray2 = str1.Split(':');
          string str2 = strArray2[0].Trim();
          string str3 = strArray2[1].Trim();
          chooser.Add((TT) Convert.ChangeType((object) str2, typeof (TT)), Convert.ToSingle(str3, (IFormatProvider) CultureInfo.InvariantCulture));
        }
      }
      return chooser;
    }

    private class Choice
    {
      public T Value;
      public float Weight;

      public Choice(T value, float weight)
      {
        this.Value = value;
        this.Weight = weight;
      }
    }
  }
}
