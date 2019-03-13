// Decompiled with JetBrains decompiler
// Type: Celeste.Hahaha
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class Hahaha : Entity
  {
    private List<Hahaha.Ha> has = new List<Hahaha.Ha>();
    private bool enabled;
    private string ifSet;
    private float timer;
    private int counter;
    private bool autoTriggerLaughSfx;
    private Vector2 autoTriggerLaughOrigin;

    public bool Enabled
    {
      get
      {
        return this.enabled;
      }
      set
      {
        if (!this.enabled & value)
        {
          this.timer = 0.0f;
          this.counter = 0;
        }
        this.enabled = value;
      }
    }

    public Hahaha(
      Vector2 position,
      string ifSet = "",
      bool triggerLaughSfx = false,
      Vector2? triggerLaughSfxOrigin = null)
    {
      this.Depth = -10001;
      this.Position = position;
      this.ifSet = ifSet;
      if (!triggerLaughSfx)
        return;
      this.autoTriggerLaughSfx = triggerLaughSfx;
      this.autoTriggerLaughOrigin = triggerLaughSfxOrigin.Value;
    }

    public Hahaha(EntityData data, Vector2 offset)
      : this(data.Position + offset, data.Attr("ifset", ""), data.Bool("triggerLaughSfx", false), new Vector2?(data.Nodes.Length != 0 ? offset + data.Nodes[0] : Vector2.Zero))
    {
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if (string.IsNullOrEmpty(this.ifSet) || (this.Scene as Level).Session.GetFlag(this.ifSet))
        return;
      this.Enabled = false;
    }

    public override void Update()
    {
      if (this.Enabled)
      {
        this.timer -= Engine.DeltaTime;
        if ((double) this.timer <= 0.0)
        {
          this.has.Add(new Hahaha.Ha());
          ++this.counter;
          if (this.counter >= 3)
          {
            this.counter = 0;
            this.timer = 1.5f;
          }
          else
            this.timer = 0.6f;
        }
        if (this.autoTriggerLaughSfx && this.Scene.OnInterval(0.4f))
          Audio.Play("event:/char/granny/laugh_oneha", this.autoTriggerLaughOrigin);
      }
      for (int index = this.has.Count - 1; index >= 0; --index)
      {
        if ((double) this.has[index].Percent > 1.0)
        {
          this.has.RemoveAt(index);
        }
        else
        {
          this.has[index].Sprite.Update();
          this.has[index].Percent += Engine.DeltaTime / this.has[index].Duration;
        }
      }
      if (!this.Enabled && !string.IsNullOrEmpty(this.ifSet) && (this.Scene as Level).Session.GetFlag(this.ifSet))
        this.Enabled = true;
      base.Update();
    }

    public override void Render()
    {
      foreach (Hahaha.Ha ha in this.has)
      {
        ha.Sprite.Position = this.Position + new Vector2(ha.Percent * 60f, (float) (-Math.Sin((double) ha.Percent * 13.0) * 4.0 - 10.0 + (double) ha.Percent * -16.0));
        ha.Sprite.Render();
      }
    }

    private class Ha
    {
      public Sprite Sprite;
      public float Percent;
      public float Duration;

      public Ha()
      {
        this.Sprite = new Sprite(GFX.Game, "characters/oldlady/");
        this.Sprite.Add("normal", "ha", 0.15f, 0, 1, 0, 1, 0, 1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
        this.Sprite.Play("normal", false, false);
        this.Sprite.JustifyOrigin(0.5f, 0.5f);
        this.Duration = (float) this.Sprite.CurrentAnimationTotalFrames * 0.15f;
      }
    }
  }
}

