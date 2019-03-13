// Decompiled with JetBrains decompiler
// Type: Celeste.StrawberryPoints
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3F0C8D56-DA65-4356-B04B-572A65ED61D1
// Assembly location: M:\code\bin\Celeste\Celeste.exe

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
    private VertexLight light;
    private BloomPoint bloom;
    private int index;

    public StrawberryPoints(Vector2 position, bool ghostberry, int index)
      : base(position)
    {
      this.Add((Component) (this.sprite = GFX.SpriteBank.Create("strawberry")));
      this.Add((Component) (this.light = new VertexLight(Color.get_White(), 1f, 16, 24)));
      this.Add((Component) (this.bloom = new BloomPoint(1f, 12f)));
      this.Depth = -1000000;
      this.Tag = (int) Tags.Persistent | (int) Tags.TransitionUpdate;
      this.ghostberry = ghostberry;
      this.index = index;
    }

    public override void Added(Scene scene)
    {
      this.index = Math.Min(5, this.index);
      if (this.index >= 5)
        Achievements.Register(Achievement.ONEUP);
      this.sprite.Play("fade" + (object) this.index, false, false);
      this.sprite.OnFinish = (Action<string>) (a => this.RemoveSelf());
      base.Added(scene);
      foreach (Entity entity in this.Scene.Tracker.GetEntities<StrawberryPoints>())
      {
        if (entity != this && (double) Vector2.DistanceSquared(entity.Position, this.Position) <= 256.0)
          entity.RemoveSelf();
      }
    }

    public override void Update()
    {
      base.Update();
      Level scene = this.Scene as Level;
      Camera camera = scene.Camera;
      this.Y -= 8f * Engine.DeltaTime;
      this.X = Calc.Clamp(this.X, camera.Left + 8f, camera.Right - 8f);
      this.Y = Calc.Clamp(this.Y, camera.Top + 8f, camera.Bottom - 8f);
      this.light.Alpha = Calc.Approach(this.light.Alpha, 0.0f, Engine.DeltaTime * 4f);
      this.bloom.Alpha = this.light.Alpha;
      ParticleType type = this.ghostberry ? Strawberry.P_GhostGlow : Strawberry.P_Glow;
      if (this.Scene.OnInterval(0.05f))
      {
        if (Color.op_Equality(this.sprite.Color, type.Color2))
          this.sprite.Color = type.Color;
        else
          this.sprite.Color = type.Color2;
      }
      if (!this.Scene.OnInterval(0.06f) || this.sprite.CurrentAnimationFrame <= 11)
        return;
      scene.ParticlesFG.Emit(type, 1, Vector2.op_Addition(this.Position, Vector2.op_Multiply(Vector2.get_UnitY(), -2f)), new Vector2(8f, 4f));
    }
  }
}
