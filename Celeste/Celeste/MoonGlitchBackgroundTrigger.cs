// Decompiled with JetBrains decompiler
// Type: Celeste.MoonGlitchBackgroundTrigger
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class MoonGlitchBackgroundTrigger : Trigger
  {
    private MoonGlitchBackgroundTrigger.Duration duration;
    private bool triggered;
    private bool stayOn;
    private bool running;
    private bool doGlitch;

    public MoonGlitchBackgroundTrigger(EntityData data, Vector2 offset)
      : base(data, offset)
    {
      this.duration = data.Enum<MoonGlitchBackgroundTrigger.Duration>(nameof (duration));
      this.stayOn = data.Bool("stay");
      this.doGlitch = data.Bool("glitch", true);
    }

    public override void OnEnter(Player player) => this.Invoke();

    public void Invoke()
    {
      if (this.triggered)
        return;
      this.triggered = true;
      if (this.doGlitch)
      {
        this.Add((Component) new Coroutine(this.InternalGlitchRoutine()));
      }
      else
      {
        if (this.stayOn)
          return;
        MoonGlitchBackgroundTrigger.Toggle(false);
      }
    }

    private IEnumerator InternalGlitchRoutine()
    {
      MoonGlitchBackgroundTrigger backgroundTrigger = this;
      backgroundTrigger.running = true;
      backgroundTrigger.Tag = (int) Tags.Persistent;
      float duration;
      if (backgroundTrigger.duration == MoonGlitchBackgroundTrigger.Duration.Short)
      {
        duration = 0.2f;
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
        Audio.Play("event:/new_content/game/10_farewell/glitch_short");
      }
      else if (backgroundTrigger.duration == MoonGlitchBackgroundTrigger.Duration.Medium)
      {
        duration = 0.5f;
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
        Audio.Play("event:/new_content/game/10_farewell/glitch_medium");
      }
      else
      {
        duration = 1.25f;
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
        Audio.Play("event:/new_content/game/10_farewell/glitch_long");
      }
      yield return (object) MoonGlitchBackgroundTrigger.GlitchRoutine(duration, backgroundTrigger.stayOn);
      backgroundTrigger.Tag = 0;
      backgroundTrigger.running = false;
    }

    private static void Toggle(bool on)
    {
      Level scene = Engine.Scene as Level;
      foreach (Backdrop backdrop in scene.Background.GetEach<Backdrop>("blackhole"))
        backdrop.ForceVisible = on;
      foreach (Backdrop backdrop in scene.Foreground.GetEach<Backdrop>("blackhole"))
        backdrop.ForceVisible = on;
    }

    private static void Fade(float alpha, bool max = false)
    {
      Level scene = Engine.Scene as Level;
      foreach (Backdrop backdrop in scene.Background.GetEach<Backdrop>("blackhole"))
        backdrop.FadeAlphaMultiplier = max ? Math.Max(backdrop.FadeAlphaMultiplier, alpha) : alpha;
      foreach (Backdrop backdrop in scene.Foreground.GetEach<Backdrop>("blackhole"))
        backdrop.FadeAlphaMultiplier = max ? Math.Max(backdrop.FadeAlphaMultiplier, alpha) : alpha;
    }

    public static IEnumerator GlitchRoutine(float duration, bool stayOn)
    {
      MoonGlitchBackgroundTrigger.Toggle(true);
      if (Settings.Instance.DisableFlashes)
      {
        float a;
        for (a = 0.0f; (double) a < 1.0; a += Engine.DeltaTime / 0.1f)
        {
          MoonGlitchBackgroundTrigger.Fade(a, true);
          yield return (object) null;
        }
        MoonGlitchBackgroundTrigger.Fade(1f);
        yield return (object) duration;
        if (!stayOn)
        {
          for (a = 0.0f; (double) a < 1.0; a += Engine.DeltaTime / 0.1f)
          {
            MoonGlitchBackgroundTrigger.Fade(1f - a);
            yield return (object) null;
          }
          MoonGlitchBackgroundTrigger.Fade(1f);
        }
      }
      else if ((double) duration > 0.4000000059604645)
      {
        Glitch.Value = 0.3f;
        yield return (object) 0.2f;
        Glitch.Value = 0.0f;
        yield return (object) (float) ((double) duration - 0.4000000059604645);
        if (!stayOn)
          Glitch.Value = 0.3f;
        yield return (object) 0.2f;
        Glitch.Value = 0.0f;
      }
      else
      {
        Glitch.Value = 0.3f;
        yield return (object) duration;
        Glitch.Value = 0.0f;
      }
      if (!stayOn)
        MoonGlitchBackgroundTrigger.Toggle(false);
    }

    public override void Removed(Scene scene)
    {
      if (this.running)
      {
        Glitch.Value = 0.0f;
        MoonGlitchBackgroundTrigger.Fade(1f);
        if (!this.stayOn)
          MoonGlitchBackgroundTrigger.Toggle(false);
      }
      base.Removed(scene);
    }

    private enum Duration
    {
      Short,
      Medium,
      Long,
    }
  }
}
