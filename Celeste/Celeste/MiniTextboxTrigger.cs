// Decompiled with JetBrains decompiler
// Type: Celeste.MiniTextboxTrigger
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  public class MiniTextboxTrigger : Trigger
  {
    private EntityID id;
    private string[] dialogOptions;
    private MiniTextboxTrigger.Modes mode;
    private bool triggered;
    private bool onlyOnce;
    private int deathCount;

    public MiniTextboxTrigger(EntityData data, Vector2 offset, EntityID id)
      : base(data, offset)
    {
      this.id = id;
      this.mode = data.Enum<MiniTextboxTrigger.Modes>(nameof (mode), MiniTextboxTrigger.Modes.OnPlayerEnter);
      this.dialogOptions = data.Attr("dialog_id", "").Split(',');
      this.onlyOnce = data.Bool("only_once", false);
      this.deathCount = data.Int("death_count", -1);
      if (this.mode != MiniTextboxTrigger.Modes.OnTheoEnter)
        return;
      this.Add((Component) new HoldableCollider((Action<Holdable>) (c => this.Trigger()), (Collider) null));
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (this.mode != MiniTextboxTrigger.Modes.OnLevelStart)
        return;
      this.Trigger();
    }

    public override void OnEnter(Player player)
    {
      if (this.mode != MiniTextboxTrigger.Modes.OnPlayerEnter)
        return;
      this.Trigger();
    }

    private void Trigger()
    {
      if (this.triggered || this.deathCount >= 0 && (this.Scene as Level).Session.DeathsInCurrentLevel != this.deathCount)
        return;
      this.triggered = true;
      this.Scene.Add((Entity) new MiniTextbox(Calc.Random.Choose<string>(this.dialogOptions)));
      if (!this.onlyOnce)
        return;
      (this.Scene as Level).Session.DoNotLoad.Add(this.id);
    }

    private enum Modes
    {
      OnPlayerEnter,
      OnLevelStart,
      OnTheoEnter,
    }
  }
}
