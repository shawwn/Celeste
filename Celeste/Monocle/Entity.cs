// Decompiled with JetBrains decompiler
// Type: Monocle.Entity
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Monocle
{
  public class Entity : IEnumerable<Component>, IEnumerable
  {
    public bool Active = true;
    public bool Visible = true;
    public bool Collidable = true;
    internal int depth = 0;
    internal double actualDepth = 0.0;
    public Vector2 Position;
    private int tag;
    private Collider collider;

    public Scene Scene { get; private set; }

    public ComponentList Components { get; private set; }

    public Entity(Vector2 position)
    {
      this.Position = position;
      this.Components = new ComponentList(this);
    }

    public Entity()
      : this(Vector2.Zero)
    {
    }

    public virtual void SceneBegin(Scene scene)
    {
    }

    public virtual void SceneEnd(Scene scene)
    {
      if (this.Components == null)
        return;
      foreach (Component component in this.Components)
        component.SceneEnd(scene);
    }

    public virtual void Awake(Scene scene)
    {
      if (this.Components == null)
        return;
      foreach (Component component in this.Components)
        component.EntityAwake();
    }

    public virtual void Added(Scene scene)
    {
      this.Scene = scene;
      if (this.Components != null)
      {
        foreach (Component component in this.Components)
          component.EntityAdded(scene);
      }
      this.Scene.SetActualDepth(this);
    }

    public virtual void Removed(Scene scene)
    {
      if (this.Components != null)
      {
        foreach (Component component in this.Components)
          component.EntityRemoved(scene);
      }
      this.Scene = (Scene) null;
    }

    public virtual void Update()
    {
      this.Components.Update();
    }

    public virtual void Render()
    {
      this.Components.Render();
    }

    public virtual void DebugRender(Camera camera)
    {
      if (this.Collider != null)
        this.Collider.Render(camera, this.Collidable ? Color.Red : Color.DarkRed);
      this.Components.DebugRender(camera);
    }

    public virtual void HandleGraphicsReset()
    {
      this.Components.HandleGraphicsReset();
    }

    public virtual void HandleGraphicsCreate()
    {
      this.Components.HandleGraphicsCreate();
    }

    public void RemoveSelf()
    {
      if (this.Scene == null)
        return;
      this.Scene.Entities.Remove(this);
    }

    public int Depth
    {
      get
      {
        return this.depth;
      }
      set
      {
        if (this.depth == value)
          return;
        this.depth = value;
        if (this.Scene != null)
          this.Scene.SetActualDepth(this);
      }
    }

    public float X
    {
      get
      {
        return this.Position.X;
      }
      set
      {
        this.Position.X = value;
      }
    }

    public float Y
    {
      get
      {
        return this.Position.Y;
      }
      set
      {
        this.Position.Y = value;
      }
    }

    public Collider Collider
    {
      get
      {
        return this.collider;
      }
      set
      {
        if (value == this.collider)
          return;
        if (this.collider != null)
          this.collider.Removed();
        this.collider = value;
        if (this.collider == null)
          return;
        this.collider.Added(this);
      }
    }

    public float Width
    {
      get
      {
        if (this.Collider == null)
          return 0.0f;
        return this.Collider.Width;
      }
    }

    public float Height
    {
      get
      {
        if (this.Collider == null)
          return 0.0f;
        return this.Collider.Height;
      }
    }

    public float Left
    {
      get
      {
        if (this.Collider == null)
          return this.X;
        return this.Position.X + this.Collider.Left;
      }
      set
      {
        if (this.Collider == null)
          this.Position.X = value;
        else
          this.Position.X = value - this.Collider.Left;
      }
    }

    public float Right
    {
      get
      {
        if (this.Collider == null)
          return this.Position.X;
        return this.Position.X + this.Collider.Right;
      }
      set
      {
        if (this.Collider == null)
          this.Position.X = value;
        else
          this.Position.X = value - this.Collider.Right;
      }
    }

    public float Top
    {
      get
      {
        if (this.Collider == null)
          return this.Position.Y;
        return this.Position.Y + this.Collider.Top;
      }
      set
      {
        if (this.Collider == null)
          this.Position.Y = value;
        else
          this.Position.Y = value - this.Collider.Top;
      }
    }

    public float Bottom
    {
      get
      {
        if (this.Collider == null)
          return this.Position.Y;
        return this.Position.Y + this.Collider.Bottom;
      }
      set
      {
        if (this.Collider == null)
          this.Position.Y = value;
        else
          this.Position.Y = value - this.Collider.Bottom;
      }
    }

    public float CenterX
    {
      get
      {
        if (this.Collider == null)
          return this.Position.X;
        return this.Position.X + this.Collider.CenterX;
      }
      set
      {
        if (this.Collider == null)
          this.Position.X = value;
        else
          this.Position.X = value - this.Collider.CenterX;
      }
    }

    public float CenterY
    {
      get
      {
        if (this.Collider == null)
          return this.Position.Y;
        return this.Position.Y + this.Collider.CenterY;
      }
      set
      {
        if (this.Collider == null)
          this.Position.Y = value;
        else
          this.Position.Y = value - this.Collider.CenterY;
      }
    }

    public Vector2 TopLeft
    {
      get
      {
        return new Vector2(this.Left, this.Top);
      }
      set
      {
        this.Left = value.X;
        this.Top = value.Y;
      }
    }

    public Vector2 TopRight
    {
      get
      {
        return new Vector2(this.Right, this.Top);
      }
      set
      {
        this.Right = value.X;
        this.Top = value.Y;
      }
    }

    public Vector2 BottomLeft
    {
      get
      {
        return new Vector2(this.Left, this.Bottom);
      }
      set
      {
        this.Left = value.X;
        this.Bottom = value.Y;
      }
    }

    public Vector2 BottomRight
    {
      get
      {
        return new Vector2(this.Right, this.Bottom);
      }
      set
      {
        this.Right = value.X;
        this.Bottom = value.Y;
      }
    }

    public Vector2 Center
    {
      get
      {
        return new Vector2(this.CenterX, this.CenterY);
      }
      set
      {
        this.CenterX = value.X;
        this.CenterY = value.Y;
      }
    }

    public Vector2 CenterLeft
    {
      get
      {
        return new Vector2(this.Left, this.CenterY);
      }
      set
      {
        this.Left = value.X;
        this.CenterY = value.Y;
      }
    }

    public Vector2 CenterRight
    {
      get
      {
        return new Vector2(this.Right, this.CenterY);
      }
      set
      {
        this.Right = value.X;
        this.CenterY = value.Y;
      }
    }

    public Vector2 TopCenter
    {
      get
      {
        return new Vector2(this.CenterX, this.Top);
      }
      set
      {
        this.CenterX = value.X;
        this.Top = value.Y;
      }
    }

    public Vector2 BottomCenter
    {
      get
      {
        return new Vector2(this.CenterX, this.Bottom);
      }
      set
      {
        this.CenterX = value.X;
        this.Bottom = value.Y;
      }
    }

    public int Tag
    {
      get
      {
        return this.tag;
      }
      set
      {
        if (this.tag == value)
          return;
        if (this.Scene != null)
        {
          for (int index = 0; index < BitTag.TotalTags; ++index)
          {
            int num = 1 << index;
            bool flag = (uint) (value & num) > 0U;
            if ((uint) (this.Tag & num) > 0U != flag)
            {
              if (flag)
                this.Scene.TagLists[index].Add(this);
              else
                this.Scene.TagLists[index].Remove(this);
            }
          }
        }
        this.tag = value;
      }
    }

    public bool TagFullCheck(int tag)
    {
      return (this.tag & tag) == tag;
    }

    public bool TagCheck(int tag)
    {
      return (uint) (this.tag & tag) > 0U;
    }

    public void AddTag(int tag)
    {
      this.Tag |= tag;
    }

    public void RemoveTag(int tag)
    {
      this.Tag &= ~tag;
    }

    public bool CollideCheck(Entity other)
    {
      return Collide.Check(this, other);
    }

    public bool CollideCheck(Entity other, Vector2 at)
    {
      return Collide.Check(this, other, at);
    }

    public bool CollideCheck(BitTag tag)
    {
      return Collide.Check(this, (IEnumerable<Entity>) this.Scene[tag]);
    }

    public bool CollideCheck(BitTag tag, Vector2 at)
    {
      return Collide.Check(this, (IEnumerable<Entity>) this.Scene[tag], at);
    }

    public bool CollideCheck<T>() where T : Entity
    {
      return Collide.Check(this, (IEnumerable<Entity>) this.Scene.Tracker.Entities[typeof (T)]);
    }

    public bool CollideCheck<T>(Vector2 at) where T : Entity
    {
      return Collide.Check(this, (IEnumerable<Entity>) this.Scene.Tracker.Entities[typeof (T)], at);
    }

    public bool CollideCheck<T, Exclude>()
      where T : Entity
      where Exclude : Entity
    {
      List<Entity> entity = this.Scene.Tracker.Entities[typeof (Exclude)];
      foreach (Entity b in this.Scene.Tracker.Entities[typeof (T)])
      {
        if (!entity.Contains(b) && Collide.Check(this, b))
          return true;
      }
      return false;
    }

    public bool CollideCheck<T, Exclude>(Vector2 at)
      where T : Entity
      where Exclude : Entity
    {
      Vector2 position = this.Position;
      this.Position = at;
      bool flag = this.CollideCheck<T, Exclude>();
      this.Position = position;
      return flag;
    }

    public bool CollideCheckByComponent<T>() where T : Component
    {
      foreach (Component component in this.Scene.Tracker.Components[typeof (T)])
      {
        if (Collide.Check(this, component.Entity))
          return true;
      }
      return false;
    }

    public bool CollideCheckByComponent<T>(Vector2 at) where T : Component
    {
      Vector2 position = this.Position;
      this.Position = at;
      bool flag = this.CollideCheckByComponent<T>();
      this.Position = position;
      return flag;
    }

    public bool CollideCheckOutside(Entity other, Vector2 at)
    {
      return !Collide.Check(this, other) && Collide.Check(this, other, at);
    }

    public bool CollideCheckOutside(BitTag tag, Vector2 at)
    {
      foreach (Entity b in this.Scene[tag])
      {
        if (!Collide.Check(this, b) && Collide.Check(this, b, at))
          return true;
      }
      return false;
    }

    public bool CollideCheckOutside<T>(Vector2 at) where T : Entity
    {
      foreach (Entity b in this.Scene.Tracker.Entities[typeof (T)])
      {
        if (!Collide.Check(this, b) && Collide.Check(this, b, at))
          return true;
      }
      return false;
    }

    public bool CollideCheckOutsideByComponent<T>(Vector2 at) where T : Component
    {
      foreach (Component component in this.Scene.Tracker.Components[typeof (T)])
      {
        if (!Collide.Check(this, component.Entity) && Collide.Check(this, component.Entity, at))
          return true;
      }
      return false;
    }

    public Entity CollideFirst(BitTag tag)
    {
      return Collide.First(this, (IEnumerable<Entity>) this.Scene[tag]);
    }

    public Entity CollideFirst(BitTag tag, Vector2 at)
    {
      return Collide.First(this, (IEnumerable<Entity>) this.Scene[tag], at);
    }

    public T CollideFirst<T>() where T : Entity
    {
      return Collide.First(this, (IEnumerable<Entity>) this.Scene.Tracker.Entities[typeof (T)]) as T;
    }

    public T CollideFirst<T>(Vector2 at) where T : Entity
    {
      return Collide.First(this, (IEnumerable<Entity>) this.Scene.Tracker.Entities[typeof (T)], at) as T;
    }

    public T CollideFirstByComponent<T>() where T : Component
    {
      foreach (Component component in this.Scene.Tracker.Components[typeof (T)])
      {
        if (Collide.Check(this, component.Entity))
          return component as T;
      }
      return default (T);
    }

    public T CollideFirstByComponent<T>(Vector2 at) where T : Component
    {
      foreach (Component component in this.Scene.Tracker.Components[typeof (T)])
      {
        if (Collide.Check(this, component.Entity, at))
          return component as T;
      }
      return default (T);
    }

    public Entity CollideFirstOutside(BitTag tag, Vector2 at)
    {
      foreach (Entity b in this.Scene[tag])
      {
        if (!Collide.Check(this, b) && Collide.Check(this, b, at))
          return b;
      }
      return (Entity) null;
    }

    public T CollideFirstOutside<T>(Vector2 at) where T : Entity
    {
      foreach (Entity b in this.Scene.Tracker.Entities[typeof (T)])
      {
        if (!Collide.Check(this, b) && Collide.Check(this, b, at))
          return b as T;
      }
      return default (T);
    }

    public T CollideFirstOutsideByComponent<T>(Vector2 at) where T : Component
    {
      foreach (Component component in this.Scene.Tracker.Components[typeof (T)])
      {
        if (!Collide.Check(this, component.Entity) && Collide.Check(this, component.Entity, at))
          return component as T;
      }
      return default (T);
    }

    public List<Entity> CollideAll(BitTag tag)
    {
      return Collide.All(this, (IEnumerable<Entity>) this.Scene[tag]);
    }

    public List<Entity> CollideAll(BitTag tag, Vector2 at)
    {
      return Collide.All(this, (IEnumerable<Entity>) this.Scene[tag], at);
    }

    public List<Entity> CollideAll<T>() where T : Entity
    {
      return Collide.All(this, (IEnumerable<Entity>) this.Scene.Tracker.Entities[typeof (T)]);
    }

    public List<Entity> CollideAll<T>(Vector2 at) where T : Entity
    {
      return Collide.All(this, (IEnumerable<Entity>) this.Scene.Tracker.Entities[typeof (T)], at);
    }

    public List<Entity> CollideAll<T>(Vector2 at, List<Entity> into) where T : Entity
    {
      into.Clear();
      return Collide.All(this, (IEnumerable<Entity>) this.Scene.Tracker.Entities[typeof (T)], into, at);
    }

    public List<T> CollideAllByComponent<T>() where T : Component
    {
      List<T> objList = new List<T>();
      foreach (Component component in this.Scene.Tracker.Components[typeof (T)])
      {
        if (Collide.Check(this, component.Entity))
          objList.Add(component as T);
      }
      return objList;
    }

    public List<T> CollideAllByComponent<T>(Vector2 at) where T : Component
    {
      Vector2 position = this.Position;
      this.Position = at;
      List<T> objList = this.CollideAllByComponent<T>();
      this.Position = position;
      return objList;
    }

    public bool CollideDo(BitTag tag, Action<Entity> action)
    {
      bool flag = false;
      foreach (Entity other in this.Scene[tag])
      {
        if (this.CollideCheck(other))
        {
          action(other);
          flag = true;
        }
      }
      return flag;
    }

    public bool CollideDo(BitTag tag, Action<Entity> action, Vector2 at)
    {
      bool flag = false;
      Vector2 position = this.Position;
      this.Position = at;
      foreach (Entity other in this.Scene[tag])
      {
        if (this.CollideCheck(other))
        {
          action(other);
          flag = true;
        }
      }
      this.Position = position;
      return flag;
    }

    public bool CollideDo<T>(Action<T> action) where T : Entity
    {
      bool flag = false;
      foreach (Entity other in this.Scene.Tracker.Entities[typeof (T)])
      {
        if (this.CollideCheck(other))
        {
          action(other as T);
          flag = true;
        }
      }
      return flag;
    }

    public bool CollideDo<T>(Action<T> action, Vector2 at) where T : Entity
    {
      bool flag = false;
      Vector2 position = this.Position;
      this.Position = at;
      foreach (Entity other in this.Scene.Tracker.Entities[typeof (T)])
      {
        if (this.CollideCheck(other))
        {
          action(other as T);
          flag = true;
        }
      }
      this.Position = position;
      return flag;
    }

    public bool CollideDoByComponent<T>(Action<T> action) where T : Component
    {
      bool flag = false;
      foreach (Component component in this.Scene.Tracker.Components[typeof (T)])
      {
        if (this.CollideCheck(component.Entity))
        {
          action(component as T);
          flag = true;
        }
      }
      return flag;
    }

    public bool CollideDoByComponent<T>(Action<T> action, Vector2 at) where T : Component
    {
      bool flag = false;
      Vector2 position = this.Position;
      this.Position = at;
      foreach (Component component in this.Scene.Tracker.Components[typeof (T)])
      {
        if (this.CollideCheck(component.Entity))
        {
          action(component as T);
          flag = true;
        }
      }
      this.Position = position;
      return flag;
    }

    public bool CollidePoint(Vector2 point)
    {
      return Collide.CheckPoint(this, point);
    }

    public bool CollidePoint(Vector2 point, Vector2 at)
    {
      return Collide.CheckPoint(this, point, at);
    }

    public bool CollideLine(Vector2 from, Vector2 to)
    {
      return Collide.CheckLine(this, from, to);
    }

    public bool CollideLine(Vector2 from, Vector2 to, Vector2 at)
    {
      return Collide.CheckLine(this, from, to, at);
    }

    public bool CollideRect(Rectangle rect)
    {
      return Collide.CheckRect(this, rect);
    }

    public bool CollideRect(Rectangle rect, Vector2 at)
    {
      return Collide.CheckRect(this, rect, at);
    }

    public void Add(Component component)
    {
      this.Components.Add(component);
    }

    public void Remove(Component component)
    {
      this.Components.Remove(component);
    }

    public void Add(params Component[] components)
    {
      this.Components.Add(components);
    }

    public void Remove(params Component[] components)
    {
      this.Components.Remove(components);
    }

    public T Get<T>() where T : Component
    {
      return this.Components.Get<T>();
    }

    public IEnumerator<Component> GetEnumerator()
    {
      return this.Components.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    public Entity Closest(params Entity[] entities)
    {
      Entity entity = entities[0];
      float num1 = Vector2.DistanceSquared(this.Position, entity.Position);
      for (int index = 1; index < entities.Length; ++index)
      {
        float num2 = Vector2.DistanceSquared(this.Position, entities[index].Position);
        if ((double) num2 < (double) num1)
        {
          entity = entities[index];
          num1 = num2;
        }
      }
      return entity;
    }

    public Entity Closest(BitTag tag)
    {
      List<Entity> entityList = this.Scene[tag];
      Entity entity = (Entity) null;
      if (entityList.Count >= 1)
      {
        entity = entityList[0];
        float num1 = Vector2.DistanceSquared(this.Position, entity.Position);
        for (int index = 1; index < entityList.Count; ++index)
        {
          float num2 = Vector2.DistanceSquared(this.Position, entityList[index].Position);
          if ((double) num2 < (double) num1)
          {
            entity = entityList[index];
            num1 = num2;
          }
        }
      }
      return entity;
    }

    public T SceneAs<T>() where T : Scene
    {
      return this.Scene as T;
    }
  }
}

