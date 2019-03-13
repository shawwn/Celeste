// Decompiled with JetBrains decompiler
// Type: Monocle.Cache
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using System;
using System.Collections.Generic;

namespace Monocle
{
  public static class Cache
  {
    public static Dictionary<Type, Stack<Entity>> cache;

    private static void Init<T>() where T : Entity, new()
    {
      if (Cache.cache == null)
        Cache.cache = new Dictionary<Type, Stack<Entity>>();
      if (Cache.cache.ContainsKey(typeof (T)))
        return;
      Cache.cache.Add(typeof (T), new Stack<Entity>());
    }

    public static void Store<T>(T instance) where T : Entity, new()
    {
      Cache.Init<T>();
      Cache.cache[typeof (T)].Push((Entity) instance);
    }

    public static T Create<T>() where T : Entity, new()
    {
      Cache.Init<T>();
      if (Cache.cache[typeof (T)].Count > 0)
        return Cache.cache[typeof (T)].Pop() as T;
      return new T();
    }

    public static void Clear<T>() where T : Entity, new()
    {
      if (Cache.cache == null || !Cache.cache.ContainsKey(typeof (T)))
        return;
      Cache.cache[typeof (T)].Clear();
    }

    public static void ClearAll()
    {
      if (Cache.cache == null)
        return;
      foreach (KeyValuePair<Type, Stack<Entity>> keyValuePair in Cache.cache)
        keyValuePair.Value.Clear();
    }
  }
}
