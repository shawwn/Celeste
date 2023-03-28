// Decompiled with JetBrains decompiler
// Type: Celeste.SummitCheckpoint
// Assembly: Celeste, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4A26F9DE-D670-4C87-A2F4-7E66D2D85163
// Assembly location: /Users/shawn/Library/Application Support/Steam/steamapps/common/Celeste/Celeste.app/Contents/Resources/Celeste.exe

using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste
{
  public class SummitCheckpoint : Entity
  {
    private const string Flag = "summit_checkpoint_";
    public bool Activated;
    public readonly int Number;
    private string numberString;
    private Vector2 respawn;
    private MTexture baseEmpty;
    private MTexture baseToggle;
    private MTexture baseActive;
    private List<MTexture> numbersEmpty;
    private List<MTexture> numbersActive;

    public SummitCheckpoint(EntityData data, Vector2 offset)
      : base(data.Position + offset)
    {
      this.Number = data.Int("number");
      this.numberString = this.Number.ToString("D2");
      this.baseEmpty = GFX.Game["scenery/summitcheckpoints/base00"];
      this.baseToggle = GFX.Game["scenery/summitcheckpoints/base01"];
      this.baseActive = GFX.Game["scenery/summitcheckpoints/base02"];
      this.numbersEmpty = GFX.Game.GetAtlasSubtextures("scenery/summitcheckpoints/numberbg");
      this.numbersActive = GFX.Game.GetAtlasSubtextures("scenery/summitcheckpoints/number");
      this.Collider = (Collider) new Hitbox(32f, 32f, -16f, -8f);
      this.Depth = 8999;
    }

    public override void Added(Scene scene)
    {
      base.Added(scene);
      if ((scene as Level).Session.GetFlag("summit_checkpoint_" + (object) this.Number))
        this.Activated = true;
      this.respawn = this.SceneAs<Level>().GetSpawnPoint(this.Position);
    }

    public override void Awake(Scene scene)
    {
      base.Awake(scene);
      if (this.Activated || !this.CollideCheck<Player>())
        return;
      this.Activated = true;
      Level scene1 = this.Scene as Level;
      scene1.Session.SetFlag("summit_checkpoint_" + (object) this.Number);
      scene1.Session.RespawnPoint = new Vector2?(this.respawn);
    }

    public override void Update()
    {
      if (this.Activated)
        return;
      Player player = this.CollideFirst<Player>();
      if (player == null || !player.OnGround() || (double) player.Speed.Y < 0.0)
        return;
      Level scene = this.Scene as Level;
      this.Activated = true;
      scene.Session.SetFlag("summit_checkpoint_" + (object) this.Number);
      scene.Session.RespawnPoint = new Vector2?(this.respawn);
      scene.Session.UpdateLevelStartDashes();
      scene.Session.HitCheckpoint = true;
      scene.Displacement.AddBurst(this.Position, 0.5f, 4f, 24f, 0.5f);
      scene.Add((Entity) new SummitCheckpoint.ConfettiRenderer(this.Position));
      Audio.Play("event:/game/07_summit/checkpoint_confetti", this.Position);
    }

    public override void Render()
    {
      List<MTexture> mtextureList = this.Activated ? this.numbersActive : this.numbersEmpty;
      MTexture mtexture = this.baseActive;
      if (!this.Activated)
        mtexture = this.Scene.BetweenInterval(0.25f) ? this.baseEmpty : this.baseToggle;
      mtexture.Draw(this.Position - new Vector2((float) (mtexture.Width / 2 + 1), (float) (mtexture.Height / 2)));
      mtextureList[(int) this.numberString[0] - 48].DrawJustified(this.Position + new Vector2(-1f, 1f), new Vector2(1f, 0.0f));
      mtextureList[(int) this.numberString[1] - 48].DrawJustified(this.Position + new Vector2(0.0f, 1f), new Vector2(0.0f, 0.0f));
    }

    public class ConfettiRenderer : Entity
    {
      private static readonly Color[] confettiColors = new Color[3]
      {
        Calc.HexToColor("fe2074"),
        Calc.HexToColor("205efe"),
        Calc.HexToColor("cefe20")
      };
      private SummitCheckpoint.ConfettiRenderer.Particle[] particles = new SummitCheckpoint.ConfettiRenderer.Particle[30];

      public ConfettiRenderer(Vector2 position)
        : base(position)
      {
        this.Depth = -10010;
        for (int index = 0; index < this.particles.Length; ++index)
        {
          this.particles[index].Position = this.Position + new Vector2((float) Calc.Random.Range(-3, 3), (float) Calc.Random.Range(-3, 3));
          this.particles[index].Color = Calc.Random.Choose<Color>(SummitCheckpoint.ConfettiRenderer.confettiColors);
          this.particles[index].Timer = Calc.Random.NextFloat();
          this.particles[index].Duration = (float) Calc.Random.Range(2, 4);
          this.particles[index].Alpha = 1f;
          float angleRadians = Calc.Random.Range(-0.5f, 0.5f) - 1.5707964f;
          int length = Calc.Random.Range(140, 220);
          this.particles[index].Speed = Calc.AngleToVector(angleRadians, (float) length);
        }
      }

      public override void Update()
      {
        for (int index = 0; index < this.particles.Length; ++index)
        {
          this.particles[index].Position += this.particles[index].Speed * Engine.DeltaTime;
          this.particles[index].Speed.X = Calc.Approach(this.particles[index].Speed.X, 0.0f, 80f * Engine.DeltaTime);
          this.particles[index].Speed.Y = Calc.Approach(this.particles[index].Speed.Y, 20f, 500f * Engine.DeltaTime);
          this.particles[index].Timer += Engine.DeltaTime;
          this.particles[index].Percent += Engine.DeltaTime / this.particles[index].Duration;
          this.particles[index].Alpha = Calc.ClampedMap(this.particles[index].Percent, 0.9f, 1f, 1f, 0.0f);
          if ((double) this.particles[index].Speed.Y > 0.0)
            this.particles[index].Approach = Calc.Approach(this.particles[index].Approach, 5f, Engine.DeltaTime * 16f);
        }
      }

      public override void Render()
      {
        for (int index = 0; index < this.particles.Length; ++index)
        {
          Vector2 position = this.particles[index].Position;
          float rotation;
          if ((double) this.particles[index].Speed.Y < 0.0)
          {
            rotation = this.particles[index].Speed.Angle();
          }
          else
          {
            rotation = (float) Math.Sin((double) this.particles[index].Timer * 4.0) * 1f;
            position += Calc.AngleToVector(1.5707964f + rotation, this.particles[index].Approach);
          }
          GFX.Game["particles/confetti"].DrawCentered(position + Vector2.UnitY, Color.Black * (this.particles[index].Alpha * 0.5f), 1f, rotation);
          GFX.Game["particles/confetti"].DrawCentered(position, this.particles[index].Color * this.particles[index].Alpha, 1f, rotation);
        }
      }

      private struct Particle
      {
        public Vector2 Position;
        public Color Color;
        public Vector2 Speed;
        public float Timer;
        public float Percent;
        public float Duration;
        public float Alpha;
        public float Approach;
      }
    }
  }
}
