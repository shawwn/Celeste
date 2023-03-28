// Decompiled with JetBrains decompiler
// Type: Celeste.StrawberryPoints
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste
{
  [Tracked(false)]
  public class StrawberryPoints : Entity
  {
    private Sprite sprite;
    private bool ghostberry;
    private bool moonberry;
    private VertexLight light;
    private BloomPoint bloom;
    private int index;
    private DisplacementRenderer.Burst burst;

    public StrawberryPoints(Vector2 position, bool ghostberry, int index, bool moonberry)
      : base(position)
    {
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("strawberry")));
      this.Add((Component) (this.light = new VertexLight(Color.White, 1f, 16, 24)));
      this.Add((Component) (this.bloom = new BloomPoint(1f, 12f)));
      this.Depth = -2000100;
      this.Tag = (int) Tags.Persistent | (int) Tags.TransitionUpdate | (int) Tags.FrozenUpdate;
      this.ghostberry = ghostberry;
      this.moonberry = moonberry;
      this.index = index;
    }

    public override void Added(Scene scene)
    {
      this.index = Math.Min(5, this.index);
      if (this.index >= 5)
        Achievements.Register(Achievement.ONEUP);
      if (this.moonberry)
        this.sprite.Play("fade_wow");
      else
        this.sprite.Play("fade" + (object) this.index);
      this.sprite.OnFinish = (Action<string>) (a => this.RemoveSelf());
      base.Added(scene);
      foreach (Entity entity in this.Scene.Tracker.GetEntities<StrawberryPoints>())
      {
        if (entity != this && (double) Vector2.DistanceSquared(entity.Position, this.Position) <= 256.0)
          entity.RemoveSelf();
      }
      this.burst = (scene as Level).Displacement.AddBurst(this.Position, 0.3f, 16f, 24f, 0.3f);
    }

    public override void Update()
    {
      Level scene = this.Scene as Level;
      if (scene.Frozen)
      {
        if (this.burst == null)
          return;
        this.burst.AlphaFrom = this.burst.AlphaTo = 0.0f;
        this.burst.Percent = this.burst.Duration;
      }
      else
      {
        base.Update();
        Camera camera = scene.Camera;
        this.Y -= 8f * Engine.DeltaTime;
        this.X = Calc.Clamp(this.X, camera.Left + 8f, camera.Right - 8f);
        this.Y = Calc.Clamp(this.Y, camera.Top + 8f, camera.Bottom - 8f);
        this.light.Alpha = Calc.Approach(this.light.Alpha, 0.0f, Engine.DeltaTime * 4f);
        this.bloom.Alpha = this.light.Alpha;
        ParticleType type = this.ghostberry ? Strawberry.P_GhostGlow : Strawberry.P_Glow;
        if (this.moonberry && !this.ghostberry)
          type = Strawberry.P_MoonGlow;
        if (this.Scene.OnInterval(0.05f))
        {
          if (this.sprite.Color == type.Color2)
            this.sprite.Color = type.Color;
          else
            this.sprite.Color = type.Color2;
        }
        if (!this.Scene.OnInterval(0.06f) || this.sprite.CurrentAnimationFrame <= 11)
          return;
        scene.ParticlesFG.Emit(type, 1, this.Position + Vector2.UnitY * -2f, new Vector2(8f, 4f));
      }
    }
  }
}
