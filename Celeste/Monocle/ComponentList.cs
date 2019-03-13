// Decompiled with JetBrains decompiler
// Type: Monocle.ComponentList
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Monocle
{
  public class ComponentList : IEnumerable<Component>, IEnumerable
  {
    private List<Component> components;
    private List<Component> toAdd;
    private List<Component> toRemove;
    private HashSet<Component> current;
    private HashSet<Component> adding;
    private HashSet<Component> removing;
    private ComponentList.LockModes lockMode;

    public Entity Entity { get; internal set; }

    internal ComponentList(Entity entity)
    {
      this.Entity = entity;
      this.components = new List<Component>();
      this.toAdd = new List<Component>();
      this.toRemove = new List<Component>();
      this.current = new HashSet<Component>();
      this.adding = new HashSet<Component>();
      this.removing = new HashSet<Component>();
    }

    internal ComponentList.LockModes LockMode
    {
      get
      {
        return this.lockMode;
      }
      set
      {
        this.lockMode = value;
        if (this.toAdd.Count > 0)
        {
          foreach (Component component in this.toAdd)
          {
            if (!this.current.Contains(component))
            {
              this.current.Add(component);
              this.components.Add(component);
              component.Added(this.Entity);
            }
          }
          this.adding.Clear();
          this.toAdd.Clear();
        }
        if (this.toRemove.Count <= 0)
          return;
        foreach (Component component in this.toRemove)
        {
          if (this.current.Contains(component))
          {
            this.current.Remove(component);
            this.components.Remove(component);
            component.Removed(this.Entity);
          }
        }
        this.removing.Clear();
        this.toRemove.Clear();
      }
    }

    public void Add(Component component)
    {
      switch (this.lockMode)
      {
        case ComponentList.LockModes.Open:
          if (this.current.Contains(component))
            break;
          this.current.Add(component);
          this.components.Add(component);
          component.Added(this.Entity);
          break;
        case ComponentList.LockModes.Locked:
          if (this.current.Contains(component) || this.adding.Contains(component))
            break;
          this.adding.Add(component);
          this.toAdd.Add(component);
          break;
        case ComponentList.LockModes.Error:
          throw new Exception("Cannot add or remove Entities at this time!");
      }
    }

    public void Remove(Component component)
    {
      switch (this.lockMode)
      {
        case ComponentList.LockModes.Open:
          if (!this.current.Contains(component))
            break;
          this.current.Remove(component);
          this.components.Remove(component);
          component.Removed(this.Entity);
          break;
        case ComponentList.LockModes.Locked:
          if (!this.current.Contains(component) || this.removing.Contains(component))
            break;
          this.removing.Add(component);
          this.toRemove.Add(component);
          break;
        case ComponentList.LockModes.Error:
          throw new Exception("Cannot add or remove Entities at this time!");
      }
    }

    public void Add(IEnumerable<Component> components)
    {
      foreach (Component component in components)
        this.Add(component);
    }

    public void Remove(IEnumerable<Component> components)
    {
      foreach (Component component in components)
        this.Remove(component);
    }

    public void RemoveAll<T>() where T : Component
    {
      this.Remove((IEnumerable<Component>) this.GetAll<T>());
    }

    public void Add(params Component[] components)
    {
      foreach (Component component in components)
        this.Add(component);
    }

    public void Remove(params Component[] components)
    {
      foreach (Component component in components)
        this.Remove(component);
    }

    public int Count
    {
      get
      {
        return this.components.Count;
      }
    }

    public Component this[int index]
    {
      get
      {
        if (index < 0 || index >= this.components.Count)
          throw new IndexOutOfRangeException();
        return this.components[index];
      }
    }

    public IEnumerator<Component> GetEnumerator()
    {
      return (IEnumerator<Component>) this.components.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    public Component[] ToArray()
    {
      return this.components.ToArray<Component>();
    }

    internal void Update()
    {
      this.LockMode = ComponentList.LockModes.Locked;
      foreach (Component component in this.components)
      {
        if (component.Active)
          component.Update();
      }
      this.LockMode = ComponentList.LockModes.Open;
    }

    internal void Render()
    {
      this.LockMode = ComponentList.LockModes.Error;
      foreach (Component component in this.components)
      {
        if (component.Visible)
          component.Render();
      }
      this.LockMode = ComponentList.LockModes.Open;
    }

    internal void DebugRender(Camera camera)
    {
      this.LockMode = ComponentList.LockModes.Error;
      foreach (Component component in this.components)
        component.DebugRender(camera);
      this.LockMode = ComponentList.LockModes.Open;
    }

    internal void HandleGraphicsReset()
    {
      this.LockMode = ComponentList.LockModes.Error;
      foreach (Component component in this.components)
        component.HandleGraphicsReset();
      this.LockMode = ComponentList.LockModes.Open;
    }

    internal void HandleGraphicsCreate()
    {
      this.LockMode = ComponentList.LockModes.Error;
      foreach (Component component in this.components)
        component.HandleGraphicsCreate();
      this.LockMode = ComponentList.LockModes.Open;
    }

    public T Get<T>() where T : Component
    {
      foreach (Component component in this.components)
      {
        if (component is T)
          return component as T;
      }
      return default (T);
    }

    public IEnumerable<T> GetAll<T>() where T : Component
    {
      foreach (Component component in this.components)
      {
        if (component is T)
          yield return component as T;
      }
    }

    public enum LockModes
    {
      Open,
      Locked,
      Error,
    }
  }
}
