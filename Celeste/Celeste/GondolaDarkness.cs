// Decompiled with JetBrains decompiler
// Type: Celeste.GondolaDarkness
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste
{
  public class GondolaDarkness : Entity
  {
    private float anxiety = 0.0f;
    private float anxietyStutter = 0.0f;
    private Sprite sprite;
    private Sprite hands;
    private GondolaDarkness.Blackness blackness;
    private WindSnowFG windSnowFG;

    public GondolaDarkness()
    {
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("gondolaDarkness")));
      this.sprite.Play("appear", false, false);
      this.Add((Component) (this.hands = GFX.SpriteBank.Create("gondolaHands")));
      this.hands.Visible = false;
      this.Visible = false;
      this.Depth = -999900;
    }

    public IEnumerator Appear(WindSnowFG windSnowFG = null)
    {
      this.windSnowFG = windSnowFG;
      this.Visible = true;
      this.Scene.Add((Entity) (this.blackness = new GondolaDarkness.Blackness()));
      for (float t = 0.0f; (double) t < 1.0; t += Engine.DeltaTime / 2f)
      {
        yield return (object) null;
        this.blackness.Fade = t;
        this.anxiety = t;
        if (windSnowFG != null)
          windSnowFG.Alpha = 1f - t;
      }
      yield return (object) null;
    }

    public IEnumerator Expand()
    {
      this.hands.Visible = true;
      this.hands.Play("appear", false, false);
      yield return (object) 1f;
    }

    public IEnumerator Reach(Gondola gondola)
    {
      this.hands.Play("grab", false, false);
      yield return (object) 0.4f;
      this.hands.Play("pull", false, false);
      gondola.PullSides();
    }

    public override void Update()
    {
      base.Update();
      if (this.Scene.OnInterval(0.05f))
        this.anxietyStutter = Calc.Random.NextFloat(0.1f);
      Distort.AnxietyOrigin = new Vector2(0.5f, 0.5f);
      Distort.Anxiety = (float) ((double) this.anxiety * 0.200000002980232 + (double) this.anxietyStutter * (double) this.anxiety);
    }

    public override void Render()
    {
      this.Position = (this.Scene as Level).Camera.Position + (this.Scene as Level).ZoomFocusPoint;
      base.Render();
    }

    public override void Removed(Scene scene)
    {
      this.anxiety = 0.0f;
      Distort.Anxiety = 0.0f;
      if (this.blackness != null)
        this.blackness.RemoveSelf();
      if (this.windSnowFG != null)
        this.windSnowFG.Alpha = 1f;
      base.Removed(scene);
    }

    private class Blackness : Entity
    {
      public float Fade;

      public Blackness()
      {
        this.Depth = 9001;
      }

      public override void Render()
      {
        base.Render();
        Camera camera = (this.Scene as Level).Camera;
        Draw.Rect(camera.Left - 1f, camera.Top - 1f, 322f, 182f, Color.Black * this.Fade);
      }
    }
  }
}

