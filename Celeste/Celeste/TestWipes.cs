// Decompiled with JetBrains decompiler
// Type: Celeste.TestWipes
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class TestWipes : Scene
  {
    private Color lastColor = Color.White;
    private Coroutine coroutine;

    public TestWipes()
    {
      this.coroutine = new Coroutine(this.routine(), true);
    }

    private IEnumerator routine()
    {
      float dur = 1f;
      yield return (object) 1f;
      while (true)
      {
        ScreenWipe.WipeColor = Color.Black;
        new CurtainWipe((Scene) this, false, (Action) null).Duration = dur;
        yield return (object) dur;
        this.lastColor = ScreenWipe.WipeColor;
        ScreenWipe.WipeColor = Calc.HexToColor("ff0034");
        new AngledWipe((Scene) this, false, (Action) null).Duration = dur;
        yield return (object) dur;
        this.lastColor = ScreenWipe.WipeColor;
        ScreenWipe.WipeColor = Calc.HexToColor("0b0960");
        new DreamWipe((Scene) this, false, (Action) null).Duration = dur;
        yield return (object) dur;
        this.lastColor = ScreenWipe.WipeColor;
        ScreenWipe.WipeColor = Calc.HexToColor("39bf00");
        new KeyDoorWipe((Scene) this, false, (Action) null).Duration = dur;
        yield return (object) dur;
        this.lastColor = ScreenWipe.WipeColor;
        ScreenWipe.WipeColor = Calc.HexToColor("4376b3");
        new WindWipe((Scene) this, false, (Action) null).Duration = dur;
        yield return (object) dur;
        this.lastColor = ScreenWipe.WipeColor;
        ScreenWipe.WipeColor = Calc.HexToColor("ffae00");
        new DropWipe((Scene) this, false, (Action) null).Duration = dur;
        yield return (object) dur;
        this.lastColor = ScreenWipe.WipeColor;
        ScreenWipe.WipeColor = Calc.HexToColor("cc54ff");
        new FallWipe((Scene) this, false, (Action) null).Duration = dur;
        yield return (object) dur;
        this.lastColor = ScreenWipe.WipeColor;
        ScreenWipe.WipeColor = Calc.HexToColor("ff007a");
        new MountainWipe((Scene) this, false, (Action) null).Duration = dur;
        yield return (object) dur;
        this.lastColor = ScreenWipe.WipeColor;
        ScreenWipe.WipeColor = Color.White;
        new HeartWipe((Scene) this, false, (Action) null).Duration = dur;
        yield return (object) dur;
        this.lastColor = ScreenWipe.WipeColor;
      }
    }

    public override void Update()
    {
      base.Update();
      this.coroutine.Update();
    }

    public override void Render()
    {
      Draw.SpriteBatch.Begin();
      Draw.Rect(-1f, -1f, 1920f, 1080f, this.lastColor);
      Draw.SpriteBatch.End();
      base.Render();
    }
  }
}

