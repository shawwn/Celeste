// Decompiled with JetBrains decompiler
// Type: Celeste.CS07_Ascend
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste
{
  public class CS07_Ascend : CutsceneEntity
  {
    private int index;
    private string cutscene;
    private BadelineDummy badeline;
    private Player player;
    private Vector2 origin;
    private bool spinning;

    public CS07_Ascend(int index, string cutscene)
      : base(true, false)
    {
      this.index = index;
      this.cutscene = cutscene;
    }

    public override void OnBegin(Level level)
    {
      this.Add((Component) new Coroutine(this.Cutscene(), true));
    }

    private IEnumerator Cutscene()
    {
      while ((this.player = this.Scene.Tracker.GetEntity<Player>()) == null)
        yield return (object) null;
      this.origin = this.player.Position;
      Audio.Play("event:/char/badeline/maddy_split");
      this.player.CreateSplitParticles();
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      this.Level.Displacement.AddBurst(this.player.Position, 0.4f, 8f, 32f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
      this.player.Dashes = 1;
      this.player.Facing = Facings.Right;
      this.Scene.Add((Entity) (this.badeline = new BadelineDummy(this.player.Position)));
      this.badeline.AutoAnimator.Enabled = false;
      this.spinning = true;
      this.Add((Component) new Coroutine(this.SpinCharacters(), true));
      yield return (object) Textbox.Say(this.cutscene);
      Audio.Play("event:/char/badeline/maddy_join");
      this.spinning = false;
      yield return (object) 0.25f;
      this.badeline.RemoveSelf();
      this.player.Dashes = 2;
      this.player.CreateSplitParticles();
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      this.Level.Displacement.AddBurst(this.player.Position, 0.4f, 8f, 32f, 0.5f, (Ease.Easer) null, (Ease.Easer) null);
      this.EndCutscene(this.Level, true);
    }

    private IEnumerator SpinCharacters()
    {
      float dist = 0.0f;
      Vector2 center = this.player.Position;
      float timer = 1.570796f;
      this.player.Sprite.Play("spin", false, false);
      this.badeline.Sprite.Play("spin", false, false);
      this.badeline.Sprite.Scale.X = 1f;
      while (this.spinning || (double) dist > 0.0)
      {
        dist = Calc.Approach(dist, this.spinning ? 1f : 0.0f, Engine.DeltaTime * 4f);
        int frame = (int) ((double) timer / 6.28318548202515 * 14.0 + 10.0);
        float sin = (float) Math.Sin((double) timer);
        float cos = (float) Math.Cos((double) timer);
        float len = Ease.CubeOut(dist) * 32f;
        this.player.Sprite.SetAnimationFrame(frame);
        this.badeline.Sprite.SetAnimationFrame(frame + 7);
        this.player.Position = center - new Vector2(sin * len, (float) ((double) cos * (double) dist * 8.0));
        this.badeline.Position = center + new Vector2(sin * len, (float) ((double) cos * (double) dist * 8.0));
        timer -= Engine.DeltaTime * 2f;
        if ((double) timer <= 0.0)
          timer += 6.283185f;
        yield return (object) null;
      }
    }

    public override void OnEnd(Level level)
    {
      if (this.badeline != null)
        this.badeline.RemoveSelf();
      if (this.player != null)
      {
        this.player.Dashes = 2;
        this.player.Position = this.origin;
      }
      level.Add((Entity) new HeightDisplay(this.index));
    }
  }
}

