// Decompiled with JetBrains decompiler
// Type: Celeste.PreviewPostcard
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Monocle;
using System.Collections;

namespace Celeste
{
  public class PreviewPostcard : Scene
  {
    private Postcard postcard;

    public PreviewPostcard(string msg, int area)
    {
      this.Add(new Entity()
      {
        (Component) new Coroutine(this.Routine(msg, area), true)
      });
      this.Add((Monocle.Renderer) new HudRenderer());
    }

    private IEnumerator Routine(string id, int area)
    {
      PreviewPostcard previewPostcard = this;
      yield return (object) 0.25f;
      previewPostcard.Add((Entity) (previewPostcard.postcard = new Postcard(Dialog.Get(id, (Language) null), area)));
      yield return (object) previewPostcard.postcard.DisplayRoutine();
      Engine.Scene = (Scene) new OverworldLoader(Overworld.StartMode.MainMenu, (HiresSnow) null);
    }

    public override void BeforeRender()
    {
      base.BeforeRender();
      if (this.postcard == null)
        return;
      this.postcard.BeforeRender();
    }
  }
}
