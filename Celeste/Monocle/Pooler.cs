// Decompiled with JetBrains decompiler
// Type: Monocle.Pooler
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Monocle
{
  public class Pooler
  {
    internal Dictionary<Type, Queue<Entity>> Pools { get; private set; }

    public Pooler()
    {
      this.Pools = new Dictionary<Type, Queue<Entity>>();
      foreach (Type type in Assembly.GetEntryAssembly().GetTypes())
      {
        if (type.GetCustomAttributes(typeof (Pooled), false).Length != 0)
        {
          if (!typeof (Entity).IsAssignableFrom(type))
            throw new Exception("Type '" + type.Name + "' cannot be Pooled because it doesn't derive from Entity");
          if (type.GetConstructor(Type.EmptyTypes) == (ConstructorInfo) null)
            throw new Exception("Type '" + type.Name + "' cannot be Pooled because it doesn't have a parameterless constructor");
          this.Pools.Add(type, new Queue<Entity>());
        }
      }
    }

    public T Create<T>() where T : Entity, new()
    {
      if (!this.Pools.ContainsKey(typeof (T)))
        return new T();
      Queue<Entity> pool = this.Pools[typeof (T)];
      if (pool.Count == 0)
        return new T();
      return pool.Dequeue() as T;
    }

    internal void EntityRemoved(Entity entity)
    {
      Type type = entity.GetType();
      if (!this.Pools.ContainsKey(type))
        return;
      this.Pools[type].Enqueue(entity);
    }

    public void Log()
    {
      if (this.Pools.Count == 0)
        Engine.Commands.Log((object) "No Entity types are marked as Pooled!");
      foreach (KeyValuePair<Type, Queue<Entity>> pool in this.Pools)
        Engine.Commands.Log((object) (pool.Key.Name + " : " + (object) pool.Value.Count));
    }
  }
}
