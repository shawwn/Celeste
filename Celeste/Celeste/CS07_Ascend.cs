// Decompiled with JetBrains decompiler
// Type: Celeste.CS07_Ascend
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

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
    private bool dark;

    public CS07_Ascend(int index, string cutscene, bool dark)
      : base()
    {
      this.index = index;
      this.cutscene = cutscene;
      this.dark = dark;
    }

    public override void OnBegin(Level level) => this.Add((Component) new Coroutine(this.Cutscene()));

    private IEnumerator Cutscene()
    {
      CS07_Ascend cs07Ascend = this;
      while ((cs07Ascend.player = cs07Ascend.Scene.Tracker.GetEntity<Player>()) == null)
        yield return (object) null;
      cs07Ascend.origin = cs07Ascend.player.Position;
      Audio.Play("event:/char/badeline/maddy_split");
      cs07Ascend.player.CreateSplitParticles();
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      cs07Ascend.Level.Displacement.AddBurst(cs07Ascend.player.Position, 0.4f, 8f, 32f, 0.5f);
      cs07Ascend.player.Dashes = 1;
      cs07Ascend.player.Facing = Facings.Right;
      cs07Ascend.Scene.Add((Entity) (cs07Ascend.badeline = new BadelineDummy(cs07Ascend.player.Position)));
      cs07Ascend.badeline.AutoAnimator.Enabled = false;
      cs07Ascend.spinning = true;
      cs07Ascend.Add((Component) new Coroutine(cs07Ascend.SpinCharacters()));
      yield return (object) Textbox.Say(cs07Ascend.cutscene);
      Audio.Play("event:/char/badeline/maddy_join");
      cs07Ascend.spinning = false;
      yield return (object) 0.25f;
      cs07Ascend.badeline.RemoveSelf();
      cs07Ascend.player.Dashes = 2;
      cs07Ascend.player.CreateSplitParticles();
      Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
      cs07Ascend.Level.Displacement.AddBurst(cs07Ascend.player.Position, 0.4f, 8f, 32f, 0.5f);
      cs07Ascend.EndCutscene(cs07Ascend.Level);
    }

    private IEnumerator SpinCharacters()
    {
      float dist = 0.0f;
      Vector2 center = this.player.Position;
      float timer = 1.5707964f;
      this.player.Sprite.Play("spin");
      this.badeline.Sprite.Play("spin");
      this.badeline.Sprite.Scale.X = 1f;
      while (this.spinning || (double) dist > 0.0)
      {
        dist = Calc.Approach(dist, this.spinning ? 1f : 0.0f, Engine.DeltaTime * 4f);
        int frame = (int) ((double) timer / 6.2831854820251465 * 14.0 + 10.0);
        float num1 = (float) Math.Sin((double) timer);
        float num2 = (float) Math.Cos((double) timer);
        float num3 = Ease.CubeOut(dist) * 32f;
        this.player.Sprite.SetAnimationFrame(frame);
        this.badeline.Sprite.SetAnimationFrame(frame + 7);
        this.player.Position = center - new Vector2(num1 * num3, (float) ((double) num2 * (double) dist * 8.0));
        this.badeline.Position = center + new Vector2(num1 * num3, (float) ((double) num2 * (double) dist * 8.0));
        timer -= Engine.DeltaTime * 2f;
        if ((double) timer <= 0.0)
          timer += 6.2831855f;
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
      if (this.dark)
        return;
      level.Add((Entity) new HeightDisplay(this.index));
    }
  }
}
