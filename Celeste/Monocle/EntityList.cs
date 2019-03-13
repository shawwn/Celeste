// Decompiled with JetBrains decompiler
// Type: Monocle.EntityList
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Monocle
{
  public class EntityList : IEnumerable<Entity>, IEnumerable
  {
    public static Comparison<Entity> CompareDepth = (Comparison<Entity>) ((a, b) => Math.Sign(b.actualDepth - a.actualDepth));
    private List<Entity> entities;
    private List<Entity> toAdd;
    private List<Entity> toAwake;
    private List<Entity> toRemove;
    private HashSet<Entity> current;
    private HashSet<Entity> adding;
    private HashSet<Entity> removing;
    private bool unsorted;

    public Scene Scene { get; private set; }

    internal EntityList(Scene scene)
    {
      this.Scene = scene;
      this.entities = new List<Entity>();
      this.toAdd = new List<Entity>();
      this.toAwake = new List<Entity>();
      this.toRemove = new List<Entity>();
      this.current = new HashSet<Entity>();
      this.adding = new HashSet<Entity>();
      this.removing = new HashSet<Entity>();
    }

    internal void MarkUnsorted()
    {
      this.unsorted = true;
    }

    public void UpdateLists()
    {
      if (this.toAdd.Count > 0)
      {
        for (int index = 0; index < this.toAdd.Count; ++index)
        {
          Entity entity = this.toAdd[index];
          if (!this.current.Contains(entity))
          {
            this.current.Add(entity);
            this.entities.Add(entity);
            if (this.Scene != null)
            {
              this.Scene.TagLists.EntityAdded(entity);
              this.Scene.Tracker.EntityAdded(entity);
              entity.Added(this.Scene);
            }
          }
        }
        this.unsorted = true;
      }
      if (this.toRemove.Count > 0)
      {
        for (int index = 0; index < this.toRemove.Count; ++index)
        {
          Entity entity = this.toRemove[index];
          if (this.entities.Contains(entity))
          {
            this.current.Remove(entity);
            this.entities.Remove(entity);
            if (this.Scene != null)
            {
              entity.Removed(this.Scene);
              this.Scene.TagLists.EntityRemoved(entity);
              this.Scene.Tracker.EntityRemoved(entity);
              Engine.Pooler.EntityRemoved(entity);
            }
          }
        }
        this.toRemove.Clear();
        this.removing.Clear();
      }
      if (this.unsorted)
      {
        this.unsorted = false;
        this.entities.Sort(EntityList.CompareDepth);
      }
      if (this.toAdd.Count <= 0)
        return;
      this.toAwake.AddRange((IEnumerable<Entity>) this.toAdd);
      this.toAdd.Clear();
      this.adding.Clear();
      foreach (Entity entity in this.toAwake)
      {
        if (entity.Scene == this.Scene)
          entity.Awake(this.Scene);
      }
      this.toAwake.Clear();
    }

    public void Add(Entity entity)
    {
      if (this.adding.Contains(entity) || this.current.Contains(entity))
        return;
      this.adding.Add(entity);
      this.toAdd.Add(entity);
    }

    public void Remove(Entity entity)
    {
      if (this.removing.Contains(entity) || !this.current.Contains(entity))
        return;
      this.removing.Add(entity);
      this.toRemove.Add(entity);
    }

    public void Add(IEnumerable<Entity> entities)
    {
      foreach (Entity entity in entities)
        this.Add(entity);
    }

    public void Remove(IEnumerable<Entity> entities)
    {
      foreach (Entity entity in entities)
        this.Remove(entity);
    }

    public void Add(params Entity[] entities)
    {
      for (int index = 0; index < entities.Length; ++index)
        this.Add(entities[index]);
    }

    public void Remove(params Entity[] entities)
    {
      for (int index = 0; index < entities.Length; ++index)
        this.Remove(entities[index]);
    }

    public int Count
    {
      get
      {
        return this.entities.Count;
      }
    }

    public Entity this[int index]
    {
      get
      {
        if (index < 0 || index >= this.entities.Count)
          throw new IndexOutOfRangeException();
        return this.entities[index];
      }
    }

    public int AmountOf<T>() where T : Entity
    {
      int num = 0;
      foreach (Entity entity in this.entities)
      {
        if (entity is T)
          ++num;
      }
      return num;
    }

    public T FindFirst<T>() where T : Entity
    {
      foreach (Entity entity in this.entities)
      {
        if (entity is T)
          return entity as T;
      }
      return default (T);
    }

    public List<T> FindAll<T>() where T : Entity
    {
      List<T> objList = new List<T>();
      foreach (Entity entity in this.entities)
      {
        if (entity is T)
          objList.Add(entity as T);
      }
      return objList;
    }

    public void With<T>(Action<T> action) where T : Entity
    {
      foreach (Entity entity in this.entities)
      {
        if (entity is T)
          action(entity as T);
      }
    }

    public IEnumerator<Entity> GetEnumerator()
    {
      return (IEnumerator<Entity>) this.entities.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    public Entity[] ToArray()
    {
      return this.entities.ToArray<Entity>();
    }

    public bool HasVisibleEntities(int matchTags)
    {
      foreach (Entity entity in this.entities)
      {
        if (entity.Visible && entity.TagCheck(matchTags))
          return true;
      }
      return false;
    }

    internal void Update()
    {
      foreach (Entity entity in this.entities)
      {
        if (entity.Active)
          entity.Update();
      }
    }

    public void Render()
    {
      foreach (Entity entity in this.entities)
      {
        if (entity.Visible)
          entity.Render();
      }
    }

    public void RenderOnly(int matchTags)
    {
      foreach (Entity entity in this.entities)
      {
        if (entity.Visible && entity.TagCheck(matchTags))
          entity.Render();
      }
    }

    public void RenderOnlyFullMatch(int matchTags)
    {
      foreach (Entity entity in this.entities)
      {
        if (entity.Visible && entity.TagFullCheck(matchTags))
          entity.Render();
      }
    }

    public void RenderExcept(int excludeTags)
    {
      foreach (Entity entity in this.entities)
      {
        if (entity.Visible && !entity.TagCheck(excludeTags))
          entity.Render();
      }
    }

    public void DebugRender(Camera camera)
    {
      foreach (Entity entity in this.entities)
        entity.DebugRender(camera);
    }

    internal void HandleGraphicsReset()
    {
      foreach (Entity entity in this.entities)
        entity.HandleGraphicsReset();
    }

    internal void HandleGraphicsCreate()
    {
      foreach (Entity entity in this.entities)
        entity.HandleGraphicsCreate();
    }
  }
}
