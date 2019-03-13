// Decompiled with JetBrains decompiler
// Type: Monocle.Scene
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Monocle
{
  public class Scene : IEnumerable<Entity>, IEnumerable
  {
    public bool Paused;
    public float TimeActive;
    public float RawTimeActive;
    private Dictionary<int, double> actualDepthLookup;

    public bool Focused { get; private set; }

    public EntityList Entities { get; private set; }

    public TagLists TagLists { get; private set; }

    public RendererList RendererList { get; private set; }

    public Entity HelperEntity { get; private set; }

    public Tracker Tracker { get; private set; }

    public event Action OnEndOfFrame;

    public Scene()
    {
      this.Tracker = new Tracker();
      this.Entities = new EntityList(this);
      this.TagLists = new TagLists();
      this.RendererList = new RendererList(this);
      this.actualDepthLookup = new Dictionary<int, double>();
      this.HelperEntity = new Entity();
      this.Entities.Add(this.HelperEntity);
    }

    public virtual void Begin()
    {
      this.Focused = true;
      foreach (Entity entity in this.Entities)
        entity.SceneBegin(this);
    }

    public virtual void End()
    {
      this.Focused = false;
      foreach (Entity entity in this.Entities)
        entity.SceneEnd(this);
    }

    public virtual void BeforeUpdate()
    {
      if (!this.Paused)
        this.TimeActive += Engine.DeltaTime;
      this.RawTimeActive += Engine.RawDeltaTime;
      this.Entities.UpdateLists();
      this.TagLists.UpdateLists();
      this.RendererList.UpdateLists();
    }

    public virtual void Update()
    {
      if (this.Paused)
        return;
      this.Entities.Update();
      this.RendererList.Update();
    }

    public virtual void AfterUpdate()
    {
      if (this.OnEndOfFrame == null)
        return;
      this.OnEndOfFrame();
      this.OnEndOfFrame = (Action) null;
    }

    public virtual void BeforeRender()
    {
      this.RendererList.BeforeRender();
    }

    public virtual void Render()
    {
      this.RendererList.Render();
    }

    public virtual void AfterRender()
    {
      this.RendererList.AfterRender();
    }

    public virtual void HandleGraphicsReset()
    {
      this.Entities.HandleGraphicsReset();
    }

    public virtual void HandleGraphicsCreate()
    {
      this.Entities.HandleGraphicsCreate();
    }

    public virtual void GainFocus()
    {
    }

    public virtual void LoseFocus()
    {
    }

    public bool OnInterval(float interval)
    {
      return (int) (((double) this.TimeActive - (double) Engine.DeltaTime) / (double) interval) < (int) ((double) this.TimeActive / (double) interval);
    }

    public bool OnInterval(float interval, float offset)
    {
      return Math.Floor(((double) this.TimeActive - (double) offset - (double) Engine.DeltaTime) / (double) interval) < Math.Floor(((double) this.TimeActive - (double) offset) / (double) interval);
    }

    public bool BetweenInterval(float interval)
    {
      return Calc.BetweenInterval(this.TimeActive, interval);
    }

    public bool OnRawInterval(float interval)
    {
      return (int) (((double) this.RawTimeActive - (double) Engine.RawDeltaTime) / (double) interval) < (int) ((double) this.RawTimeActive / (double) interval);
    }

    public bool OnRawInterval(float interval, float offset)
    {
      return Math.Floor(((double) this.RawTimeActive - (double) offset - (double) Engine.RawDeltaTime) / (double) interval) < Math.Floor(((double) this.RawTimeActive - (double) offset) / (double) interval);
    }

    public bool BetweenRawInterval(float interval)
    {
      return Calc.BetweenInterval(this.RawTimeActive, interval);
    }

    public bool CollideCheck(Vector2 point, int tag)
    {
      List<Entity> tagList = this.TagLists[tag];
      for (int index = 0; index < tagList.Count; ++index)
      {
        if (tagList[index].Collidable && tagList[index].CollidePoint(point))
          return true;
      }
      return false;
    }

    public bool CollideCheck(Vector2 from, Vector2 to, int tag)
    {
      List<Entity> tagList = this.TagLists[tag];
      for (int index = 0; index < tagList.Count; ++index)
      {
        if (tagList[index].Collidable && tagList[index].CollideLine(from, to))
          return true;
      }
      return false;
    }

    public bool CollideCheck(Rectangle rect, int tag)
    {
      List<Entity> tagList = this.TagLists[tag];
      for (int index = 0; index < tagList.Count; ++index)
      {
        if (tagList[index].Collidable && tagList[index].CollideRect(rect))
          return true;
      }
      return false;
    }

    public bool CollideCheck(Rectangle rect, Entity entity)
    {
      return entity.Collidable && entity.CollideRect(rect);
    }

    public Entity CollideFirst(Vector2 point, int tag)
    {
      List<Entity> tagList = this.TagLists[tag];
      for (int index = 0; index < tagList.Count; ++index)
      {
        if (tagList[index].Collidable && tagList[index].CollidePoint(point))
          return tagList[index];
      }
      return (Entity) null;
    }

    public Entity CollideFirst(Vector2 from, Vector2 to, int tag)
    {
      List<Entity> tagList = this.TagLists[tag];
      for (int index = 0; index < tagList.Count; ++index)
      {
        if (tagList[index].Collidable && tagList[index].CollideLine(from, to))
          return tagList[index];
      }
      return (Entity) null;
    }

    public Entity CollideFirst(Rectangle rect, int tag)
    {
      List<Entity> tagList = this.TagLists[tag];
      for (int index = 0; index < tagList.Count; ++index)
      {
        if (tagList[index].Collidable && tagList[index].CollideRect(rect))
          return tagList[index];
      }
      return (Entity) null;
    }

    public void CollideInto(Vector2 point, int tag, List<Entity> hits)
    {
      List<Entity> tagList = this.TagLists[tag];
      for (int index = 0; index < tagList.Count; ++index)
      {
        if (tagList[index].Collidable && tagList[index].CollidePoint(point))
          hits.Add(tagList[index]);
      }
    }

    public void CollideInto(Vector2 from, Vector2 to, int tag, List<Entity> hits)
    {
      List<Entity> tagList = this.TagLists[tag];
      for (int index = 0; index < tagList.Count; ++index)
      {
        if (tagList[index].Collidable && tagList[index].CollideLine(from, to))
          hits.Add(tagList[index]);
      }
    }

    public void CollideInto(Rectangle rect, int tag, List<Entity> hits)
    {
      List<Entity> tagList = this.TagLists[tag];
      for (int index = 0; index < tagList.Count; ++index)
      {
        if (tagList[index].Collidable && tagList[index].CollideRect(rect))
          tagList.Add(tagList[index]);
      }
    }

    public List<Entity> CollideAll(Vector2 point, int tag)
    {
      List<Entity> hits = new List<Entity>();
      this.CollideInto(point, tag, hits);
      return hits;
    }

    public List<Entity> CollideAll(Vector2 from, Vector2 to, int tag)
    {
      List<Entity> hits = new List<Entity>();
      this.CollideInto(from, to, tag, hits);
      return hits;
    }

    public List<Entity> CollideAll(Rectangle rect, int tag)
    {
      List<Entity> hits = new List<Entity>();
      this.CollideInto(rect, tag, hits);
      return hits;
    }

    public void CollideDo(Vector2 point, int tag, Action<Entity> action)
    {
      List<Entity> tagList = this.TagLists[tag];
      for (int index = 0; index < tagList.Count; ++index)
      {
        if (tagList[index].Collidable && tagList[index].CollidePoint(point))
          action(tagList[index]);
      }
    }

    public void CollideDo(Vector2 from, Vector2 to, int tag, Action<Entity> action)
    {
      List<Entity> tagList = this.TagLists[tag];
      for (int index = 0; index < tagList.Count; ++index)
      {
        if (tagList[index].Collidable && tagList[index].CollideLine(from, to))
          action(tagList[index]);
      }
    }

    public void CollideDo(Rectangle rect, int tag, Action<Entity> action)
    {
      List<Entity> tagList = this.TagLists[tag];
      for (int index = 0; index < tagList.Count; ++index)
      {
        if (tagList[index].Collidable && tagList[index].CollideRect(rect))
          action(tagList[index]);
      }
    }

    public Vector2 LineWalkCheck(Vector2 from, Vector2 to, int tag, float precision)
    {
      Vector2 vector2_1 = to - from;
      vector2_1.Normalize();
      Vector2 vector2_2 = vector2_1 * precision;
      int num = (int) Math.Floor((double) (from - to).Length() / (double) precision);
      Vector2 vector2_3 = from;
      Vector2 point = from + vector2_2;
      for (int index = 0; index <= num; ++index)
      {
        if (this.CollideCheck(point, tag))
          return vector2_3;
        vector2_3 = point;
        point += vector2_2;
      }
      return to;
    }

    public bool CollideCheck<T>(Vector2 point) where T : Entity
    {
      List<Entity> entity = this.Tracker.Entities[typeof (T)];
      for (int index = 0; index < entity.Count; ++index)
      {
        if (entity[index].Collidable && entity[index].CollidePoint(point))
          return true;
      }
      return false;
    }

    public bool CollideCheck<T>(Vector2 from, Vector2 to) where T : Entity
    {
      List<Entity> entity = this.Tracker.Entities[typeof (T)];
      for (int index = 0; index < entity.Count; ++index)
      {
        if (entity[index].Collidable && entity[index].CollideLine(from, to))
          return true;
      }
      return false;
    }

    public bool CollideCheck<T>(Rectangle rect) where T : Entity
    {
      List<Entity> entity = this.Tracker.Entities[typeof (T)];
      for (int index = 0; index < entity.Count; ++index)
      {
        if (entity[index].Collidable && entity[index].CollideRect(rect))
          return true;
      }
      return false;
    }

    public T CollideFirst<T>(Vector2 point) where T : Entity
    {
      List<Entity> entity = this.Tracker.Entities[typeof (T)];
      for (int index = 0; index < entity.Count; ++index)
      {
        if (entity[index].Collidable && entity[index].CollidePoint(point))
          return entity[index] as T;
      }
      return default (T);
    }

    public T CollideFirst<T>(Vector2 from, Vector2 to) where T : Entity
    {
      List<Entity> entity = this.Tracker.Entities[typeof (T)];
      for (int index = 0; index < entity.Count; ++index)
      {
        if (entity[index].Collidable && entity[index].CollideLine(from, to))
          return entity[index] as T;
      }
      return default (T);
    }

    public T CollideFirst<T>(Rectangle rect) where T : Entity
    {
      List<Entity> entity = this.Tracker.Entities[typeof (T)];
      for (int index = 0; index < entity.Count; ++index)
      {
        if (entity[index].Collidable && entity[index].CollideRect(rect))
          return entity[index] as T;
      }
      return default (T);
    }

    public void CollideInto<T>(Vector2 point, List<Entity> hits) where T : Entity
    {
      List<Entity> entity = this.Tracker.Entities[typeof (T)];
      for (int index = 0; index < entity.Count; ++index)
      {
        if (entity[index].Collidable && entity[index].CollidePoint(point))
          hits.Add(entity[index]);
      }
    }

    public void CollideInto<T>(Vector2 from, Vector2 to, List<Entity> hits) where T : Entity
    {
      List<Entity> entity = this.Tracker.Entities[typeof (T)];
      for (int index = 0; index < entity.Count; ++index)
      {
        if (entity[index].Collidable && entity[index].CollideLine(from, to))
          hits.Add(entity[index]);
      }
    }

    public void CollideInto<T>(Rectangle rect, List<Entity> hits) where T : Entity
    {
      List<Entity> entity = this.Tracker.Entities[typeof (T)];
      for (int index = 0; index < entity.Count; ++index)
      {
        if (entity[index].Collidable && entity[index].CollideRect(rect))
          entity.Add(entity[index]);
      }
    }

    public void CollideInto<T>(Vector2 point, List<T> hits) where T : Entity
    {
      List<Entity> entity = this.Tracker.Entities[typeof (T)];
      for (int index = 0; index < entity.Count; ++index)
      {
        if (entity[index].Collidable && entity[index].CollidePoint(point))
          hits.Add(entity[index] as T);
      }
    }

    public void CollideInto<T>(Vector2 from, Vector2 to, List<T> hits) where T : Entity
    {
      List<Entity> entity = this.Tracker.Entities[typeof (T)];
      for (int index = 0; index < entity.Count; ++index)
      {
        if (entity[index].Collidable && entity[index].CollideLine(from, to))
          hits.Add(entity[index] as T);
      }
    }

    public void CollideInto<T>(Rectangle rect, List<T> hits) where T : Entity
    {
      List<Entity> entity = this.Tracker.Entities[typeof (T)];
      for (int index = 0; index < entity.Count; ++index)
      {
        if (entity[index].Collidable && entity[index].CollideRect(rect))
          hits.Add(entity[index] as T);
      }
    }

    public List<T> CollideAll<T>(Vector2 point) where T : Entity
    {
      List<T> hits = new List<T>();
      this.CollideInto<T>(point, hits);
      return hits;
    }

    public List<T> CollideAll<T>(Vector2 from, Vector2 to) where T : Entity
    {
      List<T> hits = new List<T>();
      this.CollideInto<T>(from, to, hits);
      return hits;
    }

    public List<T> CollideAll<T>(Rectangle rect) where T : Entity
    {
      List<T> hits = new List<T>();
      this.CollideInto<T>(rect, hits);
      return hits;
    }

    public void CollideDo<T>(Vector2 point, Action<T> action) where T : Entity
    {
      List<Entity> entity = this.Tracker.Entities[typeof (T)];
      for (int index = 0; index < entity.Count; ++index)
      {
        if (entity[index].Collidable && entity[index].CollidePoint(point))
          action(entity[index] as T);
      }
    }

    public void CollideDo<T>(Vector2 from, Vector2 to, Action<T> action) where T : Entity
    {
      List<Entity> entity = this.Tracker.Entities[typeof (T)];
      for (int index = 0; index < entity.Count; ++index)
      {
        if (entity[index].Collidable && entity[index].CollideLine(from, to))
          action(entity[index] as T);
      }
    }

    public void CollideDo<T>(Rectangle rect, Action<T> action) where T : Entity
    {
      List<Entity> entity = this.Tracker.Entities[typeof (T)];
      for (int index = 0; index < entity.Count; ++index)
      {
        if (entity[index].Collidable && entity[index].CollideRect(rect))
          action(entity[index] as T);
      }
    }

    public Vector2 LineWalkCheck<T>(Vector2 from, Vector2 to, float precision) where T : Entity
    {
      Vector2 vector2_1 = to - from;
      vector2_1.Normalize();
      Vector2 vector2_2 = vector2_1 * precision;
      int num = (int) Math.Floor((double) (from - to).Length() / (double) precision);
      Vector2 vector2_3 = from;
      Vector2 point = from + vector2_2;
      for (int index = 0; index <= num; ++index)
      {
        if (this.CollideCheck<T>(point))
          return vector2_3;
        vector2_3 = point;
        point += vector2_2;
      }
      return to;
    }

    public bool CollideCheckByComponent<T>(Vector2 point) where T : Component
    {
      List<Component> component = this.Tracker.Components[typeof (T)];
      for (int index = 0; index < component.Count; ++index)
      {
        if (component[index].Entity.Collidable && component[index].Entity.CollidePoint(point))
          return true;
      }
      return false;
    }

    public bool CollideCheckByComponent<T>(Vector2 from, Vector2 to) where T : Component
    {
      List<Component> component = this.Tracker.Components[typeof (T)];
      for (int index = 0; index < component.Count; ++index)
      {
        if (component[index].Entity.Collidable && component[index].Entity.CollideLine(from, to))
          return true;
      }
      return false;
    }

    public bool CollideCheckByComponent<T>(Rectangle rect) where T : Component
    {
      List<Component> component = this.Tracker.Components[typeof (T)];
      for (int index = 0; index < component.Count; ++index)
      {
        if (component[index].Entity.Collidable && component[index].Entity.CollideRect(rect))
          return true;
      }
      return false;
    }

    public T CollideFirstByComponent<T>(Vector2 point) where T : Component
    {
      List<Component> component = this.Tracker.Components[typeof (T)];
      for (int index = 0; index < component.Count; ++index)
      {
        if (component[index].Entity.Collidable && component[index].Entity.CollidePoint(point))
          return component[index] as T;
      }
      return default (T);
    }

    public T CollideFirstByComponent<T>(Vector2 from, Vector2 to) where T : Component
    {
      List<Component> component = this.Tracker.Components[typeof (T)];
      for (int index = 0; index < component.Count; ++index)
      {
        if (component[index].Entity.Collidable && component[index].Entity.CollideLine(from, to))
          return component[index] as T;
      }
      return default (T);
    }

    public T CollideFirstByComponent<T>(Rectangle rect) where T : Component
    {
      List<Component> component = this.Tracker.Components[typeof (T)];
      for (int index = 0; index < component.Count; ++index)
      {
        if (component[index].Entity.Collidable && component[index].Entity.CollideRect(rect))
          return component[index] as T;
      }
      return default (T);
    }

    public void CollideIntoByComponent<T>(Vector2 point, List<Component> hits) where T : Component
    {
      List<Component> component = this.Tracker.Components[typeof (T)];
      for (int index = 0; index < component.Count; ++index)
      {
        if (component[index].Entity.Collidable && component[index].Entity.CollidePoint(point))
          hits.Add(component[index]);
      }
    }

    public void CollideIntoByComponent<T>(Vector2 from, Vector2 to, List<Component> hits) where T : Component
    {
      List<Component> component = this.Tracker.Components[typeof (T)];
      for (int index = 0; index < component.Count; ++index)
      {
        if (component[index].Entity.Collidable && component[index].Entity.CollideLine(from, to))
          hits.Add(component[index]);
      }
    }

    public void CollideIntoByComponent<T>(Rectangle rect, List<Component> hits) where T : Component
    {
      List<Component> component = this.Tracker.Components[typeof (T)];
      for (int index = 0; index < component.Count; ++index)
      {
        if (component[index].Entity.Collidable && component[index].Entity.CollideRect(rect))
          component.Add(component[index]);
      }
    }

    public void CollideIntoByComponent<T>(Vector2 point, List<T> hits) where T : Component
    {
      List<Component> component = this.Tracker.Components[typeof (T)];
      for (int index = 0; index < component.Count; ++index)
      {
        if (component[index].Entity.Collidable && component[index].Entity.CollidePoint(point))
          hits.Add(component[index] as T);
      }
    }

    public void CollideIntoByComponent<T>(Vector2 from, Vector2 to, List<T> hits) where T : Component
    {
      List<Component> component = this.Tracker.Components[typeof (T)];
      for (int index = 0; index < component.Count; ++index)
      {
        if (component[index].Entity.Collidable && component[index].Entity.CollideLine(from, to))
          hits.Add(component[index] as T);
      }
    }

    public void CollideIntoByComponent<T>(Rectangle rect, List<T> hits) where T : Component
    {
      List<Component> component = this.Tracker.Components[typeof (T)];
      for (int index = 0; index < component.Count; ++index)
      {
        if (component[index].Entity.Collidable && component[index].Entity.CollideRect(rect))
          component.Add((Component) (component[index] as T));
      }
    }

    public List<T> CollideAllByComponent<T>(Vector2 point) where T : Component
    {
      List<T> hits = new List<T>();
      this.CollideIntoByComponent<T>(point, hits);
      return hits;
    }

    public List<T> CollideAllByComponent<T>(Vector2 from, Vector2 to) where T : Component
    {
      List<T> hits = new List<T>();
      this.CollideIntoByComponent<T>(from, to, hits);
      return hits;
    }

    public List<T> CollideAllByComponent<T>(Rectangle rect) where T : Component
    {
      List<T> hits = new List<T>();
      this.CollideIntoByComponent<T>(rect, hits);
      return hits;
    }

    public void CollideDoByComponent<T>(Vector2 point, Action<T> action) where T : Component
    {
      List<Component> component = this.Tracker.Components[typeof (T)];
      for (int index = 0; index < component.Count; ++index)
      {
        if (component[index].Entity.Collidable && component[index].Entity.CollidePoint(point))
          action(component[index] as T);
      }
    }

    public void CollideDoByComponent<T>(Vector2 from, Vector2 to, Action<T> action) where T : Component
    {
      List<Component> component = this.Tracker.Components[typeof (T)];
      for (int index = 0; index < component.Count; ++index)
      {
        if (component[index].Entity.Collidable && component[index].Entity.CollideLine(from, to))
          action(component[index] as T);
      }
    }

    public void CollideDoByComponent<T>(Rectangle rect, Action<T> action) where T : Component
    {
      List<Component> component = this.Tracker.Components[typeof (T)];
      for (int index = 0; index < component.Count; ++index)
      {
        if (component[index].Entity.Collidable && component[index].Entity.CollideRect(rect))
          action(component[index] as T);
      }
    }

    public Vector2 LineWalkCheckByComponent<T>(Vector2 from, Vector2 to, float precision) where T : Component
    {
      Vector2 vector2_1 = to - from;
      vector2_1.Normalize();
      Vector2 vector2_2 = vector2_1 * precision;
      int num = (int) Math.Floor((double) (from - to).Length() / (double) precision);
      Vector2 vector2_3 = from;
      Vector2 point = from + vector2_2;
      for (int index = 0; index <= num; ++index)
      {
        if (this.CollideCheckByComponent<T>(point))
          return vector2_3;
        vector2_3 = point;
        point += vector2_2;
      }
      return to;
    }

    internal void SetActualDepth(Entity entity)
    {
      double num = 0.0;
      if (this.actualDepthLookup.TryGetValue(entity.depth, out num))
        this.actualDepthLookup[entity.depth] += 9.99999997475243E-07;
      else
        this.actualDepthLookup.Add(entity.depth, 9.99999997475243E-07);
      entity.actualDepth = (double) entity.depth - num;
      this.Entities.MarkUnsorted();
      for (int index = 0; index < BitTag.TotalTags; ++index)
      {
        if (entity.TagCheck(1 << index))
          this.TagLists.MarkUnsorted(index);
      }
    }

    public T CreateAndAdd<T>() where T : Entity, new()
    {
      T obj = Engine.Pooler.Create<T>();
      this.Add((Entity) obj);
      return obj;
    }

    public List<Entity> this[BitTag tag]
    {
      get
      {
        return this.TagLists[tag.ID];
      }
    }

    public void Add(Entity entity)
    {
      this.Entities.Add(entity);
    }

    public void Remove(Entity entity)
    {
      this.Entities.Remove(entity);
    }

    public void Add(IEnumerable<Entity> entities)
    {
      this.Entities.Add(entities);
    }

    public void Remove(IEnumerable<Entity> entities)
    {
      this.Entities.Remove(entities);
    }

    public void Add(params Entity[] entities)
    {
      this.Entities.Add(entities);
    }

    public void Remove(params Entity[] entities)
    {
      this.Entities.Remove(entities);
    }

    public IEnumerator<Entity> GetEnumerator()
    {
      return this.Entities.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    public List<Entity> GetEntitiesByTagMask(int mask)
    {
      List<Entity> entityList = new List<Entity>();
      foreach (Entity entity in this.Entities)
      {
        if ((uint) (entity.Tag & mask) > 0U)
          entityList.Add(entity);
      }
      return entityList;
    }

    public List<Entity> GetEntitiesExcludingTagMask(int mask)
    {
      List<Entity> entityList = new List<Entity>();
      foreach (Entity entity in this.Entities)
      {
        if ((entity.Tag & mask) == 0)
          entityList.Add(entity);
      }
      return entityList;
    }

    public void Add(Renderer renderer)
    {
      this.RendererList.Add(renderer);
    }

    public void Remove(Renderer renderer)
    {
      this.RendererList.Remove(renderer);
    }
  }
}

