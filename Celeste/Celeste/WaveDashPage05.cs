// Decompiled with JetBrains decompiler
// Type: Celeste.WaveDashPage05
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste
{
  public class WaveDashPage05 : WaveDashPage
  {
    private List<WaveDashPage05.Display> displays = new List<WaveDashPage05.Display>();

    public WaveDashPage05()
    {
      this.Transition = WaveDashPage.Transitions.Spiral;
      this.ClearColor = Calc.HexToColor("fff2cc");
    }

    public override void Added(WaveDashPresentation presentation)
    {
      base.Added(presentation);
      this.displays.Add(new WaveDashPage05.Display(new Vector2((float) this.Width * 0.28f, (float) (this.Height - 600)), Dialog.Get("WAVEDASH_PAGE5_INFO1"), "too_close", new Vector2(-50f, 20f)));
      this.displays.Add(new WaveDashPage05.Display(new Vector2((float) this.Width * 0.72f, (float) (this.Height - 600)), Dialog.Get("WAVEDASH_PAGE5_INFO2"), "too_far", new Vector2(-50f, -35f)));
    }

    public override IEnumerator Routine()
    {
      yield return (object) 0.5f;
    }

    public override void Update()
    {
      foreach (WaveDashPage05.Display display in this.displays)
        display.Update();
    }

    public override void Render()
    {
      ActiveFont.DrawOutline(Dialog.Clean("WAVEDASH_PAGE5_TITLE"), new Vector2(128f, 100f), Vector2.Zero, Vector2.One * 1.5f, Color.White, 2f, Color.Black);
      foreach (WaveDashPage05.Display display in this.displays)
        display.Render();
    }

    private class Display
    {
      public Vector2 Position;
      public FancyText.Text Info;
      public WaveDashPlaybackTutorial Tutorial;
      private Coroutine routine;
      private float xEase;
      private float time;

      public Display(Vector2 position, string text, string tutorial, Vector2 tutorialOffset)
      {
        this.Position = position;
        this.Info = FancyText.Parse(text, 896, 8, defaultColor: new Color?(Color.Black * 0.6f));
        this.Tutorial = new WaveDashPlaybackTutorial(tutorial, tutorialOffset, new Vector2(1f, 1f), new Vector2(1f, 1f));
        this.Tutorial.OnRender = (Action) (() => Draw.Line(-64f, 20f, 64f, 20f, Color.Black));
        this.routine = new Coroutine(this.Routine());
      }

      private IEnumerator Routine()
      {
        PlayerPlayback playback = this.Tutorial.Playback;
        int step = 0;
        while (true)
        {
          int frameIndex1 = playback.FrameIndex;
          if (step % 2 == 0)
            this.Tutorial.Update();
          int frameIndex2 = playback.FrameIndex;
          if (frameIndex1 != frameIndex2 && playback.FrameIndex == playback.FrameCount - 1)
          {
            while ((double) this.time < 3.0)
              yield return (object) null;
            yield return (object) 0.1f;
            while ((double) this.xEase < 1.0)
            {
              this.xEase = Calc.Approach(this.xEase, 1f, Engine.DeltaTime * 4f);
              yield return (object) null;
            }
            this.xEase = 1f;
            yield return (object) 0.5f;
            this.xEase = 0.0f;
            this.time = 0.0f;
          }
          ++step;
          yield return (object) null;
        }
      }

      public void Update()
      {
        this.time += Engine.DeltaTime;
        this.routine.Update();
      }

      public void Render()
      {
        this.Tutorial.Render(this.Position, 4f);
        this.Info.DrawJustifyPerLine(this.Position + Vector2.UnitY * 200f, new Vector2(0.5f, 0.0f), Vector2.One * 0.8f, 1f);
        if ((double) this.xEase <= 0.0)
          return;
        Vector2 vector = Calc.AngleToVector((float) ((1.0 - (double) this.xEase) * 0.10000000149011612 + 0.7853981852531433), 1f);
        Vector2 vector2 = vector.Perpendicular();
        float num1 = (float) (0.5 + (1.0 - (double) this.xEase) * 0.5);
        float thickness = 64f * num1;
        float num2 = 300f * num1;
        Vector2 position = this.Position;
        Draw.Line(position - vector * num2, position + vector * num2, Color.Red, thickness);
        Draw.Line(position - vector2 * num2, position + vector2 * num2, Color.Red, thickness);
      }
    }
  }
}
