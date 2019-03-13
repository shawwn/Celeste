// Decompiled with JetBrains decompiler
// Type: Celeste.Oui
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;
using System.Collections;

namespace Celeste
{
  public abstract class Oui : Entity
  {
    public bool Focused;

    public Overworld Overworld
    {
      get
      {
        return this.SceneAs<Overworld>();
      }
    }

    public bool Selected
    {
      get
      {
        return this.Overworld != null && this.Overworld.Current == this;
      }
    }

    public Oui()
    {
      this.AddTag((int) Tags.HUD);
    }

    public virtual bool IsStart(Overworld overworld, Overworld.StartMode start)
    {
      return false;
    }

    public abstract IEnumerator Enter(Oui from);

    public abstract IEnumerator Leave(Oui next);
  }
}

