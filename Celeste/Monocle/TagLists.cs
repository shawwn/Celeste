// Decompiled with JetBrains decompiler
// Type: Monocle.TagLists
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System.Collections.Generic;

namespace Monocle
{
  public class TagLists
  {
    private List<Entity>[] lists;
    private bool[] unsorted;
    private bool areAnyUnsorted;

    internal TagLists()
    {
      this.lists = new List<Entity>[BitTag.TotalTags];
      this.unsorted = new bool[BitTag.TotalTags];
      for (int index = 0; index < this.lists.Length; ++index)
        this.lists[index] = new List<Entity>();
    }

    public List<Entity> this[int index]
    {
      get
      {
        return this.lists[index];
      }
    }

    internal void MarkUnsorted(int index)
    {
      this.areAnyUnsorted = true;
      this.unsorted[index] = true;
    }

    internal void UpdateLists()
    {
      if (!this.areAnyUnsorted)
        return;
      for (int index = 0; index < this.lists.Length; ++index)
      {
        if (this.unsorted[index])
        {
          this.lists[index].Sort(EntityList.CompareDepth);
          this.unsorted[index] = false;
        }
      }
      this.areAnyUnsorted = false;
    }

    internal void EntityAdded(Entity entity)
    {
      for (int index = 0; index < BitTag.TotalTags; ++index)
      {
        if (entity.TagCheck(1 << index))
        {
          this[index].Add(entity);
          this.areAnyUnsorted = true;
          this.unsorted[index] = true;
        }
      }
    }

    internal void EntityRemoved(Entity entity)
    {
      for (int index = 0; index < BitTag.TotalTags; ++index)
      {
        if (entity.TagCheck(1 << index))
          this.lists[index].Remove(entity);
      }
    }
  }
}
