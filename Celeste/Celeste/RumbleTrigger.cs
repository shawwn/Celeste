// Decompiled with JetBrains decompiler
// Type: Celeste.RumbleTrigger
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
  public class RumbleTrigger : Trigger
  {
    private bool manualTrigger;
    private bool started;
    private bool persistent;
    private EntityID id;
    private float rumble;
    private float left;
    private float right;
    private List<Decal> decals = new List<Decal>();
    private List<CrumbleWallOnRumble> crumbles = new List<CrumbleWallOnRumble>();

    public RumbleTrigger(EntityData data, Vector2 offset, EntityID id)
      : base(data, offset)
    {
      this.manualTrigger = data.Bool(nameof (manualTrigger));
      this.persistent = data.Bool(nameof (persistent));
      this.id = id;
      Vector2[] vector2Array = data.NodesOffset(offset);
      if (vector2Array.Length < 2)
        return;
      this.left = Math.Min(vector2Array[0].X, vector2Array[1].X);
      this.right = Math.Max(vector2Array[0].X, vector2Array[1].X);
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      Level scene1 = this.Scene as Level;
      bool flag = false;
      if (this.persistent && scene1.Session.GetFlag(this.id.ToString()))
        flag = true;
      foreach (CrumbleWallOnRumble entity in scene.Tracker.GetEntities<CrumbleWallOnRumble>())
      {
        if ((double) entity.X >= (double) this.left && (double) entity.X <= (double) this.right)
        {
          if (flag)
            entity.RemoveSelf();
          else
            this.crumbles.Add(entity);
        }
      }
      if (!flag)
      {
        foreach (Decal decal in scene.Entities.FindAll<Decal>())
        {
          if (decal.IsCrack && (double) decal.X >= (double) this.left && (double) decal.X <= (double) this.right)
          {
            decal.Visible = false;
            this.decals.Add(decal);
          }
        }
        this.crumbles.Sort((Comparison<CrumbleWallOnRumble>) ((a, b) => !Calc.Random.Chance(0.5f) ? 1 : -1));
      }
      if (!flag)
        return;
      this.RemoveSelf();
    }

    public override void OnEnter(Player player)
    {
      base.OnEnter(player);
      if (this.manualTrigger)
        return;
      this.Invoke();
    }

    private void Invoke(float delay = 0.0f)
    {
      if (this.started)
        return;
      this.started = true;
      if (this.persistent)
        (this.Scene as Level).Session.SetFlag(this.id.ToString());
      this.Add((Component) new Coroutine(this.RumbleRoutine(delay)));
      this.Add((Component) new DisplacementRenderHook(new Action(this.RenderDisplacement)));
    }

    private IEnumerator RumbleRoutine(float delay)
    {
      RumbleTrigger rumbleTrigger = this;
      yield return (object) delay;
      Scene scene = rumbleTrigger.Scene;
      rumbleTrigger.rumble = 1f;
      Audio.Play("event:/new_content/game/10_farewell/quake_onset", rumbleTrigger.Position);
      Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
      foreach (Entity decal in rumbleTrigger.decals)
        decal.Visible = true;
      foreach (CrumbleWallOnRumble crumble in rumbleTrigger.crumbles)
      {
        crumble.Break();
        yield return (object) 0.05f;
      }
    }

    public override void Update()
    {
      base.Update();
      this.rumble = Calc.Approach(this.rumble, 0.0f, Engine.DeltaTime * 0.7f);
    }

    private void RenderDisplacement()
    {
      if ((double) this.rumble <= 0.0 || Settings.Instance.ScreenShake == ScreenshakeAmount.Off)
        return;
      Camera camera = (this.Scene as Level).Camera;
      int num1 = (int) ((double) camera.Left / 8.0) - 1;
      int num2 = (int) ((double) camera.Right / 8.0) + 1;
      for (int index = num1; index <= num2; ++index)
      {
        Color color = new Color(0.5f, 0.5f + (float) Math.Sin((double) this.Scene.TimeActive * 60.0 + (double) index * 0.4000000059604645) * 0.06f * this.rumble, 0.0f, 1f);
        Draw.Rect((float) (index * 8), camera.Top - 2f, 8f, 184f, color);
      }
    }

    public static void ManuallyTrigger(float x, float delay)
    {
      foreach (RumbleTrigger rumbleTrigger in Engine.Scene.Entities.FindAll<RumbleTrigger>())
      {
        if (rumbleTrigger.manualTrigger && (double) x >= (double) rumbleTrigger.left && (double) x <= (double) rumbleTrigger.right)
          rumbleTrigger.Invoke(delay);
      }
    }
  }
}
