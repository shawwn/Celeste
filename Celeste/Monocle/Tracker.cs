// Decompiled with JetBrains decompiler
// Type: Monocle.Tracker
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Monocle
{
  public class Tracker
  {
    public static Dictionary<Type, List<Type>> TrackedEntityTypes { get; private set; }

    public static Dictionary<Type, List<Type>> TrackedComponentTypes { get; private set; }

    public static HashSet<Type> StoredEntityTypes { get; private set; }

    public static HashSet<Type> StoredComponentTypes { get; private set; }

    public static void Initialize()
    {
      Tracker.TrackedEntityTypes = new Dictionary<Type, List<Type>>();
      Tracker.TrackedComponentTypes = new Dictionary<Type, List<Type>>();
      Tracker.StoredEntityTypes = new HashSet<Type>();
      Tracker.StoredComponentTypes = new HashSet<Type>();
      foreach (Type type in Assembly.GetEntryAssembly().GetTypes())
      {
        object[] customAttributes = type.GetCustomAttributes(typeof (Tracked), false);
        if (customAttributes.Length != 0)
        {
          bool inherited = (customAttributes[0] as Tracked).Inherited;
          if (typeof (Entity).IsAssignableFrom(type))
          {
            if (!type.IsAbstract)
            {
              if (!Tracker.TrackedEntityTypes.ContainsKey(type))
                Tracker.TrackedEntityTypes.Add(type, new List<Type>());
              Tracker.TrackedEntityTypes[type].Add(type);
            }
            Tracker.StoredEntityTypes.Add(type);
            if (inherited)
            {
              foreach (Type subclass in Tracker.GetSubclasses(type))
              {
                if (!subclass.IsAbstract)
                {
                  if (!Tracker.TrackedEntityTypes.ContainsKey(subclass))
                    Tracker.TrackedEntityTypes.Add(subclass, new List<Type>());
                  Tracker.TrackedEntityTypes[subclass].Add(type);
                }
              }
            }
          }
          else
          {
            if (!typeof (Component).IsAssignableFrom(type))
              throw new Exception("Type '" + type.Name + "' cannot be Tracked because it does not derive from Entity or Component");
            if (!type.IsAbstract)
            {
              if (!Tracker.TrackedComponentTypes.ContainsKey(type))
                Tracker.TrackedComponentTypes.Add(type, new List<Type>());
              Tracker.TrackedComponentTypes[type].Add(type);
            }
            Tracker.StoredComponentTypes.Add(type);
            if (inherited)
            {
              foreach (Type subclass in Tracker.GetSubclasses(type))
              {
                if (!subclass.IsAbstract)
                {
                  if (!Tracker.TrackedComponentTypes.ContainsKey(subclass))
                    Tracker.TrackedComponentTypes.Add(subclass, new List<Type>());
                  Tracker.TrackedComponentTypes[subclass].Add(type);
                }
              }
            }
          }
        }
      }
    }

    private static List<Type> GetSubclasses(Type type)
    {
      List<Type> typeList = new List<Type>();
      foreach (Type type1 in Assembly.GetEntryAssembly().GetTypes())
      {
        if (type != type1 && type.IsAssignableFrom(type1))
          typeList.Add(type1);
      }
      return typeList;
    }

    public Dictionary<Type, List<Entity>> Entities { get; private set; }

    public Dictionary<Type, List<Component>> Components { get; private set; }

    public Tracker()
    {
      this.Entities = new Dictionary<Type, List<Entity>>(Tracker.TrackedEntityTypes.Count);
      foreach (Type storedEntityType in Tracker.StoredEntityTypes)
        this.Entities.Add(storedEntityType, new List<Entity>());
      this.Components = new Dictionary<Type, List<Component>>(Tracker.TrackedComponentTypes.Count);
      foreach (Type storedComponentType in Tracker.StoredComponentTypes)
        this.Components.Add(storedComponentType, new List<Component>());
    }

    public bool IsEntityTracked<T>() where T : Entity
    {
      return this.Entities.ContainsKey(typeof (T));
    }

    public bool IsComponentTracked<T>() where T : Component
    {
      return this.Components.ContainsKey(typeof (T));
    }

    public T GetEntity<T>() where T : Entity
    {
      List<Entity> entity = this.Entities[typeof (T)];
      if (entity.Count == 0)
        return default (T);
      return entity[0] as T;
    }

    public T GetNearestEntity<T>(Vector2 nearestTo) where T : Entity
    {
      List<Entity> entities = this.GetEntities<T>();
      T obj1 = default (T);
      float num1 = 0.0f;
      foreach (T obj2 in entities)
      {
        float num2 = Vector2.DistanceSquared(nearestTo, obj2.Position);
        if ((object) obj1 == null || (double) num2 < (double) num1)
        {
          obj1 = obj2;
          num1 = num2;
        }
      }
      return obj1;
    }

    public List<Entity> GetEntities<T>() where T : Entity
    {
      return this.Entities[typeof (T)];
    }

    public List<Entity> GetEntitiesCopy<T>() where T : Entity
    {
      return new List<Entity>((IEnumerable<Entity>) this.GetEntities<T>());
    }

    public IEnumerator<T> EnumerateEntities<T>() where T : Entity
    {
      foreach (Entity entity in this.Entities[typeof (T)])
        yield return entity as T;
    }

    public int CountEntities<T>() where T : Entity
    {
      return this.Entities[typeof (T)].Count;
    }

    public T GetComponent<T>() where T : Component
    {
      List<Component> component = this.Components[typeof (T)];
      if (component.Count == 0)
        return default (T);
      return component[0] as T;
    }

    public T GetNearestComponent<T>(Vector2 nearestTo) where T : Component
    {
      List<Component> components = this.GetComponents<T>();
      T obj1 = default (T);
      float num1 = 0.0f;
      foreach (T obj2 in components)
      {
        float num2 = Vector2.DistanceSquared(nearestTo, obj2.Entity.Position);
        if ((object) obj1 == null || (double) num2 < (double) num1)
        {
          obj1 = obj2;
          num1 = num2;
        }
      }
      return obj1;
    }

    public List<Component> GetComponents<T>() where T : Component
    {
      return this.Components[typeof (T)];
    }

    public List<Component> GetComponentsCopy<T>() where T : Component
    {
      return new List<Component>((IEnumerable<Component>) this.GetComponents<T>());
    }

    public IEnumerator<T> EnumerateComponents<T>() where T : Component
    {
      foreach (Component component in this.Components[typeof (T)])
        yield return component as T;
    }

    public int CountComponents<T>() where T : Component
    {
      return this.Components[typeof (T)].Count;
    }

    internal void EntityAdded(Entity entity)
    {
      List<Type> typeList;
      if (!Tracker.TrackedEntityTypes.TryGetValue(entity.GetType(), out typeList))
        return;
      foreach (Type index in typeList)
        this.Entities[index].Add(entity);
    }

    internal void EntityRemoved(Entity entity)
    {
      List<Type> typeList;
      if (!Tracker.TrackedEntityTypes.TryGetValue(entity.GetType(), out typeList))
        return;
      foreach (Type index in typeList)
        this.Entities[index].Remove(entity);
    }

    internal void ComponentAdded(Component component)
    {
      List<Type> typeList;
      if (!Tracker.TrackedComponentTypes.TryGetValue(component.GetType(), out typeList))
        return;
      foreach (Type index in typeList)
        this.Components[index].Add(component);
    }

    internal void ComponentRemoved(Component component)
    {
      List<Type> typeList;
      if (!Tracker.TrackedComponentTypes.TryGetValue(component.GetType(), out typeList))
        return;
      foreach (Type index in typeList)
        this.Components[index].Remove(component);
    }

    public void LogEntities()
    {
      foreach (KeyValuePair<Type, List<Entity>> entity in this.Entities)
        Engine.Commands.Log((object) (entity.Key.Name + " : " + (object) entity.Value.Count));
    }

    public void LogComponents()
    {
      foreach (KeyValuePair<Type, List<Component>> component in this.Components)
        Engine.Commands.Log((object) (component.Key.Name + " : " + (object) component.Value.Count));
    }
  }
}
