// Decompiled with JetBrains decompiler
// Type: Monocle.CoroutineHolder
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System.Collections;
using System.Collections.Generic;

namespace Monocle
{
  public class CoroutineHolder : Component
  {
    private List<CoroutineHolder.CoroutineData> coroutineList;
    private HashSet<CoroutineHolder.CoroutineData> toRemove;
    private int nextID;
    private bool isRunning;

    public CoroutineHolder()
      : base(true, false)
    {
      this.coroutineList = new List<CoroutineHolder.CoroutineData>();
      this.toRemove = new HashSet<CoroutineHolder.CoroutineData>();
    }

    public override void Update()
    {
      this.isRunning = true;
      for (int index = 0; index < this.coroutineList.Count; ++index)
      {
        IEnumerator enumerator = this.coroutineList[index].Data.Peek();
        if (enumerator.MoveNext())
        {
          if (enumerator.Current is IEnumerator)
            this.coroutineList[index].Data.Push(enumerator.Current as IEnumerator);
        }
        else
        {
          this.coroutineList[index].Data.Pop();
          if (this.coroutineList[index].Data.Count == 0)
            this.toRemove.Add(this.coroutineList[index]);
        }
      }
      this.isRunning = false;
      if (this.toRemove.Count <= 0)
        return;
      foreach (CoroutineHolder.CoroutineData coroutineData in this.toRemove)
        this.coroutineList.Remove(coroutineData);
      this.toRemove.Clear();
    }

    public void EndCoroutine(int id)
    {
      foreach (CoroutineHolder.CoroutineData coroutine in this.coroutineList)
      {
        if (coroutine.ID == id)
        {
          if (this.isRunning)
          {
            this.toRemove.Add(coroutine);
            break;
          }
          this.coroutineList.Remove(coroutine);
          break;
        }
      }
    }

    public int StartCoroutine(IEnumerator functionCall)
    {
      CoroutineHolder.CoroutineData coroutineData = new CoroutineHolder.CoroutineData(this.nextID++, functionCall);
      this.coroutineList.Add(coroutineData);
      return coroutineData.ID;
    }

    public static IEnumerator WaitForFrames(int frames)
    {
      for (int i = 0; i < frames; ++i)
        yield return (object) 0;
    }

    private class CoroutineData
    {
      public int ID;
      public Stack<IEnumerator> Data;

      public CoroutineData(int id, IEnumerator functionCall)
      {
        this.ID = id;
        this.Data = new Stack<IEnumerator>();
        this.Data.Push(functionCall);
      }
    }
  }
}
